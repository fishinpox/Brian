using FluentAssertions;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Tests.Common;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Identity.Tests.Features.Auth;

public class RefreshTokenCommandHandlerTests
{
    private static ITokenService MakeTokenService(string accessToken = "new-access-token")
    {
        var tokenService = Substitute.For<ITokenService>();
        tokenService.HashRefreshToken(Arg.Any<string>()).Returns(ci => "hash:" + ci.Arg<string>());
        tokenService.GenerateRefreshToken().Returns("next-raw-refresh-token");
        tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<Profile>()).Returns(accessToken);
        return tokenService;
    }

    private static Profile SeedProfile(TestIdentityDbContext db, Guid accountId)
    {
        var profile = new Profile { AccountId = accountId, Username = "tester", DisplayName = "Tester" };
        var role = new ProfileRole { ProfileId = profile.Id, Profile = profile, Role = UserRole.Fan, GrantedAt = DateTimeOffset.UtcNow };
        profile.Roles.Add(role);
        db.Profiles.Add(profile);
        db.SaveChanges();
        return profile;
    }

    [Fact]
    public async Task Handle_ValidToken_RotatesSessionAndReturnsNewTokens()
    {
        using var db = TestIdentityDbContext.Create();
        var tokenService = MakeTokenService();
        var accountId = Guid.NewGuid();
        var profile = SeedProfile(db, accountId);

        var oldSession = Session.Issue(accountId, profile.Id, "hash:old-raw-token", DateTimeOffset.UtcNow.AddDays(10), null, null);
        db.Sessions.Add(oldSession);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new RefreshTokenCommandHandler(db, tokenService);
        var result = await handler.Handle(new RefreshTokenCommand("old-raw-token"), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.Token.Should().Be("new-access-token");
        result.Value!.RefreshToken.Should().Be("next-raw-refresh-token");
        result.Value!.AccountId.Should().Be(accountId);
        result.Value!.ProfileId.Should().Be(profile.Id);

        var reloadedOld = await db.Sessions.FindAsync(oldSession.Id);
        reloadedOld!.IsRevoked.Should().BeTrue();
        reloadedOld.ReplacedBySessionId.Should().NotBeNull();

        var newSession = await db.Sessions.FindAsync(reloadedOld.ReplacedBySessionId!.Value);
        newSession.Should().NotBeNull();
        newSession!.IsRevoked.Should().BeFalse();
        newSession.TokenHash.Should().Be("hash:next-raw-refresh-token");
    }

    [Fact]
    public async Task Handle_RevokedTokenReplayed_FailsAndRevokesAllActiveSessionsForProfile()
    {
        using var db = TestIdentityDbContext.Create();
        var tokenService = MakeTokenService();
        var accountId = Guid.NewGuid();
        var profile = SeedProfile(db, accountId);

        var revokedSession = Session.Issue(accountId, profile.Id, "hash:stolen-raw-token", DateTimeOffset.UtcNow.AddDays(10), null, null);
        revokedSession.Revoke();
        var otherActiveSession = Session.Issue(accountId, profile.Id, "hash:other-active-raw-token", DateTimeOffset.UtcNow.AddDays(10), null, null);
        db.Sessions.AddRange(revokedSession, otherActiveSession);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new RefreshTokenCommandHandler(db, tokenService);
        var result = await handler.Handle(new RefreshTokenCommand("stolen-raw-token"), CancellationToken.None);

        result.Succeeded.Should().BeFalse();

        var reloadedOther = await db.Sessions.FindAsync(otherActiveSession.Id);
        reloadedOther!.IsRevoked.Should().BeTrue("a replayed revoked token should revoke every other active session for the account+profile");
    }

    [Fact]
    public async Task Handle_ExpiredToken_FailsWithoutIssuingNewSession()
    {
        using var db = TestIdentityDbContext.Create();
        var tokenService = MakeTokenService();
        var accountId = Guid.NewGuid();
        var profile = SeedProfile(db, accountId);

        var expiredSession = Session.Issue(accountId, profile.Id, "hash:expired-raw-token", DateTimeOffset.UtcNow.AddDays(-1), null, null);
        db.Sessions.Add(expiredSession);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new RefreshTokenCommandHandler(db, tokenService);
        var result = await handler.Handle(new RefreshTokenCommand("expired-raw-token"), CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        (await db.Sessions.CountAsync(CancellationToken.None)).Should().Be(1, "no new session should be created for an expired refresh token");
    }

    [Fact]
    public async Task Handle_UnknownToken_Fails()
    {
        using var db = TestIdentityDbContext.Create();
        var tokenService = MakeTokenService();

        var handler = new RefreshTokenCommandHandler(db, tokenService);
        var result = await handler.Handle(new RefreshTokenCommand("never-issued-raw-token"), CancellationToken.None);

        result.Succeeded.Should().BeFalse();
    }
}

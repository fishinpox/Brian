using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Tests.Common;

/// <summary>
/// Minimal EF Core InMemory-backed context implementing IIdentityDbContext, so Application-layer
/// handlers can be unit tested against real LINQ/async query behavior without a SQL Server dependency.
/// </summary>
public class TestIdentityDbContext(DbContextOptions<TestIdentityDbContext> options)
    : DbContext(options), IIdentityDbContext
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileRole> ProfileRoles => Set<ProfileRole>();
    public DbSet<OAuthIdentity> OAuthIdentities => Set<OAuthIdentity>();
    public DbSet<Session> Sessions => Set<Session>();

    public static TestIdentityDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestIdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestIdentityDbContext(options);
    }
}

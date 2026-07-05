using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Common.Interfaces;

public interface IIdentityDbContext
{
    DbSet<Profile> Profiles { get; }
    DbSet<ProfileRole> ProfileRoles { get; }
    DbSet<OAuthIdentity> OAuthIdentities { get; }
    DbSet<Session> Sessions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

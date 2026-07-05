using Holodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Holodex.Application.Common.Interfaces;

public interface IHolodexDbContext
{
    DbSet<ExternalCredential> ExternalCredentials { get; }
    DbSet<FollowedChannel> FollowedChannels { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

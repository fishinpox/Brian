using Microsoft.EntityFrameworkCore;
using Twitch.Domain.Entities;

namespace Twitch.Application.Common.Interfaces;

public interface ITwitchDbContext
{
    DbSet<ExternalCredential> ExternalCredentials { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

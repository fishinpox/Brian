using Microsoft.EntityFrameworkCore;
using YouTube.Domain.Entities;

namespace YouTube.Application.Common.Interfaces;

public interface IYouTubeDbContext
{
    DbSet<ExternalCredential> ExternalCredentials { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Creator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Creator.Application.Common.Interfaces;

public interface ICreatorDbContext
{
    DbSet<Site> Sites { get; }
    DbSet<SiteAccessGrant> SiteAccessGrants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

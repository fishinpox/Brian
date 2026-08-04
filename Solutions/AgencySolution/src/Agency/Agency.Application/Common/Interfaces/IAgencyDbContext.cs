using Agency.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agency.Application.Common.Interfaces;

public interface IAgencyDbContext
{
    DbSet<Contact> Contacts { get; }
    DbSet<Company> Companies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

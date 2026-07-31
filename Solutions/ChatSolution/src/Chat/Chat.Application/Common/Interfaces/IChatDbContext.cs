using Chat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Common.Interfaces;

public interface IChatDbContext
{
    DbSet<ChatAccountLink> ChatAccountLinks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

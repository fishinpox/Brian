using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.Common.Interfaces;

public interface ICalendarDbContext
{
    DbSet<PersonalEvent> PersonalEvents { get; }
    DbSet<Reminder> Reminders { get; }
    DbSet<StreamEvent> StreamEvents { get; }
    DbSet<CalendarBackground> CalendarBackgrounds { get; }
    DbSet<Folder> Folders { get; }
    DbSet<Subfolder> Subfolders { get; }
    DbSet<CalendarSettings> CalendarSettings { get; }
    DbSet<Memo> Memos { get; }
    DbSet<Note> Notes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

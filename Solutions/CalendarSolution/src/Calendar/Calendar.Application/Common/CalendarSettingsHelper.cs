using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.Common;

public static class CalendarSettingsHelper
{
    public static async Task<CalendarSettings> GetOrCreateAsync(ICalendarDbContext db, Guid profileId, CancellationToken cancellationToken)
    {
        var settings = await db.CalendarSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId, cancellationToken);
        if (settings is not null)
            return settings;

        settings = CalendarSettings.CreateDefault(profileId);
        db.CalendarSettings.Add(settings);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Page load fires several requests concurrently (folder tree, settings, calendar view),
            // each of which lazily creates a profile's first CalendarSettings row - a race that trips
            // the unique index on ProfileId when two lose the race at once. Whichever request actually
            // won just needs to be read back here.
            return await db.CalendarSettings.FirstAsync(s => s.ProfileId == profileId, cancellationToken);
        }

        return settings;
    }
}

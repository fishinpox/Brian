using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using Calendar.Domain.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Holodex;

namespace Calendar.Infrastructure.Consumers;

public class IntegrationVideosSyncedConsumer(ICalendarDbContext db) : IConsumer<IntegrationVideosSyncedEvent>
{
    public async Task Consume(ConsumeContext<IntegrationVideosSyncedEvent> context)
    {
        var platform = Enum.TryParse<StreamPlatform>(context.Message.Provider, out var parsed)
            ? parsed
            : StreamPlatform.Other;

        var existingStreamEvents = await db.StreamEvents
            .Where(e => e.Platform == platform)
            .ToListAsync(context.CancellationToken);

        foreach (var video in context.Message.Videos)
        {
            var existing = existingStreamEvents.FirstOrDefault(e => e.PlatformVideoId == video.ExternalVideoId);

            if (existing is not null)
            {
                existing.Title = video.Title;
                existing.ThumbnailUrl = video.ThumbnailUrl;
                existing.Status = MapStatus(video.Status);
                if (video.ActualStart is not null)
                    existing.ActualStart = video.ActualStart;
                if (video.ActualEnd is not null)
                    existing.ActualEnd = video.ActualEnd;
            }
            else
            {
                var scheduledStart = video.ScheduledStart ?? video.ActualStart;
                if (scheduledStart is null)
                    continue;

                var newEvent = StreamEvent.Create(
                    null,
                    platform,
                    video.ExternalVideoId,
                    video.Title,
                    video.ThumbnailUrl,
                    null,
                    scheduledStart.Value,
                    isExternallySourced: true);

                newEvent.Status = MapStatus(video.Status);
                db.StreamEvents.Add(newEvent);
                existingStreamEvents.Add(newEvent);
            }
        }

        await db.SaveChangesAsync(context.CancellationToken);
    }

    private static EventStatus MapStatus(string status) => status switch
    {
        "live" => EventStatus.Live,
        "upcoming" => EventStatus.Upcoming,
        "past" => EventStatus.Past,
        _ => EventStatus.Upcoming
    };
}

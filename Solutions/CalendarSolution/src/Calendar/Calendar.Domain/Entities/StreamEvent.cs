using Calendar.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class StreamEvent : BaseAuditableEntity
{
    public Guid? VTuberProfileId { get; private set; }
    public StreamPlatform Platform { get; private set; }
    public string? PlatformVideoId { get; private set; }
    public string Title { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset ScheduledStart { get; set; }
    public DateTimeOffset? ActualStart { get; set; }
    public DateTimeOffset? ActualEnd { get; set; }
    public EventStatus Status { get; set; }
    public int? PeakViewers { get; set; }
    public string? VodUrl { get; set; }
    public bool IsExternallySourced { get; private set; }

    private StreamEvent() { }

    public static StreamEvent Create(
        Guid? vtuberProfileId,
        StreamPlatform platform,
        string? platformVideoId,
        string title,
        string? thumbnailUrl,
        string? description,
        DateTimeOffset scheduledStart,
        bool isExternallySourced)
    {
        return new StreamEvent
        {
            VTuberProfileId = vtuberProfileId,
            Platform = platform,
            PlatformVideoId = platformVideoId,
            Title = title,
            ThumbnailUrl = thumbnailUrl,
            Description = description,
            ScheduledStart = scheduledStart,
            Status = EventStatus.Upcoming,
            IsExternallySourced = isExternallySourced
        };
    }
}

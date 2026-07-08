using Calendar.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class CalendarBackground : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public byte[] ImageData { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public CalendarBackgroundStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    private CalendarBackground() { }

    public static CalendarBackground Submit(
        Guid profileId,
        byte[] imageData,
        string contentType,
        string fileName)
    {
        return new CalendarBackground
        {
            ProfileId = profileId,
            ImageData = imageData,
            ContentType = contentType,
            FileName = fileName,
            SizeBytes = imageData.LongLength,
            Status = CalendarBackgroundStatus.Pending,
            RejectionReason = null
        };
    }

    public void Resubmit(byte[] imageData, string contentType, string fileName)
    {
        ImageData = imageData;
        ContentType = contentType;
        FileName = fileName;
        SizeBytes = imageData.LongLength;
        Status = CalendarBackgroundStatus.Pending;
        RejectionReason = null;
    }

    public void Approve() => Status = CalendarBackgroundStatus.Approved;

    public void Reject(string reason)
    {
        Status = CalendarBackgroundStatus.Rejected;
        RejectionReason = reason;
    }
}

using MassTransit;
using Moderation.Domain.Policies;
using Shared.Contracts.Events.Calendar;
using Shared.Contracts.Events.Moderation;

namespace Moderation.API.Consumers;

public class CalendarBackgroundSubmittedConsumer(
    IPublishEndpoint publishEndpoint,
    IConfiguration configuration,
    ILogger<CalendarBackgroundSubmittedConsumer> logger)
    : IConsumer<CalendarBackgroundSubmittedEvent>
{
    public async Task Consume(ConsumeContext<CalendarBackgroundSubmittedEvent> context)
    {
        var ev = context.Message;

        // TODO: plug in real CSAM/malware scanning here (e.g. Microsoft PhotoDNA, Thorn Safer,
        // or NCMEC hash-matching). This service currently only checks type/size/signature,
        // and even that is bypassed by default via Moderation:AutoApprove below so the upload
        // pipeline can be tested end-to-end. Flip that setting to false once real scanning
        // is wired in.
        var autoApprove = configuration.GetValue("Moderation:AutoApprove", true);

        bool isValid;
        string? reason;

        if (autoApprove)
        {
            isValid = true;
            reason = null;
            logger.LogInformation(
                "Auto-approved calendar background {SubmissionId} for profile {ProfileId} (Moderation:AutoApprove is on)",
                ev.SubmissionId, ev.ProfileId);
        }
        else
        {
            isValid = ImageModerationPolicy.TryValidate(ev.ImageData, ev.ContentType, out reason);
        }

        if (isValid)
        {
            await publishEndpoint.Publish(
                new CalendarBackgroundApprovedEvent(ev.SubmissionId, ev.ProfileId, DateTimeOffset.UtcNow),
                context.CancellationToken);
        }
        else
        {
            logger.LogInformation(
                "Rejected calendar background {SubmissionId} for profile {ProfileId}: {Reason}",
                ev.SubmissionId, ev.ProfileId, reason);

            await publishEndpoint.Publish(
                new CalendarBackgroundRejectedEvent(ev.SubmissionId, ev.ProfileId, reason!, DateTimeOffset.UtcNow),
                context.CancellationToken);
        }
    }
}

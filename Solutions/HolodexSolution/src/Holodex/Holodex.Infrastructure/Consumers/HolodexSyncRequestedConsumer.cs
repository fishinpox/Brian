using Holodex.Application.Common.Events;
using Holodex.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.Holodex;

namespace Holodex.Infrastructure.Consumers;

public class HolodexSyncRequestedConsumer(
    IHolodexDbContext db,
    IHolodexApiClient holodexClient,
    IPublishEndpoint publishEndpoint,
    ILogger<HolodexSyncRequestedConsumer> logger)
    : IConsumer<HolodexSyncRequestedEvent>
{
    public async Task Consume(ConsumeContext<HolodexSyncRequestedEvent> context)
    {
        var profileId = context.Message.ProfileId;

        var credential = await db.ExternalCredentials
            .FirstOrDefaultAsync(c => c.ProfileId == profileId, context.CancellationToken);

        if (credential is null)
        {
            logger.LogInformation("No Holodex credential linked for profile {ProfileId}, skipping sync", profileId);
            return;
        }

        var channelIds = await db.FollowedChannels
            .Where(c => c.ProfileId == profileId)
            .Select(c => c.HolodexChannelId)
            .ToListAsync(context.CancellationToken);

        if (channelIds.Count == 0)
        {
            logger.LogInformation("No followed channels for profile {ProfileId}, skipping sync", profileId);
            return;
        }

        try
        {
            var apiKey = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credential.EncryptedValue));

            var videos = await holodexClient.GetLiveAndUpcomingForChannelsAsync(apiKey, string.Join(",", channelIds));

            var now = DateTimeOffset.UtcNow;
            var cutoff = now.AddDays(7);

            var filtered = videos
                .Where(v => v.Status == "live" || IsWithinCutoff(v, cutoff))
                .Select(v => new ExternalVideoDto(
                    v.Id,
                    v.Title,
                    v.Thumbnail,
                    v.Status,
                    ParseDate(v.Start_scheduled),
                    ParseDate(v.Start_actual),
                    ParseDate(v.End_actual)))
                .ToList();

            await publishEndpoint.Publish(
                new IntegrationVideosSyncedEvent(profileId, "Holodex", filtered, DateTimeOffset.UtcNow),
                context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to sync Holodex videos for profile {ProfileId}", profileId);
        }
    }

    private static bool IsWithinCutoff(HolodexVideoDto video, DateTimeOffset cutoff)
    {
        var start = ParseDate(video.Start_scheduled) ?? ParseDate(video.Start_actual);
        return start is not null && start <= cutoff;
    }

    private static DateTimeOffset? ParseDate(string? value) =>
        value is not null && DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
}

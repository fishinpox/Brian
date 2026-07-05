using Refit;

namespace Holodex.Application.Common.Interfaces;

public record HolodexVideoDto(
    string Id,
    string Title,
    string? Thumbnail,
    string Channel_id,
    string Status,
    string? Start_scheduled,
    string? Start_actual,
    string? End_actual);

public record HolodexChannelDto(
    string Id,
    string Name,
    string? English_name,
    string? Photo);

public interface IHolodexApiClient
{
    [Get("/users/live")]
    Task<List<HolodexVideoDto>> GetLiveAndUpcomingForChannelsAsync(
        [Header("X-APIKEY")] string apiKey,
        [Query] string channels);

    [Get("/channels")]
    Task<List<HolodexChannelDto>> SearchChannelsAsync(
        [Header("X-APIKEY")] string apiKey,
        [Query] string search,
        [Query] string type = "vtuber",
        [Query] int limit = 20);
}

using Microsoft.AspNetCore.SignalR.Client;
using NativeApp.Core.Models;

namespace NativeApp.Core;

public class NotificationClient
{
    public HubConnection Connection { get; }

    public NotificationClient(string gatewayBaseUrl, AuthSession authSession)
    {
        Connection = new HubConnectionBuilder()
            .WithUrl($"{gatewayBaseUrl}/hubs/notifications", opts =>
            {
                // A callback, not a captured string - re-invoked on every (re)connect, so a token
                // refreshed mid-session by AuthSession is picked up rather than going stale.
                opts.AccessTokenProvider = () => authSession.GetValidAccessTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();
    }

    public IDisposable OnWallpaperOwned(Action<WallpaperOwnedNotification> handler) =>
        Connection.On<WallpaperOwnedNotification>("wallpaper-owned", handler);

    public Task StartAsync(CancellationToken ct = default) => Connection.StartAsync(ct);

    public Task StopAsync(CancellationToken ct = default) => Connection.StopAsync(ct);
}

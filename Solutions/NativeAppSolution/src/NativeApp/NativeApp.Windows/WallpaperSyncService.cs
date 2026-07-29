using System.IO;
using NativeApp.Core;

namespace NativeApp.Windows;

/// <summary>
/// Downloads and applies every currently-pending (unapplied) owned wallpaper. Used both on app
/// startup (covers "the app wasn't running at purchase time") and from the SignalR
/// "wallpaper-owned" push (covers "the app was running") - same code path either way, so a
/// partial failure (apply succeeds, mark-applied doesn't land) just gets retried on the next call
/// since GetPendingOwnershipsAsync will return the same item again.
/// </summary>
public class WallpaperSyncService(ApiClient apiClient, AuthSession authSession)
{
    private static readonly string WallpapersDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Brian", "NativeApp", "Wallpapers");

    public async Task<int> ApplyPendingAsync(CancellationToken ct = default)
    {
        var accessToken = await authSession.GetValidAccessTokenAsync(ct);
        if (accessToken is null)
            return 0;

        var pending = await apiClient.GetPendingOwnershipsAsync(accessToken, ct);
        var appliedCount = 0;

        foreach (var ownership in pending)
        {
            accessToken = await authSession.GetValidAccessTokenAsync(ct);
            if (accessToken is null)
                break;

            var file = await apiClient.DownloadItemFileAsync(accessToken, ownership.ItemId, ct);

            Directory.CreateDirectory(WallpapersDirectory);
            var extension = Path.GetExtension(file.FileName) is { Length: > 0 } ext ? ext : ".dat";
            var localPath = Path.Combine(WallpapersDirectory, ownership.ItemId + extension);
            await File.WriteAllBytesAsync(localPath, file.ImageData, ct);

            WallpaperApplier.Apply(localPath);

            await apiClient.MarkAppliedAsync(accessToken, ownership.OwnershipId, ct);
            appliedCount++;
        }

        return appliedCount;
    }
}

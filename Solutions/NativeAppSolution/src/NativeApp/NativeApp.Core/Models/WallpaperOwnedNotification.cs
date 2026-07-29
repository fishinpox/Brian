namespace NativeApp.Core.Models;

/// <summary>Shape of the SignalR "wallpaper-owned" push - matches the anonymous object
/// Notifications.API's WallpaperOwnershipGrantedConsumer sends: new { ev.OwnershipId, ev.ItemId }.</summary>
public record WallpaperOwnedNotification(Guid OwnershipId, Guid ItemId);

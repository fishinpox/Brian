using System.Net.Http;
using System.Windows;
using NativeApp.Core;
using Application = System.Windows.Application;

namespace NativeApp.Windows;

public partial class App : Application
{
    // The Gateway is the single origin for everything (/api/auth, /api/marketplace,
    // /hubs/notifications) - matches how a browser client reaches the backend too, and keeps
    // per-service ports out of the native app's configuration.
    private const string GatewayBaseUrl = "https://localhost:7000";

    private AuthSession? _authSession;
    private ApiClient? _apiClient;
    private NotificationClient? _notificationClient;
    private WallpaperSyncService? _wallpaperSync;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var httpClient = new HttpClient { BaseAddress = new Uri(GatewayBaseUrl) };
        _apiClient = new ApiClient(httpClient);
        var tokenStore = new DpapiTokenStore();
        _authSession = new AuthSession(_apiClient, tokenStore);
        _notificationClient = new NotificationClient(GatewayBaseUrl, _authSession);
        _wallpaperSync = new WallpaperSyncService(_apiClient, _authSession);

        var hasValidSession = await _authSession.GetValidAccessTokenAsync() is not null;

        if (!hasValidSession)
        {
            var loginWindow = new LoginWindow(_authSession);
            var signedIn = loginWindow.ShowDialog();
            if (signedIn != true)
            {
                Shutdown();
                return;
            }
        }

        var mainWindow = new MainWindow(_authSession, _apiClient, _notificationClient, _wallpaperSync);
        mainWindow.Show();
    }
}

using System.Collections.ObjectModel;
using System.Windows;
using NativeApp.Core;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace NativeApp.Windows;

public partial class MainWindow : Window
{
    private readonly AuthSession _authSession;
    private readonly ApiClient _apiClient;
    private readonly NotificationClient _notificationClient;
    private readonly WallpaperSyncService _wallpaperSync;
    private readonly ObservableCollection<CatalogEntry> _catalog = [];
    private System.Windows.Forms.NotifyIcon? _trayIcon;
    private bool _isExiting;

    public MainWindow(
        AuthSession authSession,
        ApiClient apiClient,
        NotificationClient notificationClient,
        WallpaperSyncService wallpaperSync)
    {
        InitializeComponent();

        _authSession = authSession;
        _apiClient = apiClient;
        _notificationClient = notificationClient;
        _wallpaperSync = wallpaperSync;

        CatalogList.ItemsSource = _catalog;

        SetupTrayIcon();
        _notificationClient.OnWallpaperOwned(OnWallpaperOwned);

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCatalogAsync();

        StatusText.Text = "Checking for wallpapers purchased while this app wasn't running...";
        var appliedOnStartup = await SafeApplyPendingAsync();
        StatusText.Text = appliedOnStartup > 0
            ? $"Applied {appliedOnStartup} pending wallpaper(s)."
            : "Ready.";

        try
        {
            await _notificationClient.StartAsync();
        }
        catch (Exception ex)
        {
            StatusText.Text = "Connected, but live updates are unavailable right now.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async Task LoadCatalogAsync()
    {
        var accessToken = await _authSession.GetValidAccessTokenAsync();
        if (accessToken is null)
        {
            SignOutButton_Click(this, new RoutedEventArgs());
            return;
        }

        var items = await _apiClient.GetCatalogAsync(accessToken);
        _catalog.Clear();
        foreach (var item in items)
            _catalog.Add(new CatalogEntry(item.Id, item.Name, item.Description));
    }

    private async void BuyButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button { DataContext: CatalogEntry entry })
            return;

        entry.CanBuy = false;
        entry.StatusLabel = "Buying...";

        try
        {
            var accessToken = await _authSession.GetValidAccessTokenAsync()
                ?? throw new InvalidOperationException("Not signed in.");
            await _apiClient.PurchaseAsync(accessToken, entry.Id);
            entry.StatusLabel = "Purchased";
            StatusText.Text = $"Purchased \"{entry.Name}\" - applying to your desktop...";

            // The purchase also fires a SignalR push that triggers the same apply path, but doing
            // it here too gives immediate feedback rather than waiting on the round trip.
            await SafeApplyPendingAsync();
            StatusText.Text = $"\"{entry.Name}\" applied to your desktop.";
        }
        catch (Exception ex)
        {
            entry.StatusLabel = "Buy";
            entry.CanBuy = true;
            StatusText.Text = "Purchase failed. Please try again.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async void OnWallpaperOwned(NativeApp.Core.Models.WallpaperOwnedNotification notification)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            StatusText.Text = "A new wallpaper purchase was detected - applying...";
            await SafeApplyPendingAsync();
            StatusText.Text = "Ready.";
        });
    }

    private async Task<int> SafeApplyPendingAsync()
    {
        try
        {
            return await _wallpaperSync.ApplyPendingAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return 0;
        }
    }

    private async void SignOutButton_Click(object sender, RoutedEventArgs e)
    {
        await _notificationClient.StopAsync();
        await _authSession.LogoutAsync();
        _isExiting = true;
        Close();
        Application.Current.Shutdown();
    }

    private void SetupTrayIcon()
    {
        _trayIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "Brian - Wallpapers"
        };

        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("Show", null, (_, _) => ShowFromTray());
        menu.Items.Add("Exit", null, (_, _) =>
        {
            _isExiting = true;
            Close();
            Application.Current.Shutdown();
        });
        _trayIcon.ContextMenuStrip = menu;
        _trayIcon.DoubleClick += (_, _) => ShowFromTray();
    }

    private void ShowFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isExiting)
        {
            _trayIcon?.Dispose();
            return;
        }

        // Minimize to tray instead of exiting, so the SignalR connection stays alive to catch
        // "purchased while running" pushes without the user having to keep a visible window open.
        e.Cancel = true;
        Hide();
    }
}

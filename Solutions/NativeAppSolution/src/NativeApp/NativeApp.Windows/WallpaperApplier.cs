using System.IO;
using System.Runtime.InteropServices;

namespace NativeApp.Windows;

/// <summary>Sets the Windows desktop wallpaper via the Win32 SystemParametersInfo API,
/// per Documentation/Calendar/CustomizeBackground.md's implementation notes.</summary>
public static class WallpaperApplier
{
    private const uint SPI_SETDESKWALLPAPER = 0x0014;
    private const uint SPIF_UPDATEINIFILE = 0x01;
    private const uint SPIF_SENDCHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

    /// <summary>
    /// SystemParametersInfo reads the wallpaper file from disk itself - it doesn't take image
    /// bytes directly - so the caller must have already written the downloaded file somewhere
    /// permanent (e.g. %LOCALAPPDATA%\Brian\NativeApp\Wallpapers\) before calling this.
    /// </summary>
    public static void Apply(string localFilePath)
    {
        if (!File.Exists(localFilePath))
            throw new FileNotFoundException("Wallpaper file not found.", localFilePath);

        var succeeded = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, localFilePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        if (!succeeded)
        {
            var error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"SystemParametersInfo failed to set the wallpaper (Win32 error {error}).");
        }
    }
}

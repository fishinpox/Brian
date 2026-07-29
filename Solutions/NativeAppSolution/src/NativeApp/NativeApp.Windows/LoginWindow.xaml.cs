using System.Windows;
using NativeApp.Core;

namespace NativeApp.Windows;

public partial class LoginWindow : Window
{
    private readonly AuthSession _authSession;

    public LoginWindow(AuthSession authSession)
    {
        InitializeComponent();
        _authSession = authSession;
    }

    private async void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Visibility = Visibility.Collapsed;
        SignInButton.IsEnabled = false;

        try
        {
            await _authSession.LoginAsync(EmailBox.Text.Trim(), PasswordBox.Password);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ErrorText.Text = "Could not sign in. Check your email and password and try again.";
            ErrorText.Visibility = Visibility.Visible;
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            SignInButton.IsEnabled = true;
        }
    }
}

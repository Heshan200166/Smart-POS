using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using SmartPOS.Services;
using SmartPOS.Models;

namespace SmartPOS.UI;

/// <summary>
/// Login window for user authentication
/// </summary>
public partial class LoginWindow : Window
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginWindow> _logger;
    
    public User? AuthenticatedUser { get; private set; }

    public LoginWindow(IAuthenticationService authService, ILogger<LoginWindow> logger)
    {
        InitializeComponent();
        _authService = authService;
        _logger = logger;

        // Set focus to username field
        Loaded += (s, e) => UsernameTextBox.Focus();
        
        // Handle Enter key in password field
        PasswordBox.KeyDown += (s, e) => {
            if (e.Key == Key.Enter)
                Login_Click(this, new RoutedEventArgs());
        };
        
        // Handle Enter key in username field
        UsernameTextBox.KeyDown += (s, e) => {
            if (e.Key == Key.Enter)
                PasswordBox.Focus();
        };

        _logger.LogInformation("Login window initialized");
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        await PerformLoginAsync(UsernameTextBox.Text, PasswordBox.Password);
    }

    private async void QuickLogin_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string role)
            return;

        // Quick login for development - use predefined credentials
        string username = role;
        string password = "admin123"; // Default password for all demo users

        UsernameTextBox.Text = username;
        PasswordBox.Password = password;
        
        await PerformLoginAsync(username, password);
    }

    private async Task PerformLoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowStatusMessage("Please enter both username and password.", false);
            return;
        }

        try
        {
            // Update UI to show loading
            LoginButton.Content = "Authenticating...";
            LoginButton.IsEnabled = false;
            StatusMessage.Visibility = Visibility.Collapsed;

            _logger.LogInformation("Attempting login for user: {Username}", username);

            // Attempt authentication
            var user = await _authService.LoginAsync(username, password);

            if (user != null)
            {
                _logger.LogInformation("Login successful for user: {Username}, Role: {Role}", 
                    user.Username, user.Role?.Name ?? "Unknown");

                AuthenticatedUser = user;

                // Store remember me preference if needed
                if (RememberMeCheckBox.IsChecked == true)
                {
                    // TODO: Implement remember me functionality (store in secure storage)
                    _logger.LogInformation("Remember me option selected for user: {Username}", username);
                }

                DialogResult = true;
                Close();
            }
            else
            {
                _logger.LogWarning("Login failed for user: {Username}", username);
                ShowStatusMessage("Invalid username or password.", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt for user: {Username}", username);
            ShowStatusMessage("Login error occurred. Please try again.", false);
        }
        finally
        {
            // Reset UI
            LoginButton.Content = "LOGIN";
            LoginButton.IsEnabled = true;
        }
    }

    private void ShowStatusMessage(string message, bool isSuccess)
    {
        StatusMessage.Text = message;
        StatusMessage.Foreground = isSuccess ? 
            System.Windows.Media.Brushes.Green : 
            System.Windows.Media.Brushes.Red;
        StatusMessage.Visibility = Visibility.Visible;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        // Remove minimize and maximize buttons
        var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        if (hwnd != IntPtr.Zero)
        {
            var style = GetWindowLong(hwnd, -16); // GWL_STYLE
            style &= ~0x10000; // Remove WS_MAXIMIZEBOX
            style &= ~0x20000; // Remove WS_MINIMIZEBOX
            SetWindowLong(hwnd, -16, style);
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
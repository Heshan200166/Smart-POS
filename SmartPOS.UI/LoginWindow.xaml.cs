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
            System.Console.WriteLine($"[LoginWindow] Attempting login for: {username}");

            // Add timeout to prevent hanging
            var loginTask = _authService.LoginAsync(username, password);
            var timeoutTask = Task.Delay(10000); // 10 second timeout

            var completedTask = await Task.WhenAny(loginTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                System.Console.WriteLine($"[LoginWindow] Login timeout after 10 seconds");
                ShowStatusMessage("Login timeout. Database may not be initialized.", false);
                LoginButton.Content = "LOGIN";
                LoginButton.IsEnabled = true;
                return;
            }

            // Attempt authentication
            var user = await loginTask;

            if (user != null)
            {
                _logger.LogInformation("Login successful for user: {Username}, Role: {Role}", 
                    user.Username, user.Role?.Name ?? "Unknown");
                System.Console.WriteLine($"[LoginWindow] Login successful: {username}");
                System.Console.WriteLine($"[LoginWindow] Setting DialogResult = true");

                AuthenticatedUser = user;

                // Store remember me preference if needed
                if (RememberMeCheckBox.IsChecked == true)
                {
                    // TODO: Implement remember me functionality (store in secure storage)
                    _logger.LogInformation("Remember me option selected for user: {Username}", username);
                }

                System.Console.WriteLine($"[LoginWindow] About to set DialogResult and close window");
                DialogResult = true;
                System.Console.WriteLine($"[LoginWindow] DialogResult set, now closing...");
                Close();
                System.Console.WriteLine($"[LoginWindow] Window closed");
            }
            else
            {
                _logger.LogWarning("Login failed for user: {Username}", username);
                System.Console.WriteLine($"[LoginWindow] Login failed: Invalid credentials");
                System.Console.WriteLine($"[LoginWindow] User object was NULL - authentication failed");
                
                // Show detailed error to help debug
                MessageBox.Show(
                    $"Login failed for user: {username}\n\n" +
                    "Possible reasons:\n" +
                    "• Wrong username or password\n" +
                    "• User account not active\n" +
                    "• Database not initialized\n\n" +
                    "Try:\n" +
                    "• Username: admin\n" +
                    "• Password: admin123",
                    "Login Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                
                ShowStatusMessage("Invalid username or password.", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt for user: {Username}", username);
            System.Console.WriteLine($"[LoginWindow] Login error: {ex.Message}");
            System.Console.WriteLine($"[LoginWindow] Stack trace: {ex.StackTrace}");
            ShowStatusMessage($"Login error: {ex.Message}", false);
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
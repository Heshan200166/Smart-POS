using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartPOS.Data;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartPOS.Models;
using SmartPOS.Business;
using Microsoft.Extensions.DependencyInjection;

namespace SmartPOS.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MainWindow> _logger;
    private readonly IConfiguration _configuration;
    private User? _currentUser;

    public MainWindow(ApplicationDbContext context, ILogger<MainWindow> logger, IConfiguration configuration)
    {
        InitializeComponent();
        _context = context;
        _logger = logger;
        _configuration = configuration;

        _logger.LogInformation("MainWindow initialized");
        
        // Setup UI based on user role when loaded
        Loaded += MainWindow_Loaded;
        
        // Auto-test connection on startup
        _ = Task.Run(AutoTestConnectionAsync);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Get current user from Tag (set by App.xaml.cs) for application mode
        if (Tag is User user)
        {
            _currentUser = user;
            UpdateUIForUser();
        }
    }

    private void UpdateUIForUser()
    {
        if (_currentUser == null) return;

        // Update user display
        CurrentUserText.Text = $"👤 {_currentUser.FullName} ({_currentUser.Role?.Name})";

        // Show admin panel only for Admin users
        if (_currentUser.Role?.Name == "Admin")
        {
            AdminPanel.Visibility = Visibility.Visible;
        }
        else
        {
            AdminPanel.Visibility = Visibility.Collapsed;
        }

        _logger.LogInformation("UI updated for user: {Username} with role: {Role}", 
            _currentUser.Username, _currentUser.Role?.Name ?? "Unknown");
    }

    private async Task AutoTestConnectionAsync()
    {
        await Task.Delay(1000); // Give UI time to load
        
        try
        {
            Dispatcher.Invoke(() => {
                StatusText.Text = "Auto-testing database connection...";
            });

            bool canConnect = await _context.Database.CanConnectAsync();

            Dispatcher.Invoke(() => {
                if (canConnect)
                {
                    DbStatusIcon.Text = "✓ ";
                    DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
                    DbStatusText.Text = "Database Connected Successfully";
                    StatusText.Text = "Ready - Database connected";
                }
                else
                {
                    DbStatusIcon.Text = "⚠️ ";
                    DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
                    DbStatusText.Text = "Database Connection Offline";
                    StatusText.Text = "Database offline";
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Auto database test failed");
            Dispatcher.Invoke(() => {
                DbStatusIcon.Text = "⚠️ ";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
                DbStatusText.Text = "Database Error";
                StatusText.Text = "Database connection error";
            });
        }
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var result = MessageBox.Show(
                $"Are you sure you want to logout?\n\nUser: {_currentUser?.FullName}",
                "Confirm Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _logger.LogInformation("User logged out: {Username}", _currentUser?.Username ?? "Unknown");

                // Close main window and restart application with login
                Application.Current.Shutdown(0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            MessageBox.Show("Error during logout. Please try again.", "Logout Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ManageUsers_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentUser?.Role?.Name != "Admin")
            {
                MessageBox.Show("Access denied. Only administrators can manage users.", 
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _logger.LogInformation("Opening User Management window");
            
            var authService = new AuthenticationService(_context);
            // Wrap the current logger to provide the correct type
            var userMgmtLogger = new LoggerWrapper<UserManagementWindow>((ILogger)_logger);

            var userManagementWindow = new UserManagementWindow(_context, authService, userMgmtLogger)
            {
                Owner = this
            };

            userManagementWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open User Management window");
            MessageBox.Show("Failed to open User Management. Please try again.", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ManageRoles_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentUser?.Role?.Name != "Admin")
            {
                MessageBox.Show("Access denied. Only administrators can manage roles.", 
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _logger.LogInformation("Opening Role Management");

            // For now, show a simple dialog with role information
            var roles = _context.Roles.ToList();
            var roleInfo = string.Join("\n", roles.Select(r => $"• {r.Name}: {r.Description}"));

            MessageBox.Show($"Current System Roles:\n\n{roleInfo}\n\n" +
                           "Advanced role management coming in future updates!",
                           "Role Management", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open Role Management");
            MessageBox.Show("Failed to load roles. Please try again.", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _logger.LogInformation("Opening Change Password dialog for user: {Username}", _currentUser?.Username);

            var changePasswordWindow = new ChangePasswordWindow(_context, _currentUser!)
            {
                Owner = this
            };

            changePasswordWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open Change Password window");
            MessageBox.Show("Failed to open Change Password. Please try again.", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
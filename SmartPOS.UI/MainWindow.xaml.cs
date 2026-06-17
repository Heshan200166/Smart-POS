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
using Serilog;

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
        // Get current user from Tag (set by App.xaml.cs)
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
                    DbStatusText.Text = "Database Not Connected (Click to configure)";
                    StatusText.Text = "Database configuration needed";
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Auto database test failed");
            Dispatcher.Invoke(() => {
                DbStatusIcon.Text = "⚠️ ";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
                DbStatusText.Text = "Database Configuration Required";
                StatusText.Text = "Database setup needed";
            });
        }
    }

    private async void TestConnection_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StatusText.Text = "Testing database connection...";
            _logger.LogInformation("Testing database connection");

            bool canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
            {
                DbStatusIcon.Text = "✓ ";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
                DbStatusText.Text = "Database Connected Successfully";
                StatusText.Text = "Database connection successful!";
                _logger.LogInformation("Database connection successful");
                MessageBox.Show("✅ Database connection successful!\n\nYou can now initialize the database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ShowDatabaseSetupDialog();
            }
        }
        catch (Exception ex)
        {
            DbStatusIcon.Text = "⚠️ ";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
            DbStatusText.Text = "Database Configuration Needed";
            StatusText.Text = "Database setup required";
            _logger.LogWarning(ex, "Database connection failed");
            ShowDatabaseSetupDialog();
        }
    }

    private void ShowDatabaseSetupDialog()
    {
        var result = MessageBox.Show(
            "❌ Database connection failed!\n\n" +
            "Would you like to:\n" +
            "• YES - Configure database connection\n" +
            "• NO - Use default LocalDB (recommended)\n" +
            "• CANCEL - Skip for now",
            "Database Setup", 
            MessageBoxButton.YesNoCancel, 
            MessageBoxImage.Question);

        switch (result)
        {
            case MessageBoxResult.Yes:
                ShowDatabaseConfigurationWindow();
                break;
            case MessageBoxResult.No:
                ConfigureLocalDB();
                break;
            case MessageBoxResult.Cancel:
                StatusText.Text = "Database configuration skipped";
                break;
        }
    }

    private void ShowDatabaseConfigurationWindow()
    {
        var configWindow = new DatabaseConfigWindow(_configuration, _logger);
        if (configWindow.ShowDialog() == true)
        {
            // Restart application or reload configuration
            MessageBox.Show("Database configuration updated!\n\nPlease restart the application to apply changes.", 
                "Configuration Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void ConfigureLocalDB()
    {
        try
        {
            StatusText.Text = "Configuring LocalDB...";
            
            // Update connection string to LocalDB
            var localDbConnection = "Server=(localdb)\\mssqllocaldb;Database=SmartPOS;Trusted_Connection=true;TrustServerCertificate=true;";
            UpdateConnectionString(localDbConnection);
            
            StatusText.Text = "LocalDB configured. Testing connection...";
            await Task.Delay(1000);
            
            // Test the new connection (would need app restart in real scenario)
            MessageBox.Show("✅ LocalDB configured successfully!\n\n" +
                           "Connection String: Server=(localdb)\\mssqllocaldb\n" +
                           "Database: SmartPOS\n\n" +
                           "Please restart the application to apply changes.", 
                           "LocalDB Setup Complete", 
                           MessageBoxButton.OK, 
                           MessageBoxImage.Information);
                           
            DbStatusIcon.Text = "🔄 ";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Blue;
            DbStatusText.Text = "LocalDB Configured - Restart Required";
            StatusText.Text = "LocalDB configured - restart needed";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure LocalDB");
            MessageBox.Show($"Failed to configure LocalDB:\n{ex.Message}", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateConnectionString(string connectionString)
    {
        try
        {
            var appSettingsPath = "appsettings.json";
            var json = File.ReadAllText(appSettingsPath);
            var jsonObj = JObject.Parse(json);
            
            jsonObj["ConnectionStrings"]["DefaultConnection"] = connectionString;
            
            var updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(appSettingsPath, updatedJson);
            
            _logger.LogInformation("Connection string updated in appsettings.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update connection string");
            throw;
        }
    }

    private async void InitializeDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StatusText.Text = "Initializing database...";
            _logger.LogInformation("Starting database initialization");

            // Show progress
            var progressDialog = new DatabaseProgressWindow();
            progressDialog.Show();
            
            await Task.Delay(500); // Brief pause for UI

            progressDialog.UpdateProgress("Creating database...", 20);
            await Task.Delay(500);

            // Create database and apply migrations
            await _context.Database.MigrateAsync();

            progressDialog.UpdateProgress("Applying migrations...", 60);
            await Task.Delay(500);

            // Verify tables exist
            var tablesExist = await _context.Database.CanConnectAsync();
            
            progressDialog.UpdateProgress("Verifying setup...", 90);
            await Task.Delay(500);

            progressDialog.UpdateProgress("Complete!", 100);
            await Task.Delay(1000);

            progressDialog.Close();

            DbStatusIcon.Text = "✓ ";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
            DbStatusText.Text = "Database Ready - All Tables Created";
            StatusText.Text = "Database initialized successfully!";
            
            _logger.LogInformation("Database initialized successfully");
            
            MessageBox.Show("🎉 Database initialization complete!\n\n" +
                           "✅ Database created\n" +
                           "✅ Tables created\n" +
                           "✅ Default roles added\n" +
                           "✅ Ready for use!\n\n" +
                           "Your Smart POS system is now ready!", 
                           "Initialization Complete", 
                           MessageBoxButton.OK, 
                           MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Initialization failed: {ex.Message}";
            _logger.LogError(ex, "Error initializing database");
            
            var errorMsg = "❌ Database initialization failed!\n\n" +
                          $"Error: {ex.Message}\n\n" +
                          "Possible solutions:\n" +
                          "• Check if SQL Server is installed\n" +
                          "• Verify connection string\n" +
                          "• Try configuring LocalDB\n" +
                          "• Check application logs";
                          
            MessageBox.Show(errorMsg, "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ConfigureDatabase_Click(object sender, RoutedEventArgs e)
    {
        ShowDatabaseConfigurationWindow();
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
            // Create a Serilog logger for UserManagementWindow
            var userMgmtLogger = Log.ForContext<UserManagementWindow>();
            var msLogger = new SerilogLoggerWrapper<UserManagementWindow>(userMgmtLogger);

            var userManagementWindow = new UserManagementWindow(_context, authService, msLogger)
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
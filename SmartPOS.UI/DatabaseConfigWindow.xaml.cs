using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;

namespace SmartPOS.UI;

public partial class DatabaseConfigWindow : Window
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public DatabaseConfigWindow(IConfiguration configuration, ILogger logger)
    {
        InitializeComponent();
        _configuration = configuration;
        _logger = logger;
        
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Load current connection string
        var currentConnection = _configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(currentConnection))
        {
            PreviewTextBox.Text = currentConnection;
        }

        // Setup event handlers
        LocalDBRadio.Checked += OnConnectionTypeChanged;
        SqlServerRadio.Checked += OnConnectionTypeChanged;
        CustomRadio.Checked += OnConnectionTypeChanged;
        WindowsAuthCheckBox.Checked += OnAuthTypeChanged;
        WindowsAuthCheckBox.Unchecked += OnAuthTypeChanged;

        // Setup text change handlers for real-time preview
        ServerTextBox.TextChanged += OnSettingsChanged;
        DatabaseTextBox.TextChanged += OnSettingsChanged;
        UsernameTextBox.TextChanged += OnSettingsChanged;
        CustomConnectionTextBox.TextChanged += OnCustomConnectionChanged;

        UpdateUI();
        UpdatePreview();
    }

    private void OnConnectionTypeChanged(object sender, RoutedEventArgs e)
    {
        UpdateUI();
        UpdatePreview();
    }

    private void OnAuthTypeChanged(object sender, RoutedEventArgs e)
    {
        var isWindowsAuth = WindowsAuthCheckBox.IsChecked ?? true;
        UsernameLabel.IsEnabled = !isWindowsAuth;
        UsernameTextBox.IsEnabled = !isWindowsAuth;
        PasswordLabel.IsEnabled = !isWindowsAuth;
        PasswordBox.IsEnabled = !isWindowsAuth;
        
        UpdatePreview();
    }

    private void OnSettingsChanged(object sender, EventArgs e)
    {
        UpdatePreview();
    }

    private void OnCustomConnectionChanged(object sender, EventArgs e)
    {
        if (CustomRadio.IsChecked == true)
        {
            PreviewTextBox.Text = CustomConnectionTextBox.Text;
        }
    }

    private void UpdateUI()
    {
        if (LocalDBRadio.IsChecked == true)
        {
            SqlServerGroup.Visibility = Visibility.Collapsed;
            CustomGroup.Visibility = Visibility.Collapsed;
        }
        else if (SqlServerRadio.IsChecked == true)
        {
            SqlServerGroup.Visibility = Visibility.Visible;
            CustomGroup.Visibility = Visibility.Collapsed;
        }
        else if (CustomRadio.IsChecked == true)
        {
            SqlServerGroup.Visibility = Visibility.Collapsed;
            CustomGroup.Visibility = Visibility.Visible;
        }
    }

    private void UpdatePreview()
    {
        try
        {
            string connectionString = "";

            if (LocalDBRadio.IsChecked == true)
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=SmartPOS;Trusted_Connection=true;TrustServerCertificate=true;";
            }
            else if (SqlServerRadio.IsChecked == true)
            {
                var server = string.IsNullOrEmpty(ServerTextBox.Text) ? "localhost" : ServerTextBox.Text;
                var database = string.IsNullOrEmpty(DatabaseTextBox.Text) ? "SmartPOS" : DatabaseTextBox.Text;

                if (WindowsAuthCheckBox.IsChecked ?? true)
                {
                    connectionString = $"Server={server};Database={database};Trusted_Connection=true;TrustServerCertificate=true;";
                }
                else
                {
                    var username = UsernameTextBox.Text;
                    var password = PasswordBox.Password;
                    connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=true;";
                }
            }
            else if (CustomRadio.IsChecked == true)
            {
                connectionString = CustomConnectionTextBox.Text;
            }

            PreviewTextBox.Text = connectionString;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error updating connection preview");
            PreviewTextBox.Text = "Invalid connection string format";
        }
    }

    private async void TestConnection_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        
        var originalContent = button.Content;
        
        try
        {
            button.Content = "Testing...";
            button.IsEnabled = false;

            var connectionString = PreviewTextBox.Text;
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Please configure a connection string first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            
            MessageBox.Show("✅ Database connection successful!\n\nConnection is working properly.", 
                "Test Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            
            var errorMessage = "❌ Database connection failed!\n\n" +
                             $"Error: {ex.Message}\n\n" +
                             "Common solutions:\n" +
                             "• Check server name and database name\n" +
                             "• Verify SQL Server is running\n" +
                             "• Check username/password if using SQL auth\n" +
                             "• Try LocalDB for development";
                             
            MessageBox.Show(errorMessage, "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            button.Content = originalContent;
            button.IsEnabled = true;
        }
    }

    private void SaveAndApply_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var connectionString = PreviewTextBox.Text;
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Please configure a connection string first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update appsettings.json
            UpdateAppSettings(connectionString);

            MessageBox.Show("✅ Database configuration saved!\n\n" +
                           "The application will use the new connection string.\n" +
                           "Restart the application to apply changes.", 
                           "Configuration Saved", 
                           MessageBoxButton.OK, 
                           MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save database configuration");
            MessageBox.Show($"Failed to save configuration:\n{ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateAppSettings(string connectionString)
    {
        var appSettingsPath = "appsettings.json";
        
        if (!File.Exists(appSettingsPath))
        {
            // Create new appsettings if it doesn't exist
            var newSettings = new
            {
                ConnectionStrings = new { DefaultConnection = connectionString },
                Logging = new
                {
                    LogLevel = new
                    {
                        Default = "Information",
                        Microsoft = "Warning",
                        Microsoft_EntityFrameworkCore = "Information"
                    }
                },
                ApplicationSettings = new
                {
                    AppName = "Smart Retail POS Management System",
                    Version = "1.0.0"
                }
            };
            
            var json = JsonConvert.SerializeObject(newSettings, Formatting.Indented);
            File.WriteAllText(appSettingsPath, json);
        }
        else
        {
            // Update existing appsettings
            var json = File.ReadAllText(appSettingsPath);
            var jsonObj = JObject.Parse(json);
            
            if (jsonObj["ConnectionStrings"] == null)
                jsonObj["ConnectionStrings"] = new JObject();
                
            jsonObj["ConnectionStrings"]["DefaultConnection"] = connectionString;
            
            var updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(appSettingsPath, updatedJson);
        }
        
        _logger.LogInformation("Connection string updated in appsettings.json");
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
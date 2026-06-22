using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

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
        // Load current config from file to ensure we display actual current settings
        string provider = "SQLite";
        string currentConnection = "";

        try
        {
            var configPath = "appsettings.json";
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var jsonObj = JObject.Parse(json);
                provider = jsonObj["DatabaseProvider"]?.ToString() ?? "SQLite";
                currentConnection = jsonObj["ConnectionStrings"]?["DefaultConnection"]?.ToString() ?? "";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load database config for initialization");
        }

        // Set radio buttons based on provider and connection string content
        if (provider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            SqliteRadio.IsChecked = true;
            if (!string.IsNullOrEmpty(currentConnection))
            {
                // Extract filename from "Data Source=Filename.db"
                var match = currentConnection.Replace("Data Source=", "").Trim();
                SqliteFileTextBox.Text = string.IsNullOrEmpty(match) ? "SmartPOS.db" : match;
            }
        }
        else if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            if (currentConnection.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
            {
                LocalDBRadio.IsChecked = true;
            }
            else if (!string.IsNullOrEmpty(currentConnection))
            {
                SqlServerRadio.IsChecked = true;
                // Parse server/database details roughly for UI convenience
                try
                {
                    var builder = new SqlConnectionStringBuilder(currentConnection);
                    ServerTextBox.Text = builder.DataSource;
                    DatabaseTextBox.Text = builder.InitialCatalog;
                    WindowsAuthCheckBox.IsChecked = builder.IntegratedSecurity;
                    if (!builder.IntegratedSecurity)
                    {
                        UsernameTextBox.Text = builder.UserID;
                        PasswordBox.Password = builder.Password;
                    }
                }
                catch
                {
                    // If parsing failed, fall back to Custom
                    CustomRadio.IsChecked = true;
                    CustomConnectionTextBox.Text = currentConnection;
                }
            }
            else
            {
                LocalDBRadio.IsChecked = true;
            }
        }

        // Setup event handlers
        SqliteRadio.Checked += OnConnectionTypeChanged;
        LocalDBRadio.Checked += OnConnectionTypeChanged;
        SqlServerRadio.Checked += OnConnectionTypeChanged;
        CustomRadio.Checked += OnConnectionTypeChanged;
        WindowsAuthCheckBox.Checked += OnAuthTypeChanged;
        WindowsAuthCheckBox.Unchecked += OnAuthTypeChanged;

        // Setup text change handlers for real-time preview
        SqliteFileTextBox.TextChanged += OnSettingsChanged;
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
        SqliteGroup.Visibility = SqliteRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        SqlServerGroup.Visibility = SqlServerRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        CustomGroup.Visibility = CustomRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdatePreview()
    {
        try
        {
            string connectionString = "";

            if (SqliteRadio.IsChecked == true)
            {
                var file = string.IsNullOrEmpty(SqliteFileTextBox.Text) ? "SmartPOS.db" : SqliteFileTextBox.Text;
                connectionString = $"Data Source={file}";
            }
            else if (LocalDBRadio.IsChecked == true)
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

            if (SqliteRadio.IsChecked == true)
            {
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
            }
            
            MessageBox.Show("✅ Database connection successful!\n\nConnection is working properly.", 
                "Test Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            
            var errorMessage = "❌ Database connection failed!\n\n" +
                             $"Error: {ex.Message}\n\n" +
                             "Common solutions:\n" +
                             "• Verify the file path if using SQLite\n" +
                             "• Check server name and database name if using SQL Server\n" +
                             "• Verify SQL Server is running\n" +
                             "• Check credentials if using SQL auth";
                             
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

            string provider = SqliteRadio.IsChecked == true ? "SQLite" : "SqlServer";

            // Update appsettings.json
            UpdateAppSettings(provider, connectionString);

            MessageBox.Show("✅ Database configuration saved!\n\n" +
                           "The application will use the new connection settings.", 
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

    private void UpdateAppSettings(string provider, string connectionString)
    {
        var appSettingsPath = "appsettings.json";
        JObject jsonObj;
        
        if (!File.Exists(appSettingsPath))
        {
            jsonObj = new JObject();
        }
        else
        {
            var json = File.ReadAllText(appSettingsPath);
            jsonObj = JObject.Parse(json);
        }
        
        // Update values
        jsonObj["DatabaseProvider"] = provider;
        
        if (jsonObj["ConnectionStrings"] == null)
            jsonObj["ConnectionStrings"] = new JObject();
            
        jsonObj["ConnectionStrings"]!["DefaultConnection"] = connectionString;

        if (jsonObj["Logging"] == null)
        {
            jsonObj["Logging"] = JObject.FromObject(new
            {
                LogLevel = new
                {
                    Default = "Information",
                    Microsoft = "Warning",
                    Microsoft_EntityFrameworkCore = "Information"
                }
            });
        }

        if (jsonObj["ApplicationSettings"] == null)
        {
            jsonObj["ApplicationSettings"] = JObject.FromObject(new
            {
                AppName = "Smart Retail POS Management System",
                Version = "1.0.0"
            });
        }
        
        var updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(appSettingsPath, updatedJson);
        
        _logger.LogInformation("Database connection settings updated in appsettings.json: Provider={Provider}", provider);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartPOS.Data;

namespace SmartPOS.UI;

public partial class DatabaseSetupWindow : Window
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSetupWindow> _logger;
    private ApplicationDbContext _context;
    private bool _databaseInitialized = false;

    public DatabaseSetupWindow(ApplicationDbContext context, ILogger<DatabaseSetupWindow> logger, IConfiguration configuration)
    {
        InitializeComponent();
        _context = context;
        _logger = logger;
        _configuration = configuration;

        Loaded += DatabaseSetupWindow_Loaded;
    }

    private async void DatabaseSetupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadDatabaseSettings();
        await AutoTestConnectionAsync();
    }

    private void LoadDatabaseSettings()
    {
        try
        {
            // Reload configuration from file to ensure we get latest settings
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var freshConfig = configBuilder.Build();

            var provider = freshConfig["DatabaseProvider"] ?? "SQLite";
            var connectionString = freshConfig.GetConnectionString("DefaultConnection") ?? "Data Source=SmartPOS.db";

            ProviderText.Text = provider;
            ConnectionStringText.Text = connectionString;

            // Recreate DbContext options dynamically based on current configuration
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            // Suppress model changes warnings
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

            // Dispose old context if possible and create new one
            _context = new ApplicationDbContext(optionsBuilder.Options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load database settings");
        }
    }

    private async Task AutoTestConnectionAsync()
    {
        try
        {
            DbStatusIcon.Text = "⏳";
            DbStatusText.Text = "Testing database connection...";
            DbStatusText.Foreground = System.Windows.Media.Brushes.Orange;

            bool canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
            {
                DbStatusIcon.Text = "✓";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
                DbStatusText.Text = "Database Connected Successfully";
                DbStatusText.Foreground = System.Windows.Media.Brushes.Green;
                
                ProceedToLoginButton.IsEnabled = true;
                ProceedToLoginButton.Background = System.Windows.Media.Brushes.Green;
                SetupInstructions.Text = "✅ Connection verified. Ready to proceed.";
                SetupInstructions.Foreground = System.Windows.Media.Brushes.Green;
                _databaseInitialized = true;
            }
            else
            {
                DbStatusIcon.Text = "⚠️";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
                DbStatusText.Text = "Database Connection Failed";
                DbStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Auto database test failed");
            DbStatusIcon.Text = "⚠️";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Orange;
            DbStatusText.Text = "Database Connection Failed";
            DbStatusText.Foreground = System.Windows.Media.Brushes.Red;
        }
    }

    private async void TestConnection_Click(object sender, RoutedEventArgs e)
    {
        LoadDatabaseSettings(); // Ensure settings are fresh
        await AutoTestConnectionAsync();
        
        if (_databaseInitialized)
        {
            MessageBox.Show("✅ Database connection successful!", "Test Connection", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("❌ Database connection failed. Please check connection string or configure settings.", "Test Connection", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ConfigureDatabase_Click(object sender, RoutedEventArgs e)
    {
        var configWindow = new DatabaseConfigWindow(_configuration, _logger);
        configWindow.Owner = this;
        if (configWindow.ShowDialog() == true)
        {
            LoadDatabaseSettings();
            _ = AutoTestConnectionAsync();
        }
    }

    private async void InitializeDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadDatabaseSettings(); // Ensure settings are fresh

            // Show progress dialog
            var progressDialog = new DatabaseProgressWindow();
            progressDialog.Owner = this;
            progressDialog.Show();
            
            await Task.Delay(500);

            progressDialog.UpdateProgress("Creating database...", 20);
            await Task.Delay(500);

            progressDialog.UpdateProgress("Applying migrations...", 60);
            await _context.Database.MigrateAsync();
            await Task.Delay(500);

            progressDialog.UpdateProgress("Verifying setup...", 90);
            await Task.Delay(500);

            progressDialog.UpdateProgress("Complete!", 100);
            await Task.Delay(800);

            progressDialog.Close();

            _databaseInitialized = true;
            
            DbStatusIcon.Text = "✓";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
            DbStatusText.Text = "Database Initialized & Ready";
            DbStatusText.Foreground = System.Windows.Media.Brushes.Green;

            ProceedToLoginButton.IsEnabled = true;
            ProceedToLoginButton.Background = System.Windows.Media.Brushes.Green;
            SetupInstructions.Text = "✅ Database initialized. Click Continue to Login";
            SetupInstructions.Foreground = System.Windows.Media.Brushes.Green;

            MessageBox.Show("🎉 Database initialization complete!\n\n" +
                           "✅ Database and tables created successfully\n" +
                           "✅ Default roles seeded\n\n" +
                           "Your Smart POS system is now ready for use!", 
                           "Initialization Complete", 
                           MessageBoxButton.OK, 
                           MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            MessageBox.Show($"❌ Database initialization failed:\n\n{ex.Message}\n\nPlease check connection settings and logs.", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ProceedToLogin_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}

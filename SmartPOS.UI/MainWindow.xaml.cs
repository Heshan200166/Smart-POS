using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPOS.Data;

namespace SmartPOS.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ApplicationDbContext context, ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _context = context;
        _logger = logger;

        _logger.LogInformation("MainWindow initialized");
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
                MessageBox.Show("Database connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                DbStatusIcon.Text = "✗ ";
                DbStatusIcon.Foreground = System.Windows.Media.Brushes.Red;
                DbStatusText.Text = "Database Connection Failed";
                StatusText.Text = "Failed to connect to database";
                _logger.LogWarning("Database connection failed");
                MessageBox.Show("Failed to connect to database. Please check your connection string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            DbStatusIcon.Text = "✗ ";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Red;
            DbStatusText.Text = "Database Connection Error";
            StatusText.Text = $"Error: {ex.Message}";
            _logger.LogError(ex, "Error testing database connection");
            MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void InitializeDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StatusText.Text = "Initializing database...";
            _logger.LogInformation("Starting database initialization");

            // Create database and apply migrations
            await _context.Database.MigrateAsync();

            DbStatusIcon.Text = "✓ ";
            DbStatusIcon.Foreground = System.Windows.Media.Brushes.Green;
            DbStatusText.Text = "Database Initialized Successfully";
            StatusText.Text = "Database initialized successfully!";
            
            _logger.LogInformation("Database initialized successfully");
            MessageBox.Show("Database has been created and initialized successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Initialization failed: {ex.Message}";
            _logger.LogError(ex, "Error initializing database");
            MessageBox.Show($"Failed to initialize database:\n{ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
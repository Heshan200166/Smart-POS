using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartPOS.Business;
using SmartPOS.Data;
using SmartPOS.Services;

namespace SmartPOS.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    public IConfiguration? Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);

            System.Console.WriteLine("🚀 SmartPOS Application Starting...");
            System.Console.WriteLine($"📂 Working Directory: {Directory.GetCurrentDirectory()}");

            // Create logs directory if it doesn't exist
            var logsDir = "logs";
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
                System.Console.WriteLine($"📁 Created logs directory: {logsDir}");
            }

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/smartpos-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application startup initiated");
            System.Console.WriteLine("✅ Logging configured");

            // Build configuration - find appsettings.json
            var configPath = "appsettings.json";
            System.Console.WriteLine($"🔍 Looking for configuration at: {Path.Combine(Directory.GetCurrentDirectory(), configPath)}");
            
            if (!File.Exists(configPath))
            {
                System.Console.WriteLine("⚠️  appsettings.json not found, creating default...");
                Log.Warning("appsettings.json not found at {ConfigPath}, creating default...", configPath);
                CreateDefaultAppSettings();
            }
            else
            {
                System.Console.WriteLine("✅ appsettings.json found");
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configPath, optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            Log.Information("Configuration loaded successfully");
            System.Console.WriteLine("✅ Configuration loaded");

            // Setup DI Container
            System.Console.WriteLine("🔧 Configuring dependency injection...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            Log.Information("Dependency injection configured");
            System.Console.WriteLine("✅ Dependency injection configured");

            // Show main window
            System.Console.WriteLine("📱 Creating main window...");
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            System.Console.WriteLine("📱 Showing main window...");
            mainWindow.Show();
            System.Console.WriteLine("✅ Main window shown");

            Log.Information("Application started successfully");
            System.Console.WriteLine("\n🎉 Application started successfully!");
            System.Console.WriteLine("🪟 Smart POS window should now be visible.\n");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"\n❌ ERROR: {ex.Message}");
            System.Console.WriteLine($"\n📋 Details:\n{ex}");
            Log.Fatal(ex, "Application startup failed");
            MessageBox.Show($"Application failed to start:\n\n{ex.Message}\n\n{ex.StackTrace}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private void CreateDefaultAppSettings()
    {
        var defaultConfig = @"{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Server=(localdb)\\mssqllocaldb;Database=SmartPOS;Trusted_Connection=true;TrustServerCertificate=true;""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft"": ""Warning"",
      ""Microsoft.EntityFrameworkCore"": ""Information""
    }
  },
  ""ApplicationSettings"": {
    ""AppName"": ""Smart Retail POS Management System"",
    ""Version"": ""1.0.0""
  }
}";
        File.WriteAllText("appsettings.json", defaultConfig);
        Log.Information("Default appsettings.json created");
        System.Console.WriteLine("✅ Default appsettings.json created");
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        services.AddSingleton(Configuration!);

        // Logging
        services.AddLogging(configure =>
        {
            configure.AddSerilog();
        });

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = Configuration!.GetConnectionString("DefaultConnection");
            System.Console.WriteLine($"📊 Database connection: {connectionString}");
            options.UseSqlServer(connectionString);
        });

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // Windows
        services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        System.Console.WriteLine("\n🛑 Application shutting down...");
        Log.Information("Application shutting down");
        Log.CloseAndFlush();
        _serviceProvider?.Dispose();
        System.Console.WriteLine("✅ Application closed");
        base.OnExit(e);
    }
}


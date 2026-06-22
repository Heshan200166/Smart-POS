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
using SmartPOS.Models;

namespace SmartPOS.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    public IConfiguration? Configuration { get; private set; }
    private User? _currentUser;

    protected override async void OnStartup(StartupEventArgs e)
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

            // Show database setup window FIRST
            System.Console.WriteLine("🗄️ Showing database setup window...");
            await ShowDatabaseSetupWindowAsync();

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

    private async Task ShowDatabaseSetupWindowAsync()
    {
        try
        {
            System.Console.WriteLine("🗄️ Creating database setup window...");
            
            var setupWindow = _serviceProvider!.GetRequiredService<DatabaseSetupWindow>();
            
            System.Console.WriteLine("✅ Showing database setup window...");
            var setupResult = setupWindow.ShowDialog();

            if (setupResult == true)
            {
                System.Console.WriteLine("✅ Database setup completed, proceeding to login...");
                await ShowLoginWindowAsync();
            }
            else
            {
                System.Console.WriteLine("❌ Database setup cancelled");
                Log.Information("Database setup cancelled by user");
                Shutdown(0);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Database setup failed: {ex.Message}");
            System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Log.Error(ex, "Database setup failed");
            MessageBox.Show($"Database setup failed:\n{ex.Message}\n\nPlease check the console for details.", "Setup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private async Task ShowLoginWindowAsync()
    {
        try
        {
            // IMPORTANT: Ensure database and data are seeded BEFORE showing login
            System.Console.WriteLine("🔧 Initializing database and default users...");
            await EnsureDatabaseInitializedAsync();
            
            // DEBUG: Verify users were created
            var context = _serviceProvider!.GetRequiredService<ApplicationDbContext>();
            var userCount = await context.Users.CountAsync();
            System.Console.WriteLine($"📊 DEBUG: Total users in database: {userCount}");
            
            if (userCount > 0)
            {
                var users = await context.Users.Include(u => u.Role).ToListAsync();
                System.Console.WriteLine("📋 DEBUG: Users in database:");
                foreach (var u in users)
                {
                    System.Console.WriteLine($"   - {u.Username} ({u.Role?.Name}) - Active: {u.IsActive}");
                }
            }
            else
            {
                System.Console.WriteLine("⚠️ WARNING: No users found in database!");
            }
            
            var authService = _serviceProvider!.GetRequiredService<IAuthenticationService>();
            var loginLogger = _serviceProvider!.GetRequiredService<ILogger<LoginWindow>>();
            
            var loginWindow = new LoginWindow(authService, loginLogger);
            var loginResult = loginWindow.ShowDialog();

            System.Console.WriteLine($"🔍 DEBUG: Login dialog result: {loginResult}");
            System.Console.WriteLine($"🔍 DEBUG: Authenticated user: {loginWindow.AuthenticatedUser?.Username}");

            if (loginResult == true && loginWindow.AuthenticatedUser != null)
            {
                _currentUser = loginWindow.AuthenticatedUser;
                Log.Information("User logged in: {Username} ({Role})", 
                    _currentUser.Username, _currentUser.Role?.Name ?? "Unknown");
                System.Console.WriteLine($"✅ User logged in: {_currentUser.Username} ({_currentUser.Role?.Name})");
                
                // Show main POS application window
                await ShowMainWindowAsync();
            }
            else
            {
                System.Console.WriteLine("❌ Login cancelled or failed");
                Log.Information("Login cancelled by user");
                Shutdown(0);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Login process failed: {ex.Message}");
            System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Log.Error(ex, "Login process failed");
            MessageBox.Show($"Login failed:\n{ex.Message}\n\nPlease check the console for details.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private async Task ShowMainWindowAsync()
    {
        try
        {
            System.Console.WriteLine("📱 Creating main window...");
            
            // Create a new scope for the main window
            var scope = _serviceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<MainWindow>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            
            var mainWindow = new MainWindow(context, logger, configuration);
            
            // Update window title with user info
            mainWindow.Title = $"Smart Retail POS Management System - {_currentUser?.FullName} ({_currentUser?.Role?.Name})";
            
            // Store current user in window tag for access by other components
            mainWindow.Tag = _currentUser;
            
            System.Console.WriteLine("📱 Showing main window...");
            mainWindow.Show();
            System.Console.WriteLine("✅ Main window shown");

            Log.Information("Application started successfully");
            System.Console.WriteLine("\n🎉 Application started successfully!");
            System.Console.WriteLine($"Welcome, {_currentUser?.FullName}!");
            System.Console.WriteLine("🪟 Smart POS window should now be visible.\n");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Failed to show main window: {ex.Message}");
            System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Log.Error(ex, "Failed to show main window");
            MessageBox.Show($"Failed to show main window:\n{ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private async Task EnsureDatabaseInitializedAsync()
    {
        try
        {
            System.Console.WriteLine("🗄️ Checking database status...");
            
            var context = _serviceProvider!.GetRequiredService<ApplicationDbContext>();
            
            // Use migrations instead of EnsureCreated to avoid pending model changes warning
            System.Console.WriteLine("📊 Applying pending migrations...");
            await context.Database.MigrateAsync();
            System.Console.WriteLine("✅ Database ready");
            
            // Seed initial data
            System.Console.WriteLine("🌱 Seeding initial data...");
            var seedingService = _serviceProvider.GetRequiredService<IDataSeedingService>();
            
            await seedingService.SeedInitialDataAsync();
            await seedingService.SeedSampleDataAsync();
            
            Log.Information("Database and initial data ready");
            System.Console.WriteLine("✅ Database initialization complete");
            System.Console.WriteLine("✅ Default users created (admin/admin123)");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"⚠️ Database initialization error: {ex.Message}");
            System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Log.Error(ex, "Database initialization failed");
            
            // Show detailed error to user
            MessageBox.Show(
                $"Failed to initialize database:\n\n{ex.Message}\n\nThe application will continue, but you may need to initialize the database manually.\n\nSee console for details.",
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void CreateDefaultAppSettings()
    {
        var defaultConfig = @"{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Data Source=SmartPOS.db""
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
    ""Version"": ""1.0.0 - Phase 2 (SQLite)""
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

        // Database - Dynamic configuration based on appsettings.json
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var provider = Configuration?["DatabaseProvider"] ?? "SQLite";
            var connectionString = Configuration?.GetConnectionString("DefaultConnection");

            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                connectionString ??= "Server=(localdb)\\mssqllocaldb;Database=SmartPOS;Trusted_Connection=true;TrustServerCertificate=true;";
                System.Console.WriteLine($"📊 Database Provider: SQL Server | Connection: {connectionString}");
                options.UseSqlServer(connectionString);
            }
            else
            {
                connectionString ??= $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "SmartPOS.db")}";
                System.Console.WriteLine($"📊 Database Provider: SQLite | Connection: {connectionString}");
                options.UseSqlite(connectionString);
            }

            // Suppress pending model changes warning - safe for development
            options.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<IDataSeedingService, DataSeedingService>();

        // Windows - Transient so we get a new instance each time
        services.AddTransient<MainWindow>();
        services.AddTransient<DatabaseSetupWindow>();
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


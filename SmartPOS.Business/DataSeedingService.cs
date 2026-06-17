using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPOS.Data;
using SmartPOS.Models;
using SmartPOS.Services;
using BCrypt.Net;

namespace SmartPOS.Business;

/// <summary>
/// Service for seeding initial and sample data
/// </summary>
public class DataSeedingService : IDataSeedingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(ApplicationDbContext context, ILogger<DataSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedInitialDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting initial data seeding...");

            await SeedRolesAsync();
            await SeedAdminUserAsync();

            _logger.LogInformation("Initial data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed initial data");
            throw;
        }
    }

    public async Task SeedSampleDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting sample data seeding...");

            await SeedSampleUsersAsync();

            _logger.LogInformation("Sample data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed sample data");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new { Name = "Admin", Description = "System Administrator with full access" },
            new { Name = "Manager", Description = "Store Manager with management privileges" },
            new { Name = "Cashier", Description = "Cashier with basic POS operations access" }
        };

        foreach (var roleData in roles)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleData.Name);

            if (existingRole == null)
            {
                var role = new Role
                {
                    Name = roleData.Name,
                    Description = roleData.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Roles.Add(role);
                _logger.LogInformation("Created role: {RoleName}", roleData.Name);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedAdminUserAsync()
    {
        var adminRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        if (adminRole == null)
        {
            _logger.LogWarning("Admin role not found for user seeding");
            return;
        }

        var existingAdmin = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (existingAdmin == null)
        {
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "System Administrator",
                Email = "admin@smartpos.com",
                Phone = "+1234567890",
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created default admin user: {Username}", adminUser.Username);
        }
    }

    private async Task SeedSampleUsersAsync()
    {
        var managerRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Manager");
        var cashierRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Cashier");

        if (managerRole == null || cashierRole == null)
        {
            _logger.LogWarning("Required roles not found for sample user seeding");
            return;
        }

        var sampleUsers = new[]
        {
            new 
            { 
                Username = "manager", 
                FullName = "John Manager", 
                Email = "manager@smartpos.com", 
                Phone = "+1234567891", 
                RoleId = managerRole.Id 
            },
            new 
            { 
                Username = "cashier", 
                FullName = "Jane Cashier", 
                Email = "cashier@smartpos.com", 
                Phone = "+1234567892", 
                RoleId = cashierRole.Id 
            },
            new 
            { 
                Username = "cashier2", 
                FullName = "Bob Smith", 
                Email = "bob@smartpos.com", 
                Phone = "+1234567893", 
                RoleId = cashierRole.Id 
            }
        };

        foreach (var userData in sampleUsers)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userData.Username);

            if (existingUser == null)
            {
                var user = new User
                {
                    Username = userData.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Default password for all sample users
                    FullName = userData.FullName,
                    Email = userData.Email,
                    Phone = userData.Phone,
                    RoleId = userData.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                _logger.LogInformation("Created sample user: {Username}", userData.Username);
            }
        }

        await _context.SaveChangesAsync();
    }
}
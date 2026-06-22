# Smart POS — Service API Documentation

This document describes the services and backend contracts implemented in the application.

## 👥 Authentication & Security Services

### `IAuthenticationService`
Handles user login, logout, and password operations.

```csharp
namespace SmartPOS.Services;

public interface IAuthenticationService
{
    // Authenticates a user with username and password, returning the User object or null
    Task<User?> LoginAsync(string username, string password);

    // Logs out a user and records session close
    Task<bool> LogoutAsync(int userId);

    // Updates a user's password after validating the old password
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    // Returns a cryptographically secure hash of a password using BCrypt
    string HashPassword(string password);

    // Verifies a password against its hash
    bool VerifyPassword(string password, string passwordHash);
}
```

---

## 🗄️ System Initialization Services

### `IDataSeedingService`
Seeds roles and users on application startup if the database is newly initialized.

```csharp
namespace SmartPOS.Services;

public interface IDataSeedingService
{
    // Seeds default roles (Admin, Manager, Cashier, InventoryStaff) and the primary administrator user
    Task SeedInitialDataAsync();
    
    // Seeds dummy/sample records for development and QA testing
    Task SeedSampleDataAsync();
}
```

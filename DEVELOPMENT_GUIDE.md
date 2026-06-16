# 🛠️ Development Guide

## Smart Retail POS Management System

### Table of Contents
1. [Development Environment Setup](#development-environment-setup)
2. [Project Architecture](#project-architecture)
3. [Coding Standards](#coding-standards)
4. [Database Migrations](#database-migrations)
5. [Logging](#logging)
6. [Testing](#testing)
7. [Common Tasks](#common-tasks)

---

## Development Environment Setup

### Required Tools
- **Visual Studio 2022** (Community or higher) or **VS Code**
- **.NET 8 SDK** (or higher)
- **SQL Server LocalDB** (included with Visual Studio)
- **Git** for version control
- **Entity Framework Core CLI Tools**

### IDE Extensions (Recommended)
#### Visual Studio
- ReSharper (optional)
- CodeMaid
- Productivity Power Tools

#### VS Code
- C# Dev Kit
- .NET Extension Pack
- SQLTools

### Initial Setup
```bash
# Clone repository
git clone <repository-url>
cd Smart-POS

# Restore packages
dotnet restore

# Install EF Core tools (if not installed)
dotnet tool install --global dotnet-ef

# Build solution
dotnet build

# Run application
dotnet run --project SmartPOS.UI
```

---

## Project Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│     Presentation Layer (UI)         │  ← WPF, XAML, ViewModels
├─────────────────────────────────────┤
│     Business Logic Layer            │  ← Services, Business Rules
├─────────────────────────────────────┤
│     Data Access Layer (EF Core)     │  ← DbContext, Repositories
├─────────────────────────────────────┤
│     Domain Models Layer             │  ← Entities, DTOs
└─────────────────────────────────────┘
```

### Project Responsibilities

#### SmartPOS.UI
- **Purpose**: User interface and presentation logic
- **Dependencies**: Business, Services, Models
- **Contains**: Windows, Views, ViewModels, Converters

#### SmartPOS.Business
- **Purpose**: Business logic implementation
- **Dependencies**: Data, Services, Models
- **Contains**: Service implementations, business rules

#### SmartPOS.Data
- **Purpose**: Database access and ORM
- **Dependencies**: Models
- **Contains**: DbContext, Migrations, Configurations

#### SmartPOS.Models
- **Purpose**: Domain entities and data structures
- **Dependencies**: None (core library)
- **Contains**: Entities, DTOs, Enums

#### SmartPOS.Services
- **Purpose**: Service contracts and interfaces
- **Dependencies**: Models
- **Contains**: Interfaces only

#### SmartPOS.Reports
- **Purpose**: Report generation (future)
- **Dependencies**: Models
- **Contains**: Report templates, generators

#### SmartPOS.AI
- **Purpose**: Machine learning features (future)
- **Dependencies**: Models
- **Contains**: ML models, predictions

#### SmartPOS.Tests
- **Purpose**: Unit and integration tests
- **Dependencies**: All projects
- **Contains**: Test classes, mocks, fixtures

---

## Coding Standards

### Naming Conventions

**Classes & Interfaces:**
```csharp
// PascalCase for classes
public class ProductService { }

// PascalCase with 'I' prefix for interfaces
public interface IProductService { }
```

**Methods:**
```csharp
// PascalCase for public methods
public void SaveProduct() { }

// camelCase for private methods
private void validateProduct() { }
```

**Variables:**
```csharp
// camelCase for local variables
var productName = "Laptop";

// PascalCase for properties
public string ProductName { get; set; }

// _camelCase for private fields
private readonly ILogger _logger;
```

**Async Methods:**
```csharp
// Suffix with 'Async'
public async Task<User> GetUserAsync(int id) { }
public async Task SaveProductAsync(Product product) { }
```

### Code Organization

**Entity Example:**
```csharp
namespace SmartPOS.Models;

/// <summary>
/// Product entity representing store items
/// </summary>
public class Product : BaseEntity
{
    // Properties
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public Category? Category { get; set; }
    public int CategoryId { get; set; }
}
```

**Service Interface Example:**
```csharp
namespace SmartPOS.Services;

/// <summary>
/// Interface for product-related operations
/// </summary>
public interface IProductService
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}
```

**Service Implementation Example:**
```csharp
namespace SmartPOS.Business;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        ApplicationDbContext context,
        ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            throw;
        }
    }
}
```

### Best Practices

1. **Use async/await consistently**
   ```csharp
   // Good
   public async Task<User> GetUserAsync(int id)
   {
       return await _context.Users.FindAsync(id);
   }
   
   // Avoid
   public User GetUser(int id)
   {
       return _context.Users.Find(id);
   }
   ```

2. **Dispose resources properly**
   ```csharp
   // DbContext is scoped and auto-disposed
   // Manual disposal only if creating outside DI
   using var context = new ApplicationDbContext(options);
   ```

3. **Use null-coalescing and null-conditional operators**
   ```csharp
   var name = user?.Name ?? "Unknown";
   var email = user?.Email?.ToLower();
   ```

4. **Prefer LINQ over loops**
   ```csharp
   // Good
   var activeUsers = users.Where(u => u.IsActive).ToList();
   
   // Avoid
   var activeUsers = new List<User>();
   foreach (var user in users)
   {
       if (user.IsActive)
           activeUsers.Add(user);
   }
   ```

---

## Database Migrations

### Creating Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project SmartPOS.Data --startup-project SmartPOS.UI

# Example: Add Product table
dotnet ef migrations add AddProductTable --project SmartPOS.Data --startup-project SmartPOS.UI
```

### Applying Migrations

```bash
# Update to latest migration
dotnet ef database update --project SmartPOS.Data --startup-project SmartPOS.UI

# Update to specific migration
dotnet ef database update MigrationName --project SmartPOS.Data --startup-project SmartPOS.UI
```

### Removing Migrations

```bash
# Remove last migration (if not applied)
dotnet ef migrations remove --project SmartPOS.Data --startup-project SmartPOS.UI
```

### Viewing Migration SQL

```bash
# Generate SQL script
dotnet ef migrations script --project SmartPOS.Data --startup-project SmartPOS.UI
```

### Migration Best Practices

1. **One logical change per migration**
2. **Descriptive migration names**
3. **Test migrations before committing**
4. **Never modify applied migrations**
5. **Use FluentAPI for complex configurations**

---

## Logging

### Configuration

Logging is configured in `App.xaml.cs`:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/smartpos-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Usage

**Constructor Injection:**
```csharp
public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }
}
```

**Log Levels:**
```csharp
// Information - general info
_logger.LogInformation("User {UserId} logged in", userId);

// Warning - unexpected but handled
_logger.LogWarning("Product {ProductId} stock is low", productId);

// Error - exceptions and errors
_logger.LogError(ex, "Failed to save product {ProductId}", productId);

// Debug - detailed debugging info
_logger.LogDebug("Retrieving products with filter: {Filter}", filter);
```

### Log File Location

```
SmartPOS.UI/bin/Debug/net10.0-windows/logs/
└── smartpos-20260615.txt
```

---

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test SmartPOS.Tests

# Run with detailed output
dotnet test --verbosity detailed
```

### Writing Unit Tests

```csharp
using Xunit;
using Moq;

namespace SmartPOS.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenExists()
    {
        // Arrange
        var mockContext = new Mock<ApplicationDbContext>();
        var service = new ProductService(mockContext.Object, mockLogger);
        
        // Act
        var result = await service.GetByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
}
```

---

## Common Tasks

### Adding a New Entity

1. **Create Model:**
   ```csharp
   // SmartPOS.Models/Product.cs
   public class Product : BaseEntity
   {
       public required string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Update DbContext:**
   ```csharp
   // SmartPOS.Data/ApplicationDbContext.cs
   public DbSet<Product> Products { get; set; }
   ```

3. **Create Migration:**
   ```bash
   dotnet ef migrations add AddProduct --project SmartPOS.Data --startup-project SmartPOS.UI
   ```

4. **Update Database:**
   ```bash
   dotnet ef database update --project SmartPOS.Data --startup-project SmartPOS.UI
   ```

### Adding a New Service

1. **Create Interface:**
   ```csharp
   // SmartPOS.Services/IProductService.cs
   public interface IProductService
   {
       Task<Product> GetByIdAsync(int id);
   }
   ```

2. **Implement Service:**
   ```csharp
   // SmartPOS.Business/ProductService.cs
   public class ProductService : IProductService
   {
       public async Task<Product> GetByIdAsync(int id)
       {
           // Implementation
       }
   }
   ```

3. **Register in DI:**
   ```csharp
   // SmartPOS.UI/App.xaml.cs
   services.AddScoped<IProductService, ProductService>();
   ```

### Adding a New Window

1. **Create XAML:**
   ```xml
   <!-- SmartPOS.UI/Views/ProductsWindow.xaml -->
   <Window x:Class="SmartPOS.UI.Views.ProductsWindow">
       <Grid>
           <!-- UI Content -->
       </Grid>
   </Window>
   ```

2. **Create Code-Behind:**
   ```csharp
   // SmartPOS.UI/Views/ProductsWindow.xaml.cs
   public partial class ProductsWindow : Window
   {
       public ProductsWindow()
       {
           InitializeComponent();
       }
   }
   ```

3. **Register in DI (if needed):**
   ```csharp
   services.AddTransient<ProductsWindow>();
   ```

---

## Troubleshooting

### Build Errors

**Error**: Package restore failed
```bash
# Solution
dotnet restore --force
dotnet build
```

**Error**: Migration failed
```bash
# Solution: Check connection string in appsettings.json
# Verify SQL Server is running
```

### Runtime Errors

**Error**: Cannot connect to database
- Check connection string
- Verify SQL Server LocalDB is installed
- Try: `sqllocaldb start mssqllocaldb`

**Error**: DI resolution failure
- Ensure service is registered in `App.xaml.cs`
- Check service lifetime (Singleton, Scoped, Transient)

---

## Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [WPF Documentation](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [Serilog Documentation](https://serilog.net/)

---

**Last Updated**: June 15, 2026  
**Version**: 1.0.0

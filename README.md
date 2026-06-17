# Smart Retail POS Management System

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![Phase](https://img.shields.io/badge/phase-2%20Complete-green.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)

## 📋 Project Overview

The **Smart Retail POS Management System** is a comprehensive desktop-based application designed to streamline retail store operations by managing sales, inventory, customers, suppliers, employees, reporting, and business analytics with AI-powered forecasting capabilities.

## ✅ Phase 1 - Project Foundation (COMPLETED)

### Deliverables Completed

- ✅ **WPF Application Created** - Modern desktop application structure
- ✅ **.NET 8 Configured** - Latest framework implementation
- ✅ **SQL Server Database Setup** - LocalDB for development
- ✅ **Entity Framework Core Configured** - Code-first approach with migrations
- ✅ **Git Repository Created** - Version control established
- ✅ **Dependency Injection Setup** - Service container configured
- ✅ **Logging System Setup** - Serilog integration for file logging

### Features Implemented

✅ **Database Connection** - SQL Server integration with EF Core  
✅ **Application Startup** - WPF with dependency injection  
✅ **Basic Navigation** - MainWindow with status monitoring  
✅ **Error Handling** - Comprehensive logging and exception handling

## ✅ Phase 2 - User Authentication & Authorization (COMPLETED)

### Deliverables Completed

- ✅ **Login System** - Professional authentication interface
- ✅ **User Management** - Admin interface for managing users
- ✅ **Role-Based Access Control** - Three roles: Admin, Manager, Cashier
- ✅ **Password Management** - Change password functionality
- ✅ **Secure Authentication** - BCrypt password hashing
- ✅ **Data Seeding** - Automatic initialization with default users

### Features Implemented

✅ **Login Window** - Modern UI with quick access buttons  
✅ **User Management** - Add, edit, delete, toggle users  
✅ **Role-based Features** - Admin-only features visibility  
✅ **Password Security** - BCrypt hashing and change password  
✅ **Audit Logging** - Track authentication events  
✅ **Session Management** - User context throughout application  

For detailed information about Phase 2, see [PHASE2_AUTHENTICATION.md](PHASE2_AUTHENTICATION.md)

## 🏗️ Architecture

```
SmartPOS/
│
├── SmartPOS.UI              # WPF Presentation Layer
├── SmartPOS.Business        # Business Logic Layer
├── SmartPOS.Data            # Data Access Layer (EF Core)
├── SmartPOS.Models          # Domain Models/Entities
├── SmartPOS.Services        # Service Interfaces
├── SmartPOS.Reports         # Reporting Module
├── SmartPOS.AI              # AI/ML Features Module
└── SmartPOS.Tests           # Unit Tests
```

## 🛠️ Technology Stack

### Frontend
- **C#** - Primary programming language
- **WPF** - Windows Presentation Foundation
- **XAML** - UI markup
- **.NET 8** - Application framework

### Backend
- **Entity Framework Core 10.0** - ORM
- **LINQ** - Data querying
- **BCrypt.Net** - Password hashing

### Database
- **Microsoft SQL Server** - Primary database
- **LocalDB** - Development database

### Logging
- **Serilog** - Structured logging
- **File Sink** - Rolling log files

### Security
- **BCrypt** - Password hashing
- **Role-based Access Control** - Authorization ready

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK or higher
- Visual Studio 2022 or VS Code
- SQL Server or SQL Server LocalDB
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Smart-POS
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string** (if needed)
   
   Edit `SmartPOS.UI/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartPOS;Trusted_Connection=true;TrustServerCertificate=true;"
   }
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project SmartPOS.UI
   ```

6. **Initialize the database**
   - Click "Initialize Database" button in the application
   - Or run manually: `dotnet ef database update --project SmartPOS.Data --startup-project SmartPOS.UI`

## 📊 Database Structure

### Current Tables (Phase 1)

- **Users** - User accounts and authentication
- **Roles** - User roles (Admin, Manager, Cashier, InventoryStaff)

### Seeded Roles

| ID | Role Name       | Description                   |
|----|-----------------|-------------------------------|
| 1  | Admin           | Full system access            |
| 2  | Manager         | Inventory and reporting       |
| 3  | Cashier         | Sales processing              |
| 4  | InventoryStaff  | Stock management              |

## 🔑 Key Features Implemented

### 1. Database Context
- ApplicationDbContext with EF Core
- Base entity with audit fields (CreatedAt, UpdatedAt, IsDeleted)
- Global query filters for soft delete
- Automatic timestamp updates

### 2. Dependency Injection
- Service container configuration
- Scoped services for database operations
- Singleton services for application-wide resources

### 3. Logging System
- Structured logging with Serilog
- Rolling file logs (daily)
- Debug level logging for development

### 4. Authentication Service
- IAuthenticationService interface
- BCrypt password hashing
- Login/Logout functionality
- Password change support

### 5. User Interface
- Main application window
- Database connection testing
- Database initialization
- Status monitoring

## 📁 Project Structure Details

### SmartPOS.Models
```
└── Models/
    ├── BaseEntity.cs        # Base entity with common properties
    ├── User.cs              # User entity
    └── Role.cs              # Role entity
```

### SmartPOS.Data
```
└── Data/
    ├── ApplicationDbContext.cs              # Main DbContext
    ├── ApplicationDbContextFactory.cs       # Design-time factory
    └── Migrations/                          # EF Core migrations
```

### SmartPOS.Business
```
└── Business/
    └── AuthenticationService.cs    # Authentication implementation
```

### SmartPOS.Services
```
└── Services/
    └── IAuthenticationService.cs   # Authentication interface
```

### SmartPOS.UI
```
└── UI/
    ├── App.xaml                    # Application definition
    ├── App.xaml.cs                 # DI configuration
    ├── MainWindow.xaml             # Main window UI
    ├── MainWindow.xaml.cs          # Main window logic
    └── appsettings.json            # Configuration file
```

## 🔐 Security Implementation

- **Password Hashing**: BCrypt with salt
- **Role-Based Access Control**: Ready for implementation
- **SQL Injection Protection**: Parameterized queries via EF Core
- **Soft Delete**: Logical deletion with query filters

## 📝 Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ApplicationSettings": {
    "AppName": "Smart Retail POS Management System",
    "Version": "1.0.0"
  }
}
```

## 🧪 Testing

Run tests using:
```bash
dotnet test
```

## 📊 Performance Targets

- ✅ Login < 2 seconds
- ✅ Product Search < 1 second (upcoming)
- ✅ Checkout < 3 seconds (upcoming)

## 🗺️ Next Steps (Phase 2)

- [ ] User login screen
- [ ] User registration and management
- [ ] Role-based authorization
- [ ] Remember me functionality
- [ ] Session management

## 🤝 Contributing

This is a structured development project following a 16-phase roadmap. Each phase builds upon the previous one.

## 📄 License

This project is for educational and commercial purposes.

## 📞 Support

For issues and questions, please create an issue in the repository.

---

**Phase 1 Status**: ✅ **COMPLETED**  
**Next Phase**: Phase 2 - User Authentication & Authorization

Built with ❤️ using C# and .NET 8
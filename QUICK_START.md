# 🚀 Quick Start Guide

## Smart Retail POS Management System - Phase 1

### Prerequisites Check
- [ ] .NET 8 SDK installed
- [ ] Visual Studio 2022 or VS Code
- [ ] SQL Server LocalDB installed

### 5-Minute Setup

#### 1️⃣ Build the Solution
```bash
dotnet build
```

#### 2️⃣ Run the Application
```bash
dotnet run --project SmartPOS.UI
```

#### 3️⃣ Initialize Database
In the application window:
1. Click **"Initialize Database"** button
2. Wait for confirmation message
3. Database is now ready!

### Verify Installation

✅ **Application launches** - You should see the main window  
✅ **Database connection** - Click "Test Database Connection" button  
✅ **Logs created** - Check `/SmartPOS.UI/bin/Debug/net10.0-windows/logs/` folder  

### Project Structure Overview

```
SmartPOS/
├── 📱 SmartPOS.UI          ← Start here (Main application)
├── 💼 SmartPOS.Business     ← Business logic
├── 🗄️  SmartPOS.Data        ← Database access
├── 📦 SmartPOS.Models       ← Data models
├── 🔧 SmartPOS.Services     ← Service interfaces
├── 📊 SmartPOS.Reports      ← Reports (Phase 11)
├── 🤖 SmartPOS.AI           ← AI features (Phase 16)
└── 🧪 SmartPOS.Tests        ← Unit tests
```

### Connection String (Default)

```
Server=(localdb)\mssqllocaldb;
Database=SmartPOS;
Trusted_Connection=true;
TrustServerCertificate=true;
```

Located in: `SmartPOS.UI/appsettings.json`

### Common Commands

**Build:**
```bash
dotnet build
```

**Run:**
```bash
dotnet run --project SmartPOS.UI
```

**Run Tests:**
```bash
dotnet test
```

**Create Migration:**
```bash
dotnet ef migrations add MigrationName --project SmartPOS.Data --startup-project SmartPOS.UI
```

**Update Database:**
```bash
dotnet ef database update --project SmartPOS.Data --startup-project SmartPOS.UI
```

**Clean Build:**
```bash
dotnet clean
dotnet build
```

### Default Database Structure

After initialization, you'll have:

**Roles Table:**
- Admin
- Manager  
- Cashier
- InventoryStaff

**Users Table:**
- Empty (ready for Phase 2)

### What's Working?

✅ Application launches  
✅ Database connection  
✅ Database initialization  
✅ Logging to files  
✅ Dependency injection  
✅ Error handling  

### Next Steps

📖 Read: `README.md` - Comprehensive documentation  
📋 Review: `PHASE1_COMPLETION.md` - Detailed completion report  
🎯 Next: **Phase 2 - User Authentication & Authorization**

### Troubleshooting

**Problem**: Cannot connect to LocalDB  
**Solution**: Install SQL Server Express or use a different connection string

**Problem**: Build fails  
**Solution**: Run `dotnet restore` then `dotnet build`

**Problem**: Migration fails  
**Solution**: Check connection string in `appsettings.json`

### Support

For detailed information, see:
- `README.md` - Full documentation
- `PHASE1_COMPLETION.md` - Phase 1 details

---

**Phase 1 Status**: ✅ Complete  
**Ready for**: Phase 2 Development

🎉 **Congratulations! Your foundation is solid and ready for building the complete POS system!**

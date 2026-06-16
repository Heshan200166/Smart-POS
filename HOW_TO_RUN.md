# 🚀 How to Run Smart POS Application

## Quick Methods to See Your Work

### **Method 1: Terminal Command (Easiest)**

Open terminal in the project folder and run:

```bash
dotnet run --project SmartPOS.UI
```

This will:
1. Build the solution
2. Launch the WPF application window
3. Show you the main interface

---

### **Method 2: Visual Studio**

If using Visual Studio:

1. Open `SmartPOS.slnx` (double-click the file)
2. Set `SmartPOS.UI` as startup project (right-click → Set as Startup Project)
3. Press **F5** or click ▶️ **Start** button

---

### **Method 3: VS Code**

If using VS Code:

1. Open the folder in VS Code
2. Open terminal (Ctrl + `)
3. Run: `dotnet run --project SmartPOS.UI`

---

## 🖥️ What You'll See

When the application launches, you'll see:

```
┌─────────────────────────────────────────────┐
│  Smart Retail POS Management System         │
│  Version 1.0.0 - Phase 1: Project Foundation│
├─────────────────────────────────────────────┤
│                                             │
│  🚀 System Status                           │
│                                             │
│  ✓ Application Initialized                  │
│  ✓ Dependency Injection Configured          │
│  ✓ Logging System Active                    │
│  ⏳ Database Connection Pending              │
│                                             │
│  [Test Database Connection]                 │
│  [Initialize Database]                      │
│                                             │
└─────────────────────────────────────────────┘
```

---

## 📋 Step-by-Step First Run

### **Step 1: Run the Application**
```bash
dotnet run --project SmartPOS.UI
```

### **Step 2: Test Database Connection**
- Click **"Test Database Connection"** button
- You should see: ✓ Database Connected Successfully

### **Step 3: Initialize Database**
- Click **"Initialize Database"** button
- This creates tables and seeds the 4 default roles
- Wait for confirmation message

### **Step 4: Verify Success**
- All checkmarks should be green ✓
- Status bar shows "Database initialized successfully!"

---

## 🛠️ All Available Commands

### Build Commands
```bash
# Build entire solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Clean build
dotnet clean
dotnet build
```

### Run Commands
```bash
# Run the application
dotnet run --project SmartPOS.UI

# Run with verbose output
dotnet run --project SmartPOS.UI --verbosity detailed
```

### Database Commands
```bash
# Create new migration
dotnet ef migrations add MigrationName --project SmartPOS.Data --startup-project SmartPOS.UI

# Update database
dotnet ef database update --project SmartPOS.Data --startup-project SmartPOS.UI

# View migration SQL
dotnet ef migrations script --project SmartPOS.Data --startup-project SmartPOS.UI
```

### Test Commands
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed
```

---

## 📁 Where to Find Things

### Logs
```
SmartPOS.UI/bin/Debug/net10.0-windows/logs/
└── smartpos-20260615.txt (today's log file)
```

### Database
The database is automatically created in SQL Server LocalDB:
- **Database Name**: SmartPOS
- **Connection**: (localdb)\mssqllocaldb

### Configuration
```
SmartPOS.UI/appsettings.json
```

---

## 🐛 Troubleshooting

### **Problem**: Application doesn't start

**Solution 1**: Check .NET SDK
```bash
dotnet --version
# Should show 8.0 or higher
```

**Solution 2**: Restore packages
```bash
dotnet restore
dotnet build
```

---

### **Problem**: Database connection fails

**Solution 1**: Check if SQL Server LocalDB is installed
```bash
sqllocaldb info
```

**Solution 2**: Start LocalDB
```bash
sqllocaldb start mssqllocaldb
```

**Solution 3**: Use different connection string
Edit `SmartPOS.UI/appsettings.json` if you have SQL Server installed elsewhere

---

### **Problem**: Build errors

**Solution**: Clean and rebuild
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## 🎯 What to Do After Running

1. ✅ **Test Database Connection** - Verify connectivity
2. ✅ **Initialize Database** - Create tables and seed data
3. ✅ **Check Logs** - View application logs in `/logs` folder
4. ✅ **Review Code** - Browse the projects and see the architecture
5. ✅ **Read Documentation** - Check out all the `.md` files

---

## 📖 Next Steps

Once you've verified everything works:

1. **Read**: [README.md](README.md) - Full documentation
2. **Learn**: [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md) - How to develop
3. **Plan**: [PHASE_STATUS.md](PHASE_STATUS.md) - See the roadmap

---

## 💡 Quick Tips

### To keep the app running:
The application will stay open until you close the window.

### To stop the app:
- Close the WPF window
- Or press `Ctrl + C` in terminal

### To see live logs:
While the app is running, open another terminal and:
```bash
# Windows
type SmartPOS.UI\bin\Debug\net10.0-windows\logs\smartpos-*.txt

# Or use a text editor to open the log file
```

---

## 🎉 Expected Results

After successfully running:

✅ WPF window opens (< 2 seconds)  
✅ Database connects successfully  
✅ Database initializes with tables  
✅ Logs are created  
✅ No errors in console  

---

## 🆘 Still Having Issues?

1. Check all `.md` documentation files
2. Verify all prerequisites are installed
3. Ensure you're in the correct directory
4. Try rebuilding: `dotnet clean && dotnet build`

---

**Ready to run?** Execute this command:

```bash
dotnet run --project SmartPOS.UI
```

🚀 **Enjoy your Smart POS Application!**

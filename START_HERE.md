# 🎯 START HERE - Quick Run Guide

## ⚠️ Important: Don't Use Git Bash!

**Git Bash DOES NOT support WPF applications.**

You must use **PowerShell** or **Command Prompt** instead!

---

## ✅ How to Run (Pick One Method)

### **Method 1: Double-Click Batch File (EASIEST) ⭐**

1. Navigate to: `D:\Smart-POS\Smart-POS\`
2. Find file: `RUN_APPLICATION.bat`
3. **Double-click it**
4. Wait for the application to open
5. Done! ✨

---

### **Method 2: PowerShell Command (Recommended)**

1. **Close Git Bash** if open
2. **Open PowerShell**:
   - Right-click on Desktop → Open PowerShell here
   - OR Search for "PowerShell" in Start menu
3. **Type this command**:
   ```powershell
   cd D:\Smart-POS\Smart-POS
   dotnet run --project SmartPOS.UI
   ```
4. **Press Enter**
5. Application launches! 🎉

---

### **Method 3: Command Prompt (cmd.exe)**

1. **Close Git Bash** if open
2. **Press**: `Win + R`
3. **Type**: `cmd`
4. **Press**: Enter
5. **Type this**:
   ```cmd
   cd D:\Smart-POS\Smart-POS
   dotnet run --project SmartPOS.UI
   ```
6. **Press Enter**
7. Application launches! 🎉

---

## 🔄 NEW FLOW: What to Expect (3 Steps)

### **STEP 1: Database Setup Window (First Screen) 🗄️**

When you launch the app, you'll see the **Database Setup & Initialization** window FIRST.

**What you'll see:**
```
┌──────────────────────────────────────────────────────────┐
│ Smart POS - Database Setup & Initialization             │
├──────────────────────────────────────────────────────────┤
│                                                          │
│ 🚀 System Status                                         │
│                                                          │
│ ✓ Application Initialized                               │
│ ✓ Dependency Injection Configured                       │
│ ✓ Logging System Active                                 │
│ ⏳ Database Connection Pending                           │
│                                                          │
│   [ Test Database Connection ]  [ Initialize Database ] │
│   [ ⚙️ Configure Database ]                              │
│                                                          │
│   [ ✅ Continue to Login ] ← Click this after setup     │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

**What to do:**
1. Click **"Initialize Database"** button
2. Wait for progress window (Creating → Migrating → Verifying → Complete!)
3. See success message: "🎉 Database initialization complete!"
4. The **"✅ Continue to Login"** button turns green
5. Click **"✅ Continue to Login"**

---

### **STEP 2: Login Window (Second Screen) 🔐**

After database setup, the **Login Window** appears.

**Default credentials:**
- 👤 Username: `admin`
- 🔑 Password: `admin123`

**Other test accounts:**
- Manager: `manager` / `manager123`
- Cashier: `cashier` / `cashier123`

**What you'll see:**
```
┌─────────────────────────────────────┐
│    Smart Retail POS System          │
│    🔐 User Login                    │
├─────────────────────────────────────┤
│                                     │
│  Username: [admin____________]      │
│  Password: [••••••••_________]      │
│                                     │
│         [ 🔓 LOGIN ]                │
│                                     │
│  Quick Access (Dev):                │
│  [Admin] [Manager] [Cashier]        │
│                                     │
└─────────────────────────────────────┘
```

**What to do:**
1. Type username: `admin`
2. Type password: `admin123`
3. Click **Login** or press **Enter**

---

### **STEP 3: Main Application (Third Screen) 🎉**

After successful login, the **Main Smart POS Application** opens!

**What you'll see:**
```
┌────────────────────────────────────────────────────────────┐
│ Smart Retail POS Management System                        │
│ Version 1.0.0 - Phase 2: User Authentication              │
│                           👤 Admin User (Admin) [ 🚪 Logout ] │
├────────────────────────────────────────────────────────────┤
│                                                            │
│ 🚀 System Status                                           │
│                                                            │
│ ✓ Application Initialized Successfully                     │
│ ✓ Dependency Injection Configured                          │
│ ✓ Logging System Active                                    │
│ ✓ Database Connected Successfully                          │
│                                                            │
│ 👥 Administrative Functions                                │
│    [ 👥 Manage Users ]                                     │
│    [ 🔒 Manage Roles ]                                     │
│    [ 🔑 Change Password ]                                  │
│                                                            │
│ Phase 2 Authentication Complete ✅                         │
└────────────────────────────────────────────────────────────┘
```

**You can now:**
- ✅ Manage users (Admin only)
- ✅ Change your password
- ✅ View system status
- ✅ Access full POS features
- ✅ Logout when done

---

## 📝 Complete Example Session

```powershell
PS D:\Smart-POS\Smart-POS> dotnet run --project SmartPOS.UI

🚀 SmartPOS Application Starting...
📂 Working Directory: D:\Smart-POS\Smart-POS
✅ Logging configured
✅ Configuration loaded
✅ Dependency injection configured
🗄️ Showing database setup window...

[Database Setup Window Opens]
[You click "Initialize Database"]
[Progress: Creating database... 20%]
[Progress: Applying migrations... 60%]
[Progress: Verifying setup... 90%]
[Progress: Complete! 100%]
[Success message shown]
[You click "✅ Continue to Login"]

� Initializing database and default users...
✅ Database ready
✅ Default users created (admin/admin123)
🔐 Showing login window...

[Login Window Opens]
[You type: admin / admin123]
[You click Login]

✅ User logged in: admin (Admin)
📱 Creating main window...
📱 Showing main window...
✅ Main window shown

🎉 Application started successfully!
Welcome, Admin User!
🪟 Smart POS window should now be visible.

[Main Application Window is now open and ready!]
```

---

## 🔄 Subsequent Runs (After First Time)

On future runs, the flow is the same but faster:

1. **Database Setup Window** opens
   - Shows: "✓ Database Connected Successfully"
   - Click **"✅ Continue to Login"** (no need to reinitialize)

2. **Login Window** appears
   - Enter your credentials

3. **Main Application** opens
   - Ready to use!

---

## ✅ Quick Reference

### Default Login Credentials
| Username | Password | Role | Access Level |
|----------|----------|------|--------------|
| admin | admin123 | Admin | Full system access |
| manager | manager123 | Manager | Management functions |
| cashier | cashier123 | Cashier | POS operations only |

### Important Files
- **Database**: `D:\Smart-POS\Smart-POS\SmartPOS.db` (auto-created)
- **Logs**: `D:\Smart-POS\Smart-POS\logs\smartpos-YYYYMMDD.txt`
- **Config**: `D:\Smart-POS\Smart-POS\appsettings.json`

---

## 🐛 Troubleshooting

### **Problem: "Continue to Login" button is grayed out**
**Solution**: Click **"Initialize Database"** first to create the database.

### **Problem: Login fails with "User not found"**
**Solution**: 
- Go back to database setup and click "Initialize Database"
- Make sure you typed: `admin` / `admin123` (case-sensitive)
- Check console for error messages

### **Problem: Nothing happens after login**
**Solution**:
- Check console output for errors
- Look in `logs/` folder for detailed error messages
- Restart application and try again

### **Problem: Application won't start**
**Solution**:
- Make sure you're NOT using Git Bash
- Use PowerShell or Command Prompt
- Verify .NET 10.0 SDK is installed: `dotnet --version`
- Run from correct directory: `D:\Smart-POS\Smart-POS`

### **Problem: "Build failed"**
**Solution**:
```powershell
dotnet clean
dotnet build
dotnet run --project SmartPOS.UI
```

---

## 📚 Additional Documentation

- 📖 **NEW_STARTUP_FLOW.md** - Complete detailed flow documentation
- 📖 **PHASE2_AUTHENTICATION.md** - User authentication system details
- 📖 **DATABASE_SETUP_GUIDE.md** - Database configuration help
- 📖 **DEVELOPMENT_GUIDE.md** - Development information
- 📖 **README.md** - Full project documentation

---

## 🎉 You're All Set!

Your **Smart Retail POS Management System** is ready with the new flow:

```
╔══════════════════════════════════════════╗
║  NEW 3-STEP FLOW:                        ║
║                                          ║
║  1️⃣ Database Setup Window               ║
║     ↓ Initialize & Continue              ║
║                                          ║
║  2️⃣ Login Window                         ║
║     ↓ Enter credentials                  ║
║                                          ║
║  3️⃣ Main Application                     ║
║     ↓ Full POS System Ready!             ║
║                                          ║
║     🚀 LET'S GO! 🚀                      ║
╚══════════════════════════════════════════╝
```

**To run:**
1. Close Git Bash (if open)
2. Open PowerShell
3. `cd D:\Smart-POS\Smart-POS`
4. `dotnet run --project SmartPOS.UI`
5. Follow the 3-step flow above!

---

**Last Updated**: June 22, 2026  
**Status**: ✅ New Flow Implemented  
**Version**: 1.0.0 - Phase 2 (SQLite with Authentication)

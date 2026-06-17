# Phase 2 Quick Start Guide

## 🚀 Getting Started with Authentication

### Step 1: Run the Application

**Option A: Using PowerShell**
```powershell
cd D:\Smart-POS\Smart-POS
dotnet run --project SmartPOS.UI
```

**Option B: Using Batch File**
```bash
Double-click: DEBUG_RUN.bat
```

**Option C: Using Visual Studio**
1. Open SmartPOS.slnx
2. Set SmartPOS.UI as startup project
3. Press F5

### Step 2: Login

When the application starts, you'll see the **Login Window**.

**Quick Access (Easiest)**:
- Click the **Admin** button (red) → logs in as admin
- Click the **Manager** button (orange) → logs in as manager
- Click the **Cashier** button (green) → logs in as cashier

**Manual Login**:
- Username: `admin`
- Password: `admin123`
- Click **LOGIN**

### Step 3: Explore the Main Window

After successful login, you'll see the **Main Window** displaying:
- Your name and role (top-right corner)
- System status indicators
- Database connection status
- Role-specific features

## 👥 Using User Management (Admin Only)

### Accessing User Management
1. Login as **Admin**
2. Click **"👥 Manage Users"** button
3. The User Management window opens

### Managing Users

**Add a New User**:
1. Click **"➕ Add User"** button
2. Fill in the form:
   - Username (required)
   - Full Name (required)
   - Email (optional)
   - Phone (optional)
   - Role (required)
   - Password (required, min 6 characters)
3. Click **"💾 Save User"**

**Edit a User**:
1. Find the user in the list
2. Click **"✏️"** button (edit)
3. Modify the information
4. Click **"💾 Update User"**

**Deactivate a User**:
1. Find the user in the list
2. Click **"⏸️"** button (pause icon for active users)
3. User status changes to Inactive
4. Click **"▶️"** to reactivate

**Reset Password**:
1. Find the user in the list
2. Click **"🔑"** button (key icon)
3. Password resets to: `admin123`

**Search Users**:
1. Type in the search box
2. List filters by username, full name, or email
3. Clear search to see all users

## 🔑 Changing Your Password

### Steps to Change Password
1. Click **"🔑 Change Password"** button
2. Enter your **Current Password**
3. Enter your **New Password** (must be:
   - At least 6 characters
   - Different from current password
   - Confirmed in next field
4. Click **"💾 Change Password"**
5. On success, you'll need to login again with new password

## 🚪 Logging Out

### To Logout
1. Click **"🚪 Logout"** button (top-right corner)
2. You'll return to the login screen
3. Login with a different account if desired

## 🔍 Understanding Roles

### Admin Role
- Full system access
- Can manage users and roles
- Can view all features
- Can perform administrative tasks
- Sees "Administrative Functions" section

### Manager Role
- Store management privileges
- Cannot manage users
- Limited access to reports
- Can manage inventory and sales

### Cashier Role
- Basic POS operations
- Can process sales
- Can view inventory
- Cannot manage users or configuration

## 📝 Testing Scenarios

### Scenario 1: Create a New User
1. Login as Admin (click Admin button)
2. Click "👥 Manage Users"
3. Click "➕ Add User"
4. Enter details:
   - Username: `testuser`
   - Full Name: `Test User`
   - Role: `Cashier`
   - Password: `testpass123`
5. Click Save
6. Logout
7. Login with new credentials: `testuser` / `testpass123`

### Scenario 2: Test Role Restrictions
1. Login as Admin → See admin panel with management buttons
2. Logout
3. Login as Manager → Admin panel is NOT visible
4. Logout
5. Login as Cashier → Admin panel is NOT visible

### Scenario 3: Change Password
1. Login as any user
2. Click "🔑 Change Password"
3. Enter current password
4. Enter new password twice
5. Click "💾 Change Password"
6. Logout
7. Login with new password (should succeed)

### Scenario 4: Manage User Status
1. Login as Admin
2. Click "👥 Manage Users"
3. Find "manager" user
4. Click "⏸️" to deactivate
5. Try logging in as manager → Should fail (inactive user)
6. As Admin, click "▶️" to reactivate
7. Try logging in as manager → Should succeed

## 🐛 Troubleshooting

### "Login Failed" Error
- Check username spelling
- Verify password is correct (default: `admin123`)
- Ensure user is Active (not deactivated)

### "Access Denied" Error
- You don't have permission for this feature
- This is role-based - only Admins can manage users
- Login as Admin to access user management

### Database Connection Error
1. Click "Test Database Connection"
2. Click "Initialize Database"
3. Check LocalDB is installed
4. See DATABASE_SETUP_GUIDE.md for help

### Window Appears Behind Others
- Press Alt+Tab to find the application window
- Or click the taskbar icon

## 📊 Default Test Accounts

| Username | Password   | Role    | Full Name                 |
|----------|-----------|---------|---------------------------|
| admin    | admin123  | Admin   | System Administrator      |
| manager  | admin123  | Manager | John Manager              |
| cashier  | admin123  | Cashier | Jane Cashier              |
| cashier2 | admin123  | Cashier | Bob Smith                 |

## 📚 More Information

- **[PHASE2_AUTHENTICATION.md](PHASE2_AUTHENTICATION.md)** - Complete feature documentation
- **[PHASE2_COMPLETION.md](PHASE2_COMPLETION.md)** - Detailed implementation summary
- **[README.md](README.md)** - Project overview
- **[DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md)** - Development setup

## ✨ Key Features Implemented

✅ Modern login interface with validation  
✅ BCrypt secure password hashing  
✅ Role-based access control (3 roles)  
✅ User management interface  
✅ Password change functionality  
✅ Automatic database seeding  
✅ Audit logging of authentication events  
✅ Session management  
✅ Beautiful, user-friendly UI  

## 🎯 What's Next?

Phase 3 will add:
- Product Management (CRUD operations)
- Barcode scanning
- Category management
- Supplier management
- Inventory tracking

Enjoy exploring Phase 2! 🎉
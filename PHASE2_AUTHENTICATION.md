# Phase 2 - User Authentication & Authorization

## Overview
Phase 2 implements comprehensive user authentication and role-based access control (RBAC) for the Smart POS system. Users must authenticate before accessing the main application, and features are restricted based on user roles.

## Completed Features

### 1. Login System
- **LoginWindow.xaml/cs**: Professional login interface with:
  - Username and password fields
  - Remember Me checkbox
  - Quick access buttons for development (Admin, Manager, Cashier roles)
  - Real-time validation and error messages
  - Keyboard navigation support (Tab, Enter)

### 2. Authentication Service
- **AuthenticationService.cs**: Core authentication logic
  - `LoginAsync()` - Authenticate user with credentials
  - `LogoutAsync()` - Handle user logout
  - `ChangePasswordAsync()` - Allow users to change their password
  - `HashPassword()` - Secure password hashing using BCrypt
  - `VerifyPassword()` - Verify user credentials

### 3. User Management
- **UserManagementWindow.xaml/cs**: Admin interface to manage users
  - View all users in a sortable/searchable list
  - Add new users with role assignment
  - Edit existing user information
  - Toggle user active/inactive status
  - Reset user password to default
  - Search users by username, full name, or email

### 4. Change Password
- **ChangePasswordWindow.xaml/cs**: Allow users to change their own password
  - Verify current password
  - Validate new password requirements (6+ characters)
  - Prevent reusing current password
  - Password confirmation matching

### 5. Role-Based Access Control
- **Three predefined roles**:
  - **Admin**: Full system access including user management
  - **Manager**: Store management privileges
  - **Cashier**: Basic POS operations only
- **Role-based UI**: Admin panel only shows for Admin users

### 6. Data Seeding
- **DataSeedingService.cs**: Automatic initialization
  - Creates default roles (Admin, Manager, Cashier)
  - Creates default admin user (username: admin, password: admin123)
  - Creates sample users for testing (manager, cashier, cashier2)
  - All sample users have password: admin123

### 7. Security Features
- **Secure password storage**: BCrypt hashing algorithm
- **Session management**: User info stored in window context
- **Audit logging**: Login/logout activities logged
- **Access control**: Role-based feature visibility

## Database Schema

### Users Table
```
- Id (Primary Key)
- Username (Unique, Required)
- PasswordHash (Required)
- FullName (Required)
- Email (Optional)
- Phone (Optional)
- RoleId (Foreign Key to Roles)
- IsActive (Boolean, Default: true)
- LastLoginAt (DateTime, Nullable)
- CreatedAt (DateTime)
- UpdatedAt (DateTime, Nullable)
- IsDeleted (Boolean, Default: false)
```

### Roles Table
```
- Id (Primary Key)
- Name (Unique, Required)
- Description (Optional)
- CreatedAt (DateTime)
- UpdatedAt (DateTime, Nullable)
- IsDeleted (Boolean, Default: false)
```

## How to Use

### First Time Login
1. Run the application using: `dotnet run --project SmartPOS.UI`
2. Login window appears
3. Use quick access buttons or enter credentials:
   - **Default Admin**: Username: `admin`, Password: `admin123`
   - **Sample Manager**: Username: `manager`, Password: `admin123`
   - **Sample Cashier**: Username: `cashier`, Password: `admin123`

### Managing Users (Admin Only)
1. After logging in as Admin, click "👥 Manage Users" button
2. View all system users in the list
3. Actions available:
   - ✏️ Edit: Modify user details
   - ⏸️/▶️ Toggle: Activate/deactivate user
   - 🔑 Reset: Reset password to default (admin123)
4. Add new users with specific roles

### Changing Your Password
1. Click "🔑 Change Password" button
2. Enter current password
3. Enter new password (must be different and at least 6 characters)
4. Confirm new password

### Logout
1. Click "🚪 Logout" button in top-right corner
2. Application returns to login screen

## Development Notes

### Quick Access Buttons
Quick access buttons are provided for development convenience:
- **Admin**: Automatically logs in as admin
- **Manager**: Automatically logs in as manager  
- **Cashier**: Automatically logs in as cashier

All use the default password `admin123`. These are intended for development/testing only.

### Default Credentials
- **Admin Account**:
  - Username: `admin`
  - Password: `admin123`
  - Full Name: System Administrator
  - Email: admin@smartpos.com

- **Sample Accounts** (auto-created):
  - manager / admin123
  - cashier / admin123
  - cashier2 / admin123

### Database Initialization
On first run, the DataSeedingService automatically:
1. Checks if roles exist; creates them if needed
2. Checks if admin user exists; creates with default credentials if needed
3. Creates sample users for testing

### Logging
All authentication events are logged:
- Successful logins with user and role
- Failed login attempts
- Password changes
- User management operations

## Architecture

### Components
- **LoginWindow**: Entry point for authentication
- **AuthenticationService**: Core business logic
- **UserManagementWindow**: Admin interface
- **ChangePasswordWindow**: Password management
- **DataSeedingService**: Database initialization
- **LoggerWrapper**: Dependency injection helper

### Authentication Flow
1. Application starts → App.xaml.cs
2. LoginWindow shows → User enters credentials
3. AuthenticationService.LoginAsync() validates
4. On success → MainWindow opens with user context
5. UI updates based on user's role
6. User can access role-specific features

### Security Considerations
- Passwords stored as BCrypt hashes (not reversible)
- LastLoginAt updated after successful authentication
- Failed attempts logged without storing credentials
- User context stored in window Tag property
- Role-based feature visibility (not just hidden, but permission-checked)

## Testing Checklist

- [ ] Login with valid admin credentials
- [ ] Login with invalid credentials (should fail)
- [ ] Login with quick access buttons
- [ ] Admin sees user management buttons
- [ ] Non-admin users don't see admin buttons
- [ ] Add new user successfully
- [ ] Edit user details
- [ ] Toggle user status
- [ ] Reset user password
- [ ] Change own password
- [ ] Logout and login with new password
- [ ] Search users by name/email
- [ ] Remember Me checkbox functionality

## Future Enhancements

Phase 3+ could include:
- Two-factor authentication
- Password expiration policies
- Session timeout
- Concurrent login prevention
- User audit trail
- Permission-based feature access (beyond roles)
- LDAP/Active Directory integration
- OAuth2/OpenID Connect integration

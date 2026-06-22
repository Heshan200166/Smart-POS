# Smart POS — User Guide

Welcome to the **Smart Retail POS Management System**. This guide provides an overview of how to set up, log in, and use the application.

## 🗄️ Initial Setup
When starting the application for the first time:
1. **Database Setup Window**: You will be prompted to configure the database.
2. Select either **SQLite** (single-file database, recommended for easy setup/demo) or **SQL Server**.
3. Click **Test Connection** to verify settings.
4. Click **Initialize Database** to create tables and seed default data.
5. Click **Continue to Login** once setup is ready.

---

## 🔑 Default Test Credentials
All passwords are set to `admin123`.

| Username | Role | Full Name | Purpose |
|----------|------|-----------|---------|
| `admin` | Admin | System Administrator | Managing users, full settings access |
| `manager` | Manager | John Manager | Store operations and reporting |
| `cashier` | Cashier | Jane Cashier | Sales checkout and transactions |
| `cashier2` | Cashier | Bob Smith | Secondary cashier user |

---

## 👥 Managing Users (Admin Only)
Administrators can manage users by:
1. Logging in as `admin` (`admin123`).
2. Clicking **👥 Manage Users** in the Admin panel.
3. Adding new users, editing details, resetting passwords, or activating/deactivating accounts.

---

## 🔑 Changing Password
Any logged-in user can change their password:
1. Click **🔑 Change Password** in the toolbar.
2. Enter the current password, followed by the new password twice.
3. Click **Change Password** to save.

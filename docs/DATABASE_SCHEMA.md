# Smart POS — Database Schema

This document details the database schema for the Smart Retail POS Management System.

## Base Entity (Common Fields)
All main entities inherit from `BaseEntity` which provides auditing and soft-delete capabilities:
- `Id`: `INTEGER` (PK, Auto-increment)
- `CreatedAt`: `DATETIME` (Default: UTC Now)
- `UpdatedAt`: `DATETIME` (Nullable)
- `IsDeleted`: `BOOLEAN` (Default: `false` - for soft delete filtering)

---

## 👥 Authentication & User Management

### Roles Table
Stores system roles for Role-Based Access Control (RBAC).

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| `Id` | `int` | No | Primary Key |
| `Name` | `nvarchar(max)` / `TEXT` | No | Name of the role (Admin, Manager, Cashier, InventoryStaff) |
| `Description` | `nvarchar(max)` / `TEXT` | Yes | Description of the role |
| `CreatedAt` | `datetime` | No | Creation timestamp |
| `UpdatedAt` | `datetime` | Yes | Last update timestamp |
| `IsDeleted` | `bit` / `boolean` | No | Soft delete status |

### Users Table
Stores user accounts for POS authentication.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| `Id` | `int` | No | Primary Key |
| `Username` | `nvarchar(max)` / `TEXT` | No | Unique login username |
| `PasswordHash` | `nvarchar(max)` / `TEXT` | No | BCrypt hashed password |
| `FullName` | `nvarchar(max)` / `TEXT` | No | User's full name |
| `Email` | `nvarchar(max)` / `TEXT` | Yes | User's email address |
| `Phone` | `nvarchar(max)` / `TEXT` | Yes | User's phone number |
| `RoleId` | `int` | No | Foreign Key to Roles |
| `IsActive` | `bit` / `boolean` | No | Toggle to enable/disable login (Default: `true`) |
| `LastLoginAt` | `datetime` | Yes | Timestamp of last successful login |
| `CreatedAt` | `datetime` | No | Creation timestamp |
| `UpdatedAt` | `datetime` | Yes | Last update timestamp |
| `IsDeleted` | `bit` / `boolean` | No | Soft delete status |

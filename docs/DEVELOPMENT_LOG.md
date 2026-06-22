# Smart POS — Development Log

## 📅 June 11, 2026 - Phase 1 Complete
- **Focus**: Project Foundation
- **Milestones**:
  - Solution structure initialized with 8 distinct projects.
  - Setup WPF Presentation layer with Dependency Injection.
  - Added Serilog daily rolling file logging.
  - Configured EF Core with SQLite (switchable to SQL Server).
  - Configured design-time migration support.

## 📅 June 15, 2026 - Phase 2 Complete
- **Focus**: User Authentication & Authorization
- **Milestones**:
  - Added a responsive, styled login interface.
  - Implemented secure password hashing via BCrypt.Net.
  - Implemented User Management CRUD with interactive WPF window.
  - Added Role-Based Access Control (RBAC) with default system roles (Admin, Manager, Cashier).
  - Enabled self-service password changing with rules.
  - Configured automated database seeding for default users on first boot.

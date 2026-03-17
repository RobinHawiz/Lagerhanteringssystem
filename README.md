# Inventory Management System (ASP.NET Core MVC)

An intranet-style inventory management web app built with ASP.NET Core MVC, Entity Framework Core, and SQL Server.  
The system supports role-based access via ASP.NET Core Identity (**Admin** and **Warehouse**) and provides public read-only pages for browsing items and the employees.

## Features

- ASP.NET Core Identity (Individual Accounts) with role-based authorization
  - **Admin**: full CRUD for categories, items and employees
  - **Warehouse**: stock adjustment page (update item amount only)
- Inventory management
  - Categories (admin-only)
  - Items (public read-only + admin CRUD)
- Employee management (public read-only + admin CRUD actions)
- Dev seeding
  - Roles + demo accounts
  - Demo data for categories, employees, and items

## Tech Stack

- **ASP.NET Core MVC**
- **Entity Framework Core** + **SQL Server**
- **ASP.NET Core Identity** + role-based authorization
- **Bootstrap**

## Solution Structure

```
# Repo root
Lagerhanteringssystem.slnx
MVC/
├── Program.cs                     # Startup, EF/Identity setup, localization, dev seeding
├── appsettings.json               # SQL Server connection string
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Models/
│   ├── DbAccountSeeder.cs         # Seeds roles + demo users (dev-only)
│   ├── StockAdjust.cs             # DTO for stock adjustments
│   └── Entities/
│       ├── Item.cs
│       ├── Category.cs
│       └── Employee.cs
├── Controllers/
│   ├── HomeController.cs
│   ├── ItemsController.cs
│   ├── CategoriesController.cs
│   ├── EmployeesController.cs
│   └── StockController.cs
├── Views/
│   ├── Home/                      # Dashboard
│   ├── Items/                     # CRUD views
│   ├── Categories/                # CRUD views
│   ├── Employees/                 # CRUD views
│   └── Stock/                     # Warehouse stock adjustment view
├── Areas/Identity/                # Scaffolded Identity UI (Register/Login)
└── Migrations/                    # EF Core migrations
```

## Running Locally

### Prerequisites

- .NET 10 SDK
- SQL Server
- Git

### 1) Clone

```bash
git clone https://github.com/RobinHawiz/Lagerhanteringssystem.git
cd Lagerhanteringssystem
```

### 2) Configure database connection

Edit `MVC/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=StockManagementDb;Trusted_Connection=True;TrustServerCertificate=true"
  }
}
```

If you use another SQL Server instance (e.g. LocalDB or SQLEXPRESS), update `Server=...` accordingly.

### 3) Apply migrations

**Package Manager Console (Visual Studio):**
```powershell
Update-Database
```

**Or .NET CLI (from repo root):**
```bash
dotnet ef database update --project MVC
```

> [!NOTE]
> Seeding is configured via EF Core `UseSeeding` and runs during database creation/migration.

### 4) Run the app

From repo root:

```bash
dotnet run --project MVC
```

Or from the `MVC/` folder:

```bash
cd MVC
dotnet run
```

> [!NOTE]
> The console will print the local URL (typically `https://localhost:xxxx`).

## Demo Accounts & Roles (Development)

In Development mode, the app seeds roles and two demo accounts via `DbAccountSeeder`.

**Admin**
- Email: `admin@company.com`
- Password: `Passwordadmin1234!`
- Role: `Admin`

**Warehouse**
- Email: `warehouse@company.com`
- Password: `Passwordwarehouse1234!`
- Role: `Warehouse`

### Registration behavior

The scaffolded Register page is customized so that newly registered users are automatically assigned the **Warehouse** role after successful account creation.

The project currently uses `RequireConfirmedAccount = true`. Newly registered accounts must be confirmed on the confirmation page shown after registration before they can sign in. Seeded demo users can sign in immediately because they are created with `EmailConfirmed = true`.

## Data Structures

The application uses **SQL Server** as the database with **Entity Framework Core** as the ORM.
Below, “Validation” reflects DataAnnotations in `MVC/Models/Entities/*.cs` used by MVC model validation (via ModelState) when creating/updating data.

### `Category`

| Field  | SQL Server Type | Constraints | Description | Validation (DataAnnotations) |
| ------ | --------------- | ---------- | ----------- | ---------------------------- |
| `Id`   | `int`           | PK         | Primary key | — |
| `Name` | `nvarchar(100)` | NOT NULL   | Category name | `Required`; `StringLength(100)` |

> [!NOTE]
> Relationship: `Category (1) → Item (many)`

### `Item`

| Field         | SQL Server Type     | Constraints                  | Description | Validation (DataAnnotations) |
| ------------- | ------------------- | ---------------------------- | ----------- | ---------------------------- |
| `Id`          | `int`               | PK                           | Primary key | — |
| `Name`        | `nvarchar(100)`     | NOT NULL                     | Item name | `Required`; `StringLength(100)` |
| `Description` | `nvarchar(200)`     | NOT NULL                     | Item description | `Required`; `StringLength(200)` |
| `Price`       | `decimal(10,2)`     | NOT NULL                     | Item price | Column configured as `decimal(10,2)` |
| `Amount`      | `int`               | NOT NULL                     | Stock amount | `Range(0, int.MaxValue)` |
| `CategoryId`  | `int`               | NOT NULL, FK → `Category.Id` | Category reference | — |

> [!NOTE]
> `Category` is a navigation property in the entity model (not a separate database column) that EF Core can populate when loading related data, e.g. with `.Include(i => i.Category)`.

### `Employee`

| Field         | SQL Server Type | Constraints | Description | Validation (DataAnnotations) |
| ------------- | --------------- | ---------- | ----------- | ---------------------------- |
| `Id`          | `int`           | PK         | Primary key | — |
| `FirstName`   | `nvarchar(50)`  | NOT NULL   | First name | `Required`; `StringLength(50)` |
| `LastName`    | `nvarchar(50)`  | NOT NULL   | Last name | `Required`; `StringLength(50)` |
| `PhoneNumber` | `nvarchar(15)`  | NOT NULL   | Phone number | `Required`; `StringLength(15)` |
| `Email`       | `nvarchar(100)` | NOT NULL   | Email | `Required`; `EmailAddress`; `StringLength(100)` |

### Identity tables (generated)

Authentication/authorization is handled by ASP.NET Core Identity, which generates its own tables (e.g.):
- `AspNetUsers` (accounts)
- `AspNetRoles` (roles like Admin/Warehouse)
- `AspNetUserRoles` (user-role mapping)
- claims/logins/tokens tables

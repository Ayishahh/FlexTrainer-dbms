# Flex Trainer - Gym Management System

A comprehensive gym management database system built with C# Windows Forms and SQL Server.

![Database Systems Project](docs/diagrams/ERDPlus%20Image.png)

## 📋 Project Overview

Flex Trainer is a desktop application designed for managing gym operations including:
- **Member Management**: Registration, memberships, and profile management
- **Trainer Management**: Trainer registration with gym owner approval workflow
- **Workout Planning**: Create, share, and track exercise plans
- **Diet Planning**: Nutrition plans with meal tracking and allergen management
- **Appointment Booking**: Schedule training sessions with personal trainers
- **Feedback System**: Rate and review trainers
- **Administrative Dashboard**: Gym performance analytics and approval workflows

## 🏗️ Project Structure

```
flex-trainer/
├── src/                          # C# Application Source Code
│   ├── *.cs                      # Form code files
│   ├── *.Designer.cs             # Designer-generated code
│   ├── *.resx                    # Resource files
│   ├── App.config                # Connection string configuration
│   ├── DatabaseHelper.cs         # Centralized DB connection helper
│   └── DB_phase2_project.sln     # Visual Studio Solution
│
├── database/                     # SQL Server Scripts
│   ├── 00_SETUP_README.sql       # Setup instructions
│   ├── 01_schema.sql             # Database and tables
│   ├── 02_triggers.sql           # Audit trail triggers (30+)
│   ├── 03_procedures.sql         # Stored procedures
│   ├── 04_sample_data.sql        # Sample data (50+ users)
│   └── 05_reports.sql            # Report queries
│
├── docs/                         # Documentation
│   ├── ERD_Diagrams.md           # Mermaid ERD diagrams
│   ├── Database_Schema.md        # Schema documentation
│   ├── diagrams/                 # Visual diagrams (PNG)
│   └── excel_data/               # Sample Excel data
│
├── docker-compose.yml            # Docker SQL Server setup
├── setup_database.sh             # Database initialization script
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- **For C# Application**: Visual Studio 2019+ (Windows)
- **For Database**: Docker Desktop OR SQL Server 2019+

---

## 🐳 Option 1: Docker Setup (Recommended)

### Step 1: Start SQL Server Container

```bash
# Navigate to project directory
cd flex-trainer

# Start SQL Server in Docker
docker-compose up -d

# Wait for container to be ready (about 30 seconds)
docker logs -f flex-trainer-db
# Wait until you see "SQL Server is now ready for client connections"
# Press Ctrl+C to exit logs
```

### Step 2: Initialize Database

**Option A: Using setup script (Mac/Linux)**
```bash
./setup_database.sh
```

**Option B: Manual setup**
```bash
# 1. Execute schema
docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/01_schema.sql

# 2. Execute triggers
docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/02_triggers.sql

# 3. Execute procedures
docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/03_procedures.sql

# 4. Insert sample data
docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/04_sample_data.sql
```

### Step 3: Connection Details for C# App

```
Server: localhost,1433
Database: DB_PROJECT
Username: sa
Password: FlexTrainer2024!
```

**Connection String** (already configured in `App.config`):
```
Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True
```

### Docker Commands

```bash
# Stop container
docker-compose down

# Stop and remove data
docker-compose down -v

# View logs
docker logs flex-trainer-db

# Connect with sqlcmd
docker exec -it flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C
```

---

## 💻 Option 2: Windows SQL Server Setup

### Step 1: Install SQL Server
Download and install [SQL Server 2019 Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Step 2: Create Database
1. Open SQL Server Management Studio (SSMS)
2. Connect to your local SQL Server
3. Execute scripts in order:
   - `database/01_schema.sql`
   - `database/02_triggers.sql`
   - `database/03_procedures.sql`
   - `database/04_sample_data.sql`

### Step 3: Update Connection String
Edit `src/App.config`:
```xml
<add name="FlexTrainerDB" 
     connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=DB_PROJECT;Integrated Security=True" 
     providerName="System.Data.SqlClient" />
```

---

## 🖥️ Running the C# Application

### In Visual Studio (Windows/Parallels VM)

1. Open `src/DB_phase2_project.sln` in Visual Studio
2. Ensure App.config has correct connection string
3. Build solution: `Ctrl + Shift + B`
4. Run: `F5` or click "Start"

### Test Accounts

| Role | Username | Password |
|------|----------|----------|
| Admin | admin_user | admin123 |
| Member | john_doe | password123 |
| Trainer | sarah_trainer | trainer123 |
| Gym Owner | gym_owner1 | owner123 |

> **Note**: Check `database/04_sample_data.sql` for actual usernames and passwords in your dataset.

---

## 👥 User Roles

| Role | Description | Key Features |
|------|-------------|--------------|
| **Member** | Gym members | Create/view workout & diet plans, book trainers, submit feedback |
| **Trainer** | Personal trainers | Create plans for clients, manage appointments, view feedback |
| **Gym Owner** | Facility owners | Approve trainers, manage members, view reports |
| **Admin** | System administrators | Approve gyms, view analytics, bulk import users from Excel |

---

## 📊 Database Features

### Normalization
- Fully normalized to **Third Normal Form (3NF)**
- 20+ entities with proper relationships
- Junction tables for all many-to-many relationships

### Audit Trail
- **30+ triggers** for comprehensive logging
- All INSERT, UPDATE, DELETE operations tracked in `System_Log` table
- Archived data for deleted trainers

### Stored Procedures
- `Add_New_User` - Register new users with role-specific data
- `User_Login` - Authenticate users and return role
- `Request_Appointment` - Book training sessions
- `Delete_Gym_Procedure` - Safely remove gyms
- And more...

### Reports (20 Total)
1. Members trained by specific trainer at specific gym
2. Members following specific diet plan at gym
3. Members across gyms by trainer and diet plan
4. Machine usage count by day
5. Diet plans with <500 calorie breakfast
6. Diet plans with <300g total carbs
7. Workout plans not using specific machine
8. Diet plans without specific allergen
9. New memberships (last 3 months)
10. Gym member comparison
11-20. Additional performance and analytics reports

---

## 🔧 Key Features

### Excel Import (Admin Dashboard)
- Bulk import users from Excel files (.xlsx, .xls)
- Preview data before import
- Uses `Add_New_User` stored procedure

### Approval Workflows
- Trainer registration → Gym Owner approval
- Gym registration → Admin approval
- Status tracking (Pending/Approved/Rejected)

### Security Features
- Parameterized SQL queries (SQL injection prevention)
- Role-based access control
- Centralized connection string management

---

## 🧪 Testing Checklist

### Member Interface
- [ ] Register new member
- [ ] Login as member
- [ ] Create workout plan
- [ ] Create diet plan
- [ ] Book appointment with trainer
- [ ] Submit trainer feedback

### Trainer Interface
- [ ] Register new trainer (verify request created)
- [ ] Login as approved trainer
- [ ] Create workout plan for client
- [ ] View appointments

### Gym Owner Interface
- [ ] Approve trainer request
- [ ] View member reports
- [ ] Add new trainer directly

### Admin Interface
- [ ] Approve gym request
- [ ] View gym performance reports
- [ ] Import users from Excel

---

## 🎯 Academic Requirements Met

- [x] 4 User interfaces (Member, Trainer, Gym Owner, Admin)
- [x] Database normalized to 3NF
- [x] 20 reports (10 required + 10 additional)
- [x] Excel data import (50+ users)
- [x] Audit trail with 30+ triggers
- [x] Full CRUD operations
- [x] Approval workflows
- [x] ERD and schema documentation

---

## 📝 License

This project was created for CS2005: Database Systems coursework (Spring 2024).

## 👨‍💻 Team

Database Systems Project - Spring 2024

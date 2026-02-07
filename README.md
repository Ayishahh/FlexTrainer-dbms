# Flex Trainer - Gym Management System

A comprehensive gym management database system built with C# Windows Forms and SQL Server.

---

## Overview

Flex Trainer is a desktop application for managing gym operations with role-based access control supporting four user types: Members, Trainers, Gym Owners, and Administrators.

### Core Features
- Member registration and profile management
- Trainer registration with approval workflow
- Workout and diet plan creation with nutritional tracking
- Appointment booking and scheduling
- Feedback and rating system
- 20 comprehensive business intelligence reports
- Administrative dashboards with analytics
- Excel bulk user import
- SHA256 password hashing
- SQL injection prevention
- Input validation

---

## Project Structure

```
DB_Project/
├── src/                              # C# Application
│   ├── DatabaseHelper.cs             # Connection, password, validation
│   ├── LogIn.cs                      # Authentication
│   ├── *Dashboard.cs                 # Role-specific interfaces
│   ├── *Reports.cs                   # Report UI forms
│   └── DB_phase2_project.sln
│
├── database/
│   ├── 01_schema.sql                 # 29 tables
│   ├── 02_triggers.sql               # 49 audit triggers
│   ├── 03_procedures.sql             # CRUD stored procedures
│   ├── 04_sample_data.sql            # Test data 
│   ├── 06_comprehensive_reports.sql  # 20 report procedures
│   └── PASSWORD_REFERENCE.md         # Test credentials
│
├── docs/
│   
└── docker-compose.yml
```

---

## Quick Start

### Prerequisites
- Docker Desktop (recommended) or SQL Server 2019+
- Visual Studio 2019+ with .NET Framework 4.7.2
- 4GB free disk space

### Database Setup

```bash
# Start SQL Server container
cd DB_Project
docker-compose up -d

# Wait for container to be ready
docker logs -f flex-trainer-db
# Look for: "SQL Server is now ready for client connections"

# Initialize database
chmod +x setup_database.sh
./setup_database.sh
```

Manual setup if needed:

```bash
# Execute each script in order
docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C \
    -i /docker-entrypoint-initdb.d/01_schema.sql

docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C \
    -i /docker-entrypoint-initdb.d/02_triggers.sql

docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C \
    -i /docker-entrypoint-initdb.d/03_procedures.sql

docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C \
    -i /docker-entrypoint-initdb.d/04_sample_data.sql

docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C \
    -i /docker-entrypoint-initdb.d/06_comprehensive_reports.sql
```

### Application Setup

**Connection String** (pre-configured in `src/App.config`):
```
Data Source=localhost,1433;
Initial Catalog=DB_PROJECT;
User ID=sa;
Password=FlexTrainer2024!;
TrustServerCertificate=True
```

**For Parallels Users**: If Docker runs on macOS and Visual Studio on Windows, replace `localhost` with your Mac's IP address.

**Build and Run**:
1. Open `src/Flex-Trainer.sln` in Visual Studio
2. Build
3. Run

---

## Test Accounts

All passwords are SHA256 hashed in the database. Use plain-text passwords below for login.

**Admins**
- alice_smith / admin123
- jessica_lopez / admin456

**Gym Owners**
- eva_brown / owner123
- james_williams / owner456

**Trainers**
- bob_johnson / trainer123
- zara_garcia / trainer456

**Members**
- john_doe / member123
- jane_doe / member456
- michael_lee / member789

Complete list: See [database/PASSWORD_REFERENCE.md](database/PASSWORD_REFERENCE.md)

---

## User Roles

### Member
- Register with validated input
- Create and manage workout/diet plans
- Book trainer appointments
- Submit feedback and ratings
- Edit profile

### Trainer
- Register (requires gym owner approval)
- Create plans for assigned members
- View appointment schedule
- View client feedback
- Edit profile (experience, specialization)

### Gym Owner
- Approve/reject trainer requests
- View all members and trainers at gym
- Access gym-specific reports
- Manage gym operations

### Administrator
- Approve/reject gym registration requests
- Import users from Excel (bulk upload)
- View system-wide analytics
- Manage all gyms, members, and trainers
- Access all 20 business intelligence reports

---

## Database Architecture

**Schema**: 29 tables normalized to 3NF

**Core Components**:
- Users (role-based authentication)
- Member, Trainer, Gym_Owner, Admin (role-specific data)
- Gym, Membership (facility management)
- Workout_Plan, Diet_Plan (fitness tracking)
- Exercise, Machine, Meal, Allergen (supporting entities)
- Appointment, Feedback (interactions)
- System_Log (audit trail)

**Audit Trail**: 49 triggers logging all INSERT, UPDATE, and DELETE operations to System_Log table.

**Constraints**:
- CHECK: Data validation (ratings 1-5, positive values, age limits)
- DEFAULT: Automatic timestamps and status values
- FOREIGN KEY: Referential integrity
- UNIQUE: Username and email uniqueness

---

## Reports

1. Members by trainer and gym
2. Members by diet plan and gym
3. Cross-gym member analysis
4. Machine usage by day
5. Low-calorie breakfast plans (<500 cal)
6. Low-carb diet plans (<300g)
7. Workout plans excluding specific machine
8. Allergen-free diet plans
9. New memberships (last 3 months)
10. Gym member growth comparison
11. Trainer performance metrics
12. Popular workout plans
13. Popular diet plans
14. Gym revenue analysis
15. Trainer appointment schedules
16. Member activity metrics
17. Workout plans by goal
18. Diet plans by type
19. Pending approval requests
20. System audit log

Reports accessible through role-specific UI forms.

---

## Security

**Password Security**
- SHA256 hashing for all passwords
- No plain-text storage
- Hash verification on login

**SQL Injection Prevention**
- 100% parameterized queries
- All 50+ input points secured
- Stored procedures for database operations

**Input Validation**
- Email format (RFC compliant)
- Password strength (minimum 8 characters)
- Username format (3-50 alphanumeric + underscore)
- Age validation (minimum 13 years)
- Positive number validation

**Connection Security**
- Using statements for automatic disposal
- Centralized connection management
- No connection strings in error messages

---

## Docker Commands

```bash
# Start
docker-compose up -d

# Stop (keep data)
docker-compose down

# Stop and remove data
docker-compose down -v

# View logs
docker logs -f flex-trainer-db

# Connect to SQL Server
docker exec -it flex-trainer-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P FlexTrainer2024! -C
```

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

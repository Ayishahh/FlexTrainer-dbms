# Flex Trainer - Database Schema Documentation

## Overview

The Flex Trainer database supports comprehensive gym management with multi-role access control, workout and diet planning, appointment scheduling, and complete audit trail logging.

### Key Features
- Role-based user management (Members, Trainers, Gym Owners, Admins)
- Workout and diet plan creation with nutritional tracking
- Appointment scheduling with approval workflow
- Feedback and rating system
- Comprehensive audit trail with 49 triggers
- Data integrity through CHECK, DEFAULT, and FOREIGN KEY constraints

## Normalization

The database is normalized to **Third Normal Form (3NF)**:
- **1NF**: All attributes contain atomic values
- **2NF**: No partial dependencies on composite keys
- **3NF**: No transitive dependencies

## Schema Summary

- **Tables**: 29
- **Triggers**: 49 (audit logging for INSERT, UPDATE, DELETE)
- **Stored Procedures**: 29+ (CRUD operations)
- **Report Procedures**: 20 (business intelligence)

---

## Core User Tables

### Users
Base table for all system users with role-based inheritance.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| User_ID | INT | PK, IDENTITY | Unique user identifier |
| First_name | VARCHAR(255) | NOT NULL | User's first name |
| Last_name | VARCHAR(255) | NOT NULL | User's last name |
| Username | VARCHAR(255) | UNIQUE, NOT NULL | Login username |
| Password | VARCHAR(255) | NOT NULL | SHA256 hashed password |
| email | VARCHAR(255) | UNIQUE, NOT NULL | Email address |
| DOB | DATE | NOT NULL, CHECK(DOB <= GETDATE()) | Date of birth (must not be future) |
| Role | VARCHAR(50) | NOT NULL | Member/Trainer/Admin/GymOwner |

### Member
Extends Users for gym members.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Member_ID | INT | PK, FK(Users) | References User_ID |
| Gym_ID | INT | FK(Gym) | Member's gym |
| Membership_ID | INT | FK(Membership) | Membership type |

### Trainer
Extends Users for personal trainers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Trainer_ID | INT | PK, FK(Users) | References User_ID |
| Experience | INT | | Years of experience |
| Speciality | VARCHAR(255) | | Training specialization |

### Admin
Extends Users for system administrators.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Admin_ID | INT | PK, FK(Users) | References User_ID |

### Gym_Owner
Extends Users for gym facility owners.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| GymOwner_ID | INT | PK, FK(Users) | References User_ID |

---

## Facility Tables

### Gym
Physical gym locations.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Gym_ID | INT | PK, IDENTITY | Unique gym identifier |
| Gym_name | VARCHAR(255) | NOT NULL | Gym name |
| Gym_location | VARCHAR(255) | | Physical address |
| GymOwner_ID | INT | FK(Gym_Owner) | Owning user |

### Membership
Subscription plans available at gyms.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Membership_ID | INT | PK, IDENTITY | Unique membership type ID |
| Membership_name | VARCHAR(255) | NOT NULL | Plan name |
| Membership_duration | INT | NOT NULL, CHECK(> 0) | Duration in months |
| Membership_charges | NUMERIC(10,2) | NOT NULL, CHECK(> 0) | Monthly/annual cost |

---

## Workout Management Tables

### Workout_Plan
Exercise plans created by users or trainers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Plan_ID | INT | PK, IDENTITY | Unique plan identifier |
| Plan_name | VARCHAR(255) | NOT NULL | Plan name |
| Goal | VARCHAR(255) | | Fitness goal |
| Level | INT | | Difficulty level |
| Charges | NUMERIC(10,2) | CHECK(>= 0) | Plan cost (0 for free) |
| Creator_ID | INT | FK(Users) | User who created the plan |

### Exercise
Individual exercises.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Exercise_ID | INT | PK, IDENTITY | Unique exercise identifier |
| Exercise_name | VARCHAR(255) | NOT NULL | Exercise name |
| Focus_muscle | VARCHAR(100) | | Target muscle group |

### Machine
Gym equipment.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Machine_ID | INT | PK, IDENTITY | Unique machine identifier |
| Machine_name | VARCHAR(255) | NOT NULL | Equipment name |
| Exercise_ID | INT | FK(Exercise) | Related exercise |

### Workout_Exercise
Junction table linking exercises to workout plans.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Workout_ID | INT | PK, IDENTITY | Unique workout exercise identifier |
| Workout_sets | INT | NOT NULL, CHECK(> 0) | Number of sets |
| Workout_reps | INT | NOT NULL, CHECK(> 0) | Repetitions per set |
| Rest_intervals | INT | NOT NULL, CHECK(>= 0) | Rest time in seconds |
| Exercise_ID | INT | FK(Exercise) | Exercise reference |
| Plan_ID | INT | FK(Workout_Plan) | Plan reference |

---

## Diet Management Tables

### Diet_Plan
Nutrition plans created by users or trainers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Diet_ID | INT | PK, IDENTITY | Unique diet plan identifier |
| Diet_name | VARCHAR(255) | NOT NULL | Plan name |
| Diet_type | VARCHAR(100) | | Type (Vegan, Vegetarian, Non-Veg) |
| Diet_goal | VARCHAR(255) | | Goal (weight loss, muscle gain, etc.) |
| Creator_ID | INT | FK(Users) | User who created the plan |

### Meals
Individual meals with nutritional information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Meal_ID | INT | PK, IDENTITY | Unique meal identifier |
| Meal_name | VARCHAR(255) | NOT NULL | Meal name |
| Meal_type | VARCHAR(255) | NOT NULL, CHECK(IN ('Breakfast', 'Lunch', 'Dinner', 'Snack')) | Meal time category |
| Protein | NUMERIC(10,2) | NOT NULL, CHECK(>= 0) | Protein content (grams) |
| Carbohydrates | NUMERIC(10,2) | NOT NULL, CHECK(>= 0) | Carbs content (grams) |
| Fibre | NUMERIC(10,2) | NOT NULL, CHECK(>= 0) | Fiber content (grams) |
| Fats | NUMERIC(10,2) | NOT NULL, CHECK(>= 0) | Fat content (grams) |
| Calories | NUMERIC(10,2) | NOT NULL, CHECK(>= 0) | Total caloric content |

### Allergen
Food allergens for dietary restrictions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Allergen_ID | INT | PK, IDENTITY | Unique allergen identifier |
| Allergen_names | VARCHAR(255) | NOT NULL | Allergen name (Peanuts, Gluten, etc.) |

### Diet_Meal
Junction table linking meals to diet plans.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Diet_ID | INT | PK, FK(Diet_Plan) | Diet plan reference |
| Meal_ID | INT | PK, FK(Meals) | Meal reference |

### Meal_Contains
Junction table linking allergens to meals.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Allergen_ID | INT | PK, FK(Allergen) | Allergen reference |
| Meal_ID | INT | PK, FK(Meals) | Meal reference |

---

## Interaction Tables

### Feedback
Ratings and reviews from members to trainers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Feedback_ID | INT | PK, IDENTITY | Unique feedback identifier |
| Rating | INT | NOT NULL, CHECK(BETWEEN 1 AND 5) | Numeric rating 1-5 |
| Review | TEXT | | Written review |
| Member_ID | INT | FK(Member) | Member who gave feedback |
| Trainer_ID | INT | FK(Trainer) | Trainer receiving feedback |

### Appointment
Training sessions between members and trainers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Appointment_ID | INT | PK, IDENTITY | Unique appointment identifier |
| Appointment_date | DATE | NOT NULL | Session date |
| Appointment_start_time | TIME(0) | NOT NULL | Start time |
| Appointment_end_time | TIME(0) | NOT NULL, CHECK(> start_time) | End time |
| Appointment_status | VARCHAR(50) | DEFAULT 'Scheduled', CHECK(IN ('Scheduled', 'Completed', 'Cancelled')) | Appointment status |
| Member_ID | INT | NOT NULL, FK(Member) | Attending member |
| Trainer_ID | INT | NOT NULL, FK(Trainer) | Conducting trainer |

### Appointment_Requests
Pending appointment requests for trainer approval.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Request_ID | INT | PK, IDENTITY | Unique request identifier |
| Request_date | DATE | NOT NULL, DEFAULT GETDATE() | When request was made |
| Request_start_time | TIME(0) | NOT NULL | Requested start time |
| Request_end_time | TIME(0) | NOT NULL, CHECK(> start_time) | Requested end time |
| Request_status | VARCHAR(255) | NOT NULL, DEFAULT 'Pending', CHECK(IN ('Pending', 'Approved', 'Rejected')) | Request status |
| Member_ID | INT | FK(Member) | Member requesting |
| Trainer_ID | INT | FK(Trainer) | Trainer to book |

---

## Approval Workflow Tables

### Gym_Request
Pending gym registration requests for admin approval.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Request_ID | INT | PK, IDENTITY | Unique request identifier |
| Gym_name | VARCHAR(255) | NOT NULL | Proposed gym name |
| Gym_location | VARCHAR(255) | | Proposed location |
| Request_date | DATE | NOT NULL, DEFAULT GETDATE() | When request was submitted |
| Request_status | VARCHAR(255) | NOT NULL, DEFAULT 'Pending', CHECK(IN ('Pending', 'Approved', 'Rejected')) | Request status |
| GymOwner_ID | INT | FK(Gym_Owner) | Owner requesting approval |

### Trainer_Requests
Pending trainer registration requests for gym owner approval.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Request_ID | INT | PK, IDENTITY | Unique request identifier |
| Request_date | DATE | NOT NULL, DEFAULT GETDATE() | When request was submitted |
| Request_status | VARCHAR(255) | NOT NULL, DEFAULT 'Pending', CHECK(IN ('Pending', 'Approved', 'Rejected')) | Request status |
| Trainer_ID | INT | FK(Trainer) | Trainer requesting approval |
| Gym_ID | INT | FK(Gym) | Gym to join |

---

## Relationship Tables (Junction)

### Trains
Many-to-many relationship between trainers and members.

| Column | Type | Constraints |
|--------|------|-------------|
| Trainer_ID | INT | PK, FK(Trainer) |
| Member_ID | INT | PK, FK(Member) |

### Works_For
Many-to-many relationship between trainers and gyms.

| Column | Type | Constraints |
|--------|------|-------------|
| Gym_ID | INT | PK, FK(Gym) |
| Trainer_ID | INT | PK, FK(Trainer) |

### Access_Workout_Plan
Many-to-many relationship for member access to workout plans.

| Column | Type | Constraints |
|--------|------|-------------|
| Member_ID | INT | PK, FK(Member) |
| Plan_ID | INT | PK, FK(Workout_Plan) |

### Access_Diet_Plan
Many-to-many relationship for member access to diet plans.

| Column | Type | Constraints |
|--------|------|-------------|
| Member_ID | INT | PK, FK(Member) |
| Diet_ID | INT | PK, FK(Diet_Plan) |

### Member_Joins_Gym
Historical record of member gym memberships.

| Column | Type | Constraints |
|--------|------|-------------|
| Member_ID | INT | PK, FK(Member) |
| Gym_ID | INT | PK, FK(Gym) |
| Join_date | DATE | NOT NULL |
| Leave_date | DATE | |

### Adds
Records of gym owners adding trainers directly.

| Column | Type | Constraints |
|--------|------|-------------|
| GymOwner_ID | INT | PK, FK(Gym_Owner) |
| Trainer_ID | INT | PK, FK(Trainer) |
| Gym_ID | INT | FK(Gym) |

---

## Audit Tables

### System_Log
Comprehensive audit trail for all database operations.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Log_ID | INT | PK, IDENTITY | Unique log identifier |
| Log_date | DATETIME | NOT NULL, DEFAULT GETDATE() | When action occurred |
| Activity_type | VARCHAR(50) | DEFAULT 'SYSTEM' | Type of activity |
| Table_affected | VARCHAR(50) | | Affected table name |
| Record_ID | INT | | ID of affected record |
| User_ID | INT | FK(Users) | User who performed action |
| Log_description | VARCHAR(255) | | Description of action |

### Previous_Trainer
Archive of trainers who left gyms.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Trainer_ID | INT | PK, FK(Trainer) | Trainer reference |
| Gym_ID | INT | PK, FK(Gym) | Gym reference |
| Leave_date | DATE | | When trainer left |

---

## Stored Procedures

### CRUD Operations (29+)

| Procedure Name | Purpose |
|----------------|---------|
| SP_User_Login | Authenticate users and return role (with password hash verification) |
| SP_Add_New_User | Insert new user with role-specific data (hashes password) |
| SP_Update_Existing_User | Update user information |
| SP_Return_Gym_ID | Get gym ID by name |
| SP_Return_Membership_ID | Get membership ID by name |
| SP_Make_Workout_Exercises | Add exercise to workout plan |
| SP_Request_Appointment | Book appointment with trainer |
| SP_Approve_Gym_Request | Admin approves gym registration |
| SP_Reject_Gym_Request | Admin rejects gym registration |
| SP_Approve_Trainer_Request | Gym owner approves trainer |
| SP_Reject_Trainer_Request | Gym owner rejects trainer |
| SP_Delete_Gym_Procedure | Remove gym and related data |
| SP_Remove_Trainer_From_Tables | Remove trainer from system |
| SP_Remove_Member_From_Tables | Remove member from system |

### Report Procedures (20)

| Procedure Name | Purpose |
|----------------|---------|
| SP_Report_Members_By_Trainer_And_Gym | Members trained by specific trainer at gym |
| SP_Report_Members_By_Diet_Plan_And_Gym | Members following diet plan at gym |
| SP_Report_Members_Cross_Gym_By_Trainer_Diet | Cross-gym member analysis |
| SP_Report_Machine_Usage_By_Day | Machine usage count by day |
| SP_Report_Diet_Plans_Low_Calorie_Breakfast | Diet plans with <500 cal breakfast |
| SP_Report_Diet_Plans_Low_Carb | Diet plans with <300g carbs |
| SP_Report_Workout_Plans_Without_Machine | Plans not using specific machine |
| SP_Report_Diet_Plans_Without_Allergen | Allergen-free diet plans |
| SP_Report_New_Memberships_Recent | New memberships (last 3 months) |
| SP_Report_Gym_Member_Comparison | Gym growth comparison |
| SP_Report_Trainer_Performance | Trainer ratings and metrics |
| SP_Report_Popular_Workout_Plans | Most accessed workout plans |
| SP_Report_Popular_Diet_Plans | Most accessed diet plans |
| SP_Report_Gym_Revenue | Revenue analysis by gym |
| SP_Report_Trainer_Appointments | Trainer schedules |
| SP_Report_Member_Activity | Member engagement metrics |
| SP_Report_Workout_Plans_By_Goal | Plans by fitness goal |
| SP_Report_Diet_Plans_By_Type | Plans by diet type |
| SP_Report_Pending_Requests | All pending approvals |
| SP_Report_System_Audit_Log | Recent audit log entries |

---

## Triggers (49 total)

The database includes 49 triggers for comprehensive audit logging across all major tables.

### Trigger Categories

**INSERT Triggers**: Log all new record creations
- Users, Member, Trainer, Admin, Gym_Owner
- Gym, Membership, Workout_Plan, Diet_Plan
- Exercise, Machine, Meals, Allergen
- Appointment, Feedback, and all junction tables

**UPDATE Triggers**: Log modifications
- Users, Gym, Workout_Plan, Diet_Plan
- Gym_Request, Trainer_Requests, Appointment_Requests
- Member, Trainer profile updates

**DELETE Triggers**: Log deletions and archive data
- Trainer deletions (archived to Previous_Trainer)
- Gym, Member, Workout_Plan deletions

All triggers write to the System_Log table with timestamp, affected table, record ID, and user ID.

---

## Data Integrity Constraints

### CHECK Constraints
- DOB must not be in future
- Membership duration and charges > 0
- Workout sets, reps > 0, rest intervals >= 0
- Feedback rating between 1 and 5
- Nutritional values >= 0
- Appointment/request end time > start time
- Status fields constrained to valid enum values

### DEFAULT Constraints
- Request dates default to GETDATE()
- Request statuses default to 'Pending'
- Appointment status defaults to 'Scheduled'
- System_Log timestamps default to GETDATE()

### FOREIGN KEY Constraints
- Cascade rules enforce referential integrity
- Prevent orphaned records
- Maintain relationship consistency

### UNIQUE Constraints
- Username must be unique
- Email must be unique

---

## Security

### Password Storage
- All passwords hashed using SHA256
- No plain-text passwords stored
- Hash comparison on login via SP_User_Login

### SQL Injection Prevention
- All database operations use parameterized queries or stored procedures
- No dynamic SQL construction with user input
- 100% coverage across all 50+ input points

### Input Validation
- Email format validation
- Password strength requirements (minimum 8 characters)
- Username format (alphanumeric + underscore, 3-50 chars)
- Age validation (minimum 13 years)
- Positive number validation for charges and durations

---

## Relationships Summary

| Relationship | Cardinality | Junction Table |
|--------------|-------------|----------------|
| Users → Member | 1:0..1 | None (inheritance) |
| Users → Trainer | 1:0..1 | None (inheritance) |
| Users → Admin | 1:0..1 | None (inheritance) |
| Users → Gym_Owner | 1:0..1 | None (inheritance) |
| Gym_Owner → Gym | 1:N | None |
| Gym → Member | 1:N | None |
| Member → Membership | N:1 | None |
| Trainer ↔ Gym | M:N | Works_For |
| Trainer ↔ Member | M:N | Trains |
| Workout_Plan ↔ Exercise | M:N | Workout_Exercise |
| Diet_Plan ↔ Meals | M:N | Diet_Meal |
| Meals ↔ Allergen | M:N | Meal_Contains |
| Member ↔ Workout_Plan | M:N | Access_Workout_Plan |
| Member ↔ Diet_Plan | M:N | Access_Diet_Plan |
| Member ↔ Gym (history) | M:N | Member_Joins_Gym |


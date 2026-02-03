-- =============================================================================
-- FLEX TRAINER DATABASE SCHEMA
-- Purpose: Creates the database and all tables
-- =============================================================================

-- Create database
CREATE DATABASE DB_PROJECT;
GO

USE DB_PROJECT;
GO

-- =============================================================================
-- CORE TABLES
-- =============================================================================

CREATE TABLE Users(
    User_ID INT IDENTITY(1,1) PRIMARY KEY,
    First_name VARCHAR(255) NOT NULL,
    Last_name VARCHAR(255) NOT NULL,
    DOB DATE NOT NULL,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    Role VARCHAR(255) NOT NULL CHECK(Role IN ('Trainer', 'Member', 'Admin', 'GymOwner')),
    Security_question VARCHAR(255)
);
GO

CREATE TABLE Trainer(
    Trainer_ID INT PRIMARY KEY,
    Experience INT,
    Speciality VARCHAR(255),
    FOREIGN KEY (Trainer_ID) REFERENCES Users(User_ID)
);
GO

CREATE TABLE Admin(
    Admin_ID INT PRIMARY KEY,
    FOREIGN KEY (Admin_ID) REFERENCES Users(User_ID)
);
GO

CREATE TABLE Gym_Owner(
    GymOwner_ID INT PRIMARY KEY,
    FOREIGN KEY (GymOwner_ID) REFERENCES Users(User_ID)
);
GO

CREATE TABLE Gym(
    Gym_ID INT IDENTITY(1,1) PRIMARY KEY,
    Gym_name VARCHAR(255) NOT NULL UNIQUE,
    Gym_location VARCHAR(255) NOT NULL,
    GymOwner_ID INT,
    FOREIGN KEY (GymOwner_ID) REFERENCES Gym_Owner(GymOwner_ID)
);
GO

CREATE TABLE Membership(
    Membership_ID INT IDENTITY(1,1) PRIMARY KEY,
    Membership_name VARCHAR(255) NOT NULL,
    Membership_duration INT NOT NULL,
    Membership_charges NUMERIC(10,2) NOT NULL
);
GO

CREATE TABLE Member(
    Member_ID INT PRIMARY KEY,
    Membership_ID INT,
    Gym_ID INT,
    FOREIGN KEY (Member_ID) REFERENCES Users(User_ID),
    FOREIGN KEY (Membership_ID) REFERENCES Membership(Membership_ID),
    FOREIGN KEY (Gym_ID) REFERENCES Gym(Gym_ID)
);
GO

-- =============================================================================
-- WORKOUT & EXERCISE TABLES
-- =============================================================================

CREATE TABLE Workout_Plan(
    Plan_ID INT IDENTITY(1,1) PRIMARY KEY,
    Plan_name VARCHAR(255) NOT NULL UNIQUE,
    Goal VARCHAR(255) NOT NULL CHECK(Goal IN ('Weight Loss', 'Muscle Gain', 'Strength Gain')),
    Level INT NOT NULL CHECK(Level IN (1, 2, 3)),
    Charges NUMERIC(10,2) NOT NULL,
    Creator_ID INT,
    FOREIGN KEY (Creator_ID) REFERENCES Users(User_ID)
);
GO

CREATE TABLE Exercise(
    Exercise_ID INT IDENTITY(1,1) PRIMARY KEY,
    Exercise_name VARCHAR(255) NOT NULL,
    Focus_muscle VARCHAR(255) NOT NULL
);
GO

CREATE TABLE Machine(
    Machine_Id INT IDENTITY(1,1) PRIMARY KEY,
    Machine_name VARCHAR(255) NOT NULL,
    Machine_price NUMERIC(10,2) NOT NULL,
    Exercise_ID INT,
    FOREIGN KEY (Exercise_ID) REFERENCES Exercise(Exercise_ID)
);
GO

CREATE TABLE Workout_Exercise(
    Workout_ID INT IDENTITY(1,1) PRIMARY KEY,
    Workout_sets INT NOT NULL,
    Workout_reps INT NOT NULL,
    Rest_intervals INT NOT NULL,
    Exercise_ID INT,
    Plan_ID INT,
    FOREIGN KEY (Exercise_ID) REFERENCES Exercise(Exercise_ID),
    FOREIGN KEY (Plan_ID) REFERENCES Workout_Plan(Plan_ID)
);
GO

-- =============================================================================
-- DIET & MEAL TABLES
-- =============================================================================

CREATE TABLE Diet_Plan(
    Diet_ID INT IDENTITY (1,1) PRIMARY KEY,
    Diet_name VARCHAR(255) NOT NULL,
    Diet_type VARCHAR(255) NOT NULL CHECK(Diet_type IN ('Vegan', 'Vegetarian', 'Non-Vegetarian')),
    Diet_goal VARCHAR(255) NOT NULL CHECK(Diet_goal IN ('Weight Loss', 'Muscle Gain', 'Strength Gain')),
    Creator_ID INT,
    FOREIGN KEY (Creator_ID) REFERENCES Users(User_ID)
);
GO

CREATE TABLE Meals(
    Meal_ID INT IDENTITY(1,1) PRIMARY KEY,
    Meal_name VARCHAR(255) NOT NULL,
    Meal_type VARCHAR(255) NOT NULL CHECK(Meal_type IN ('Vegan', 'Vegetarian', 'Non-Vegetarian')),
    Protein NUMERIC(10,2) NOT NULL,
    Carbohydrates NUMERIC(10,2) NOT NULL,
    Fibre NUMERIC(10,2) NOT NULL,
    Fats NUMERIC(10,2) NOT NULL
);
GO

CREATE TABLE Allergen(
    Allergen_ID INT IDENTITY(1,1) PRIMARY KEY,
    Allergen_names VARCHAR(255) NOT NULL
);
GO

CREATE TABLE Diet_Meal(
    Diet_ID INT,
    Meal_ID INT,
    PRIMARY KEY (Diet_ID, Meal_ID),
    FOREIGN KEY (Diet_ID) REFERENCES Diet_Plan(Diet_ID),
    FOREIGN KEY (Meal_ID) REFERENCES Meals(Meal_ID)
);
GO

CREATE TABLE Meal_Contains(
    Allergen_ID INT,
    Meal_ID INT,
    PRIMARY KEY (Allergen_ID, Meal_ID),
    FOREIGN KEY (Allergen_ID) REFERENCES Allergen(Allergen_ID),
    FOREIGN KEY (Meal_ID) REFERENCES Meals(Meal_ID)
);
GO

-- =============================================================================
-- ACCESS & RELATIONSHIP TABLES
-- =============================================================================

CREATE TABLE Access_Workout_Plan(
    Member_ID INT,
    Plan_ID INT,
    PRIMARY KEY (Member_ID, Plan_ID),
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Plan_ID) REFERENCES Workout_Plan(Plan_ID)
);
GO

CREATE TABLE Access_Diet_Plan(
    Member_ID INT,
    Diet_ID INT,
    PRIMARY KEY (Member_ID, Diet_ID),
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Diet_ID) REFERENCES Diet_Plan(Diet_ID)
);
GO

CREATE TABLE Trains(
    Trainer_ID INT,
    Member_ID INT,
    PRIMARY KEY (Trainer_ID, Member_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID),
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID)
);
GO

CREATE TABLE Works_For(
    Gym_ID INT,
    Trainer_ID INT,
    PRIMARY KEY (Gym_ID, Trainer_ID),
    FOREIGN KEY (Gym_ID) REFERENCES Gym(Gym_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID)
);
GO

CREATE TABLE Adds(
    GymOwner_ID INT,
    Trainer_ID INT,
    PRIMARY KEY (GymOwner_ID, Trainer_ID),
    FOREIGN KEY (GymOwner_ID) REFERENCES Gym_Owner(GymOwner_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID)
);
GO

CREATE TABLE Member_Joins_Gym(
    Member_ID INT,
    Gym_ID INT,
    Join_date DATE NOT NULL,
    Leave_date DATE,
    PRIMARY KEY (Member_ID, Gym_ID),
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Gym_ID) REFERENCES Gym(Gym_ID)
);
GO

-- =============================================================================
-- APPOINTMENT TABLES
-- =============================================================================

CREATE TABLE Appointment(
    Appointment_ID INT IDENTITY(1,1) PRIMARY KEY,
    Appointment_date DATE NOT NULL,
    Appointment_start_time TIME(0) NOT NULL,
    Appointment_end_time TIME(0) NOT NULL,
    Member_ID INT NOT NULL,
    Trainer_ID INT NOT NULL,
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID)
);
GO

CREATE TABLE Appointment_Requests(
    Request_ID INT IDENTITY(1,1) PRIMARY KEY,
    Request_date DATE NOT NULL,
    Request_start_time TIME(0) NOT NULL,
    Request_end_time TIME(0) NOT NULL,
    Request_status VARCHAR(255) NOT NULL CHECK(Request_status IN ('Pending', 'Approved', 'Rejected')),
    Member_ID INT,
    Trainer_ID INT,
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID)
);
GO

-- =============================================================================
-- REQUEST TABLES
-- =============================================================================

CREATE TABLE Gym_Request(
    Request_ID INT IDENTITY(1,1) PRIMARY KEY,
    Request_date DATE NOT NULL,
    Request_status VARCHAR(255) NOT NULL CHECK(Request_status IN ('Pending', 'Approved', 'Rejected')),
    Gym_name VARCHAR(255) NOT NULL,
    Gym_location VARCHAR(255) NOT NULL,
    Admin_ID INT,
    GymOwner_ID INT,
    FOREIGN KEY (Admin_ID) REFERENCES Admin(Admin_ID),
    FOREIGN KEY (GymOwner_ID) REFERENCES Gym_Owner(GymOwner_ID)
);
GO

CREATE TABLE Trainer_Requests(
    Request_ID INT IDENTITY(1,1) PRIMARY KEY,
    Request_date DATE NOT NULL,
    Request_status VARCHAR(255) NOT NULL CHECK(Request_status IN ('Pending', 'Approved', 'Rejected')),
    Trainer_ID INT,
    GymOwner_ID INT,
    Gym_ID INT,
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID),
    FOREIGN KEY (GymOwner_ID) REFERENCES Gym_Owner(GymOwner_ID),
    FOREIGN KEY (Gym_ID) REFERENCES Gym(Gym_ID)
);
GO

-- =============================================================================
-- FEEDBACK TABLE
-- =============================================================================

CREATE TABLE Feedback(
    Feedback_ID INT IDENTITY(1,1) PRIMARY KEY,
    Rating INT NOT NULL CHECK(Rating IN (1, 2, 3, 4, 5)),
    Review VARCHAR(255) NOT NULL,
    Member_ID INT,
    Trainer_ID INT,
    FOREIGN KEY (Member_ID) REFERENCES Member(Member_ID),
    FOREIGN KEY (Trainer_ID) REFERENCES Trainer(Trainer_ID)
);
GO

-- =============================================================================
-- ARCHIVE TABLE
-- =============================================================================

CREATE TABLE Previous_Trainer(
    Trainer_ID INT PRIMARY KEY,
    First_name VARCHAR(255) NOT NULL,
    Last_name VARCHAR(255) NOT NULL,
    DOB DATE NOT NULL,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    Security_question VARCHAR(255),
    Experience VARCHAR(255),
    Speciality VARCHAR(255),
    Gym_ID INT
);
GO

-- =============================================================================
-- SYSTEM LOG TABLE
-- =============================================================================

CREATE TABLE System_Log(
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    Log_date DATE NOT NULL,
    Log_description VARCHAR(255) NOT NULL
);
GO

PRINT 'Schema created successfully!';
GO
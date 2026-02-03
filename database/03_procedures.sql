-- =============================================================================
-- FLEX TRAINER DATABASE STORED PROCEDURES
-- Purpose: Creates all stored procedures for business logic
-- =============================================================================

USE DB_PROJECT;
GO

-- =============================================================================
-- USER MANAGEMENT PROCEDURES
-- =============================================================================

-- Add a new user with role-specific record creation
CREATE PROCEDURE SP_Add_New_User
    @First_name NVARCHAR(50),
    @Last_name NVARCHAR(50),
    @DOB DATE,
    @Username NVARCHAR(50),
    @Password NVARCHAR(50),
    @email NVARCHAR(50),
    @Role NVARCHAR(50),
    @Experience NVARCHAR(50) = NULL,
    @Specialization NVARCHAR(50) = NULL,
    @Gym_ID INT = NULL,
    @Membership_ID INT = NULL
AS
BEGIN
    -- Insert into Users table (trigger will create role-specific record)
    INSERT INTO Users (First_name, Last_name, DOB, Username, Password, email, Role)
    VALUES (@First_name, @Last_name, @DOB, @Username, @Password, @email, @Role);
    
    DECLARE @User_ID INT = SCOPE_IDENTITY();
    
    -- Update role-specific details if provided
    IF @Role = 'Trainer' AND @Experience IS NOT NULL
    BEGIN
        UPDATE Trainer SET Experience = @Experience, Speciality = @Specialization WHERE Trainer_ID = @User_ID;
    END
    ELSE IF @Role = 'Member' AND @Gym_ID IS NOT NULL
    BEGIN
        UPDATE Member SET Gym_ID = @Gym_ID, Membership_ID = @Membership_ID WHERE Member_ID = @User_ID;
    END
    
    SELECT @User_ID AS NewUserID;
END;
GO

-- Update existing user information
CREATE PROCEDURE SP_Update_Existing_User
    @User_ID INT,
    @First_name NVARCHAR(50),
    @Last_name NVARCHAR(50),
    @DOB DATE,
    @Password NVARCHAR(50),
    @email NVARCHAR(50),
    @Experience NVARCHAR(50) = NULL,
    @Specialization NVARCHAR(50) = NULL,
    @Membership_ID INT = NULL
AS
BEGIN
    UPDATE Users 
    SET First_name = @First_name, 
        Last_name = @Last_name, 
        DOB = @DOB, 
        Password = @Password, 
        email = @email
    WHERE User_ID = @User_ID;
    
    -- Update role-specific tables
    IF EXISTS (SELECT 1 FROM Trainer WHERE Trainer_ID = @User_ID)
    BEGIN
        UPDATE Trainer SET Experience = @Experience, Speciality = @Specialization WHERE Trainer_ID = @User_ID;
    END
    
    IF EXISTS (SELECT 1 FROM Member WHERE Member_ID = @User_ID)
    BEGIN
        UPDATE Member SET Membership_ID = @Membership_ID WHERE Member_ID = @User_ID;
    END
END;
GO

-- =============================================================================
-- AUTHENTICATION PROCEDURES
-- =============================================================================

-- User login validation
CREATE PROCEDURE SP_User_Login
    @Username NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username AND Password = @Password)
        BEGIN
            SELECT Role FROM Users WHERE Username = @Username;
        END
        ELSE
        BEGIN
            SELECT 'Incorrect Password' AS Role;
        END
    END
    ELSE
    BEGIN
        SELECT 'Username does not exist' AS Role;
    END
END;
GO

-- =============================================================================
-- LOOKUP PROCEDURES
-- =============================================================================

-- Return Gym ID by name
CREATE PROCEDURE SP_Return_Gym_ID
    @Gym_name NVARCHAR(50)
AS
BEGIN
    SELECT Gym_ID FROM Gym WHERE Gym_name = @Gym_name;
END;
GO

-- Return Membership ID by name
CREATE PROCEDURE SP_Return_Membership_ID
    @Membership_name NVARCHAR(50)
AS
BEGIN
    SELECT Membership_ID FROM Membership WHERE Membership_name = @Membership_name;
END;
GO

-- Return all gym owner names
CREATE PROCEDURE SP_Return_GymOwner_Names
AS
BEGIN
    SELECT First_name, Last_name FROM Users WHERE Role = 'GymOwner';
END;
GO

-- Return trainer names for a specific gym
CREATE PROCEDURE SP_Return_Trainer_Names
    @Gym_ID INT
AS
BEGIN
    SELECT u.First_name, u.Last_name 
    FROM Users u
    WHERE u.Role = 'Trainer' AND u.User_ID IN (SELECT Trainer_ID FROM Works_For WHERE Gym_ID = @Gym_ID);
END;
GO

-- Return member names for a specific gym
CREATE PROCEDURE SP_Return_Member_Names
    @Gym_ID INT
AS
BEGIN
    SELECT u.First_name, u.Last_name 
    FROM Users u
    WHERE u.Role = 'Member' AND u.User_ID IN (SELECT Member_ID FROM Member WHERE Gym_ID = @Gym_ID);
END;
GO

-- Return member IDs for a gym owner's gym
CREATE PROCEDURE SP_Return_Member_ID
    @GymOwner_username NVARCHAR(50)
AS
BEGIN
    DECLARE @GymOwner_ID INT;
    SELECT @GymOwner_ID = User_ID FROM Users WHERE Username = @GymOwner_username;
    SELECT Member_ID FROM Member WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE GymOwner_ID = @GymOwner_ID);
END;
GO

-- Return trainer IDs for a gym owner's gym
CREATE PROCEDURE SP_Return_Trainer_ID
    @GymOwner_username NVARCHAR(50)
AS
BEGIN
    DECLARE @GymOwner_ID INT;
    SELECT @GymOwner_ID = User_ID FROM Users WHERE Username = @GymOwner_username;
    SELECT Trainer_ID FROM Works_For WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE GymOwner_ID = @GymOwner_ID);
END;
GO

-- =============================================================================
-- MEMBER DETAIL PROCEDURES
-- =============================================================================

-- Return full member details
CREATE PROCEDURE SP_Return_Member_Detail
    @Username NVARCHAR(50)
AS
BEGIN
    SELECT u.User_ID, CONCAT(u.First_name, ' ', u.Last_name) AS Full_name, 
           u.email, u.Password, u.DOB, u.Role, 
           g.Gym_name, m.Membership_name, m.Membership_duration, m.Membership_charges
    FROM Users u
    JOIN Member mem ON u.User_ID = mem.Member_ID
    LEFT JOIN Gym g ON mem.Gym_ID = g.Gym_ID
    LEFT JOIN Membership m ON mem.Membership_ID = m.Membership_ID
    WHERE u.Username = @Username;
END;
GO

-- Return trainer from a request
CREATE PROCEDURE SP_Return_Trainer_From_Request
    @Request_ID INT
AS
BEGIN
    DECLARE @Trainer_ID INT;
    SELECT @Trainer_ID = Trainer_ID FROM Trainer_Requests WHERE Request_ID = @Request_ID;
    
    SELECT u.First_name, u.Last_name, u.email, u.Username, u.DOB, t.Experience, t.Speciality 
    FROM Users u
    JOIN Trainer t ON u.User_ID = t.Trainer_ID
    WHERE t.Trainer_ID = @Trainer_ID;
END;
GO

-- =============================================================================
-- APPOINTMENT PROCEDURES
-- =============================================================================

-- Request an appointment with a trainer
CREATE PROCEDURE SP_Request_Appointment
    @Date NVARCHAR(50),
    @Start NVARCHAR(50),
    @End NVARCHAR(50),
    @Member_ID INT,
    @Trainer_ID INT
AS
BEGIN
    DECLARE @Date_time DATE = CAST(@Date AS DATE);
    DECLARE @Start_time TIME = CAST(@Start AS TIME(0));
    DECLARE @End_time TIME = CAST(@End AS TIME(0));
    
    INSERT INTO Appointment_Requests(Request_date, Request_start_time, Request_end_time, Request_status, Member_ID, Trainer_ID)
    VALUES(@Date_time, @Start_time, @End_time, 'Pending', @Member_ID, @Trainer_ID);
    
    SELECT SCOPE_IDENTITY() AS Request_ID;
END;
GO

-- Approve an appointment request
CREATE PROCEDURE SP_Approve_Appointment
    @Request_ID INT
AS
BEGIN
    DECLARE @Date DATE, @Start TIME, @End TIME, @Member_ID INT, @Trainer_ID INT;
    
    SELECT @Date = Request_date, @Start = Request_start_time, @End = Request_end_time,
           @Member_ID = Member_ID, @Trainer_ID = Trainer_ID
    FROM Appointment_Requests WHERE Request_ID = @Request_ID;
    
    UPDATE Appointment_Requests SET Request_status = 'Approved' WHERE Request_ID = @Request_ID;
    
    INSERT INTO Appointment(Appointment_date, Appointment_start_time, Appointment_end_time, Member_ID, Trainer_ID)
    VALUES(@Date, @Start, @End, @Member_ID, @Trainer_ID);
END;
GO

-- Reject an appointment request
CREATE PROCEDURE SP_Reject_Appointment
    @Request_ID INT
AS
BEGIN
    UPDATE Appointment_Requests SET Request_status = 'Rejected' WHERE Request_ID = @Request_ID;
END;
GO

-- =============================================================================
-- WORKOUT PLAN PROCEDURES
-- =============================================================================

-- Create a new workout plan
CREATE PROCEDURE SP_Create_Workout_Plan
    @Plan_name NVARCHAR(255),
    @Goal NVARCHAR(50),
    @Level INT,
    @Charges NUMERIC(10,2),
    @Creator_ID INT
AS
BEGIN
    INSERT INTO Workout_Plan(Plan_name, Goal, Level, Charges, Creator_ID)
    VALUES(@Plan_name, @Goal, @Level, @Charges, @Creator_ID);
    
    SELECT SCOPE_IDENTITY() AS Plan_ID;
END;
GO

-- Add exercise to a workout plan
CREATE PROCEDURE SP_Make_Workout_Exercises
    @Exercise_ID INT,
    @Sets INT,
    @Reps INT,
    @Rest INT,
    @Plan_ID INT
AS
BEGIN
    INSERT INTO Workout_Exercise(Workout_sets, Workout_reps, Rest_intervals, Exercise_ID, Plan_ID)
    VALUES(@Sets, @Reps, @Rest, @Exercise_ID, @Plan_ID);
END;
GO

-- Grant member access to a workout plan
CREATE PROCEDURE SP_Grant_Workout_Access
    @Member_ID INT,
    @Plan_ID INT
AS
BEGIN
    INSERT INTO Access_Workout_Plan(Member_ID, Plan_ID) VALUES(@Member_ID, @Plan_ID);
END;
GO

-- =============================================================================
-- DIET PLAN PROCEDURES
-- =============================================================================

-- Create a new diet plan
CREATE PROCEDURE SP_Create_Diet_Plan
    @Diet_name NVARCHAR(255),
    @Diet_type NVARCHAR(50),
    @Diet_goal NVARCHAR(50),
    @Creator_ID INT
AS
BEGIN
    INSERT INTO Diet_Plan(Diet_name, Diet_type, Diet_goal, Creator_ID)
    VALUES(@Diet_name, @Diet_type, @Diet_goal, @Creator_ID);
    
    SELECT SCOPE_IDENTITY() AS Diet_ID;
END;
GO

-- Add meal to a diet plan
CREATE PROCEDURE SP_Add_Diet_Meal
    @Diet_ID INT,
    @Meal_ID INT
AS
BEGIN
    INSERT INTO Diet_Meal(Diet_ID, Meal_ID) VALUES(@Diet_ID, @Meal_ID);
END;
GO

-- Grant member access to a diet plan
CREATE PROCEDURE SP_Grant_Diet_Access
    @Member_ID INT,
    @Diet_ID INT
AS
BEGIN
    INSERT INTO Access_Diet_Plan(Member_ID, Diet_ID) VALUES(@Member_ID, @Diet_ID);
END;
GO

-- =============================================================================
-- GYM MANAGEMENT PROCEDURES
-- =============================================================================

-- Submit gym registration request
CREATE PROCEDURE SP_Submit_Gym_Request
    @Gym_name NVARCHAR(255),
    @Gym_location NVARCHAR(255),
    @GymOwner_ID INT
AS
BEGIN
    INSERT INTO Gym_Request(Request_date, Request_status, Gym_name, Gym_location, GymOwner_ID)
    VALUES(GETDATE(), 'Pending', @Gym_name, @Gym_location, @GymOwner_ID);
    
    SELECT SCOPE_IDENTITY() AS Request_ID;
END;
GO

-- Approve gym registration request
CREATE PROCEDURE SP_Approve_Gym_Request
    @Request_ID INT,
    @Admin_ID INT = NULL
AS
BEGIN
    DECLARE @Gym_name NVARCHAR(255), @Gym_location NVARCHAR(255), @GymOwner_ID INT;
    
    SELECT @Gym_name = Gym_name, @Gym_location = Gym_location, @GymOwner_ID = GymOwner_ID
    FROM Gym_Request WHERE Request_ID = @Request_ID;
    
    UPDATE Gym_Request SET Request_status = 'Approved', Admin_ID = @Admin_ID WHERE Request_ID = @Request_ID;
    
    INSERT INTO Gym(Gym_name, Gym_location, GymOwner_ID) VALUES(@Gym_name, @Gym_location, @GymOwner_ID);
END;
GO

-- Submit trainer request to join gym
CREATE PROCEDURE SP_Submit_Trainer_Request
    @Trainer_ID INT,
    @Gym_ID INT,
    @GymOwner_ID INT
AS
BEGIN
    INSERT INTO Trainer_Requests(Request_date, Request_status, Trainer_ID, Gym_ID, GymOwner_ID)
    VALUES(GETDATE(), 'Pending', @Trainer_ID, @Gym_ID, @GymOwner_ID);
    
    SELECT SCOPE_IDENTITY() AS Request_ID;
END;
GO

-- Approve trainer request
CREATE PROCEDURE SP_Approve_Trainer_Request
    @Request_ID INT
AS
BEGIN
    DECLARE @Trainer_ID INT, @Gym_ID INT;
    
    SELECT @Trainer_ID = Trainer_ID, @Gym_ID = Gym_ID 
    FROM Trainer_Requests WHERE Request_ID = @Request_ID;
    
    UPDATE Trainer_Requests SET Request_status = 'Approved' WHERE Request_ID = @Request_ID;
    
    INSERT INTO Works_For(Gym_ID, Trainer_ID) VALUES(@Gym_ID, @Trainer_ID);
END;
GO

-- =============================================================================
-- FEEDBACK PROCEDURES
-- =============================================================================

-- Submit feedback for a trainer
CREATE PROCEDURE SP_Submit_Feedback
    @Rating INT,
    @Review NVARCHAR(255),
    @Member_ID INT,
    @Trainer_ID INT
AS
BEGIN
    INSERT INTO Feedback(Rating, Review, Member_ID, Trainer_ID)
    VALUES(@Rating, @Review, @Member_ID, @Trainer_ID);
END;
GO

PRINT 'Stored procedures created successfully!';
GO

-- =============================================================================
-- ADMIN MANAGEMENT PROCEDURES (MISSING FROM INITIAL SET)
-- =============================================================================

-- Delete a gym
CREATE PROCEDURE SP_Delete_Gym
    @Gym_name NVARCHAR(255)
AS
BEGIN
    DELETE FROM Gym WHERE Gym_name = @Gym_name;
END;
GO

-- Remove a trainer (completely from system)
CREATE PROCEDURE SP_Remove_Trainer
    @Trainer_ID INT
AS
BEGIN
    -- This will trigger cascade deletes if set up, or trigger logic
    -- TR_Users_Delete should handle cleanup of Trainer table
    DELETE FROM Users WHERE User_ID = @Trainer_ID;
END;
GO

-- Remove a member (completely from system)
CREATE PROCEDURE SP_Remove_Member
    @Member_ID INT
AS
BEGIN
    -- TR_Users_Delete should handle cleanup of Member table
    DELETE FROM Users WHERE User_ID = @Member_ID;
END;
GO
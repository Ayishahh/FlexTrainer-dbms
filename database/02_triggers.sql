-- =============================================================================
-- FLEX TRAINER DATABASE TRIGGERS
-- Purpose: Creates all database triggers for logging and automation
-- =============================================================================

USE DB_PROJECT;
GO

-- =============================================================================
-- USER TRIGGERS
-- =============================================================================

-- Trigger: Log new user additions and create role-specific records
CREATE TRIGGER TR_Users_Insert
ON Users
AFTER INSERT
AS
BEGIN
    DECLARE @Role VARCHAR(255);
    DECLARE @User_ID INT;
    SELECT @Role = Role, @User_ID = User_ID FROM inserted;
    
    -- Log the user addition
    INSERT INTO System_Log(Log_date, Log_description) 
    VALUES (GETDATE(), CONCAT('New ', @Role, ' added with ID: ', @User_ID));
    
    -- Create role-specific record
    IF @Role = 'Trainer'
    BEGIN
        INSERT INTO Trainer(Trainer_ID) VALUES (@User_ID);
    END
    ELSE IF @Role = 'Admin'
    BEGIN
        INSERT INTO Admin(Admin_ID) VALUES (@User_ID);
    END
    ELSE IF @Role = 'GymOwner'
    BEGIN
        INSERT INTO Gym_Owner(GymOwner_ID) VALUES (@User_ID);
    END
    ELSE IF @Role = 'Member'
    BEGIN
        DECLARE @Membership_ID INT;
        SELECT TOP 1 @Membership_ID = Membership_ID FROM Membership ORDER BY NEWID();
        DECLARE @Gym_ID INT;
        SELECT TOP 1 @Gym_ID = Gym_ID FROM Gym ORDER BY NEWID();
        INSERT INTO Member(Member_ID, Membership_ID, Gym_ID) VALUES (@User_ID, @Membership_ID, @Gym_ID);
    END
END;
GO

-- Trigger: Log user updates
CREATE TRIGGER TR_Users_Update
ON Users
AFTER UPDATE
AS
BEGIN
    DECLARE @User_ID INT;
    SELECT @User_ID = User_ID FROM inserted;
    
    IF UPDATE(First_name)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' First name updated'));
    ELSE IF UPDATE(Last_name)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' Last name updated'));
    ELSE IF UPDATE(DOB)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' DOB updated'));
    ELSE IF UPDATE(Username)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' Username updated'));
    ELSE IF UPDATE(Password)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' Password updated'));
    ELSE IF UPDATE(email)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' email updated'));
    ELSE IF UPDATE(Role)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' Role updated'));
    ELSE IF UPDATE(Security_question)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' Security question updated'));
END;
GO

-- Trigger: Log user deletions and clean up role-specific records
CREATE TRIGGER TR_Users_Delete
ON Users
AFTER DELETE
AS
BEGIN
    DECLARE @User_ID INT;
    DECLARE @Role VARCHAR(255);
    SELECT @User_ID = User_ID, @Role = Role FROM deleted;
    
    IF @Role = 'Trainer'
        DELETE FROM Trainer WHERE Trainer_ID = @User_ID;
    ELSE IF @Role = 'Admin'
        DELETE FROM Admin WHERE Admin_ID = @User_ID;
    ELSE IF @Role = 'GymOwner'
        DELETE FROM Gym_Owner WHERE GymOwner_ID = @User_ID;
    ELSE IF @Role = 'Member'
        DELETE FROM Member WHERE Member_ID = @User_ID;
    
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@User_ID, ' deleted from Users'));
END;
GO

-- =============================================================================
-- TRAINER TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Trainer_Insert
ON Trainer
AFTER INSERT
AS
BEGIN
    DECLARE @Trainer_ID INT;
    SELECT @Trainer_ID = Trainer_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Trainer_ID, ' added as Trainer'));
END;
GO

CREATE TRIGGER TR_Trainer_Update
ON Trainer
AFTER UPDATE
AS
BEGIN
    DECLARE @Trainer_ID INT;
    SELECT @Trainer_ID = Trainer_ID FROM inserted;
    IF UPDATE(Experience)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Trainer_ID, ' Experience updated'));
    ELSE IF UPDATE(Speciality)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Trainer_ID, ' Speciality updated'));
END;
GO

CREATE TRIGGER TR_Trainer_Delete
ON Trainer
AFTER DELETE
AS
BEGIN
    DECLARE @Trainer_ID INT;
    SELECT @Trainer_ID = Trainer_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Trainer_ID, ' deleted as Trainer'));
END;
GO

-- =============================================================================
-- ADMIN TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Admin_Insert
ON Admin
AFTER INSERT
AS
BEGIN
    DECLARE @Admin_ID INT;
    SELECT @Admin_ID = Admin_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Admin_ID, ' added as Admin'));
END;
GO

CREATE TRIGGER TR_Admin_Delete
ON Admin
AFTER DELETE
AS
BEGIN
    DECLARE @Admin_ID INT;
    SELECT @Admin_ID = Admin_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Admin_ID, ' deleted as Admin'));
END;
GO

-- =============================================================================
-- GYM OWNER TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_GymOwner_Insert
ON Gym_Owner
AFTER INSERT
AS
BEGIN
    DECLARE @GymOwner_ID INT;
    SELECT @GymOwner_ID = GymOwner_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@GymOwner_ID, ' added as Gym Owner'));
END;
GO

CREATE TRIGGER TR_GymOwner_Delete
ON Gym_Owner
AFTER DELETE
AS
BEGIN
    DECLARE @GymOwner_ID INT;
    SELECT @GymOwner_ID = GymOwner_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@GymOwner_ID, ' deleted as Gym Owner'));
END;
GO

-- =============================================================================
-- GYM TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Gym_Insert
ON Gym
AFTER INSERT
AS
BEGIN
    DECLARE @Gym_ID INT;
    SELECT @Gym_ID = Gym_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Gym_ID, ' added to Gym'));
END;
GO

CREATE TRIGGER TR_Gym_Update
ON Gym
AFTER UPDATE
AS
BEGIN
    DECLARE @Gym_ID INT;
    SELECT @Gym_ID = Gym_ID FROM inserted;
    IF UPDATE(Gym_name)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Gym_ID, ' Gym name updated'));
    ELSE IF UPDATE(Gym_location)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Gym_ID, ' Gym location updated'));
END;
GO

CREATE TRIGGER TR_Gym_Delete
ON Gym
AFTER DELETE
AS
BEGIN
    DECLARE @Gym_ID INT;
    SELECT @Gym_ID = Gym_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Gym_ID, ' deleted from Gym'));
END;
GO

-- =============================================================================
-- MEMBERSHIP TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Membership_Insert
ON Membership
AFTER INSERT
AS
BEGIN
    DECLARE @Membership_ID INT;
    SELECT @Membership_ID = Membership_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Membership_ID, ' added to Membership'));
END;
GO

CREATE TRIGGER TR_Membership_Delete
ON Membership
AFTER DELETE
AS
BEGIN
    DECLARE @Membership_ID INT;
    SELECT @Membership_ID = Membership_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Membership_ID, ' deleted from Membership'));
END;
GO

-- =============================================================================
-- MEMBER TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Member_Insert
ON Member
AFTER INSERT
AS
BEGIN
    DECLARE @Member_ID INT;
    SELECT @Member_ID = Member_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' added to Member'));
END;
GO

CREATE TRIGGER TR_Member_Delete
ON Member
AFTER DELETE
AS
BEGIN
    DECLARE @Member_ID INT;
    SELECT @Member_ID = Member_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' deleted from Member'));
END;
GO

-- =============================================================================
-- WORKOUT PLAN TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_WorkoutPlan_Insert
ON Workout_Plan
AFTER INSERT
AS
BEGIN
    DECLARE @Plan_ID INT;
    SELECT @Plan_ID = Plan_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Plan_ID, ' added to Workout_Plan'));
END;
GO

CREATE TRIGGER TR_WorkoutPlan_Delete
ON Workout_Plan
AFTER DELETE
AS
BEGIN
    DECLARE @Plan_ID INT;
    SELECT @Plan_ID = Plan_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Plan_ID, ' deleted from Workout_Plan'));
END;
GO

-- =============================================================================
-- EXERCISE TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Exercise_Insert
ON Exercise
AFTER INSERT
AS
BEGIN
    DECLARE @Exercise_ID INT;
    SELECT @Exercise_ID = Exercise_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Exercise_ID, ' added to Exercise'));
END;
GO

CREATE TRIGGER TR_Exercise_Delete
ON Exercise
AFTER DELETE
AS
BEGIN
    DECLARE @Exercise_ID INT;
    SELECT @Exercise_ID = Exercise_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Exercise_ID, ' deleted from Exercise'));
END;
GO

-- =============================================================================
-- WORKOUT EXERCISE TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_WorkoutExercise_Insert
ON Workout_Exercise
AFTER INSERT
AS
BEGIN
    DECLARE @Workout_ID INT;
    SELECT @Workout_ID = Workout_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Workout_ID, ' added to Workout_Exercise'));
END;
GO

CREATE TRIGGER TR_WorkoutExercise_Delete
ON Workout_Exercise
AFTER DELETE
AS
BEGIN
    DECLARE @Workout_ID INT;
    SELECT @Workout_ID = Workout_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Workout_ID, ' deleted from Workout_Exercise'));
END;
GO

-- =============================================================================
-- MACHINE TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Machine_Insert
ON Machine
AFTER INSERT
AS
BEGIN
    DECLARE @Machine_ID INT;
    SELECT @Machine_ID = Machine_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Machine_ID, ' added to Machine'));
END;
GO

CREATE TRIGGER TR_Machine_Delete
ON Machine
AFTER DELETE
AS
BEGIN
    DECLARE @Machine_ID INT;
    SELECT @Machine_ID = Machine_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Machine_ID, ' deleted from Machine'));
END;
GO

-- =============================================================================
-- DIET PLAN TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_DietPlan_Insert
ON Diet_Plan
AFTER INSERT
AS
BEGIN
    DECLARE @Diet_ID INT;
    SELECT @Diet_ID = Diet_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Diet_ID, ' added to Diet_Plan'));
END;
GO

CREATE TRIGGER TR_DietPlan_Delete
ON Diet_Plan
AFTER DELETE
AS
BEGIN
    DECLARE @Diet_ID INT;
    SELECT @Diet_ID = Diet_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Diet_ID, ' deleted from Diet_Plan'));
END;
GO

-- =============================================================================
-- MEALS TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Meals_Insert
ON Meals
AFTER INSERT
AS
BEGIN
    DECLARE @Meal_ID INT;
    SELECT @Meal_ID = Meal_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Meal_ID, ' added to Meals'));
END;
GO

CREATE TRIGGER TR_Meals_Delete
ON Meals
AFTER DELETE
AS
BEGIN
    DECLARE @Meal_ID INT;
    SELECT @Meal_ID = Meal_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Meal_ID, ' deleted from Meals'));
END;
GO

-- =============================================================================
-- ALLERGEN TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Allergen_Insert
ON Allergen
AFTER INSERT
AS
BEGIN
    DECLARE @Allergen_ID INT;
    SELECT @Allergen_ID = Allergen_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Allergen_ID, ' added to Allergen'));
END;
GO

CREATE TRIGGER TR_Allergen_Delete
ON Allergen
AFTER DELETE
AS
BEGIN
    DECLARE @Allergen_ID INT;
    SELECT @Allergen_ID = Allergen_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Allergen_ID, ' deleted from Allergen'));
END;
GO

-- =============================================================================
-- FEEDBACK TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Feedback_Insert
ON Feedback
AFTER INSERT
AS
BEGIN
    DECLARE @Feedback_ID INT;
    SELECT @Feedback_ID = Feedback_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Feedback_ID, ' added to Feedback'));
END;
GO

CREATE TRIGGER TR_Feedback_Delete
ON Feedback
AFTER DELETE
AS
BEGIN
    DECLARE @Feedback_ID INT;
    SELECT @Feedback_ID = Feedback_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Feedback_ID, ' deleted from Feedback'));
END;
GO

-- =============================================================================
-- APPOINTMENT TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_Appointment_Insert
ON Appointment
AFTER INSERT
AS
BEGIN
    DECLARE @Appointment_ID INT;
    SELECT @Appointment_ID = Appointment_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Appointment_ID, ' added to Appointment'));
END;
GO

CREATE TRIGGER TR_Appointment_Delete
ON Appointment
AFTER DELETE
AS
BEGIN
    DECLARE @Appointment_ID INT;
    SELECT @Appointment_ID = Appointment_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Appointment_ID, ' deleted from Appointment'));
END;
GO

-- =============================================================================
-- REQUEST TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_GymRequest_Insert
ON Gym_Request
AFTER INSERT
AS
BEGIN
    DECLARE @Request_ID INT;
    SELECT @Request_ID = Request_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Request_ID, ' added to Gym_Request'));
END;
GO

CREATE TRIGGER TR_GymRequest_Update
ON Gym_Request
AFTER UPDATE
AS
BEGIN
    DECLARE @Request_ID INT;
    SELECT @Request_ID = Request_ID FROM inserted;
    IF UPDATE(Request_status)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Request_ID, ' Gym Request status updated'));
END;
GO

CREATE TRIGGER TR_GymRequest_Delete
ON Gym_Request
AFTER DELETE
AS
BEGIN
    DECLARE @Request_ID INT;
    SELECT @Request_ID = Request_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Request_ID, ' deleted from Gym_Request'));
END;
GO

CREATE TRIGGER TR_TrainerRequest_Insert
ON Trainer_Requests
AFTER INSERT
AS
BEGIN
    DECLARE @Request_ID INT;
    SELECT @Request_ID = Request_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Request_ID, ' added to Trainer_Requests'));
END;
GO

CREATE TRIGGER TR_TrainerRequest_Update
ON Trainer_Requests
AFTER UPDATE
AS
BEGIN
    DECLARE @Request_ID INT;
    SELECT @Request_ID = Request_ID FROM inserted;
    IF UPDATE(Request_status)
        INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Request_ID, ' Trainer Request status updated'));
END;
GO

-- =============================================================================
-- RELATIONSHIP TABLE TRIGGERS
-- =============================================================================

CREATE TRIGGER TR_AccessWorkoutPlan_Insert
ON Access_Workout_Plan
AFTER INSERT
AS
BEGIN
    DECLARE @Member_ID INT, @Plan_ID INT;
    SELECT @Member_ID = Member_ID, @Plan_ID = Plan_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' granted access to Workout_Plan ', @Plan_ID));
END;
GO

CREATE TRIGGER TR_AccessWorkoutPlan_Delete
ON Access_Workout_Plan
AFTER DELETE
AS
BEGIN
    DECLARE @Member_ID INT, @Plan_ID INT;
    SELECT @Member_ID = Member_ID, @Plan_ID = Plan_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' removed from Access_Workout_Plan ', @Plan_ID));
END;
GO

CREATE TRIGGER TR_AccessDietPlan_Insert
ON Access_Diet_Plan
AFTER INSERT
AS
BEGIN
    DECLARE @Member_ID INT, @Diet_ID INT;
    SELECT @Member_ID = Member_ID, @Diet_ID = Diet_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' granted access to Diet_Plan ', @Diet_ID));
END;
GO

CREATE TRIGGER TR_AccessDietPlan_Delete
ON Access_Diet_Plan
AFTER DELETE
AS
BEGIN
    DECLARE @Member_ID INT, @Diet_ID INT;
    SELECT @Member_ID = Member_ID, @Diet_ID = Diet_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT(@Member_ID, ' removed from Access_Diet_Plan ', @Diet_ID));
END;
GO

CREATE TRIGGER TR_MealContains_Insert
ON Meal_Contains
AFTER INSERT
AS
BEGIN
    DECLARE @Allergen_ID INT, @Meal_ID INT;
    SELECT @Allergen_ID = Allergen_ID, @Meal_ID = Meal_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT('Allergen ', @Allergen_ID, ' added to Meal ', @Meal_ID));
END;
GO

CREATE TRIGGER TR_MealContains_Delete
ON Meal_Contains
AFTER DELETE
AS
BEGIN
    DECLARE @Allergen_ID INT, @Meal_ID INT;
    SELECT @Allergen_ID = Allergen_ID, @Meal_ID = Meal_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT('Allergen ', @Allergen_ID, ' removed from Meal ', @Meal_ID));
END;
GO

CREATE TRIGGER TR_WorksFor_Delete
ON Works_For
AFTER DELETE
AS
BEGIN
    DECLARE @Gym_ID INT, @Trainer_ID INT;
    SELECT @Gym_ID = Gym_ID, @Trainer_ID = Trainer_ID FROM deleted;
    
    -- Archive the trainer info
    INSERT INTO Previous_Trainer(Trainer_ID, First_name, Last_name, DOB, Username, Password, email, Security_question, Experience, Speciality, Gym_ID)
    SELECT @Trainer_ID, u.First_name, u.Last_name, u.DOB, u.Username, u.Password, u.email, u.Security_question, t.Experience, t.Speciality, @Gym_ID
    FROM Users u
    JOIN Trainer t ON u.User_ID = t.Trainer_ID
    WHERE u.User_ID = @Trainer_ID;
    
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT('Trainer ', @Trainer_ID, ' removed from Gym ', @Gym_ID));
END;
GO

CREATE TRIGGER TR_MemberJoinsGym_Insert
ON Member_Joins_Gym
AFTER INSERT
AS
BEGIN
    DECLARE @Member_ID INT, @Gym_ID INT;
    SELECT @Member_ID = Member_ID, @Gym_ID = Gym_ID FROM inserted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT('Member ', @Member_ID, ' joined Gym ', @Gym_ID));
END;
GO

CREATE TRIGGER TR_MemberJoinsGym_Delete
ON Member_Joins_Gym
AFTER DELETE
AS
BEGIN
    DECLARE @Member_ID INT, @Gym_ID INT;
    SELECT @Member_ID = Member_ID, @Gym_ID = Gym_ID FROM deleted;
    INSERT INTO System_Log(Log_date, Log_description) VALUES (GETDATE(), CONCAT('Member ', @Member_ID, ' left Gym ', @Gym_ID));
END;
GO

PRINT 'Triggers created successfully!';
GO

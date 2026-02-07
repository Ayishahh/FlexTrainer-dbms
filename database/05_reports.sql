-- =============================================================================
-- FLEX TRAINER DATABASE COMPREHENSIVE REPORTS
-- Purpose: 20 comprehensive report stored procedures for analytics and insights
-- =============================================================================

USE DB_PROJECT;
GO

-- Report 1: Members of specific gym trained by specific trainer
CREATE OR ALTER PROCEDURE SP_Report_Members_By_Trainer_And_Gym
    @Trainer_ID INT,
    @Gym_ID INT
AS
BEGIN
    SELECT
        u.User_ID,
        u.First_name,
        u.Last_name,
        u.email,
        u.DOB,
        ms.Membership_name,
        g.Gym_name,
        t.Experience AS Trainer_Experience,
        t.Speciality AS Trainer_Speciality
    FROM Users u
    JOIN Member m ON u.User_ID = m.Member_ID
    JOIN Trains tr ON m.Member_ID = tr.Member_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    JOIN Trainer t ON tr.Trainer_ID = t.Trainer_ID
    WHERE tr.Trainer_ID = @Trainer_ID
      AND m.Gym_ID = @Gym_ID
    ORDER BY u.Last_name, u.First_name;
END;
GO

-- Report 2: Members of gym following specific diet plan
CREATE OR ALTER PROCEDURE SP_Report_Members_By_Diet_Plan_And_Gym
    @Diet_Plan_ID INT,
    @Gym_ID INT
AS
BEGIN
    SELECT
        u.User_ID,
        u.First_name,
        u.Last_name,
        u.email,
        dp.Diet_name,
        dp.Diet_type,
        dp.Diet_goal,
        g.Gym_name,
        ms.Membership_name
    FROM Users u
    JOIN Member m ON u.User_ID = m.Member_ID
    JOIN Access_Diet_Plan adp ON m.Member_ID = adp.Member_ID
    JOIN Diet_Plan dp ON adp.Diet_ID = dp.Diet_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    WHERE dp.Diet_ID = @Diet_Plan_ID
      AND m.Gym_ID = @Gym_ID
    ORDER BY u.Last_name, u.First_name;
END;
GO

-- Report 3: Members across all gyms of trainer following specific diet plan
CREATE OR ALTER PROCEDURE SP_Report_Members_Cross_Gym_By_Trainer_Diet
    @Trainer_ID INT,
    @Diet_Plan_ID INT
AS
BEGIN
    SELECT
        u.User_ID,
        u.First_name,
        u.Last_name,
        u.email,
        g.Gym_name,
        dp.Diet_name,
        dp.Diet_type,
        ms.Membership_name,
        COUNT(DISTINCT m.Gym_ID) AS Number_Of_Gyms
    FROM Users u
    JOIN Member m ON u.User_ID = m.Member_ID
    JOIN Trains tr ON m.Member_ID = tr.Member_ID
    JOIN Access_Diet_Plan adp ON m.Member_ID = adp.Member_ID
    JOIN Diet_Plan dp ON adp.Diet_ID = dp.Diet_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    WHERE tr.Trainer_ID = @Trainer_ID
      AND dp.Diet_ID = @Diet_Plan_ID
    GROUP BY u.User_ID, u.First_name, u.Last_name, u.email, g.Gym_name,
             dp.Diet_name, dp.Diet_type, ms.Membership_name
    ORDER BY u.Last_name, u.First_name;
END;
GO

-- Report 4: Count of members using specific machines on given day
CREATE OR ALTER PROCEDURE SP_Report_Machine_Usage_By_Day
    @Machine_ID INT,
    @Usage_Date DATE
AS
BEGIN
    SELECT
        m.Machine_ID,
        mach.Machine_name,
        mach.Muscle_group,
        @Usage_Date AS Usage_Date,
        COUNT(DISTINCT m.Member_ID) AS Members_Count,
        COUNT(*) AS Total_Usage_Count,
        STRING_AGG(CONCAT(u.First_name, ' ', u.Last_name), ', ') AS Member_Names
    FROM Member_Uses_Machine m
    JOIN Machine mach ON m.Machine_ID = mach.Machine_ID
    JOIN Users u ON m.Member_ID = u.User_ID
    WHERE m.Machine_ID = @Machine_ID
      AND CAST(m.Usage_start_time AS DATE) = @Usage_Date
    GROUP BY m.Machine_ID, mach.Machine_name, mach.Muscle_group;
END;
GO

-- Report 5: Diet plans with low-calorie breakfast meals (<500 calories)
CREATE OR ALTER PROCEDURE SP_Report_Diet_Plans_Low_Calorie_Breakfast
    @Max_Calories INT = 500
AS
BEGIN
    SELECT
        dp.Diet_ID,
        dp.Diet_name,
        dp.Diet_type,
        dp.Diet_goal,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        meal.Meal_name,
        meal.Meal_type,
        (meal.Protein * 4 + meal.Carbohydrates * 4 + meal.Fats * 9) AS Estimated_Calories,
        meal.Protein,
        meal.Carbohydrates,
        meal.Fats
    FROM Diet_Plan dp
    JOIN Diet_Meal dm ON dp.Diet_ID = dm.Diet_ID
    JOIN Meals meal ON dm.Meal_ID = meal.Meal_ID
    JOIN Users creator ON dp.Creator_ID = creator.User_ID
    WHERE meal.Meal_type IN ('Breakfast', 'Vegan', 'Vegetarian')
      AND (meal.Protein * 4 + meal.Carbohydrates * 4 + meal.Fats * 9) < @Max_Calories
    ORDER BY dp.Diet_name, Estimated_Calories;
END;
GO

-- Report 6: Diet plans with low total carbohydrates (<300g)
CREATE OR ALTER PROCEDURE SP_Report_Diet_Plans_Low_Carb
    @Max_Carbs INT = 300
AS
BEGIN
    SELECT
        dp.Diet_ID,
        dp.Diet_name,
        dp.Diet_type,
        dp.Diet_goal,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        SUM(meal.Carbohydrates) AS Total_Carbohydrates,
        SUM(meal.Protein) AS Total_Protein,
        SUM(meal.Fats) AS Total_Fats,
        COUNT(dm.Meal_ID) AS Meal_Count
    FROM Diet_Plan dp
    JOIN Diet_Meal dm ON dp.Diet_ID = dm.Diet_ID
    JOIN Meals meal ON dm.Meal_ID = meal.Meal_ID
    JOIN Users creator ON dp.Creator_ID = creator.User_ID
    GROUP BY dp.Diet_ID, dp.Diet_name, dp.Diet_type, dp.Diet_goal,
             creator.First_name, creator.Last_name
    HAVING SUM(meal.Carbohydrates) < @Max_Carbs
    ORDER BY Total_Carbohydrates;
END;
GO

-- Report 7: Workout plans not using specific machine
CREATE OR ALTER PROCEDURE SP_Report_Workout_Plans_Without_Machine
    @Machine_Name NVARCHAR(255)
AS
BEGIN
    SELECT
        wp.Plan_ID,
        wp.Plan_name,
        wp.Goal,
        wp.Level,
        wp.Charges,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        COUNT(we.Exercise_ID) AS Exercise_Count
    FROM Workout_Plan wp
    JOIN Users creator ON wp.Creator_ID = creator.User_ID
    LEFT JOIN Workout_Exercise we ON wp.Plan_ID = we.Plan_ID
    WHERE wp.Plan_ID NOT IN (
        SELECT DISTINCT we2.Plan_ID
        FROM Workout_Exercise we2
        JOIN Exercise e ON we2.Exercise_ID = e.Exercise_ID
        JOIN Machine_Exercise me ON e.Exercise_ID = me.Exercise_ID
        JOIN Machine m ON me.Machine_ID = m.Machine_ID
        WHERE m.Machine_name = @Machine_Name
    )
    GROUP BY wp.Plan_ID, wp.Plan_name, wp.Goal, wp.Level, wp.Charges,
             creator.First_name, creator.Last_name
    ORDER BY wp.Plan_name;
END;
GO

-- Report 8: Diet plans without specific allergen (e.g., peanuts)
CREATE OR ALTER PROCEDURE SP_Report_Diet_Plans_Without_Allergen
    @Allergen_Name NVARCHAR(50)
AS
BEGIN
    SELECT
        dp.Diet_ID,
        dp.Diet_name,
        dp.Diet_type,
        dp.Diet_goal,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        COUNT(dm.Meal_ID) AS Meal_Count
    FROM Diet_Plan dp
    JOIN Users creator ON dp.Creator_ID = creator.User_ID
    LEFT JOIN Diet_Meal dm ON dp.Diet_ID = dm.Diet_ID
    WHERE dp.Diet_ID NOT IN (
        SELECT DISTINCT dm2.Diet_ID
        FROM Diet_Meal dm2
        JOIN Meal_Allergen ma ON dm2.Meal_ID = ma.Meal_ID
        JOIN Allergen a ON ma.Allergen_ID = a.Allergen_ID
        WHERE a.Allergen_names = @Allergen_Name
    )
    GROUP BY dp.Diet_ID, dp.Diet_name, dp.Diet_type, dp.Diet_goal,
             creator.First_name, creator.Last_name
    ORDER BY dp.Diet_name;
END;
GO

-- Report 9: New membership data in last N months
CREATE OR ALTER PROCEDURE SP_Report_New_Memberships_Recent
    @Months_Back INT = 3
AS
BEGIN
    DECLARE @Start_Date DATE = DATEADD(MONTH, -@Months_Back, GETDATE());

    SELECT
        u.User_ID,
        u.First_name,
        u.Last_name,
        u.email,
        u.DOB,
        g.Gym_name,
        ms.Membership_name,
        ms.Membership_duration,
        ms.Membership_charges,
        u.Role
    FROM Users u
    JOIN Member m ON u.User_ID = m.Member_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    WHERE u.User_ID IN (
        SELECT User_ID
        FROM Users
        WHERE Role = 'Member'
    )
    ORDER BY u.User_ID DESC;
END;
GO

-- Report 10: Gym member comparison over past N months
CREATE OR ALTER PROCEDURE SP_Report_Gym_Member_Comparison
    @Months_Back INT = 6
AS
BEGIN
    SELECT
        g.Gym_ID,
        g.Gym_name,
        g.Gym_location,
        CONCAT(owner.First_name, ' ', owner.Last_name) AS Owner_Name,
        COUNT(m.Member_ID) AS Current_Member_Count,
        COUNT(DISTINCT t.Trainer_ID) AS Trainer_Count,
        AVG(ms.Membership_charges) AS Avg_Membership_Price
    FROM Gym g
    JOIN Gym_Owner go ON g.GymOwner_ID = go.GymOwner_ID
    JOIN Users owner ON go.GymOwner_ID = owner.User_ID
    LEFT JOIN Member m ON g.Gym_ID = m.Gym_ID
    LEFT JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    LEFT JOIN Works_For wf ON g.Gym_ID = wf.Gym_ID
    LEFT JOIN Trainer t ON wf.Trainer_ID = t.Trainer_ID
    GROUP BY g.Gym_ID, g.Gym_name, g.Gym_location, owner.First_name, owner.Last_name
    ORDER BY Current_Member_Count DESC;
END;
GO

-- Report 11: Trainer performance and ratings
CREATE OR ALTER PROCEDURE SP_Report_Trainer_Performance
    @Trainer_ID INT = NULL
AS
BEGIN
    SELECT
        t.Trainer_ID,
        CONCAT(u.First_name, ' ', u.Last_name) AS Trainer_Name,
        u.email,
        t.Experience,
        t.Speciality,
        COUNT(DISTINCT tr.Member_ID) AS Total_Members,
        COUNT(DISTINCT a.Appointment_ID) AS Total_Appointments,
        AVG(CAST(f.Rating AS FLOAT)) AS Average_Rating,
        COUNT(f.Feedback_ID) AS Total_Feedback_Count,
        COUNT(DISTINCT wf.Gym_ID) AS Gyms_Working_At
    FROM Trainer t
    JOIN Users u ON t.Trainer_ID = u.User_ID
    LEFT JOIN Trains tr ON t.Trainer_ID = tr.Trainer_ID
    LEFT JOIN Appointment a ON t.Trainer_ID = a.Trainer_ID
    LEFT JOIN Feedback f ON t.Trainer_ID = f.Trainer_ID
    LEFT JOIN Works_For wf ON t.Trainer_ID = wf.Trainer_ID
    WHERE @Trainer_ID IS NULL OR t.Trainer_ID = @Trainer_ID
    GROUP BY t.Trainer_ID, u.First_name, u.Last_name, u.email, t.Experience, t.Speciality
    ORDER BY Average_Rating DESC, Total_Members DESC;
END;
GO

-- Report 12: Popular workout plans (most accessed)
CREATE OR ALTER PROCEDURE SP_Report_Popular_Workout_Plans
    @Top_N INT = 10
AS
BEGIN
    SELECT TOP (@Top_N)
        wp.Plan_ID,
        wp.Plan_name,
        wp.Goal,
        wp.Level,
        wp.Charges,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        COUNT(DISTINCT awp.Member_ID) AS Access_Count,
        COUNT(DISTINCT we.Exercise_ID) AS Exercise_Count,
        AVG(we.Workout_sets * we.Workout_reps) AS Avg_Volume
    FROM Workout_Plan wp
    JOIN Users creator ON wp.Creator_ID = creator.User_ID
    LEFT JOIN Access_Workout_Plan awp ON wp.Plan_ID = awp.Plan_ID
    LEFT JOIN Workout_Exercise we ON wp.Plan_ID = we.Plan_ID
    GROUP BY wp.Plan_ID, wp.Plan_name, wp.Goal, wp.Level, wp.Charges,
             creator.First_name, creator.Last_name
    ORDER BY Access_Count DESC, Exercise_Count DESC;
END;
GO

-- Report 13: Popular diet plans (most accessed)
CREATE OR ALTER PROCEDURE SP_Report_Popular_Diet_Plans
    @Top_N INT = 10
AS
BEGIN
    SELECT TOP (@Top_N)
        dp.Diet_ID,
        dp.Diet_name,
        dp.Diet_type,
        dp.Diet_goal,
        CONCAT(creator.First_name, ' ', creator.Last_name) AS Creator_Name,
        COUNT(DISTINCT adp.Member_ID) AS Access_Count,
        COUNT(DISTINCT dm.Meal_ID) AS Meal_Count,
        SUM(m.Protein) AS Total_Protein,
        SUM(m.Carbohydrates) AS Total_Carbs,
        SUM(m.Fats) AS Total_Fats
    FROM Diet_Plan dp
    JOIN Users creator ON dp.Creator_ID = creator.User_ID
    LEFT JOIN Access_Diet_Plan adp ON dp.Diet_ID = adp.Diet_ID
    LEFT JOIN Diet_Meal dm ON dp.Diet_ID = dm.Diet_ID
    LEFT JOIN Meals m ON dm.Meal_ID = m.Meal_ID
    GROUP BY dp.Diet_ID, dp.Diet_name, dp.Diet_type, dp.Diet_goal,
             creator.First_name, creator.Last_name
    ORDER BY Access_Count DESC, Meal_Count DESC;
END;
GO

-- Report 14: Gym revenue analysis
CREATE OR ALTER PROCEDURE SP_Report_Gym_Revenue
    @Gym_ID INT = NULL
AS
BEGIN
    SELECT
        g.Gym_ID,
        g.Gym_name,
        g.Gym_location,
        CONCAT(owner.First_name, ' ', owner.Last_name) AS Owner_Name,
        COUNT(m.Member_ID) AS Total_Members,
        SUM(ms.Membership_charges) AS Total_Revenue,
        AVG(ms.Membership_charges) AS Avg_Revenue_Per_Member,
        COUNT(DISTINCT wf.Trainer_ID) AS Trainer_Count
    FROM Gym g
    JOIN Gym_Owner go ON g.GymOwner_ID = go.GymOwner_ID
    JOIN Users owner ON go.GymOwner_ID = owner.User_ID
    LEFT JOIN Member m ON g.Gym_ID = m.Gym_ID
    LEFT JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    LEFT JOIN Works_For wf ON g.Gym_ID = wf.Gym_ID
    WHERE @Gym_ID IS NULL OR g.Gym_ID = @Gym_ID
    GROUP BY g.Gym_ID, g.Gym_name, g.Gym_location, owner.First_name, owner.Last_name
    ORDER BY Total_Revenue DESC;
END;
GO

-- Report 15: Trainer appointment schedules
CREATE OR ALTER PROCEDURE SP_Report_Trainer_Appointments
    @Trainer_ID INT,
    @Start_Date DATE = NULL,
    @End_Date DATE = NULL
AS
BEGIN
    IF @Start_Date IS NULL SET @Start_Date = CAST(GETDATE() AS DATE);
    IF @End_Date IS NULL SET @End_Date = DATEADD(DAY, 30, @Start_Date);

    SELECT
        a.Appointment_ID,
        a.Appointment_date,
        a.Appointment_start_time,
        a.Appointment_end_time,
        CONCAT(member.First_name, ' ', member.Last_name) AS Member_Name,
        member.email AS Member_Email,
        g.Gym_name,
        DATEDIFF(MINUTE, a.Appointment_start_time, a.Appointment_end_time) AS Duration_Minutes
    FROM Appointment a
    JOIN Users member ON a.Member_ID = member.User_ID
    JOIN Member m ON a.Member_ID = m.Member_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    WHERE a.Trainer_ID = @Trainer_ID
      AND a.Appointment_date BETWEEN @Start_Date AND @End_Date
    ORDER BY a.Appointment_date, a.Appointment_start_time;
END;
GO

-- Report 16: Member activity metrics
CREATE OR ALTER PROCEDURE SP_Report_Member_Activity
    @Member_ID INT = NULL
AS
BEGIN
    SELECT
        u.User_ID AS Member_ID,
        CONCAT(u.First_name, ' ', u.Last_name) AS Member_Name,
        u.email,
        g.Gym_name,
        ms.Membership_name,
        COUNT(DISTINCT a.Appointment_ID) AS Appointment_Count,
        COUNT(DISTINCT mum.Machine_ID) AS Machines_Used,
        COUNT(DISTINCT awp.Plan_ID) AS Workout_Plans_Accessed,
        COUNT(DISTINCT adp.Diet_ID) AS Diet_Plans_Accessed,
        COUNT(DISTINCT f.Feedback_ID) AS Feedback_Given
    FROM Users u
    JOIN Member m ON u.User_ID = m.Member_ID
    JOIN Gym g ON m.Gym_ID = g.Gym_ID
    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
    LEFT JOIN Appointment a ON m.Member_ID = a.Member_ID
    LEFT JOIN Member_Uses_Machine mum ON m.Member_ID = mum.Member_ID
    LEFT JOIN Access_Workout_Plan awp ON m.Member_ID = awp.Member_ID
    LEFT JOIN Access_Diet_Plan adp ON m.Member_ID = adp.Member_ID
    LEFT JOIN Feedback f ON m.Member_ID = f.Member_ID
    WHERE @Member_ID IS NULL OR u.User_ID = @Member_ID
    GROUP BY u.User_ID, u.First_name, u.Last_name, u.email, g.Gym_name, ms.Membership_name
    ORDER BY Appointment_Count DESC, Machines_Used DESC;
END;
GO

-- Report 17: Workout plans by goal category
CREATE OR ALTER PROCEDURE SP_Report_Workout_Plans_By_Goal
    @Goal NVARCHAR(50) = NULL
AS
BEGIN
    SELECT
        wp.Goal,
        wp.Level,
        COUNT(*) AS Plan_Count,
        AVG(wp.Charges) AS Avg_Charges,
        AVG(CAST(exercise_counts.Exercise_Count AS FLOAT)) AS Avg_Exercises_Per_Plan,
        STRING_AGG(wp.Plan_name, ', ') AS Plan_Names
    FROM Workout_Plan wp
    LEFT JOIN (
        SELECT Plan_ID, COUNT(*) AS Exercise_Count
        FROM Workout_Exercise
        GROUP BY Plan_ID
    ) exercise_counts ON wp.Plan_ID = exercise_counts.Plan_ID
    WHERE @Goal IS NULL OR wp.Goal = @Goal
    GROUP BY wp.Goal, wp.Level
    ORDER BY wp.Goal, wp.Level;
END;
GO

-- Report 18: Diet plans by type (Vegan/Vegetarian/Non-Veg)
CREATE OR ALTER PROCEDURE SP_Report_Diet_Plans_By_Type
    @Diet_Type NVARCHAR(50) = NULL
AS
BEGIN
    SELECT
        dp.Diet_type,
        dp.Diet_goal,
        COUNT(*) AS Plan_Count,
        AVG(CAST(meal_counts.Meal_Count AS FLOAT)) AS Avg_Meals_Per_Plan,
        STRING_AGG(dp.Diet_name, ', ') AS Plan_Names
    FROM Diet_Plan dp
    LEFT JOIN (
        SELECT Diet_ID, COUNT(*) AS Meal_Count
        FROM Diet_Meal
        GROUP BY Diet_ID
    ) meal_counts ON dp.Diet_ID = meal_counts.Diet_ID
    WHERE @Diet_Type IS NULL OR dp.Diet_type = @Diet_Type
    GROUP BY dp.Diet_type, dp.Diet_goal
    ORDER BY dp.Diet_type, dp.Diet_goal;
END;
GO

-- Report 19: Pending approval requests (all types)
CREATE OR ALTER PROCEDURE SP_Report_Pending_Requests
AS
BEGIN
    -- Gym Requests
    SELECT
        'Gym Registration' AS Request_Type,
        gr.Request_ID,
        gr.Request_date,
        gr.Request_status,
        gr.Gym_name AS Details,
        gr.Gym_location AS Additional_Info,
        CONCAT(owner.First_name, ' ', owner.Last_name) AS Requester_Name,
        owner.email AS Requester_Email
    FROM Gym_Request gr
    JOIN Gym_Owner go ON gr.GymOwner_ID = go.GymOwner_ID
    JOIN Users owner ON go.GymOwner_ID = owner.User_ID
    WHERE gr.Request_status = 'Pending'

    UNION ALL

    -- Trainer Requests
    SELECT
        'Trainer Join Gym' AS Request_Type,
        tr.Request_ID,
        tr.Request_date,
        tr.Request_status,
        g.Gym_name AS Details,
        CONCAT(trainer.First_name, ' ', trainer.Last_name) AS Additional_Info,
        CONCAT(trainer.First_name, ' ', trainer.Last_name) AS Requester_Name,
        trainer.email AS Requester_Email
    FROM Trainer_Requests tr
    JOIN Gym g ON tr.Gym_ID = g.Gym_ID
    JOIN Trainer t ON tr.Trainer_ID = t.Trainer_ID
    JOIN Users trainer ON t.Trainer_ID = trainer.User_ID
    WHERE tr.Request_status = 'Pending'

    UNION ALL

    -- Appointment Requests
    SELECT
        'Appointment' AS Request_Type,
        ar.Request_ID,
        CAST(ar.Request_date AS DATE) AS Request_date,
        ar.Request_status,
        CONCAT('Appointment on ', ar.Request_date) AS Details,
        CONCAT(CAST(ar.Request_start_time AS VARCHAR), ' - ', CAST(ar.Request_end_time AS VARCHAR)) AS Additional_Info,
        CONCAT(member.First_name, ' ', member.Last_name) AS Requester_Name,
        member.email AS Requester_Email
    FROM Appointment_Requests ar
    JOIN Users member ON ar.Member_ID = member.User_ID
    WHERE ar.Request_status = 'Pending'

    ORDER BY Request_date DESC;
END;
GO

-- Report 20: System audit log report
CREATE OR ALTER PROCEDURE SP_Report_System_Audit_Log
    @Start_Date DATE = NULL,
    @End_Date DATE = NULL,
    @Activity_Type NVARCHAR(50) = NULL
AS
BEGIN
    IF @Start_Date IS NULL SET @Start_Date = DATEADD(DAY, -30, GETDATE());
    IF @End_Date IS NULL SET @End_Date = GETDATE();

    SELECT
        sl.Log_ID,
        sl.Log_date,
        sl.Activity_type,
        sl.Table_affected,
        sl.Record_ID,
        sl.User_ID,
        CONCAT(u.First_name, ' ', u.Last_name) AS User_Name,
        u.Role AS User_Role
    FROM System_Log sl
    LEFT JOIN Users u ON sl.User_ID = u.User_ID
    WHERE CAST(sl.Log_date AS DATE) BETWEEN @Start_Date AND @End_Date
      AND (@Activity_Type IS NULL OR sl.Activity_type = @Activity_Type)
    ORDER BY sl.Log_date DESC;
END;
GO

PRINT 'All 20 comprehensive report stored procedures created successfully!';
GO

-- =============================================================================
-- FLEX TRAINER DATABASE SAMPLE DATA
-- Purpose: Inserts sample data for testing and development
-- =============================================================================

USE DB_PROJECT;
GO

-- =============================================================================
-- EXERCISES
-- =============================================================================

INSERT INTO Exercise(Exercise_name, Focus_muscle) VALUES
('Bench Press', 'Chest'),
('Squats', 'Legs'),
('Deadlift', 'Back'),
('Shoulder Press', 'Shoulders'),
('Bicep Curls', 'Biceps'),
('Tricep Extensions', 'Triceps'),
('Leg Press', 'Legs'),
('Lat Pulldown', 'Back'),
('Chest Fly', 'Chest'),
('Leg Curls', 'Legs'),
('Leg Extensions', 'Legs'),
('Calf Raises', 'Legs'),
('Hammer Curls', 'Biceps'),
('Skull Crushers', 'Triceps'),
('Shoulder Shrugs', 'Shoulders'),
('Lateral Raises', 'Shoulders'),
('Front Raises', 'Shoulders'),
('Pull Ups', 'Back'),
('Push Ups', 'Chest'),
('Planks', 'Core');
GO

-- =============================================================================
-- MEMBERSHIPS
-- =============================================================================

INSERT INTO Membership (Membership_name, Membership_duration, Membership_charges) VALUES
('Basic', 1, 100),
('Silver', 3, 250),
('Gold', 6, 500),
('Platinum', 12, 1000),
('Student', 6, 400),
('Senior Citizen', 6, 300),
('Corporate', 12, 800),
('Family', 12, 1500),
('Couple', 6, 700),
('Weekend', 3, 200);
GO

-- =============================================================================
-- ALLERGENS
-- =============================================================================

INSERT INTO Allergen (Allergen_names) VALUES
('Peanuts'),
('Tree Nuts'),
('Milk'),
('Eggs'),
('Fish'),
('Shellfish'),
('Wheat'),
('Soy'),
('Sesame'),
('Mustard');
GO

-- =============================================================================
-- MEALS
-- =============================================================================

INSERT INTO Meals (Meal_name, Meal_type, Protein, Carbohydrates, Fibre, Fats) VALUES
('Breakfast Burrito', 'Vegan', 20, 30, 5, 10),
('Grilled Chicken Salad', 'Non-Vegetarian', 25, 35, 7, 15),
('Quinoa Buddha Bowl', 'Vegan', 30, 40, 10, 20),
('Salmon with Vegetables', 'Non-Vegetarian', 35, 45, 12, 25),
('Tofu Stir-Fry', 'Vegan', 40, 50, 15, 30),
('Turkey Chili', 'Non-Vegetarian', 45, 55, 17, 35),
('Grilled Shrimp Quinoa', 'Non-Vegetarian', 50, 60, 20, 40),
('Vegetable Omelette', 'Vegetarian', 55, 65, 22, 45),
('Greek Yogurt Parfait', 'Vegetarian', 60, 70, 25, 50),
('Black Bean Burger', 'Vegetarian', 65, 75, 27, 55);
GO

-- =============================================================================
-- USERS - ADMINS
-- =============================================================================

-- Disable the trigger temporarily to insert users without auto role-record creation
-- We'll manually create role records with proper data

SET IDENTITY_INSERT Users OFF;
GO

-- NOTE: All passwords are SHA256 hashed. Plain-text equivalents:
-- alice_smith: admin123 | jessica_lopez: admin456
INSERT INTO Users (First_name, Last_name, DOB, Username, Password, email, Role, Security_question) VALUES
('Alice', 'Smith', '1985-03-15', 'alice_smith', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 'alice.smith@email.com', 'Admin', 'Roots School'),
('Jessica', 'Lopez', '1983-06-27', 'jessica_lopez', 'vs938+yCpDQit3EhNNGGDjIFxs53iwhBenOJtD8rRmE=', 'jessica.lopez@email.com', 'Admin', 'Greenfield High');
GO

-- Get Admin IDs and insert into Admin table (trigger should handle this, but ensure it)
INSERT INTO Admin (Admin_ID) 
SELECT User_ID FROM Users WHERE Role = 'Admin' AND User_ID NOT IN (SELECT Admin_ID FROM Admin);
GO

-- =============================================================================
-- USERS - GYM OWNERS
-- =============================================================================

-- NOTE: Plain-text passwords: eva_brown: owner123 | james_williams: owner456 | sophia_martinez: owner789
INSERT INTO Users (First_name, Last_name, DOB, Username, Password, email, Role, Security_question) VALUES
('Eva', 'Brown', '1983-11-10', 'eva_brown', 'Q6DRcXip0myeD+mnSwtF440y8nrtiHoAilS/bgM797k=', 'eva.brown@email.com', 'GymOwner', 'Pinecrest Prep'),
('James', 'Williams', '1984-07-29', 'james_williams', 'k4FM4iMzbdle9aoqymwDJlJmMLJJJZul8r2p4gE94hQ=', 'james.williams@email.com', 'GymOwner', 'Riverdale High'),
('Sophia', 'Martinez', '1985-01-16', 'sophia_martinez', 'Tmu00+ftb6wd16ibbhTT9kQK9ZdHO5TjWQqqGoEMMDA=', 'sophia.martinez@email.com', 'GymOwner', 'Hilltop Academy');
GO

INSERT INTO Gym_Owner (GymOwner_ID)
SELECT User_ID FROM Users WHERE Role = 'GymOwner' AND User_ID NOT IN (SELECT GymOwner_ID FROM Gym_Owner);
GO

-- =============================================================================
-- GYMS (Depends on Gym_Owner)
-- =============================================================================

INSERT INTO Gym (Gym_name, Gym_location, GymOwner_ID) VALUES
('Fitness Palace', '123 Main Street', (SELECT TOP 1 GymOwner_ID FROM Gym_Owner)),
('Muscle Beach', '456 Elm Street', (SELECT TOP 1 GymOwner_ID FROM Gym_Owner)),
('Iron Gym', '789 Oak Street', (SELECT TOP 1 GymOwner_ID FROM Gym_Owner)),
('Body Sculpt', '321 Pine Street', (SELECT TOP 1 GymOwner_ID FROM Gym_Owner)),
('Powerhouse Fitness', '567 Maple Street', (SELECT TOP 1 GymOwner_ID FROM Gym_Owner));
GO

-- =============================================================================
-- USERS - TRAINERS
-- =============================================================================

-- NOTE: Plain-text passwords: bob_johnson: trainer123 | zara_garcia: trainer456 | emily_anderson: trainer789 | daniel_gonzalez: trainer101 | andrew_young: trainer102
INSERT INTO Users (First_name, Last_name, DOB, Username, Password, email, Role, Security_question) VALUES
('Bob', 'Johnson', '1988-07-20', 'bob_johnson', 'Wz0mTkzcLDnKZwiz4eIfCCcivhLmPuIUhL2+FXNasGY=', 'bob.johnson@email.com', 'Trainer', 'Palm Beach'),
('Zara', 'Garcia', '1990-06-05', 'zara_garcia', 'KSmMYxX1JHcBpwJfxtbWEPtjZe6Tpo1Nm9517ZwbVSE=', 'zara.garcia@email.com', 'Trainer', 'Valley Ridge'),
('Emily', 'Anderson', '1991-03-18', 'emily_anderson', 'c3e4K6/XXh79YV6R3d/w60+vHnzqjNNojK2PshGfeXY=', 'emily.anderson@email.com', 'Trainer', 'Bayside Prep'),
('Daniel', 'Gonzalez', '1989-02-08', 'daniel_gonzalez', 'p3+4afpAovHSc7o3pF01lQ4NXC6HRzKN7Kg94c//a6Q=', 'daniel.gonzalez@email.com', 'Trainer', 'Sunset High'),
('Andrew', 'Young', '1988-04-03', 'andrew_young', 'CLjk8Zr1AvZx2cUcI4LMFkz6AibA4fqBWvXM9+rwx9g=', 'andrew.young@email.com', 'Trainer', 'Liberty High');
GO

INSERT INTO Trainer (Trainer_ID, Experience, Speciality)
SELECT User_ID, 5, 'Strength Training' FROM Users WHERE Role = 'Trainer' AND User_ID NOT IN (SELECT Trainer_ID FROM Trainer);
GO

UPDATE Trainer SET Speciality = 'Weight Loss' WHERE Trainer_ID = (SELECT Trainer_ID FROM Trainer ORDER BY Trainer_ID OFFSET 1 ROW FETCH NEXT 1 ROW ONLY);
UPDATE Trainer SET Speciality = 'Cardio' WHERE Trainer_ID = (SELECT Trainer_ID FROM Trainer ORDER BY Trainer_ID OFFSET 2 ROWS FETCH NEXT 1 ROW ONLY);
GO

-- =============================================================================
-- USERS - MEMBERS
-- =============================================================================

-- NOTE: Plain-text passwords for members: member123-107
INSERT INTO Users (First_name, Last_name, DOB, Username, Password, email, Role, Security_question) VALUES
('John', 'Doe', '1990-01-01', 'john_doe', 'VgA3boY9L1egU1GPMkrThAsLwjSLVzrygae3y+eiKMY=', 'john.doe@email.com', 'Member', 'Roots School'),
('Jane', 'Doe', '1992-02-02', 'jane_doe', 'wjVjCy378fT9c9ZOAePMZtE6fzWLAeIA6Dy2Bwg/sZU=', 'jane.doe@email.com', 'Member', 'Greenfield High'),
('Michael', 'Lee', '1987-09-25', 'michael_lee', 'nMH4AIy9h6qMqi0aMq6MlCoU9qbxIPPLUeINyxY5UPM=', 'michael.lee@email.com', 'Member', 'Lakeview Academy'),
('Sarah', 'Wilson', '1990-08-22', 'sarah_wilson', 't/9A21ihkATnxQNFu8IYTq8R/7lqZxSKOUs9EetmV7E=', 'sarah.wilson@email.com', 'Member', 'Riverdale High'),
('Kevin', 'Hernandez', '1992-09-14', 'kevin_hernandez', 'raljqiCXR+SR/Td//EOkVXwJf44W3pJlAgt5aWf5Ed4=', 'kevin.hernandez@email.com', 'Member', 'Palm Beach'),
('Amanda', 'White', '1986-02-11', 'amanda_white', 'UQ4s6mBL3xXwRMsqA3JLPvnDFyEKEYDSawsIVgEqaYg=', 'amanda.white@email.com', 'Member', 'Bayside Prep'),
('Ryan', 'Clark', '1990-08-20', 'ryan_clark', 'YA+duPv5+W59OFlfyxLy8xv2xPwgOAHhd7TDqOmrDXs=', 'ryan.clark@email.com', 'Member', 'Valley Ridge'),
('Nicole', 'Flores', '1989-06-14', 'nicole_flores', 'Gb6HsUIHjL6pTbPqt2zgr7qdupbnYtqx149OtFe3rAg=', 'nicole.flores@email.com', 'Member', 'Sunset High'),
('Tyler', 'Garcia', '1992-10-18', 'tyler_garcia', 'yQd7dOoXEGSHgAmpyqulNVTF4CcbAS2nUZ+NDF15GzE=', 'tyler.garcia@email.com', 'Member', 'North Shore'),
('Hannah', 'Gonzalez', '1989-07-19', 'hannah_gonzalez', 'PGDHVKYb+YgXZ/ihyXWAV06nx0dRQO4YSYEDZyqGy1s=', 'hannah.gonzalez@email.com', 'Member', 'Evergreen High');
GO

INSERT INTO Member (Member_ID, Membership_ID, Gym_ID)
SELECT u.User_ID, 
       (SELECT TOP 1 Membership_ID FROM Membership ORDER BY NEWID()), 
       (SELECT TOP 1 Gym_ID FROM Gym ORDER BY NEWID())
FROM Users u 
WHERE u.Role = 'Member' AND u.User_ID NOT IN (SELECT Member_ID FROM Member);
GO

-- =============================================================================
-- WORKOUT PLANS (Depends on Users - Trainers/Members as creators)
-- =============================================================================

INSERT INTO Workout_Plan (Plan_name, Goal, Level, Charges, Creator_ID) VALUES
('Beginner Weight Loss', 'Weight Loss', 1, 50, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Intermediate Weight Loss', 'Weight Loss', 2, 100, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Advanced Weight Loss', 'Weight Loss', 3, 150, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Beginner Muscle Gain', 'Muscle Gain', 1, 60, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Intermediate Muscle Gain', 'Muscle Gain', 2, 120, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Advanced Muscle Gain', 'Muscle Gain', 3, 180, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Beginner Strength', 'Strength Gain', 1, 70, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Intermediate Strength', 'Strength Gain', 2, 140, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Advanced Strength', 'Strength Gain', 3, 210, (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Full Body Blast', 'Muscle Gain', 2, 100, (SELECT TOP 1 Trainer_ID FROM Trainer));
GO

-- =============================================================================
-- DIET PLANS
-- =============================================================================

INSERT INTO Diet_Plan (Diet_name, Diet_type, Diet_goal, Creator_ID) VALUES
('Vegan Slim', 'Vegan', 'Weight Loss', (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Vegetarian Power', 'Vegetarian', 'Muscle Gain', (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Keto Burn', 'Non-Vegetarian', 'Weight Loss', (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Paleo Muscle', 'Non-Vegetarian', 'Muscle Gain', (SELECT TOP 1 Trainer_ID FROM Trainer)),
('Mediterranean Glow', 'Non-Vegetarian', 'Weight Loss', (SELECT TOP 1 Trainer_ID FROM Trainer));
GO

-- =============================================================================
-- MACHINES
-- =============================================================================

INSERT INTO Machine (Machine_name, Machine_price, Exercise_ID) VALUES
('Treadmill', 1000, 1),
('Elliptical', 1500, 2),
('Stationary Bike', 1200, 3),
('Rowing Machine', 2000, 4),
('Leg Press Machine', 2500, 7),
('Chest Press Machine', 3000, 1),
('Lat Pulldown Machine', 3500, 8),
('Shoulder Press Machine', 4000, 4),
('Cable Machine', 4500, 5),
('Smith Machine', 5000, 2);
GO

-- =============================================================================
-- WORKOUT EXERCISES (Links exercises to plans)
-- =============================================================================

INSERT INTO Workout_Exercise (Workout_sets, Workout_reps, Rest_intervals, Exercise_ID, Plan_ID) VALUES
(3, 10, 60, 1, 1),
(3, 12, 60, 2, 1),
(4, 8, 90, 3, 2),
(4, 10, 90, 4, 2),
(5, 6, 120, 5, 3),
(3, 15, 45, 1, 4),
(4, 12, 60, 2, 4),
(4, 10, 75, 7, 5),
(5, 8, 90, 8, 5),
(3, 12, 60, 9, 6);
GO

-- =============================================================================
-- DIET MEALS (Links meals to diet plans)
-- =============================================================================

INSERT INTO Diet_Meal (Diet_ID, Meal_ID) VALUES
(1, 1), (1, 3), (1, 5),
(2, 8), (2, 9), (2, 10),
(3, 2), (3, 4),
(4, 6), (4, 7),
(5, 2), (5, 4), (5, 6);
GO

-- =============================================================================
-- MEAL ALLERGENS
-- =============================================================================

INSERT INTO Meal_Contains (Allergen_ID, Meal_ID) VALUES
(3, 8), (4, 8),
(5, 4), (6, 7),
(7, 1), (8, 5);
GO

-- =============================================================================
-- TRAINER-GYM RELATIONSHIPS
-- =============================================================================

INSERT INTO Works_For (Gym_ID, Trainer_ID)
SELECT TOP 5 g.Gym_ID, t.Trainer_ID
FROM Gym g CROSS JOIN Trainer t
WHERE NOT EXISTS (SELECT 1 FROM Works_For wf WHERE wf.Gym_ID = g.Gym_ID AND wf.Trainer_ID = t.Trainer_ID);
GO

-- =============================================================================
-- TRAINER-MEMBER RELATIONSHIPS
-- =============================================================================

INSERT INTO Trains (Trainer_ID, Member_ID)
SELECT TOP 10 t.Trainer_ID, m.Member_ID
FROM Trainer t CROSS JOIN Member m
WHERE NOT EXISTS (SELECT 1 FROM Trains tr WHERE tr.Trainer_ID = t.Trainer_ID AND tr.Member_ID = m.Member_ID);
GO

-- =============================================================================
-- GYM MEMBERSHIP HISTORY
-- =============================================================================

INSERT INTO Member_Joins_Gym (Member_ID, Gym_ID, Join_date, Leave_date)
SELECT m.Member_ID, m.Gym_ID, DATEADD(MONTH, -6, GETDATE()), NULL
FROM Member m
WHERE NOT EXISTS (SELECT 1 FROM Member_Joins_Gym mjg WHERE mjg.Member_ID = m.Member_ID);
GO

-- =============================================================================
-- WORKOUT PLAN ACCESS
-- =============================================================================

INSERT INTO Access_Workout_Plan (Member_ID, Plan_ID)
SELECT TOP 10 m.Member_ID, p.Plan_ID
FROM Member m CROSS JOIN Workout_Plan p
WHERE NOT EXISTS (SELECT 1 FROM Access_Workout_Plan awp WHERE awp.Member_ID = m.Member_ID AND awp.Plan_ID = p.Plan_ID);
GO

-- =============================================================================
-- DIET PLAN ACCESS
-- =============================================================================

INSERT INTO Access_Diet_Plan (Member_ID, Diet_ID)
SELECT TOP 10 m.Member_ID, d.Diet_ID
FROM Member m CROSS JOIN Diet_Plan d
WHERE NOT EXISTS (SELECT 1 FROM Access_Diet_Plan adp WHERE adp.Member_ID = m.Member_ID AND adp.Diet_ID = d.Diet_ID);
GO

-- =============================================================================
-- FEEDBACK
-- =============================================================================

INSERT INTO Feedback (Rating, Review, Member_ID, Trainer_ID)
SELECT 5, 'Excellent trainer!', m.Member_ID, t.Trainer_ID
FROM (SELECT TOP 1 Member_ID FROM Member) m
CROSS JOIN (SELECT TOP 1 Trainer_ID FROM Trainer) t;
GO

INSERT INTO Feedback (Rating, Review, Member_ID, Trainer_ID)
SELECT 4, 'Very helpful and motivating.', m.Member_ID, t.Trainer_ID
FROM (SELECT TOP 1 Member_ID FROM Member ORDER BY Member_ID DESC) m
CROSS JOIN (SELECT TOP 1 Trainer_ID FROM Trainer) t;
GO

-- =============================================================================
-- GYM REQUESTS (Sample pending requests)
-- =============================================================================

INSERT INTO Gym_Request (Request_date, Request_status, Gym_name, Gym_location, Admin_ID, GymOwner_ID) VALUES
('2024-01-15', 'Pending', 'New Fitness Hub', '100 Broadway', 
    (SELECT TOP 1 Admin_ID FROM Admin), (SELECT TOP 1 GymOwner_ID FROM Gym_Owner)),
('2024-01-20', 'Pending', 'Elite Athletics', '200 Park Ave', 
    (SELECT TOP 1 Admin_ID FROM Admin), (SELECT TOP 1 GymOwner_ID FROM Gym_Owner ORDER BY GymOwner_ID DESC));
GO

-- =============================================================================
-- APPOINTMENT REQUESTS
-- =============================================================================

INSERT INTO Appointment_Requests (Request_date, Request_start_time, Request_end_time, Request_status, Member_ID, Trainer_ID)
SELECT '2024-02-01', '10:00:00', '11:00:00', 'Pending', m.Member_ID, t.Trainer_ID
FROM (SELECT TOP 1 Member_ID FROM Member) m
CROSS JOIN (SELECT TOP 1 Trainer_ID FROM Trainer) t;
GO

INSERT INTO Appointment_Requests (Request_date, Request_start_time, Request_end_time, Request_status, Member_ID, Trainer_ID)
SELECT '2024-02-02', '14:00:00', '15:00:00', 'Approved', m.Member_ID, t.Trainer_ID
FROM (SELECT TOP 1 Member_ID FROM Member ORDER BY Member_ID DESC) m
CROSS JOIN (SELECT TOP 1 Trainer_ID FROM Trainer ORDER BY Trainer_ID DESC) t;
GO

-- =============================================================================
-- APPOINTMENTS (From approved requests)
-- =============================================================================

INSERT INTO Appointment (Appointment_date, Appointment_start_time, Appointment_end_time, Member_ID, Trainer_ID)
SELECT ar.Request_date, ar.Request_start_time, ar.Request_end_time, ar.Member_ID, ar.Trainer_ID
FROM Appointment_Requests ar
WHERE ar.Request_status = 'Approved';
GO

-- =============================================================================
-- VERIFICATION QUERIES
-- =============================================================================

PRINT 'Sample data inserted successfully!';
PRINT '';
PRINT 'Data Summary:';
GO

SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL SELECT 'Admins', COUNT(*) FROM Admin
UNION ALL SELECT 'Gym_Owners', COUNT(*) FROM Gym_Owner
UNION ALL SELECT 'Trainers', COUNT(*) FROM Trainer
UNION ALL SELECT 'Members', COUNT(*) FROM Member
UNION ALL SELECT 'Gyms', COUNT(*) FROM Gym
UNION ALL SELECT 'Exercises', COUNT(*) FROM Exercise
UNION ALL SELECT 'Workout_Plans', COUNT(*) FROM Workout_Plan
UNION ALL SELECT 'Diet_Plans', COUNT(*) FROM Diet_Plan
UNION ALL SELECT 'Meals', COUNT(*) FROM Meals;
GO

-- =============================================
-- Flex Trainer Database - Complete Setup Script
-- Run this script to create the entire database
-- =============================================

-- Run each file in order:
-- 1. 01_schema.sql       - Creates database and tables
-- 2. 02_triggers.sql     - Creates audit trail triggers
-- 3. 03_procedures.sql   - Creates stored procedures
-- 4. 04_sample_data.sql  - Inserts sample data (50+ users)

-- =============================================
-- QUICK START INSTRUCTIONS
-- =============================================
-- 
-- Option 1: Docker (Recommended)
-- ------------------------------
-- 1. Run: docker-compose up -d
-- 2. Wait 30 seconds for SQL Server to start
-- 3. Connect using:
--    - Server: localhost,1433
--    - Username: sa
--    - Password: FlexTrainer2024!
-- 4. Execute scripts in order (01 through 05)
--
-- Option 2: Manual SQL Server
-- ----------------------------
-- 1. Open SQL Server Management Studio
-- 2. Connect to your local SQL Server instance
-- 3. Execute scripts in order (01 through 05)
--
-- =============================================
-- CONNECTION STRING FOR C# APPLICATION
-- =============================================
-- 
-- For Docker:
-- "Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True"
--
-- For Windows SQL Server:
-- "Data Source=YOUR_SERVER_NAME;Initial Catalog=DB_PROJECT;Integrated Security=True"
--
-- =============================================

-- Verify database was created successfully
USE DB_PROJECT;
GO

SELECT 
    'Database created successfully!' AS Status,
    (SELECT COUNT(*) FROM Users) AS TotalUsers,
    (SELECT COUNT(*) FROM Gym) AS TotalGyms,
    (SELECT COUNT(*) FROM Workout_Plan) AS TotalWorkoutPlans,
    (SELECT COUNT(*) FROM Diet_Plan) AS TotalDietPlans;
GO

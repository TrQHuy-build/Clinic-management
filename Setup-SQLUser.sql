-- ========================================
-- SETUP SQL USER FOR CLINIC DATABASE
-- ========================================

USE master;
GO

-- Táº¡o Login náº¿u chÆ°a tá»“n táº¡i
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ClinicUser')
BEGIN
    CREATE LOGIN [ClinicUser] WITH PASSWORD = 'Clinic@User2026', CHECK_POLICY = OFF;
    PRINT 'âœ… Login ClinicUser created';
END
ELSE
BEGIN
    PRINT 'âš ï¸  Login ClinicUser already exists';
END
GO

-- Chuyá»ƒn sang database
USE DentalClinicDB;
GO

-- Táº¡o User náº¿u chÆ°a tá»“n táº¡i
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ClinicUser')
BEGIN
    CREATE USER [ClinicUser] FOR LOGIN [ClinicUser];
    PRINT 'âœ… User ClinicUser created in database';
END
ELSE
BEGIN
    PRINT 'âš ï¸  User ClinicUser already exists in database';
END
GO

-- Cáº¥p quyá»n
ALTER ROLE db_owner ADD MEMBER [ClinicUser];
PRINT 'âœ… User ClinicUser added to db_owner role';
GO

-- Verify
SELECT 
    'Server: ' + @@SERVERNAME as Info
UNION ALL
SELECT 
    'Database: ' + DB_NAME()
UNION ALL
SELECT 
    'User: ClinicUser - Permissions: db_owner';
GO

PRINT '';
PRINT '========================================';
PRINT 'âœ… SETUP COMPLETED!';
PRINT '========================================';
PRINT '';
PRINT 'Connection string for clients:';
PRINT 'Data Source=10.128.43.222\SQLEXPRESS,1433;Initial Catalog=DentalClinicDB;User ID=ClinicUser;Password=Clinic@User2026;TrustServerCertificate=True';
PRINT '';

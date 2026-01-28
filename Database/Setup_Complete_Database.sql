-- =============================================
-- COMPLETE DATABASE SETUP
-- Create database, schema, and seed large data
-- =============================================

PRINT '=========================================='
PRINT 'COMPLETE DATABASE SETUP'
PRINT 'Start Time: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- Run SQLQuery2.sql (Create database and all tables)
PRINT 'Step 1: Running SQLQuery2.sql (Create database and tables)...'
PRINT 'Please run: sqlcmd -S "(localdb)\MSSQLLocalDB" -i "Database\SQLQuery2.sql"'
PRINT ''

-- Run Migration_Complete_Update.sql
PRINT 'Step 2: Running Migration_Complete_Update.sql (Add new columns and tables)...'
PRINT 'Please run: sqlcmd -S "(localdb)\MSSQLLocalDB" -d DentalClinicDB -i "Database\Migration_Complete_Update.sql"'
PRINT ''

-- Run Seed_Services.sql
PRINT 'Step 3: Running Seed_Services.sql (Add 25 dental services)...'
PRINT 'Please run: sqlcmd -S "(localdb)\MSSQLLocalDB" -d DentalClinicDB -i "Database\Seed_Services.sql"'
PRINT ''

-- Run Seed_Large_Data_2024_2026.sql
PRINT 'Step 4: Running Seed_Large_Data_2024_2026.sql (Create 2 years of data)...'
PRINT 'Please run: sqlcmd -S "(localdb)\MSSQLLocalDB" -d DentalClinicDB -i "Database\Seed_Large_Data_2024_2026.sql"'
PRINT ''

PRINT '=========================================='
PRINT 'SETUP COMPLETE'
PRINT '=========================================='

-- ========================================
-- MIGRATION: Add reject_reason column to Appointment table
-- Date: 2025-01-15
-- Purpose: Allow doctors to provide rejection reasons for appointments
-- ========================================

USE DentalClinicDB;
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('Appointment') 
    AND name = 'reject_reason'
)
BEGIN
    -- Add reject_reason column
    ALTER TABLE Appointment
    ADD reject_reason NVARCHAR(500) NULL;
    
    PRINT '? Successfully added reject_reason column to Appointment table';
END
ELSE
BEGIN
    PRINT '?? Column reject_reason already exists in Appointment table';
END
GO

-- Verify the change
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Appointment'
AND COLUMN_NAME = 'reject_reason';
GO

PRINT '========================================';
PRINT 'Migration completed successfully!';
PRINT '========================================';

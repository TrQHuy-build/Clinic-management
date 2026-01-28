-- =============================================
-- Script: Update Shift table for registration feature
-- Date: January 29, 2026
-- =============================================

USE DentalClinicDB;
GO

-- Thêm cột shift_type nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Shift') AND name = 'shift_type')
BEGIN
    ALTER TABLE Shift ADD shift_type NVARCHAR(10) DEFAULT 'AM';
    PRINT 'Added column: shift_type';
END
GO

-- Thêm cột max_patients nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Shift') AND name = 'max_patients')
BEGIN
    ALTER TABLE Shift ADD max_patients INT DEFAULT 10;
    PRINT 'Added column: max_patients';
END
GO

-- Cập nhật shift_type cho các record cũ dựa trên start_time
UPDATE Shift
SET shift_type = CASE 
    WHEN start_time < '12:00:00' THEN 'AM'
    ELSE 'PM'
END
WHERE shift_type IS NULL;
GO

PRINT 'Shift table updated successfully!';

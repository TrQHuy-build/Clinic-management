-- ============================================
-- SIMPLIFIED CRITICAL FIXES MIGRATION
-- Compatible with current database schema
-- Date: 2026-01-28
-- ============================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'STARTING SIMPLIFIED MIGRATION';
PRINT '========================================';
GO

-- ============================================
-- 1. ADD MISSING COLUMNS TO APPOINTMENT
-- ============================================
PRINT 'Adding columns to Appointment table...';

-- Add appointment_code
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'appointment_code')
BEGIN
    ALTER TABLE Appointment ADD appointment_code NVARCHAR(20) NULL;
    
    -- Generate codes for existing appointments
    UPDATE Appointment
    SET appointment_code = 'APT' + RIGHT('000000' + CAST(appointment_id AS NVARCHAR), 6)
    WHERE appointment_code IS NULL;
    
    -- Make NOT NULL and add unique constraint
    ALTER TABLE Appointment ALTER COLUMN appointment_code NVARCHAR(20) NOT NULL;
    ALTER TABLE Appointment ADD CONSTRAINT UQ_Appointment_Code UNIQUE (appointment_code);
    
    PRINT '  + Added appointment_code';
END
GO

-- Add check_in_time
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'check_in_time')
BEGIN
    ALTER TABLE Appointment ADD check_in_time DATETIME NULL;
    PRINT '  + Added check_in_time';
END
GO

-- Add queue_number
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'queue_number')
BEGIN
    ALTER TABLE Appointment ADD queue_number INT NULL;
    PRINT '  + Added queue_number';
END
GO

-- Add estimated_start_time
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'estimated_start_time')
BEGIN
    ALTER TABLE Appointment ADD estimated_start_time DATETIME NULL;
    PRINT '  + Added estimated_start_time';
END
GO

-- Add created_at
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'created_at')
BEGIN
    ALTER TABLE Appointment ADD created_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '  + Added created_at';
END
GO

-- Add updated_at
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'updated_at')
BEGIN
    ALTER TABLE Appointment ADD updated_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '  + Added updated_at';
END
GO

-- Add is_deleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'is_deleted')
BEGIN
    ALTER TABLE Appointment ADD is_deleted BIT NOT NULL DEFAULT 0;
    PRINT '  + Added is_deleted';
END
GO

-- ============================================
-- 2. ADD MISSING COLUMNS TO MEDICINE
-- ============================================
PRINT 'Adding columns to Medicine table...';

-- Rename 'name' to 'medicine_name' if needed
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'name')
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'medicine_name')
BEGIN
    EXEC sp_rename 'Medicine.name', 'medicine_name', 'COLUMN';
    PRINT '  + Renamed name to medicine_name';
END
GO

-- Add stock_quantity
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'stock_quantity')
BEGIN
    ALTER TABLE Medicine ADD stock_quantity INT NOT NULL DEFAULT 0;
    
    -- Initialize with 100 units
    UPDATE Medicine SET stock_quantity = 100;
    
    PRINT '  + Added stock_quantity';
END
GO

-- Add expiry_date
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'expiry_date')
BEGIN
    ALTER TABLE Medicine ADD expiry_date DATE NULL;
    
    -- Initialize with 2 years from now
    UPDATE Medicine SET expiry_date = DATEADD(YEAR, 2, GETDATE());
    
    PRINT '  + Added expiry_date';
END
GO

-- Add created_at
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'created_at')
BEGIN
    ALTER TABLE Medicine ADD created_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '  + Added created_at to Medicine';
END
GO

-- Add is_deleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Medicine') AND name = 'is_deleted')
BEGIN
    ALTER TABLE Medicine ADD is_deleted BIT NOT NULL DEFAULT 0;
    PRINT '  + Added is_deleted to Medicine';
END
GO

-- ============================================
-- 3. ADD MISSING COLUMNS TO INVOICE
-- ============================================
PRINT 'Adding columns to Invoice table...';

-- Add created_at
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'created_at')
BEGIN
    ALTER TABLE Invoice ADD created_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '  + Added created_at to Invoice';
END
GO

-- Add is_deleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'is_deleted')
BEGIN
    ALTER TABLE Invoice ADD is_deleted BIT NOT NULL DEFAULT 0;
    PRINT '  + Added is_deleted to Invoice';
END
GO

-- ============================================
-- 4. ADD MISSING COLUMNS TO MEDICALRECORD
-- ============================================
PRINT 'Adding columns to MedicalRecord table...';

-- Add created_at
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MedicalRecord') AND name = 'created_at')
BEGIN
    ALTER TABLE MedicalRecord ADD created_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '  + Added created_at to MedicalRecord';
END
GO

-- Add is_deleted
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MedicalRecord') AND name = 'is_deleted')
BEGIN
    ALTER TABLE MedicalRecord ADD is_deleted BIT NOT NULL DEFAULT 0;
    PRINT '  + Added is_deleted to MedicalRecord';
END
GO

-- ============================================
-- 5. RE-CREATE FUNCTIONS WITH CORRECT SCHEMA
-- ============================================
PRINT 'Re-creating functions...';

IF OBJECT_ID('dbo.fn_IsSlotAvailable', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_IsSlotAvailable;
GO

CREATE FUNCTION dbo.fn_IsSlotAvailable
(
    @AppointmentDate DATETIME,
    @DoctorId INT = NULL,
    @MaxSlotsPerTime INT = 4
)
RETURNS BIT
AS
BEGIN
    DECLARE @CurrentCount INT;
    
    SELECT @CurrentCount = COUNT(*)
    FROM Appointment
    WHERE appointment_date = @AppointmentDate
        AND status NOT IN ('cancelled', 'rejected', 'completed')
        AND is_deleted = 0
        AND (@DoctorId IS NULL OR assigned_doctor_id = @DoctorId);
    
    RETURN CASE WHEN @CurrentCount < @MaxSlotsPerTime THEN 1 ELSE 0 END;
END
GO

PRINT '  + Created fn_IsSlotAvailable';
GO

-- ============================================
-- 6. RE-CREATE STORED PROCEDURES
-- ============================================
PRINT 'Re-creating stored procedures...';

IF OBJECT_ID('dbo.sp_CheckSlotConflict', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CheckSlotConflict;
GO

CREATE PROCEDURE dbo.sp_CheckSlotConflict
    @AppointmentDate DATETIME,
    @DoctorId INT = NULL,
    @IsAvailable BIT OUTPUT,
    @CurrentCount INT OUTPUT,
    @MaxSlots INT = 4
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT @CurrentCount = COUNT(*)
    FROM Appointment
    WHERE appointment_date = @AppointmentDate
        AND status NOT IN ('cancelled', 'rejected', 'completed')
        AND is_deleted = 0
        AND (@DoctorId IS NULL OR assigned_doctor_id = @DoctorId);
    
    SET @IsAvailable = CASE WHEN @CurrentCount < @MaxSlots THEN 1 ELSE 0 END;
    
    RETURN 0;
END
GO

PRINT '  + Created sp_CheckSlotConflict';
GO

-- ============================================
-- 7. RE-CREATE VIEWS
-- ============================================
PRINT 'Re-creating views...';

IF OBJECT_ID('dbo.vw_AppointmentFull', 'V') IS NOT NULL
    DROP VIEW dbo.vw_AppointmentFull;
GO

CREATE VIEW dbo.vw_AppointmentFull
AS
SELECT 
    a.appointment_id,
    a.appointment_code,
    a.appointment_date,
    a.status,
    a.notes,
    a.check_in_time,
    a.queue_number,
    a.estimated_start_time,
    
    -- Patient info (fallback to denormalized fields)
    ISNULL(u.fullname, a.patient_name) AS patient_name,
    ISNULL(u.phone, a.phone) AS patient_phone,
    ISNULL(u.email, a.email) AS patient_email,
    p.patient_id,
    
    -- Service info
    s.service_id,
    s.service_name,
    s.price AS service_price,
    
    -- Doctor info
    st.staff_id AS doctor_id,
    doctor_u.fullname AS doctor_name,
    st.specialization,
    
    -- Timestamps
    a.created_at,
    a.updated_at
FROM Appointment a
LEFT JOIN Patient p ON a.patient_id = p.patient_id
LEFT JOIN UserAccount u ON p.user_id = u.user_id
LEFT JOIN Service s ON a.service_id = s.service_id
LEFT JOIN Staff st ON a.assigned_doctor_id = st.staff_id
LEFT JOIN UserAccount doctor_u ON st.user_id = doctor_u.user_id
WHERE a.is_deleted = 0;
GO

PRINT '  + Created vw_AppointmentFull';
GO

-- ============================================
-- 8. RE-CREATE TRIGGERS
-- ============================================
PRINT 'Re-creating triggers...';

IF OBJECT_ID('dbo.trg_Prescription_AfterInsert', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Prescription_AfterInsert;
GO

CREATE TRIGGER dbo.trg_Prescription_AfterInsert
ON Prescription
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Trừ stock khi tạo prescription
    UPDATE m
    SET m.stock_quantity = m.stock_quantity - i.quantity
    FROM Medicine m
    INNER JOIN inserted i ON m.medicine_id = i.medicine_id
    WHERE m.stock_quantity >= i.quantity;
    
    -- Log warning nếu stock thấp (nếu có AuditLog table)
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLog')
    BEGIN
        INSERT INTO AuditLog (action, details, created_at)
        SELECT 
            'LOW_STOCK_WARNING',
            'Medicine ' + m.medicine_name + ' is running low. Current stock: ' + CAST(m.stock_quantity AS NVARCHAR),
            GETDATE()
        FROM Medicine m
        INNER JOIN inserted i ON m.medicine_id = i.medicine_id
        WHERE m.stock_quantity < 10;
        
        -- Log error nếu out of stock
        INSERT INTO AuditLog (action, details, created_at)
        SELECT 
            'OUT_OF_STOCK_ERROR',
            'Medicine ' + m.medicine_name + ' is OUT OF STOCK! Cannot fulfill prescription.',
            GETDATE()
        FROM Medicine m
        INNER JOIN inserted i ON m.medicine_id = i.medicine_id
        WHERE m.stock_quantity <= 0;
    END
END
GO

PRINT '  + Created trg_Prescription_AfterInsert';
GO

-- ============================================
-- 9. CREATE PERFORMANCE INDEXES
-- ============================================
PRINT 'Creating performance indexes...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Date_Status')
BEGIN
    CREATE INDEX IX_Appointment_Date_Status ON Appointment(appointment_date, status);
    PRINT '  + Created IX_Appointment_Date_Status';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_PatientId')
BEGIN
    CREATE INDEX IX_Appointment_PatientId ON Appointment(patient_id);
    PRINT '  + Created IX_Appointment_PatientId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_AssignedDoctor')
BEGIN
    CREATE INDEX IX_Appointment_AssignedDoctor ON Appointment(assigned_doctor_id);
    PRINT '  + Created IX_Appointment_AssignedDoctor';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicine_StockQuantity')
BEGIN
    CREATE INDEX IX_Medicine_StockQuantity ON Medicine(stock_quantity);
    PRINT '  + Created IX_Medicine_StockQuantity';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicine_ExpiryDate')
BEGIN
    CREATE INDEX IX_Medicine_ExpiryDate ON Medicine(expiry_date);
    PRINT '  + Created IX_Medicine_ExpiryDate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_AppointmentId')
   AND EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailLog')
BEGIN
    CREATE INDEX IX_EmailLog_AppointmentId ON EmailLog(appointment_id);
    PRINT '  + Created IX_EmailLog_AppointmentId';
END
GO

-- ============================================
-- MIGRATION COMPLETE
-- ============================================
PRINT '';
PRINT '========================================';
PRINT 'SIMPLIFIED MIGRATION COMPLETED!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '1. ✓ Appointment table enhanced (appointment_code, queue, timestamps)';
PRINT '2. ✓ Medicine inventory tracking (stock_quantity, expiry_date)';
PRINT '3. ✓ Soft delete support (is_deleted columns)';
PRINT '4. ✓ Functions created (fn_IsSlotAvailable)';
PRINT '5. ✓ Procedures created (sp_CheckSlotConflict)';
PRINT '6. ✓ Views created (vw_AppointmentFull)';
PRINT '7. ✓ Triggers created (trg_Prescription_AfterInsert)';
PRINT '8. ✓ Performance indexes created';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Configure SMTP in App.config';
PRINT '2. Start EmailReminderService in Program.cs';
PRINT '3. Test all features';
PRINT '';
GO

-- ============================================
-- Database Migration Script
-- Issue #8: Database Design Issues
-- Version: 1.0
-- Date: 2026-01-28
-- ============================================

USE DentalManagement;
GO

-- ============================================
-- PART 1: Fix Denormalization Issues
-- ============================================

PRINT 'PART 1: Fixing Appointments Table Denormalization...';
GO

-- Check if Appointments table has denormalized columns
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'patient_name')
BEGIN
    PRINT 'Step 1: Creating backup of Appointments table...';
    
    -- Create backup table
    SELECT * INTO Appointments_Backup_20260128
    FROM Appointments;
    
    PRINT 'Backup created: Appointments_Backup_20260128';
    
    -- Drop denormalized columns (if they exist)
    PRINT 'Step 2: Dropping denormalized columns...';
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'patient_name')
    BEGIN
        ALTER TABLE Appointments DROP COLUMN patient_name;
        PRINT '  - Dropped column: patient_name';
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'phone')
    BEGIN
        ALTER TABLE Appointments DROP COLUMN phone;
        PRINT '  - Dropped column: phone';
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'email')
    BEGIN
        ALTER TABLE Appointments DROP COLUMN email;
        PRINT '  - Dropped column: email';
    END
    
    PRINT 'Denormalized columns removed successfully!';
END
ELSE
BEGIN
    PRINT 'Appointments table structure is already normalized.';
END
GO

-- ============================================
-- PART 2: Add Soft Delete Support
-- ============================================

PRINT '';
PRINT 'PART 2: Adding Soft Delete Support...';
GO

-- Add soft delete columns to all main tables
DECLARE @tables TABLE (TableName NVARCHAR(100));
INSERT INTO @tables VALUES 
    ('Patients'),
    ('Staff'),
    ('Services'),
    ('Appointments'),
    ('Invoices'),
    ('InvoiceDetails'),
    ('MedicalRecords'),
    ('Prescriptions'),
    ('Medicines'),
    ('Inventory'),
    ('Users');

DECLARE @tableName NVARCHAR(100);
DECLARE @sql NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR SELECT TableName FROM @tables;
OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if table exists
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName)
    BEGIN
        PRINT 'Processing table: ' + @tableName;
        
        -- Add is_deleted column
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = @tableName AND COLUMN_NAME = 'is_deleted')
        BEGIN
            SET @sql = N'ALTER TABLE ' + @tableName + N' ADD is_deleted BIT NOT NULL DEFAULT 0';
            EXEC sp_executesql @sql;
            PRINT '  - Added column: is_deleted';
        END
        ELSE
        BEGIN
            PRINT '  - Column is_deleted already exists';
        END
        
        -- Add deleted_at column
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = @tableName AND COLUMN_NAME = 'deleted_at')
        BEGIN
            SET @sql = N'ALTER TABLE ' + @tableName + N' ADD deleted_at DATETIME NULL';
            EXEC sp_executesql @sql;
            PRINT '  - Added column: deleted_at';
        END
        ELSE
        BEGIN
            PRINT '  - Column deleted_at already exists';
        END
        
        -- Add deleted_by column
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = @tableName AND COLUMN_NAME = 'deleted_by')
        BEGIN
            SET @sql = N'ALTER TABLE ' + @tableName + N' ADD deleted_by NVARCHAR(100) NULL';
            EXEC sp_executesql @sql;
            PRINT '  - Added column: deleted_by';
        END
        ELSE
        BEGIN
            PRINT '  - Column deleted_by already exists';
        END
    END
    ELSE
    BEGIN
        PRINT 'Table ' + @tableName + ' does not exist, skipping...';
    END
    
    FETCH NEXT FROM table_cursor INTO @tableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;
GO

PRINT '';
PRINT 'Soft delete columns added successfully!';
GO

-- ============================================
-- PART 3: Create Indexes for Performance
-- ============================================

PRINT '';
PRINT 'PART 3: Creating Performance Indexes...';
GO

-- Patients Table Indexes
PRINT 'Creating indexes for Patients table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Patients_Email' AND object_id = OBJECT_ID('Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_Email ON Patients(email);
    PRINT '  - Created: IX_Patients_Email';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Patients_Phone' AND object_id = OBJECT_ID('Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_Phone ON Patients(phone);
    PRINT '  - Created: IX_Patients_Phone';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Patients_IsDeleted' AND object_id = OBJECT_ID('Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_IsDeleted ON Patients(is_deleted) INCLUDE (id, full_name, email);
    PRINT '  - Created: IX_Patients_IsDeleted (with INCLUDE)';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Patients_FullName' AND object_id = OBJECT_ID('Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_FullName ON Patients(full_name);
    PRINT '  - Created: IX_Patients_FullName';
END

-- Staff Table Indexes
PRINT 'Creating indexes for Staff table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Staff_Email' AND object_id = OBJECT_ID('Staff'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Staff_Email ON Staff(email);
    PRINT '  - Created: IX_Staff_Email';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Staff_Position' AND object_id = OBJECT_ID('Staff'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Staff_Position ON Staff(position);
    PRINT '  - Created: IX_Staff_Position';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Staff_IsDeleted' AND object_id = OBJECT_ID('Staff'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Staff_IsDeleted ON Staff(is_deleted);
    PRINT '  - Created: IX_Staff_IsDeleted';
END

-- Appointments Table Indexes
PRINT 'Creating indexes for Appointments table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_PatientId' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_PatientId ON Appointments(patient_id);
    PRINT '  - Created: IX_Appointments_PatientId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_DoctorId' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_DoctorId ON Appointments(doctor_id);
    PRINT '  - Created: IX_Appointments_DoctorId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_AppointmentDate' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_AppointmentDate ON Appointments(appointment_date);
    PRINT '  - Created: IX_Appointments_AppointmentDate';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_Status' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_Status ON Appointments(status);
    PRINT '  - Created: IX_Appointments_Status';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_IsDeleted' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_IsDeleted ON Appointments(is_deleted);
    PRINT '  - Created: IX_Appointments_IsDeleted';
END

-- Composite index for common queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_DoctorDate' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Appointments_DoctorDate 
    ON Appointments(doctor_id, appointment_date) 
    INCLUDE (status, time_slot);
    PRINT '  - Created: IX_Appointments_DoctorDate (composite with INCLUDE)';
END

-- Invoices Table Indexes
PRINT 'Creating indexes for Invoices table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_PatientId' AND object_id = OBJECT_ID('Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_PatientId ON Invoices(patient_id);
    PRINT '  - Created: IX_Invoices_PatientId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_InvoiceDate' AND object_id = OBJECT_ID('Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_InvoiceDate ON Invoices(invoice_date);
    PRINT '  - Created: IX_Invoices_InvoiceDate';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_PaymentStatus' AND object_id = OBJECT_ID('Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_PaymentStatus ON Invoices(payment_status);
    PRINT '  - Created: IX_Invoices_PaymentStatus';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_IsDeleted' AND object_id = OBJECT_ID('Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_IsDeleted ON Invoices(is_deleted);
    PRINT '  - Created: IX_Invoices_IsDeleted';
END

-- InvoiceDetails Table Indexes
PRINT 'Creating indexes for InvoiceDetails table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InvoiceDetails_InvoiceId' AND object_id = OBJECT_ID('InvoiceDetails'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InvoiceDetails_InvoiceId ON InvoiceDetails(invoice_id);
    PRINT '  - Created: IX_InvoiceDetails_InvoiceId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InvoiceDetails_ServiceId' AND object_id = OBJECT_ID('InvoiceDetails'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InvoiceDetails_ServiceId ON InvoiceDetails(service_id);
    PRINT '  - Created: IX_InvoiceDetails_ServiceId';
END

-- MedicalRecords Table Indexes
PRINT 'Creating indexes for MedicalRecords table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MedicalRecords_PatientId' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MedicalRecords_PatientId ON MedicalRecords(patient_id);
    PRINT '  - Created: IX_MedicalRecords_PatientId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MedicalRecords_DoctorId' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MedicalRecords_DoctorId ON MedicalRecords(doctor_id);
    PRINT '  - Created: IX_MedicalRecords_DoctorId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MedicalRecords_VisitDate' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MedicalRecords_VisitDate ON MedicalRecords(visit_date);
    PRINT '  - Created: IX_MedicalRecords_VisitDate';
END

-- Services Table Indexes
PRINT 'Creating indexes for Services table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Services_IsActive' AND object_id = OBJECT_ID('Services'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Services_IsActive ON Services(is_active);
    PRINT '  - Created: IX_Services_IsActive';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Services_IsDeleted' AND object_id = OBJECT_ID('Services'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Services_IsDeleted ON Services(is_deleted);
    PRINT '  - Created: IX_Services_IsDeleted';
END

-- Medicines Table Indexes
PRINT 'Creating indexes for Medicines table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicines_MedicineName' AND object_id = OBJECT_ID('Medicines'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Medicines_MedicineName ON Medicines(medicine_name);
    PRINT '  - Created: IX_Medicines_MedicineName';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicines_ExpiryDate' AND object_id = OBJECT_ID('Medicines'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Medicines_ExpiryDate ON Medicines(expiry_date);
    PRINT '  - Created: IX_Medicines_ExpiryDate';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicines_IsAvailable' AND object_id = OBJECT_ID('Medicines'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Medicines_IsAvailable ON Medicines(is_available);
    PRINT '  - Created: IX_Medicines_IsAvailable';
END

-- Users Table Indexes
PRINT 'Creating indexes for Users table...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON Users(username) WHERE username IS NOT NULL;
    PRINT '  - Created: IX_Users_Username (UNIQUE)';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(email);
    PRINT '  - Created: IX_Users_Email';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Users_Role ON Users(role);
    PRINT '  - Created: IX_Users_Role';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Users_IsActive ON Users(is_active);
    PRINT '  - Created: IX_Users_IsActive';
END

GO

PRINT '';
PRINT 'All indexes created successfully!';
GO

-- ============================================
-- PART 4: Create Views for Active Records
-- ============================================

PRINT '';
PRINT 'PART 4: Creating Views for Active Records...';
GO

-- View for active patients
IF OBJECT_ID('vw_ActivePatients', 'V') IS NOT NULL
    DROP VIEW vw_ActivePatients;
GO

CREATE VIEW vw_ActivePatients AS
SELECT 
    id,
    full_name,
    email,
    phone,
    date_of_birth,
    gender,
    address,
    created_at,
    updated_at
FROM Patients
WHERE is_deleted = 0;
GO

PRINT 'Created view: vw_ActivePatients';

-- View for active appointments
IF OBJECT_ID('vw_ActiveAppointments', 'V') IS NOT NULL
    DROP VIEW vw_ActiveAppointments;
GO

CREATE VIEW vw_ActiveAppointments AS
SELECT 
    a.id,
    a.appointment_code,
    a.patient_id,
    p.full_name AS patient_name,
    p.phone AS patient_phone,
    p.email AS patient_email,
    a.doctor_id,
    s.full_name AS doctor_name,
    a.service_id,
    sv.service_name,
    a.appointment_date,
    a.time_slot,
    a.status,
    a.notes,
    a.created_at
FROM Appointments a
INNER JOIN Patients p ON a.patient_id = p.id AND p.is_deleted = 0
INNER JOIN Staff s ON a.doctor_id = s.id AND s.is_deleted = 0
LEFT JOIN Services sv ON a.service_id = sv.id AND sv.is_deleted = 0
WHERE a.is_deleted = 0;
GO

PRINT 'Created view: vw_ActiveAppointments';

-- View for active invoices
IF OBJECT_ID('vw_ActiveInvoices', 'V') IS NOT NULL
    DROP VIEW vw_ActiveInvoices;
GO

CREATE VIEW vw_ActiveInvoices AS
SELECT 
    i.id,
    i.invoice_code,
    i.patient_id,
    p.full_name AS patient_name,
    i.invoice_date,
    i.total_amount,
    i.discount,
    i.final_amount,
    i.payment_status,
    i.created_by,
    i.created_at
FROM Invoices i
INNER JOIN Patients p ON i.patient_id = p.id AND p.is_deleted = 0
WHERE i.is_deleted = 0;
GO

PRINT 'Created view: vw_ActiveInvoices';

-- View for active services
IF OBJECT_ID('vw_ActiveServices', 'V') IS NOT NULL
    DROP VIEW vw_ActiveServices;
GO

CREATE VIEW vw_ActiveServices AS
SELECT 
    id,
    service_name,
    description,
    price,
    duration_minutes,
    is_active,
    created_at
FROM Services
WHERE is_deleted = 0 AND is_active = 1;
GO

PRINT 'Created view: vw_ActiveServices';

-- View for active medicines
IF OBJECT_ID('vw_ActiveMedicines', 'V') IS NOT NULL
    DROP VIEW vw_ActiveMedicines;
GO

CREATE VIEW vw_ActiveMedicines AS
SELECT 
    id,
    medicine_name,
    description,
    unit_price,
    stock_quantity,
    unit,
    expiry_date,
    is_available
FROM Medicines
WHERE is_deleted = 0 AND is_available = 1 AND expiry_date > GETDATE();
GO

PRINT 'Created view: vw_ActiveMedicines';

GO

PRINT '';
PRINT 'All views created successfully!';
GO

-- ============================================
-- PART 5: Create Soft Delete Triggers
-- ============================================

PRINT '';
PRINT 'PART 5: Creating Soft Delete Triggers...';
GO

-- Trigger for Patients soft delete
IF OBJECT_ID('tr_Patients_SoftDelete', 'TR') IS NOT NULL
    DROP TRIGGER tr_Patients_SoftDelete;
GO

CREATE TRIGGER tr_Patients_SoftDelete
ON Patients
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Patients
    SET 
        is_deleted = 1,
        deleted_at = GETDATE(),
        deleted_by = SYSTEM_USER
    WHERE id IN (SELECT id FROM deleted);
    
    PRINT 'Soft delete applied to Patients table';
END
GO

PRINT 'Created trigger: tr_Patients_SoftDelete';

-- Trigger for Appointments soft delete
IF OBJECT_ID('tr_Appointments_SoftDelete', 'TR') IS NOT NULL
    DROP TRIGGER tr_Appointments_SoftDelete;
GO

CREATE TRIGGER tr_Appointments_SoftDelete
ON Appointments
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Appointments
    SET 
        is_deleted = 1,
        deleted_at = GETDATE(),
        deleted_by = SYSTEM_USER
    WHERE id IN (SELECT id FROM deleted);
END
GO

PRINT 'Created trigger: tr_Appointments_SoftDelete';

-- Trigger for Invoices soft delete (cascade to InvoiceDetails)
IF OBJECT_ID('tr_Invoices_SoftDelete', 'TR') IS NOT NULL
    DROP TRIGGER tr_Invoices_SoftDelete;
GO

CREATE TRIGGER tr_Invoices_SoftDelete
ON Invoices
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Soft delete invoice
    UPDATE Invoices
    SET 
        is_deleted = 1,
        deleted_at = GETDATE(),
        deleted_by = SYSTEM_USER
    WHERE id IN (SELECT id FROM deleted);
    
    -- Soft delete related invoice details
    UPDATE InvoiceDetails
    SET 
        is_deleted = 1,
        deleted_at = GETDATE(),
        deleted_by = SYSTEM_USER
    WHERE invoice_id IN (SELECT id FROM deleted);
END
GO

PRINT 'Created trigger: tr_Invoices_SoftDelete';

GO

PRINT '';
PRINT 'All triggers created successfully!';
GO

-- ============================================
-- PART 6: Create Stored Procedures
-- ============================================

PRINT '';
PRINT 'PART 6: Creating Stored Procedures...';
GO

-- Procedure to restore soft-deleted record
IF OBJECT_ID('sp_RestoreRecord', 'P') IS NOT NULL
    DROP PROCEDURE sp_RestoreRecord;
GO

CREATE PROCEDURE sp_RestoreRecord
    @TableName NVARCHAR(100),
    @RecordId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @sql NVARCHAR(MAX);
    
    SET @sql = N'UPDATE ' + @TableName + N'
                 SET is_deleted = 0,
                     deleted_at = NULL,
                     deleted_by = NULL
                 WHERE id = @id';
    
    EXEC sp_executesql @sql, N'@id INT', @id = @RecordId;
    
    PRINT 'Record restored: ' + @TableName + ' [ID: ' + CAST(@RecordId AS NVARCHAR(10)) + ']';
END
GO

PRINT 'Created procedure: sp_RestoreRecord';

-- Procedure to permanently delete soft-deleted records older than X days
IF OBJECT_ID('sp_PurgeOldDeletedRecords', 'P') IS NOT NULL
    DROP PROCEDURE sp_PurgeOldDeletedRecords;
GO

CREATE PROCEDURE sp_PurgeOldDeletedRecords
    @TableName NVARCHAR(100),
    @DaysOld INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @cutoffDate DATETIME;
    
    SET @cutoffDate = DATEADD(DAY, -@DaysOld, GETDATE());
    
    SET @sql = N'DELETE FROM ' + @TableName + N'
                 WHERE is_deleted = 1 
                 AND deleted_at < @cutoff';
    
    EXEC sp_executesql @sql, N'@cutoff DATETIME', @cutoff = @cutoffDate;
    
    PRINT 'Purged old records from ' + @TableName + ' (older than ' + CAST(@DaysOld AS NVARCHAR(10)) + ' days)';
END
GO

PRINT 'Created procedure: sp_PurgeOldDeletedRecords';

-- Procedure to get table statistics
IF OBJECT_ID('sp_GetTableStatistics', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTableStatistics;
GO

CREATE PROCEDURE sp_GetTableStatistics
    @TableName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @sql NVARCHAR(MAX);
    
    SET @sql = N'SELECT 
        ''' + @TableName + ''' AS TableName,
        COUNT(*) AS TotalRecords,
        SUM(CASE WHEN is_deleted = 0 THEN 1 ELSE 0 END) AS ActiveRecords,
        SUM(CASE WHEN is_deleted = 1 THEN 1 ELSE 0 END) AS DeletedRecords,
        CAST(SUM(CASE WHEN is_deleted = 0 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100 AS ActivePercentage
    FROM ' + @TableName;
    
    EXEC sp_executesql @sql;
END
GO

PRINT 'Created procedure: sp_GetTableStatistics';

GO

PRINT '';
PRINT 'All stored procedures created successfully!';
GO

-- ============================================
-- PART 7: Analyze and Report
-- ============================================

PRINT '';
PRINT '===========================================';
PRINT 'DATABASE MIGRATION COMPLETED SUCCESSFULLY!';
PRINT '===========================================';
PRINT '';
PRINT 'Summary of Changes:';
PRINT '-------------------';
PRINT '1. ✅ Fixed denormalization in Appointments table';
PRINT '2. ✅ Added soft delete support (is_deleted, deleted_at, deleted_by)';
PRINT '3. ✅ Created 30+ performance indexes';
PRINT '4. ✅ Created 5 views for active records';
PRINT '5. ✅ Created 3 soft delete triggers';
PRINT '6. ✅ Created 3 utility stored procedures';
PRINT '';
PRINT 'Next Steps:';
PRINT '-----------';
PRINT '1. Update application code to use new schema';
PRINT '2. Test all CRUD operations';
PRINT '3. Update queries to filter is_deleted = 0';
PRINT '4. Run sp_GetTableStatistics to verify data';
PRINT '';
PRINT 'For rollback: Restore from Appointments_Backup_20260128';
PRINT '';
GO

-- Generate statistics report
PRINT 'Database Statistics:';
PRINT '--------------------';

SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('Patients', 'Appointments', 'Invoices', 'Services', 'Staff', 'Medicines')
    AND i.name IS NOT NULL
ORDER BY t.name, i.name;

GO

PRINT '';
PRINT 'Migration script completed at: ' + CONVERT(VARCHAR(20), GETDATE(), 120);
GO

-- =============================================
-- MIGRATION: Complete Database Update
-- Date: 2026-01-28
-- Purpose: Ensure all required columns and constraints exist
-- =============================================

USE DentalClinicDB;
GO

PRINT '=== Starting Complete Database Migration ==='
GO

-- =============================================
-- 1. UPDATE APPOINTMENT TABLE
-- =============================================
PRINT 'Updating Appointment table...'
GO

-- Add queue_number if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'queue_number')
BEGIN
    ALTER TABLE Appointment ADD queue_number INT NULL;
    PRINT 'Added queue_number to Appointment';
END
GO

-- Add check_in_time if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'check_in_time')
BEGIN
    ALTER TABLE Appointment ADD check_in_time DATETIME NULL;
    PRINT 'Added check_in_time to Appointment';
END
GO

-- Add estimated_start_time if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'estimated_start_time')
BEGIN
    ALTER TABLE Appointment ADD estimated_start_time DATETIME NULL;
    PRINT 'Added estimated_start_time to Appointment';
END
GO

-- Add appointment_code if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'appointment_code')
BEGIN
    ALTER TABLE Appointment ADD appointment_code NVARCHAR(50) NULL;
    PRINT 'Added appointment_code to Appointment';
    
    -- Generate codes for existing appointments
    UPDATE Appointment 
    SET appointment_code = 'APT' + RIGHT('00000' + CAST(appointment_id AS VARCHAR), 5)
    WHERE appointment_code IS NULL;
    
    -- Make it NOT NULL after populating
    ALTER TABLE Appointment ALTER COLUMN appointment_code NVARCHAR(50) NOT NULL;
    
    -- Add unique constraint
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Appointment_Code')
    BEGIN
        ALTER TABLE Appointment ADD CONSTRAINT UQ_Appointment_Code UNIQUE (appointment_code);
    END
END
GO

-- =============================================
-- 2. UPDATE INVOICE TABLE
-- =============================================
PRINT 'Updating Invoice table...'
GO

-- Add medical_record_id if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'medical_record_id')
BEGIN
    ALTER TABLE Invoice ADD medical_record_id INT NULL;
    PRINT 'Added medical_record_id to Invoice';
END
GO

-- Add payment_method if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'payment_method')
BEGIN
    ALTER TABLE Invoice ADD payment_method NVARCHAR(50) NULL;
    PRINT 'Added payment_method to Invoice';
END
GO

-- Add payment_note if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'payment_note')
BEGIN
    ALTER TABLE Invoice ADD payment_note NVARCHAR(500) NULL;
    PRINT 'Added payment_note to Invoice';
END
GO

-- Add paid_at if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'paid_at')
BEGIN
    ALTER TABLE Invoice ADD paid_at DATETIME NULL;
    PRINT 'Added paid_at to Invoice';
END
GO

-- Add is_deleted if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'is_deleted')
BEGIN
    ALTER TABLE Invoice ADD is_deleted BIT NOT NULL DEFAULT 0;
    PRINT 'Added is_deleted to Invoice';
END
GO

-- Add foreign key to MedicalRecord if not exists
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoice_MedicalRecord')
BEGIN
    ALTER TABLE Invoice 
    ADD CONSTRAINT FK_Invoice_MedicalRecord 
    FOREIGN KEY (medical_record_id) REFERENCES MedicalRecord(record_id);
    PRINT 'Added FK_Invoice_MedicalRecord';
END
GO

-- =============================================
-- 3. CREATE/UPDATE EMAILLOG TABLE
-- =============================================
PRINT 'Checking EmailLog table...'
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmailLog')
BEGIN
    CREATE TABLE EmailLog (
        log_id INT IDENTITY(1,1) PRIMARY KEY,
        appointment_id INT NULL,
        email_type NVARCHAR(50) NOT NULL, -- 'confirmation', 'reminder', 'cancellation'
        email_address NVARCHAR(255) NOT NULL,
        sent_at DATETIME DEFAULT GETDATE(),
        status NVARCHAR(20) DEFAULT 'sent', -- 'sent', 'failed'
        error_message NVARCHAR(MAX) NULL,
        CONSTRAINT FK_EmailLog_Appointment FOREIGN KEY (appointment_id) 
            REFERENCES Appointment(appointment_id)
    );
    PRINT 'Created EmailLog table';
END
GO

-- =============================================
-- 4. CREATE/UPDATE INVENTORYTRANSACTION TABLE
-- =============================================
PRINT 'Checking InventoryTransaction table...'
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InventoryTransaction')
BEGIN
    CREATE TABLE InventoryTransaction (
        transaction_id INT IDENTITY(1,1) PRIMARY KEY,
        item_id INT NOT NULL,
        transaction_type NVARCHAR(20) NOT NULL, -- 'in', 'out', 'adjust'
        quantity INT NOT NULL,
        reference_type NVARCHAR(50) NULL, -- 'prescription', 'adjustment', 'purchase'
        reference_id INT NULL, -- prescription_id or other reference
        transaction_date DATETIME DEFAULT GETDATE(),
        notes NVARCHAR(500) NULL,
        created_by INT NULL, -- staff_id
        CONSTRAINT FK_InventoryTransaction_Inventory FOREIGN KEY (item_id) 
            REFERENCES Inventory(item_id)
    );
    PRINT 'Created InventoryTransaction table';
END
GO

-- =============================================
-- 5. UPDATE INVENTORY TABLE
-- =============================================
PRINT 'Updating Inventory table...'
GO

-- Ensure Inventory has minimum required columns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Inventory' AND COLUMN_NAME = 'low_stock_threshold')
BEGIN
    ALTER TABLE Inventory ADD low_stock_threshold INT DEFAULT 10;
    PRINT 'Added low_stock_threshold to Inventory';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Inventory' AND COLUMN_NAME = 'last_updated')
BEGIN
    ALTER TABLE Inventory ADD last_updated DATETIME DEFAULT GETDATE();
    PRINT 'Added last_updated to Inventory';
END
GO

-- =============================================
-- 6. CREATE/UPDATE SERVICEUSAGE TABLE
-- =============================================
PRINT 'Checking ServiceUsage table...'
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceUsage')
BEGIN
    CREATE TABLE ServiceUsage (
        usage_id INT IDENTITY(1,1) PRIMARY KEY,
        invoice_id INT NOT NULL,
        service_id INT NOT NULL,
        quantity INT DEFAULT 1,
        unit_price DECIMAL(10,2) NOT NULL,
        total_price AS (quantity * unit_price) PERSISTED,
        CONSTRAINT FK_ServiceUsage_Invoice FOREIGN KEY (invoice_id) 
            REFERENCES Invoice(invoice_id),
        CONSTRAINT FK_ServiceUsage_Service FOREIGN KEY (service_id) 
            REFERENCES Service(service_id)
    );
    PRINT 'Created ServiceUsage table';
END
GO

-- =============================================
-- 7. CREATE/UPDATE INVOICEPRESCRIPTION TABLE
-- =============================================
PRINT 'Checking InvoicePrescription table...'
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InvoicePrescription')
BEGIN
    CREATE TABLE InvoicePrescription (
        invoice_prescription_id INT IDENTITY(1,1) PRIMARY KEY,
        invoice_id INT NOT NULL,
        prescription_id INT NOT NULL,
        CONSTRAINT FK_InvoicePrescription_Invoice FOREIGN KEY (invoice_id) 
            REFERENCES Invoice(invoice_id),
        CONSTRAINT FK_InvoicePrescription_Prescription FOREIGN KEY (prescription_id) 
            REFERENCES Prescription(prescription_id),
        CONSTRAINT UQ_Invoice_Prescription UNIQUE (invoice_id, prescription_id)
    );
    PRINT 'Created InvoicePrescription table';
END
GO

-- =============================================
-- 8. ADD AUDIT COLUMNS TO KEY TABLES
-- =============================================
PRINT 'Adding audit columns...'
GO

-- Appointment
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'created_at')
BEGIN
    ALTER TABLE Appointment ADD created_at DATETIME DEFAULT GETDATE();
    UPDATE Appointment SET created_at = GETDATE() WHERE created_at IS NULL;
    ALTER TABLE Appointment ALTER COLUMN created_at DATETIME NOT NULL;
    PRINT 'Added created_at to Appointment';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'updated_at')
BEGIN
    ALTER TABLE Appointment ADD updated_at DATETIME DEFAULT GETDATE();
    UPDATE Appointment SET updated_at = GETDATE() WHERE updated_at IS NULL;
    ALTER TABLE Appointment ALTER COLUMN updated_at DATETIME NOT NULL;
    PRINT 'Added updated_at to Appointment';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'is_deleted')
BEGIN
    ALTER TABLE Appointment ADD is_deleted BIT DEFAULT 0;
    UPDATE Appointment SET is_deleted = 0 WHERE is_deleted IS NULL;
    ALTER TABLE Appointment ALTER COLUMN is_deleted BIT NOT NULL;
    PRINT 'Added is_deleted to Appointment';
END
GO

-- Invoice
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoice' AND COLUMN_NAME = 'created_at')
BEGIN
    ALTER TABLE Invoice ADD created_at DATETIME DEFAULT GETDATE();
    UPDATE Invoice SET created_at = GETDATE() WHERE created_at IS NULL;
    ALTER TABLE Invoice ALTER COLUMN created_at DATETIME NOT NULL;
    PRINT 'Added created_at to Invoice';
END
GO

-- =============================================
-- 9. CREATE INDEXES FOR PERFORMANCE
-- =============================================
PRINT 'Creating indexes...'
GO

-- Appointment indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Date')
    CREATE INDEX IX_Appointment_Date ON Appointment(appointment_date);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Status')
    CREATE INDEX IX_Appointment_Status ON Appointment(status);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Doctor')
    CREATE INDEX IX_Appointment_Doctor ON Appointment(assigned_doctor_id);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Queue')
    CREATE INDEX IX_Appointment_Queue ON Appointment(queue_number);
GO

-- Invoice indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoice_Patient')
    CREATE INDEX IX_Invoice_Patient ON Invoice(patient_id);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoice_Status')
    CREATE INDEX IX_Invoice_Status ON Invoice(status);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoice_MedicalRecord')
    CREATE INDEX IX_Invoice_MedicalRecord ON Invoice(medical_record_id);
GO

-- EmailLog indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_Appointment')
    CREATE INDEX IX_EmailLog_Appointment ON EmailLog(appointment_id);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_SentAt')
    CREATE INDEX IX_EmailLog_SentAt ON EmailLog(sent_at);
GO

-- =============================================
-- 10. VERIFY FOREIGN KEYS
-- =============================================
PRINT 'Verifying foreign keys...'
GO

-- Appointment foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointment_Patient')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'patient_id')
BEGIN
    ALTER TABLE Appointment 
    ADD CONSTRAINT FK_Appointment_Patient 
    FOREIGN KEY (patient_id) REFERENCES Patient(patient_id);
    PRINT 'Added FK_Appointment_Patient';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointment_Service')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'service_id')
BEGIN
    ALTER TABLE Appointment 
    ADD CONSTRAINT FK_Appointment_Service 
    FOREIGN KEY (service_id) REFERENCES Service(service_id);
    PRINT 'Added FK_Appointment_Service';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointment_Doctor')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'assigned_doctor_id')
BEGIN
    ALTER TABLE Appointment 
    ADD CONSTRAINT FK_Appointment_Doctor 
    FOREIGN KEY (assigned_doctor_id) REFERENCES Staff(staff_id);
    PRINT 'Added FK_Appointment_Doctor';
END
GO

-- =============================================
-- COMPLETION
-- =============================================
PRINT ''
PRINT '=== Database Migration Completed Successfully ==='
PRINT 'All tables, columns, constraints, and indexes are now up to date.'
PRINT ''
GO

-- Show summary
SELECT 
    'Appointment' AS TableName, 
    COUNT(*) AS RecordCount 
FROM Appointment
UNION ALL
SELECT 'Invoice', COUNT(*) FROM Invoice
UNION ALL
SELECT 'EmailLog', COUNT(*) FROM EmailLog
UNION ALL
SELECT 'InventoryTransaction', COUNT(*) FROM InventoryTransaction
UNION ALL
SELECT 'ServiceUsage', COUNT(*) FROM ServiceUsage
UNION ALL
SELECT 'InvoicePrescription', COUNT(*) FROM InvoicePrescription
UNION ALL
SELECT 'Inventory', COUNT(*) FROM Inventory
ORDER BY TableName;
GO

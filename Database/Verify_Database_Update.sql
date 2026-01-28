-- =============================================
-- VERIFICATION SCRIPT: Database Update Status
-- Purpose: Check if all required tables, columns, and constraints exist
-- =============================================

USE DentalClinicDB;
GO

PRINT '=============================================='
PRINT 'DATABASE UPDATE VERIFICATION REPORT'
PRINT 'Date: ' + CAST(GETDATE() AS VARCHAR)
PRINT '=============================================='
PRINT ''

-- 1. Check all required tables
PRINT '1. CHECKING REQUIRED TABLES:'
PRINT '----------------------------'

DECLARE @tables TABLE (TableName NVARCHAR(100), Status NVARCHAR(10))

INSERT INTO @tables VALUES 
    ('Appointment', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Appointment') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Patient', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Patient') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Staff', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Service', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Service') THEN 'EXISTS' ELSE 'MISSING' END),
    ('MedicalRecord', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MedicalRecord') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Invoice', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Invoice') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Prescription', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Prescription') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Medicine', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Medicine') THEN 'EXISTS' ELSE 'MISSING' END),
    ('Inventory', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Inventory') THEN 'EXISTS' ELSE 'MISSING' END),
    ('EmailLog', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmailLog') THEN 'EXISTS' ELSE 'MISSING' END),
    ('InventoryTransaction', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InventoryTransaction') THEN 'EXISTS' ELSE 'MISSING' END),
    ('ServiceUsage', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceUsage') THEN 'EXISTS' ELSE 'MISSING' END),
    ('InvoicePrescription', CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InvoicePrescription') THEN 'EXISTS' ELSE 'MISSING' END)

SELECT 
    TableName,
    Status,
    CASE WHEN Status = 'EXISTS' THEN '✓' ELSE '✗' END AS Check
FROM @tables
ORDER BY TableName
GO

PRINT ''
PRINT '2. CHECKING APPOINTMENT TABLE COLUMNS:'
PRINT '---------------------------------------'

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN COLUMN_NAME IN ('appointment_code', 'queue_number', 'check_in_time', 'estimated_start_time', 'is_deleted', 'created_at', 'updated_at') 
        THEN '✓ NEW' 
        ELSE '  OK' 
    END AS Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Appointment'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '3. CHECKING INVOICE TABLE COLUMNS:'
PRINT '-----------------------------------'

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN COLUMN_NAME IN ('medical_record_id', 'payment_method', 'payment_note', 'paid_at', 'is_deleted', 'created_at') 
        THEN '✓ NEW' 
        ELSE '  OK' 
    END AS Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Invoice'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '4. CHECKING INVENTORY TABLE COLUMNS:'
PRINT '-------------------------------------'

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN COLUMN_NAME IN ('low_stock_threshold', 'last_updated') 
        THEN '✓ NEW' 
        ELSE '  OK' 
    END AS Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Inventory'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '5. CHECKING FOREIGN KEY CONSTRAINTS:'
PRINT '-------------------------------------'

SELECT 
    OBJECT_NAME(f.parent_object_id) AS TableName,
    f.name AS ForeignKeyName,
    OBJECT_NAME(f.referenced_object_id) AS ReferencedTable,
    CASE 
        WHEN f.name LIKE '%MedicalRecord%' OR f.name LIKE '%EmailLog%' OR f.name LIKE '%InventoryTransaction%'
        THEN '✓ NEW'
        ELSE '  OK'
    END AS Status
FROM sys.foreign_keys AS f
WHERE OBJECT_NAME(f.parent_object_id) IN ('Appointment', 'Invoice', 'EmailLog', 'InventoryTransaction', 'ServiceUsage', 'InvoicePrescription')
ORDER BY TableName, ForeignKeyName
GO

PRINT ''
PRINT '6. CHECKING INDEXES:'
PRINT '--------------------'

SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE 
        WHEN i.name LIKE 'IX_%' 
        THEN '✓ NEW'
        ELSE '  OK'
    END AS Status
FROM sys.indexes AS i
WHERE OBJECT_NAME(i.object_id) IN ('Appointment', 'Invoice', 'EmailLog')
    AND i.name IS NOT NULL
ORDER BY TableName, IndexName
GO

PRINT ''
PRINT '7. RECORD COUNTS:'
PRINT '-----------------'

SELECT 'Appointment' AS TableName, COUNT(*) AS RecordCount FROM Appointment
UNION ALL
SELECT 'Patient', COUNT(*) FROM Patient
UNION ALL
SELECT 'Staff', COUNT(*) FROM Staff
UNION ALL
SELECT 'Service', COUNT(*) FROM Service
UNION ALL
SELECT 'MedicalRecord', COUNT(*) FROM MedicalRecord
UNION ALL
SELECT 'Invoice', COUNT(*) FROM Invoice
UNION ALL
SELECT 'Prescription', COUNT(*) FROM Prescription
UNION ALL
SELECT 'Medicine', COUNT(*) FROM Medicine
UNION ALL
SELECT 'Inventory', COUNT(*) FROM Inventory
UNION ALL
SELECT 'EmailLog', COUNT(*) FROM EmailLog
UNION ALL
SELECT 'InventoryTransaction', COUNT(*) FROM InventoryTransaction
UNION ALL
SELECT 'ServiceUsage', COUNT(*) FROM ServiceUsage
UNION ALL
SELECT 'InvoicePrescription', COUNT(*) FROM InvoicePrescription
ORDER BY TableName
GO

PRINT ''
PRINT '=============================================='
PRINT 'VERIFICATION COMPLETE'
PRINT '=============================================='
GO

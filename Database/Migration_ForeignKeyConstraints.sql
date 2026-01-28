-- ============================================
-- Foreign Key Constraints Migration
-- Issue #9: Prevent Deletion of In-Use Records
-- Version: 1.0
-- Date: 2026-01-28
-- ============================================

USE DentalManagement;
GO

PRINT '===========================================';
PRINT 'Adding Foreign Key Constraints';
PRINT '===========================================';
PRINT '';

-- ============================================
-- PART 1: Add Foreign Key Constraints
-- ============================================

PRINT 'PART 1: Adding Foreign Key Constraints...';
GO

-- Appointments Table Foreign Keys
PRINT 'Adding foreign keys to Appointments table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Patients')
BEGIN
    ALTER TABLE Appointments
    ADD CONSTRAINT FK_Appointments_Patients 
    FOREIGN KEY (patient_id) REFERENCES Patients(id);
    PRINT '  ✅ Added: FK_Appointments_Patients';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Appointments_Patients';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Staff')
BEGIN
    ALTER TABLE Appointments
    ADD CONSTRAINT FK_Appointments_Staff 
    FOREIGN KEY (doctor_id) REFERENCES Staff(id);
    PRINT '  ✅ Added: FK_Appointments_Staff';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Appointments_Staff';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Services')
BEGIN
    ALTER TABLE Appointments
    ADD CONSTRAINT FK_Appointments_Services 
    FOREIGN KEY (service_id) REFERENCES Services(id);
    PRINT '  ✅ Added: FK_Appointments_Services';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Appointments_Services';

GO

-- Invoices Table Foreign Keys
PRINT 'Adding foreign keys to Invoices table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_Patients')
BEGIN
    ALTER TABLE Invoices
    ADD CONSTRAINT FK_Invoices_Patients 
    FOREIGN KEY (patient_id) REFERENCES Patients(id);
    PRINT '  ✅ Added: FK_Invoices_Patients';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Invoices_Patients';

GO

-- InvoiceDetails Table Foreign Keys
PRINT 'Adding foreign keys to InvoiceDetails table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InvoiceDetails_Invoices')
BEGIN
    ALTER TABLE InvoiceDetails
    ADD CONSTRAINT FK_InvoiceDetails_Invoices 
    FOREIGN KEY (invoice_id) REFERENCES Invoices(id);
    PRINT '  ✅ Added: FK_InvoiceDetails_Invoices';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_InvoiceDetails_Invoices';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InvoiceDetails_Services')
BEGIN
    ALTER TABLE InvoiceDetails
    ADD CONSTRAINT FK_InvoiceDetails_Services 
    FOREIGN KEY (service_id) REFERENCES Services(id);
    PRINT '  ✅ Added: FK_InvoiceDetails_Services';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_InvoiceDetails_Services';

GO

-- MedicalRecords Table Foreign Keys
PRINT 'Adding foreign keys to MedicalRecords table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients')
BEGIN
    ALTER TABLE MedicalRecords
    ADD CONSTRAINT FK_MedicalRecords_Patients 
    FOREIGN KEY (patient_id) REFERENCES Patients(id);
    PRINT '  ✅ Added: FK_MedicalRecords_Patients';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_MedicalRecords_Patients';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Staff')
BEGIN
    ALTER TABLE MedicalRecords
    ADD CONSTRAINT FK_MedicalRecords_Staff 
    FOREIGN KEY (doctor_id) REFERENCES Staff(id);
    PRINT '  ✅ Added: FK_MedicalRecords_Staff';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_MedicalRecords_Staff';

GO

-- Prescriptions Table Foreign Keys
PRINT 'Adding foreign keys to Prescriptions table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_MedicalRecords')
BEGIN
    ALTER TABLE Prescriptions
    ADD CONSTRAINT FK_Prescriptions_MedicalRecords 
    FOREIGN KEY (record_id) REFERENCES MedicalRecords(id);
    PRINT '  ✅ Added: FK_Prescriptions_MedicalRecords';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Prescriptions_MedicalRecords';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_Medicines')
BEGIN
    ALTER TABLE Prescriptions
    ADD CONSTRAINT FK_Prescriptions_Medicines 
    FOREIGN KEY (medicine_id) REFERENCES Medicines(id);
    PRINT '  ✅ Added: FK_Prescriptions_Medicines';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Prescriptions_Medicines';

GO

-- Inventory Table Foreign Keys
PRINT 'Adding foreign keys to Inventory table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Inventory_Medicines')
BEGIN
    ALTER TABLE Inventory
    ADD CONSTRAINT FK_Inventory_Medicines 
    FOREIGN KEY (medicine_id) REFERENCES Medicines(id);
    PRINT '  ✅ Added: FK_Inventory_Medicines';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Inventory_Medicines';

GO

-- Users Table Foreign Keys
PRINT 'Adding foreign keys to Users table...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Staff')
BEGIN
    ALTER TABLE Users
    ADD CONSTRAINT FK_Users_Staff 
    FOREIGN KEY (staff_id) REFERENCES Staff(id);
    PRINT '  ✅ Added: FK_Users_Staff';
END
ELSE
    PRINT '  ⚠️ Already exists: FK_Users_Staff';

GO

PRINT '';
PRINT 'Foreign key constraints added successfully!';
GO

-- ============================================
-- PART 2: Create Check Constraint Functions
-- ============================================

PRINT '';
PRINT 'PART 2: Creating Check Functions...';
GO

-- Function to check if service is in use
IF OBJECT_ID('fn_IsServiceInUse', 'FN') IS NOT NULL
    DROP FUNCTION fn_IsServiceInUse;
GO

CREATE FUNCTION fn_IsServiceInUse(@ServiceId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsInUse BIT = 0;
    
    IF EXISTS (
        SELECT 1 FROM Appointments 
        WHERE service_id = @ServiceId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    IF EXISTS (
        SELECT 1 FROM InvoiceDetails 
        WHERE service_id = @ServiceId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    RETURN @IsInUse;
END
GO

PRINT '  ✅ Created: fn_IsServiceInUse';

-- Function to check if medicine is in use
IF OBJECT_ID('fn_IsMedicineInUse', 'FN') IS NOT NULL
    DROP FUNCTION fn_IsMedicineInUse;
GO

CREATE FUNCTION fn_IsMedicineInUse(@MedicineId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsInUse BIT = 0;
    
    IF EXISTS (
        SELECT 1 FROM Prescriptions 
        WHERE medicine_id = @MedicineId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    IF EXISTS (
        SELECT 1 FROM Inventory 
        WHERE medicine_id = @MedicineId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    RETURN @IsInUse;
END
GO

PRINT '  ✅ Created: fn_IsMedicineInUse';

-- Function to check if patient is in use
IF OBJECT_ID('fn_IsPatientInUse', 'FN') IS NOT NULL
    DROP FUNCTION fn_IsPatientInUse;
GO

CREATE FUNCTION fn_IsPatientInUse(@PatientId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsInUse BIT = 0;
    
    IF EXISTS (
        SELECT 1 FROM Appointments 
        WHERE patient_id = @PatientId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    IF EXISTS (
        SELECT 1 FROM Invoices 
        WHERE patient_id = @PatientId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    IF EXISTS (
        SELECT 1 FROM MedicalRecords 
        WHERE patient_id = @PatientId AND is_deleted = 0
    )
    BEGIN
        SET @IsInUse = 1;
    END
    
    RETURN @IsInUse;
END
GO

PRINT '  ✅ Created: fn_IsPatientInUse';

GO

-- ============================================
-- PART 3: Create Usage Views
-- ============================================

PRINT '';
PRINT 'PART 3: Creating Usage Views...';
GO

-- View for service usage
IF OBJECT_ID('vw_ServiceUsage', 'V') IS NOT NULL
    DROP VIEW vw_ServiceUsage;
GO

CREATE VIEW vw_ServiceUsage AS
SELECT 
    s.id AS service_id,
    s.service_name,
    s.is_active,
    (SELECT COUNT(*) FROM Appointments 
     WHERE service_id = s.id AND is_deleted = 0) AS appointment_count,
    (SELECT COUNT(*) FROM InvoiceDetails 
     WHERE service_id = s.id AND is_deleted = 0) AS invoice_detail_count,
    (SELECT COUNT(*) FROM Appointments 
     WHERE service_id = s.id AND is_deleted = 0 
     AND appointment_date >= GETDATE()) AS upcoming_appointment_count,
    CASE 
        WHEN EXISTS (SELECT 1 FROM Appointments WHERE service_id = s.id AND is_deleted = 0)
            OR EXISTS (SELECT 1 FROM InvoiceDetails WHERE service_id = s.id AND is_deleted = 0)
        THEN 1 
        ELSE 0 
    END AS is_in_use
FROM Services s
WHERE s.is_deleted = 0;
GO

PRINT '  ✅ Created: vw_ServiceUsage';

-- View for medicine usage
IF OBJECT_ID('vw_MedicineUsage', 'V') IS NOT NULL
    DROP VIEW vw_MedicineUsage;
GO

CREATE VIEW vw_MedicineUsage AS
SELECT 
    m.id AS medicine_id,
    m.medicine_name,
    m.is_available,
    (SELECT COUNT(*) FROM Prescriptions 
     WHERE medicine_id = m.id AND is_deleted = 0) AS prescription_count,
    (SELECT COUNT(*) FROM Inventory 
     WHERE medicine_id = m.id AND is_deleted = 0) AS inventory_record_count,
    (SELECT ISNULL(SUM(quantity), 0) FROM Inventory 
     WHERE medicine_id = m.id AND is_deleted = 0) AS total_stock_quantity,
    (SELECT COUNT(DISTINCT mr.patient_id) 
     FROM Prescriptions p
     INNER JOIN MedicalRecords mr ON p.record_id = mr.id
     WHERE p.medicine_id = m.id AND p.is_deleted = 0) AS patient_count,
    CASE 
        WHEN EXISTS (SELECT 1 FROM Prescriptions WHERE medicine_id = m.id AND is_deleted = 0)
            OR EXISTS (SELECT 1 FROM Inventory WHERE medicine_id = m.id AND is_deleted = 0)
        THEN 1 
        ELSE 0 
    END AS is_in_use
FROM Medicines m
WHERE m.is_deleted = 0;
GO

PRINT '  ✅ Created: vw_MedicineUsage';

-- View for patient usage
IF OBJECT_ID('vw_PatientUsage', 'V') IS NOT NULL
    DROP VIEW vw_PatientUsage;
GO

CREATE VIEW vw_PatientUsage AS
SELECT 
    p.id AS patient_id,
    p.full_name,
    p.email,
    (SELECT COUNT(*) FROM Appointments 
     WHERE patient_id = p.id AND is_deleted = 0) AS appointment_count,
    (SELECT COUNT(*) FROM Invoices 
     WHERE patient_id = p.id AND is_deleted = 0) AS invoice_count,
    (SELECT COUNT(*) FROM MedicalRecords 
     WHERE patient_id = p.id AND is_deleted = 0) AS medical_record_count,
    (SELECT COUNT(*) FROM Appointments 
     WHERE patient_id = p.id AND is_deleted = 0 
     AND appointment_date >= GETDATE()) AS upcoming_appointment_count,
    CASE 
        WHEN EXISTS (SELECT 1 FROM Appointments WHERE patient_id = p.id AND is_deleted = 0)
            OR EXISTS (SELECT 1 FROM Invoices WHERE patient_id = p.id AND is_deleted = 0)
            OR EXISTS (SELECT 1 FROM MedicalRecords WHERE patient_id = p.id AND is_deleted = 0)
        THEN 1 
        ELSE 0 
    END AS is_in_use
FROM Patients p
WHERE p.is_deleted = 0;
GO

PRINT '  ✅ Created: vw_PatientUsage';

GO

-- ============================================
-- PART 4: Create Validation Stored Procedures
-- ============================================

PRINT '';
PRINT 'PART 4: Creating Validation Procedures...';
GO

-- Procedure to validate service deletion
IF OBJECT_ID('sp_ValidateServiceDeletion', 'P') IS NOT NULL
    DROP PROCEDURE sp_ValidateServiceDeletion;
GO

CREATE PROCEDURE sp_ValidateServiceDeletion
    @ServiceId INT,
    @CanDelete BIT OUTPUT,
    @Message NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AppointmentCount INT;
    DECLARE @InvoiceDetailCount INT;
    DECLARE @UpcomingCount INT;
    
    SELECT 
        @AppointmentCount = appointment_count,
        @InvoiceDetailCount = invoice_detail_count,
        @UpcomingCount = upcoming_appointment_count
    FROM vw_ServiceUsage
    WHERE service_id = @ServiceId;
    
    IF @AppointmentCount > 0 OR @InvoiceDetailCount > 0
    BEGIN
        SET @CanDelete = 0;
        SET @Message = N'Không thể xóa dịch vụ này!' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
                       N'Đang được sử dụng bởi:' + CHAR(13) + CHAR(10) +
                       N'- ' + CAST(@AppointmentCount AS NVARCHAR(10)) + N' lịch hẹn' + CHAR(13) + CHAR(10) +
                       N'- ' + CAST(@InvoiceDetailCount AS NVARCHAR(10)) + N' hóa đơn' + CHAR(13) + CHAR(10);
        
        IF @UpcomingCount > 0
        BEGIN
            SET @Message = @Message + CHAR(13) + CHAR(10) +
                          N'⚠️ Có ' + CAST(@UpcomingCount AS NVARCHAR(10)) + N' lịch hẹn sắp tới!' + CHAR(13) + CHAR(10);
        END
        
        SET @Message = @Message + CHAR(13) + CHAR(10) +
                      N'Vui lòng chọn "Vô hiệu hóa" thay vì xóa.';
    END
    ELSE
    BEGIN
        SET @CanDelete = 1;
        SET @Message = N'Có thể xóa dịch vụ này.';
    END
END
GO

PRINT '  ✅ Created: sp_ValidateServiceDeletion';

-- Procedure to validate medicine deletion
IF OBJECT_ID('sp_ValidateMedicineDeletion', 'P') IS NOT NULL
    DROP PROCEDURE sp_ValidateMedicineDeletion;
GO

CREATE PROCEDURE sp_ValidateMedicineDeletion
    @MedicineId INT,
    @CanDelete BIT OUTPUT,
    @Message NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PrescriptionCount INT;
    DECLARE @InventoryRecordCount INT;
    DECLARE @TotalStock INT;
    DECLARE @PatientCount INT;
    
    SELECT 
        @PrescriptionCount = prescription_count,
        @InventoryRecordCount = inventory_record_count,
        @TotalStock = total_stock_quantity,
        @PatientCount = patient_count
    FROM vw_MedicineUsage
    WHERE medicine_id = @MedicineId;
    
    IF @PrescriptionCount > 0 OR @InventoryRecordCount > 0
    BEGIN
        SET @CanDelete = 0;
        SET @Message = N'Không thể xóa thuốc này!' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
                       N'Đang được sử dụng bởi:' + CHAR(13) + CHAR(10) +
                       N'- ' + CAST(@PrescriptionCount AS NVARCHAR(10)) + N' đơn thuốc' + CHAR(13) + CHAR(10) +
                       N'- ' + CAST(@PatientCount AS NVARCHAR(10)) + N' bệnh nhân' + CHAR(13) + CHAR(10) +
                       N'- ' + CAST(@InventoryRecordCount AS NVARCHAR(10)) + N' bản ghi kho' + CHAR(13) + CHAR(10) +
                       N'- Tồn kho: ' + CAST(@TotalStock AS NVARCHAR(10)) + N' đơn vị' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
                       N'Vui lòng chọn "Đánh dấu không khả dụng" thay vì xóa.';
    END
    ELSE
    BEGIN
        SET @CanDelete = 1;
        SET @Message = N'Có thể xóa thuốc này.';
    END
END
GO

PRINT '  ✅ Created: sp_ValidateMedicineDeletion';

GO

-- ============================================
-- PART 5: Summary Report
-- ============================================

PRINT '';
PRINT '===========================================';
PRINT 'FOREIGN KEY CONSTRAINTS MIGRATION COMPLETE!';
PRINT '===========================================';
PRINT '';
PRINT 'Summary of Changes:';
PRINT '-------------------';
PRINT '1. ✅ Added foreign key constraints (9 constraints)';
PRINT '2. ✅ Created check functions (3 functions)';
PRINT '3. ✅ Created usage views (3 views)';
PRINT '4. ✅ Created validation procedures (2 procedures)';
PRINT '';
PRINT 'Foreign Keys Added:';
PRINT '------------------';
PRINT '  Appointments: patient_id, doctor_id, service_id';
PRINT '  Invoices: patient_id';
PRINT '  InvoiceDetails: invoice_id, service_id';
PRINT '  MedicalRecords: patient_id, doctor_id';
PRINT '  Prescriptions: record_id, medicine_id';
PRINT '  Inventory: medicine_id';
PRINT '  Users: staff_id';
PRINT '';
PRINT 'Usage Views Created:';
PRINT '-------------------';
PRINT '  vw_ServiceUsage - Service usage statistics';
PRINT '  vw_MedicineUsage - Medicine usage statistics';
PRINT '  vw_PatientUsage - Patient usage statistics';
PRINT '';
PRINT 'Test the views:';
PRINT '--------------';
PRINT '  SELECT * FROM vw_ServiceUsage WHERE is_in_use = 1;';
PRINT '  SELECT * FROM vw_MedicineUsage WHERE is_in_use = 1;';
PRINT '  SELECT * FROM vw_PatientUsage WHERE is_in_use = 1;';
PRINT '';
GO

-- Generate FK report
PRINT 'Foreign Key Constraints:';
PRINT '------------------------';

SELECT 
    OBJECT_NAME(f.parent_object_id) AS TableName,
    f.name AS ConstraintName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
    OBJECT_NAME(f.referenced_object_id) AS ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys f
INNER JOIN sys.foreign_key_columns fc ON f.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(f.parent_object_id) IN (
    'Appointments', 'Invoices', 'InvoiceDetails', 
    'MedicalRecords', 'Prescriptions', 'Inventory', 'Users'
)
ORDER BY TableName, ConstraintName;

GO

PRINT '';
PRINT 'Migration completed at: ' + CONVERT(VARCHAR(20), GETDATE(), 120);
GO

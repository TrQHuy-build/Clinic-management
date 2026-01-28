-- ========================================
-- ISSUE #10, #11, #12: CRITICAL FIXES
-- - Normalize Appointment table
-- - Add EmailLog table
-- - Add medical_record_id to Invoice
-- - Add slot conflict detection
-- ========================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'STARTING CRITICAL FIXES MIGRATION';
PRINT '========================================';
GO

-- ========================================
-- 1. CREATE EmailLog TABLE
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailLog')
BEGIN
    CREATE TABLE EmailLog (
        log_id INT IDENTITY(1,1) PRIMARY KEY,
        appointment_id INT NULL,
        email_type NVARCHAR(50) NOT NULL, -- confirmation, reminder, cancellation, invoice
        email_address NVARCHAR(100) NOT NULL,
        sent_at DATETIME DEFAULT GETDATE(),
        status NVARCHAR(20) DEFAULT 'sent', -- sent, failed
        error_message NVARCHAR(MAX) NULL,
        FOREIGN KEY (appointment_id) REFERENCES Appointment(appointment_id)
    );
    
    CREATE INDEX IX_EmailLog_AppointmentId ON EmailLog(appointment_id);
    CREATE INDEX IX_EmailLog_SentAt ON EmailLog(sent_at);
    CREATE INDEX IX_EmailLog_EmailType ON EmailLog(email_type);
    
    PRINT '✅ Created EmailLog table with indexes';
END
ELSE
BEGIN
    PRINT '⚠️  EmailLog table already exists';
END
GO

-- ========================================
-- 2. ADD patient_id TO Appointment (NORMALIZE)
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Appointment') 
               AND name = 'patient_id')
BEGIN
    ALTER TABLE Appointment ADD patient_id INT NULL;
    PRINT '✅ Added patient_id column to Appointment';
    
    -- Migrate data: Tìm patient_id từ email/phone
    UPDATE a
    SET a.patient_id = p.patient_id
    FROM Appointment a
    INNER JOIN Patient p ON p.patient_id = (
        SELECT TOP 1 p2.patient_id
        FROM Patient p2
        INNER JOIN UserAccount u ON p2.user_id = u.user_id
        WHERE u.email = a.email 
        OR u.phone = a.phone
        ORDER BY p2.patient_id DESC
    )
    WHERE a.patient_id IS NULL;
    
    PRINT '✅ Migrated existing appointment data to use patient_id';
    
    -- Add foreign key (không bắt buộc NOT NULL để tương thích với guest appointments)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                   WHERE name = 'FK_Appointments_Patients')
    BEGIN
        ALTER TABLE Appointment
        ADD CONSTRAINT FK_Appointments_Patients
        FOREIGN KEY (patient_id) REFERENCES Patient(patient_id);
        
        PRINT '✅ Added FK_Appointments_Patients constraint';
    END
END
ELSE
BEGIN
    PRINT '⚠️  patient_id already exists in Appointment';
END
GO

-- ========================================
-- 3. ADD assigned_doctor_id TO Appointment
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Appointment') 
               AND name = 'assigned_doctor_id')
BEGIN
    ALTER TABLE Appointment ADD assigned_doctor_id INT NULL;
    PRINT '✅ Added assigned_doctor_id column to Appointment';
    
    -- Add foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                   WHERE name = 'FK_Appointments_AssignedDoctor')
    BEGIN
        ALTER TABLE Appointment
        ADD CONSTRAINT FK_Appointments_AssignedDoctor
        FOREIGN KEY (assigned_doctor_id) REFERENCES Staff(staff_id);
        
        PRINT '✅ Added FK_Appointments_AssignedDoctor constraint';
    END
    
    CREATE INDEX IX_Appointment_AssignedDoctor ON Appointment(assigned_doctor_id);
    PRINT '✅ Created index on assigned_doctor_id';
END
ELSE
BEGIN
    PRINT '⚠️  assigned_doctor_id already exists in Appointment';
END
GO

-- ========================================
-- 4. ADD appointment_code TO Appointment
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Appointment') 
               AND name = 'appointment_code')
BEGIN
    ALTER TABLE Appointment ADD appointment_code NVARCHAR(20) NULL;
    PRINT '✅ Added appointment_code column to Appointment';
    
    -- Generate codes for existing appointments
    UPDATE Appointment
    SET appointment_code = 'APT' + RIGHT('000000' + CAST(appointment_id AS VARCHAR), 6)
    WHERE appointment_code IS NULL;
    
    PRINT '✅ Generated appointment codes for existing records';
    
    -- Make it unique
    ALTER TABLE Appointment ALTER COLUMN appointment_code NVARCHAR(20) NOT NULL;
    ALTER TABLE Appointment ADD CONSTRAINT UQ_Appointment_Code UNIQUE (appointment_code);
    
    PRINT '✅ Made appointment_code unique';
END
ELSE
BEGIN
    PRINT '⚠️  appointment_code already exists in Appointment';
END
GO

-- ========================================
-- 5. ADD medical_record_id TO Invoice
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Invoice') 
               AND name = 'medical_record_id')
BEGIN
    ALTER TABLE Invoice ADD medical_record_id INT NULL;
    PRINT '✅ Added medical_record_id column to Invoice';
    
    -- Add foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                   WHERE name = 'FK_Invoices_MedicalRecords')
    BEGIN
        ALTER TABLE Invoice
        ADD CONSTRAINT FK_Invoices_MedicalRecords
        FOREIGN KEY (medical_record_id) REFERENCES MedicalRecord(record_id);
        
        PRINT '✅ Added FK_Invoices_MedicalRecords constraint';
    END
    
    CREATE INDEX IX_Invoice_MedicalRecordId ON Invoice(medical_record_id);
    PRINT '✅ Created index on medical_record_id';
END
ELSE
BEGIN
    PRINT '⚠️  medical_record_id already exists in Invoice';
END
GO

-- ========================================
-- 6. ADD payment_method TO Invoice
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Invoice') 
               AND name = 'payment_method')
BEGIN
    ALTER TABLE Invoice ADD payment_method NVARCHAR(20) NULL 
        CHECK (payment_method IN ('cash', 'card', 'transfer', 'insurance', NULL));
    
    ALTER TABLE Invoice ADD payment_note NVARCHAR(255) NULL;
    ALTER TABLE Invoice ADD paid_at DATETIME NULL;
    
    PRINT '✅ Added payment tracking columns to Invoice';
END
ELSE
BEGIN
    PRINT '⚠️  payment_method already exists in Invoice';
END
GO

-- ========================================
-- 7. ADD appointment_id TO MedicalRecord
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('MedicalRecord') 
               AND name = 'appointment_id')
BEGIN
    ALTER TABLE MedicalRecord ADD appointment_id INT NULL;
    PRINT '✅ Added appointment_id column to MedicalRecord';
    
    -- Add foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                   WHERE name = 'FK_MedicalRecords_Appointments')
    BEGIN
        ALTER TABLE MedicalRecord
        ADD CONSTRAINT FK_MedicalRecords_Appointments
        FOREIGN KEY (appointment_id) REFERENCES Appointment(appointment_id);
        
        PRINT '✅ Added FK_MedicalRecords_Appointments constraint';
    END
    
    CREATE INDEX IX_MedicalRecord_AppointmentId ON MedicalRecord(appointment_id);
    PRINT '✅ Created index on appointment_id';
END
ELSE
BEGIN
    PRINT '⚠️  appointment_id already exists in MedicalRecord';
END
GO

-- ========================================
-- 8. ADD stock_quantity TO Medicine
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Medicine') 
               AND name = 'stock_quantity')
BEGIN
    ALTER TABLE Medicine ADD stock_quantity INT DEFAULT 0;
    PRINT '✅ Added stock_quantity column to Medicine';
    
    -- Khởi tạo stock = 100 cho các thuốc hiện có
    UPDATE Medicine SET stock_quantity = 100 WHERE stock_quantity = 0;
    PRINT '✅ Initialized stock_quantity for existing medicines';
END
ELSE
BEGIN
    PRINT '⚠️  stock_quantity already exists in Medicine';
END
GO

-- ========================================
-- 9. ADD expiry_date TO Medicine
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Medicine') 
               AND name = 'expiry_date')
BEGIN
    ALTER TABLE Medicine ADD expiry_date DATE NULL;
    PRINT '✅ Added expiry_date column to Medicine';
    
    -- Set expiry date = 2 năm sau cho các thuốc hiện có
    UPDATE Medicine SET expiry_date = DATEADD(YEAR, 2, GETDATE()) WHERE expiry_date IS NULL;
    PRINT '✅ Set default expiry dates for existing medicines';
END
ELSE
BEGIN
    PRINT '⚠️  expiry_date already exists in Medicine';
END
GO

-- ========================================
-- 10. CREATE FUNCTION: Check slot availability
-- ========================================
IF OBJECT_ID('dbo.fn_IsSlotAvailable', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_IsSlotAvailable;
GO

CREATE FUNCTION dbo.fn_IsSlotAvailable(
    @AppointmentDate DATETIME,
    @DoctorId INT = NULL,
    @MaxSlotsPerTime INT = 4
)
RETURNS BIT
AS
BEGIN
    DECLARE @SlotCount INT;
    
    SELECT @SlotCount = COUNT(*)
    FROM Appointment
    WHERE appointment_date = @AppointmentDate
    AND status NOT IN ('cancelled', 'rejected')
    AND ISNULL(is_deleted, 0) = 0
    AND (@DoctorId IS NULL OR assigned_doctor_id = @DoctorId);
    
    IF @SlotCount < @MaxSlotsPerTime
        RETURN 1;
    
    RETURN 0;
END
GO

PRINT '✅ Created fn_IsSlotAvailable function';
GO

-- ========================================
-- 11. CREATE STORED PROCEDURE: Check slot conflicts
-- ========================================
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
    AND status NOT IN ('cancelled', 'rejected')
    AND ISNULL(is_deleted, 0) = 0
    AND (@DoctorId IS NULL OR assigned_doctor_id = @DoctorId);
    
    IF @CurrentCount < @MaxSlots
        SET @IsAvailable = 1;
    ELSE
        SET @IsAvailable = 0;
END
GO

PRINT '✅ Created sp_CheckSlotConflict stored procedure';
GO

-- ========================================
-- 12. CREATE VIEW: Appointment with patient info
-- ========================================
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
    
    -- Patient info (from patient_id FK or fallback to denormalized fields)
    CASE 
        WHEN a.patient_id IS NOT NULL THEN u_patient.fullname
        ELSE a.patient_name
    END AS patient_name,
    
    CASE 
        WHEN a.patient_id IS NOT NULL THEN u_patient.phone
        ELSE a.phone
    END AS patient_phone,
    
    CASE 
        WHEN a.patient_id IS NOT NULL THEN u_patient.email
        ELSE a.email
    END AS patient_email,
    
    a.patient_id,
    
    -- Service info
    s.service_id,
    s.service_name,
    s.price AS service_price,
    
    -- Assigned doctor info
    a.assigned_doctor_id,
    u_doctor.fullname AS doctor_name,
    staff.position AS doctor_position,
    staff.specialization AS doctor_specialization,
    
    -- Timestamps
    a.created_at,
    a.updated_at,
    a.is_deleted
    
FROM Appointment a
LEFT JOIN Patient p ON a.patient_id = p.patient_id
LEFT JOIN UserAccount u_patient ON p.user_id = u_patient.user_id
LEFT JOIN Service s ON a.service_id = s.service_id
LEFT JOIN Staff staff ON a.assigned_doctor_id = staff.staff_id
LEFT JOIN UserAccount u_doctor ON staff.user_id = u_doctor.user_id
WHERE ISNULL(a.is_deleted, 0) = 0;
GO

PRINT '✅ Created vw_AppointmentFull view';
GO

-- ========================================
-- 13. CREATE TRIGGER: Auto-deduct inventory
-- ========================================
IF OBJECT_ID('dbo.trg_Prescription_AfterInsert', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Prescription_AfterInsert;
GO

CREATE TRIGGER trg_Prescription_AfterInsert
ON Prescription
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Trừ số lượng thuốc trong kho
    UPDATE Medicine
    SET stock_quantity = stock_quantity - i.quantity
    FROM Medicine m
    INNER JOIN inserted i ON m.medicine_id = i.medicine_id
    WHERE m.stock_quantity >= i.quantity; -- Chỉ trừ nếu đủ hàng
    
    -- Log warning nếu tồn kho thấp
    IF EXISTS (SELECT 1 FROM Medicine WHERE stock_quantity < 10 AND stock_quantity > 0)
    BEGIN
        INSERT INTO AuditLog (user_id, action, description, timestamp)
        SELECT 
            1, -- System user
            'LOW_STOCK_WARNING',
            'Thuốc ' + name + ' sắp hết (còn ' + CAST(stock_quantity AS VARCHAR) + ')',
            GETDATE()
        FROM Medicine
        WHERE stock_quantity < 10 AND stock_quantity > 0;
    END
    
    -- Log error nếu hết hàng
    IF EXISTS (SELECT 1 FROM Medicine WHERE stock_quantity <= 0)
    BEGIN
        INSERT INTO AuditLog (user_id, action, description, timestamp)
        SELECT 
            1,
            'OUT_OF_STOCK_ERROR',
            'Thuốc ' + name + ' đã HẾT HÀNG!',
            GETDATE()
        FROM Medicine
        WHERE stock_quantity <= 0;
    END
END
GO

PRINT '✅ Created trg_Prescription_AfterInsert trigger for auto inventory deduction';
GO

-- ========================================
-- 14. CREATE INDEXES for performance
-- ========================================

-- Appointment indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Date_Status')
    CREATE INDEX IX_Appointment_Date_Status ON Appointment(appointment_date, status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_PatientId')
    CREATE INDEX IX_Appointment_PatientId ON Appointment(patient_id);

-- Medicine indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicine_StockQuantity')
    CREATE INDEX IX_Medicine_StockQuantity ON Medicine(stock_quantity);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Medicine_ExpiryDate')
    CREATE INDEX IX_Medicine_ExpiryDate ON Medicine(expiry_date);

PRINT '✅ Created additional performance indexes';
GO

-- ========================================
-- 15. SUMMARY
-- ========================================
PRINT '';
PRINT '========================================';
PRINT 'MIGRATION COMPLETE! ✅';
PRINT '========================================';
PRINT '';
PRINT 'CHANGES APPLIED:';
PRINT '1. ✅ Created EmailLog table';
PRINT '2. ✅ Normalized Appointment table (added patient_id FK)';
PRINT '3. ✅ Added assigned_doctor_id to Appointment';
PRINT '4. ✅ Added appointment_code (unique)';
PRINT '5. ✅ Added medical_record_id to Invoice';
PRINT '6. ✅ Added payment tracking to Invoice';
PRINT '7. ✅ Added appointment_id to MedicalRecord';
PRINT '8. ✅ Added stock_quantity to Medicine';
PRINT '9. ✅ Added expiry_date to Medicine';
PRINT '10. ✅ Created fn_IsSlotAvailable function';
PRINT '11. ✅ Created sp_CheckSlotConflict procedure';
PRINT '12. ✅ Created vw_AppointmentFull view';
PRINT '13. ✅ Created auto-deduct inventory trigger';
PRINT '14. ✅ Created performance indexes';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '1. Configure SMTP settings in App.config';
PRINT '2. Start EmailReminderService in Program.cs';
PRINT '3. Update DoctorExamine to use InvoiceAutoGenerator';
PRINT '4. Update booking forms to use slot conflict detection';
PRINT '========================================';
GO

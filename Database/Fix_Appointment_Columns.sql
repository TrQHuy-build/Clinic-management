-- =============================================
-- ADD MISSING COLUMNS TO APPOINTMENT TABLE
-- =============================================

USE DentalClinicDB;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '=========================================='
PRINT 'ADDING MISSING COLUMNS TO APPOINTMENT'
PRINT '=========================================='
PRINT ''

-- Add patient_id column (FK to Patient table)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'patient_id')
BEGIN
    ALTER TABLE Appointment
    ADD patient_id INT NULL;
    
    PRINT '✓ Added column: patient_id'
    
    -- Add FK constraint
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointment_Patient')
    BEGIN
        ALTER TABLE Appointment
        ADD CONSTRAINT FK_Appointment_Patient
        FOREIGN KEY (patient_id) REFERENCES Patient(patient_id);
        
        PRINT '✓ Added FK: FK_Appointment_Patient'
    END
END
ELSE
BEGIN
    PRINT '- Column patient_id already exists'
END
GO

-- Add assigned_doctor_id column (FK to Staff table)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'assigned_doctor_id')
BEGIN
    ALTER TABLE Appointment
    ADD assigned_doctor_id INT NULL;
    
    PRINT '✓ Added column: assigned_doctor_id'
    
    -- Add FK constraint
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointment_Doctor')
    BEGIN
        ALTER TABLE Appointment
        ADD CONSTRAINT FK_Appointment_Doctor
        FOREIGN KEY (assigned_doctor_id) REFERENCES Staff(staff_id);
        
        PRINT '✓ Added FK: FK_Appointment_Doctor'
    END
END
ELSE
BEGIN
    PRINT '- Column assigned_doctor_id already exists'
END
GO

PRINT ''
PRINT '=========================================='
PRINT 'UPDATING EXISTING APPOINTMENTS'
PRINT '=========================================='
PRINT ''

-- Update patient_id based on phone/email matching
UPDATE a
SET a.patient_id = (
    SELECT TOP 1 p.patient_id 
    FROM Patient p
    INNER JOIN UserAccount u ON p.user_id = u.user_id
    WHERE u.phone = a.phone OR u.email = a.email
    ORDER BY p.patient_id
)
WHERE a.patient_id IS NULL;

DECLARE @UpdatedPatients INT = @@ROWCOUNT;
PRINT '✓ Updated patient_id for ' + CAST(@UpdatedPatients AS VARCHAR) + ' appointments';

-- Assign random doctors to appointments that don't have one
DECLARE @DoctorIds TABLE (staff_id INT);
INSERT INTO @DoctorIds
SELECT s.staff_id 
FROM Staff s 
INNER JOIN UserAccount u ON s.user_id = u.user_id
WHERE s.position IN ('Doctor', 'doctor', N'Bác sĩ')
AND u.status = 'active';

DECLARE @DoctorCount INT = (SELECT COUNT(*) FROM @DoctorIds);

IF @DoctorCount > 0
BEGIN
    -- Update appointments with random doctors
    UPDATE a
    SET a.assigned_doctor_id = (
        SELECT TOP 1 staff_id 
        FROM @DoctorIds 
        ORDER BY NEWID()
    )
    FROM Appointment a
    WHERE a.assigned_doctor_id IS NULL
    AND a.status NOT IN ('cancelled', 'rejected');
    
    DECLARE @UpdatedDoctors INT = @@ROWCOUNT;
    PRINT '✓ Assigned doctors to ' + CAST(@UpdatedDoctors AS VARCHAR) + ' appointments';
END
ELSE
BEGIN
    PRINT '⚠ No active doctors found to assign!';
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_PatientId')
BEGIN
    CREATE INDEX IX_Appointment_PatientId ON Appointment(patient_id);
    PRINT '✓ Created index: IX_Appointment_PatientId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_DoctorId')
BEGIN
    CREATE INDEX IX_Appointment_DoctorId ON Appointment(assigned_doctor_id);
    PRINT '✓ Created index: IX_Appointment_DoctorId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Status')
BEGIN
    CREATE INDEX IX_Appointment_Status ON Appointment(status);
    PRINT '✓ Created index: IX_Appointment_Status'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_Date')
BEGIN
    CREATE INDEX IX_Appointment_Date ON Appointment(appointment_date);
    PRINT '✓ Created index: IX_Appointment_Date'
END
GO

PRINT ''
PRINT '=========================================='
PRINT 'VERIFICATION'
PRINT '=========================================='
PRINT ''

-- Show summary
SELECT 
    'Total Appointments' AS [Metric],
    COUNT(*) AS [Count]
FROM Appointment
UNION ALL
SELECT 
    'With Patient ID',
    COUNT(*)
FROM Appointment WHERE patient_id IS NOT NULL
UNION ALL
SELECT 
    'With Doctor Assigned',
    COUNT(*)
FROM Appointment WHERE assigned_doctor_id IS NOT NULL
UNION ALL
SELECT 
    'Missing Patient ID',
    COUNT(*)
FROM Appointment WHERE patient_id IS NULL
UNION ALL
SELECT 
    'Missing Doctor',
    COUNT(*)
FROM Appointment WHERE assigned_doctor_id IS NULL AND status NOT IN ('cancelled', 'rejected');
GO

PRINT ''
PRINT '✅ APPOINTMENT TABLE UPDATED SUCCESSFULLY!'
PRINT ''

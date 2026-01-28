-- =============================================
-- UPDATE APPOINTMENT DATA
-- Link patients and assign doctors
-- =============================================

USE DentalClinicDB;
GO

PRINT '=========================================='
PRINT 'UPDATING APPOINTMENT DATA'
PRINT '=========================================='
PRINT ''

-- Step 1: Link patients based on phone/email
PRINT 'Step 1: Linking patients...'

UPDATE Appointment
SET patient_id = (
    SELECT TOP 1 p.patient_id 
    FROM Patient p
    INNER JOIN UserAccount u ON p.user_id = u.user_id
    WHERE u.phone = Appointment.phone OR u.email = Appointment.email
    ORDER BY p.patient_id
)
WHERE patient_id IS NULL;

DECLARE @LinkedPatients INT = @@ROWCOUNT;
PRINT '  ✓ Linked ' + CAST(@LinkedPatients AS VARCHAR) + ' appointments to patients';
PRINT ''

-- Step 2: Assign doctors randomly to appointments
PRINT 'Step 2: Assigning doctors...'

-- Get list of active doctors
DECLARE @DoctorIds TABLE (staff_id INT, row_num INT);
INSERT INTO @DoctorIds
SELECT s.staff_id, ROW_NUMBER() OVER (ORDER BY s.staff_id) AS row_num
FROM Staff s 
INNER JOIN UserAccount u ON s.user_id = u.user_id
WHERE s.position IN ('Doctor', 'doctor', N'Bác sĩ')
AND u.status = 'active';

DECLARE @DoctorCount INT = (SELECT COUNT(*) FROM @DoctorIds);
PRINT '  Found ' + CAST(@DoctorCount AS VARCHAR) + ' active doctors';

IF @DoctorCount > 0
BEGIN
    -- Assign doctors to appointments using round-robin + randomness
    DECLARE @AppointmentId INT;
    DECLARE @Counter INT = 0;
    
    DECLARE appointment_cursor CURSOR FOR
    SELECT appointment_id
    FROM Appointment
    WHERE assigned_doctor_id IS NULL
    AND status NOT IN ('cancelled', 'rejected')
    ORDER BY appointment_date;
    
    OPEN appointment_cursor;
    FETCH NEXT FROM appointment_cursor INTO @AppointmentId;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Get a doctor (round-robin with some randomness)
        DECLARE @DoctorId INT = (
            SELECT TOP 1 staff_id 
            FROM @DoctorIds 
            WHERE row_num = ((@Counter % @DoctorCount) + 1)
        );
        
        UPDATE Appointment
        SET assigned_doctor_id = @DoctorId
        WHERE appointment_id = @AppointmentId;
        
        SET @Counter = @Counter + 1;
        
        -- Progress report every 1000
        IF @Counter % 1000 = 0
            PRINT '  - Processed ' + CAST(@Counter AS VARCHAR) + ' appointments...';
        
        FETCH NEXT FROM appointment_cursor INTO @AppointmentId;
    END
    
    CLOSE appointment_cursor;
    DEALLOCATE appointment_cursor;
    
    PRINT '  ✓ Assigned doctors to ' + CAST(@Counter AS VARCHAR) + ' appointments';
END
ELSE
BEGIN
    PRINT '  ⚠ WARNING: No active doctors found!';
END
GO

PRINT ''
PRINT '=========================================='
PRINT 'VERIFICATION'
PRINT '=========================================='
PRINT ''

-- Summary
SELECT 
    'Total Appointments' AS [Metric],
    COUNT(*) AS [Count],
    CAST(100.0 AS VARCHAR) + '%' AS [Percentage]
FROM Appointment
UNION ALL
SELECT 
    'With Patient ID',
    COUNT(*),
    CAST(CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Appointment) AS DECIMAL(5,2)) AS VARCHAR) + '%'
FROM Appointment WHERE patient_id IS NOT NULL
UNION ALL
SELECT 
    'With Doctor Assigned',
    COUNT(*),
    CAST(CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Appointment) AS DECIMAL(5,2)) AS VARCHAR) + '%'
FROM Appointment WHERE assigned_doctor_id IS NOT NULL
UNION ALL
SELECT 
    'Missing Patient ID',
    COUNT(*),
    CAST(CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Appointment) AS DECIMAL(5,2)) AS VARCHAR) + '%'
FROM Appointment WHERE patient_id IS NULL
UNION ALL
SELECT 
    'Missing Doctor (excl. cancelled)',
    COUNT(*),
    CAST(CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Appointment WHERE status NOT IN ('cancelled', 'rejected')) AS DECIMAL(5,2)) AS VARCHAR) + '%'
FROM Appointment 
WHERE assigned_doctor_id IS NULL 
AND status NOT IN ('cancelled', 'rejected');
GO

-- Show sample data
PRINT ''
PRINT 'Sample appointments with patient and doctor:'
PRINT ''

SELECT TOP 5
    a.appointment_id,
    a.patient_name,
    p.patient_id,
    a.appointment_date,
    a.status,
    u.fullname AS [Doctor Name],
    a.assigned_doctor_id
FROM Appointment a
LEFT JOIN Patient p ON a.patient_id = p.patient_id
LEFT JOIN Staff s ON a.assigned_doctor_id = s.staff_id
LEFT JOIN UserAccount u ON s.user_id = u.user_id
ORDER BY a.appointment_date DESC;
GO

PRINT ''
PRINT '✅ APPOINTMENT DATA UPDATED SUCCESSFULLY!'
PRINT ''

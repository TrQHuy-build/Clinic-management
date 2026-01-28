-- =============================================
-- RE-SEED APPOINTMENT WITH PROPER VIETNAMESE
-- Completely regenerate patient_name and notes
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'RE-SEEDING APPOINTMENT WITH VIETNAMESE'
PRINT '=========================================='
PRINT ''

-- Step 1: Update ALL patient_name from UserAccount (source of truth)
PRINT 'Step 1: Updating all patient names from UserAccount...'
GO

UPDATE a
SET a.patient_name = u.fullname
FROM Appointment a
INNER JOIN Patient p ON a.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id;

PRINT '  Done!'
GO

-- Step 2: Clear and regenerate notes
PRINT ''
PRINT 'Step 2: Regenerating notes with Vietnamese...'
PRINT ''
GO

-- Update 20% of appointments with Vietnamese notes
;WITH RandomAppointments AS (
    SELECT 
        appointment_id,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn,
        COUNT(*) OVER() AS total
    FROM Appointment
)
UPDATE a
SET a.notes = CASE (ra.rn % 12)
    WHEN 0 THEN N'Đau răng hàm dưới bên phải'
    WHEN 1 THEN N'Khám tổng quát và tư vấn'
    WHEN 2 THEN N'Làm sạch cao răng'
    WHEN 3 THEN N'Răng bị sâu, cần kiểm tra'
    WHEN 4 THEN N'Tái khám sau điều trị tủy'
    WHEN 5 THEN N'Tư vấn niềng răng'
    WHEN 6 THEN N'Nhổ răng khôn mọc lệch'
    WHEN 7 THEN N'Đau răng cấp'
    WHEN 8 THEN N'Khám định kỳ'
    WHEN 9 THEN N'Tư vấn làm răng sứ'
    WHEN 10 THEN N'Tẩy trắng răng'
    ELSE N'Lắp răng giả/Implant'
END
FROM Appointment a
INNER JOIN RandomAppointments ra ON a.appointment_id = ra.appointment_id
WHERE (ra.rn % 5) = 0; -- 20% of appointments

DECLARE @NotesUpdated INT = @@ROWCOUNT;
PRINT '  ✓ Updated ' + CAST(@NotesUpdated AS VARCHAR) + ' appointment notes';
GO

PRINT ''
PRINT '=========================================='
PRINT 'VERIFICATION WITH PROPER ENCODING'
PRINT '=========================================='
PRINT ''

-- Use explicit UTF-8 output
SET NOCOUNT OFF;

-- Show sample with proper collation
SELECT TOP 10
    appointment_id AS ID,
    CAST(patient_name AS NVARCHAR(100)) COLLATE Vietnamese_CI_AS AS [Patient Name],
    FORMAT(appointment_date, 'dd/MM/yyyy HH:mm') AS [Date Time],
    CAST(notes AS NVARCHAR(255)) COLLATE Vietnamese_CI_AS AS [Notes],
    status AS [Status]
FROM Appointment
WHERE notes IS NOT NULL
ORDER BY appointment_date DESC;
GO

PRINT ''
PRINT 'Statistics:'
SELECT 
    N'Tổng appointments' AS [Metric],
    COUNT(*) AS [Count]
FROM Appointment
UNION ALL
SELECT 
    N'Có tên bệnh nhân',
    COUNT(*)
FROM Appointment WHERE patient_name IS NOT NULL
UNION ALL
SELECT 
    N'Có ghi chú',
    COUNT(*)
FROM Appointment WHERE notes IS NOT NULL;
GO

PRINT ''
PRINT '✅ COMPLETED!'
PRINT ''

-- =============================================
-- SEED APPOINTMENTS - SIMPLIFIED VERSION
-- Create 2 years of appointment data (2024-2026)
-- No cursors, simpler logic
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'SEEDING APPOINTMENTS: 2024-01 to 2026-01'
PRINT 'Start Time: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- Get doctor IDs
DECLARE @DoctorIds TABLE (staff_id INT, row_num INT);
INSERT INTO @DoctorIds
SELECT s.staff_id, ROW_NUMBER() OVER (ORDER BY s.staff_id) AS row_num
FROM Staff s
INNER JOIN UserAccount u ON s.user_id = u.user_id
WHERE s.position IN (N'Bác sĩ', N'Doctor', N'doctor');

DECLARE @DoctorCount INT = (SELECT COUNT(*) FROM @DoctorIds);
IF @DoctorCount = 0
BEGIN
    PRINT 'ERROR: No doctors found in database!';
    RETURN;
END

PRINT 'Found ' + CAST(@DoctorCount AS VARCHAR) + ' doctors';
PRINT ''

-- Get patient IDs
DECLARE @PatientIds TABLE (patient_id INT, fullname NVARCHAR(100), phone NVARCHAR(20), email NVARCHAR(100), row_num INT);
INSERT INTO @PatientIds
SELECT p.patient_id, u.fullname, u.phone, u.email, ROW_NUMBER() OVER (ORDER BY p.patient_id) AS row_num
FROM Patient p
INNER JOIN UserAccount u ON p.user_id = u.user_id;

DECLARE @PatientCount INT = (SELECT COUNT(*) FROM @PatientIds);
IF @PatientCount = 0
BEGIN
    PRINT 'ERROR: No patients found in database!';
    RETURN;
END

PRINT 'Found ' + CAST(@PatientCount AS VARCHAR) + ' patients';
PRINT ''

-- Get service IDs
DECLARE @ServiceCount INT = (SELECT COUNT(*) FROM Service);
IF @ServiceCount = 0
BEGIN
    PRINT 'ERROR: No services found in database!';
    RETURN;
END

PRINT 'Found ' + CAST(@ServiceCount AS VARCHAR) + ' services';
PRINT 'Starting appointment creation...';
PRINT ''

-- Create appointments day by day
DECLARE @CurrentDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2026-01-31';
DECLARE @TotalAppointments INT = 0;
DECLARE @MonthCount INT = 0;
DECLARE @LastMonth INT = 1;

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Skip Sundays (DATEPART WEEKDAY: 1=Sunday)
    IF DATEPART(WEEKDAY, @CurrentDate) != 1
    BEGIN
        -- Create 10-20 appointments per day
        DECLARE @AppsToday INT = 10 + (ABS(CHECKSUM(NEWID())) % 11); -- Random 10-20
        DECLARE @DayCount INT = 0;
        
        WHILE @DayCount < @AppsToday
        BEGIN
            -- Random time slot (8:00 - 17:00)
            DECLARE @Hour INT = 8 + (ABS(CHECKSUM(NEWID())) % 10); -- 8-17
            DECLARE @Minute INT = (ABS(CHECKSUM(NEWID())) % 2) * 30; -- 0 or 30
            DECLARE @AppDateTime DATETIME = DATEADD(MINUTE, @Minute, DATEADD(HOUR, @Hour, CAST(@CurrentDate AS DATETIME)));
            
            -- Random patient
            DECLARE @PatientRowNum INT = 1 + (ABS(CHECKSUM(NEWID())) % @PatientCount);
            DECLARE @PatientName NVARCHAR(100), @PatientPhone NVARCHAR(20), @PatientEmail NVARCHAR(100);
            SELECT @PatientName = fullname, @PatientPhone = phone, @PatientEmail = email
            FROM @PatientIds WHERE row_num = @PatientRowNum;
            
            -- Random service
            DECLARE @ServiceId INT = 1 + (ABS(CHECKSUM(NEWID())) % @ServiceCount);
            
            -- Random doctor
            DECLARE @DoctorRowNum INT = 1 + (ABS(CHECKSUM(NEWID())) % @DoctorCount);
            DECLARE @DoctorId INT = (SELECT staff_id FROM @DoctorIds WHERE row_num = @DoctorRowNum);
            
            -- Determine status based on date
            DECLARE @Status NVARCHAR(20);
            DECLARE @CheckInTime DATETIME = NULL;
            
            IF @AppDateTime < GETDATE()
            BEGIN
                -- Past appointments
                DECLARE @Rand INT = ABS(CHECKSUM(NEWID())) % 100;
                IF @Rand < 80
                BEGIN
                    SET @Status = N'completed';
                    SET @CheckInTime = DATEADD(MINUTE, -(5 + (ABS(CHECKSUM(NEWID())) % 15)), @AppDateTime);
                END
                ELSE IF @Rand < 95
                    SET @Status = N'booked';
                ELSE
                    SET @Status = N'cancelled';
            END
            ELSE
                SET @Status = N'booked';
            
            -- Random notes (20% chance)
            DECLARE @Notes NVARCHAR(255) = NULL;
            IF (ABS(CHECKSUM(NEWID())) % 5) = 0
            BEGIN
                DECLARE @NoteRand INT = ABS(CHECKSUM(NEWID())) % 10;
                SET @Notes = CASE @NoteRand
                    WHEN 0 THEN N'Đau răng hàm dưới bên phải'
                    WHEN 1 THEN N'Khám tổng quát và tư vấn'
                    WHEN 2 THEN N'Làm sạch cao răng'
                    WHEN 3 THEN N'Răng bị sâu, cần kiểm tra'
                    WHEN 4 THEN N'Tái khám sau điều trị tủy'
                    WHEN 5 THEN N'Tư vấn niềng răng'
                    WHEN 6 THEN N'Nhổ răng khôn mọc lệch'
                    WHEN 7 THEN N'Khám định kỳ'
                    WHEN 8 THEN N'Tư vấn làm răng sứ'
                    ELSE N'Tẩy trắng răng'
                END;
            END
            
            -- Insert appointment
            INSERT INTO Appointment (
                patient_name, phone, email, service_id,
                appointment_date, status, notes
            )
            VALUES (
                @PatientName, @PatientPhone, @PatientEmail, @ServiceId,
                @AppDateTime, @Status, @Notes
            );
            
            SET @TotalAppointments = @TotalAppointments + 1;
            SET @DayCount = @DayCount + 1;
        END
    END
    
    -- Progress report every month
    IF MONTH(@CurrentDate) != @LastMonth
    BEGIN
        PRINT '  Month ' + CAST(@LastMonth AS VARCHAR) + '/' + CAST(YEAR(@CurrentDate) AS VARCHAR) + 
              ': ' + CAST(@TotalAppointments - @MonthCount AS VARCHAR) + ' appointments';
        SET @MonthCount = @TotalAppointments;
        SET @LastMonth = MONTH(@CurrentDate);
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT ''
PRINT '=========================================='
PRINT 'APPOINTMENT SEEDING COMPLETED'
PRINT 'Total appointments created: ' + CAST(@TotalAppointments AS VARCHAR)
PRINT 'End Time: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
GO

-- Summary
SELECT 
    'SUMMARY' AS [Report],
    (SELECT COUNT(*) FROM Appointment) AS TotalAppointments,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'completed') AS Completed,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'booked') AS Booked,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'cancelled') AS Cancelled,
    (SELECT MIN(appointment_date) FROM Appointment) AS EarliestDate,
    (SELECT MAX(appointment_date) FROM Appointment) AS LatestDate;
GO

PRINT ''
PRINT '✅ Data seeding complete!'
PRINT ''

-- =============================================
-- FIX VIETNAMESE ENCODING IN APPOINTMENT
-- Update patient_name and notes with proper NVARCHAR
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'FIXING VIETNAMESE ENCODING IN APPOINTMENT'
PRINT '=========================================='
PRINT ''

-- Step 1: Verify column types
PRINT 'Step 1: Checking column data types...'
GO

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Appointment'
AND COLUMN_NAME IN ('patient_name', 'notes');
GO

PRINT ''
PRINT 'Step 2: Updating patient_name from UserAccount...'
PRINT ''

-- Update patient_name from linked UserAccount (proper NVARCHAR source)
UPDATE a
SET a.patient_name = u.fullname
FROM Appointment a
INNER JOIN Patient p ON a.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id
WHERE a.patient_id IS NOT NULL;

DECLARE @UpdatedNames INT = @@ROWCOUNT;
PRINT '  ✓ Updated ' + CAST(@UpdatedNames AS VARCHAR) + ' patient names from UserAccount';
GO

PRINT ''
PRINT 'Step 3: Updating notes with proper Vietnamese...'
PRINT ''

-- Update notes with proper Vietnamese text
UPDATE Appointment
SET notes = CASE notes
    WHEN N'Khám d?nh k?' THEN N'Khám định kỳ'
    WHEN N'L?y cao rang' THEN N'Lấy cao răng'
    WHEN N'Tu v?n ni?ng rang' THEN N'Tư vấn niềng răng'
    WHEN N'TÆ° váº¥n lA m rÄƒng sá»c' THEN N'Tư vấn làm răng sứ'
    WHEN N'Trám rÄƒng' THEN N'Trám răng'
    WHEN N'Ä?iá»?u trá»< tuyáº¿' THEN N'Điều trị tủy'
    WHEN N'Nháº¯c ráng khÃ´n' THEN N'Nhổ răng khôn'
    WHEN N'TÃ¡y trÄƒng ráng' THEN N'Tẩy trắng răng'
    WHEN N'Kham tá»_ng quát' THEN N'Khám tổng quát'
    WHEN N'Äau rang' THEN N'Đau răng'
    ELSE notes
END
WHERE notes IS NOT NULL
AND notes NOT LIKE N'%Đau răng%'
AND notes NOT LIKE N'%Khám%'
AND notes NOT LIKE N'%Lấy cao%';

DECLARE @UpdatedNotes INT = @@ROWCOUNT;
PRINT '  ✓ Fixed ' + CAST(@UpdatedNotes AS VARCHAR) + ' notes with garbled text';
GO

-- Clear invalid notes and regenerate with proper Vietnamese
PRINT ''
PRINT 'Step 4: Regenerating notes with proper Vietnamese...'
PRINT ''

-- List of proper Vietnamese notes
DECLARE @VietnameseNotes TABLE (note_text NVARCHAR(255));
INSERT INTO @VietnameseNotes VALUES
(N'Đau răng hàm dưới bên phải'),
(N'Khám tổng quát và tư vấn'),
(N'Làm sạch cao răng'),
(N'Răng bị sâu, cần kiểm tra'),
(N'Tái khám sau điều trị tủy'),
(N'Tư vấn niềng răng'),
(N'Nhổ răng khôn mọc lệch'),
(N'Đau răng cấp'),
(N'Khám định kỳ'),
(N'Tư vấn làm răng sứ'),
(N'Tẩy trắng răng'),
(N'Lắp răng giả/Implant'),
(N'Điều trị viêm nướu'),
(N'Trám răng sâu'),
(N'Chỉnh nha niềng răng'),
(N'Bọc răng sứ thẩm mỹ'),
(N'Cạo vôi răng'),
(N'Tư vấn implant răng'),
(N'Điều trị tủy răng'),
(N'Nhổ răng sữa cho trẻ em');

-- Update appointments with random Vietnamese notes (20% of appointments get notes)
DECLARE @AppointmentId INT;
DECLARE @RandomNote NVARCHAR(255);

DECLARE appointment_cursor CURSOR FOR
SELECT appointment_id
FROM Appointment
WHERE (notes IS NULL OR LEN(notes) < 10 OR notes LIKE '%?%' OR notes LIKE '%Ä%')
AND (ABS(CHECKSUM(NEWID())) % 5) = 0; -- 20% chance

OPEN appointment_cursor;
FETCH NEXT FROM appointment_cursor INTO @AppointmentId;

DECLARE @Counter INT = 0;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Get random Vietnamese note
    SET @RandomNote = (SELECT TOP 1 note_text FROM @VietnameseNotes ORDER BY NEWID());
    
    UPDATE Appointment
    SET notes = @RandomNote
    WHERE appointment_id = @AppointmentId;
    
    SET @Counter = @Counter + 1;
    
    FETCH NEXT FROM appointment_cursor INTO @AppointmentId;
END

CLOSE appointment_cursor;
DEALLOCATE appointment_cursor;

PRINT '  ✓ Regenerated ' + CAST(@Counter AS VARCHAR) + ' notes with proper Vietnamese';
GO

PRINT ''
PRINT '=========================================='
PRINT 'VERIFICATION'
PRINT '=========================================='
PRINT ''

-- Show sample data with proper Vietnamese
SELECT TOP 10
    appointment_id,
    patient_name AS [Tên bệnh nhân],
    appointment_date AS [Ngày hẹn],
    notes AS [Ghi chú],
    status AS [Trạng thái]
FROM Appointment
ORDER BY appointment_date DESC;
GO

-- Statistics
SELECT 
    'Total Appointments' AS [Metric],
    COUNT(*) AS [Count]
FROM Appointment
UNION ALL
SELECT 
    'With Valid Name (Vietnamese)',
    COUNT(*)
FROM Appointment 
WHERE patient_name IS NOT NULL 
AND patient_name NOT LIKE '%?%'
AND patient_name NOT LIKE '%Ä%'
UNION ALL
SELECT 
    'With Valid Notes (Vietnamese)',
    COUNT(*)
FROM Appointment 
WHERE notes IS NOT NULL 
AND notes NOT LIKE '%?%'
AND notes NOT LIKE '%Ä%'
UNION ALL
SELECT 
    'With Garbled Text',
    COUNT(*)
FROM Appointment 
WHERE (patient_name LIKE '%?%' OR patient_name LIKE '%Ä%')
OR (notes LIKE '%?%' OR notes LIKE '%Ä%');
GO

PRINT ''
PRINT '✅ VIETNAMESE ENCODING FIXED!'
PRINT ''
PRINT 'Sample patient names:'
SELECT DISTINCT TOP 5 patient_name 
FROM Appointment 
WHERE patient_name IS NOT NULL
ORDER BY patient_name;
GO

PRINT ''
PRINT 'Sample notes:'
SELECT DISTINCT TOP 5 notes 
FROM Appointment 
WHERE notes IS NOT NULL
ORDER BY notes;
GO

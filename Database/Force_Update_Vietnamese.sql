-- =============================================
-- FORCE UPDATE VIETNAMESE IN DATABASE
-- Direct UPDATE with proper NVARCHAR
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'FORCE UPDATING VIETNAMESE IN DATABASE'
PRINT '=========================================='
PRINT ''

-- Step 1: Update ALL patient_name directly
PRINT 'Step 1: Updating patient_name with Vietnamese...'
GO

-- Update from UserAccount (100% source)
UPDATE a
SET a.patient_name = u.fullname
FROM Appointment a
INNER JOIN Patient p ON a.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id
WHERE a.patient_id IS NOT NULL;

PRINT '  Done updating patient names!'
GO

-- Step 2: Update notes directly with specific Vietnamese text
PRINT ''
PRINT 'Step 2: Updating notes with Vietnamese...'
PRINT ''
GO

-- Clear all garbled notes first
UPDATE Appointment
SET notes = NULL
WHERE notes IS NOT NULL
AND (
    notes LIKE '%?%'
    OR notes LIKE '%Ä%'
    OR notes LIKE '%á»%'
    OR notes LIKE '%Ã%'
    OR notes LIKE '%â%'
);

PRINT '  Cleared garbled notes'
GO

-- Update with proper Vietnamese notes (30% of appointments)
;WITH NumberedAppointments AS (
    SELECT 
        appointment_id,
        ROW_NUMBER() OVER (ORDER BY appointment_id) AS rn
    FROM Appointment
    WHERE notes IS NULL
)
UPDATE a
SET a.notes = CASE (na.rn % 20)
    WHEN 0 THEN N'Đau răng hàm dưới bên phải'
    WHEN 1 THEN N'Khám tổng quát và tư vấn'
    WHEN 2 THEN N'Làm sạch cao răng'
    WHEN 3 THEN N'Răng bị sâu cần kiểm tra'
    WHEN 4 THEN N'Tái khám sau điều trị tủy'
    WHEN 5 THEN N'Tư vấn niềng răng'
    WHEN 6 THEN N'Nhổ răng khôn mọc lệch'
    WHEN 7 THEN N'Đau răng cấp cần khám ngay'
    WHEN 8 THEN N'Khám định kỳ 6 tháng'
    WHEN 9 THEN N'Tư vấn làm răng sứ thẩm mỹ'
    WHEN 10 THEN N'Tẩy trắng răng tại phòng khám'
    WHEN 11 THEN N'Lắp răng giả hoặc Implant'
    WHEN 12 THEN N'Điều trị viêm nướu'
    WHEN 13 THEN N'Trám răng sâu'
    WHEN 14 THEN N'Chỉnh nha niềng răng kim loại'
    WHEN 15 THEN N'Bọc răng sứ'
    WHEN 16 THEN N'Cạo vôi răng siêu âm'
    WHEN 17 THEN N'Tư vấn cấy ghép Implant'
    WHEN 18 THEN N'Điều trị tủy răng'
    ELSE N'Nhổ răng sữa cho trẻ em'
END
FROM Appointment a
INNER JOIN NumberedAppointments na ON a.appointment_id = na.appointment_id
WHERE (na.rn % 3) = 0; -- 33% có notes

DECLARE @NotesCount INT = @@ROWCOUNT;
PRINT '  Updated ' + CAST(@NotesCount AS VARCHAR) + ' notes with Vietnamese'
GO

PRINT ''
PRINT '=========================================='
PRINT 'FINAL VERIFICATION'
PRINT '=========================================='
PRINT ''
GO

-- Show sample data
PRINT 'Sample 10 appointments with Vietnamese:'
PRINT ''
GO

SELECT TOP 10
    appointment_id AS ID,
    patient_name AS [Ten benh nhan],
    FORMAT(appointment_date, 'dd/MM/yyyy HH:mm') AS [Ngay hen],
    ISNULL(notes, N'(Không có ghi chú)') AS [Ghi chu],
    status AS [Trang thai]
FROM Appointment
ORDER BY appointment_id DESC;
GO

-- Statistics
PRINT ''
PRINT 'Statistics:'
GO

SELECT 
    N'Tổng số appointments' AS [Thong ke],
    COUNT(*) AS [So luong]
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
PRINT '=========================================='
PRINT 'COMPLETED!'
PRINT '=========================================='
PRINT ''
PRINT 'Du lieu da duoc cap nhat voi tieng Viet NVARCHAR'
PRINT 'Chay ung dung de xem ket qua!'
PRINT ''
GO

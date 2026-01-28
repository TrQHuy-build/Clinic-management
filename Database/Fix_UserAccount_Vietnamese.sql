-- ===============================================
-- FIX USERACCOUNT VIETNAMESE ENCODING
-- Xóa và tạo lại data với NVARCHAR đúng
-- ===============================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'BƯỚC 1: XÓA DỮ LIỆU SAI';
PRINT '========================================';

-- Xóa appointments trước (FK constraint)
DELETE FROM Appointment;
PRINT 'Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' appointments';

-- Xóa patients (có FK tới UserAccount)
DELETE FROM Patient WHERE user_id IN (
    SELECT user_id FROM UserAccount WHERE role = 'patient'
);
PRINT 'Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' patients';

-- Xóa UserAccount role=patient
DELETE FROM UserAccount WHERE role = 'patient';
PRINT 'Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' user accounts (patient)';

GO

PRINT '';
PRINT '========================================';
PRINT 'BƯỚC 2: TẠO LẠI PATIENTS VỚI UTF-8 ĐÚNG';
PRINT '========================================';

-- Tạo danh sách họ tên tiếng Việt CHUẨN
DECLARE @FirstNames TABLE (name NVARCHAR(50));
INSERT INTO @FirstNames VALUES
    (N'Nguyễn'), (N'Trần'), (N'Lê'), (N'Phạm'), (N'Hoàng'),
    (N'Huỳnh'), (N'Vũ'), (N'Võ'), (N'Phan'), (N'Trương'),
    (N'Bùi'), (N'Đặng'), (N'Đỗ'), (N'Ngô'), (N'Dương'),
    (N'Lý'), (N'Hà'), (N'Đinh'), (N'Mai'), (N'Tô');

DECLARE @MiddleNames TABLE (name NVARCHAR(50));
INSERT INTO @MiddleNames VALUES
    (N'Văn'), (N'Thị'), (N'Hữu'), (N'Minh'), (N'Bảo'),
    (N'Thanh'), (N'Quốc'), (N'Anh'), (N'Kim'), (N'Xuân'),
    (N'Thu'), (N'Hồng'), (N'Phương'), (N'Hà'), (N'Ngọc');

DECLARE @LastNames TABLE (name NVARCHAR(50));
INSERT INTO @LastNames VALUES
    (N'An'), (N'Bình'), (N'Cường'), (N'Dũng'), (N'Em'),
    (N'Phong'), (N'Giang'), (N'Hải'), (N'Hùng'), (N'Khoa'),
    (N'Long'), (N'Minh'), (N'Nam'), (N'Oanh'), (N'Phúc'),
    (N'Quyên'), (N'Sơn'), (N'Thảo'), (N'Uyên'), (N'Vy'),
    (N'Hoa'), (N'Lan'), (N'Linh'), (N'Mai'), (N'Nhung'),
    (N'Phương'), (N'Quỳnh'), (N'Tâm'), (N'Tú'), (N'Yến'),
    (N'Anh'), (N'Đạt'), (N'Khanh'), (N'Tuấn'), (N'Vân');

-- Tạo 104 patients mới
DECLARE @counter INT = 1;
DECLARE @fullname NVARCHAR(100);
DECLARE @email NVARCHAR(100);
DECLARE @phone NVARCHAR(20);
DECLARE @user_id INT;
DECLARE @patient_id INT;

-- Lấy giá trị tối đa hiện tại
DECLARE @max_user_id INT = (SELECT ISNULL(MAX(user_id), 0) FROM UserAccount);
DECLARE @max_patient_id INT = (SELECT ISNULL(MAX(patient_id), 0) FROM Patient);

WHILE @counter <= 104
BEGIN
    -- Tạo tên random từ các components
    SET @fullname = (
        SELECT TOP 1 f.name + N' ' + m.name + N' ' + l.name
        FROM @FirstNames f
        CROSS JOIN @MiddleNames m
        CROSS JOIN @LastNames l
        ORDER BY NEWID()
    );
    
    SET @email = 'patient' + CAST(@counter AS VARCHAR) + '@gmail.com';
    SET @phone = '0' + RIGHT('00000000' + CAST((900000000 + @counter * 1111) AS VARCHAR), 9);
    
    -- Insert vào UserAccount
    INSERT INTO UserAccount (username, password_hash, email, phone, fullname, role, is_active, created_at)
    VALUES (
        @email,
        '$2a$11$YourHashedPasswordHere', -- Fixed hash
        @email,
        @phone,
        @fullname,
        'patient',
        1,
        DATEADD(DAY, -@counter, GETDATE())
    );
    
    SET @user_id = SCOPE_IDENTITY();
    
    -- Insert vào Patient
    INSERT INTO Patient (user_id, date_of_birth, gender, address, blood_type, allergies, medical_notes)
    VALUES (
        @user_id,
        DATEADD(YEAR, -(20 + (@counter % 50)), GETDATE()), -- Tuổi 20-70
        CASE WHEN @counter % 2 = 0 THEN N'Nam' ELSE N'Nữ' END,
        N'Địa chỉ ' + CAST(@counter AS NVARCHAR) + N', Quận ' + CAST((@counter % 12) + 1 AS NVARCHAR) + N', TP.HCM',
        CASE (@counter % 4) WHEN 0 THEN 'A' WHEN 1 THEN 'B' WHEN 2 THEN 'AB' ELSE 'O' END,
        CASE WHEN @counter % 10 = 0 THEN N'Dị ứng thuốc kháng sinh' ELSE NULL END,
        CASE WHEN @counter % 7 = 0 THEN N'Bệnh nhân cần theo dõi đặc biệt' ELSE NULL END
    );
    
    SET @patient_id = SCOPE_IDENTITY();
    
    IF @counter % 10 = 0
        PRINT N'Đã tạo ' + CAST(@counter AS NVARCHAR) + '/104 patients...';
    
    SET @counter = @counter + 1;
END

PRINT '';
PRINT N'✅ Đã tạo 104 patients mới với tên tiếng Việt CHUẨN!';
PRINT '';

-- Kiểm tra kết quả
SELECT TOP 10 
    u.user_id,
    u.fullname AS [Tên đầy đủ],
    u.phone AS [Số điện thoại],
    p.patient_id,
    CAST(u.fullname AS VARBINARY(200)) AS [Hex value]
FROM UserAccount u
INNER JOIN Patient p ON u.user_id = p.user_id
WHERE u.role = 'patient'
ORDER BY u.user_id DESC;

PRINT '';
PRINT '========================================';
PRINT 'BƯỚC 3: TẠO LẠI APPOINTMENTS';
PRINT '========================================';

-- Tạo lại appointments với dữ liệu mới
DECLARE @start_date DATE = '2024-01-01';
DECLARE @end_date DATE = '2026-01-31';
DECLARE @current_date DATE = @start_date;
DECLARE @appointments_created INT = 0;

DECLARE @patient_ids TABLE (pid INT);
INSERT INTO @patient_ids SELECT patient_id FROM Patient;

DECLARE @service_ids TABLE (sid INT);
INSERT INTO @service_ids SELECT service_id FROM Service;

DECLARE @doctor_ids TABLE (did INT);
INSERT INTO @doctor_ids SELECT staff_id FROM Staff WHERE role = N'Bác sĩ';

-- Tạo appointments
WHILE @current_date <= @end_date
BEGIN
    -- Skip Chủ nhật
    IF DATEPART(WEEKDAY, @current_date) <> 1
    BEGIN
        DECLARE @appointments_today INT = 10 + (ABS(CHECKSUM(NEWID())) % 11); -- 10-20 appointments/day
        DECLARE @appt_counter INT = 0;
        
        WHILE @appt_counter < @appointments_today
        BEGIN
            DECLARE @time_hour INT = 8 + (@appt_counter % 9); -- 8AM-4PM
            DECLARE @time_minute INT = (@appt_counter % 2) * 30; -- :00 or :30
            
            DECLARE @random_patient INT = (SELECT TOP 1 pid FROM @patient_ids ORDER BY NEWID());
            DECLARE @random_service INT = (SELECT TOP 1 sid FROM @service_ids ORDER BY NEWID());
            DECLARE @random_doctor INT = (SELECT TOP 1 did FROM @doctor_ids ORDER BY NEWID());
            
            DECLARE @random_status NVARCHAR(20);
            DECLARE @random_notes NVARCHAR(255) = NULL;
            
            -- 80% completed, 15% booked, 5% cancelled
            DECLARE @status_rand INT = ABS(CHECKSUM(NEWID())) % 100;
            IF @status_rand < 80
            BEGIN
                SET @random_status = N'Completed';
                -- 50% có ghi chú
                IF @status_rand % 2 = 0
                    SET @random_notes = (
                        SELECT TOP 1 note FROM (VALUES
                            (N'Đau răng hàm dưới bên phải'),
                            (N'Khám tổng quát và tư vấn'),
                            (N'Làm sạch cao răng'),
                            (N'Răng bị sâu cần kiểm tra'),
                            (N'Tái khám sau điều trị tủy'),
                            (N'Tư vấn niềng răng'),
                            (N'Khám định kỳ'),
                            (N'Điều trị viêm nướu'),
                            (N'Bọc răng sứ'),
                            (N'Nhổ răng khôn'),
                            (N'Trám răng thẩm mỹ'),
                            (N'Tẩy trắng răng'),
                            (N'Điều trị tủy răng'),
                            (N'Cấy ghép Implant'),
                            (N'Làm cầu răng sứ'),
                            (N'Chụp X-quang răng'),
                            (N'Kiểm tra tổng quát'),
                            (N'Vệ sinh răng miệng'),
                            (N'Điều trị sâu răng'),
                            (N'Tư vấn chỉnh nha')
                        ) AS notes(note)
                        ORDER BY NEWID()
                    );
            END
            ELSE IF @status_rand < 95
                SET @random_status = N'Booked';
            ELSE
            BEGIN
                SET @random_status = N'Cancelled';
                SET @random_notes = N'Bệnh nhân hủy lịch';
            END
            
            INSERT INTO Appointment (
                patient_id,
                service_id,
                assigned_doctor_id,
                appointment_date,
                status,
                notes,
                patient_name,
                created_at
            )
            SELECT 
                @random_patient,
                @random_service,
                @random_doctor,
                DATEADD(MINUTE, @time_minute, DATEADD(HOUR, @time_hour, @current_date)),
                @random_status,
                @random_notes,
                u.fullname,
                @current_date
            FROM Patient p
            INNER JOIN UserAccount u ON p.user_id = u.user_id
            WHERE p.patient_id = @random_patient;
            
            SET @appointments_created = @appointments_created + 1;
            SET @appt_counter = @appt_counter + 1;
        END
    END
    
    IF DAY(@current_date) = 1
        PRINT N'Đã tạo appointments đến ' + FORMAT(@current_date, 'MM/yyyy') + N': ' + CAST(@appointments_created AS NVARCHAR) + N' appointments';
    
    SET @current_date = DATEADD(DAY, 1, @current_date);
END

PRINT '';
PRINT N'✅ Tạo xong ' + CAST(@appointments_created AS NVARCHAR) + N' appointments!';
PRINT '';

-- Kiểm tra kết quả cuối cùng
SELECT TOP 10
    a.appointment_id,
    a.patient_name AS [Tên bệnh nhân],
    FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
    a.notes AS [Ghi chú],
    a.status AS [Trạng thái],
    CAST(a.patient_name AS VARBINARY(200)) AS [Hex value]
FROM Appointment a
WHERE a.notes IS NOT NULL
ORDER BY a.appointment_id DESC;

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT! CHẠy APPLICATION ĐỂ KIỂM TRA';
PRINT '========================================';

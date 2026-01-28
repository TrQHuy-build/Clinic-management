-- =============================================
-- RESET DATABASE & SEED NEW DATA (2024-2026)
-- Date: January 28, 2026
-- Purpose: Clean all data and create realistic data from 2024-01 to 2026-01
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'STARTING DATABASE RESET AND SEED PROCESS'
PRINT 'Date Range: January 2024 - January 2026'
PRINT '=========================================='
PRINT ''

-- =============================================
-- STEP 1: DISABLE FOREIGN KEY CONSTRAINTS
-- =============================================
PRINT 'Step 1: Disabling foreign key constraints...'
GO

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO

-- =============================================
-- STEP 2: DELETE ALL DATA
-- =============================================
PRINT 'Step 2: Deleting all existing data...'
GO

DELETE FROM InvoicePrescription;
DELETE FROM ServiceUsage;
DELETE FROM InventoryTransaction;
DELETE FROM EmailLog;
DELETE FROM Prescription;
DELETE FROM Invoice;
DELETE FROM MedicalRecord;
DELETE FROM Appointment;
DELETE FROM Patient;
DELETE FROM Inventory;
DELETE FROM TreatmentService;
DELETE FROM AuditLog;
DELETE FROM PasswordReset;
DELETE FROM Shift;
DELETE FROM Salary;
-- Keep Staff, Service, Medicine, UserAccount

PRINT '  - Deleted all transactional data'
PRINT '  - Kept Staff, Service, Medicine, UserAccount'
GO

-- =============================================
-- STEP 3: RESET IDENTITY SEEDS
-- =============================================
PRINT 'Step 3: Resetting identity seeds...'
GO

DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('Patient', RESEED, 0);
DBCC CHECKIDENT ('MedicalRecord', RESEED, 0);
DBCC CHECKIDENT ('Invoice', RESEED, 0);
DBCC CHECKIDENT ('Prescription', RESEED, 0);
DBCC CHECKIDENT ('EmailLog', RESEED, 0);
DBCC CHECKIDENT ('InventoryTransaction', RESEED, 0);
DBCC CHECKIDENT ('ServiceUsage', RESEED, 0);
DBCC CHECKIDENT ('InvoicePrescription', RESEED, 0);
DBCC CHECKIDENT ('Inventory', RESEED, 0);
GO

-- =============================================
-- STEP 4: RE-ENABLE FOREIGN KEY CONSTRAINTS
-- =============================================
PRINT 'Step 4: Re-enabling foreign key constraints...'
GO

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
GO

PRINT ''
PRINT '=========================================='
PRINT 'STARTING DATA SEEDING'
PRINT '=========================================='
PRINT ''

-- =============================================
-- SEED 1: PATIENTS (100 patients)
-- =============================================
PRINT 'Seed 1: Creating 100 patients...'
GO

DECLARE @PatientCounter INT = 1;
DECLARE @FirstNames TABLE (name NVARCHAR(50));
DECLARE @LastNames TABLE (name NVARCHAR(50));

-- Vietnamese first names
INSERT INTO @FirstNames VALUES 
(N'Nguyễn'),(N'Trần'),(N'Lê'),(N'Phạm'),(N'Hoàng'),(N'Huỳnh'),(N'Phan'),(N'Vũ'),(N'Võ'),(N'Đặng'),
(N'Bùi'),(N'Đỗ'),(N'Hồ'),(N'Ngô'),(N'Dương'),(N'Lý'),(N'Mai'),(N'Đinh'),(N'Tô'),(N'Trương');

-- Vietnamese middle/given names
INSERT INTO @LastNames VALUES
(N'Văn An'),(N'Thị Bình'),(N'Minh Châu'),(N'Thị Dung'),(N'Văn Dũng'),(N'Thị Em'),(N'Văn Giang'),(N'Thị Hà'),
(N'Văn Hải'),(N'Thị Hương'),(N'Văn Kiên'),(N'Thị Lan'),(N'Văn Long'),(N'Thị Mai'),(N'Văn Nam'),
(N'Thị Nga'),(N'Văn Phong'),(N'Thị Quyên'),(N'Văn Sơn'),(N'Thị Thảo'),(N'Văn Thắng'),(N'Thị Thu'),
(N'Văn Tuấn'),(N'Thị Tuyết'),(N'Văn Vũ'),(N'Thị Xuân'),(N'Minh Anh'),(N'Hoàng Bảo'),(N'Thanh Bình'),
(N'Quốc Cường'),(N'Hữu Đạt'),(N'Đức Duy'),(N'Gia Hân'),(N'Bảo Hân'),(N'Khánh Hòa'),(N'Gia Hưng'),
(N'Bảo Khang'),(N'Anh Khoa'),(N'Minh Khuê'),(N'Hồng Loan'),(N'Thanh Long'),(N'Trúc Mai'),(N'Quỹnh My'),
(N'Phương Nam'),(N'Hải Nam'),(N'Hà Nhi'),(N'Bích Ngọc'),(N'Ngọc Nhi'),(N'Thanh Nhàn'),(N'Phương Oanh'),
(N'Thu Phương'),(N'Quang Phúc'),(N'Bảo Quân'),(N'Khánh Quyên'),(N'Thanh Sang'),(N'Quang Sơn'),(N'Minh Tâm'),
(N'Hoài Thương'),(N'Minh Thư'),(N'Hồng Thy'),(N'Anh Thư'),(N'Bảo Trâm'),(N'Gia Trí'),(N'Thanh Trúc'),
(N'Phương Trang'),(N'Minh Triết'),(N'Bích Trâm'),(N'Đức Trung'),(N'Tuấn Tú'),(N'Hồng Vân'),(N'Khánh Vy'),
(N'Hữu Vinh'),(N'Thanh Xuân'),(N'Quang Huy'),(N'Đình Huy'),(N'Minh Hiếu'),(N'Hoàng Anh'),(N'Tuấn Kiệt'),
(N'Minh Khang'),(N'Hoàng Long'),(N'Hữu Lộc'),(N'Thanh Lương'),(N'Minh Nhật'),(N'Bảo Phúc'),(N'Gia Phúc'),
(N'Quốc Thắng'),(N'Hữu Thịnh'),(N'Minh Tiến'),(N'Đức Trí'),(N'Quốc Tuấn'),(N'Hoàng Vũ'),(N'Anh Vũ');

WHILE @PatientCounter <= 100
BEGIN
    DECLARE @RandomFirst NVARCHAR(50) = (SELECT TOP 1 name FROM @FirstNames ORDER BY NEWID());
    DECLARE @RandomLast NVARCHAR(50) = (SELECT TOP 1 name FROM @LastNames ORDER BY NEWID());
    DECLARE @FullName NVARCHAR(100) = @RandomFirst + N' ' + @RandomLast;
    DECLARE @Gender NVARCHAR(10) = CASE WHEN (@PatientCounter % 2) = 0 THEN N'Nam' ELSE N'Nữ' END;
    DECLARE @Phone NVARCHAR(20) = '09' + RIGHT('0000000' + CAST(ABS(CHECKSUM(NEWID())) % 100000000 AS VARCHAR), 8);
    DECLARE @Email NVARCHAR(100) = LOWER(REPLACE(@RandomLast, N' ', '')) + CAST(@PatientCounter AS VARCHAR) + '@email.com';
    DECLARE @BirthYear INT = 1960 + (ABS(CHECKSUM(NEWID())) % 45); -- 1960-2004
    DECLARE @BirthMonth INT = 1 + (ABS(CHECKSUM(NEWID())) % 12);
    DECLARE @BirthDay INT = 1 + (ABS(CHECKSUM(NEWID())) % 28);
    DECLARE @DOB DATE = CAST(CAST(@BirthYear AS VARCHAR) + '-' + 
                              RIGHT('0' + CAST(@BirthMonth AS VARCHAR), 2) + '-' + 
                              RIGHT('0' + CAST(@BirthDay AS VARCHAR), 2) AS DATE);
    
    -- Vietnamese addresses
    DECLARE @Streets TABLE (street NVARCHAR(100));
    INSERT INTO @Streets VALUES 
    (N'Lý Thường Kiệt'),(N'Trần Hưng Đạo'),(N'Nguyễn Huệ'),(N'Lê Lợi'),(N'Hai Bà Trưng'),
    (N'Nguyễn Trãi'),(N'Phan Đình Phùng'),(N'Hoàng Diệu'),(N'Điện Biên Phủ'),(N'Cách Mạng Tháng 8'),
    (N'Lê Duẩn'),(N'Võ Văn Kiệt'),(N'Pasteur'),(N'Nam Kỳ Khởi Nghĩa'),(N'Nguyễn Đình Chiểu');
    
    DECLARE @Districts TABLE (district NVARCHAR(50));
    INSERT INTO @Districts VALUES
    (N'Quận 1'),(N'Quận 2'),(N'Quận 3'),(N'Quận 4'),(N'Quận 5'),(N'Quận 6'),(N'Quận 7'),
    (N'Quận 8'),(N'Quận 9'),(N'Quận 10'),(N'Quận 11'),(N'Quận 12'),(N'Bình Thạnh'),(N'Phú Nhuận');
    
    DECLARE @Street NVARCHAR(100) = (SELECT TOP 1 street FROM @Streets ORDER BY NEWID());
    DECLARE @District NVARCHAR(50) = (SELECT TOP 1 district FROM @Districts ORDER BY NEWID());
    DECLARE @StreetNum INT = 1 + (ABS(CHECKSUM(NEWID())) % 999);
    DECLARE @Address NVARCHAR(200) = CAST(@StreetNum AS NVARCHAR) + N' ' + @Street + N', ' + @District + N', TP.HCM';
    
    INSERT INTO Patient (patient_name, date_of_birth, gender, phone, email, address)
    VALUES (@FullName, @DOB, @Gender, @Phone, @Email, @Address);
    
    SET @PatientCounter = @PatientCounter + 1;
END

PRINT '  - Created 100 patients with Vietnamese names and addresses'
GO

-- =============================================
-- SEED 2: INVENTORY ITEMS (30 items)
-- =============================================
PRINT 'Seed 2: Creating inventory items...'
GO

INSERT INTO Inventory (item_name, type, quantity, unit, supplier, low_stock_threshold, last_updated) VALUES
(N'Găng tay y tế', N'Vật tư tiêu hao', 5000, N'Chiếc', N'Công ty TNHH Thiết bị Y tế Việt', 1000, GETDATE()),
(N'Khẩu trang y tế', N'Vật tư tiêu hao', 10000, N'Chiếc', N'Công ty TNHH Thiết bị Y tế Việt', 2000, GETDATE()),
(N'Bông gạc vô trùng', N'Vật tư tiêu hao', 2000, N'Gói', N'Công ty TNHH Thiết bị Y tế Việt', 500, GETDATE()),
(N'Kim tiêm', N'Vật tư tiêu hao', 3000, N'Chiếc', N'Công ty TNHH Thiết bị Y tế Việt', 500, GETDATE()),
(N'Băng cá nhân', N'Vật tư tiêu hao', 1500, N'Cuộn', N'Công ty TNHH Thiết bị Y tế Việt', 300, GETDATE()),
(N'Composite A2', N'Vật liệu trám', 50, N'Ống', N'Công ty TNHH Vật liệu Nha khoa', 10, GETDATE()),
(N'Composite A3', N'Vật liệu trám', 45, N'Ống', N'Công ty TNHH Vật liệu Nha khoa', 10, GETDATE()),
(N'Amalgam', N'Vật liệu trám', 30, N'Hộp', N'Công ty TNHH Vật liệu Nha khoa', 5, GETDATE()),
(N'Xi măng nha khoa', N'Vật liệu', 40, N'Hộp', N'Công ty TNHH Vật liệu Nha khoa', 10, GETDATE()),
(N'Gutta-percha', N'Vật liệu điều trị tủy', 100, N'Hộp', N'Công ty TNHH Vật liệu Nha khoa', 20, GETDATE()),
(N'Kim nội nha', N'Dụng cụ', 200, N'Bộ', N'Công ty TNHH Dụng cụ Nha khoa', 50, GETDATE()),
(N'Mũi khoan kim cương', N'Dụng cụ', 150, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 30, GETDATE()),
(N'Mũi khoan carbide', N'Dụng cụ', 180, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 40, GETDATE()),
(N'Dây cung niềng răng', N'Vật liệu chỉnh nha', 80, N'Cuộn', N'Công ty TNHH Vật liệu Chỉnh nha', 20, GETDATE()),
(N'Mắc cài kim loại', N'Vật liệu chỉnh nha', 500, N'Chiếc', N'Công ty TNHH Vật liệu Chỉnh nha', 100, GETDATE()),
(N'Mắc cài sứ', N'Vật liệu chỉnh nha', 300, N'Chiếc', N'Công ty TNHH Vật liệu Chỉnh nha', 80, GETDATE()),
(N'Cao răng siêu âm - đầu tips', N'Dụng cụ', 50, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 15, GETDATE()),
(N'Đèn composite LED', N'Thiết bị', 5, N'Cái', N'Công ty TNHH Thiết bị Nha khoa', 2, GETDATE()),
(N'Gương nha khoa', N'Dụng cụ', 100, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 30, GETDATE()),
(N'Kẹp nha khoa', N'Dụng cụ', 80, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 25, GETDATE()),
(N'Hút nước bọt', N'Vật tư tiêu hao', 2000, N'Ống', N'Công ty TNHH Thiết bị Y tế Việt', 500, GETDATE()),
(N'Cốc giấy', N'Vật tư tiêu hao', 5000, N'Chiếc', N'Công ty TNHH Thiết bị Y tế Việt', 1000, GETDATE()),
(N'Khay nhựa dùng một lần', N'Vật tư tiêu hao', 3000, N'Chiếc', N'Công ty TNHH Thiết bị Y tế Việt', 800, GETDATE()),
(N'Nước súc miệng Fluoride', N'Dung dịch', 200, N'Chai', N'Công ty TNHH Dược phẩm', 50, GETDATE()),
(N'Gel tê tại chỗ', N'Thuốc', 80, N'Tuýp', N'Công ty TNHH Dược phẩm', 20, GETDATE()),
(N'Dung dịch sát khuẩn', N'Dung dịch', 150, N'Chai', N'Công ty TNHH Dược phẩm', 40, GETDATE()),
(N'Film X-quang răng', N'Vật tư chẩn đoán', 500, N'Tấm', N'Công ty TNHH Thiết bị Y tế', 100, GETDATE()),
(N'Tăm nước', N'Dụng cụ', 100, N'Hộp', N'Công ty TNHH Dụng cụ Nha khoa', 30, GETDATE()),
(N'Chỉ nha khoa', N'Dụng cụ', 300, N'Hộp', N'Công ty TNHH Dụng cụ Nha khoa', 80, GETDATE()),
(N'Bàn chải đánh răng', N'Dụng cụ', 200, N'Chiếc', N'Công ty TNHH Dụng cụ Nha khoa', 50, GETDATE());

PRINT '  - Created 30 inventory items'
GO

-- =============================================
-- SEED 3: APPOINTMENTS (2024-01 to 2026-01)
-- =============================================
PRINT 'Seed 3: Creating appointments from 2024-01 to 2026-01...'
GO

DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2026-01-31';
DECLARE @CurrentDate DATE = @StartDate;
DECLARE @AppointmentCount INT = 0;
DECLARE @DoctorIds TABLE (staff_id INT);

-- Get doctor IDs
INSERT INTO @DoctorIds
SELECT staff_id FROM Staff WHERE position = N'Bác sĩ';

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Skip Sundays
    IF DATEPART(WEEKDAY, @CurrentDate) != 1
    BEGIN
        DECLARE @TimeSlot INT = 0;
        
        -- Morning slots: 8:00, 8:30, 9:00, 9:30, 10:00, 10:30, 11:00, 11:30
        -- Afternoon slots: 13:00, 13:30, 14:00, 14:30, 15:00, 15:30, 16:00, 16:30
        WHILE @TimeSlot < 16
        BEGIN
            DECLARE @Hour INT = CASE WHEN @TimeSlot < 8 THEN 8 + (@TimeSlot / 2) ELSE 13 + ((@TimeSlot - 8) / 2) END;
            DECLARE @Minute INT = CASE WHEN @TimeSlot % 2 = 0 THEN 0 ELSE 30 END;
            DECLARE @AppointmentTime DATETIME = DATEADD(MINUTE, @Minute, DATEADD(HOUR, @Hour, @CurrentDate));
            
            -- Random 1-3 appointments per slot
            DECLARE @SlotsThisTime INT = 1 + (ABS(CHECKSUM(NEWID())) % 3);
            DECLARE @SlotCounter INT = 0;
            
            WHILE @SlotCounter < @SlotsThisTime
            BEGIN
                DECLARE @PatientId INT = 1 + (ABS(CHECKSUM(NEWID())) % 100);
                DECLARE @ServiceId INT = 1 + (ABS(CHECKSUM(NEWID())) % 25);
                DECLARE @DoctorId INT = (SELECT TOP 1 staff_id FROM @DoctorIds ORDER BY NEWID());
                
                -- Determine status based on date
                DECLARE @Status NVARCHAR(20);
                DECLARE @CheckInTime DATETIME = NULL;
                DECLARE @QueueNum INT = NULL;
                
                IF @AppointmentTime < GETDATE()
                BEGIN
                    -- Past appointments
                    DECLARE @StatusRand INT = ABS(CHECKSUM(NEWID())) % 100;
                    IF @StatusRand < 75
                    BEGIN
                        SET @Status = N'completed';
                        SET @CheckInTime = DATEADD(MINUTE, -(5 + (ABS(CHECKSUM(NEWID())) % 20)), @AppointmentTime);
                        SET @QueueNum = 1 + (@SlotCounter * 3) + (ABS(CHECKSUM(NEWID())) % 10);
                    END
                    ELSE IF @StatusRand < 85
                        SET @Status = N'cancelled';
                    ELSE IF @StatusRand < 92
                        SET @Status = N'no-show';
                    ELSE
                        SET @Status = N'rejected';
                END
                ELSE IF @AppointmentTime > DATEADD(DAY, 1, GETDATE())
                BEGIN
                    -- Future appointments
                    SET @Status = N'confirmed';
                END
                ELSE
                BEGIN
                    -- Today's appointments
                    SET @Status = N'pending';
                END
                
                -- Get patient info
                DECLARE @PatientName NVARCHAR(100), @PatientPhone NVARCHAR(20), @PatientEmail NVARCHAR(100);
                SELECT @PatientName = patient_name, @PatientPhone = phone, @PatientEmail = email
                FROM Patient WHERE patient_id = @PatientId;
                
                -- Random notes
                DECLARE @Notes NVARCHAR(MAX) = NULL;
                IF (ABS(CHECKSUM(NEWID())) % 5) = 0
                BEGIN
                    DECLARE @NotesList TABLE (note NVARCHAR(200));
                    INSERT INTO @NotesList VALUES
                    (N'Bệnh nhân đau răng hàm dưới bên phải'),(N'Cần khám tổng quát răng miệng'),
                    (N'Bệnh nhân muốn làm sạch cao răng'),(N'Răng bị sâu cần trám'),
                    (N'Kiểm tra lại sau khi điều trị tủy'),(N'Tư vấn niềng răng'),
                    (N'Nhổ răng khôn'),(N'Đau răng cấp'),(N'Kiểm tra định kỳ'),
                    (N'Lắp răng giả'),(N'Bọc răng sứ'),(N'Tẩy trắng răng');
                    
                    SET @Notes = (SELECT TOP 1 note FROM @NotesList ORDER BY NEWID());
                END
                
                INSERT INTO Appointment (
                    patient_name, phone, email, service_id, appointment_date,
                    status, notes, patient_id, assigned_doctor_id,
                    check_in_time, queue_number, appointment_code,
                    created_at, updated_at, is_deleted
                )
                VALUES (
                    @PatientName, @PatientPhone, @PatientEmail, @ServiceId, @AppointmentTime,
                    @Status, @Notes, @PatientId, @DoctorId,
                    @CheckInTime, @QueueNum, 
                    'APT' + RIGHT('00000' + CAST((@AppointmentCount + 1) AS VARCHAR), 5),
                    DATEADD(DAY, -7, @AppointmentTime), @AppointmentTime, 0
                );
                
                SET @AppointmentCount = @AppointmentCount + 1;
                SET @SlotCounter = @SlotCounter + 1;
            END
            
            SET @TimeSlot = @TimeSlot + 1;
        END
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT '  - Created ' + CAST(@AppointmentCount AS VARCHAR) + ' appointments'
GO

-- =============================================
-- SEED 4: MEDICAL RECORDS & PRESCRIPTIONS
-- =============================================
PRINT 'Seed 4: Creating medical records and prescriptions...'
GO

DECLARE @RecordCount INT = 0;
DECLARE @PrescriptionCount INT = 0;

DECLARE @CompletedAppointments TABLE (
    appointment_id INT,
    patient_id INT,
    assigned_doctor_id INT,
    appointment_date DATETIME,
    service_id INT
);

INSERT INTO @CompletedAppointments
SELECT appointment_id, patient_id, assigned_doctor_id, appointment_date, service_id
FROM Appointment
WHERE status = N'completed';

DECLARE @AppId INT, @PatId INT, @DocId INT, @AppDate DATETIME, @SvcId INT;

DECLARE appointment_cursor CURSOR FOR
SELECT appointment_id, patient_id, assigned_doctor_id, appointment_date, service_id
FROM @CompletedAppointments;

OPEN appointment_cursor;
FETCH NEXT FROM appointment_cursor INTO @AppId, @PatId, @DocId, @AppDate, @SvcId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Create medical record
    DECLARE @Diagnosis NVARCHAR(MAX);
    DECLARE @Treatment NVARCHAR(MAX);
    
    -- Vietnamese diagnoses
    DECLARE @DiagnosisList TABLE (diagnosis NVARCHAR(500));
    INSERT INTO @DiagnosisList VALUES
    (N'Sâu răng mô 6 hàm dưới bên phải, mức độ trung bình'),
    (N'Viêm nha chu mức độ nhẹ, chảy máu chân răng'),
    (N'Sâu răng số 7 hàm trên bên trái, sâu sâu đến gần tủy'),
    (N'Răng khôn mọc lệch, gây đau và viêm nướu'),
    (N'Tủy răng số 6 hàm dưới bị nhiễm trùng'),
    (N'Vôi răng nhiều, viêm nướu nhẹ'),
    (N'Răng cửa bị mẻ, cần hàn răng thẩm mỹ'),
    (N'Khớp cắn không đều, cần niềng răng'),
    (N'Răng đã điều trị tủy trước đó, cần bọc sứ bảo vệ'),
    (N'Mất răng số 6, cần cấy ghép implant'),
    (N'Răng nhiễm màu nặng, cần tẩy trắng'),
    (N'Nứt men răng, gây ê buốt'),
    (N'Viêm tủy cấp, đau nhiều'),
    (N'Áp xe quanh cuống răng'),
    (N'Răng lung lay do viêm nha chu nặng');
    
    SET @Diagnosis = (SELECT TOP 1 diagnosis FROM @DiagnosisList ORDER BY NEWID());
    
    -- Vietnamese treatments
    DECLARE @TreatmentList TABLE (treatment NVARCHAR(500));
    INSERT INTO @TreatmentList VALUES
    (N'Đã trám răng bằng composite màu A2, khuyến cáo vệ sinh răng miệng tốt'),
    (N'Lấy cao răng và đánh bóng, hướng dẫn đánh răng đúng cách'),
    (N'Điều trị tủy răng hoàn tất, hẹn lần sau trám răng'),
    (N'Nhổ răng khôn, kê đơn thuốc kháng sinh và giảm đau'),
    (N'Điều trị tủy ống tủy gần, trám tủy tạm'),
    (N'Lấy vôi và làm sạch túi lợi, tái khám sau 1 tháng'),
    (N'Hàn răng composite, khuyên tránh cắn vật cứng'),
    (N'Tư vấn phương án niềng răng, lên kế hoạch điều trị'),
    (N'Lấy dấu răng, hẹn lần sau thử răng sứ'),
    (N'Đánh giá xương hàm bằng phim CT, lên phương án implant'),
    (N'Tẩy trắng răng tại phòng khám, đạt màu A1'),
    (N'Trám composite phủ nứt, khuyên dùng kem đánh răng cho răng nhạy cảm'),
    (N'Mở tủy, dẫn lưu mủ, kê đơn thuốc'),
    (N'Dẫn lưu áp xe, đợi hết viêm sẽ điều trị tủy'),
    (N'Lấy cao răng sâu, nạo túi lợi, tư vấn chăm sóc nha chu');
    
    SET @Treatment = (SELECT TOP 1 treatment FROM @TreatmentList ORDER BY NEWID());
    
    INSERT INTO MedicalRecord (
        patient_id, appointment_id, doctor_id, record_date,
        diagnosis, treatment, notes
    )
    VALUES (
        @PatId, @AppId, @DocId, @AppDate,
        @Diagnosis, @Treatment, N'Bệnh nhân hợp tác tốt trong quá trình điều trị'
    );
    
    DECLARE @RecordId INT = SCOPE_IDENTITY();
    SET @RecordCount = @RecordCount + 1;
    
    -- 60% chance of having prescription
    IF (ABS(CHECKSUM(NEWID())) % 100) < 60
    BEGIN
        -- Random 1-3 medicines
        DECLARE @MedCount INT = 1 + (ABS(CHECKSUM(NEWID())) % 3);
        DECLARE @MedCounter INT = 0;
        
        WHILE @MedCounter < @MedCount
        BEGIN
            DECLARE @MedicineId INT = 1 + (ABS(CHECKSUM(NEWID())) % 50);
            DECLARE @Quantity INT = 5 + (ABS(CHECKSUM(NEWID())) % 20); -- 5-24 viên/gói
            
            -- Vietnamese dosage instructions
            DECLARE @DosageList TABLE (dosage NVARCHAR(200));
            INSERT INTO @DosageList VALUES
            (N'Uống 1 viên x 2 lần/ngày sau ăn'),
            (N'Uống 1 viên x 3 lần/ngày sau ăn'),
            (N'Uống 2 viên x 2 lần/ngày'),
            (N'Uống 1 viên khi đau'),
            (N'Bôi ngoài da vùng sưng 2-3 lần/ngày'),
            (N'Súc miệng 2-3 lần/ngày sau ăn'),
            (N'Uống 1 gói x 2 lần/ngày'),
            (N'Ngậm dưới lưỡi khi đau'),
            (N'Uống 1 viên trước khi đi ngủ'),
            (N'Uống 1 viên x 3 lần/ngày trước ăn 30 phút');
            
            DECLARE @Dosage NVARCHAR(200) = (SELECT TOP 1 dosage FROM @DosageList ORDER BY NEWID());
            
            INSERT INTO Prescription (
                record_id, medicine_id, quantity, dosage, notes
            )
            VALUES (
                @RecordId, @MedicineId, @Quantity, @Dosage,
                N'Dùng đủ liều, không tự ý ngừng thuốc'
            );
            
            SET @PrescriptionCount = @PrescriptionCount + 1;
            SET @MedCounter = @MedCounter + 1;
        END
    END
    
    FETCH NEXT FROM appointment_cursor INTO @AppId, @PatId, @DocId, @AppDate, @SvcId;
END

CLOSE appointment_cursor;
DEALLOCATE appointment_cursor;

PRINT '  - Created ' + CAST(@RecordCount AS VARCHAR) + ' medical records'
PRINT '  - Created ' + CAST(@PrescriptionCount AS VARCHAR) + ' prescriptions'
GO

-- =============================================
-- SEED 5: INVOICES (AUTO-GENERATE)
-- =============================================
PRINT 'Seed 5: Creating invoices from medical records...'
GO

DECLARE @InvoiceCount INT = 0;

DECLARE @Records TABLE (
    record_id INT,
    patient_id INT,
    appointment_id INT,
    doctor_id INT
);

INSERT INTO @Records
SELECT record_id, patient_id, appointment_id, doctor_id
FROM MedicalRecord;

DECLARE @RecId INT, @InvPatId INT, @InvAppId INT, @InvDocId INT;

DECLARE invoice_cursor CURSOR FOR
SELECT record_id, patient_id, appointment_id, doctor_id FROM @Records;

OPEN invoice_cursor;
FETCH NEXT FROM invoice_cursor INTO @RecId, @InvPatId, @InvAppId, @InvDocId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Create invoice
    INSERT INTO Invoice (
        patient_id, staff_id, invoice_date, total_amount,
        status, medical_record_id, created_at, is_deleted
    )
    VALUES (
        @InvPatId, @InvDocId, GETDATE(), 0,
        CASE WHEN (ABS(CHECKSUM(NEWID())) % 100) < 70 THEN N'paid' ELSE N'unpaid' END,
        @RecId, GETDATE(), 0
    );
    
    DECLARE @InvoiceId INT = SCOPE_IDENTITY();
    
    -- Add service from appointment
    DECLARE @AppSvcId INT, @AppSvcPrice DECIMAL(10,2);
    SELECT @AppSvcId = service_id FROM Appointment WHERE appointment_id = @InvAppId;
    SELECT @AppSvcPrice = price FROM Service WHERE service_id = @AppSvcId;
    
    IF @AppSvcId IS NOT NULL
    BEGIN
        INSERT INTO ServiceUsage (invoice_id, service_id, quantity, unit_price)
        VALUES (@InvoiceId, @AppSvcId, 1, @AppSvcPrice);
    END
    
    -- Add prescriptions
    DECLARE @TotalServiceAmount DECIMAL(10,2) = ISNULL(@AppSvcPrice, 0);
    DECLARE @TotalMedicineAmount DECIMAL(10,2) = 0;
    
    DECLARE @PrescIds TABLE (prescription_id INT);
    INSERT INTO @PrescIds
    SELECT prescription_id FROM Prescription WHERE record_id = @RecId;
    
    IF EXISTS (SELECT 1 FROM @PrescIds)
    BEGIN
        DECLARE @PrescId INT;
        DECLARE presc_cursor CURSOR FOR SELECT prescription_id FROM @PrescIds;
        
        OPEN presc_cursor;
        FETCH NEXT FROM presc_cursor INTO @PrescId;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO InvoicePrescription (invoice_id, prescription_id)
            VALUES (@InvoiceId, @PrescId);
            
            -- Calculate medicine cost
            DECLARE @MedPrice DECIMAL(10,2), @MedQty INT;
            SELECT @MedPrice = m.price, @MedQty = p.quantity
            FROM Prescription p
            INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
            WHERE p.prescription_id = @PrescId;
            
            SET @TotalMedicineAmount = @TotalMedicineAmount + (@MedPrice * @MedQty);
            
            FETCH NEXT FROM presc_cursor INTO @PrescId;
        END
        
        CLOSE presc_cursor;
        DEALLOCATE presc_cursor;
    END
    
    -- Update total amount
    UPDATE Invoice
    SET total_amount = @TotalServiceAmount + @TotalMedicineAmount
    WHERE invoice_id = @InvoiceId;
    
    SET @InvoiceCount = @InvoiceCount + 1;
    
    FETCH NEXT FROM invoice_cursor INTO @RecId, @InvPatId, @InvAppId, @InvDocId;
END

CLOSE invoice_cursor;
DEALLOCATE invoice_cursor;

PRINT '  - Created ' + CAST(@InvoiceCount AS VARCHAR) + ' invoices'
GO

-- =============================================
-- SEED 6: EMAIL LOGS (FOR PAST APPOINTMENTS)
-- =============================================
PRINT 'Seed 6: Creating email logs...'
GO

DECLARE @EmailCount INT = 0;

-- Confirmation emails for all past confirmed/completed appointments
INSERT INTO EmailLog (appointment_id, email_type, email_address, sent_at, status)
SELECT 
    appointment_id,
    N'confirmation',
    email,
    DATEADD(MINUTE, 5, created_at),
    N'sent'
FROM Appointment
WHERE status IN (N'confirmed', N'completed')
AND appointment_date < GETDATE();

SET @EmailCount = @@ROWCOUNT;

-- Reminder emails for appointments (24h before)
INSERT INTO EmailLog (appointment_id, email_type, email_address, sent_at, status)
SELECT 
    appointment_id,
    N'reminder',
    email,
    DATEADD(HOUR, -24, appointment_date),
    N'sent'
FROM Appointment
WHERE status IN (N'confirmed', N'completed')
AND appointment_date < GETDATE()
AND appointment_date > DATEADD(DAY, -60, GETDATE()); -- Last 60 days only

SET @EmailCount = @EmailCount + @@ROWCOUNT;

-- Cancellation emails
INSERT INTO EmailLog (appointment_id, email_type, email_address, sent_at, status)
SELECT 
    appointment_id,
    N'cancellation',
    email,
    updated_at,
    N'sent'
FROM Appointment
WHERE status = N'cancelled';

SET @EmailCount = @EmailCount + @@ROWCOUNT;

PRINT '  - Created ' + CAST(@EmailCount AS VARCHAR) + ' email logs'
GO

-- =============================================
-- FINAL SUMMARY
-- =============================================
PRINT ''
PRINT '=========================================='
PRINT 'DATABASE SEEDING COMPLETED'
PRINT '=========================================='
GO

SELECT 
    'Summary' AS [Report],
    (SELECT COUNT(*) FROM Patient) AS Patients,
    (SELECT COUNT(*) FROM Appointment) AS Appointments,
    (SELECT COUNT(*) FROM MedicalRecord) AS MedicalRecords,
    (SELECT COUNT(*) FROM Prescription) AS Prescriptions,
    (SELECT COUNT(*) FROM Invoice) AS Invoices,
    (SELECT COUNT(*) FROM ServiceUsage) AS ServiceUsages,
    (SELECT COUNT(*) FROM InvoicePrescription) AS InvoicePrescriptions,
    (SELECT COUNT(*) FROM EmailLog) AS EmailLogs,
    (SELECT COUNT(*) FROM Inventory) AS InventoryItems,
    (SELECT COUNT(*) FROM Service) AS Services,
    (SELECT COUNT(*) FROM Medicine) AS Medicines,
    (SELECT COUNT(*) FROM Staff) AS Staff;
GO

PRINT ''
PRINT '✅ Database is ready with 2-year historical data (2024-2026)'
PRINT '✅ All Vietnamese text properly stored with NVARCHAR'
PRINT ''

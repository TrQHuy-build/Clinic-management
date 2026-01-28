-- =============================================
-- SEED LARGE DATA: 2024-2026 (2 YEARS)
-- Purpose: Create realistic 2-year historical data
-- Encoding: NVARCHAR for Vietnamese text
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'LARGE DATA SEEDING: 2024-01 to 2026-01'
PRINT 'Start Time: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- =============================================
-- STEP 1: CLEAN EXISTING DATA
-- =============================================
PRINT 'Step 1: Cleaning existing patient data...'
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
DELETE FROM UserAccount WHERE role = 'patient';
DELETE FROM Inventory;

DBCC CHECKIDENT ('Patient', RESEED, 0);
DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('MedicalRecord', RESEED, 0);
DBCC CHECKIDENT ('Invoice', RESEED, 0);
DBCC CHECKIDENT ('UserAccount', RESEED, (SELECT ISNULL(MAX(user_id), 0) FROM UserAccount));
GO

PRINT '  ✓ Data cleaned successfully'
PRINT ''

-- =============================================
-- STEP 2: CREATE 100 PATIENTS
-- =============================================
PRINT 'Step 2: Creating 100 patient accounts...'
GO

DECLARE @i INT = 1;

WHILE @i <= 100
BEGIN
    -- Vietnamese names
    DECLARE @LastNames NVARCHAR(50) = (
        SELECT TOP 1 name FROM (VALUES 
            (N'Nguyễn'),(N'Trần'),(N'Lê'),(N'Phạm'),(N'Hoàng'),
            (N'Huỳnh'),(N'Phan'),(N'Vũ'),(N'Võ'),(N'Đặng'),
            (N'Bùi'),(N'Đỗ'),(N'Hồ'),(N'Ngô'),(N'Dương')
        ) AS T(name) ORDER BY NEWID()
    );
    
    DECLARE @FirstNames NVARCHAR(50) = (
        SELECT TOP 1 name FROM (VALUES
            (N'Văn An'),(N'Thị Bình'),(N'Minh Châu'),(N'Thị Dung'),
            (N'Văn Dũng'),(N'Thị Em'),(N'Văn Giang'),(N'Thị Hà'),
            (N'Văn Hải'),(N'Thị Hương'),(N'Văn Kiên'),(N'Thị Lan'),
            (N'Văn Long'),(N'Thị Mai'),(N'Văn Nam'),(N'Thị Nga'),
            (N'Văn Phong'),(N'Thị Quyên'),(N'Văn Sơn'),(N'Thị Thảo'),
            (N'Minh Anh'),(N'Hoàng Bảo'),(N'Quốc Cường'),(N'Hữu Đạt'),
            (N'Đức Duy'),(N'Gia Hân'),(N'Bảo Khang'),(N'Anh Khoa'),
            (N'Hồng Loan'),(N'Thanh Long'),(N'Bích Ngọc'),(N'Khánh Vy')
        ) AS T(name) ORDER BY NEWID()
    );
    
    DECLARE @FullName NVARCHAR(100) = @LastNames + N' ' + @FirstNames;
    DECLARE @Phone NVARCHAR(20) = '09' + RIGHT('00000000' + CAST(10000000 + @i * 12345 AS VARCHAR), 8);
    DECLARE @Email NVARCHAR(100) = 'patient' + CAST(@i AS VARCHAR) + '@gmail.com';
    DECLARE @PasswordHash NVARCHAR(255) = CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', 'Password123!'), 2);
    
    -- Create user account
    INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status, created_at)
    VALUES (@FullName, @Phone, @Email, @PasswordHash, 'patient', N'active', GETDATE());
    
    DECLARE @UserId INT = SCOPE_IDENTITY();
    
    -- Create patient profile
    DECLARE @Gender NVARCHAR(10) = CASE WHEN (@i % 2) = 0 THEN N'Male' ELSE N'Female' END;
    DECLARE @BirthYear INT = 1960 + (@i % 45);
    DECLARE @DOB DATE = CAST(CAST(@BirthYear AS VARCHAR) + '-' + 
                            RIGHT('0' + CAST(1 + (@i % 12) AS VARCHAR), 2) + '-' + 
                            RIGHT('0' + CAST(1 + (@i % 28) AS VARCHAR), 2) AS DATE);
    
    DECLARE @StreetNum INT = 10 + (@i * 7) % 990;
    DECLARE @Streets NVARCHAR(100) = (
        SELECT TOP 1 street FROM (VALUES
            (N'Lý Thường Kiệt'),(N'Trần Hưng Đạo'),(N'Nguyễn Huệ'),
            (N'Lê Lợi'),(N'Hai Bà Trưng'),(N'Nguyễn Trãi'),
            (N'Phan Đình Phùng'),(N'Điện Biên Phủ'),(N'Võ Văn Kiệt'),
            (N'Pasteur'),(N'Nam Kỳ Khởi Nghĩa'),(N'Lê Duẩn')
        ) AS T(street) ORDER BY NEWID()
    );
    
    DECLARE @Districts NVARCHAR(50) = (
        SELECT TOP 1 district FROM (VALUES
            (N'Quận 1'),(N'Quận 3'),(N'Quận 5'),(N'Quận 7'),
            (N'Quận 10'),(N'Bình Thạnh'),(N'Phú Nhuận'),(N'Gò Vấp')
        ) AS T(district) ORDER BY NEWID()
    );
    
    DECLARE @Address NVARCHAR(255) = CAST(@StreetNum AS NVARCHAR) + N' ' + @Streets + N', ' + @Districts + N', TP.HCM';
    DECLARE @Insurance NVARCHAR(100) = CASE WHEN (@i % 10) < 7 THEN N'BHYT - ' + @Phone ELSE NULL END;
    
    INSERT INTO Patient (user_id, date_of_birth, gender, address, insurance)
    VALUES (@UserId, @DOB, @Gender, @Address, @Insurance);
    
    SET @i = @i + 1;
    
    IF @i % 20 = 0
        PRINT '  - Created ' + CAST(@i AS VARCHAR) + ' patients...';
END

PRINT '  ✓ Created 100 patients successfully'
PRINT ''

-- =============================================
-- STEP 3: CREATE 30 INVENTORY ITEMS
-- =============================================
PRINT 'Step 3: Creating 30 inventory items...'
GO

INSERT INTO Inventory (item_name, type, quantity, unit, supplier) VALUES
(N'Găng tay y tế', N'Material', 5000, N'chiếc', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Khẩu trang y tế 3 lớp', N'Material', 10000, N'hộp', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Bông gạc vô trùng', N'Material', 2000, N'gói', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Kim tiêm vô trùng', N'Material', 3000, N'chiếc', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Băng cuộn y tế', N'Material', 1500, N'cuộn', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Composite A2 - Vật liệu trám răng', N'Material', 50, N'ống', N'Công ty TNHH Vật liệu Nha khoa Đại Việt'),
(N'Composite A3 - Vật liệu trám răng', N'Material', 45, N'ống', N'Công ty TNHH Vật liệu Nha khoa Đại Việt'),
(N'Amalgam - Hỗn hống trám răng', N'Material', 30, N'hộp', N'Công ty TNHH Vật liệu Nha khoa Đại Việt'),
(N'Xi măng nha khoa', N'Material', 40, N'hộp', N'Công ty TNHH Vật liệu Nha khoa Đại Việt'),
(N'Gutta-percha - Vật liệu điều trị tủy', N'Material', 100, N'hộp', N'Công ty TNHH Vật liệu Nha khoa Đại Việt'),
(N'Kim nội nha - Endodontic files', N'Equipment', 200, N'bộ', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Mũi khoan kim cương', N'Equipment', 150, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Mũi khoan Carbide', N'Equipment', 180, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Dây cung niềng răng Ni-Ti', N'Material', 80, N'cuộn', N'Công ty TNHH Vật liệu Chỉnh nha'),
(N'Mắc cài kim loại - Metal brackets', N'Material', 500, N'chiếc', N'Công ty TNHH Vật liệu Chỉnh nha'),
(N'Mắc cài sứ - Ceramic brackets', N'Material', 300, N'chiếc', N'Công ty TNHH Vật liệu Chỉnh nha'),
(N'Đầu tips lấy cao răng siêu âm', N'Equipment', 50, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Đèn composite LED', N'Equipment', 5, N'cái', N'Công ty TNHH Thiết bị Nha khoa Hitech'),
(N'Gương nha khoa inox', N'Equipment', 100, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Kẹp nha khoa inox', N'Equipment', 80, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Ống hút nước bọt dùng một lần', N'Material', 2000, N'hộp', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Cốc giấy dùng một lần', N'Material', 5000, N'chiếc', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Khay nhựa dùng một lần', N'Material', 3000, N'chiếc', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Nước súc miệng Fluoride', N'Material', 200, N'chai', N'Công ty TNHH Dược phẩm Hà Nội'),
(N'Gel tê tại chỗ Lidocaine', N'Material', 80, N'tuýp', N'Công ty TNHH Dược phẩm Hà Nội'),
(N'Dung dịch sát khuẩn Betadine', N'Material', 150, N'chai', N'Công ty TNHH Dược phẩm Hà Nội'),
(N'Film X-quang răng kỹ thuật số', N'Equipment', 500, N'tấm', N'Công ty TNHH Thiết bị Y tế Việt Nam'),
(N'Tăm nước - Water flosser', N'Equipment', 100, N'cái', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Chỉ nha khoa Oral-B', N'Material', 300, N'hộp', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt'),
(N'Bàn chải đánh răng cao cấp', N'Material', 200, N'chiếc', N'Công ty TNHH Dụng cụ Nha khoa Thành Đạt');

PRINT '  ✓ Created 30 inventory items'
PRINT ''

-- =============================================
-- STEP 4: CREATE APPOINTMENTS (2024-2026)
-- This will create ~20,000 appointments
-- =============================================
PRINT 'Step 4: Creating appointments (2024-01 to 2026-01)...'
PRINT '  This will take several minutes. Please wait...'
PRINT ''
GO

DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2026-01-31';
DECLARE @CurrentDate DATE = @StartDate;
DECLARE @TotalAppointments INT = 0;
DECLARE @BatchSize INT = 0;

-- Create temp table for doctors
DECLARE @Doctors TABLE (staff_id INT, doctor_name NVARCHAR(100));
INSERT INTO @Doctors
SELECT s.staff_id, u.fullname
FROM Staff s
INNER JOIN UserAccount u ON s.user_id = u.user_id
WHERE s.position = N'Bác sĩ';

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Skip Sundays
    IF DATEPART(WEEKDAY, @CurrentDate) != 1
    BEGIN
        -- Morning slots: 8:00, 8:30, 9:00, 9:30, 10:00, 10:30, 11:00, 11:30
        -- Afternoon slots: 13:00, 13:30, 14:00, 14:30, 15:00, 15:30, 16:00, 16:30
        DECLARE @TimeSlots TABLE (slot_hour INT, slot_minute INT);
        DELETE FROM @TimeSlots;
        INSERT INTO @TimeSlots VALUES
        (8,0),(8,30),(9,0),(9,30),(10,0),(10,30),(11,0),(11,30),
        (13,0),(13,30),(14,0),(14,30),(15,0),(15,30),(16,0),(16,30);
        
        DECLARE @SlotHour INT, @SlotMinute INT;
        
        DECLARE slot_cursor CURSOR LOCAL FAST_FORWARD FOR 
        SELECT slot_hour, slot_minute FROM @TimeSlots;
        
        OPEN slot_cursor;
        FETCH NEXT FROM slot_cursor INTO @SlotHour, @SlotMinute;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Create appointment time as DATETIME
            DECLARE @AppDateTime DATETIME = DATEADD(MINUTE, @SlotMinute, DATEADD(HOUR, @SlotHour, CAST(@CurrentDate AS DATETIME)));
            
            -- 1-3 appointments per slot (random)
            DECLARE @AppsInSlot INT = 1 + (ABS(CHECKSUM(NEWID())) % 3);
            DECLARE @SlotCount INT = 0;
            
            WHILE @SlotCount < @AppsInSlot
            BEGIN
                -- Random patient (1-100)
                DECLARE @PatientId INT = 1 + (ABS(CHECKSUM(NEWID())) % 100);
                
                -- Get patient info
                DECLARE @PatientName NVARCHAR(100), @PatientPhone NVARCHAR(20), @PatientEmail NVARCHAR(100);
                SELECT @PatientName = u.fullname, @PatientPhone = u.phone, @PatientEmail = u.email
                FROM Patient p
                INNER JOIN UserAccount u ON p.user_id = u.user_id
                WHERE p.patient_id = @PatientId;
                
                -- Random service (1-25)
                DECLARE @ServiceId INT = 1 + (ABS(CHECKSUM(NEWID())) % 25);
                
                -- Random doctor
                DECLARE @DoctorId INT = (SELECT TOP 1 staff_id FROM @Doctors ORDER BY NEWID());
                
                -- Determine status based on date
                DECLARE @Status NVARCHAR(20);
                DECLARE @CheckInTime DATETIME = NULL;
                DECLARE @QueueNum INT = NULL;
                DECLARE @AppCode NVARCHAR(50);
                
                IF @AppDateTime < GETDATE()
                BEGIN
                    -- Past appointments
                    DECLARE @Rand INT = ABS(CHECKSUM(NEWID())) % 100;
                    IF @Rand < 75
                    BEGIN
                        SET @Status = N'completed';
                        SET @CheckInTime = DATEADD(MINUTE, -(5 + (ABS(CHECKSUM(NEWID())) % 20)), @AppDateTime);
                        SET @QueueNum = 1 + (@TotalAppointments % 50);
                    END
                    ELSE IF @Rand < 90
                        SET @Status = N'cancelled';
                    ELSE
                        SET @Status = N'booked';
                END
                ELSE
                    SET @Status = N'booked';
                
                -- Random notes
                DECLARE @Notes NVARCHAR(255) = NULL;
                IF (ABS(CHECKSUM(NEWID())) % 5) = 0
                BEGIN
                    SET @Notes = (
                        SELECT TOP 1 note FROM (VALUES
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
                            (N'Lắp răng giả/Implant')
                        ) AS T(note) ORDER BY NEWID()
                    );
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
                
                DECLARE @NewAppId INT = SCOPE_IDENTITY();
                SET @AppCode = 'APT' + RIGHT('00000' + CAST(@NewAppId AS VARCHAR), 5);
                
                -- Update extra columns if they exist
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'patient_id')
                    UPDATE Appointment SET patient_id = @PatientId WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'assigned_doctor_id')
                    UPDATE Appointment SET assigned_doctor_id = @DoctorId WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'appointment_code')
                    UPDATE Appointment SET appointment_code = @AppCode WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'check_in_time')
                    UPDATE Appointment SET check_in_time = @CheckInTime WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'queue_number')
                    UPDATE Appointment SET queue_number = @QueueNum WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'created_at')
                    UPDATE Appointment SET created_at = DATEADD(DAY, -7, @AppDateTime) WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'updated_at')
                    UPDATE Appointment SET updated_at = @AppDateTime WHERE appointment_id = @NewAppId;
                    
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Appointment') AND name = 'is_deleted')
                    UPDATE Appointment SET is_deleted = 0 WHERE appointment_id = @NewAppId;
                
                SET @TotalAppointments = @TotalAppointments + 1;
                SET @BatchSize = @BatchSize + 1;
                SET @SlotCount = @SlotCount + 1;
            END
            
            FETCH NEXT FROM slot_cursor INTO @SlotHour, @SlotMinute;
        END
        
        CLOSE slot_cursor;
        DEALLOCATE slot_cursor;
    END
    
    -- Progress report every 30 days
    IF DAY(@CurrentDate) = 1 AND @BatchSize > 0
    BEGIN
        PRINT '  - ' + CONVERT(VARCHAR, @CurrentDate, 23) + ': ' + CAST(@BatchSize AS VARCHAR) + ' appointments (Total: ' + CAST(@TotalAppointments AS VARCHAR) + ')';
        SET @BatchSize = 0;
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT '  ✓ Created ' + CAST(@TotalAppointments AS VARCHAR) + ' appointments'
PRINT ''

-- =============================================
-- SUMMARY
-- =============================================
PRINT '=========================================='
PRINT 'DATA SEEDING COMPLETED'
PRINT 'End Time: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

SELECT 
    'SUMMARY' AS [Report],
    (SELECT COUNT(*) FROM UserAccount WHERE role = 'patient') AS PatientAccounts,
    (SELECT COUNT(*) FROM Patient) AS Patients,
    (SELECT COUNT(*) FROM Appointment) AS Appointments,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'completed') AS CompletedAppointments,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'booked') AS BookedAppointments,
    (SELECT COUNT(*) FROM Appointment WHERE status = N'cancelled') AS CancelledAppointments,
    (SELECT COUNT(*) FROM Inventory) AS InventoryItems,
    (SELECT COUNT(*) FROM Service) AS Services,
    (SELECT COUNT(*) FROM Medicine) AS Medicines;
GO

PRINT ''
PRINT '✅ All data seeded successfully with Vietnamese NVARCHAR encoding'
PRINT '✅ Data range: January 2024 to January 2026 (2 years)'
PRINT '✅ Ready for Medical Records, Prescriptions, and Invoices generation'
PRINT ''

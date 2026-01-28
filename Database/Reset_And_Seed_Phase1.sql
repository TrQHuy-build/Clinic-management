-- =============================================
-- RESET & SEED DATABASE WITH CORRECT SCHEMA
-- Date: January 28, 2026
-- Data Range: 2024-01 to 2026-01
-- =============================================

USE DentalClinicDB;
GO

SET NOCOUNT ON;
GO

PRINT '=========================================='
PRINT 'STARTING DATABASE RESET'
PRINT '=========================================='
PRINT ''

-- =============================================
-- STEP 1: DELETE DATA (Keep UserAccount, Staff, Service, Medicine)
-- =============================================
PRINT 'Step 1: Deleting existing data...'
GO

DELETE FROM InvoicePrescription;
DELETE FROM ServiceUsage;
DELETE FROM InventoryTransaction;
DELETE FROM EmailLog WHERE appointment_id IS NOT NULL;
DELETE FROM Prescription;
DELETE FROM Invoice;
DELETE FROM MedicalRecord;
DELETE FROM Appointment;
DELETE FROM Patient;
DELETE FROM Inventory;
DELETE FROM Shift;
DELETE FROM Salary;
DELETE FROM PasswordReset WHERE user_id IN (SELECT user_id FROM UserAccount WHERE role = 'patient');
DELETE FROM UserAccount WHERE role = 'patient';

PRINT '  - Cleaned all patient and transactional data'
GO

-- Reset identity
DBCC CHECKIDENT ('Patient', RESEED, 0);
DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('MedicalRecord', RESEED, 0);
DBCC CHECKIDENT ('Invoice', RESEED, 0);
DBCC CHECKIDENT ('Prescription', RESEED, 0);
DBCC CHECKIDENT ('EmailLog', RESEED, 0);
DBCC CHECKIDENT ('Inventory', RESEED, 0);
DBCC CHECKIDENT ('InventoryTransaction', RESEED, 0);
DBCC CHECKIDENT ('ServiceUsage', RESEED, 0);
DBCC CHECKIDENT ('InvoicePrescription', RESEED, 0);
DBCC CHECKIDENT ('UserAccount', RESEED, (SELECT MAX(user_id) FROM UserAccount));
GO

PRINT ''
PRINT '=========================================='
PRINT 'STARTING DATA SEEDING (2024-2026)'
PRINT '=========================================='
PRINT ''

-- =============================================
-- SEED 1: PATIENT USER ACCOUNTS (100)
-- =============================================
PRINT 'Seed 1: Creating 100 patient accounts...'
GO

DECLARE @Counter INT = 1;

WHILE @Counter <= 100
BEGIN
    DECLARE @FirstNames TABLE (name NVARCHAR(50));
    INSERT INTO @FirstNames VALUES 
    (N'Nguyễn'),(N'Trần'),(N'Lê'),(N'Phạm'),(N'Hoàng'),(N'Huỳnh'),(N'Phan'),(N'Vũ'),(N'Võ'),(N'Đặng'),
    (N'Bùi'),(N'Đỗ'),(N'Hồ'),(N'Ngô'),(N'Dương'),(N'Lý'),(N'Mai'),(N'Đinh'),(N'Tô'),(N'Trương');

    DECLARE @GivenNames TABLE (name NVARCHAR(50));
    INSERT INTO @GivenNames VALUES
    (N'Văn An'),(N'Thị Bình'),(N'Minh Châu'),(N'Thị Dung'),(N'Văn Dũng'),(N'Thị Em'),(N'Văn Giang'),
    (N'Thị Hà'),(N'Văn Hải'),(N'Thị Hương'),(N'Văn Kiên'),(N'Thị Lan'),(N'Văn Long'),(N'Thị Mai'),
    (N'Văn Nam'),(N'Thị Nga'),(N'Văn Phong'),(N'Thị Quyên'),(N'Văn Sơn'),(N'Thị Thảo'),(N'Văn Thắng'),
    (N'Thị Thu'),(N'Văn Tuấn'),(N'Thị Tuyết'),(N'Văn Vũ'),(N'Thị Xuân'),(N'Minh Anh'),(N'Hoàng Bảo'),
    (N'Thanh Bình'),(N'Quốc Cường'),(N'Hữu Đạt'),(N'Đức Duy'),(N'Gia Hân'),(N'Bảo Hân'),(N'Khánh Hòa'),
    (N'Gia Hưng'),(N'Bảo Khang'),(N'Anh Khoa'),(N'Minh Khuê'),(N'Hồng Loan'),(N'Thanh Long'),(N'Trúc Mai'),
    (N'Quỳnh My'),(N'Phương Nam'),(N'Hải Nam'),(N'Hà Nhi'),(N'Bích Ngọc'),(N'Ngọc Nhi'),(N'Thanh Nhàn'),
    (N'Phương Oanh'),(N'Thu Phương'),(N'Quang Phúc'),(N'Bảo Quân'),(N'Khánh Quyên'),(N'Thanh Sang'),
    (N'Quang Sơn'),(N'Minh Tâm'),(N'Hoài Thương'),(N'Minh Thư'),(N'Hồng Thy'),(N'Anh Thư'),(N'Bảo Trâm'),
    (N'Gia Trí'),(N'Thanh Trúc'),(N'Phương Trang'),(N'Minh Triết'),(N'Bích Trâm'),(N'Đức Trung'),
    (N'Tuấn Tú'),(N'Hồng Vân'),(N'Khánh Vy'),(N'Hữu Vinh'),(N'Thanh Xuân'),(N'Quang Huy'),(N'Đình Huy'),
    (N'Minh Hiếu'),(N'Hoàng Anh'),(N'Tuấn Kiệt'),(N'Minh Khang'),(N'Hoàng Long'),(N'Hữu Lộc'),
    (N'Thanh Lương'),(N'Minh Nhật'),(N'Bảo Phúc'),(N'Gia Phúc'),(N'Quốc Thắng'),(N'Hữu Thịnh'),
    (N'Minh Tiến'),(N'Đức Trí'),(N'Quốc Tuấn'),(N'Hoàng Vũ'),(N'Anh Vũ'),(N'Minh Quân'),(N'Đức Anh');

    DECLARE @RandFirst NVARCHAR(50) = (SELECT TOP 1 name FROM @FirstNames ORDER BY NEWID());
    DECLARE @RandGiven NVARCHAR(50) = (SELECT TOP 1 name FROM @GivenNames ORDER BY NEWID());
    DECLARE @FullName NVARCHAR(100) = @RandFirst + N' ' + @RandGiven;
    DECLARE @Phone NVARCHAR(20) = '09' + RIGHT('00000000' + CAST(ABS(CHECKSUM(NEWID())) % 100000000 AS VARCHAR), 8);
    DECLARE @Email NVARCHAR(100) = LOWER(REPLACE(REPLACE(@RandGiven, N' ', ''), N'ă', 'a')) + CAST(@Counter AS VARCHAR) + '@gmail.com';
    DECLARE @PasswordHash NVARCHAR(255) = 'hashed_password_' + CAST(@Counter AS VARCHAR);
    
    -- Create user account
    INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status, created_at)
    VALUES (@FullName, @Phone, @Email, @PasswordHash, 'patient', N'active', GETDATE());
    
    DECLARE @UserId INT = SCOPE_IDENTITY();
    
    -- Create patient profile
    DECLARE @Gender NVARCHAR(10) = CASE WHEN (@Counter % 2) = 0 THEN N'Male' ELSE N'Female' END;
    DECLARE @BirthYear INT = 1960 + (ABS(CHECKSUM(NEWID())) % 45);
    DECLARE @BirthMonth INT = 1 + (ABS(CHECKSUM(NEWID())) % 12);
    DECLARE @BirthDay INT = 1 + (ABS(CHECKSUM(NEWID())) % 28);
    DECLARE @DOB DATE = CAST(CAST(@BirthYear AS VARCHAR) + '-' + 
                              RIGHT('0' + CAST(@BirthMonth AS VARCHAR), 2) + '-' + 
                              RIGHT('0' + CAST(@BirthDay AS VARCHAR), 2) AS DATE);
    
    DECLARE @Streets TABLE (street NVARCHAR(100));
    INSERT INTO @Streets VALUES 
    (N'Lý Thường Kiệt'),(N'Trần Hưng Đạo'),(N'Nguyễn Huệ'),(N'Lê Lợi'),(N'Hai Bà Trưng'),
    (N'Nguyễn Trãi'),(N'Phan Đình Phùng'),(N'Hoàng Diệu'),(N'Điện Biên Phủ'),(N'Cách Mạng Tháng 8'),
    (N'Lê Duẩn'),(N'Võ Văn Kiệt'),(N'Pasteur'),(N'Nam Kỳ Khởi Nghĩa'),(N'Nguyễn Đình Chiểu'),
    (N'Lê Văn Sỹ'),(N'Trường Chinh'),(N'Xô Viết Nghệ Tĩnh'),(N'Phan Văn Trị'),(N'Hoàng Hoa Thám');
    
    DECLARE @Districts TABLE (district NVARCHAR(50));
    INSERT INTO @Districts VALUES
    (N'Quận 1'),(N'Quận 2'),(N'Quận 3'),(N'Quận 4'),(N'Quận 5'),(N'Quận 6'),(N'Quận 7'),
    (N'Quận 8'),(N'Quận 9'),(N'Quận 10'),(N'Quận 11'),(N'Quận 12'),(N'Bình Thạnh'),
    (N'Phú Nhuận'),(N'Gò Vấp'),(N'Tân Bình'),(N'Tân Phú'),(N'Bình Tân');
    
    DECLARE @Street NVARCHAR(100) = (SELECT TOP 1 street FROM @Streets ORDER BY NEWID());
    DECLARE @District NVARCHAR(50) = (SELECT TOP 1 district FROM @Districts ORDER BY NEWID());
    DECLARE @StreetNum INT = 1 + (ABS(CHECKSUM(NEWID())) % 999);
    DECLARE @Address NVARCHAR(255) = CAST(@StreetNum AS NVARCHAR) + N' ' + @Street + N', ' + @District + N', TP. Hồ Chí Minh';
    
    DECLARE @Insurance NVARCHAR(100) = CASE 
        WHEN (ABS(CHECKSUM(NEWID())) % 10) < 7 THEN N'BHYT Việt Nam - ' + @Phone
        ELSE NULL
    END;
    
    INSERT INTO Patient (user_id, date_of_birth, gender, address, insurance)
    VALUES (@UserId, @DOB, @Gender, @Address, @Insurance);
    
    SET @Counter = @Counter + 1;
END

PRINT '  - Created 100 patients with user accounts'
GO

-- =============================================
-- SEED 2: INVENTORY (30 items)
-- =============================================
PRINT 'Seed 2: Creating 30 inventory items...'
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

PRINT '  - Created 30 inventory items'
GO

-- =============================================
-- SEED 3: APPOINTMENTS (2024-2026)
-- =============================================
PRINT 'Seed 3: Creating appointments (2024-01 to 2026-01)...'
PRINT '  This may take a few minutes...'
GO

DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2026-01-31';
DECLARE @CurrentDate DATE = @StartDate;
DECLARE @AppCount INT = 0;

-- Get staff IDs (doctors)
DECLARE @DoctorIds TABLE (staff_id INT);
INSERT INTO @DoctorIds
SELECT staff_id FROM Staff WHERE position = N'Bác sĩ' OR position = N'Doctor';

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Skip Sundays (DATEPART WEEKDAY: 1=Sunday, 2=Monday, etc.)
    IF DATEPART(WEEKDAY, @CurrentDate) != 1
    BEGIN
        -- Time slots: 8:00-11:30 morning, 13:00-16:30 afternoon (30 min intervals)
        DECLARE @TimeSlots TABLE (slot_hour INT, slot_minute INT);
        INSERT INTO @TimeSlots VALUES
        (8,0),(8,30),(9,0),(9,30),(10,0),(10,30),(11,0),(11,30),
        (13,0),(13,30),(14,0),(14,30),(15,0),(15,30),(16,0),(16,30);
        
        DECLARE @SlotHour INT, @SlotMinute INT;
        
        DECLARE slot_cursor CURSOR FOR SELECT slot_hour, slot_minute FROM @TimeSlots;
        OPEN slot_cursor;
        FETCH NEXT FROM slot_cursor INTO @SlotHour, @SlotMinute;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @AppTime DATETIME = DATEADD(MINUTE, @SlotMinute, DATEADD(HOUR, @SlotHour, @CurrentDate));
            
            -- 1-3 appointments per slot
            DECLARE @AppsInSlot INT = 1 + (ABS(CHECKSUM(NEWID())) % 3);
            DECLARE @SlotCount INT = 0;
            
            WHILE @SlotCount < @AppsInSlot
            BEGIN
                -- Random patient (1-100)
                DECLARE @RandPatientId INT = 1 + (ABS(CHECKSUM(NEWID())) % 100);
                
                -- Get patient info from UserAccount via Patient
                DECLARE @PatientUserId INT, @PatientName NVARCHAR(100), @PatientPhone NVARCHAR(20), @PatientEmail NVARCHAR(100);
                SELECT @PatientUserId = user_id FROM Patient WHERE patient_id = @RandPatientId;
                SELECT @PatientName = fullname, @PatientPhone = phone, @PatientEmail = email
                FROM UserAccount WHERE user_id = @PatientUserId;
                
                -- Random service (1-25)
                DECLARE @RandServiceId INT = 1 + (ABS(CHECKSUM(NEWID())) % 25);
                
                -- Determine status based on date
                DECLARE @AppStatus NVARCHAR(20);
                IF @AppTime < GETDATE()
                BEGIN
                    DECLARE @Rand INT = ABS(CHECKSUM(NEWID())) % 100;
                    IF @Rand < 80
                        SET @AppStatus = N'completed';
                    ELSE IF @Rand < 90
                        SET @AppStatus = N'cancelled';
                    ELSE
                        SET @AppStatus = N'booked';
                END
                ELSE
                    SET @AppStatus = N'booked';
                
                -- Random notes
                DECLARE @AppNotes NVARCHAR(255) = NULL;
                IF (ABS(CHECKSUM(NEWID())) % 5) = 0
                BEGIN
                    DECLARE @NoteOptions TABLE (note NVARCHAR(255));
                    INSERT INTO @NoteOptions VALUES
                    (N'Bệnh nhân đau răng hàm dưới bên phải'),
                    (N'Cần khám tổng quát và tư vấn'),
                    (N'Yêu cầu làm sạch cao răng'),
                    (N'Răng bị sâu, cần kiểm tra'),
                    (N'Tái khám sau điều trị tủy'),
                    (N'Tư vấn niềng răng chỉnh nha'),
                    (N'Nhổ răng khôn bị mọc lệch'),
                    (N'Đau răng cấp, cần khám gấp'),
                    (N'Kiểm tra sức khỏe răng miệng định kỳ'),
                    (N'Tư vấn làm răng sứ thẩm mỹ'),
                    (N'Tẩy trắng răng'),
                    (N'Lắp răng giả/Implant');
                    
                    SET @AppNotes = (SELECT TOP 1 note FROM @NoteOptions ORDER BY NEWID());
                END
                
                -- Insert appointment
                INSERT INTO Appointment (
                    patient_name, phone, email, service_id, 
                    appointment_date, status, notes
                )
                VALUES (
                    @PatientName, @PatientPhone, @PatientEmail, @RandServiceId,
                    @AppTime, @AppStatus, @AppNotes
                );
                
                -- Link to patient_id and doctor via extra columns if they exist
                DECLARE @AppId INT = SCOPE_IDENTITY();
                
                -- Update patient_id if column exists
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'patient_id')
                BEGIN
                    EXEC('UPDATE Appointment SET patient_id = ' + @RandPatientId + ' WHERE appointment_id = ' + @AppId);
                END
                
                -- Update assigned_doctor_id if column exists
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'assigned_doctor_id')
                BEGIN
                    DECLARE @RandDoctorId INT = (SELECT TOP 1 staff_id FROM @DoctorIds ORDER BY NEWID());
                    EXEC('UPDATE Appointment SET assigned_doctor_id = ' + @RandDoctorId + ' WHERE appointment_id = ' + @AppId);
                END
                
                -- Update appointment_code if column exists
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'Appointment' AND COLUMN_NAME = 'appointment_code')
                BEGIN
                    DECLARE @AppCode NVARCHAR(50) = 'APT' + RIGHT('00000' + CAST(@AppId AS VARCHAR), 5);
                    EXEC('UPDATE Appointment SET appointment_code = ''' + @AppCode + ''' WHERE appointment_id = ' + @AppId);
                END
                
                SET @AppCount = @AppCount + 1;
                SET @SlotCount = @SlotCount + 1;
            END
            
            FETCH NEXT FROM slot_cursor INTO @SlotHour, @SlotMinute;
        END
        
        CLOSE slot_cursor;
        DEALLOCATE slot_cursor;
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    
    -- Progress indicator every 30 days
    IF DATEDIFF(DAY, @StartDate, @CurrentDate) % 30 = 0
    BEGIN
        PRINT '  - Progress: ' + CONVERT(VARCHAR, @CurrentDate, 23) + ' (' + CAST(@AppCount AS VARCHAR) + ' appointments)'
    END
END

PRINT '  - Created ' + CAST(@AppCount AS VARCHAR) + ' appointments total'
GO

PRINT ''
PRINT 'Database seeding completed successfully!'
PRINT 'Run a SELECT COUNT query to verify data...'
GO

-- Summary
SELECT 
    'SUMMARY' AS [Status],
    (SELECT COUNT(*) FROM UserAccount WHERE role = 'patient') AS PatientAccounts,
    (SELECT COUNT(*) FROM Patient) AS Patients,
    (SELECT COUNT(*) FROM Appointment) AS Appointments,
    (SELECT COUNT(*) FROM Inventory) AS InventoryItems,
    (SELECT COUNT(*) FROM Service) AS Services,
    (SELECT COUNT(*) FROM Medicine) AS Medicines,
    (SELECT COUNT(*) FROM Staff) AS Staff;
GO

PRINT ''
PRINT '✅ Data seeded from 2024-01 to 2026-01'
PRINT '✅ All Vietnamese text stored with NVARCHAR'
PRINT '✅ Ready to continue with Medical Records and Invoices'
GO

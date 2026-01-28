-- ============================================
-- RESET & SEED DATABASE WITH REALISTIC DATA
-- Period: 2024-01-01 to 2026-01-28
-- Dental Clinic Management System
-- ============================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'STARTING DATABASE RESET & SEED';
PRINT 'Period: 2024-01-01 to 2026-01-28';
PRINT '========================================';
GO

-- ============================================
-- STEP 1: DISABLE FOREIGN KEY CONSTRAINTS
-- ============================================
PRINT 'Disabling foreign key constraints...';

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- ============================================
-- STEP 2: CLEAR EXISTING DATA (Keep structure)
-- ============================================
PRINT 'Clearing existing data...';

-- Transaction tables (clear first due to FKs)
DELETE FROM InvoicePrescription;
DELETE FROM ServiceUsage;
DELETE FROM Prescription;
DELETE FROM Invoice;
DELETE FROM MedicalRecord;
DELETE FROM Appointment;
DELETE FROM InventoryTransaction;
DELETE FROM Shift;
DELETE FROM Salary;
DELETE FROM AuditLog;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailLog')
    DELETE FROM EmailLog;

-- Master data (keep some, regenerate others)
DELETE FROM Inventory;
DELETE FROM Medicine;
DELETE FROM Service;

-- Users (will regenerate patients, keep staff)
DELETE FROM Patient;
-- Don't delete Staff and UserAccount yet

PRINT '  + Cleared all transaction data';
GO

-- ============================================
-- STEP 3: RESET IDENTITY SEEDS
-- ============================================
PRINT 'Resetting identity seeds...';

DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('MedicalRecord', RESEED, 0);
DBCC CHECKIDENT ('Prescription', RESEED, 0);
DBCC CHECKIDENT ('Invoice', RESEED, 0);
DBCC CHECKIDENT ('ServiceUsage', RESEED, 0);
DBCC CHECKIDENT ('InvoicePrescription', RESEED, 0);
DBCC CHECKIDENT ('Patient', RESEED, 0);
DBCC CHECKIDENT ('Medicine', RESEED, 0);
DBCC CHECKIDENT ('Service', RESEED, 0);
DBCC CHECKIDENT ('Inventory', RESEED, 0);
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailLog')
    DBCC CHECKIDENT ('EmailLog', RESEED, 0);

PRINT '  + Reset all identity seeds';
GO

-- ============================================
-- STEP 4: SEED SERVICES (20 services)
-- ============================================
PRINT 'Seeding services...';

SET IDENTITY_INSERT Service ON;

INSERT INTO Service (service_id, service_name, description, price, created_at, is_deleted) VALUES
(1, N'Khám tổng quát', N'Khám và tư vấn sức khỏe răng miệng', 100000, '2024-01-01', 0),
(2, N'Lấy cao răng', N'Vệ sinh răng miệng, lấy cao răng', 200000, '2024-01-01', 0),
(3, N'Trám răng Composite', N'Trám răng sâu bằng vật liệu Composite', 300000, '2024-01-01', 0),
(4, N'Trám răng Amalgam', N'Trám răng sâu bằng Amalgam', 250000, '2024-01-01', 0),
(5, N'Nhổ răng thường', N'Nhổ răng sữa hoặc răng vĩnh viễn đơn giản', 150000, '2024-01-01', 0),
(6, N'Nhổ răng khôn', N'Nhổ răng khôn mọc lệch, mọc ngầm', 800000, '2024-01-01', 0),
(7, N'Điều trị tủy', N'Điều trị tủy răng, chữa tủy', 500000, '2024-01-01', 0),
(8, N'Bọc răng sứ', N'Bọc răng sứ kim loại hoặc toàn sứ', 2000000, '2024-01-01', 0),
(9, N'Cắm implant', N'Cấy ghép implant răng', 15000000, '2024-01-01', 0),
(10, N'Niềng răng mắc cài kim loại', N'Chỉnh nha bằng mắc cài kim loại', 25000000, '2024-01-01', 0),
(11, N'Niềng răng mắc cài sứ', N'Chỉnh nha bằng mắc cài sứ thẩm mỹ', 35000000, '2024-01-01', 0),
(12, N'Niềng răng invisalign', N'Chỉnh nha bằng khay trong suốt', 80000000, '2024-01-01', 0),
(13, N'Tẩy trắng răng', N'Tẩy trắng răng công nghệ cao', 3000000, '2024-01-01', 0),
(14, N'Làm cầu răng', N'Làm cầu răng cố định', 5000000, '2024-01-01', 0),
(15, N'Hàm giả tháo lắp', N'Làm hàm giả có thể tháo lắp', 8000000, '2024-01-01', 0),
(16, N'X-quang răng', N'Chụp X-quang răng', 150000, '2024-01-01', 0),
(17, N'Phẫu thuật nha chu', N'Phẫu thuật điều trị bệnh nha chu', 2500000, '2024-01-01', 0),
(18, N'Cắt lợi thẩm mỹ', N'Phẫu thuật cắt lợi thẩm mỹ', 3000000, '2024-01-01', 0),
(19, N'Làm răng giả toàn hàm', N'Làm răng giả cố định toàn hàm', 120000000, '2024-01-01', 0),
(20, N'Điều chỉnh khớp cắn', N'Điều chỉnh khớp cắn, chức năng nhai', 5000000, '2024-01-01', 0);

SET IDENTITY_INSERT Service OFF;

PRINT '  + Created 20 services';
GO

-- ============================================
-- STEP 5: SEED MEDICINES (50 medicines)
-- ============================================
PRINT 'Seeding medicines...';

SET IDENTITY_INSERT Medicine ON;

INSERT INTO Medicine (medicine_id, medicine_name, unit, manufacturer, price, stock_quantity, expiry_date, created_at, is_deleted) VALUES
-- Pain killers
(1, N'Paracetamol 500mg', N'Viên', N'Traphaco', 2000, 500, '2027-12-31', '2024-01-01', 0),
(2, N'Ibuprofen 400mg', N'Viên', N'Pymepharco', 3000, 400, '2027-12-31', '2024-01-01', 0),
(3, N'Ketoprofen 100mg', N'Viên', N'Boston', 5000, 300, '2027-12-31', '2024-01-01', 0),
(4, N'Tramadol 50mg', N'Viên', N'Sandoz', 10000, 200, '2027-12-31', '2024-01-01', 0),
-- Antibiotics
(5, N'Amoxicillin 500mg', N'Viên', N'Imexpharm', 3500, 600, '2027-12-31', '2024-01-01', 0),
(6, N'Augmentin 625mg', N'Viên', N'GSK', 8000, 400, '2027-12-31', '2024-01-01', 0),
(7, N'Metronidazole 500mg', N'Viên', N'Domesco', 2500, 500, '2027-12-31', '2024-01-01', 0),
(8, N'Ciprofloxacin 500mg', N'Viên', N'Stada', 4000, 350, '2027-12-31', '2024-01-01', 0),
(9, N'Azithromycin 250mg', N'Viên', N'Pfizer', 12000, 300, '2027-12-31', '2024-01-01', 0),
(10, N'Clindamycin 300mg', N'Viên', N'Pfizer', 15000, 250, '2027-12-31', '2024-01-01', 0),
-- Anti-inflammatory
(11, N'Prednisolone 5mg', N'Viên', N'Boston', 1500, 400, '2027-12-31', '2024-01-01', 0),
(12, N'Dexamethasone 0.5mg', N'Viên', N'Vinphaco', 1000, 500, '2027-12-31', '2024-01-01', 0),
-- Antiseptic mouthwash
(13, N'Chlorhexidine 0.2%', N'Chai', N'Colgate', 50000, 200, '2027-12-31', '2024-01-01', 0),
(14, N'Betadine mouthwash', N'Chai', N'Mundipharma', 80000, 150, '2027-12-31', '2024-01-01', 0),
(15, N'Listerine mouthwash', N'Chai', N'J&J', 120000, 180, '2027-12-31', '2024-01-01', 0),
-- Local anesthetics
(16, N'Lidocaine 2% (Xylocaine)', N'Ống', N'AstraZeneca', 25000, 300, '2027-12-31', '2024-01-01', 0),
(17, N'Articaine 4%', N'Ống', N'Septodont', 45000, 250, '2027-12-31', '2024-01-01', 0),
(18, N'Mepivacaine 3%', N'Ống', N'Dentsply', 40000, 200, '2027-12-31', '2024-01-01', 0),
-- Vitamin & supplements
(19, N'Vitamin C 500mg', N'Viên', N'DHG Pharma', 5000, 400, '2027-12-31', '2024-01-01', 0),
(20, N'Calcium + D3', N'Viên', N'Zentiva', 8000, 350, '2027-12-31', '2024-01-01', 0),
-- Hemostatic agents
(21, N'Gelfoam (Spongostan)', N'Miếng', N'Ethicon', 35000, 150, '2027-12-31', '2024-01-01', 0),
(22, N'Surgicel', N'Miếng', N'Ethicon', 50000, 120, '2027-12-31', '2024-01-01', 0),
-- Dental materials
(23, N'Composite resin A2', N'Gram', N'3M ESPE', 200000, 100, '2027-12-31', '2024-01-01', 0),
(24, N'Composite resin A3', N'Gram', N'3M ESPE', 200000, 100, '2027-12-31', '2024-01-01', 0),
(25, N'Glass ionomer cement', N'Gram', N'GC', 150000, 80, '2027-12-31', '2024-01-01', 0),
(26, N'Zinc oxide eugenol', N'Set', N'Dentsply', 80000, 100, '2027-12-31', '2024-01-01', 0),
(27, N'Calcium hydroxide', N'Gram', N'Dentsply', 120000, 90, '2027-12-31', '2024-01-01', 0),
(28, N'Bonding agent', N'Chai', N'3M ESPE', 500000, 50, '2027-12-31', '2024-01-01', 0),
(29, N'Etching gel 37%', N'Chai', N'Kerr', 180000, 80, '2027-12-31', '2024-01-01', 0),
(30, N'Impression material (Alginate)', N'Gói', N'Zhermack', 250000, 100, '2027-12-31', '2024-01-01', 0),
-- Endodontic materials
(31, N'Gutta percha points', N'Hộp', N'Dentsply', 300000, 60, '2027-12-31', '2024-01-01', 0),
(32, N'Endodontic sealer', N'Tube', N'Dentsply', 450000, 50, '2027-12-31', '2024-01-01', 0),
(33, N'EDTA solution 17%', N'Chai', N'Vista Dental', 180000, 70, '2027-12-31', '2024-01-01', 0),
(34, N'Sodium hypochlorite 5.25%', N'Chai', N'Vista Dental', 120000, 100, '2027-12-31', '2024-01-01', 0),
-- Orthodontic supplies
(35, N'Orthodontic brackets (metal)', N'Set', N'Ormco', 1500000, 30, '2027-12-31', '2024-01-01', 0),
(36, N'Orthodontic brackets (ceramic)', N'Set', N'Ormco', 2500000, 25, '2027-12-31', '2024-01-01', 0),
(37, N'Orthodontic wire 0.016', N'Cuộn', N'3M Unitek', 800000, 40, '2027-12-31', '2024-01-01', 0),
(38, N'Orthodontic elastics', N'Gói', N'Ormco', 150000, 80, '2027-12-31', '2024-01-01', 0),
(39, N'Orthodontic wax', N'Hộp', N'GUM', 50000, 150, '2027-12-31', '2024-01-01', 0),
-- Prosthodontic materials
(40, N'Acrylic resin (denture base)', N'Kg', N'Vertex', 800000, 40, '2027-12-31', '2024-01-01', 0),
(41, N'Acrylic teeth set', N'Bộ', N'Ivoclar', 1200000, 30, '2027-12-31', '2024-01-01', 0),
(42, N'Dental cement', N'Hộp', N'GC', 350000, 60, '2027-12-31', '2024-01-01', 0),
(43, N'Temporary crown material', N'Set', N'3M ESPE', 650000, 50, '2027-12-31', '2024-01-01', 0),
-- Surgical supplies
(44, N'Surgical suture 3-0', N'Gói', N'Ethicon', 80000, 100, '2027-12-31', '2024-01-01', 0),
(45, N'Surgical suture 4-0', N'Gói', N'Ethicon', 85000, 100, '2027-12-31', '2024-01-01', 0),
(46, N'Surgical blade #15', N'Hộp', N'Feather', 120000, 80, '2027-12-31', '2024-01-01', 0),
-- Hygiene products
(47, N'Fluoride varnish', N'Chai', N'Colgate', 450000, 50, '2027-12-31', '2024-01-01', 0),
(48, N'Fluoride gel', N'Chai', N'Sultan', 380000, 60, '2027-12-31', '2024-01-01', 0),
(49, N'Prophy paste', N'Hộp', N'Kerr', 280000, 80, '2027-12-31', '2024-01-01', 0),
(50, N'Dental floss', N'Hộp', N'Oral-B', 80000, 200, '2027-12-31', '2024-01-01', 0);

SET IDENTITY_INSERT Medicine OFF;

PRINT '  + Created 50 medicines with realistic stock';
GO

-- ============================================
-- STEP 6: SEED PATIENTS (500 patients)
-- ============================================
PRINT 'Seeding patients...';

DECLARE @counter INT = 1;
DECLARE @userCount INT;
SELECT @userCount = COUNT(*) FROM UserAccount WHERE role = 'patient';

-- Generate 500 patients
WHILE @counter <= 500
BEGIN
    DECLARE @email NVARCHAR(100) = 'patient' + CAST(@counter AS NVARCHAR) + '@gmail.com';
    DECLARE @fullname NVARCHAR(100);
    DECLARE @phone NVARCHAR(15) = '09' + RIGHT('00000000' + CAST(@counter AS NVARCHAR), 8);
    DECLARE @gender NVARCHAR(10) = CASE WHEN @counter % 2 = 0 THEN 'male' ELSE 'female' END;
    DECLARE @dob DATE = DATEADD(YEAR, -(@counter % 60 + 18), '2024-01-01'); -- Age 18-78
    DECLARE @userId INT;
    DECLARE @patientId INT;
    
    -- Generate Vietnamese names
    SET @fullname = CASE @counter % 50
        WHEN 0 THEN N'Nguyễn Văn Anh'
        WHEN 1 THEN N'Trần Thị Bình'
        WHEN 2 THEN N'Lê Văn Cường'
        WHEN 3 THEN N'Phạm Thị Dung'
        WHEN 4 THEN N'Hoàng Văn Em'
        WHEN 5 THEN N'Vũ Thị Hoa'
        WHEN 6 THEN N'Đặng Văn Khoa'
        WHEN 7 THEN N'Bùi Thị Lan'
        WHEN 8 THEN N'Đỗ Văn Minh'
        WHEN 9 THEN N'Ngô Thị Nga'
        WHEN 10 THEN N'Phan Văn Phong'
        WHEN 11 THEN N'Dương Thị Quỳnh'
        WHEN 12 THEN N'Lý Văn Sơn'
        WHEN 13 THEN N'Võ Thị Tâm'
        WHEN 14 THEN N'Trương Văn Út'
        WHEN 15 THEN N'Huỳnh Thị Vân'
        WHEN 16 THEN N'Mai Văn Xuân'
        WHEN 17 THEN N'Tô Thị Yến'
        WHEN 18 THEN N'Đinh Văn Bảo'
        WHEN 19 THEN N'Lâm Thị Châu'
        ELSE N'Nguyễn Văn ' + CAST(@counter AS NVARCHAR)
    END;
    
    -- Check if UserAccount exists
    IF NOT EXISTS (SELECT 1 FROM UserAccount WHERE email = @email)
    BEGIN
        INSERT INTO UserAccount (email, password, fullname, phone, role, date_of_birth, gender, address, created_at)
        VALUES (@email, '$argon2id$v=19$m=65536,t=3,p=1$defaultsalt$defaulthash', @fullname, @phone, 'patient', 
                @dob, @gender, N'Hà Nội', DATEADD(DAY, -(@counter % 730), '2024-01-01'));
        
        SET @userId = SCOPE_IDENTITY();
        
        INSERT INTO Patient (user_id, blood_type, allergies, medical_history, created_at)
        VALUES (@userId, 
                CASE @counter % 4 WHEN 0 THEN 'A' WHEN 1 THEN 'B' WHEN 2 THEN 'O' ELSE 'AB' END,
                CASE WHEN @counter % 10 = 0 THEN N'Dị ứng penicillin' ELSE NULL END,
                CASE WHEN @counter % 5 = 0 THEN N'Tiểu đường' WHEN @counter % 7 = 0 THEN N'Cao huyết áp' ELSE NULL END,
                DATEADD(DAY, -(@counter % 730), '2024-01-01'));
    END
    
    SET @counter = @counter + 1;
END

PRINT '  + Created 500 patients';
GO

-- ============================================
-- STEP 7: GENERATE APPOINTMENTS (5000+ appointments)
-- Period: 2024-01-01 to 2026-01-28
-- ============================================
PRINT 'Generating appointments (this may take a few minutes)...';

DECLARE @startDate DATETIME = '2024-01-01 00:00:00';
DECLARE @endDate DATETIME = '2026-01-28 00:00:00';
DECLARE @currentDate DATETIME = @startDate;
DECLARE @appointmentCount INT = 0;
DECLARE @maxAppointmentsPerDay INT;
DECLARE @dailyAppointments INT;

-- Get staff IDs (doctors)
DECLARE @doctorIds TABLE (staff_id INT, doctor_name NVARCHAR(100));
INSERT INTO @doctorIds (staff_id, doctor_name)
SELECT s.staff_id, u.fullname
FROM Staff s
INNER JOIN UserAccount u ON s.user_id = u.user_id
WHERE s.position LIKE N'%Bác sĩ%' OR s.position LIKE N'%Doctor%';

-- Get patient IDs
DECLARE @patientIds TABLE (patient_id INT);
INSERT INTO @patientIds
SELECT patient_id FROM Patient;

-- Check if we have doctors and patients
DECLARE @doctorCount INT = (SELECT COUNT(*) FROM @doctorIds);
DECLARE @patientCount INT = (SELECT COUNT(*) FROM @patientIds);

IF @doctorCount = 0
BEGIN
    PRINT '  ! No doctors found in database. Skipping appointments...';
    GOTO SkipAppointments;
END

IF @patientCount = 0
BEGIN
    PRINT '  ! No patients found in database. Skipping appointments...';
    GOTO SkipAppointments;
END

PRINT '  + Found ' + CAST(@doctorCount AS NVARCHAR) + ' doctors and ' + CAST(@patientCount AS NVARCHAR) + ' patients';

WHILE @currentDate <= @endDate
BEGIN
    -- Skip Sundays (day 1 = Sunday)
    IF DATEPART(WEEKDAY, @currentDate) != 1
    BEGIN
        -- Randomly 10-25 appointments per day (weekdays busier)
        SET @maxAppointmentsPerDay = CASE 
            WHEN DATEPART(WEEKDAY, @currentDate) = 7 THEN 12 -- Saturday: less
            ELSE 20 + (ABS(CHECKSUM(NEWID())) % 6) -- Mon-Fri: 20-25
        END;
        
        SET @dailyAppointments = 0;
        
        -- Generate appointments for this day
        WHILE @dailyAppointments < @maxAppointmentsPerDay
        BEGIN
            DECLARE @hour INT = 8 + (@dailyAppointments % 10); -- 8AM-6PM
            DECLARE @minute INT = CASE (@dailyAppointments % 2) WHEN 0 THEN 0 ELSE 30 END;
            DECLARE @apptDateTime DATETIME = DATEADD(MINUTE, @minute, DATEADD(HOUR, @hour, CAST(@currentDate AS DATETIME)));
            
            -- Random patient
            DECLARE @randomPatientId INT = (SELECT TOP 1 patient_id FROM @patientIds ORDER BY NEWID());
            
            -- Get patient info
            DECLARE @patientName NVARCHAR(100), @patientPhone NVARCHAR(15), @patientEmail NVARCHAR(100);
            SELECT @patientName = u.fullname, @patientPhone = u.phone, @patientEmail = u.email
            FROM Patient p
            INNER JOIN UserAccount u ON p.user_id = u.user_id
            WHERE p.patient_id = @randomPatientId;
            
            -- Random service (more common services have higher chance)
            DECLARE @randomServiceId INT = CASE (ABS(CHECKSUM(NEWID())) % 100)
                WHEN 0 THEN 9  -- Implant (rare, 1%)
                WHEN 1 THEN 10 -- Niềng răng (rare, 1%)
                WHEN 2 THEN 12 -- Invisalign (rare, 1%)
                WHEN 3 THEN 19 -- Toàn hàm (rare, 1%)
                ELSE CASE (ABS(CHECKSUM(NEWID())) % 10)
                    WHEN 0 THEN 1  -- Khám tổng quát (common)
                    WHEN 1 THEN 2  -- Lấy cao răng (common)
                    WHEN 2 THEN 3  -- Trám răng (common)
                    WHEN 3 THEN 5  -- Nhổ răng (common)
                    WHEN 4 THEN 6  -- Nhổ răng khôn
                    WHEN 5 THEN 7  -- Điều trị tủy
                    WHEN 6 THEN 8  -- Bọc răng sứ
                    WHEN 7 THEN 13 -- Tẩy trắng
                    WHEN 8 THEN 16 -- X-quang
                    ELSE 2 -- Lấy cao răng
                END
            END;
            
            -- Random doctor
            DECLARE @randomDoctorId INT = (SELECT TOP 1 staff_id FROM @doctorIds ORDER BY NEWID());
            
            -- Status based on date
            DECLARE @status NVARCHAR(20) = CASE
                WHEN @currentDate < '2026-01-01' THEN 'completed' -- Past appointments: completed
                WHEN @currentDate < '2026-01-15' THEN 
                    CASE (ABS(CHECKSUM(NEWID())) % 10)
                        WHEN 0 THEN 'cancelled' -- 10% cancelled
                        WHEN 1 THEN 'rejected'  -- 10% rejected
                        ELSE 'completed'        -- 80% completed
                    END
                WHEN @currentDate < '2026-01-28' THEN 'confirmed' -- Recent: confirmed
                ELSE 'booked' -- Future: booked
            END;
            
            -- Generate appointment code
            DECLARE @apptCode NVARCHAR(20) = 'APT' + RIGHT('000000' + CAST(@appointmentCount + 1 AS NVARCHAR), 6);
            
            -- Insert appointment
            INSERT INTO Appointment (
                patient_id, patient_name, phone, email, service_id, appointment_date,
                status, notes, assigned_doctor_id, appointment_code,
                check_in_time, queue_number, created_at, updated_at, is_deleted
            )
            VALUES (
                @randomPatientId, @patientName, @patientPhone, @patientEmail, @randomServiceId, @apptDateTime,
                @status, N'Đặt lịch ' + CAST(@apptCode AS NVARCHAR), @randomDoctorId, @apptCode,
                CASE WHEN @status IN ('completed', 'confirmed') THEN DATEADD(MINUTE, -10, @apptDateTime) ELSE NULL END,
                CASE WHEN @status IN ('completed', 'confirmed') THEN @dailyAppointments + 1 ELSE NULL END,
                DATEADD(DAY, -1, @apptDateTime), @apptDateTime, 0
            );
            
            SET @appointmentCount = @appointmentCount + 1;
            SET @dailyAppointments = @dailyAppointments + 1;
        END
    END
    
    SET @currentDate = DATEADD(DAY, 1, @currentDate);
    
    -- Progress indicator every 30 days
    IF DATEDIFF(DAY, @startDate, @currentDate) % 30 = 0
        PRINT '  + Generated appointments up to ' + CAST(@currentDate AS NVARCHAR(25)) + ' (' + CAST(@appointmentCount AS NVARCHAR) + ' total)';
END

SkipAppointments:
PRINT '  + Created ' + CAST(@appointmentCount AS NVARCHAR) + ' appointments';
GO

-- ============================================
-- STEP 8: RE-ENABLE FOREIGN KEY CONSTRAINTS
-- ============================================
PRINT 'Re-enabling foreign key constraints...';

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

-- ============================================
-- STEP 9: FINAL STATISTICS
-- ============================================
PRINT '';
PRINT '========================================';
PRINT 'DATABASE SEED COMPLETED!';
PRINT '========================================';
PRINT '';

DECLARE @serviceCount INT, @medicineCount INT, @patientCount INT, @apptCount INT;
DECLARE @completedAppt INT, @upcomingAppt INT;

SELECT @serviceCount = COUNT(*) FROM Service;
SELECT @medicineCount = COUNT(*) FROM Medicine;
SELECT @patientCount = COUNT(*) FROM Patient;
SELECT @apptCount = COUNT(*) FROM Appointment;
SELECT @completedAppt = COUNT(*) FROM Appointment WHERE status = 'completed';
SELECT @upcomingAppt = COUNT(*) FROM Appointment WHERE status IN ('booked', 'confirmed');

PRINT 'Summary:';
PRINT '  Services: ' + CAST(@serviceCount AS NVARCHAR);
PRINT '  Medicines: ' + CAST(@medicineCount AS NVARCHAR);
PRINT '  Patients: ' + CAST(@patientCount AS NVARCHAR);
PRINT '  Appointments: ' + CAST(@apptCount AS NVARCHAR);
PRINT '    - Completed: ' + CAST(@completedAppt AS NVARCHAR);
PRINT '    - Upcoming: ' + CAST(@upcomingAppt AS NVARCHAR);
PRINT '';
PRINT 'Period: 2024-01-01 to 2026-01-28';
PRINT '';
PRINT 'Next: Generate medical records, prescriptions, and invoices';
PRINT '       (Run Part 2 script)';
PRINT '';
GO

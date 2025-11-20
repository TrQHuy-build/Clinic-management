-- ========================================
-- TẠO DATABASE
-- ========================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DentalClinicDB')
BEGIN
    CREATE DATABASE DentalClinicDB;
END
GO

USE DentalClinicDB;
GO

-- ========================================
-- 1. Bảng tài khoản chung
-- ========================================
CREATE TABLE [UserAccount] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    fullname NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20) UNIQUE NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    role NVARCHAR(20) NOT NULL CHECK (role IN ('admin','doctor','staff','patient')),
    status NVARCHAR(20) DEFAULT N'active',
    created_at DATETIME DEFAULT GETDATE()
);
GO

-- ========================================
-- 2. Bảng bệnh nhân
-- ========================================
CREATE TABLE Patient (
    patient_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    date_of_birth DATE,
    gender NVARCHAR(10) CHECK (gender IN (N'Male',N'Female',N'Other')),
    address NVARCHAR(255),
    insurance NVARCHAR(100),
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);
GO

-- ========================================
-- 3. Bảng nhân viên (bác sĩ + lễ tân)
-- ========================================
CREATE TABLE Staff (
    staff_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    position NVARCHAR(50),
    specialization NVARCHAR(100),
    salary DECIMAL(15,2),
    hire_date DATE,
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);
GO

-- ========================================
-- 4. Bảng dịch vụ
-- ========================================
CREATE TABLE Service (
    service_id INT IDENTITY(1,1) PRIMARY KEY,
    service_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    price DECIMAL(15,2) NOT NULL,
    status NVARCHAR(20) DEFAULT N'available'
);
GO

-- ========================================
-- 5. Bảng thuốc
-- ========================================
CREATE TABLE Medicine (
    medicine_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    unit NVARCHAR(50),
    manufacturer NVARCHAR(100),
    price DECIMAL(15,2) NOT NULL
);
GO

-- ========================================
-- 6. Bảng vật tư / thiết bị
-- ========================================
CREATE TABLE Inventory (
    item_id INT IDENTITY(1,1) PRIMARY KEY,
    item_name NVARCHAR(100) NOT NULL,
    type NVARCHAR(50) CHECK (type IN ('Material', 'Equipment')),
    quantity INT DEFAULT 0,
    unit NVARCHAR(50),
    supplier NVARCHAR(100)
);
GO

-- ========================================
-- 7. Bảng lịch hẹn (ĐÃ SỬA – không dùng patient_id/staff_id)
-- ========================================
CREATE TABLE Appointment (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    patient_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(15) NULL,
    email NVARCHAR(100) NULL,
    service_id INT NULL,
    appointment_date DATETIME NOT NULL,
    status NVARCHAR(20) DEFAULT N'booked' CHECK (status IN ('booked','completed','cancelled')),
    notes NVARCHAR(255),
    FOREIGN KEY (service_id) REFERENCES Service(service_id)
);
GO

-- ========================================
-- 8. Hồ sơ bệnh án
-- ========================================
CREATE TABLE MedicalRecord (
    record_id INT IDENTITY(1,1) PRIMARY KEY,
    patient_id INT NOT NULL,
    staff_id INT NOT NULL,
    diagnosis NVARCHAR(255),
    treatment NVARCHAR(255),
    record_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (patient_id) REFERENCES Patient(patient_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);
GO

-- ========================================
-- 9. Toa thuốc
-- ========================================
CREATE TABLE Prescription (
    prescription_id INT IDENTITY(1,1) PRIMARY KEY,
    record_id INT NOT NULL,
    medicine_id INT NOT NULL,
    dosage NVARCHAR(100),
    quantity INT NOT NULL,
    notes NVARCHAR(255),
    FOREIGN KEY (record_id) REFERENCES MedicalRecord(record_id),
    FOREIGN KEY (medicine_id) REFERENCES Medicine(medicine_id)
);
GO

-- ========================================
-- 10. Hóa đơn
-- ========================================
CREATE TABLE Invoice (
    invoice_id INT IDENTITY(1,1) PRIMARY KEY,
    patient_id INT NOT NULL,
    staff_id INT NOT NULL,
    invoice_date DATETIME DEFAULT GETDATE(),
    total_amount DECIMAL(15,2) DEFAULT 0,
    status NVARCHAR(20) DEFAULT N'unpaid' CHECK (status IN ('paid','unpaid','cancelled')),
    FOREIGN KEY (patient_id) REFERENCES Patient(patient_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);
GO

-- ========================================
-- 11. Hóa đơn dịch vụ
-- ========================================
CREATE TABLE ServiceUsage (
    usage_id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    service_id INT NOT NULL,
    quantity INT DEFAULT 1,
    FOREIGN KEY (invoice_id) REFERENCES Invoice(invoice_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id)
);
GO

-- ========================================
-- 12. Hóa đơn thuốc
-- ========================================
CREATE TABLE InvoicePrescription (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    prescription_id INT NOT NULL,
    FOREIGN KEY (invoice_id) REFERENCES Invoice(invoice_id),
    FOREIGN KEY (prescription_id) REFERENCES Prescription(prescription_id)
);
GO

-- ========================================
-- 13. Giao dịch kho
-- ========================================
CREATE TABLE InventoryTransaction (
    trans_id INT IDENTITY(1,1) PRIMARY KEY,
    item_id INT NOT NULL,
    trans_date DATETIME DEFAULT GETDATE(),
    quantity INT NOT NULL,
    type NVARCHAR(20) CHECK (type IN ('import','export')),
    staff_id INT,
    FOREIGN KEY (item_id) REFERENCES Inventory(item_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);
GO

-- ========================================
-- 14. Lịch trực
-- ========================================
CREATE TABLE Shift (
    shift_id INT IDENTITY(1,1) PRIMARY KEY,
    staff_id INT NOT NULL,
    shift_date DATE NOT NULL,
    start_time TIME,
    end_time TIME,
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);
GO

-- ========================================
-- 15. Lương
-- ========================================
CREATE TABLE Salary (
    salary_id INT IDENTITY(1,1) PRIMARY KEY,
    staff_id INT NOT NULL,
    month INT NOT NULL CHECK (month BETWEEN 1 AND 12),
    year INT NOT NULL,
    base_salary DECIMAL(15,2) NOT NULL,
    bonus DECIMAL(15,2) DEFAULT 0,
    deduction DECIMAL(15,2) DEFAULT 0,
    total_salary AS (base_salary + bonus - deduction) PERSISTED,
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id),
    UNIQUE (staff_id, month, year)
);
GO

-- ========================================
-- 16. Log hệ thống
-- ========================================
CREATE TABLE AuditLog (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    action NVARCHAR(100),
    description NVARCHAR(255),
    timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);
GO

-- ========================================
-- CHÈN DỮ LIỆU MẪU
-- ========================================

-- 1. UserAccount
INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status) VALUES
(N'Nguyễn Văn A', '0901111111', 'admin@clinic.com', 'hash_admin', 'admin', N'active'),
(N'Bác sĩ Trần Văn B', '0902222222', 'doctor1@clinic.com', 'hash_doctor1', 'doctor', N'active'),
(N'Bác sĩ Lê Thị C', '0903333333', 'doctor2@clinic.com', 'hash_doctor2', 'doctor', N'active'),
(N'Bác sĩ Phạm Văn D', '0904444444', 'doctor3@clinic.com', 'hash_doctor3', 'doctor', N'active'),
(N'Nhân viên Nguyễn Văn E', '0905555555', 'staff1@clinic.com', 'hash_staff1', 'staff', N'active'),
(N'Nhân viên Lê Thị F', '0906666666', 'staff2@clinic.com', 'hash_staff2', 'staff', N'active'),
(N'Bệnh nhân Hoàng Văn G', '0907777777', 'patient1@clinic.com', 'hash_patient1', 'patient', N'active'),
(N'Bệnh nhân Phan Thị H', '0908888888', 'patient2@clinic.com', 'hash_patient2', 'patient', N'active'),
(N'Bệnh nhân Vũ Văn I', '0909999999', 'patient3@clinic.com', 'hash_patient3', 'patient', N'active'),
(N'Bệnh nhân Trần Thị J', '0910000000', 'patient4@clinic.com', 'hash_patient4', 'patient', N'active');
GO

-- 2. Patient
INSERT INTO Patient (user_id, date_of_birth, gender, address, insurance) VALUES
(7, '1990-05-10', N'Male', N'Hà Nội', N'BHYT-001'),
(8, '1985-08-22', N'Female', N'Hồ Chí Minh', N'BHYT-002'),
(9, '1992-01-15', N'Male', N'Đà Nẵng', N'BHYT-003'),
(10, '2000-11-30', N'Female', N'Cần Thơ', N'BHYT-004');
GO

-- 3. Staff
INSERT INTO Staff (user_id, position, specialization, salary, hire_date) VALUES
(2, N'Doctor', N'Nha chu', 20000000, '2020-01-15'),
(3, N'Doctor', N'Chỉnh nha', 22000000, '2021-03-10'),
(4, N'Doctor', N'Phục hình răng', 21000000, '2019-06-05'),
(5, N'Staff', NULL, 12000000, '2022-07-01'),
(6, N'Staff', NULL, 11000000, '2023-02-12');
GO

-- 4. Service
INSERT INTO Service (service_name, description, price) VALUES
(N'Khám tổng quát', N'Khám răng miệng tổng quát', 200000),
(N'Lấy cao răng', N'Vệ sinh răng miệng', 300000),
(N'Hàn răng', N'Điều trị sâu răng', 500000),
(N'Nhổ răng', N'Nhổ răng thường', 800000),
(N'Niềng răng', N'Chỉnh nha - niềng răng', 20000000),
(N'Tẩy trắng răng', N'Tẩy trắng bằng công nghệ laser', 2500000),
(N'Cấy ghép Implant', N'Trồng răng Implant', 15000000),
(N'Bọc răng sứ', N'Bọc răng sứ thẩm mỹ', 10000000),
(N'Chữa tủy răng', N'Điều trị viêm tủy', 1200000),
(N'Khám cấp cứu', N'Cấp cứu nha khoa', 500000);
GO

-- 5. Medicine
INSERT INTO Medicine (name, unit, manufacturer, price) VALUES
(N'Paracetamol', N'Viên', N'Tràng An Pharma', 2000),
(N'Amoxicillin', N'Viên', N'Mekophar', 3000),
(N'Ibuprofen', N'Viên', N'Dược Hậu Giang', 2500),
(N'Vitamin C', N'Viên', N'Imexpharm', 1500),
(N'Metronidazole', N'Viên', N'Sanofi', 4000),
(N'Chlorhexidine', N'Lọ', N'GlaxoSmithKline', 50000),
(N'Lidocaine', N'Ống', N'AstraZeneca', 70000),
(N'Augmentin', N'Viên', N'GSK', 6000),
(N'Dexamethasone', N'Ống', N'Pharbaco', 8000),
(N'Cefuroxime', N'Viên', N'Hasan Pharma', 5000);
GO

-- 6. Inventory
INSERT INTO Inventory (item_name, type, quantity, unit, supplier) VALUES
(N'Găng tay y tế', N'Material', 500, N'Hộp', N'Medical Supply Co'),
(N'Khẩu trang y tế', N'Material', 1000, N'Hộp', N'Medical Supply Co'),
(N'Kim tiêm', N'Material', 300, N'Hộp', N'VN Medical'),
(N'Ghế nha khoa', N'Equipment', 5, N'Chiếc', N'YteViet'),
(N'Đèn chiếu', N'Equipment', 10, N'Chiếc', N'DentalTech'),
(N'Bộ dụng cụ nhổ răng', N'Equipment', 15, N'Bộ', N'DentalCare'),
(N'Máy X-quang răng', N'Equipment', 2, N'Chiếc', N'Dental Imaging'),
(N'Nước súc miệng', N'Material', 200, N'Chai', N'Dr. Thanh'),
(N'Chỉ nha khoa', N'Material', 150, N'Hộp', N'Oral-B'),
(N'Máy lấy cao răng', N'Equipment', 3, N'Chiếc', N'DentalTech');
GO

-- 7. Appointment (dùng tên bệnh nhân, không FK)
INSERT INTO Appointment (patient_name, phone, email, service_id, appointment_date, status, notes) VALUES
(N'Hoàng Văn G', '0907777777', 'patient1@clinic.com', 1, '2025-10-05 09:00:00', N'booked', N'Khám định kỳ'),
(N'Phan Thị H', '0908888888', 'patient2@clinic.com', 2, '2025-10-06 10:00:00', N'booked', N'Lấy cao răng'),
(N'Vũ Văn I', '0909999999', 'patient3@clinic.com', 5, '2025-10-07 14:00:00', N'booked', N'Tư vấn niềng răng');
GO

-- 8. MedicalRecord
INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment) VALUES
(1, 1, N'Sâu răng', N'Hàn răng'),
(2, 2, N'Viêm lợi', N'Vệ sinh và thuốc'),
(3, 3, N'Lệch khớp cắn', N'Niềng răng');
GO

-- 9. Prescription
INSERT INTO Prescription (record_id, medicine_id, dosage, quantity, notes) VALUES
(1, 1, N'2 viên/ngày', 10, N'Uống sau ăn'),
(2, 2, N'3 viên/ngày', 15, N'Uống sáng - chiều'),
(3, 4, N'1 viên/ngày', 30, N'Bổ sung vitamin');
GO

-- 10. Invoice
INSERT INTO Invoice (patient_id, staff_id, total_amount, status) VALUES
(1, 1, 500000, N'paid'),
(2, 2, 350000, N'unpaid');
GO

-- 11. ServiceUsage
INSERT INTO ServiceUsage (invoice_id, service_id, quantity) VALUES
(1, 3, 1),
(2, 2, 1);
GO

-- 12. InvoicePrescription
INSERT INTO InvoicePrescription (invoice_id, prescription_id) VALUES
(1, 1),
(2, 2);
GO

-- 13. InventoryTransaction
INSERT INTO InventoryTransaction (item_id, quantity, type, staff_id) VALUES
(1, 100, 'import', 4),
(3, 50, 'export', 5);
GO

-- 14. Shift
INSERT INTO Shift (staff_id, shift_date, start_time, end_time) VALUES
(1, '2025-10-05', '08:00', '12:00'),
(2, '2025-10-06', '13:00', '17:00');
GO

-- 15. Salary
INSERT INTO Salary (staff_id, month, year, base_salary, bonus, deduction) VALUES
(1, 9, 2025, 20000000, 2000000, 500000),
(2, 9, 2025, 22000000, 1500000, 0);
GO

-- 16. AuditLog
INSERT INTO AuditLog (user_id, action, description) VALUES
(1, N'LOGIN', N'Admin đăng nhập hệ thống'),
(2, N'CREATE_APPOINTMENT', N'Bác sĩ đặt lịch hẹn cho bệnh nhân'),
(7, N'PAY_INVOICE', N'Bệnh nhân thanh toán hóa đơn');
GO

-- ========================================
-- HOÀN TẤT
-- ========================================
PRINT 'Database DentalClinicDB đã được tạo và chèn dữ liệu thành công!';
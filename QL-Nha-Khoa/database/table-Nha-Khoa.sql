-- ============================
-- Bảng tài khoản chung
-- ============================
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

-- ============================
-- Bảng bệnh nhân
-- ============================
CREATE TABLE Patient (
    patient_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    date_of_birth DATE,
    gender NVARCHAR(10) CHECK (gender IN (N'Male',N'Female',N'Other')),
    address NVARCHAR(255),
    insurance NVARCHAR(100),
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);

-- ============================
-- Bảng nhân viên (bao gồm bác sĩ)
-- ============================
CREATE TABLE Staff (
    staff_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    position NVARCHAR(50),
    specialization NVARCHAR(100), -- chỉ dùng cho doctor
    salary DECIMAL(15,2),
    hire_date DATE,
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);

-- ============================
-- Bảng dịch vụ
-- ============================
CREATE TABLE Service (
    service_id INT IDENTITY(1,1) PRIMARY KEY,
    service_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    price DECIMAL(15,2) NOT NULL,
    status NVARCHAR(20) DEFAULT N'available'
);

-- ============================
-- Bảng thuốc
-- ============================
CREATE TABLE Medicine (
    medicine_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    unit NVARCHAR(50), -- hộp, viên, ống...
    manufacturer NVARCHAR(100),
    price DECIMAL(15,2) NOT NULL
);

-- ============================
-- Bảng vật tư / thiết bị
-- ============================
CREATE TABLE Inventory (
    item_id INT IDENTITY(1,1) PRIMARY KEY,
    item_name NVARCHAR(100) NOT NULL,
    type NVARCHAR(50), -- material / equipment
    quantity INT DEFAULT 0,
    unit NVARCHAR(50),
    supplier NVARCHAR(100)
);

-- ============================
-- Bảng lịch hẹn
-- ============================
CREATE TABLE Appointment (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    patient_id INT NOT NULL,
    staff_id INT NOT NULL,
    service_id INT NULL, -- dịch vụ chính
    appointment_date DATETIME NOT NULL,
    status NVARCHAR(20) DEFAULT N'booked',
    notes NVARCHAR(255),
    FOREIGN KEY (patient_id) REFERENCES Patient(patient_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id)
);

-- ============================
-- Hồ sơ bệnh án
-- ============================
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

-- ============================
-- Toa thuốc
-- ============================
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

-- ============================
-- Hóa đơn
-- ============================
CREATE TABLE Invoice (
    invoice_id INT IDENTITY(1,1) PRIMARY KEY,
    patient_id INT NOT NULL,
    staff_id INT NOT NULL,
    invoice_date DATETIME DEFAULT GETDATE(),
    total_amount DECIMAL(15,2) DEFAULT 0,
    status NVARCHAR(20) DEFAULT N'unpaid',
    FOREIGN KEY (patient_id) REFERENCES Patient(patient_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);

-- ============================
-- Hóa đơn dịch vụ
-- ============================
CREATE TABLE ServiceUsage (
    usage_id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    service_id INT NOT NULL,
    quantity INT DEFAULT 1,
    FOREIGN KEY (invoice_id) REFERENCES Invoice(invoice_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id)
);

-- ============================
-- Hóa đơn thuốc
-- ============================
CREATE TABLE InvoicePrescription (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    prescription_id INT NOT NULL,
    FOREIGN KEY (invoice_id) REFERENCES Invoice(invoice_id),
    FOREIGN KEY (prescription_id) REFERENCES Prescription(prescription_id)
);

-- ============================
-- Giao dịch kho
-- ============================
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

-- ============================
-- Lịch trực
-- ============================
CREATE TABLE Shift (
    shift_id INT IDENTITY(1,1) PRIMARY KEY,
    staff_id INT NOT NULL,
    shift_date DATE NOT NULL,
    start_time TIME,
    end_time TIME,
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);

-- ============================
-- Lương
-- ============================
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

-- ============================
-- Log hệ thống
-- ============================
CREATE TABLE AuditLog (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    action NVARCHAR(100),
    description NVARCHAR(255),
    timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES UserAccount(user_id)
);
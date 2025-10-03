INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status)
VALUES
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


INSERT INTO Patient (user_id, date_of_birth, gender, address, insurance)
VALUES
(7, '1990-05-10', N'Male', N'Hà Nội', N'BHYT-001'),
(8, '1985-08-22', N'Female', N'Hồ Chí Minh', N'BHYT-002'),
(9, '1992-01-15', N'Male', N'Đà Nẵng', N'BHYT-003'),
(10, '2000-11-30', N'Female', N'Cần Thơ', N'BHYT-004');


INSERT INTO Staff (user_id, position, specialization, salary, hire_date)
VALUES
(2, N'Doctor', N'Nha chu', 20000000, '2020-01-15'),
(3, N'Doctor', N'Chỉnh nha', 22000000, '2021-03-10'),
(4, N'Doctor', N'Phục hình răng', 21000000, '2019-06-05'),
(5, N'Staff', NULL, 12000000, '2022-07-01'),
(6, N'Staff', NULL, 11000000, '2023-02-12');


INSERT INTO Service (service_name, description, price)
VALUES
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


INSERT INTO Medicine (name, unit, manufacturer, price)
VALUES
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


INSERT INTO Inventory (item_name, type, quantity, unit, supplier)
VALUES
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


INSERT INTO Appointment (patient_id, staff_id, service_id, appointment_date, status, notes)
VALUES
(1, 1, 1, '2025-10-05 09:00:00', N'booked', N'Khám định kỳ'),
(2, 2, 2, '2025-10-06 10:00:00', N'booked', N'Lấy cao răng'),
(3, 3, 5, '2025-10-07 14:00:00', N'booked', N'Tư vấn niềng răng');


INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment)
VALUES
(1, 1, N'Sâu răng', N'Hàn răng'),
(2, 2, N'Viêm lợi', N'Vệ sinh và thuốc'),
(3, 3, N'Lệch khớp cắn', N'Niềng răng');

INSERT INTO Prescription (record_id, medicine_id, dosage, quantity, notes)
VALUES
(1, 1, N'2 viên/ngày', 10, N'Uống sau ăn'),
(2, 2, N'3 viên/ngày', 15, N'Uống sáng - chiều'),
(3, 4, N'1 viên/ngày', 30, N'Bổ sung vitamin');


INSERT INTO Invoice (patient_id, staff_id, total_amount, status)
VALUES
(1, 1, 500000, N'paid'),
(2, 2, 350000, N'unpaid');

INSERT INTO ServiceUsage (invoice_id, service_id, quantity)
VALUES
(1, 3, 1),
(2, 2, 1);

INSERT INTO InvoicePrescription (invoice_id, prescription_id)
VALUES
(1, 1),
(2, 2);


INSERT INTO InventoryTransaction (item_id, quantity, type, staff_id)
VALUES
(1, 100, 'import', 4),
(3, 50, 'export', 5);


INSERT INTO Shift (staff_id, shift_date, start_time, end_time)
VALUES
(1, '2025-10-05', '08:00', '12:00'),
(2, '2025-10-06', '13:00', '17:00');


INSERT INTO Salary (staff_id, month, year, base_salary, bonus, deduction)
VALUES
(1, 9, 2025, 20000000, 2000000, 500000),
(2, 9, 2025, 22000000, 1500000, 0);


INSERT INTO AuditLog (user_id, action, description)
VALUES
(1, N'LOGIN', N'Admin đăng nhập hệ thống'),
(2, N'CREATE_APPOINTMENT', N'Bác sĩ đặt lịch hẹn cho bệnh nhân'),
(7, N'PAY_INVOICE', N'Bệnh nhân thanh toán hóa đơn');

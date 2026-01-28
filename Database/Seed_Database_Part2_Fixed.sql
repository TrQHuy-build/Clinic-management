-- ============================================
-- SEED DATABASE PART 2: TRANSACTIONS (FIXED)
-- Generate Medical Records, Prescriptions, Invoices
-- For completed appointments (2024-2026)
-- ============================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'GENERATING TRANSACTION DATA (PART 2)';
PRINT 'Medical Records, Prescriptions, Invoices';
PRINT '========================================';
GO

-- ============================================
-- STEP 1: GENERATE MEDICAL RECORDS
-- For all completed appointments
-- ============================================
PRINT 'Generating medical records...';

DECLARE @completedAppointments TABLE (
    appointment_id INT, 
    patient_id INT, 
    assigned_doctor_id INT,
    service_id INT,
    appointment_date DATETIME
);

INSERT INTO @completedAppointments
SELECT appointment_id, patient_id, assigned_doctor_id, service_id, appointment_date
FROM Appointment
WHERE status = 'completed'
ORDER BY appointment_date;

DECLARE @apptId INT, @patId INT, @docId INT, @svcId INT, @apptDate DATETIME;
DECLARE @recordCount INT = 0;

DECLARE appointment_cursor CURSOR FOR
SELECT appointment_id, patient_id, assigned_doctor_id, service_id, appointment_date
FROM @completedAppointments;

OPEN appointment_cursor;
FETCH NEXT FROM appointment_cursor INTO @apptId, @patId, @docId, @svcId, @apptDate;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Generate diagnosis based on service
    DECLARE @diagnosis NVARCHAR(MAX) = CASE @svcId
        WHEN 1 THEN N'Răng miệng khỏe mạnh, không có vấn đề đặc biệt'
        WHEN 2 THEN N'Tích tụ cao răng mức độ vừa, viêm lợi nhẹ'
        WHEN 3 THEN N'Sâu răng cấp độ 2, cần trám composite'
        WHEN 4 THEN N'Sâu răng cấp độ 1-2, trám amalgam'
        WHEN 5 THEN N'Răng lung lay độ 3, chỉ định nhổ'
        WHEN 6 THEN N'Răng khôn mọc lệch, gây đau, cần nhổ'
        WHEN 7 THEN N'Viêm tủy răng cấp, đau nhiều, cần điều trị tủy'
        WHEN 8 THEN N'Răng mất mô nhiều, cần bọc sứ bảo vệ'
        WHEN 9 THEN N'Mất răng số 6 hàm dưới, chỉ định cắm implant'
        WHEN 10 THEN N'Răng chen chúc, móm, chỉ định niềng răng'
        WHEN 11 THEN N'Răng vẩu nhẹ, niềng răng mắc cài sứ thẩm mỹ'
        WHEN 12 THEN N'Răng chen chúc nhẹ, phù hợp invisalign'
        WHEN 13 THEN N'Răng ố vàng, muốn tẩy trắng thẩm mỹ'
        WHEN 14 THEN N'Mất răng 3 chiếc liên tiếp, làm cầu răng'
        WHEN 15 THEN N'Mất răng nhiều, cần hàm giả tháo lắp'
        WHEN 16 THEN N'Chụp X-quang toàn cảnh để chẩn đoán'
        WHEN 17 THEN N'Viêm nha chu mức độ nặng, túi nha chu sâu'
        WHEN 18 THEN N'Cười hở lợi, cần cắt lợi thẩm mỹ'
        WHEN 19 THEN N'Mất răng toàn bộ, cần làm răng giả toàn hàm'
        WHEN 20 THEN N'Khớp cắn sai, ảnh hưởng nhai, cần điều chỉnh'
        ELSE N'Khám và tư vấn điều trị'
    END;
    
    -- Treatment plan
    DECLARE @treatment NVARCHAR(MAX) = CASE @svcId
        WHEN 1 THEN N'Tư vấn vệ sinh răng miệng, hẹn tái khám 6 tháng'
        WHEN 2 THEN N'Lấy cao răng bằng máy siêu âm, đánh bóng răng'
        WHEN 3 THEN N'Mài răng, trám composite màu A2, đánh bóng'
        WHEN 4 THEN N'Mài lỗ sâu, trám amalgam, tạo hình'
        WHEN 5 THEN N'Gây tê, nhổ răng, cầm máu, kê đơn thuốc'
        WHEN 6 THEN N'Chụp phim, gây tê, rạch lợi, nhổ răng khôn, khâu vết thương'
        WHEN 7 THEN N'Gây tê, mở tủy, làm sạch ống tủy, bơm rửa, trám tủy tạm'
        WHEN 8 THEN N'Mài răng, lấy dấu hàm, thử sứ, gắn răng sứ'
        WHEN 9 THEN N'Đánh giá xương, cắm implant, may lành 3-6 tháng'
        WHEN 10 THEN N'Gắn mắc cài kim loại, siết dây, tái khám 1 tháng/lần'
        WHEN 11 THEN N'Gắn mắc cài sứ, siết dây, tái khám 1 tháng/lần'
        WHEN 12 THEN N'Lấy dấu hàm, làm khay trong suốt, đeo 22h/ngày'
        WHEN 13 THEN N'Bảo vệ lợi, bôi gel tẩy trắng, chiếu đèn 3 lần x 15 phút'
        WHEN 14 THEN N'Mài răng trụ, lấy dấu hàm, thử cầu, gắn cầu cố định'
        WHEN 15 THEN N'Lấy dấu hàm, chọn màu răng, gắn hàm giả tháo lắp'
        WHEN 16 THEN N'Chụp X-quang panorama, đánh giá tổng quan'
        WHEN 17 THEN N'Vét vôi dưới lợi, vạt nha chu, khâu'
        WHEN 18 THEN N'Gây tê, cắt bỏ lợi thừa, điện đốt cầm máu, khâu'
        WHEN 19 THEN N'Lấy dấu nhiều lần, chọn màu răng, làm răng giả toàn hàm cố định'
        WHEN 20 THEN N'Điều chỉnh khớp cắn bằng mài chọn lọc hoặc chỉnh nha'
        ELSE N'Thực hiện điều trị theo kế hoạch'
    END;
    
    -- Insert medical record (correct column names: record_id, staff_id, not assigned_doctor_id)
    INSERT INTO MedicalRecord (
        appointment_id, patient_id, staff_id,
        diagnosis, treatment,
        record_date, created_at, is_deleted
    )
    VALUES (
        @apptId, @patId, @docId,
        @diagnosis, @treatment,
        @apptDate, @apptDate, 0
    );
    
    SET @recordCount = @recordCount + 1;
    
    -- Progress indicator every 500 records
    IF @recordCount % 1000 = 0
        PRINT '  + Generated ' + CAST(@recordCount AS NVARCHAR) + ' medical records...';
    
    FETCH NEXT FROM appointment_cursor INTO @apptId, @patId, @docId, @svcId, @apptDate;
END

CLOSE appointment_cursor;
DEALLOCATE appointment_cursor;

PRINT '  + Created ' + CAST(@recordCount AS NVARCHAR) + ' medical records';
GO

-- ============================================
-- STEP 2: GENERATE PRESCRIPTIONS
-- For medical records that need medication (70%)
-- ============================================
PRINT 'Generating prescriptions...';

DECLARE @medicalRecords TABLE (
    record_id INT,
    patient_id INT,
    staff_id INT,
    record_date DATETIME
);

INSERT INTO @medicalRecords
SELECT record_id, patient_id, staff_id, record_date
FROM MedicalRecord
WHERE (record_id % 10) <= 6 -- 70% need medication
ORDER BY record_date;

DECLARE @recordId INT, @patientId INT, @doctorId INT, @recordDate DATETIME;
DECLARE @prescriptionCount INT = 0;

DECLARE record_cursor CURSOR FOR
SELECT record_id, patient_id, staff_id, record_date
FROM @medicalRecords;

OPEN record_cursor;
FETCH NEXT FROM record_cursor INTO @recordId, @patientId, @doctorId, @recordDate;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Determine medicine combination
    DECLARE @needsPainKiller BIT = 1;
    DECLARE @needsAntibiotic BIT = CASE WHEN (@recordId % 3) = 0 THEN 1 ELSE 0 END;
    DECLARE @needsMouthwash BIT = CASE WHEN (@recordId % 2) = 0 THEN 1 ELSE 0 END;
    
    -- Pain killer
    IF @needsPainKiller = 1
    BEGIN
        DECLARE @painKillerId INT = CASE (@recordId % 4)
            WHEN 0 THEN 1 -- Paracetamol
            WHEN 1 THEN 2 -- Ibuprofen
            WHEN 2 THEN 3 -- Ketoprofen
            ELSE 1 -- Paracetamol
        END;
        
        -- Check if medicine exists
        IF EXISTS (SELECT 1 FROM Medicine WHERE medicine_id = @painKillerId)
        BEGIN
            INSERT INTO Prescription (record_id, medicine_id, quantity, dosage, notes)
            VALUES (@recordId, @painKillerId, 10, N'Uống 1 viên khi đau', N'Tối đa 3 viên/ngày');
        END
    END
    
    -- Antibiotic
    IF @needsAntibiotic = 1
    BEGIN
        DECLARE @antibioticId INT = CASE (@recordId % 4)
            WHEN 0 THEN 5 -- Amoxicillin
            WHEN 1 THEN 6 -- Augmentin
            WHEN 2 THEN 7 -- Metronidazole
            ELSE 5 -- Amoxicillin
        END;
        
        IF EXISTS (SELECT 1 FROM Medicine WHERE medicine_id = @antibioticId)
        BEGIN
            INSERT INTO Prescription (record_id, medicine_id, quantity, dosage, notes)
            VALUES (@recordId, @antibioticId, 21, N'Uống 1 viên x 3 lần/ngày', N'Sau ăn, đủ 7 ngày');
        END
    END
    
    -- Mouthwash
    IF @needsMouthwash = 1
    BEGIN
        DECLARE @mouthwashId INT = CASE (@recordId % 3)
            WHEN 0 THEN 13 -- Chlorhexidine
            WHEN 1 THEN 14 -- Betadine
            ELSE 15 -- Listerine
        END;
        
        IF EXISTS (SELECT 1 FROM Medicine WHERE medicine_id = @mouthwashId)
        BEGIN
            INSERT INTO Prescription (record_id, medicine_id, quantity, dosage, notes)
            VALUES (@recordId, @mouthwashId, 1, N'Súc miệng 2 lần/ngày', N'Sau đánh răng');
        END
    END
    
    SET @prescriptionCount = @prescriptionCount + 1;
    
    -- Progress indicator
    IF @prescriptionCount % 1000 = 0
        PRINT '  + Generated prescriptions for ' + CAST(@prescriptionCount AS NVARCHAR) + ' records...';
    
    FETCH NEXT FROM record_cursor INTO @recordId, @patientId, @doctorId, @recordDate;
END

CLOSE record_cursor;
DEALLOCATE record_cursor;

DECLARE @totalPrescriptions INT;
SELECT @totalPrescriptions = COUNT(*) FROM Prescription;
PRINT '  + Created ' + CAST(@totalPrescriptions AS NVARCHAR) + ' prescription items';
GO

-- ============================================
-- STEP 3: GENERATE INVOICES
-- Auto-generate invoices for all completed appointments
-- ============================================
PRINT 'Generating invoices...';

DECLARE @invoiceRecords TABLE (
    record_id INT,
    appointment_id INT,
    patient_id INT,
    staff_id INT,
    service_id INT,
    record_date DATETIME
);

INSERT INTO @invoiceRecords
SELECT 
    mr.record_id,
    mr.appointment_id,
    mr.patient_id,
    mr.staff_id,
    a.service_id,
    mr.record_date
FROM MedicalRecord mr
INNER JOIN Appointment a ON mr.appointment_id = a.appointment_id
WHERE a.status = 'completed'
ORDER BY mr.record_date;

DECLARE @recId INT, @apptIdInv INT, @patIdInv INT, @staffId INT, @serviceId INT, @invDate DATETIME;
DECLARE @invoiceCount INT = 0;

DECLARE invoice_cursor CURSOR FOR
SELECT record_id, appointment_id, patient_id, staff_id, service_id, record_date
FROM @invoiceRecords;

OPEN invoice_cursor;
FETCH NEXT FROM invoice_cursor INTO @recId, @apptIdInv, @patIdInv, @staffId, @serviceId, @invDate;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Get service price
    DECLARE @servicePrice DECIMAL(18,2) = 0;
    SELECT @servicePrice = ISNULL(price, 0) FROM Service WHERE service_id = @serviceId;
    
    -- Calculate medicine cost for this record
    DECLARE @medicineCost DECIMAL(18,2) = 0;
    SELECT @medicineCost = ISNULL(SUM(m.price * p.quantity), 0)
    FROM Prescription p
    INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
    WHERE p.record_id = @recId;
    
    -- Total amount
    DECLARE @totalAmount DECIMAL(18,2) = @servicePrice + @medicineCost;
    
    -- Payment method (80% cash, 15% card, 5% transfer)
    DECLARE @paymentMethod NVARCHAR(50) = CASE (@invoiceCount % 20)
        WHEN 0 THEN N'Chuyển khoản'
        WHEN 1 THEN N'Thẻ'
        WHEN 2 THEN N'Thẻ'
        WHEN 3 THEN N'Thẻ'
        ELSE N'Tiền mặt'
    END;
    
    -- Status (95% paid immediately) - Column name is 'status' not 'payment_status'
    DECLARE @invStatus NVARCHAR(20) = CASE (@invoiceCount % 20)
        WHEN 0 THEN 'unpaid' -- 5% unpaid
        ELSE 'paid'
    END;
    
    DECLARE @paidDate DATETIME = CASE 
        WHEN @invStatus = 'paid' THEN @invDate
        ELSE NULL
    END;
    
    -- Insert invoice (correct column names)
    INSERT INTO Invoice (
        patient_id, staff_id, medical_record_id, invoice_date, total_amount, 
        status, payment_method, payment_note, 
        paid_at, created_at, is_deleted
    )
    VALUES (
        @patIdInv, @staffId, @recId, @invDate, @totalAmount,
        @invStatus, @paymentMethod, N'Thanh toán ' + LOWER(@paymentMethod),
        @paidDate, @invDate, 0
    );
    
    DECLARE @newInvoiceId INT = SCOPE_IDENTITY();
    
    -- Insert service usage (no unit_price or created_at columns)
    INSERT INTO ServiceUsage (invoice_id, service_id, quantity)
    VALUES (@newInvoiceId, @serviceId, 1);
    
    -- Insert invoice prescriptions (if any) - No created_at column
    IF @medicineCost > 0
    BEGIN
        INSERT INTO InvoicePrescription (invoice_id, prescription_id)
        SELECT @newInvoiceId, prescription_id
        FROM Prescription
        WHERE record_id = @recId;
    END
    
    SET @invoiceCount = @invoiceCount + 1;
    
    -- Progress indicator
    IF @invoiceCount % 1000 = 0
        PRINT '  + Generated ' + CAST(@invoiceCount AS NVARCHAR) + ' invoices...';
    
    FETCH NEXT FROM invoice_cursor INTO @recId, @apptIdInv, @patIdInv, @staffId, @serviceId, @invDate;
END

CLOSE invoice_cursor;
DEALLOCATE invoice_cursor;

PRINT '  + Created ' + CAST(@invoiceCount AS NVARCHAR) + ' invoices';
GO

-- ============================================
-- STEP 4: FINAL STATISTICS & SUMMARY
-- ============================================
PRINT '';
PRINT '========================================';
PRINT 'TRANSACTION DATA GENERATION COMPLETED!';
PRINT '========================================';
PRINT '';

-- Count all data
PRINT 'Complete Database Summary:';
SELECT 'Services' AS Entity, COUNT(*) AS [Count] FROM Service
UNION ALL
SELECT 'Medicines', COUNT(*) FROM Medicine
UNION ALL
SELECT 'Patients', COUNT(*) FROM Patient
UNION ALL
SELECT 'Appointments', COUNT(*) FROM Appointment
UNION ALL
SELECT 'Medical Records', COUNT(*) FROM MedicalRecord
UNION ALL
SELECT 'Prescriptions', COUNT(*) FROM Prescription
UNION ALL
SELECT 'Invoices', COUNT(*) FROM Invoice
ORDER BY [Count] DESC;

PRINT '';
PRINT 'Appointment Status Breakdown:';
SELECT 
    status AS [Status],
    COUNT(*) AS [Count],
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Appointment) AS DECIMAL(5,2)) AS [Percentage]
FROM Appointment
GROUP BY status
ORDER BY COUNT(*) DESC;

PRINT '';
PRINT 'Revenue Statistics (by Year):';
SELECT 
    YEAR(invoice_date) AS [Year],
    COUNT(*) AS [Invoices],
    SUM(total_amount) AS [Total Revenue],
    AVG(total_amount) AS [Average Invoice]
FROM Invoice
GROUP BY YEAR(invoice_date)
ORDER BY YEAR(invoice_date);

PRINT '';
PRINT 'Top 10 Most Used Medicines:';
SELECT TOP 10
    m.medicine_name AS [Medicine],
    COUNT(p.prescription_id) AS [Times Prescribed],
    SUM(p.quantity) AS [Total Quantity],
    m.stock_quantity AS [Current Stock]
FROM Prescription p
INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
GROUP BY m.medicine_name, m.stock_quantity
ORDER BY COUNT(p.prescription_id) DESC;

PRINT '';
PRINT 'Top 10 Most Popular Services:';
SELECT TOP 10
    s.service_name AS [Service],
    COUNT(a.appointment_id) AS [Times Booked],
    s.price AS [Price],
    COUNT(a.appointment_id) * s.price AS [Total Revenue]
FROM Appointment a
INNER JOIN Service s ON a.service_id = s.service_id
WHERE a.status = 'completed'
GROUP BY s.service_name, s.price
ORDER BY COUNT(a.appointment_id) DESC;

PRINT '';
PRINT '========================================';
PRINT 'DATABASE IS READY FOR TESTING!';
PRINT 'Period: 2024-01-01 to 2026-01-28';
PRINT '';
PRINT 'You can now:';
PRINT '1. Test appointment booking system';
PRINT '2. Test medical record creation';
PRINT '3. Test prescription & inventory system';
PRINT '4. Test invoice generation';
PRINT '5. Test email notifications';
PRINT '6. Run analytics & reports';
PRINT '========================================';
PRINT '';
GO

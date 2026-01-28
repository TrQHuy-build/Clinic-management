-- ===============================================
-- UPDATE VIETNAMESE DATA DIRECTLY
-- Không xóa, chỉ UPDATE với tên Việt đúng
-- ===============================================

USE DentalClinicDB;
GO

PRINT '========================================';
PRINT 'CẬP NHẬT TÊN TIẾNG VIỆT CHO USER ACCOUNT';
PRINT '========================================';

-- Danh sách 104 tên tiếng Việt chuẩn
DECLARE @VietnameseNames TABLE (
    rn INT IDENTITY(1,1),
    fullname NVARCHAR(100)
);

INSERT INTO @VietnameseNames (fullname) VALUES
    (N'Nguyễn Văn An'), (N'Trần Thị Bích'), (N'Lê Minh Cường'), (N'Phạm Thanh Dung'),
    (N'Hoàng Hữu Em'), (N'Huỳnh Bảo Phương'), (N'Vũ Quốc Giang'), (N'Võ Thị Hoa'),
    (N'Phan Anh Hùng'), (N'Trương Kim Khanh'), (N'Bùi Thanh Long'), (N'Đặng Hồng Mai'),
    (N'Đỗ Ngọc Nam'), (N'Ngô Phương Oanh'), (N'Dương Văn Phúc'), (N'Lý Thị Quyên'),
    (N'Hà Minh Sơn'), (N'Đinh Xuân Thảo'), (N'Mai Thu Uyên'), (N'Tô Hà Vy'),
    (N'Nguyễn Thị Lan'), (N'Trần Văn Bình'), (N'Lê Hồng Linh'), (N'Phạm Minh Châu'),
    (N'Hoàng Thị Dung'), (N'Huỳnh Văn Em'), (N'Vũ Bảo Phong'), (N'Võ Thanh Giang'),
    (N'Phan Thị Hải'), (N'Trương Anh Hùng'), (N'Bùi Kim Khoa'), (N'Đặng Quốc Long'),
    (N'Đỗ Thị Minh'), (N'Ngô Văn Nam'), (N'Dương Hồng Oanh'), (N'Lý Ngọc Phúc'),
    (N'Hà Thị Quyên'), (N'Đinh Phương Sơn'), (N'Mai Minh Thảo'), (N'Tô Xuân Uyên'),
    (N'Nguyễn Hà Vy'), (N'Trần Thanh Anh'), (N'Lê Văn Bình'), (N'Phạm Thị Cúc'),
    (N'Hoàng Minh Đạt'), (N'Huỳnh Bảo Em'), (N'Vũ Thị Phương'), (N'Võ Quốc Giang'),
    (N'Phan Anh Hải'), (N'Trương Thị Hoa'), (N'Bùi Kim Khanh'), (N'Đặng Văn Long'),
    (N'Đỗ Hồng Mai'), (N'Ngô Ngọc Minh'), (N'Dương Thị Nam'), (N'Lý Phương Oanh'),
    (N'Hà Văn Phúc'), (N'Đinh Thị Quyên'), (N'Mai Minh Sơn'), (N'Tô Xuân Thảo'),
    (N'Nguyễn Hà Uyên'), (N'Trần Thanh Vy'), (N'Lê Văn Anh'), (N'Phạm Thị Bình'),
    (N'Hoàng Minh Cường'), (N'Huỳnh Bảo Dung'), (N'Vũ Thị Em'), (N'Võ Quốc Phong'),
    (N'Phan Anh Giang'), (N'Trương Thị Hải'), (N'Bùi Kim Hùng'), (N'Đặng Văn Khoa'),
    (N'Đỗ Hồng Long'), (N'Ngô Ngọc Mai'), (N'Dương Thị Minh'), (N'Lý Phương Nam'),
    (N'Hà Văn Oanh'), (N'Đinh Thị Phúc'), (N'Mai Minh Quyên'), (N'Tô Xuân Sơn'),
    (N'Nguyễn Hà Thảo'), (N'Trần Thanh Uyên'), (N'Lê Văn Vy'), (N'Phạm Thị An'),
    (N'Hoàng Minh Bình'), (N'Huỳnh Bảo Cường'), (N'Vũ Thị Dung'), (N'Võ Quốc Em'),
    (N'Phan Anh Phong'), (N'Trương Thị Giang'), (N'Bùi Kim Hải'), (N'Đặng Văn Hùng'),
    (N'Đỗ Hồng Khoa'), (N'Ngô Ngọc Long'), (N'Dương Thị Mai'), (N'Lý Phương Minh'),
    (N'Hà Văn Nam'), (N'Đinh Thị Oanh'), (N'Mai Minh Phúc'), (N'Tô Xuân Quyên'),
    (N'Nguyễn Hà Sơn'), (N'Trần Thanh Thảo'), (N'Lê Văn Uyên'), (N'Phạm Thị Vy');

-- UPDATE UserAccount với tên mới
;WITH NumberedUsers AS (
    SELECT 
        user_id,
        ROW_NUMBER() OVER (ORDER BY user_id) AS rn
    FROM UserAccount
    WHERE role = 'patient'
)
UPDATE u
SET u.fullname = vn.fullname
FROM UserAccount u
INNER JOIN NumberedUsers nu ON u.user_id = nu.user_id
INNER JOIN @VietnameseNames vn ON nu.rn = vn.rn;

PRINT N'✅ Đã cập nhật ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' UserAccount records';

-- UPDATE Appointment.patient_name từ UserAccount mới
UPDATE a
SET a.patient_name = u.fullname
FROM Appointment a
INNER JOIN Patient p ON a.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id
WHERE u.role = 'patient';

PRINT N'✅ Đã cập nhật ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' Appointment records';

-- UPDATE notes với Vietnamese
PRINT '';
PRINT 'Đang cập nhật notes...';

;WITH NumberedAppointments AS (
    SELECT 
        appointment_id,
        ROW_NUMBER() OVER (ORDER BY appointment_id) AS rn
    FROM Appointment
    WHERE status = N'Completed'
)
UPDATE a
SET a.notes = CASE (na.rn % 20)
    WHEN 0 THEN N'Đau răng hàm dưới bên phải'
    WHEN 1 THEN N'Khám tổng quát và tư vấn'
    WHEN 2 THEN N'Làm sạch cao răng'
    WHEN 3 THEN N'Răng bị sâu cần kiểm tra'
    WHEN 4 THEN N'Tái khám sau điều trị tủy'
    WHEN 5 THEN N'Tư vấn niềng răng'
    WHEN 6 THEN N'Khám định kỳ'
    WHEN 7 THEN N'Điều trị viêm nướu'
    WHEN 8 THEN N'Bọc răng sứ'
    WHEN 9 THEN N'Nhổ răng khôn'
    WHEN 10 THEN N'Trám răng thẩm mỹ'
    WHEN 11 THEN N'Tẩy trắng răng'
    WHEN 12 THEN N'Điều trị tủy răng'
    WHEN 13 THEN N'Cấy ghép Implant'
    WHEN 14 THEN N'Làm cầu răng sứ'
    WHEN 15 THEN N'Chụp X-quang răng'
    WHEN 16 THEN N'Kiểm tra tổng quát'
    WHEN 17 THEN N'Vệ sinh răng miệng'
    WHEN 18 THEN N'Điều trị sâu răng'
    WHEN 19 THEN N'Tư vấn chỉnh nha'
END
FROM Appointment a
INNER JOIN NumberedAppointments na ON a.appointment_id = na.appointment_id
WHERE (na.rn % 3) = 0; -- 33% appointments có notes

PRINT N'✅ Đã cập nhật ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' notes';

PRINT '';
PRINT '========================================';
PRINT 'KIỂM TRA KẾT QUẢ';
PRINT '========================================';

-- Kiểm tra UserAccount
SELECT TOP 10
    u.user_id,
    u.fullname AS [Họ tên],
    u.phone AS [Điện thoại],
    CAST(u.fullname AS VARBINARY(200)) AS [Hex - MUST start with correct UTF-16]
FROM UserAccount u
WHERE u.role = 'patient'
ORDER BY u.user_id;

PRINT '';

-- Kiểm tra Appointment
SELECT TOP 10
    a.appointment_id,
    a.patient_name AS [Tên bệnh nhân],
    FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
    a.notes AS [Ghi chú],
    a.status AS [Trạng thái],
    CAST(a.patient_name AS VARBINARY(200)) AS [Hex]
FROM Appointment a
WHERE a.notes IS NOT NULL
ORDER BY a.appointment_id DESC;

-- Statistics
PRINT '';
PRINT '========================================';
PRINT 'THỐNG KÊ';
PRINT '========================================';

SELECT 
    COUNT(*) AS [Tổng số appointments],
    SUM(CASE WHEN patient_name IS NOT NULL THEN 1 ELSE 0 END) AS [Có tên bệnh nhân],
    SUM(CASE WHEN notes IS NOT NULL THEN 1 ELSE 0 END) AS [Có ghi chú]
FROM Appointment;

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT! CHẠY TestVietnamese.exe HOẶC';
PRINT 'APPLICATION ĐỂ KIỂM TRA';
PRINT '========================================';

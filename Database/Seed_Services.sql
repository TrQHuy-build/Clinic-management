-- =============================================
-- SEED DATA: Dental Services
-- Purpose: Add common dental services with pricing
-- =============================================

USE DentalClinicDB;
GO

PRINT '=== Adding Dental Services ==='
GO

-- Clear existing services (if needed)
-- DELETE FROM Service WHERE service_id > 0;
-- DBCC CHECKIDENT ('Service', RESEED, 0);

-- Insert dental services
INSERT INTO Service (service_name, description, price, status) VALUES
    (N'Khám tổng quát', N'Khám và tư vấn tổng quát về sức khỏe răng miệng', 100000, N'active'),
    (N'Lấy cao răng', N'Vệ sinh răng miệng, loại bỏ cao răng và mảng bám', 200000, N'active'),
    (N'Trám răng Composite', N'Trám răng bằng vật liệu Composite thẩm mỹ', 300000, N'active'),
    (N'Trám răng Amalgam', N'Trám răng bằng hỗn hống (răng hàm)', 250000, N'active'),
    (N'Nhổ răng thường', N'Nhổ răng sữa hoặc răng vĩnh viễn đơn giản', 200000, N'active'),
    (N'Nhổ răng khôn', N'Nhổ răng khôn mọc lệch hoặc mọc ngầm', 800000, N'active'),
    (N'Điều trị tủy răng', N'Điều trị tủy răng nhiễm trùng (1 ống tủy)', 500000, N'active'),
    (N'Bọc răng sứ Titan', N'Bọc răng sứ Titan kim loại', 1500000, N'active'),
    (N'Bọc răng sứ Zirconia', N'Bọc răng sứ Zirconia toàn sứ cao cấp', 3500000, N'active'),
    (N'Cấy ghép Implant', N'Cấy ghép răng Implant (chưa bao gồm răng sứ)', 15000000, N'active'),
    (N'Tẩy trắng răng tại phòng khám', N'Tẩy trắng răng công nghệ Bleaching', 2000000, N'active'),
    (N'Tẩy trắng răng tại nhà', N'Bộ kit tẩy trắng răng sử dụng tại nhà', 1200000, N'active'),
    (N'Niềng răng mắc cài kim loại', N'Chi phí niềng răng mắc cài kim loại (toàn hàm)', 25000000, N'active'),
    (N'Niềng răng mắc cài sứ', N'Chi phí niềng răng mắc cài sứ thẩm mỹ (toàn hàm)', 35000000, N'active'),
    (N'Niềng răng trong suốt Invisalign', N'Niềng răng không mắc cài Invisalign', 80000000, N'active'),
    (N'Cạo vôi răng siêu âm', N'Làm sạch vôi răng bằng công nghệ siêu âm', 150000, N'active'),
    (N'Chụp X-quang răng', N'Chụp X-quang toàn cảnh răng', 150000, N'active'),
    (N'Chụp CT Cone Beam', N'Chụp CT 3D răng hàm mặt', 500000, N'active'),
    (N'Hàn răng thẩm mỹ', N'Hàn răng bị mẻ hoặc gãy bằng composite', 400000, N'active'),
    (N'Bọc răng Veneer', N'Dán răng sứ Veneer siêu mỏng', 4000000, N'active'),
    (N'Điều chỉnh khớp cắn', N'Mài chỉnh khớp cắn', 200000, N'active'),
    (N'Phục hình răng tháo lắp', N'Làm răng giả tháo lắp (1 hàm)', 3000000, N'active'),
    (N'Phun xăm môi', N'Phun xăm môi thẩm mỹ', 2500000, N'active'),
    (N'Laser điều trị nướu', N'Điều trị viêm nướu bằng laser', 1000000, N'active'),
    (N'Nạo túi lợi', N'Nạo viêm túi lợi sâu', 300000, N'active');

PRINT 'Added 25 dental services successfully.'
GO

-- Verify data
SELECT 
    service_id, 
    service_name, 
    FORMAT(price, 'N0', 'vi-VN') + ' VNĐ' AS price,
    status
FROM Service
ORDER BY service_id;
GO

PRINT ''
PRINT '=== Service Seeding Complete ==='
GO

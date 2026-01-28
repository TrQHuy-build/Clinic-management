-- =============================================
-- SCRIPT MIGRATION PASSWORD - NÂNG CẤP BẢO MẬT
-- =============================================
-- Script này sẽ giúp migrate tất cả passwords hiện có
-- sang format mới (PBKDF2) một cách an toàn

USE DentalClinicDB;
GO

-- Bước 1: Backup bảng UserAccount trước khi migrate
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserAccount_Backup')
BEGIN
    SELECT * INTO UserAccount_Backup FROM UserAccount;
    PRINT '✅ Đã backup bảng UserAccount sang UserAccount_Backup';
END
GO

-- Bước 2: Thêm cột tạm để đánh dấu accounts đã migrate
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('UserAccount') 
               AND name = 'password_migrated')
BEGIN
    ALTER TABLE UserAccount ADD password_migrated BIT DEFAULT 0;
    PRINT '✅ Đã thêm cột password_migrated';
END
GO

-- Bước 3: Kiểm tra số lượng accounts cần migrate
DECLARE @OldFormatCount INT;
SELECT @OldFormatCount = COUNT(*) 
FROM UserAccount 
WHERE password_migrated = 0 OR password_migrated IS NULL;

PRINT '📊 Số tài khoản cần migrate: ' + CAST(@OldFormatCount AS VARCHAR(10));
GO

-- =============================================
-- HƯỚNG DẪN MIGRATION
-- =============================================
PRINT '
═══════════════════════════════════════════════════
📖 HƯỚNG DẪN MIGRATION PASSWORD
═══════════════════════════════════════════════════

⚠️ LƯU Ý QUAN TRỌNG:
   Migration password không thể tự động vì ta không biết 
   password gốc của users!

🔐 CÓ 3 PHƯƠNG ÁN:

───────────────────────────────────────────────────
PHƯƠNG ÁN 1: AUTO-MIGRATE KHI ĐĂNG NHẬP (KHUYẾN NGHỊ)
───────────────────────────────────────────────────
✅ Đã được implement trong frmLogin.cs
✅ User đăng nhập bình thường
✅ Hệ thống tự động detect format cũ
✅ Auto update sang format mới
✅ Lần login tiếp theo dùng format mới

👉 KHÔNG CẦN LÀM GÌ THÊM - Hệ thống tự xử lý!

───────────────────────────────────────────────────
PHƯƠNG ÁN 2: RESET PASSWORD CHO TẤT CẢ USER
───────────────────────────────────────────────────
Đặt mật khẩu mặc định: "TempPass123!"
User phải đổi password lần đầu đăng nhập

-- Uncommment các dòng dưới để chạy:
/*
DECLARE @DefaultPassword NVARCHAR(255);
-- Đây là hash của "TempPass123!" bằng PBKDF2
-- Bạn cần chạy app C# để generate hash này:
-- string hash = PasswordHasher.HashPassword("TempPass123!");
SET @DefaultPassword = ''100000:HASH_HERE'';

UPDATE UserAccount 
SET password_hash = @DefaultPassword,
    password_migrated = 1
WHERE password_migrated = 0 OR password_migrated IS NULL;

PRINT ''✅ Đã reset password cho tất cả users'';
PRINT ''⚠️ Password mặc định: TempPass123!'';
PRINT ''⚠️ Yêu cầu users đổi password ngay!'';
*/

───────────────────────────────────────────────────
PHƯƠNG ÁN 3: RESET PASSWORD TỪNG USER
───────────────────────────────────────────────────
Sử dụng cho testing hoặc tạo user mới

-- Example: Tạo user admin với password "Admin@123"
-- Bạn cần chạy C# code để generate hash:
/*
string hash = PasswordHasher.HashPassword("Admin@123");
Console.WriteLine(hash);
*/

-- Sau đó update vào database:
/*
UPDATE UserAccount 
SET password_hash = ''100000:BASE64_SALT_HERE:BASE64_HASH_HERE'',
    password_migrated = 1
WHERE user_id = 1;
*/

═══════════════════════════════════════════════════
✨ KHUYẾN NGHỊ: Sử dụng Phương án 1
═══════════════════════════════════════════════════
Đây là cách an toàn nhất và không làm gián đoạn users.

📝 Sau khi tất cả users đã login ít nhất 1 lần:
   - Chạy query dưới để check:
   
   SELECT COUNT(*) AS remaining
   FROM UserAccount 
   WHERE password_migrated = 0;
   
   - Nếu còn accounts chưa migrate, có thể:
     + Đợi họ login
     + Hoặc reset password cho họ (Phương án 2)

🗑️ Sau khi hoàn tất migration (vài tuần):
   - Xóa cột password_migrated
   - Xóa bảng UserAccount_Backup (sau khi backup ra file)

═══════════════════════════════════════════════════
';

-- Bước 4: Query kiểm tra trạng thái migration
SELECT 
    COUNT(*) AS total_users,
    SUM(CASE WHEN password_migrated = 1 THEN 1 ELSE 0 END) AS migrated,
    SUM(CASE WHEN password_migrated = 0 OR password_migrated IS NULL THEN 1 ELSE 0 END) AS not_migrated,
    CAST(SUM(CASE WHEN password_migrated = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS percent_migrated
FROM UserAccount;
GO

-- Bước 5: Chi tiết users chưa migrate
SELECT 
    user_id,
    fullname,
    email,
    role,
    created_at,
    CASE 
        WHEN password_migrated = 1 THEN '✅ Đã migrate'
        ELSE '⏳ Chưa migrate'
    END AS migration_status
FROM UserAccount
ORDER BY password_migrated ASC, created_at DESC;
GO

PRINT '
═══════════════════════════════════════════════════
✅ Script migration đã sẵn sàng!
═══════════════════════════════════════════════════
';

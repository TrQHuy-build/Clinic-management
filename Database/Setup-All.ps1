# =============================================
# CHẠY TẤT CẢ CÁC SCRIPT DATABASE
# =============================================

Write-Host "=========================================="  -ForegroundColor Cyan
Write-Host "  DATABASE SETUP - DENTAL CLINIC"  -ForegroundColor Cyan
Write-Host "=========================================="  -ForegroundColor Cyan
Write-Host ""

$server = "(localdb)\MSSQLLocalDB"
$database = "DentalClinicDB"
$errorCount = 0

# Function để chạy SQL script
function Run-SqlScript {
    param (
        [string]$ScriptPath,
        [string]$Description
    )
    
    Write-Host "► $Description..." -ForegroundColor Yellow
    
    if (-not (Test-Path $ScriptPath)) {
        Write-Host "  ✗ ERROR: File không tồn tại: $ScriptPath" -ForegroundColor Red
        $script:errorCount++
        return $false
    }
    
    try {
        $result = sqlcmd -S $server -d $database -i $ScriptPath -b 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Hoàn thành!" -ForegroundColor Green
            return $true
        } else {
            Write-Host "  ✗ Có lỗi xảy ra (Exit code: $LASTEXITCODE)" -ForegroundColor Red
            $script:errorCount++
            return $false
        }
    }
    catch {
        Write-Host "  ✗ Exception: $_" -ForegroundColor Red
        $script:errorCount++
        return $false
    }
}

Write-Host "1. Tạo database và tables cơ bản..." -ForegroundColor Cyan
Run-SqlScript "Database\SQLQuery2.sql" "Tạo database DentalClinicDB"

Write-Host ""
Write-Host "2. Cập nhật schema (Migration)..." -ForegroundColor Cyan
Run-SqlScript "Database\Migration_Complete_Update.sql" "Thêm columns và tables mới"

Write-Host ""
Write-Host "3. Seed dữ liệu cơ bản..." -ForegroundColor Cyan
Run-SqlScript "Database\Seed_Services.sql" "Thêm 35 dịch vụ nha khoa"

Write-Host ""
Write-Host "4. Tạo bảng Salary và seed dữ liệu..." -ForegroundColor Cyan
Run-SqlScript "Database\Create_Salary_Table.sql" "Tạo bảng lương với 6 tháng dữ liệu"

Write-Host ""
Write-Host "5. Tạo dữ liệu lớn (2 năm)..." -ForegroundColor Cyan
Write-Host "► Đang tạo 100 patients và ~10,000 appointments..." -ForegroundColor Yellow
Run-SqlScript "Database\Seed_Large_Data_2024_2026.sql" "Seed 100 patients"
Run-SqlScript "Database\Seed_Appointments_Simple.sql" "Seed ~10,000 appointments (2024-2026)"

Write-Host ""
Write-Host "=========================================="  -ForegroundColor Cyan

if ($errorCount -eq 0) {
    Write-Host "  ✅ HOÀN THÀNH TẤT CẢ!" -ForegroundColor Green
    Write-Host "=========================================="  -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📊 Tổng quan database:" -ForegroundColor White
    
    # Hiển thị thống kê
    $stats = sqlcmd -S $server -d $database -Q "
        SELECT 
            'Patients' AS [Table], COUNT(*) AS [Records] FROM Patient
        UNION ALL SELECT 'Appointments', COUNT(*) FROM Appointment
        UNION ALL SELECT 'Services', COUNT(*) FROM Service
        UNION ALL SELECT 'Inventory', COUNT(*) FROM Inventory
        UNION ALL SELECT 'Staff', COUNT(*) FROM Staff
        UNION ALL SELECT 'Salary Records', COUNT(*) FROM Salary
        UNION ALL SELECT 'Shift Records', COUNT(*) FROM Shift
        UNION ALL SELECT 'Medicine', COUNT(*) FROM Medicine
    " -h -1
    
    Write-Host $stats
    Write-Host ""
    Write-Host "✅ Bạn có thể chạy ứng dụng ngay bây giờ!" -ForegroundColor Green
    
} else {
    Write-Host "  ⚠️ HOÀN THÀNH VỚI $errorCount LỖI" -ForegroundColor Yellow
    Write-Host "=========================================="  -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Vui lòng kiểm tra lại các file log để biết chi tiết lỗi." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Nhấn Enter để đóng..." -ForegroundColor Gray
$null = Read-Host

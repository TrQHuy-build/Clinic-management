# Script kiểm tra Database - DentalClinicDB
# Chạy script này để debug vấn đề

Write-Host "=== CHECKING DATABASE CONNECTION ===" -ForegroundColor Cyan

# 1. Kiểm tra LocalDB instances
Write-Host "`n[1] Checking LocalDB instances..." -ForegroundColor Yellow
try {
    SqlLocalDB info
    Write-Host "✓ LocalDB command found" -ForegroundColor Green
} catch {
    Write-Host "✗ SqlLocalDB not found. Please install SQL Server Express LocalDB" -ForegroundColor Red
    exit
}

# 2. Kiểm tra MSSQLLocalDB instance
Write-Host "`n[2] Checking MSSQLLocalDB instance..." -ForegroundColor Yellow
try {
    SqlLocalDB info MSSQLLocalDB
    Write-Host "✓ MSSQLLocalDB instance exists" -ForegroundColor Green
} catch {
    Write-Host "✗ MSSQLLocalDB instance not found" -ForegroundColor Red
    Write-Host "Creating MSSQLLocalDB instance..." -ForegroundColor Yellow
    SqlLocalDB create MSSQLLocalDB
    SqlLocalDB start MSSQLLocalDB
}

# 3. Kiểm tra connection string
Write-Host "`n[3] Checking appsettings.json..." -ForegroundColor Yellow
$appsettingsPath = "d:\C#\Winform\New folder (2)\web\Web-BenhNhan\Web-BenhNhan\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    $connString = $appsettings.ConnectionStrings.DefaultConnection
    Write-Host "Connection String: $connString" -ForegroundColor Cyan
    Write-Host "✓ appsettings.json found" -ForegroundColor Green
} else {
    Write-Host "✗ appsettings.json not found" -ForegroundColor Red
}

# 4. Test SQL connection
Write-Host "`n[4] Testing SQL connection..." -ForegroundColor Yellow
try {
    $sqlCmd = @"
SELECT 
    name,
    database_id,
    create_date,
    compatibility_level
FROM sys.databases
WHERE name = 'DentalClinicDB'
"@
    
    # Sử dụng sqlcmd để test
    $result = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q $sqlCmd -W
    if ($result) {
        Write-Host "✓ Database DentalClinicDB exists" -ForegroundColor Green
        Write-Host $result -ForegroundColor Cyan
    } else {
        Write-Host "⚠ Database might not exist" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Cannot connect to SQL Server" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

# 5. Kiểm tra bảng Appointment
Write-Host "`n[5] Checking Appointment table..." -ForegroundColor Yellow
try {
    $sqlCmd = @"
USE DentalClinicDB;
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Appointment';
"@
    
    $result = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q $sqlCmd -W
    if ($result -match "Appointment") {
        Write-Host "✓ Appointment table exists" -ForegroundColor Green
        
        # Đếm số records
        $countCmd = "USE DentalClinicDB; SELECT COUNT(*) as TotalRows FROM Appointment;"
        $count = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q $countCmd -h -1 -W
        Write-Host "  Total records: $count" -ForegroundColor Cyan
        
        # Hiển thị 5 records mới nhất
        $selectCmd = @"
USE DentalClinicDB;
SELECT TOP 5 
    appointment_id,
    patient_name,
    phone,
    appointment_date,
    status
FROM Appointment
ORDER BY appointment_id DESC;
"@
        Write-Host "`n  Latest appointments:" -ForegroundColor Cyan
        sqlcmd -S "(localdb)\MSSQLLocalDB" -Q $selectCmd -W
        
    } else {
        Write-Host "⚠ Appointment table does not exist" -ForegroundColor Yellow
        Write-Host "  Run: dotnet ef database update" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Error checking table" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

# 6. Kiểm tra EF Core migrations
Write-Host "`n[6] Checking EF Core migrations..." -ForegroundColor Yellow
$projectPath = "d:\C#\Winform\New folder (2)\web\Web-BenhNhan\Web-BenhNhan"
Push-Location $projectPath

try {
    $migrations = dotnet ef migrations list 2>&1
    if ($migrations -match "No migrations") {
        Write-Host "⚠ No migrations found" -ForegroundColor Yellow
        Write-Host "  Create migration: dotnet ef migrations add InitialCreate" -ForegroundColor Yellow
    } else {
        Write-Host "✓ Migrations found:" -ForegroundColor Green
        Write-Host $migrations -ForegroundColor Cyan
    }
} catch {
    Write-Host "✗ Error checking migrations" -ForegroundColor Red
}

Pop-Location

# Summary
Write-Host "`n=== SUMMARY ===" -ForegroundColor Cyan
Write-Host "Next steps if database is empty:" -ForegroundColor Yellow
Write-Host "1. cd 'd:\C#\Winform\New folder (2)\web\Web-BenhNhan\Web-BenhNhan'" -ForegroundColor White
Write-Host "2. dotnet ef migrations add InitialCreate" -ForegroundColor White
Write-Host "3. dotnet ef database update" -ForegroundColor White
Write-Host "4. dotnet run" -ForegroundColor White
Write-Host "5. Navigate to https://localhost:7xxx/Home/TestDb to test" -ForegroundColor White
Write-Host "`nOr manually insert test data:" -ForegroundColor Yellow
Write-Host @"
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "
USE DentalClinicDB;
INSERT INTO Appointment (patient_name, phone, email, service_id, appointment_date, status, notes)
VALUES ('Test User', '0901234567', 'test@test.com', 1, GETDATE(), 'booked', 'Test appointment');
SELECT * FROM Appointment;
"
"@ -ForegroundColor White

Write-Host "`n=== DONE ===" -ForegroundColor Green

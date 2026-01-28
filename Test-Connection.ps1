# ========================================
# TEST DATABASE CONNECTION
# ========================================

param(
    [string]$ServerIP = "10.128.43.222",
    [string]$Username = "ClinicUser",
    [string]$Password = "Clinic@User2026"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DATABASE CONNECTION TEST" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Ping server
Write-Host "Test 1: Ping server..." -ForegroundColor Yellow
$pingResult = Test-Connection -ComputerName $ServerIP -Count 2 -Quiet
if ($pingResult) {
    Write-Host "✅ Server is reachable" -ForegroundColor Green
} else {
    Write-Host "❌ Cannot reach server!" -ForegroundColor Red
    Write-Host "   Kiểm tra: IP address, network cable, WiFi" -ForegroundColor Gray
    exit
}
Write-Host ""

# Test 2: Test port 1433
Write-Host "Test 2: Testing port 1433..." -ForegroundColor Yellow
try {
    $tcpTest = Test-NetConnection -ComputerName $ServerIP -Port 1433 -WarningAction SilentlyContinue
    if ($tcpTest.TcpTestSucceeded) {
        Write-Host "✅ Port 1433 is open" -ForegroundColor Green
    } else {
        Write-Host "❌ Port 1433 is closed!" -ForegroundColor Red
        Write-Host "   Kiểm tra: Firewall, SQL Server service" -ForegroundColor Gray
        exit
    }
} catch {
    Write-Host "❌ Cannot test port: $($_.Exception.Message)" -ForegroundColor Red
    exit
}
Write-Host ""

# Test 3: Test SQL Server connection (local)
Write-Host "Test 3: Testing SQL Server (localhost)..." -ForegroundColor Yellow
$localTest = sqlcmd -S "localhost\SQLEXPRESS" -U $Username -P $Password -Q "SELECT @@VERSION" -h -1 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Local SQL Server connection successful" -ForegroundColor Green
} else {
    Write-Host "❌ Cannot connect to local SQL Server!" -ForegroundColor Red
    Write-Host "   Lỗi: $localTest" -ForegroundColor Gray
}
Write-Host ""

# Test 4: Test SQL Server connection (remote)
Write-Host "Test 4: Testing SQL Server (remote: $ServerIP)..." -ForegroundColor Yellow
$remoteTest = sqlcmd -S "$ServerIP\SQLEXPRESS" -U $Username -P $Password -Q "SELECT @@VERSION" -h -1 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Remote SQL Server connection successful" -ForegroundColor Green
} else {
    Write-Host "❌ Cannot connect to remote SQL Server!" -ForegroundColor Red
    Write-Host "   Lỗi: $remoteTest" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   Các bước kiểm tra:" -ForegroundColor Yellow
    Write-Host "   1. SQL Server service đang chạy?" -ForegroundColor Gray
    Write-Host "      Get-Service MSSQL`$SQLEXPRESS" -ForegroundColor Gray
    Write-Host "   2. SQL Server Browser service đang chạy?" -ForegroundColor Gray
    Write-Host "      Get-Service SQLBrowser" -ForegroundColor Gray
    Write-Host "   3. User '$Username' đã được tạo?" -ForegroundColor Gray
    Write-Host "      sqlcmd -S localhost\SQLEXPRESS -E -Q `"SELECT name FROM sys.server_principals WHERE name = '$Username'`"" -ForegroundColor Gray
    exit
}
Write-Host ""

# Test 5: Test database access
Write-Host "Test 5: Testing database access..." -ForegroundColor Yellow
$dbTest = sqlcmd -S "$ServerIP\SQLEXPRESS" -U $Username -P $Password -d DentalClinicDB -Q "SELECT COUNT(*) FROM Patient" -h -1 2>&1
if ($LASTEXITCODE -eq 0) {
    $patientCount = $dbTest.Trim()
    Write-Host "✅ Database access successful" -ForegroundColor Green
    Write-Host "   Số bệnh nhân: $patientCount" -ForegroundColor Cyan
} else {
    Write-Host "❌ Cannot access database!" -ForegroundColor Red
    Write-Host "   Lỗi: $dbTest" -ForegroundColor Gray
}
Write-Host ""

# Test 6: Test query performance
Write-Host "Test 6: Testing query performance..." -ForegroundColor Yellow
$startTime = Get-Date
$perfTest = sqlcmd -S "$ServerIP\SQLEXPRESS" -U $Username -P $Password -d DentalClinicDB -Q "SELECT TOP 1000 * FROM Appointment ORDER BY appointment_date DESC" -h -1 2>&1
$endTime = Get-Date
$duration = ($endTime - $startTime).TotalMilliseconds
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Query completed in $([math]::Round($duration, 2)) ms" -ForegroundColor Green
    if ($duration -lt 100) {
        Write-Host "   Performance: Excellent" -ForegroundColor Green
    } elseif ($duration -lt 500) {
        Write-Host "   Performance: Good" -ForegroundColor Cyan
    } else {
        Write-Host "   Performance: Slow (check network)" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Query failed!" -ForegroundColor Red
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "📋 CONNECTION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Server IP:      $ServerIP" -ForegroundColor White
Write-Host "SQL Instance:   SQLEXPRESS" -ForegroundColor White
Write-Host "Database:       DentalClinicDB" -ForegroundColor White
Write-Host "Username:       $Username" -ForegroundColor White
Write-Host ""
Write-Host "Connection String for App.config:" -ForegroundColor Yellow
Write-Host "Data Source=$ServerIP\SQLEXPRESS,1433;Initial Catalog=DentalClinicDB;User ID=$Username;Password=$Password;TrustServerCertificate=True" -ForegroundColor Cyan
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "Nhấn Enter để thoát"

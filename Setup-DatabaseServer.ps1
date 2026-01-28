# ========================================
# AUTO SETUP SQL SERVER EXPRESS AS SHARED DATABASE
# ========================================

param(
    [string]$SQLPassword = "ClinicDB@2026",
    [string]$ClientUser = "ClinicUser",
    [string]$ClientPassword = "Clinic@User2026"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SQL SERVER SETUP FOR SHARED DATABASE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Kiểm tra SQL Server Express đã cài chưa
Write-Host "Step 1: Checking SQL Server Express installation..." -ForegroundColor Yellow

$sqlInstances = Get-Service | Where-Object {$_.Name -like "MSSQL*"}
if ($sqlInstances.Count -eq 0) {
    Write-Host "❌ SQL Server Express NOT FOUND!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng cài đặt SQL Server Express:" -ForegroundColor Yellow
    Write-Host "1. Tải từ: https://go.microsoft.com/fwlink/?linkid=866658" -ForegroundColor Green
    Write-Host "2. Chọn 'Basic' installation" -ForegroundColor Green
    Write-Host "3. Chọn 'Mixed Mode' authentication" -ForegroundColor Green
    Write-Host "4. Đặt mật khẩu SA và chạy lại script này" -ForegroundColor Green
    Write-Host ""
    Read-Host "Nhấn Enter để thoát"
    exit
}

Write-Host "✅ SQL Server found: $($sqlInstances.Name -join ', ')" -ForegroundColor Green
Write-Host ""

# Step 2: Lấy địa chỉ IP
Write-Host "Step 2: Getting server IP address..." -ForegroundColor Yellow
$ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -like "192.168.*" -or $_.IPAddress -like "10.*"} | Select-Object -First 1).IPAddress

if (-not $ipAddress) {
    $ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notlike "*Loopback*"} | Select-Object -First 1).IPAddress
}

Write-Host "✅ Server IP: $ipAddress" -ForegroundColor Green
Write-Host ""

# Step 3: Mở Windows Firewall port 1433
Write-Host "Step 3: Configuring Windows Firewall..." -ForegroundColor Yellow

try {
    $existingRule = Get-NetFirewallRule -DisplayName "SQL Server Port 1433" -ErrorAction SilentlyContinue
    if ($existingRule) {
        Write-Host "⚠️  Firewall rule already exists" -ForegroundColor Yellow
    } else {
        New-NetFirewallRule -DisplayName "SQL Server Port 1433" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow -ErrorAction Stop
        Write-Host "✅ Firewall rule created for port 1433" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Failed to create firewall rule: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Vui lòng chạy PowerShell as Administrator" -ForegroundColor Yellow
}
Write-Host ""

# Step 4: Export database từ LocalDB
Write-Host "Step 4: Exporting database from LocalDB..." -ForegroundColor Yellow

$backupPath = "D:\DatabaseBackup"
if (-not (Test-Path $backupPath)) {
    New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
}

$backupFile = "$backupPath\DentalClinicDB.bak"

try {
    $exportCmd = @"
BACKUP DATABASE DentalClinicDB 
TO DISK = '$backupFile'
WITH FORMAT, INIT, COMPRESSION, STATS = 10;
"@
    
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d DentalClinicDB -Q $exportCmd
    
    if (Test-Path $backupFile) {
        Write-Host "✅ Database exported to: $backupFile" -ForegroundColor Green
    } else {
        Write-Host "❌ Backup file not created" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Export failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Step 5: Tạo Connection String cho Server và Client
Write-Host "Step 5: Generating connection strings..." -ForegroundColor Yellow
Write-Host ""

$serverConnString = "Data Source=localhost\SQLEXPRESS;Initial Catalog=DentalClinicDB;User ID=$ClientUser;Password=$ClientPassword;TrustServerCertificate=True"
$clientConnString = "Data Source=$ipAddress\SQLEXPRESS,1433;Initial Catalog=DentalClinicDB;User ID=$ClientUser;Password=$ClientPassword;TrustServerCertificate=True"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CONNECTION STRINGS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📌 FOR SERVER (this computer):" -ForegroundColor Green
Write-Host $serverConnString -ForegroundColor White
Write-Host ""
Write-Host "📌 FOR CLIENT (other computers):" -ForegroundColor Green
Write-Host $clientConnString -ForegroundColor White
Write-Host ""

# Lưu connection strings ra file
$configFile = "ConnectionStrings_Generated.txt"
@"
========================================
DENTAL CLINIC DATABASE CONNECTION INFO
========================================

SERVER IP: $ipAddress
SQL Server Instance: SQLEXPRESS
Database Name: DentalClinicDB
Username: $ClientUser
Password: $ClientPassword

========================================
CONNECTION STRING FOR SERVER
========================================
$serverConnString

========================================
CONNECTION STRING FOR CLIENT
========================================
$clientConnString

========================================
HOW TO USE IN APP.CONFIG
========================================
<connectionStrings>
    <add name="DentalClinicDB" 
         connectionString="$clientConnString" 
         providerName="System.Data.SqlClient" />
</connectionStrings>

========================================
NEXT STEPS
========================================
1. Restore database vào SQL Server Express:
   - Mở SQL Server Management Studio (SSMS)
   - Connect tới localhost\SQLEXPRESS
   - Right-click Databases → Restore Database
   - Chọn file: $backupFile

2. Tạo SQL Login:
   - Chạy script: Setup-SQLUser.sql

3. Test connection từ máy khác:
   sqlcmd -S $ipAddress\SQLEXPRESS -U $ClientUser -P $ClientPassword -Q "SELECT @@VERSION"

4. Copy Application tới máy Client và update App.config

========================================
"@ | Out-File -FilePath $configFile -Encoding UTF8

Write-Host "✅ Connection strings saved to: $configFile" -ForegroundColor Green
Write-Host ""

# Step 6: Tạo SQL script để setup user
Write-Host "Step 6: Creating SQL setup script..." -ForegroundColor Yellow

$sqlSetupScript = @"
-- ========================================
-- SETUP SQL USER FOR CLINIC DATABASE
-- ========================================

USE master;
GO

-- Tạo Login nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$ClientUser')
BEGIN
    CREATE LOGIN [$ClientUser] WITH PASSWORD = '$ClientPassword', CHECK_POLICY = OFF;
    PRINT '✅ Login $ClientUser created';
END
ELSE
BEGIN
    PRINT '⚠️  Login $ClientUser already exists';
END
GO

-- Chuyển sang database
USE DentalClinicDB;
GO

-- Tạo User nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '$ClientUser')
BEGIN
    CREATE USER [$ClientUser] FOR LOGIN [$ClientUser];
    PRINT '✅ User $ClientUser created in database';
END
ELSE
BEGIN
    PRINT '⚠️  User $ClientUser already exists in database';
END
GO

-- Cấp quyền
ALTER ROLE db_owner ADD MEMBER [$ClientUser];
PRINT '✅ User $ClientUser added to db_owner role';
GO

-- Verify
SELECT 
    'Server: ' + @@SERVERNAME as Info
UNION ALL
SELECT 
    'Database: ' + DB_NAME()
UNION ALL
SELECT 
    'User: $ClientUser - Permissions: db_owner';
GO

PRINT '';
PRINT '========================================';
PRINT '✅ SETUP COMPLETED!';
PRINT '========================================';
PRINT '';
PRINT 'Connection string for clients:';
PRINT '$clientConnString';
PRINT '';
"@

$sqlSetupScript | Out-File -FilePath "Setup-SQLUser.sql" -Encoding UTF8
Write-Host "✅ SQL script saved to: Setup-SQLUser.sql" -ForegroundColor Green
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "📋 SUMMARY - NEXT STEPS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Backup file created: $backupFile" -ForegroundColor Green
Write-Host "✅ Connection strings saved: $configFile" -ForegroundColor Green
Write-Host "✅ SQL setup script created: Setup-SQLUser.sql" -ForegroundColor Green
Write-Host ""
Write-Host "🔧 MANUAL STEPS REQUIRED:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1️⃣  Cài SQL Server Management Studio (SSMS):" -ForegroundColor White
Write-Host "   https://aka.ms/ssmsfullsetup" -ForegroundColor Cyan
Write-Host ""
Write-Host "2️⃣  Restore database:" -ForegroundColor White
Write-Host "   - Open SSMS → Connect to localhost\SQLEXPRESS" -ForegroundColor Gray
Write-Host "   - Right-click Databases → Restore Database" -ForegroundColor Gray
Write-Host "   - Select file: $backupFile" -ForegroundColor Gray
Write-Host ""
Write-Host "3️⃣  Run SQL setup script:" -ForegroundColor White
Write-Host "   sqlcmd -S localhost\SQLEXPRESS -i Setup-SQLUser.sql" -ForegroundColor Gray
Write-Host ""
Write-Host "4️⃣  Update App.config on ALL computers:" -ForegroundColor White
Write-Host "   - Server: Use localhost\SQLEXPRESS" -ForegroundColor Gray
Write-Host "   - Clients: Use $ipAddress\SQLEXPRESS" -ForegroundColor Gray
Write-Host ""
Write-Host "5️⃣  Test từ máy Client:" -ForegroundColor White
Write-Host "   sqlcmd -S $ipAddress\SQLEXPRESS -U $ClientUser -P $ClientPassword -Q `"SELECT @@VERSION`"" -ForegroundColor Gray
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "Nhấn Enter để thoát"

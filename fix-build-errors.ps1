# Fix Build Errors - Automated Script
# Fixes Logger API and ExecuteScalar usage

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FIXING BUILD ERRORS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# File paths
$invoiceFile = "DataAccess\InvoiceAutoGenerator.cs"
$emailFile = "Services\EmailReminderService.cs"
$apptFile = "DataAccess\AppointmentHelper.cs"

# Backup files
Write-Host "Creating backups..." -ForegroundColor Yellow
Copy-Item $invoiceFile "$invoiceFile.bak" -Force
Copy-Item $emailFile "$emailFile.bak" -Force
Copy-Item $apptFile "$apptFile.bak" -Force
Write-Host "  + Backups created" -ForegroundColor Green
Write-Host ""

# Fix 1: Replace Logger.LogError with Logger.LogAction
Write-Host "Fix 1: Logger.LogError -> Logger.LogAction" -ForegroundColor Yellow
$content = Get-Content $invoiceFile -Raw
$content = $content -replace 'Logger\.LogError\("([^"]+)",\s*([^\)]+)\)', 'Logger.LogAction("$1", $2)'
Set-Content $invoiceFile $content
Write-Host "  + Fixed $invoiceFile" -ForegroundColor Green

$content = Get-Content $emailFile -Raw
$content = $content -replace 'Logger\.LogError\("([^"]+)",\s*([^\)]+)\)', 'Logger.LogAction("$1", $2)'
Set-Content $emailFile $content
Write-Host "  + Fixed $emailFile" -ForegroundColor Green
Write-Host ""

# Fix 2: Replace Logger.LogWarning with Logger.LogAction
Write-Host "Fix 2: Logger.LogWarning -> Logger.LogAction" -ForegroundColor Yellow
$content = Get-Content $emailFile -Raw
$content = $content -replace 'Logger\.LogWarning\("([^"]+)",\s*([^\)]+)\)', 'Logger.LogAction("$1", $2)'
Set-Content $emailFile $content
Write-Host "  + Fixed $emailFile" -ForegroundColor Green
Write-Host ""

# Fix 3: Remove <int> and <decimal> from ExecuteScalar
Write-Host "Fix 3: ExecuteScalar generic -> non-generic" -ForegroundColor Yellow
$content = Get-Content $invoiceFile -Raw
$content = $content -replace 'ExecuteScalar<int>', 'ExecuteScalar'
$content = $content -replace 'ExecuteScalar<decimal>', 'ExecuteScalar'
$content = $content -replace 'ExecuteScalar<int\?>', 'ExecuteScalar'
Set-Content $invoiceFile $content
Write-Host "  + Fixed $invoiceFile" -ForegroundColor Green

$content = Get-Content $emailFile -Raw
$content = $content -replace 'ExecuteScalar<int>', 'ExecuteScalar'
$content = $content -replace 'ExecuteScalar<int\?>', 'ExecuteScalar'
Set-Content $emailFile $content
Write-Host "  + Fixed $emailFile" -ForegroundColor Green

$content = Get-Content $apptFile -Raw
$content = $content -replace 'ExecuteScalar<int>', 'ExecuteScalar'
$content = $content -replace 'ExecuteScalar<int\?>', 'ExecuteScalar'
Set-Content $apptFile $content
Write-Host "  + Fixed $apptFile" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FIXES COMPLETED!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next: Need to manually fix type casting for ExecuteScalar results" -ForegroundColor Yellow
Write-Host "   Example: int result = (int)DatabaseHelper.ExecuteScalar(...)" -ForegroundColor Gray
Write-Host ""
Write-Host "Run build again to check remaining errors" -ForegroundColor Yellow

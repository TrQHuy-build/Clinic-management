# Script test gửi appointment trực tiếp đến API

Write-Host "=== TEST APPOINTMENT SUBMISSION ===" -ForegroundColor Cyan

# Tạo dữ liệu test
$timestamp = Get-Date -Format 'HHmmss'
$appointmentData = @{
    patient_name = "Test User $timestamp"
    phone = "0901234567"
    email = "test@test.com"
    service_id = "1"
    appointment_date = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss")
    status = "booked"
    notes = "Test appointment from PowerShell"
}

Write-Host "Data will send:" -ForegroundColor Yellow
$appointmentData | Format-Table -AutoSize

# Tạo form data
$boundary = [System.Guid]::NewGuid().ToString()
$LF = "`r`n"
$bodyLines = @()

foreach ($key in $appointmentData.Keys) {
    $bodyLines += "--$boundary"
    $bodyLines += "Content-Disposition: form-data; name=`"$key`""
    $bodyLines += ""
    $bodyLines += $appointmentData[$key]
}
$bodyLines += "--$boundary--"

$body = $bodyLines -join $LF

# Gửi request
Write-Host "Sending request to /Home/Book..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest `
        -Uri "https://localhost:7064/Home/Book" `
        -Method POST `
        -ContentType "multipart/form-data; boundary=$boundary" `
        -Body $body `
        -SkipCertificateCheck

    Write-Host "Response Status: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response Body:" -ForegroundColor Green
    $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
    
    # Kiểm tra database
    Write-Host "=== Checking Database ===" -ForegroundColor Cyan
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d DentalClinicDB -Q "SELECT TOP 5 appointment_id, patient_name, phone, appointment_date, status FROM Appointment ORDER BY appointment_id DESC"
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}

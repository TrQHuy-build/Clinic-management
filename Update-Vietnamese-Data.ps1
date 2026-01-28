# Generate SQL UPDATE commands for 104 Vietnamese names
$names = @(
    "Nguyễn Văn An", "Trần Thị Bích", "Lê Minh Cường", "Phạm Thanh Dung",
    "Hoàng Hữu Em", "Huỳnh Bảo Phương", "Vũ Quốc Giang", "Võ Thị Hoa",
    "Phan Anh Hùng", "Trương Kim Khanh", "Bùi Thanh Long", "Đặng Hồng Mai",
    "Đỗ Ngọc Nam", "Ngô Phương Oanh", "Dương Văn Phúc", "Lý Thị Quyên",
    "Hà Minh Sơn", "Đinh Xuân Thảo", "Mai Thu Uyên", "Tô Hà Vy",
    "Nguyễn Thị Lan", "Trần Văn Bình", "Lê Hồng Linh", "Phạm Minh Châu",
    "Hoàng Thị Dung", "Huỳnh Văn Em", "Vũ Bảo Phong", "Võ Thanh Giang",
    "Phan Thị Hải", "Trương Anh Hùng", "Bùi Kim Khoa", "Đặng Quốc Long",
    "Đỗ Thị Minh", "Ngô Văn Nam", "Dương Hồng Oanh", "Lý Ngọc Phúc",
    "Hà Thị Quyên", "Đinh Phương Sơn", "Mai Minh Thảo", "Tô Xuân Uyên",
    "Nguyễn Hà Vy", "Trần Thanh Anh", "Lê Văn Bình", "Phạm Thị Cúc",
    "Hoàng Minh Đạt", "Huỳnh Bảo Em", "Vũ Thị Phương", "Võ Quốc Giang",
    "Phan Anh Hải", "Trương Thị Hoa", "Bùi Kim Khanh", "Đặng Văn Long",
    "Đỗ Hồng Mai", "Ngô Ngọc Minh", "Dương Thị Nam", "Lý Phương Oanh",
    "Hà Văn Phúc", "Đinh Thị Quyên", "Mai Minh Sơn", "Tô Xuân Thảo",
    "Nguyễn Hà Uyên", "Trần Thanh Vy", "Lê Văn Anh", "Phạm Thị Bình",
    "Hoàng Minh Cường", "Huỳnh Bảo Dung", "Vũ Thị Em", "Võ Quốc Phong",
    "Phan Anh Giang", "Trương Thị Hải", "Bùi Kim Hùng", "Đặng Văn Khoa",
    "Đỗ Hồng Long", "Ngô Ngọc Mai", "Dương Thị Minh", "Lý Phương Nam",
    "Hà Văn Oanh", "Đinh Thị Phúc", "Mai Minh Quyên", "Tô Xuân Sơn",
    "Nguyễn Hà Thảo", "Trần Thanh Uyên", "Lê Văn Vy", "Phạm Thị An",
    "Hoàng Minh Bình", "Huỳnh Bảo Cường", "Vũ Thị Dung", "Võ Quốc Em",
    "Phan Anh Phong", "Trương Thị Giang", "Bùi Kim Hải", "Đặng Văn Hùng",
    "Đỗ Hồng Khoa", "Ngô Ngọc Long", "Dương Thị Mai", "Lý Phương Minh",
    "Hà Văn Nam", "Đinh Thị Oanh", "Mai Minh Phúc", "Tô Xuân Quyên",
    "Nguyễn Hà Sơn", "Trần Thanh Thảo", "Lê Văn Uyên", "Phạm Thị Vy"
)

# Get patient user IDs
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True"
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT user_id FROM UserAccount WHERE role = 'patient' ORDER BY user_id"
$reader = $cmd.ExecuteReader()

$userIds = @()
while ($reader.Read()) {
    $userIds += $reader["user_id"]
}
$reader.Close()

Write-Host "Found $($userIds.Count) patient users"
Write-Host "Updating UserAccount with Vietnamese names..."

# Update each user
for ($i = 0; $i -lt $userIds.Count -and $i -lt $names.Count; $i++) {
    $userId = $userIds[$i]
    $name = $names[$i]
    
    $updateCmd = $conn.CreateCommand()
    $updateCmd.CommandText = "UPDATE UserAccount SET fullname = @name WHERE user_id = @id"
    $updateCmd.Parameters.AddWithValue("@name", $name) | Out-Null
    $updateCmd.Parameters.AddWithValue("@id", $userId) | Out-Null
    
    $result = $updateCmd.ExecuteNonQuery()
    
    if (($i + 1) % 10 -eq 0) {
        Write-Host "Updated $($i + 1) / $($userIds.Count) users..."
    }
}

Write-Host "`n✅ Updated $($userIds.Count) UserAccount records!"

# Now update Appointment.patient_name from UserAccount
Write-Host "`nUpdating Appointment.patient_name..."
$updateAppointments = $conn.CreateCommand()
$updateAppointments.CommandText = @"
UPDATE a
SET a.patient_name = u.fullname
FROM Appointment a
INNER JOIN Patient p ON a.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id
WHERE u.role = 'patient'
"@
$appointmentsUpdated = $updateAppointments.ExecuteNonQuery()
Write-Host "✅ Updated $appointmentsUpdated Appointment records!"

# Update notes with Vietnamese
Write-Host "`nUpdating notes with Vietnamese..."
$vietnameseNotes = @(
    "Đau răng hàm dưới bên phải",
    "Khám tổng quát và tư vấn",
    "Làm sạch cao răng",
    "Răng bị sâu cần kiểm tra",
    "Tái khám sau điều trị tủy",
    "Tư vấn niềng răng",
    "Khám định kỳ",
    "Điều trị viêm nướu",
    "Bọc răng sứ",
    "Nhổ răng khôn",
    "Trám răng thẩm mỹ",
    "Tẩy trắng răng",
    "Điều trị tủy răng",
    "Cấy ghép Implant",
    "Làm cầu răng sứ",
    "Chụp X-quang răng",
    "Kiểm tra tổng quát",
    "Vệ sinh răng miệng",
    "Điều trị sâu răng",
    "Tư vấn chỉnh nha"
)

# Get completed appointments
$getAppointments = $conn.CreateCommand()
$getAppointments.CommandText = "SELECT appointment_id FROM Appointment WHERE status = N'Completed' ORDER BY appointment_id"
$apptReader = $getAppointments.ExecuteReader()

$appointmentIds = @()
while ($apptReader.Read()) {
    $appointmentIds += $apptReader["appointment_id"]
}
$apptReader.Close()

Write-Host "Updating notes for $($appointmentIds.Count) completed appointments..."

$notesUpdated = 0
for ($i = 0; $i -lt $appointmentIds.Count; $i++) {
    # Update 33% of appointments with notes
    if ($i % 3 -eq 0) {
        $apptId = $appointmentIds[$i]
        $note = $vietnameseNotes[$i % $vietnameseNotes.Count]
        
        $updateNote = $conn.CreateCommand()
        $updateNote.CommandText = "UPDATE Appointment SET notes = @note WHERE appointment_id = @id"
        $updateNote.Parameters.AddWithValue("@note", $note) | Out-Null
        $updateNote.Parameters.AddWithValue("@id", $apptId) | Out-Null
        
        $updateNote.ExecuteNonQuery() | Out-Null
        $notesUpdated++
    }
    
    if (($i + 1) % 500 -eq 0) {
        Write-Host "Processed $($i + 1) / $($appointmentIds.Count) appointments..."
    }
}

Write-Host "✅ Updated $notesUpdated notes!"

$conn.Close()

Write-Host "`n========================================"
Write-Host "VERIFICATION - Sample data:"
Write-Host "========================================"

# Verify results
$conn.Open()
$verify = $conn.CreateCommand()
$verify.CommandText = @"
SELECT TOP 10
    u.user_id,
    u.fullname AS [Họ tên],
    u.phone AS [SĐT]
FROM UserAccount u
WHERE u.role = 'patient'
ORDER BY u.user_id
"@
$verifyReader = $verify.ExecuteReader()

Write-Host "`nUserAccount (top 10):"
while ($verifyReader.Read()) {
    Write-Host ("  ID: {0,-5} | Tên: {1,-25} | SĐT: {2}" -f $verifyReader["user_id"], $verifyReader["Họ tên"], $verifyReader["SĐT"])
}
$verifyReader.Close()

$verifyAppt = $conn.CreateCommand()
$verifyAppt.CommandText = @"
SELECT TOP 10
    a.appointment_id,
    a.patient_name,
    FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS appointment_time,
    a.notes
FROM Appointment a
WHERE a.notes IS NOT NULL
ORDER BY a.appointment_id DESC
"@
$verifyApptReader = $verifyAppt.ExecuteReader()

Write-Host "`nAppointments with notes (top 10):"
while ($verifyApptReader.Read()) {
    Write-Host ("  ID: {0,-5} | Tên: {1,-20} | Thời gian: {2,-16} | Ghi chú: {3}" -f 
        $verifyApptReader["appointment_id"], 
        $verifyApptReader["patient_name"], 
        $verifyApptReader["appointment_time"],
        $verifyApptReader["notes"])
}
$verifyApptReader.Close()

$conn.Close()

Write-Host "`n========================================"
Write-Host "✅ HOÀN TẤT! BẠN NÊN THẤY TIẾNG VIỆT ĐÚNG Ở TRÊN!"
Write-Host "Bây giờ chạy TestVietnamese.exe hoặc application chính"
Write-Host "========================================"

using System;
using System.Data.SqlClient;
using System.Collections.Generic;

class SeedData
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
        
        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();
            
            // STEP 1: Update UserAccount names
            Console.WriteLine("Step 1: Updating UserAccount with Vietnamese names...");
            string[] names = {
                "Nguyễn Văn An","Trần Thị Bích","Lê Minh Cường","Phạm Thanh Dung",
                "Hoàng Hữu Em","Huỳnh Bảo Phương","Vũ Quốc Giang","Võ Thị Hoa",
                "Phan Anh Hùng","Trương Kim Khanh","Bùi Thanh Long","Đặng Hồng Mai",
                "Đỗ Ngọc Nam","Ngô Phương Oanh","Dương Văn Phúc","Lý Thị Quyên",
                "Hà Minh Sơn","Đinh Xuân Thảo","Mai Thu Uyên","Tô Hà Vy",
                "Nguyễn Thị Lan","Trần Văn Bình","Lê Hồng Linh","Phạm Minh Châu",
                "Hoàng Thị Dung","Huỳnh Văn Em","Vũ Bảo Phong","Võ Thanh Giang",
                "Phan Thị Hải","Trương Anh Hùng","Bùi Kim Khoa","Đặng Quốc Long",
                "Đỗ Thị Minh","Ngô Văn Nam","Dương Hồng Oanh","Lý Ngọc Phúc",
                "Hà Thị Quyên","Đinh Phương Sơn","Mai Minh Thảo","Tô Xuân Uyên",
                "Nguyễn Hà Vy","Trần Thanh Anh","Lê Văn Bình","Phạm Thị Cúc",
                "Hoàng Minh Đạt","Huỳnh Bảo Em","Vũ Thị Phương","Võ Quốc Giang",
                "Phan Anh Hải","Trương Thị Hoa","Bùi Kim Khanh","Đặng Văn Long",
                "Đỗ Hồng Mai","Ngô Ngọc Minh","Dương Thị Nam","Lý Phương Oanh",
                "Hà Văn Phúc","Đinh Thị Quyên","Mai Minh Sơn","Tô Xuân Thảo",
                "Nguyễn Hà Uyên","Trần Thanh Vy","Lê Văn Anh","Phạm Thị Bình",
                "Hoàng Minh Cường","Huỳnh Bảo Dung","Vũ Thị Em","Võ Quốc Phong",
                "Phan Anh Giang","Trương Thị Hải","Bùi Kim Hùng","Đặng Văn Khoa",
                "Đỗ Hồng Long","Ngô Ngọc Mai","Dương Thị Minh","Lý Phương Nam",
                "Hà Văn Oanh","Đinh Thị Phúc","Mai Minh Quyên","Tô Xuân Sơn",
                "Nguyễn Hà Thảo","Trần Thanh Uyên","Lê Văn Vy","Phạm Thị An",
                "Hoàng Minh Bình","Huỳnh Bảo Cường","Vũ Thị Dung","Võ Quốc Em",
                "Phan Anh Phong","Trương Thị Giang","Bùi Kim Hải","Đặng Văn Hùng",
                "Đỗ Hồng Khoa","Ngô Ngọc Long","Dương Thị Mai","Lý Phương Minh",
                "Hà Văn Nam","Đinh Thị Oanh","Mai Minh Phúc","Tô Xuân Quyên",
                "Nguyễn Hà Sơn","Trần Thanh Thảo","Lê Văn Uyên","Phạm Thị Vy"
            };
            
            // Get patient user IDs
            List<int> userIds = new List<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT user_id FROM UserAccount WHERE role = 'patient' ORDER BY user_id", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) userIds.Add((int)reader["user_id"]);
            }
            
            // Update names
            for (int i = 0; i < userIds.Count && i < names.Length; i++)
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE UserAccount SET fullname = @name WHERE user_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@name", names[i]);
                    cmd.Parameters.AddWithValue("@id", userIds[i]);
                    cmd.ExecuteNonQuery();
                }
                if ((i + 1) % 20 == 0) Console.WriteLine("  Updated " + (i + 1) + " users...");
            }
            Console.WriteLine("✅ Updated " + userIds.Count + " UserAccount records");
            
            // STEP 2: Create appointments
            Console.WriteLine("\nStep 2: Creating appointments...");
            
            // Get patient IDs
            List<int> patientIds = new List<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT patient_id FROM Patient", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) patientIds.Add((int)reader["patient_id"]);
            }
            
            // Get service IDs
            List<int> serviceIds = new List<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT service_id FROM Service", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) serviceIds.Add((int)reader["service_id"]);
            }
            
            // Get doctor IDs
            List<int> doctorIds = new List<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT staff_id FROM Staff WHERE position = 'Doctor'", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) doctorIds.Add((int)reader["staff_id"]);
            }
            
            // Vietnamese notes
            string[] notes = {
                "Đau răng hàm dưới bên phải", "Khám tổng quát và tư vấn",
                "Làm sạch cao răng", "Răng bị sâu cần kiểm tra",
                "Tái khám sau điều trị tủy", "Tư vấn niềng răng",
                "Khám định kỳ", "Điều trị viêm nướu",
                "Bọc răng sứ", "Nhổ răng khôn",
                "Trám răng thẩm mỹ", "Tẩy trắng răng",
                "Điều trị tủy răng", "Cấy ghép Implant",
                "Làm cầu răng sứ", "Chụp X-quang răng",
                "Kiểm tra tổng quát", "Vệ sinh răng miệng",
                "Điều trị sâu răng", "Tư vấn chỉnh nha"
            };
            
            Random rand = new Random();
            DateTime startDate = new DateTime(2024, 1, 1);
            DateTime endDate = new DateTime(2026, 1, 31);
            int created = 0;
            
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday) continue;
                
                int appointmentsToday = 10 + rand.Next(11);
                for (int i = 0; i < appointmentsToday; i++)
                {
                    int hour = 8 + (i % 9);
                    int minute = (i % 2) * 30;
                    DateTime apptTime = date.AddHours(hour).AddMinutes(minute);
                    
                    int patientId = patientIds[rand.Next(patientIds.Count)];
                    int serviceId = serviceIds[rand.Next(serviceIds.Count)];
                    int doctorId = doctorIds[rand.Next(doctorIds.Count)];
                    
                    string status;
                    string note = null;
                    int statusRand = rand.Next(100);
                    if (statusRand < 80)
                    {
                        status = "Completed";
                        if (statusRand % 2 == 0) note = notes[rand.Next(notes.Length)];
                    }
                    else if (statusRand < 95) status = "Booked";
                    else
                    {
                        status = "Cancelled";
                        note = "Bệnh nhân hủy lịch";
                    }
                    
                    // Get patient name
                    string patientName = "";
                    using (SqlCommand cmd = new SqlCommand("SELECT u.fullname FROM Patient p INNER JOIN UserAccount u ON p.user_id = u.user_id WHERE p.patient_id = @pid", conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", patientId);
                        patientName = (string)cmd.ExecuteScalar();
                    }
                    
                    // Insert appointment
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Appointment (patient_id, service_id, assigned_doctor_id, appointment_date, status, notes, patient_name)
                        VALUES (@pid, @sid, @did, @date, @status, @notes, @name)", conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", patientId);
                        cmd.Parameters.AddWithValue("@sid", serviceId);
                        cmd.Parameters.AddWithValue("@did", doctorId);
                        cmd.Parameters.AddWithValue("@date", apptTime);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@notes", (object)note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@name", patientName);
                        cmd.ExecuteNonQuery();
                    }
                    
                    created++;
                }
                
                if (date.Day == 1) Console.WriteLine("  Created appointments up to " + date.ToString("MM/yyyy") + ": " + created);
            }
            
            Console.WriteLine("✅ Created " + created + " appointments!");
            
            // STEP 3: Verify
            Console.WriteLine("\n========================================");
            Console.WriteLine("VERIFICATION:");
            Console.WriteLine("========================================");
            
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 10
                    appointment_id,
                    patient_name,
                    FORMAT(appointment_date, 'dd/MM/yyyy HH:mm') AS time,
                    notes,
                    status
                FROM Appointment
                WHERE notes IS NOT NULL
                ORDER BY appointment_id DESC", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["appointment_id"] + 
                                    " | Tên: " + reader["patient_name"] + 
                                    " | Thời gian: " + reader["time"] +
                                    " | Ghi chú: " + reader["notes"]);
                }
            }
            
            Console.WriteLine("\n========================================");
            Console.WriteLine("✅ HOÀN TẤT! NẾU BẠN THẤY TIẾNG VIỆT ĐÚNG");
            Console.WriteLine("   → CHẠY APPLICATION CHÍNH (F5)");
            Console.WriteLine("========================================");
        }
        
        Console.WriteLine("\nNhấn Enter để thoát...");
        Console.ReadLine();
    }
}

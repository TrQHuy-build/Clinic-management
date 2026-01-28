using System;
using System.Data.SqlClient;
using System.Collections.Generic;

class SeedInvoiceAndShift
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
        
        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();
            
            Console.WriteLine("========================================");
            Console.WriteLine("TẠO DỮ LIỆU HÓA ĐƠN VÀ LỊCH TRỰC");
            Console.WriteLine("========================================\n");
            
            // 1. Tạo MedicalRecord cho completed appointments
            Console.WriteLine("1. Creating MedicalRecords...");
            int recordsCreated = CreateMedicalRecords(conn);
            Console.WriteLine("✅ Created " + recordsCreated + " medical records\n");
            
            // 2. Tạo Invoice cho completed appointments
            Console.WriteLine("2. Creating Invoices...");
            int invoicesCreated = CreateInvoices(conn);
            Console.WriteLine("✅ Created " + invoicesCreated + " invoices\n");
            
            // 3. Tạo Shift cho staff
            Console.WriteLine("3. Creating Shifts...");
            int shiftsCreated = CreateShifts(conn);
            Console.WriteLine("✅ Created " + shiftsCreated + " shifts\n");
            
            Console.WriteLine("========================================");
            Console.WriteLine("✅ HOÀN TẤT! KIỂM TRA DỮ LIỆU:");
            Console.WriteLine("========================================\n");
            
            VerifyData(conn);
        }
        
        Console.WriteLine("\nNhấn Enter để thoát...");
        Console.ReadLine();
    }
    
    static int CreateMedicalRecords(SqlConnection conn)
    {
        // Lấy completed appointments chưa có medical record
        List<AppointmentInfo> appointments = new List<AppointmentInfo>();
        
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT a.appointment_id, a.patient_id, a.assigned_doctor_id, a.appointment_date, a.notes
            FROM Appointment a
            WHERE a.status = 'Completed' 
            AND a.assigned_doctor_id IS NOT NULL
            AND NOT EXISTS (SELECT 1 FROM MedicalRecord m WHERE m.patient_id = a.patient_id AND CAST(m.record_date AS DATE) = CAST(a.appointment_date AS DATE))
            ORDER BY a.appointment_date", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                appointments.Add(new AppointmentInfo {
                    AppointmentId = (int)reader["appointment_id"],
                    PatientId = (int)reader["patient_id"],
                    DoctorId = (int)reader["assigned_doctor_id"],
                    AppointmentDate = (DateTime)reader["appointment_date"],
                    Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : null
                });
            }
        }
        
        // Danh sách chẩn đoán và điều trị
        string[] diagnoses = {
            "Sâu răng hàm 6 răng 16",
            "Viêm tủy răng cửa trên",
            "Viêm nướu lan tỏa",
            "Răng khôn mọc lệch",
            "Mất răng hàm 7 dưới",
            "Răng cửa bị mẻ",
            "Răng vàng do tetracycline",
            "Khớp cắn không đều",
            "Nha chu viêm mãn tính",
            "Áp xe quanh chóp răng",
            "Răng hô nhẹ",
            "Mòn cổ răng",
            "Ê buốt răng",
            "Tủy răng hoại tử",
            "Mất men răng"
        };
        
        string[] treatments = {
            "Trám răng composite",
            "Điều trị tủy 3 chân, trám bít ống tủy",
            "Cạo vôi răng, vệ sinh răng miệng, kê thuốc kháng sinh",
            "Nhổ răng khôn, khâu lợi",
            "Tư vấn làm implant hoặc cầu răng",
            "Trám răng thẩm mỹ bằng composite",
            "Tư vấn bọc răng sứ toàn hàm",
            "Tư vấn niềng răng chỉnh nha",
            "Nạo túi lợi, hướng dẫn vệ sinh răng miệng",
            "Rạch thoát mủ, điều trị tủy, kê thuốc",
            "Tư vấn chỉnh nha mắc cài kim loại",
            "Hướng dẫn đánh răng đúng cách, trám composite",
            "Phủ flour, hướng dẫn giảm ê buốt",
            "Lấy tủy, trám bít ống tủy, bọc răng sứ",
            "Tư vấn chế độ ăn, phủ flour"
        };
        
        Random rand = new Random();
        int created = 0;
        
        // Chỉ tạo 50% records để không quá nhiều
        for (int i = 0; i < appointments.Count; i += 2)
        {
            var appt = appointments[i];
            
            string diagnosis = appt.Notes != null ? 
                "Bệnh nhân " + appt.Notes.ToLower() + ". Chẩn đoán: " + diagnoses[rand.Next(diagnoses.Length)] :
                diagnoses[rand.Next(diagnoses.Length)];
                
            string treatment = treatments[rand.Next(treatments.Length)];
            
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment, record_date)
                VALUES (@pid, @sid, @diagnosis, @treatment, @date)", conn))
            {
                cmd.Parameters.AddWithValue("@pid", appt.PatientId);
                cmd.Parameters.AddWithValue("@sid", appt.DoctorId);
                cmd.Parameters.AddWithValue("@diagnosis", diagnosis);
                cmd.Parameters.AddWithValue("@treatment", treatment);
                cmd.Parameters.AddWithValue("@date", appt.AppointmentDate);
                cmd.ExecuteNonQuery();
                created++;
            }
            
            if ((created) % 500 == 0) Console.WriteLine("  Created " + created + " records...");
        }
        
        return created;
    }
    
    static int CreateInvoices(SqlConnection conn)
    {
        // Lấy completed appointments chưa có invoice
        List<InvoiceInfo> invoiceData = new List<InvoiceInfo>();
        
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT 
                a.appointment_id,
                a.patient_id,
                a.assigned_doctor_id as staff_id,
                a.appointment_date,
                s.price,
                m.record_id
            FROM Appointment a
            INNER JOIN Service s ON a.service_id = s.service_id
            LEFT JOIN MedicalRecord m ON m.patient_id = a.patient_id AND CAST(m.record_date AS DATE) = CAST(a.appointment_date AS DATE)
            WHERE a.status = 'Completed'
            AND a.assigned_doctor_id IS NOT NULL
            AND NOT EXISTS (SELECT 1 FROM Invoice i WHERE i.patient_id = a.patient_id AND CAST(i.invoice_date AS DATE) = CAST(a.appointment_date AS DATE))
            ORDER BY a.appointment_date", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                invoiceData.Add(new InvoiceInfo {
                    PatientId = (int)reader["patient_id"],
                    StaffId = (int)reader["staff_id"],
                    InvoiceDate = (DateTime)reader["appointment_date"],
                    TotalAmount = (decimal)reader["price"],
                    MedicalRecordId = reader["record_id"] != DBNull.Value ? (int?)reader["record_id"] : null
                });
            }
        }
        
        string[] paymentMethods = { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng", "Ví điện tử" };
        string[] paymentNotes = {
            "Thanh toán đầy đủ",
            "Đã thanh toán toàn bộ chi phí",
            "Thanh toán qua chuyển khoản ngân hàng",
            "Thanh toán bằng thẻ Visa",
            "Thanh toán qua MoMo",
            "Thanh toán qua ZaloPay",
            "Khách hàng đã thanh toán",
            null,
            null,
            null
        };
        
        Random rand = new Random();
        int created = 0;
        
        foreach (var inv in invoiceData)
        {
            // 95% đã thanh toán, 5% chưa thanh toán
            bool isPaid = rand.Next(100) < 95;
            string status = isPaid ? "paid" : "unpaid";
            DateTime? paidAt = isPaid ? (DateTime?)inv.InvoiceDate.AddMinutes(rand.Next(30, 120)) : null;
            string paymentMethod = isPaid ? paymentMethods[rand.Next(paymentMethods.Length)] : null;
            string paymentNote = isPaid ? paymentNotes[rand.Next(paymentNotes.Length)] : "Chưa thu tiền";
            
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Invoice (patient_id, staff_id, invoice_date, total_amount, status, medical_record_id, payment_method, payment_note, paid_at, is_deleted)
                VALUES (@pid, @sid, @date, @amount, @status, @record_id, @method, @note, @paid_at, 0)", conn))
            {
                cmd.Parameters.AddWithValue("@pid", inv.PatientId);
                cmd.Parameters.AddWithValue("@sid", inv.StaffId);
                cmd.Parameters.AddWithValue("@date", inv.InvoiceDate);
                cmd.Parameters.AddWithValue("@amount", inv.TotalAmount);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@record_id", (object)inv.MedicalRecordId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@method", (object)paymentMethod ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@note", (object)paymentNote ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@paid_at", (object)paidAt ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                created++;
            }
            
            if (created % 500 == 0) Console.WriteLine("  Created " + created + " invoices...");
        }
        
        return created;
    }
    
    static int CreateShifts(SqlConnection conn)
    {
        // Lấy danh sách staff (doctors + staff)
        List<int> staffIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT staff_id FROM Staff ORDER BY staff_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) staffIds.Add((int)reader["staff_id"]);
        }
        
        // Tạo lịch trực từ 2024-01-01 đến 2026-01-31
        DateTime startDate = new DateTime(2024, 1, 1);
        DateTime endDate = new DateTime(2026, 1, 31);
        
        // Các ca trực
        string[][] shifts = {
            new string[] {"07:00:00", "15:00:00"}, // Ca sáng
            new string[] {"13:00:00", "21:00:00"}, // Ca chiều
            new string[] {"08:00:00", "17:00:00"}  // Ca hành chính
        };
        
        Random rand = new Random();
        int created = 0;
        
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Skip Chủ nhật
            if (date.DayOfWeek == DayOfWeek.Sunday) continue;
            
            // Mỗi ngày có 2-3 người trực
            int staffCount = 2 + rand.Next(2); // 2-3 người
            List<int> todayStaff = new List<int>();
            
            // Random chọn staff
            for (int i = 0; i < staffCount; i++)
            {
                int staffId = staffIds[rand.Next(staffIds.Count)];
                if (!todayStaff.Contains(staffId))
                {
                    todayStaff.Add(staffId);
                    
                    // Random chọn ca
                    string[] shift = shifts[rand.Next(shifts.Length)];
                    
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Shift (staff_id, shift_date, start_time, end_time)
                        VALUES (@sid, @date, @start, @end)", conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", staffId);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@start", shift[0]);
                        cmd.Parameters.AddWithValue("@end", shift[1]);
                        cmd.ExecuteNonQuery();
                        created++;
                    }
                }
            }
            
            if (date.Day == 1) Console.WriteLine("  Created shifts up to " + date.ToString("MM/yyyy") + ": " + created + " shifts");
        }
        
        return created;
    }
    
    static void VerifyData(SqlConnection conn)
    {
        // MedicalRecord
        Console.WriteLine("--- MEDICAL RECORDS (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 5 
                m.record_id,
                u.fullname as patient_name,
                m.diagnosis,
                m.treatment,
                FORMAT(m.record_date, 'dd/MM/yyyy') as date
            FROM MedicalRecord m
            INNER JOIN Patient p ON m.patient_id = p.patient_id
            INNER JOIN UserAccount u ON p.user_id = u.user_id
            ORDER BY m.record_date DESC", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  ID: " + reader["record_id"] + " | BN: " + reader["patient_name"]);
                Console.WriteLine("    Chẩn đoán: " + reader["diagnosis"]);
                Console.WriteLine("    Điều trị: " + reader["treatment"]);
                Console.WriteLine("    Ngày: " + reader["date"]);
                Console.WriteLine();
            }
        }
        
        // Invoice
        Console.WriteLine("--- INVOICES (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 5 
                i.invoice_id,
                u.fullname as patient_name,
                i.total_amount,
                CASE i.status 
                    WHEN 'paid' THEN N'Đã thanh toán'
                    WHEN 'unpaid' THEN N'Chưa thanh toán'
                    WHEN 'cancelled' THEN N'Đã hủy'
                END as status,
                i.payment_method,
                FORMAT(i.invoice_date, 'dd/MM/yyyy') as date
            FROM Invoice i
            INNER JOIN Patient p ON i.patient_id = p.patient_id
            INNER JOIN UserAccount u ON p.user_id = u.user_id
            ORDER BY i.invoice_date DESC", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  ID: " + reader["invoice_id"] + " | BN: " + reader["patient_name"]);
                Console.WriteLine("    Số tiền: " + string.Format("{0:N0}", reader["total_amount"]) + " VNĐ");
                Console.WriteLine("    Trạng thái: " + reader["status"]);
                Console.WriteLine("    Thanh toán: " + (reader["payment_method"] != DBNull.Value ? reader["payment_method"] : "Chưa thanh toán"));
                Console.WriteLine("    Ngày: " + reader["date"]);
                Console.WriteLine();
            }
        }
        
        // Shift
        Console.WriteLine("--- SHIFTS (Top 10) ---");
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 10 
                s.shift_id,
                u.fullname as staff_name,
                FORMAT(s.shift_date, 'dd/MM/yyyy') as date,
                CONVERT(VARCHAR(5), s.start_time, 108) as start_time,
                CONVERT(VARCHAR(5), s.end_time, 108) as end_time
            FROM Shift s
            INNER JOIN Staff st ON s.staff_id = st.staff_id
            INNER JOIN UserAccount u ON st.user_id = u.user_id
            ORDER BY s.shift_date DESC", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  " + reader["date"] + " | " + reader["staff_name"] + 
                                " | " + reader["start_time"] + " - " + reader["end_time"]);
            }
        }
        
        // Statistics
        Console.WriteLine("\n--- THỐNG KÊ ---");
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT 
                (SELECT COUNT(*) FROM MedicalRecord) as total_records,
                (SELECT COUNT(*) FROM Invoice) as total_invoices,
                (SELECT COUNT(*) FROM Invoice WHERE status = 'paid') as paid_invoices,
                (SELECT COUNT(*) FROM Shift) as total_shifts", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                Console.WriteLine("  Tổng hồ sơ bệnh án: " + reader["total_records"]);
                Console.WriteLine("  Tổng hóa đơn: " + reader["total_invoices"]);
                Console.WriteLine("  Đã thanh toán: " + reader["paid_invoices"]);
                Console.WriteLine("  Tổng ca trực: " + reader["total_shifts"]);
            }
        }
    }
    
    class AppointmentInfo
    {
        public int AppointmentId;
        public int PatientId;
        public int DoctorId;
        public DateTime AppointmentDate;
        public string Notes;
    }
    
    class InvoiceInfo
    {
        public int PatientId;
        public int StaffId;
        public DateTime InvoiceDate;
        public decimal TotalAmount;
        public int? MedicalRecordId;
    }
}

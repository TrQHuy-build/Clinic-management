using System;
using System.Data.SqlClient;
using System.Text;

namespace FixVietnameseData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
            
            // Danh sách 104 tên tiếng Việt CHUẨN
            string[] vietnameseNames = new string[] {
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
            };
            
            Console.WriteLine("========================================");
            Console.WriteLine("SỬA DỮ LIỆU TIẾNG VIỆT BẰNG C#");
            Console.WriteLine("========================================");
            Console.WriteLine();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Đã kết nối database!");
                    Console.WriteLine();
                    
                    // Bước 1: UPDATE UserAccount
                    Console.WriteLine("BƯỚC 1: Cập nhật UserAccount...");
                    string query1 = @"
                        WITH NumberedUsers AS (
                            SELECT user_id, ROW_NUMBER() OVER (ORDER BY user_id) AS rn
                            FROM UserAccount WHERE role = 'patient'
                        )
                        SELECT user_id, rn FROM NumberedUsers ORDER BY rn";
                    
                    int updated = 0;
                    using (SqlCommand cmd = new SqlCommand(query1, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = (int)reader["user_id"];
                            int rn = (int)reader["rn"];
                            
                            if (rn <= vietnameseNames.Length)
                            {
                                string newName = vietnameseNames[rn - 1];
                                
                                using (SqlConnection conn2 = new SqlConnection(connectionString))
                                {
                                    conn2.Open();
                                    using (SqlCommand updateCmd = new SqlCommand(
                                        "UPDATE UserAccount SET fullname = @name WHERE user_id = @id", conn2))
                                    {
                                        updateCmd.Parameters.AddWithValue("@name", newName);
                                        updateCmd.Parameters.AddWithValue("@id", userId);
                                        updateCmd.ExecuteNonQuery();
                                        updated++;
                                        
                                        if (updated % 20 == 0)
                                            Console.WriteLine("  Đã cập nhật " + updated + "/104 users...");
                                    }
                                }
                            }
                        }
                    }
                    
                    Console.WriteLine("✅ Đã cập nhật " + updated + " UserAccount records");
                    Console.WriteLine();
                    
                    // Bước 2: UPDATE Appointment.patient_name
                    Console.WriteLine("BƯỚC 2: Cập nhật Appointment.patient_name...");
                    string query2 = @"
                        UPDATE a SET a.patient_name = u.fullname
                        FROM Appointment a
                        INNER JOIN Patient p ON a.patient_id = p.patient_id
                        INNER JOIN UserAccount u ON p.user_id = u.user_id
                        WHERE u.role = 'patient'";
                    
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        int appointmentsUpdated = cmd.ExecuteNonQuery();
                        Console.WriteLine("✅ Đã cập nhật " + appointmentsUpdated + " Appointment records");
                    }
                    
                    Console.WriteLine();
                    
                    // Bước 3: UPDATE notes
                    Console.WriteLine("BƯỚC 3: Cập nhật notes...");
                    string[] vietnameseNotes = new string[] {
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
                    };
                    
                    string query3 = @"
                        SELECT appointment_id
                        FROM Appointment
                        WHERE status = N'Completed'
                        ORDER BY appointment_id";
                    
                    int notesUpdated = 0;
                    int index = 0;
                    using (SqlCommand cmd = new SqlCommand(query3, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int appointmentId = (int)reader["appointment_id"];
                            
                            // 33% appointments có notes
                            if (index % 3 == 0)
                            {
                                string note = vietnameseNotes[index % vietnameseNotes.Length];
                                
                                using (SqlConnection conn3 = new SqlConnection(connectionString))
                                {
                                    conn3.Open();
                                    using (SqlCommand updateCmd = new SqlCommand(
                                        "UPDATE Appointment SET notes = @note WHERE appointment_id = @id", conn3))
                                    {
                                        updateCmd.Parameters.AddWithValue("@note", note);
                                        updateCmd.Parameters.AddWithValue("@id", appointmentId);
                                        updateCmd.ExecuteNonQuery();
                                        notesUpdated++;
                                        
                                        if (notesUpdated % 500 == 0)
                                            Console.WriteLine("  Đã cập nhật " + notesUpdated + " notes...");
                                    }
                                }
                            }
                            
                            index++;
                        }
                    }
                    
                    Console.WriteLine("✅ Đã cập nhật " + notesUpdated + " notes");
                    Console.WriteLine();
                    
                    Console.WriteLine("========================================");
                    Console.WriteLine("HOÀN TẤT!");
                    Console.WriteLine("========================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LỖI: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine();
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
    }
}

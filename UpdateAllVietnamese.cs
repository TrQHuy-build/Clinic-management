using System;
using System.Data.SqlClient;
using System.Collections.Generic;

class UpdateAllVietnamese
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
        
        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();
            
            Console.WriteLine("========================================");
            Console.WriteLine("CẬP NHẬT TOÀN BỘ DATABASE VỚI TIẾNG VIỆT");
            Console.WriteLine("========================================\n");
            
            // 1. UPDATE INVENTORY
            Console.WriteLine("1. Updating Inventory...");
            UpdateInventory(conn);
            
            // 2. UPDATE PATIENT ADDRESSES
            Console.WriteLine("\n2. Updating Patient addresses...");
            UpdatePatientAddresses(conn);
            
            // 3. UPDATE SALARY NOTES
            Console.WriteLine("\n3. Updating Salary notes...");
            UpdateSalaryNotes(conn);
            
            // 4. UPDATE SERVICES
            Console.WriteLine("\n4. Updating Services...");
            UpdateServices(conn);
            
            // 5. UPDATE STAFF NAMES (nếu cần)
            Console.WriteLine("\n5. Updating Staff names...");
            UpdateStaffNames(conn);
            
            Console.WriteLine("\n========================================");
            Console.WriteLine("✅ HOÀN TẤT! TẤT CẢ DỮ LIỆU ĐÃ CẬP NHẬT");
            Console.WriteLine("========================================\n");
            
            // VERIFICATION
            Console.WriteLine("KIỂM TRA MẪU DỮ LIỆU:\n");
            VerifyData(conn);
        }
        
        Console.WriteLine("\nNhấn Enter để thoát...");
        Console.ReadLine();
    }
    
    static void UpdateInventory(SqlConnection conn)
    {
        // Lấy tất cả inventory items
        List<int> inventoryIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT item_id FROM Inventory ORDER BY item_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) inventoryIds.Add((int)reader["item_id"]);
        }
        
        // Danh sách items nha khoa
        string[] itemNames = {
            "Gương nha khoa", "Kìm nhổ răng", "Máy khoan răng", "Đèn chiếu nha khoa",
            "Ghế nha khoa điện", "Máy làm sạch siêu âm", "Kẹp nha khoa", "Thìa trộn amalgam",
            "Kim tiêm nha khoa", "Bông y tế vô trùng", "Găng tay y tế", "Khẩu trang y tế",
            "Xi măng nha khoa", "Composite trám răng", "Vật liệu lấy dấu", "Vật liệu đắp tủy",
            "Thuốc tê Lidocaine", "Thuốc tê Articaine", "Gel bôi tê bề mặt", "Dung dịch súc miệng",
            "Chỉ nha khoa", "Kim khâu nha khoa", "Băng gạc vô trùng", "Bông gòn y tế",
            "Que khuấy hỗn hợp", "Cốc đựng dụng cụ", "Khay đựng dụng cụ", "Giấy lau tay",
            "Dung dịch khử trùng", "Túi tiệt trùng", "Băng dính y tế", "Ống hút nước bọt",
            "Đĩa trộn xi măng", "Giấy articulating", "Matrice răng", "Nêm gỗ nha khoa",
            "Dao cạo vôi răng", "Que đánh bóng răng", "Bàn chải vệ sinh", "Khăn trải bệnh nhân"
        };
        
        string[] suppliers = {
            "Công ty TNHH Thiết Bị Y Tế Việt", "Công ty Vật Tư Nha Khoa Sài Gòn",
            "Công ty CP Dụng Cụ Y Tế Hà Nội", "Công ty Nhập Khẩu Y Tế Phương Đông",
            "Công ty TNHH Nha Khoa Đức Minh", "Công ty CP Thiết Bị Nha Khoa Á Châu",
            "Công ty Vật Liệu Y Tế Thành Đạt", "Công ty TNHH Dụng Cụ Nha Khoa Quốc Tế",
            "Công ty CP Y Tế Phú Thọ", "Công ty TNHH Thiết Bị Medic Việt Nam"
        };
        
        string[] units = {
            "Cái", "Bộ", "Hộp", "Túi", "Chai", "Lọ", "Gói", "Cuộn", "Chiếc", "Bịch"
        };
        
        int updated = 0;
        for (int i = 0; i < inventoryIds.Count; i++)
        {
            string itemName = itemNames[i % itemNames.Length];
            string supplier = suppliers[i % suppliers.Length];
            string unit = units[i % units.Length];
            
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Inventory 
                SET item_name = @name, 
                    supplier = @supplier, 
                    unit = @unit
                WHERE item_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@name", itemName);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@unit", unit);
                cmd.Parameters.AddWithValue("@id", inventoryIds[i]);
                cmd.ExecuteNonQuery();
                updated++;
            }
            
            if ((i + 1) % 10 == 0) Console.WriteLine("  Updated " + (i + 1) + " items...");
        }
        
        Console.WriteLine("✅ Updated " + updated + " Inventory items");
    }
    
    static void UpdatePatientAddresses(SqlConnection conn)
    {
        List<int> patientIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT patient_id FROM Patient ORDER BY patient_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) patientIds.Add((int)reader["patient_id"]);
        }
        
        string[] streets = {
            "Nguyễn Huệ", "Lê Lợi", "Trần Hưng Đạo", "Hai Bà Trưng", "Lý Thường Kiệt",
            "Nguyễn Thị Minh Khai", "Điện Biên Phủ", "Võ Văn Tần", "Pasteur", "Cách Mạng Tháng 8",
            "Ba Tháng Hai", "Nguyễn Trãi", "Phạm Ngũ Lão", "Bùi Viện", "Đề Thám"
        };
        
        string[] districts = {
            "Quận 1", "Quận 2", "Quận 3", "Quận 4", "Quận 5",
            "Quận 6", "Quận 7", "Quận 8", "Quận 10", "Quận 11",
            "Quận Bình Thạnh", "Quận Phú Nhuận", "Quận Tân Bình", "Quận Gò Vấp"
        };
        
        int updated = 0;
        for (int i = 0; i < patientIds.Count; i++)
        {
            string street = streets[i % streets.Length];
            string district = districts[i % districts.Length];
            int houseNum = 10 + (i * 7) % 500;
            string address = houseNum + " " + street + ", " + district + ", TP.HCM";
            
            using (SqlCommand cmd = new SqlCommand("UPDATE Patient SET address = @addr WHERE patient_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@addr", address);
                cmd.Parameters.AddWithValue("@id", patientIds[i]);
                cmd.ExecuteNonQuery();
                updated++;
            }
            
            if ((i + 1) % 20 == 0) Console.WriteLine("  Updated " + (i + 1) + " addresses...");
        }
        
        Console.WriteLine("✅ Updated " + updated + " Patient addresses");
    }
    
    static void UpdateSalaryNotes(SqlConnection conn)
    {
        List<int> salaryIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT salary_id FROM Salary ORDER BY salary_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) salaryIds.Add((int)reader["salary_id"]);
        }
        
        string[] notes = {
            "Lương tháng đầy đủ",
            "Có thưởng thành tích",
            "Tăng ca cuối tuần",
            "Đạt KPI tháng",
            "Thưởng dự án hoàn thành",
            "Lương cơ bản",
            "Có phụ cấp đi lại",
            "Thưởng lễ Tết",
            "Nghỉ phép có lương",
            "Đạt doanh số cao"
        };
        
        int updated = 0;
        for (int i = 0; i < salaryIds.Count; i++)
        {
            string note = notes[i % notes.Length];
            
            using (SqlCommand cmd = new SqlCommand("UPDATE Salary SET notes = @note WHERE salary_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@note", note);
                cmd.Parameters.AddWithValue("@id", salaryIds[i]);
                cmd.ExecuteNonQuery();
                updated++;
            }
        }
        
        Console.WriteLine("✅ Updated " + updated + " Salary records");
    }
    
    static void UpdateServices(SqlConnection conn)
    {
        // Lấy danh sách services hiện tại
        List<int> serviceIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT service_id FROM Service ORDER BY service_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) serviceIds.Add((int)reader["service_id"]);
        }
        
        // Danh sách dịch vụ nha khoa chi tiết
        string[][] services = {
            new string[] {"Khám tổng quát", "Kiểm tra sức khỏe răng miệng toàn diện, phát hiện các vấn đề tiềm ẩn"},
            new string[] {"Làm sạch cao răng", "Loại bỏ cao răng và mảng bám, làm sạch răng miệng"},
            new string[] {"Trám răng sâu", "Điều trị và hàn trám răng bị sâu, phục hồi chức năng ăn nhai"},
            new string[] {"Nhổ răng", "Nhổ răng khôn, răng sữa hoặc răng vĩnh viễn bị hỏng"},
            new string[] {"Điều trị tủy răng", "Điều trị viêm tủy, hoại tử tủy, lấy tủy răng"},
            new string[] {"Bọc răng sứ", "Phục hồi thẩm mỹ răng bằng mão sứ, cải thiện nụ cười"},
            new string[] {"Cấy ghép Implant", "Cấy ghép răng giả vào xương hàm, thay thế răng mất"},
            new string[] {"Niềng răng", "Chỉnh nha, điều chỉnh răng khấp khểnh, móm, hô"},
            new string[] {"Tẩy trắng răng", "Làm trắng răng bằng công nghệ hiện đại, an toàn"},
            new string[] {"Điều trị viêm nướu", "Chữa trị viêm lợi, viêm nha chu, chảy máu chân răng"},
            new string[] {"Phẫu thuật hàm mặt", "Phẫu thuật chỉnh hình hàm, điều trị khớp thái dương hàm"},
            new string[] {"Nhổ răng khôn", "Nhổ răng khôn mọc lệch, mọc ngầm gây đau"},
            new string[] {"Làm cầu răng", "Phục hồi răng mất bằng cầu răng cố định"},
            new string[] {"Hàm giả tháo lắp", "Làm hàm giả di động cho người mất nhiều răng"},
            new string[] {"Chụp X-quang răng", "Chụp phim toàn cảnh, phim sọ nghiêng để chẩn đoán"},
            new string[] {"Điều trị tủy lại", "Điều trị lại răng đã lấy tủy nhưng bị tái phát"},
            new string[] {"Phục hồi răng sứ", "Làm mặt dán sứ Veneer, làm đẹp răng"},
            new string[] {"Trám răng thẩm mỹ", "Trám răng bằng composite màu răng, tự nhiên"},
            new string[] {"Nạo túi lợi", "Điều trị viêm nha chu, làm sạch túi lợi sâu"},
            new string[] {"Phủ Flour cho trẻ em", "Phòng ngừa sâu răng cho trẻ nhỏ bằng Flour"},
            new string[] {"Hàn trám Amalgam", "Trám răng bằng hỗn hợp amalgam bền chắc"},
            new string[] {"Điều trị tủy 1 chân", "Lấy tủy và trám bít ống tủy răng 1 chân"},
            new string[] {"Điều trị tủy 2 chân", "Lấy tủy và trám bít ống tủy răng 2 chân"},
            new string[] {"Điều trị tủy 3 chân", "Lấy tủy và trám bít ống tủy răng 3 chân"},
            new string[] {"Bọc răng kim loại", "Làm mão răng kim loại, bền chắc giá rẻ"},
            new string[] {"Làm răng giả Implant", "Gắn răng sứ lên trụ Implant đã cấy"},
            new string[] {"Chỉnh nha mắc cài", "Niềng răng bằng mắc cài kim loại hoặc sứ"},
            new string[] {"Chỉnh nha trong suốt", "Niềng răng bằng khay nhựa trong suốt Invisalign"},
            new string[] {"Phẫu thuật cười hở lợi", "Chỉnh sửa nụ cười lộ quá nhiều lợi"},
            new string[] {"Cắt lợi tạo hình", "Chỉnh sửa đường viền lợi cho đẹp"},
            new string[] {"Ghép xương hàm", "Tăng xương hàm trước khi cấy Implant"},
            new string[] {"Nâng xoang hàm", "Nâng đáy xoang hàm để đủ xương cấy ghép"},
            new string[] {"Tư vấn nha khoa", "Tư vấn các phương án điều trị phù hợp"},
            new string[] {"Chụp CT Cone Beam", "Chụp CT 3D răng hàm mặt để chẩn đoán chính xác"},
            new string[] {"Tư vấn chỉnh nha", "Tư vấn phương án niềng răng, thời gian chi phí"}
        };
        
        int updated = 0;
        for (int i = 0; i < serviceIds.Count && i < services.Length; i++)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Service 
                SET service_name = @name, 
                    description = @desc
                WHERE service_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@name", services[i][0]);
                cmd.Parameters.AddWithValue("@desc", services[i][1]);
                cmd.Parameters.AddWithValue("@id", serviceIds[i]);
                cmd.ExecuteNonQuery();
                updated++;
            }
            
            if ((i + 1) % 10 == 0) Console.WriteLine("  Updated " + (i + 1) + " services...");
        }
        
        Console.WriteLine("✅ Updated " + updated + " Services");
    }
    
    static void UpdateStaffNames(SqlConnection conn)
    {
        // Cập nhật tên staff (admin, doctor, staff)
        string[] staffNames = {
            "Admin Quản Trị Viên",
            "BS. Nguyễn Văn Hùng",
            "BS. Trần Thị Mai",
            "BS. Lê Quốc Đạt",
            "NV. Phạm Thị Lan",
            "NV. Hoàng Minh Tuấn"
        };
        
        List<int> userIds = new List<int>();
        using (SqlCommand cmd = new SqlCommand("SELECT user_id FROM UserAccount WHERE role IN ('admin', 'doctor', 'staff') ORDER BY user_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) userIds.Add((int)reader["user_id"]);
        }
        
        int updated = 0;
        for (int i = 0; i < userIds.Count && i < staffNames.Length; i++)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE UserAccount SET fullname = @name WHERE user_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@name", staffNames[i]);
                cmd.Parameters.AddWithValue("@id", userIds[i]);
                cmd.ExecuteNonQuery();
                updated++;
            }
        }
        
        Console.WriteLine("✅ Updated " + updated + " Staff names");
    }
    
    static void VerifyData(SqlConnection conn)
    {
        // Inventory
        Console.WriteLine("--- INVENTORY (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 item_name, supplier, unit FROM Inventory", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  - " + reader["item_name"] + " | NCC: " + reader["supplier"] + " | ĐVT: " + reader["unit"]);
            }
        }
        
        // Patient
        Console.WriteLine("\n--- PATIENT ADDRESSES (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 u.fullname, p.address FROM Patient p INNER JOIN UserAccount u ON p.user_id = u.user_id", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  - " + reader["fullname"] + " | " + reader["address"]);
            }
        }
        
        // Salary
        Console.WriteLine("\n--- SALARY (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 s.notes, s.base_salary FROM Salary s WHERE notes IS NOT NULL", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  - " + reader["notes"] + " | Lương: " + reader["base_salary"]);
            }
        }
        
        // Service
        Console.WriteLine("\n--- SERVICES (Top 5) ---");
        using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 service_name, description FROM Service", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  - " + reader["service_name"]);
                Console.WriteLine("    → " + reader["description"]);
            }
        }
        
        // Staff
        Console.WriteLine("\n--- STAFF ---");
        using (SqlCommand cmd = new SqlCommand("SELECT fullname, role FROM UserAccount WHERE role IN ('admin', 'doctor', 'staff')", conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine("  - " + reader["fullname"] + " (" + reader["role"] + ")");
            }
        }
    }
}

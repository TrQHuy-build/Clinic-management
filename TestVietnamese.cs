using System;
using System.Data;
using System.Data.SqlClient;

namespace TestVietnamese
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console to UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
            
            Console.WriteLine("========================================");
            Console.WriteLine("TEST TIẾNG VIỆT TRONG DATABASE");
            Console.WriteLine("========================================");
            Console.WriteLine();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    string query = @"
                        SELECT TOP 10 
                            appointment_id,
                            patient_name,
                            notes,
                            FORMAT(appointment_date, 'dd/MM/yyyy HH:mm') AS appointment_time
                        FROM Appointment
                        WHERE notes IS NOT NULL
                        ORDER BY appointment_id DESC";
                    
                    int count = 0;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count++;
                            Console.WriteLine("--- Appointment #" + count + " ---");
                            Console.WriteLine("ID: " + reader["appointment_id"]);
                            Console.WriteLine("Bệnh nhân: " + reader["patient_name"]);
                            Console.WriteLine("Thời gian: " + reader["appointment_time"]);
                            Console.WriteLine("Ghi chú: " + reader["notes"]);
                            Console.WriteLine();
                        }
                    }
                    
                    Console.WriteLine("========================================");
                    Console.WriteLine("✅ HIỂN THỊ " + count + " APPOINTMENTS");
                    Console.WriteLine("========================================");
                    Console.WriteLine();
                    Console.WriteLine("NẾU BẠN THẤY TIẾNG VIỆT ĐÚNG Ở ĐÂY");
                    Console.WriteLine("→ DATABASE ĐÃ ĐÚNG 100%!");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LỖI: " + ex.Message);
            }
            
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
    }
}

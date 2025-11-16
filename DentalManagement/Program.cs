using System;
using System.Windows.Forms;
using DentalClinicManagement.Forms;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Test database connection
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show(
                    "Không thể kết nối database!\n\n" +
                    "Vui lòng kiểm tra:\n" +
                    "1. SQL Server đã chạy chưa\n" +
                    "2. Connection string trong DatabaseHelper.cs\n" +
                    "3. Database 'DentalClinicDB' đã tạo chưa",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Run login form
            Application.Run(new frmLogin());
        }
    }
}
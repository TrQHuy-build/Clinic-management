using System;
using System.Windows.Forms;
using DentalClinicManagement.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

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
            // ✅ Setup global exception handlers
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ErrorHandler.HandleUIThreadException;
            AppDomain.CurrentDomain.UnhandledException += ErrorHandler.HandleUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cleanup old logs (keep last 30 days)
            ErrorHandler.CleanupOldLogs(30);

            // Validate configuration first
            if (!ConfigHelper.ValidateConfiguration())
            {
                MessageBox.Show(
                    "Lỗi đọc configuration từ App.config!\n\n" +
                    "Vui lòng kiểm tra:\n" +
                    "1. File App.config tồn tại\n" +
                    "2. ConnectionStrings section đã được cấu hình\n" +
                    "3. AppSettings đã được cấu hình đúng",
                    "Lỗi Configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Test database connection
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show(
                    "Không thể kết nối database!\n\n" +
                    "Vui lòng kiểm tra:\n" +
                    "1. SQL Server đã chạy chưa\n" +
                    "2. Connection string trong App.config\n" +
                    "3. Database 'DentalClinicDB' đã tạo chưa\n\n" +
                    $"Connection: {ConfigHelper.GetConnectionString()}",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Log application start
            ErrorHandler.LogMessage("Application started successfully", ErrorLevel.INFO);

            // Run login form
            try
            {
                Application.Run(new frmLogin());
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "Application.Run failed", showToUser: true);
            }
            finally
            {
                ErrorHandler.LogMessage("Application shutdown", ErrorLevel.INFO);
            }
        }
    }
}
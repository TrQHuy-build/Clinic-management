using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Centralized error handling and logging system
    /// ✅ File-based logging thay thế Console.WriteLine
    /// ✅ Windows Event Log cho production
    /// ✅ User-friendly error messages
    /// ✅ Detailed error tracking với stack trace
    /// </summary>
    public static class ErrorHandler
    {
        private static readonly string LogDirectory;
        private static readonly string LogFilePath;
        private static readonly object FileLock = new object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Static constructor - Khởi tạo log directory
        /// </summary>
        static ErrorHandler()
        {
            try
            {
                // Lấy đường dẫn từ App.config hoặc sử dụng default
                string configuredPath = ConfigHelper.GetAppSetting("LogFilePath", "");
                
                if (!string.IsNullOrEmpty(configuredPath))
                {
                    LogDirectory = Path.GetDirectoryName(configuredPath);
                    LogFilePath = configuredPath;
                }
                else
                {
                    // Default: Application folder/Logs
                    LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    string datePrefix = DateTime.Now.ToString("yyyyMMdd");
                    LogFilePath = Path.Combine(LogDirectory, $"error_{datePrefix}.log");
                }

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                _isInitialized = true;
            }
            catch
            {
                // Fallback nếu không thể tạo log directory
                LogDirectory = Path.GetTempPath();
                LogFilePath = Path.Combine(LogDirectory, "dental_clinic_error.log");
                _isInitialized = false;
            }
        }

        #region Public Methods

        /// <summary>
        /// Log exception với level ERROR
        /// Sử dụng khi có lỗi nghiêm trọng cần ghi lại
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <param name="context">Context/location where error occurred</param>
        /// <param name="showToUser">Có hiển thị thông báo cho user không</param>
        public static void LogError(Exception ex, string context = "", bool showToUser = false)
        {
            LogException(ex, ErrorLevel.ERROR, context, showToUser);
        }

        /// <summary>
        /// Log exception với level WARNING
        /// Sử dụng cho các lỗi không nghiêm trọng
        /// </summary>
        public static void LogWarning(Exception ex, string context = "", bool showToUser = false)
        {
            LogException(ex, ErrorLevel.WARNING, context, showToUser);
        }

        /// <summary>
        /// Log message đơn giản (không phải exception)
        /// </summary>
        /// <param name="message">Nội dung message</param>
        /// <param name="level">Error level</param>
        public static void LogMessage(string message, ErrorLevel level = ErrorLevel.INFO)
        {
            try
            {
                string logEntry = FormatLogMessage(message, level);
                WriteToFile(logEntry);

                // Ghi vào Windows Event Log nếu là ERROR hoặc CRITICAL
                if (level >= ErrorLevel.ERROR)
                {
                    WriteToEventLog(message, level);
                }
            }
            catch
            {
                // Silent fail - không làm crash app khi log thất bại
            }
        }

        /// <summary>
        /// Handle unhandled exception (dùng trong Program.cs)
        /// </summary>
        public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogError(ex, "UNHANDLED EXCEPTION", true);
                
                // Hiển thị message box cho user
                MessageBox.Show(
                    "Đã xảy ra lỗi nghiêm trọng!\n\n" +
                    $"Chi tiết: {ex.Message}\n\n" +
                    $"Log đã được lưu tại: {LogFilePath}",
                    "Lỗi Hệ Thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            // Terminate application nếu là fatal error
            if (e.IsTerminating)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Handle UI thread exceptions (dùng trong Program.cs)
        /// </summary>
        public static void HandleUIThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception, "UI THREAD EXCEPTION", true);

            DialogResult result = MessageBox.Show(
                "Đã xảy ra lỗi trong ứng dụng!\n\n" +
                $"Chi tiết: {e.Exception.Message}\n\n" +
                "Bạn có muốn tiếp tục sử dụng không?",
                "Lỗi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error
            );

            if (result == DialogResult.No)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Lấy đường dẫn file log hiện tại
        /// </summary>
        public static string GetLogFilePath()
        {
            return LogFilePath;
        }

        /// <summary>
        /// Kiểm tra xem error logging có được bật không
        /// </summary>
        public static bool IsEnabled()
        {
            return ConfigHelper.GetBoolSetting("EnableLogging", true);
        }

        /// <summary>
        /// Xóa các file log cũ (giữ lại 30 ngày gần nhất)
        /// </summary>
        public static void CleanupOldLogs(int daysToKeep = 30)
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                    return;

                DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                string[] logFiles = Directory.GetFiles(LogDirectory, "error_*.log");

                foreach (string file in logFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Silent fail
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Core method để log exception
        /// </summary>
        private static void LogException(Exception ex, ErrorLevel level, string context, bool showToUser)
        {
            if (!IsEnabled())
                return;

            try
            {
                // Format log entry
                string logEntry = FormatExceptionLog(ex, level, context);

                // Write to file
                WriteToFile(logEntry);

                // Write to Windows Event Log (only for ERROR and CRITICAL)
                if (level >= ErrorLevel.ERROR)
                {
                    WriteToEventLog(ex, level, context);
                }

                // Show to user if requested
                if (showToUser)
                {
                    ShowErrorToUser(ex, level);
                }
            }
            catch
            {
                // Silent fail - không làm crash app khi log thất bại
                // Fallback: Thử ghi vào Temp directory
                try
                {
                    string fallbackPath = Path.Combine(Path.GetTempPath(), "dental_clinic_error.log");
                    File.AppendAllText(fallbackPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}\n");
                }
                catch
                {
                    // Give up silently
                }
            }
        }

        /// <summary>
        /// Format exception thành log entry
        /// </summary>
        private static string FormatExceptionLog(Exception ex, ErrorLevel level, string context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine($"[{level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Context: {context}");
            sb.AppendLine($"User: {(Auth.IsAuthenticated() ? Auth.CurrentUserName : "Not authenticated")}");
            sb.AppendLine("───────────────────────────────────────────────────────────");
            sb.AppendLine($"Exception: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
            }

            sb.AppendLine("Stack Trace:");
            sb.AppendLine(ex.StackTrace ?? "N/A");
            
            // Include inner exception stack trace
            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner Exception Stack Trace:");
                sb.AppendLine(ex.InnerException.StackTrace ?? "N/A");
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Format simple message log
        /// </summary>
        private static string FormatLogMessage(string message, ErrorLevel level)
        {
            return $"[{level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";
        }

        /// <summary>
        /// Ghi log vào file (thread-safe)
        /// </summary>
        private static void WriteToFile(string logEntry)
        {
            if (!_isInitialized)
                return;

            lock (FileLock)
            {
                try
                {
                    // Kiểm tra kích thước file (rotate nếu > 10MB)
                    FileInfo fileInfo = new FileInfo(LogFilePath);
                    if (fileInfo.Exists && fileInfo.Length > 10 * 1024 * 1024) // 10MB
                    {
                        RotateLogFile();
                    }

                    // Append log entry
                    File.AppendAllText(LogFilePath, logEntry, Encoding.UTF8);
                }
                catch
                {
                    // Silent fail
                }
            }
        }

        /// <summary>
        /// Rotate log file khi quá lớn
        /// </summary>
        private static void RotateLogFile()
        {
            try
            {
                string backupPath = LogFilePath.Replace(".log", $"_{DateTime.Now:HHmmss}.log");
                File.Move(LogFilePath, backupPath);
            }
            catch
            {
                // Silent fail
            }
        }

        /// <summary>
        /// Ghi vào Windows Event Log (chỉ cho ERROR và CRITICAL)
        /// </summary>
        private static void WriteToEventLog(Exception ex, ErrorLevel level, string context)
        {
            try
            {
                string sourceName = "DentalClinicManagement";
                string message = $"Context: {context}\nError: {ex.Message}\nStack: {ex.StackTrace}";

                EventLogEntryType entryType = level == ErrorLevel.CRITICAL
                    ? EventLogEntryType.Error
                    : EventLogEntryType.Warning;

                // Kiểm tra xem source đã tồn tại chưa
                if (!EventLog.SourceExists(sourceName))
                {
                    // Cần quyền admin để tạo source - skip nếu không có
                    return;
                }

                EventLog.WriteEntry(sourceName, message, entryType);
            }
            catch
            {
                // Silent fail - không có quyền ghi Event Log
            }
        }

        /// <summary>
        /// Overload cho simple message
        /// </summary>
        private static void WriteToEventLog(string message, ErrorLevel level)
        {
            try
            {
                string sourceName = "DentalClinicManagement";
                EventLogEntryType entryType = level == ErrorLevel.CRITICAL
                    ? EventLogEntryType.Error
                    : EventLogEntryType.Warning;

                if (!EventLog.SourceExists(sourceName))
                {
                    return;
                }

                EventLog.WriteEntry(sourceName, message, entryType);
            }
            catch
            {
                // Silent fail
            }
        }

        /// <summary>
        /// Hiển thị lỗi cho user với message thân thiện
        /// </summary>
        private static void ShowErrorToUser(Exception ex, ErrorLevel level)
        {
            string title = level == ErrorLevel.CRITICAL ? "Lỗi Nghiêm Trọng" : "Thông Báo Lỗi";
            MessageBoxIcon icon = level == ErrorLevel.CRITICAL ? MessageBoxIcon.Error : MessageBoxIcon.Warning;

            string message = GetUserFriendlyMessage(ex);

            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                icon
            );
        }

        /// <summary>
        /// Chuyển đổi technical error thành user-friendly message
        /// </summary>
        private static string GetUserFriendlyMessage(Exception ex)
        {
            // Database errors
            if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case -1: // Timeout
                        return "Kết nối cơ sở dữ liệu bị timeout.\nVui lòng thử lại sau.";
                    case -2: // Cannot connect
                        return "Không thể kết nối đến cơ sở dữ liệu.\nVui lòng kiểm tra kết nối mạng.";
                    case 2627: // Primary key violation
                        return "Dữ liệu bị trùng lặp.\nVui lòng kiểm tra lại thông tin.";
                    case 547: // Foreign key violation
                        return "Không thể xóa do dữ liệu đang được sử dụng ở nơi khác.";
                    default:
                        return $"Lỗi cơ sở dữ liệu.\nMã lỗi: {sqlEx.Number}\nChi tiết: {sqlEx.Message}";
                }
            }

            // File I/O errors
            if (ex is IOException)
            {
                return "Lỗi đọc/ghi file.\nVui lòng kiểm tra quyền truy cập file.";
            }

            // Network errors
            if (ex is System.Net.WebException)
            {
                return "Lỗi kết nối mạng.\nVui lòng kiểm tra kết nối internet.";
            }

            // Validation errors
            if (ex is ArgumentException || ex is FormatException)
            {
                return $"Dữ liệu không hợp lệ.\n{ex.Message}";
            }

            // Unauthorized access
            if (ex is UnauthorizedAccessException)
            {
                return "Bạn không có quyền thực hiện thao tác này.";
            }

            // Generic error
            return $"Đã xảy ra lỗi!\n\nChi tiết: {ex.Message}\n\nVui lòng liên hệ quản trị viên nếu lỗi tiếp tục xảy ra.";
        }

        #endregion
    }

    #region Error Level Enum

    /// <summary>
    /// Mức độ nghiêm trọng của lỗi
    /// </summary>
    public enum ErrorLevel
    {
        INFO = 0,       // Thông tin (không phải lỗi)
        WARNING = 1,    // Cảnh báo (có thể xử lý được)
        ERROR = 2,      // Lỗi (cần attention)
        CRITICAL = 3    // Lỗi nghiêm trọng (có thể crash app)
    }

    #endregion
}

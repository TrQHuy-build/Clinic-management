using System;
using System.Data.SqlClient;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class ghi log vào database (AuditLog table)
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Ghi log hành động của user
        /// </summary>
        public static void LogAction(string action, string description)
        {
            if (!Auth.IsAuthenticated())
                return;

            try
            {
                string query = @"
                    INSERT INTO AuditLog (user_id, action, description, timestamp)
                    VALUES (@userId, @action, @description, GETDATE())";

                SqlParameter[] parameters = {
                    new SqlParameter("@userId", Auth.CurrentUserId),
                    new SqlParameter("@action", action ?? ""),
                    new SqlParameter("@description", description ?? "")
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // Silent fail - không hiển thị lỗi log để không ảnh hưởng UX
                Console.WriteLine($"Log error: {ex.Message}");
            }
        }

        /// <summary>
        /// Log đăng nhập
        /// </summary>
        public static void LogLogin()
        {
            LogAction("LOGIN", $"{Auth.CurrentUserName} đăng nhập hệ thống");
        }

        /// <summary>
        /// Log đăng xuất
        /// </summary>
        public static void LogLogout()
        {
            LogAction("LOGOUT", $"{Auth.CurrentUserName} đăng xuất");
        }

        /// <summary>
        /// Log thêm mới
        /// </summary>
        public static void LogCreate(string entity, string details)
        {
            LogAction($"CREATE_{entity.ToUpper()}", $"Thêm mới {entity}: {details}");
        }

        /// <summary>
        /// Log cập nhật
        /// </summary>
        public static void LogUpdate(string entity, string details)
        {
            LogAction($"UPDATE_{entity.ToUpper()}", $"Cập nhật {entity}: {details}");
        }

        /// <summary>
        /// Log xóa
        /// </summary>
        public static void LogDelete(string entity, string details)
        {
            LogAction($"DELETE_{entity.ToUpper()}", $"Xóa {entity}: {details}");
        }

        /// <summary>
        /// Log đặt lịch hẹn
        /// </summary>
        public static void LogBookAppointment(string patientName, DateTime appointmentDate)
        {
            LogAction("BOOK_APPOINTMENT",
                $"Đặt lịch hẹn cho {patientName} vào {appointmentDate:dd/MM/yyyy HH:mm}");
        }

        /// <summary>
        /// Log khám bệnh
        /// </summary>
        public static void LogExamination(string patientName)
        {
            LogAction("EXAMINE_PATIENT", $"Khám bệnh cho {patientName}");
        }

        /// <summary>
        /// Log thanh toán
        /// </summary>
        public static void LogPayment(int invoiceId, decimal amount)
        {
            LogAction("PAYMENT",
                $"Thanh toán hóa đơn #{invoiceId} - Số tiền: {Formatter.FormatCurrency(amount)}");
        }

        /// <summary>
        /// Log nhập kho
        /// </summary>
        public static void LogImportInventory(string itemName, int quantity)
        {
            LogAction("IMPORT_INVENTORY", $"Nhập kho: {itemName} - SL: {quantity}");
        }

        /// <summary>
        /// Log xuất kho
        /// </summary>
        public static void LogExportInventory(string itemName, int quantity)
        {
            LogAction("EXPORT_INVENTORY", $"Xuất kho: {itemName} - SL: {quantity}");
        }

        /// <summary>
        /// Log tạo đơn thuốc
        /// </summary>
        public static void LogPrescription(string patientName, int medicineCount)
        {
            LogAction("CREATE_PRESCRIPTION",
                $"Kê đơn thuốc cho {patientName} - Số lượng thuốc: {medicineCount}");
        }

        /// <summary>
        /// Log tạo hóa đơn
        /// </summary>
        public static void LogInvoiceCreation(string patientName, decimal totalAmount)
        {
            LogAction("CREATE_INVOICE",
                $"Tạo hóa đơn cho {patientName} - Tổng tiền: {Formatter.FormatCurrency(totalAmount)}");
        }

        /// <summary>
        /// Log thay đổi mật khẩu
        /// </summary>
        public static void LogPasswordChange()
        {
            LogAction("CHANGE_PASSWORD", $"{Auth.CurrentUserName} đổi mật khẩu");
        }

        /// <summary>
        /// Log xuất báo cáo
        /// </summary>
        public static void LogReportExport(string reportType)
        {
            LogAction("EXPORT_REPORT", $"Xuất báo cáo: {reportType}");
        }
    }
}
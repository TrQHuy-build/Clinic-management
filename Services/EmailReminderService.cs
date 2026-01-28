using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Services
{
    /// <summary>
    /// Background service để gửi email reminder tự động
    /// Chạy mỗi ngày vào 8:00 AM để gửi reminder cho appointments ngày mai
    /// </summary>
    public class EmailReminderService
    {
        private static System.Threading.Timer _timer;
        private static bool _isRunning = false;

        /// <summary>
        /// Bắt đầu service (gọi khi app khởi động)
        /// </summary>
        public static void Start()
        {
            if (_isRunning)
            {
                Logger.LogAction("REMINDER_SERVICE", "Email reminder service đã chạy rồi");
                return;
            }

            // Tính thời gian đến 8:00 AM tiếp theo
            DateTime now = DateTime.Now;
            DateTime next8AM = DateTime.Today.AddHours(8);
            if (now > next8AM)
            {
                next8AM = next8AM.AddDays(1);
            }

            TimeSpan timeUntilFirst = next8AM - now;

            // Chạy mỗi 24 giờ (86400000 ms)
            _timer = new System.Threading.Timer(
                callback: async (state) => await SendDailyRemindersAsync(),
                state: null,
                dueTime: timeUntilFirst,
                period: TimeSpan.FromHours(24)
            );

            _isRunning = true;
            Logger.LogAction("REMINDER_SERVICE_START", $"Email reminder service started. First run at {next8AM:yyyy-MM-dd HH:mm:ss}");
        }

        /// <summary>
        /// Dừng service
        /// </summary>
        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
                _isRunning = false;
                Logger.LogAction("REMINDER_SERVICE_STOP", "Email reminder service stopped");
            }
        }

        /// <summary>
        /// Gửi reminders cho tất cả appointments ngày mai
        /// </summary>
        public static async Task SendDailyRemindersAsync()
        {
            try
            {
                Logger.LogAction("REMINDER_DAILY_START", "Bắt đầu gửi email reminders hàng ngày");

                // Lấy danh sách appointments ngày mai (status = confirmed hoặc pending)
                DateTime tomorrow = DateTime.Today.AddDays(1);
                DateTime tomorrowEnd = tomorrow.AddDays(1);

                string query = @"
                    SELECT 
                        a.appointment_id,
                        a.patient_name,
                        a.email,
                        a.appointment_date,
                        ISNULL(s.service_name, N'Khám tổng quát') AS service_name,
                        CONCAT('APT', RIGHT('000000' + CAST(a.appointment_id AS VARCHAR), 6)) AS appointment_code
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE a.appointment_date >= @tomorrow
                    AND a.appointment_date < @tomorrowEnd
                    AND a.status IN ('confirmed', 'pending', 'booked')
                    AND a.email IS NOT NULL
                    AND a.email != ''
                    AND ISNULL(a.is_deleted, 0) = 0
                ";

                SqlParameter[] parameters = {
                    new SqlParameter("@tomorrow", tomorrow),
                    new SqlParameter("@tomorrowEnd", tomorrowEnd)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                int sentCount = 0;
                int failedCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        string email = row["email"].ToString();
                        string patientName = row["patient_name"].ToString();
                        DateTime appointmentDate = Convert.ToDateTime(row["appointment_date"]);
                        string serviceName = row["service_name"].ToString();
                        string appointmentCode = row["appointment_code"].ToString();

                        bool success = await EmailService.SendAppointmentReminderAsync(
                            email, patientName, appointmentDate, serviceName, appointmentCode);

                        if (success)
                        {
                            sentCount++;
                            
                            // Log vào database
                            LogEmailSent(Convert.ToInt32(row["appointment_id"]), "reminder", email);
                        }
                        else
                        {
                            failedCount++;
                        }

                        // Delay nhỏ để tránh spam
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        Logger.LogAction("REMINDER_SEND_ERROR", $"Lỗi gửi reminder: {ex.Message}");
                    }
                }

                Logger.LogAction("REMINDER_DAILY_COMPLETE", 
                    $"Hoàn thành gửi reminders. Thành công: {sentCount}, Thất bại: {failedCount}");
            }
            catch (Exception ex)
            {
                Logger.LogAction("REMINDER_DAILY_ERROR", $"Lỗi trong quá trình gửi reminders: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi reminder cho một appointment cụ thể (manual trigger)
        /// </summary>
        public static async Task<bool> SendReminderForAppointmentAsync(int appointmentId)
        {
            try
            {
                string query = @"
                    SELECT 
                        a.patient_name,
                        a.email,
                        a.appointment_date,
                        ISNULL(s.service_name, N'Khám tổng quát') AS service_name,
                        CONCAT('APT', RIGHT('000000' + CAST(a.appointment_id AS VARCHAR), 6)) AS appointment_code
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE a.appointment_id = @id
                ";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                if (dt.Rows.Count == 0)
                {
                    Logger.LogAction("REMINDER_NOT_FOUND", $"Không tìm thấy appointment #{appointmentId}");
                    return false;
                }

                DataRow row = dt.Rows[0];
                string email = row["email"].ToString();
                
                if (string.IsNullOrEmpty(email))
                {
                    Logger.LogAction("REMINDER_NO_EMAIL", $"Appointment #{appointmentId} không có email");
                    return false;
                }

                bool success = await EmailService.SendAppointmentReminderAsync(
                    email,
                    row["patient_name"].ToString(),
                    Convert.ToDateTime(row["appointment_date"]),
                    row["service_name"].ToString(),
                    row["appointment_code"].ToString()
                );

                if (success)
                {
                    LogEmailSent(appointmentId, "reminder_manual", email);
                }

                return success;
            }
            catch (Exception ex)
            {
                Logger.LogAction("REMINDER_SEND_ERROR", $"Lỗi gửi reminder cho appointment #{appointmentId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Log email đã gửi vào database (để tránh gửi trùng)
        /// </summary>
        private static void LogEmailSent(int appointmentId, string emailType, string emailAddress)
        {
            try
            {
                string query = @"
                    INSERT INTO EmailLog (appointment_id, email_type, email_address, sent_at, status)
                    VALUES (@appointmentId, @emailType, @emailAddress, GETDATE(), 'sent')
                ";

                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@appointmentId", appointmentId),
                    new SqlParameter("@emailType", emailType),
                    new SqlParameter("@emailAddress", emailAddress)
                });
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_LOG_ERROR", $"Lỗi log email: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem email đã được gửi chưa (để tránh spam)
        /// </summary>
        public static bool WasEmailSentToday(int appointmentId, string emailType)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM EmailLog
                    WHERE appointment_id = @appointmentId
                    AND email_type = @emailType
                    AND CAST(sent_at AS DATE) = CAST(GETDATE() AS DATE)
                    AND status = 'sent'
                ";

                object countResult = DatabaseHelper.ExecuteScalar(query, new SqlParameter[] {
                    new SqlParameter("@appointmentId", appointmentId),
                    new SqlParameter("@emailType", emailType)
                });

                int count = Convert.ToInt32(countResult ?? 0);

                return count > 0;
            }
            catch
            {
                return false; // Nếu table chưa tồn tại, return false
            }
        }
    }
}




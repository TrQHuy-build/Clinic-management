using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DentalClinicManagement.Models;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Email service cho việc gửi notification
    /// Hỗ trợ: Appointment confirmation, reminders, invoice notifications
    /// </summary>
    public class EmailService
    {
        private static string _smtpHost;
        private static int _smtpPort;
        private static string _smtpUsername;
        private static string _smtpPassword;
        private static bool _enableSsl;
        private static string _fromEmail;
        private static string _fromName;

        static EmailService()
        {
            LoadConfiguration();
        }

        /// <summary>
        /// Load cấu hình email từ App.config
        /// </summary>
        private static void LoadConfiguration()
        {
            try
            {
                _smtpHost = ConfigHelper.GetAppSetting("SmtpHost", "smtp.gmail.com");
                _smtpPort = int.Parse(ConfigHelper.GetAppSetting("SmtpPort", "587"));
                _smtpUsername = ConfigHelper.GetAppSetting("SmtpUsername", "");
                _smtpPassword = ConfigHelper.GetAppSetting("SmtpPassword", "");
                _enableSsl = bool.Parse(ConfigHelper.GetAppSetting("SmtpEnableSsl", "true"));
                _fromEmail = ConfigHelper.GetAppSetting("EmailFromAddress", _smtpUsername);
                _fromName = ConfigHelper.GetAppSetting("EmailFromName", "Phòng khám Nha khoa UTC2");
            }
            catch (Exception ex)
            {
                Logger.LogError("EMAIL_CONFIG_ERROR", $"Lỗi load cấu hình email: {ex.Message}");
                throw new Exception("Không thể load cấu hình email. Vui lòng kiểm tra App.config", ex);
            }
        }

        /// <summary>
        /// Gửi email xác nhận sau khi đặt lịch
        /// </summary>
        public static async Task<bool> SendAppointmentConfirmationAsync(string email, string patientName, 
            DateTime appointmentDate, string serviceName, string appointmentCode)
        {
            if (string.IsNullOrEmpty(email))
            {
                Logger.LogWarning("EMAIL_SKIP", "Không có email để gửi confirmation");
                return false;
            }

            string subject = "✅ Xác nhận đặt lịch khám - Phòng khám UTC2";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; text-align: center; }}
        .content {{ padding: 20px; }}
        .appointment-info {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .info-row {{ padding: 10px 0; border-bottom: 1px solid #e0e0e0; }}
        .info-label {{ font-weight: bold; color: #555; }}
        .info-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #888; font-size: 12px; }}
        .button {{ background-color: #28a745; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>🦷 Phòng khám Nha khoa UTC2</h1>
            <p style='margin: 10px 0 0 0;'>Xác nhận đặt lịch khám thành công</p>
        </div>
        <div class='content'>
            <h2>Xin chào {patientName},</h2>
            <p>Cảm ơn bạn đã đặt lịch khám tại phòng khám chúng tôi. Lịch hẹn của bạn đã được xác nhận với thông tin sau:</p>
            
            <div class='appointment-info'>
                <div class='info-row'>
                    <span class='info-label'>📋 Mã lịch hẹn:</span>
                    <span class='info-value'>{appointmentCode}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>📅 Ngày giờ:</span>
                    <span class='info-value'>{appointmentDate:dd/MM/yyyy HH:mm}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>🏥 Dịch vụ:</span>
                    <span class='info-value'>{serviceName}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>📍 Địa chỉ:</span>
                    <span class='info-value'>Số 1 Phố Trần Phú, Hà Đông, Hà Nội</span>
                </div>
            </div>

            <h3>📌 Lưu ý quan trọng:</h3>
            <ul>
                <li>Vui lòng đến <strong>trước 10 phút</strong> để làm thủ tục</li>
                <li>Mang theo thẻ BHYT (nếu có)</li>
                <li>Nếu không thể đến, vui lòng <strong>hủy lịch trước 24 giờ</strong></li>
                <li>Hotline: <strong>0243.555.1234</strong></li>
            </ul>

            <p style='text-align: center;'>
                <a href='http://localhost:5000/appointments' class='button'>Xem chi tiết lịch hẹn</a>
            </p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
            <p>© 2026 Phòng khám Nha khoa UTC2. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";

            return await SendEmailAsync(email, subject, body, isHtml: true);
        }

        /// <summary>
        /// Gửi email nhắc nhở trước 24h
        /// </summary>
        public static async Task<bool> SendAppointmentReminderAsync(string email, string patientName,
            DateTime appointmentDate, string serviceName, string appointmentCode)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string subject = "⏰ Nhắc nhở: Lịch khám ngày mai tại UTC2";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; text-align: center; }}
        .reminder-box {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
        .button {{ background-color: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 10px 5px; }}
        .button-danger {{ background-color: #dc3545; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>⏰ Nhắc nhở lịch khám</h1>
        </div>
        <div style='padding: 20px;'>
            <h2>Xin chào {patientName},</h2>
            
            <div class='reminder-box'>
                <h3 style='margin-top: 0;'>🔔 Bạn có lịch khám <strong>ngày mai</strong>!</h3>
                <p style='margin: 0;'>
                    <strong>Ngày giờ:</strong> {appointmentDate:dd/MM/yyyy HH:mm}<br>
                    <strong>Dịch vụ:</strong> {serviceName}<br>
                    <strong>Mã lịch hẹn:</strong> {appointmentCode}
                </p>
            </div>

            <p>Chúng tôi mong chờ được gặp bạn. Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ:</p>
            <p><strong>📞 Hotline:</strong> 0243.555.1234</p>

            <p style='text-align: center;'>
                <a href='http://localhost:5000/appointments' class='button'>Xác nhận tham dự</a>
                <a href='http://localhost:5000/appointments/cancel' class='button button-danger'>Hủy lịch</a>
            </p>
        </div>
    </div>
</body>
</html>
";

            return await SendEmailAsync(email, subject, body, isHtml: true);
        }

        /// <summary>
        /// Gửi email thông báo Invoice
        /// </summary>
        public static async Task<bool> SendInvoiceNotificationAsync(string email, string patientName,
            int invoiceId, decimal totalAmount, DateTime invoiceDate)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string subject = "💰 Hóa đơn khám bệnh - Phòng khám UTC2";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; padding: 30px; }}
        .header {{ background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 20px; text-align: center; }}
        .invoice-info {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .total-amount {{ font-size: 24px; font-weight: bold; color: #28a745; text-align: center; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>💰 Hóa đơn khám bệnh</h1>
        </div>
        <div style='padding: 20px;'>
            <h2>Xin chào {patientName},</h2>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Dưới đây là thông tin hóa đơn:</p>
            
            <div class='invoice-info'>
                <p><strong>Mã hóa đơn:</strong> #{invoiceId:D6}</p>
                <p><strong>Ngày tạo:</strong> {invoiceDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Trạng thái:</strong> Chưa thanh toán</p>
            </div>

            <div class='total-amount'>
                Tổng tiền: {totalAmount:N0} VNĐ
            </div>

            <p style='text-align: center;'>Vui lòng thanh toán tại quầy lễ tân hoặc chuyển khoản theo thông tin:</p>
            <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px;'>
                <p><strong>Ngân hàng:</strong> Vietcombank</p>
                <p><strong>Số tài khoản:</strong> 1234567890</p>
                <p><strong>Chủ tài khoản:</strong> Phòng khám Nha khoa UTC2</p>
                <p><strong>Nội dung:</strong> {patientName} HD{invoiceId:D6}</p>
            </div>
        </div>
    </div>
</body>
</html>
";

            return await SendEmailAsync(email, subject, body, isHtml: true);
        }

        /// <summary>
        /// Gửi email thông báo hủy lịch
        /// </summary>
        public static async Task<bool> SendAppointmentCancellationAsync(string email, string patientName,
            DateTime appointmentDate, string reason)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string subject = "❌ Thông báo hủy lịch khám - UTC2";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; padding: 30px; }}
        .header {{ background: linear-gradient(135deg, #ee0979 0%, #ff6a00 100%); color: white; padding: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>❌ Lịch hẹn đã bị hủy</h1>
        </div>
        <div style='padding: 20px;'>
            <h2>Xin chào {patientName},</h2>
            <p>Lịch hẹn của bạn vào ngày <strong>{appointmentDate:dd/MM/yyyy HH:mm}</strong> đã bị hủy.</p>
            {(string.IsNullOrEmpty(reason) ? "" : $"<p><strong>Lý do:</strong> {reason}</p>")}
            <p>Nếu bạn muốn đặt lịch mới, vui lòng truy cập website hoặc liên hệ hotline: <strong>0243.555.1234</strong></p>
        </div>
    </div>
</body>
</html>
";

            return await SendEmailAsync(email, subject, body, isHtml: true);
        }

        /// <summary>
        /// Core method để gửi email
        /// </summary>
        private static async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // Kiểm tra cấu hình
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                Logger.LogWarning("EMAIL_NOT_CONFIGURED", "SMTP chưa được cấu hình. Email không được gửi.");
                return false;
            }

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isHtml;
                    message.Priority = MailPriority.Normal;

                    using (var client = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = _enableSsl;
                        client.Timeout = 30000; // 30 seconds

                        await client.SendMailAsync(message);
                        Logger.LogAction("EMAIL_SENT", $"Đã gửi email đến {toEmail}: {subject}");
                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                Logger.LogError("EMAIL_SMTP_ERROR", $"Lỗi SMTP: {smtpEx.Message} - StatusCode: {smtpEx.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("EMAIL_SEND_ERROR", $"Lỗi gửi email đến {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email đồng bộ (blocking) - Dùng cho các case không cần async
        /// </summary>
        public static bool SendEmailSync(string toEmail, string subject, string body, bool isHtml = true)
        {
            return SendEmailAsync(toEmail, subject, body, isHtml).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Test email configuration
        /// </summary>
        public static async Task<bool> TestEmailConfigurationAsync()
        {
            try
            {
                string testEmail = _smtpUsername; // Gửi test đến chính mình
                string subject = "Test Email Configuration - UTC2";
                string body = $@"
<html>
<body>
    <h2>Email Configuration Test</h2>
    <p>This is a test email sent at {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
    <p>SMTP Host: {_smtpHost}</p>
    <p>SMTP Port: {_smtpPort}</p>
    <p>Enable SSL: {_enableSsl}</p>
    <p>From: {_fromEmail}</p>
</body>
</html>
";

                return await SendEmailAsync(testEmail, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                Logger.LogError("EMAIL_TEST_ERROR", $"Test email failed: {ex.Message}");
                return false;
            }
        }
    }
}

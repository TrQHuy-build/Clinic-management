using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Services
{
    /// <summary>
    /// Service gửi email notification cho bệnh nhân
    /// - Confirmation email sau khi đặt lịch
    /// - Reminder email 24h trước appointment
    /// - Invoice notification
    /// - Cancellation notification
    /// </summary>
    public class EmailService
    {
        private static string SmtpHost => ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
        private static int SmtpPort => int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        private static string SmtpUsername => ConfigurationManager.AppSettings["SmtpUsername"];
        private static string SmtpPassword => ConfigurationManager.AppSettings["SmtpPassword"];
        private static string FromEmail => ConfigurationManager.AppSettings["EmailFromAddress"];
        private static string FromName => ConfigurationManager.AppSettings["EmailFromName"] ?? "Phòng khám Nha khoa UTC2";
        private static bool EnableSsl => bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");

        /// <summary>
        /// Gửi email xác nhận sau khi đặt lịch
        /// </summary>
        public static async Task<bool> SendAppointmentConfirmationAsync(
            string toEmail, 
            string patientName, 
            DateTime appointmentDate, 
            string serviceName,
            string appointmentCode)
        {
            try
            {
                string subject = "✅ Xác nhận đặt lịch khám - Phòng khám Nha khoa UTC2";
                
                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 20px 0; border-left: 4px solid #667eea; border-radius: 5px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🦷 Phòng khám Nha khoa UTC2</h1>
            <p>Xác nhận đặt lịch thành công</p>
        </div>
        <div class='content'>
            <h2>Xin chào {patientName},</h2>
            <p>Cảm ơn bạn đã đặt lịch khám tại phòng khám của chúng tôi!</p>
            
            <div class='info-box'>
                <h3>📋 Thông tin lịch hẹn:</h3>
                <p><strong>Mã lịch hẹn:</strong> {appointmentCode}</p>
                <p><strong>Ngày giờ:</strong> {appointmentDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Dịch vụ:</strong> {serviceName}</p>
                <p><strong>Địa chỉ:</strong> Số 1 Phố Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội</p>
            </div>
            
            <h3>⚠️ Lưu ý quan trọng:</h3>
            <ul>
                <li>Vui lòng đến <strong>đúng giờ</strong> để được phục vụ tốt nhất</li>
                <li>Mang theo <strong>giấy tờ tùy thân</strong> và thẻ bảo hiểm (nếu có)</li>
                <li>Nếu không thể đến, vui lòng <strong>hủy lịch trước 24h</strong></li>
                <li>Chúng tôi sẽ gửi email nhắc nhở <strong>1 ngày trước</strong> cuộc hẹn</li>
            </ul>
            
            <p>Nếu bạn có câu hỏi, vui lòng liên hệ:</p>
            <p>📞 Hotline: <strong>024-3868-3100</strong></p>
            <p>📧 Email: <strong>contact@dentalutc2.vn</strong></p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
            <p>&copy; 2026 Phòng khám Nha khoa UTC2. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                await SendEmailAsync(toEmail, subject, body, isHtml: true);
                
                // Log email sent
                Logger.LogAction("EMAIL_SENT", $"Confirmation email sent to {toEmail} for appointment {appointmentCode}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_ERROR", $"Failed to send confirmation email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email nhắc nhở 24h trước appointment
        /// </summary>
        public static async Task<bool> SendAppointmentReminderAsync(
            string toEmail,
            string patientName,
            DateTime appointmentDate,
            string serviceName,
            string appointmentCode)
        {
            try
            {
                string subject = "⏰ Nhắc nhở: Bạn có lịch khám ngày mai - Phòng khám UTC2";

                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .reminder-box {{ background: #fff3cd; border: 2px solid #ffc107; padding: 20px; margin: 20px 0; border-radius: 5px; text-align: center; }}
        .info-box {{ background: white; padding: 20px; margin: 20px 0; border-left: 4px solid #f5576c; border-radius: 5px; }}
        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⏰ Nhắc nhở lịch hẹn</h1>
        </div>
        <div class='content'>
            <div class='reminder-box'>
                <h2>🔔 Bạn có lịch khám <strong>NGÀY MAI</strong>!</h2>
            </div>
            
            <h3>Xin chào {patientName},</h3>
            <p>Đây là email nhắc nhở về lịch hẹn của bạn tại Phòng khám Nha khoa UTC2.</p>
            
            <div class='info-box'>
                <h3>📋 Thông tin lịch hẹn:</h3>
                <p><strong>Mã lịch hẹn:</strong> {appointmentCode}</p>
                <p><strong>Ngày giờ:</strong> <span style='color: #f5576c; font-size: 18px;'>{appointmentDate:dd/MM/yyyy HH:mm}</span></p>
                <p><strong>Dịch vụ:</strong> {serviceName}</p>
                <p><strong>Địa chỉ:</strong> Số 1 Phố Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội</p>
            </div>
            
            <h3>📌 Chuẩn bị trước khi đến:</h3>
            <ul>
                <li>✅ Giấy tờ tùy thân (CMND/CCCD)</li>
                <li>✅ Thẻ bảo hiểm y tế (nếu có)</li>
                <li>✅ Hồ sơ bệnh án cũ (nếu có)</li>
                <li>✅ Đến trước 10 phút để làm thủ tục</li>
            </ul>
            
            <p style='color: #d9534f;'><strong>⚠️ Nếu không thể đến, vui lòng liên hệ ngay:</strong></p>
            <p>📞 Hotline: <strong>024-3868-3100</strong></p>
            <p>📧 Email: <strong>contact@dentalutc2.vn</strong></p>
        </div>
        <div class='footer'>
            <p>Chúng tôi rất mong được phục vụ bạn!</p>
            <p>&copy; 2026 Phòng khám Nha khoa UTC2. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                await SendEmailAsync(toEmail, subject, body, isHtml: true);

                // Log email sent
                Logger.LogAction("EMAIL_SENT", $"Reminder email sent to {toEmail} for appointment {appointmentCode}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_ERROR", $"Failed to send reminder email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email thông báo hóa đơn
        /// </summary>
        public static async Task<bool> SendInvoiceNotificationAsync(
            string toEmail,
            string patientName,
            int invoiceId,
            decimal totalAmount,
            DateTime invoiceDate)
        {
            try
            {
                string subject = "💰 Hóa đơn thanh toán - Phòng khám Nha khoa UTC2";

                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .invoice-box {{ background: white; padding: 20px; margin: 20px 0; border: 2px solid #4facfe; border-radius: 5px; }}
        .total {{ font-size: 24px; color: #4facfe; font-weight: bold; text-align: center; margin: 20px 0; }}
        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>💰 Hóa đơn thanh toán</h1>
        </div>
        <div class='content'>
            <h3>Xin chào {patientName},</h3>
            <p>Cảm ơn bạn đã sử dụng dịch vụ tại Phòng khám Nha khoa UTC2.</p>
            
            <div class='invoice-box'>
                <h3>📄 Thông tin hóa đơn:</h3>
                <p><strong>Mã hóa đơn:</strong> #{invoiceId.ToString("D6")}</p>
                <p><strong>Ngày lập:</strong> {invoiceDate:dd/MM/yyyy HH:mm}</p>
                <div class='total'>
                    Tổng tiền: {totalAmount:N0} VNĐ
                </div>
            </div>
            
            <p>Bạn có thể thanh toán qua các hình thức:</p>
            <ul>
                <li>💵 Tiền mặt tại quầy</li>
                <li>💳 Thẻ ATM/Credit Card</li>
                <li>🏦 Chuyển khoản ngân hàng</li>
            </ul>
            
            <p><strong>Thông tin chuyển khoản:</strong></p>
            <p>Ngân hàng: <strong>Vietcombank</strong></p>
            <p>STK: <strong>0123456789</strong></p>
            <p>Chủ TK: <strong>Phòng khám Nha khoa UTC2</strong></p>
            <p>Nội dung: <strong>TT {invoiceId.ToString("D6")} {patientName}</strong></p>
            
            <p>Nếu có thắc mắc, vui lòng liên hệ:</p>
            <p>📞 Hotline: <strong>024-3868-3100</strong></p>
        </div>
        <div class='footer'>
            <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!</p>
            <p>&copy; 2026 Phòng khám Nha khoa UTC2. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                await SendEmailAsync(toEmail, subject, body, isHtml: true);

                // Log email sent
                Logger.LogAction("EMAIL_SENT", $"Invoice email sent to {toEmail} for invoice #{invoiceId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_ERROR", $"Failed to send invoice email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email thông báo hủy lịch
        /// </summary>
        public static async Task<bool> SendAppointmentCancellationAsync(
            string toEmail,
            string patientName,
            DateTime appointmentDate,
            string reason)
        {
            try
            {
                string subject = "❌ Thông báo hủy lịch hẹn - Phòng khám UTC2";

                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc3545; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .cancel-box {{ background: #f8d7da; border: 2px solid #dc3545; padding: 20px; margin: 20px 0; border-radius: 5px; }}
        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>❌ Thông báo hủy lịch hẹn</h1>
        </div>
        <div class='content'>
            <h3>Xin chào {patientName},</h3>
            
            <div class='cancel-box'>
                <p><strong>Lịch hẹn của bạn đã bị hủy</strong></p>
                <p><strong>Ngày giờ:</strong> {appointmentDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Lý do:</strong> {reason}</p>
            </div>
            
            <p>Chúng tôi rất tiếc về sự bất tiện này.</p>
            <p>Nếu bạn muốn đặt lịch mới, vui lòng:</p>
            <ul>
                <li>🌐 Đặt lịch online: <a href='http://localhost:5000'>http://localhost:5000</a></li>
                <li>📞 Gọi hotline: <strong>024-3868-3100</strong></li>
                <li>📧 Email: <strong>contact@dentalutc2.vn</strong></li>
            </ul>
        </div>
        <div class='footer'>
            <p>Chúng tôi mong được phục vụ bạn trong thời gian tới!</p>
            <p>&copy; 2026 Phòng khám Nha khoa UTC2. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                await SendEmailAsync(toEmail, subject, body, isHtml: true);

                // Log email sent
                Logger.LogAction("EMAIL_SENT", $"Cancellation email sent to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_ERROR", $"Failed to send cancellation email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Core method để gửi email (private)
        /// </summary>
        private static async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            if (string.IsNullOrEmpty(SmtpUsername) || string.IsNullOrEmpty(SmtpPassword))
            {
                throw new InvalidOperationException("SMTP credentials not configured in App.config");
            }

            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.EnableSsl = EnableSsl;
                client.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
                client.Timeout = 30000; // 30 seconds

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(FromEmail, FromName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isHtml;
                    message.Priority = MailPriority.Normal;

                    await client.SendMailAsync(message);
                }
            }
        }

        /// <summary>
        /// Test SMTP configuration
        /// </summary>
        public static async Task<bool> TestEmailConfigurationAsync()
        {
            try
            {
                await SendEmailAsync(
                    FromEmail,
                    "Test Email Configuration",
                    "This is a test email from Dental Clinic Management System. If you receive this, your email configuration is working correctly!",
                    isHtml: false
                );
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogAction("EMAIL_TEST_FAILED", ex.Message);
                return false;
            }
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_BenhNhan.Models;
using Web_BenhNhan.Data;
using Microsoft.EntityFrameworkCore;

namespace Web_BenhNhan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Trang danh sách cuộc hẹn
        public IActionResult Appointments()
        {
            return View();
        }

        // Trang test form
        public IActionResult TestForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(
            [FromForm] string patient_name,
            [FromForm] string phone,
            [FromForm] string? email,
            [FromForm] int service_id,
            [FromForm] string appointment_date,
            [FromForm] string? status,
            [FromForm] string? notes)
        {
            try
            {
                _logger.LogInformation("=== BOOKING REQUEST RECEIVED ===");
                _logger.LogInformation($"Patient Name: {patient_name}");
                _logger.LogInformation($"Phone: {phone}");
                _logger.LogInformation($"Email: {email}");
                _logger.LogInformation($"Service ID: {service_id}");
                _logger.LogInformation($"Appointment Date: {appointment_date}");
                _logger.LogInformation($"Status: {status}");
                _logger.LogInformation($"Notes: {notes}");

                var errors = new List<string>();

                // Validate patient name
                if (string.IsNullOrWhiteSpace(patient_name))
                {
                    errors.Add("Vui lòng nhập họ và tên");
                }
                else if (patient_name.Trim().Length < 2)
                {
                    errors.Add("Họ tên phải có ít nhất 2 ký tự");
                }
                else if (patient_name.Trim().Length > 100)
                {
                    errors.Add("Họ tên không được quá 100 ký tự");
                }

                // Validate phone
                if (string.IsNullOrWhiteSpace(phone))
                {
                    errors.Add("Vui lòng nhập số điện thoại");
                }
                else
                {
                    var cleanPhone = phone.Replace(" ", "").Replace("-", "");
                    if (cleanPhone.Length < 10)
                    {
                        errors.Add("Số điện thoại phải có ít nhất 10 số");
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(cleanPhone, @"^(0[3|5|7|8|9])+([0-9]{8})$"))
                    {
                        errors.Add("Số điện thoại không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09)");
                    }
                }

                // Validate email (optional but if provided must be valid)
                if (!string.IsNullOrWhiteSpace(email))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                    {
                        errors.Add("Email không hợp lệ");
                    }
                    else if (email.Length > 255)
                    {
                        errors.Add("Email không được quá 255 ký tự");
                    }
                }

                // Validate service
                if (service_id < 1 || service_id > 6)
                {
                    errors.Add("Vui lòng chọn dịch vụ hợp lệ");
                }

                // Validate appointment date
                if (!DateTime.TryParse(appointment_date, out var parsedDate))
                {
                    errors.Add("Ngày khám không hợp lệ");
                }
                else
                {
                    // Check if date is in the past
                    if (parsedDate < DateTime.Now)
                    {
                        errors.Add("Không thể đặt lịch cho thời gian đã qua");
                    }
                    
                    // Check if date is too far in the future (max 3 months)
                    if (parsedDate > DateTime.Now.AddMonths(3))
                    {
                        errors.Add("Chỉ có thể đặt lịch tối đa 3 tháng trước");
                    }

                    // Check business hours (8:00 - 18:00)
                    if (parsedDate.Hour < 8 || parsedDate.Hour >= 18)
                    {
                        errors.Add("Giờ khám phải trong khoảng 08:00 - 18:00");
                    }

                    // Check if date is weekend (optional - uncomment if needed)
                    // if (parsedDate.DayOfWeek == DayOfWeek.Sunday)
                    // {
                    //     errors.Add("Phòng khám không làm việc vào Chủ nhật");
                    // }
                }

                // Validate notes length
                if (!string.IsNullOrWhiteSpace(notes) && notes.Length > 500)
                {
                    errors.Add("Ghi chú không được quá 500 ký tự");
                }

                // Return validation errors if any
                if (errors.Count > 0)
                {
                    _logger.LogWarning($"Validation failed: {string.Join(", ", errors)}");
                    return Json(new { success = false, errors = errors.ToArray() });
                }

                // Check for duplicate appointments (same patient, same date/time)
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => 
                        a.Phone == phone.Replace(" ", "").Replace("-", "") && 
                        a.AppointmentDate == parsedDate &&
                        a.Status != "cancelled");

                if (existingAppointment != null)
                {
                    errors.Add($"Bạn đã có lịch hẹn vào thời gian này (ID: {existingAppointment.AppointmentId})");
                    return Json(new { success = false, errors = errors.ToArray() });
                }

                // Create appointment object
                var appointment = new Appointment
                {
                    PatientName = patient_name.Trim(),
                    Phone = phone,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                    ServiceId = service_id,
                    AppointmentDate = parsedDate,
                    Status = string.IsNullOrWhiteSpace(status) ? "booked" : status,
                    Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
                };

                _logger.LogInformation("Adding to database...");
                _context.Appointments.Add(appointment);
                
                var savedCount = await _context.SaveChangesAsync();
                _logger.LogInformation($"SaveChanges completed. Rows affected: {savedCount}");
                _logger.LogInformation($"Appointment ID: {appointment.AppointmentId}");

                return Json(new { 
                    success = true, 
                    message = "Đặt lịch thành công!", 
                    appointmentId = appointment.AppointmentId,
                    appointmentDate = parsedDate.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR booking appointment: {Message}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);
                }
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        // API: Lấy tất cả cuộc hẹn
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointments = await _context.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    total = appointments.Count,
                    data = appointments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all appointments");
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // API: Lấy cuộc hẹn theo ID
        [HttpGet]
        public async Task<IActionResult> GetAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy cuộc hẹn" });
                }

                return Json(new { success = true, data = appointment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment by ID");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // TEST ENDPOINT - Kiểm tra database connection
        [HttpGet]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _context.Database.GetConnectionString();
                var appointmentCount = await _context.Appointments.CountAsync();
                var appointments = await _context.Appointments
                    .OrderByDescending(a => a.AppointmentId)
                    .Take(5)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    canConnect = canConnect,
                    connectionString = connectionString,
                    totalAppointments = appointmentCount,
                    latestAppointments = appointments
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

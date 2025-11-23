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
            [FromForm] string password,
            [FromForm] int service_id,
            [FromForm] string appointment_date,
            [FromForm] string? status,
            [FromForm] string? notes)
        {
            try
            {
                _logger.LogInformation("=== REGISTRATION REQUEST RECEIVED ===");
                _logger.LogInformation($"Full Name: {patient_name}");
                _logger.LogInformation($"Phone: {phone}");
                _logger.LogInformation($"Email: {email}");
                _logger.LogInformation($"Service ID: {service_id}");
                _logger.LogInformation($"Appointment Date: {appointment_date}");

                var errors = new List<string>();

                // Validate patient name (fullname)
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

                // Validate email (required)
                if (string.IsNullOrWhiteSpace(email))
                {
                    errors.Add("Vui lòng nhập email");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                {
                    errors.Add("Email không hợp lệ");
                }
                else if (email.Length > 255)
                {
                    errors.Add("Email không được quá 255 ký tự");
                }

                // Validate password
                if (string.IsNullOrWhiteSpace(password))
                {
                    errors.Add("Vui lòng nhập mật khẩu");
                }
                else if (password.Length < 6)
                {
                    errors.Add("Mật khẩu phải có ít nhất 6 ký tự");
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
                    if (parsedDate < DateTime.Now)
                    {
                        errors.Add("Không thể đặt lịch cho thời gian đã qua");
                    }
                    
                    if (parsedDate > DateTime.Now.AddMonths(3))
                    {
                        errors.Add("Chỉ có thể đặt lịch tối đa 3 tháng trước");
                    }

                    if (parsedDate.Hour < 8 || parsedDate.Hour >= 18)
                    {
                        errors.Add("Giờ khám phải trong khoảng 08:00 - 18:00");
                    }
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

                // Check if email already exists first
                var existingEmail = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email.Trim());
                if (existingEmail != null)
                {
                    // Verify password matches
                    if (existingEmail.PasswordHash != password)
                    {
                        errors.Add("Mật khẩu không đúng");
                        return Json(new { success = false, errors = errors.ToArray() });
                    }

                    _logger.LogInformation($"Email {email} already exists. Returning user info for confirmation.");
                    return Json(new 
                    { 
                        success = false, 
                        emailExists = true,
                        existingUser = new 
                        {
                            fullName = existingEmail.FullName,
                            phone = existingEmail.Phone,
                            email = existingEmail.Email
                        }
                    });
                }

                // Only check phone duplicate if email is new
                var existingPhone = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Phone == phone.Replace(" ", "").Replace("-", ""));
                if (existingPhone != null)
                {
                    errors.Add("Số điện thoại này đã được đăng ký");
                    return Json(new { success = false, errors = errors.ToArray() });
                }

                // 1. Create UserAccount with plain password (no hashing)
                var userAccount = new UserAccount
                {
                    FullName = patient_name.Trim(),
                    Phone = phone.Replace(" ", "").Replace("-", ""),
                    Email = email.Trim(),
                    PasswordHash = password, // Store plain password
                    Role = "patient",
                    Status = "active",
                    CreatedAt = DateTime.Now
                };

                _logger.LogInformation("Creating UserAccount...");
                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"UserAccount created with ID: {userAccount.UserId}");

                // 2. Create Appointment using UserAccount info with patient_name from fullname
                var appointment = new Appointment
                {
                    PatientName = userAccount.FullName, // Use fullname from UserAccount
                    Phone = userAccount.Phone,
                    Email = userAccount.Email,
                    ServiceId = service_id,
                    AppointmentDate = parsedDate,
                    Status = string.IsNullOrWhiteSpace(status) ? "booked" : status,
                    Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
                };

                _logger.LogInformation("Creating Appointment...");
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Appointment created with ID: {appointment.AppointmentId}");

                return Json(new { 
                    success = true, 
                    message = "Đăng ký thành công!", 
                    userId = userAccount.UserId,
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

        // API: Đặt lịch với thông tin từ UserAccount đã tồn tại
        [HttpPost]
        public async Task<IActionResult> BookWithExistingUser(string email, int service_id, string appointment_date, string status, string notes)
        {
            try
            {
                _logger.LogInformation("=== BOOKING WITH EXISTING USER ===");
                _logger.LogInformation($"Email: {email}");
                _logger.LogInformation($"Service ID: {service_id}");
                _logger.LogInformation($"Appointment Date: {appointment_date}");

                // Find existing user by email
                var existingUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email.Trim());
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng với email này" });
                }

                // Parse appointment date
                DateTime parsedDate;
                if (!DateTime.TryParse(appointment_date, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedDate))
                {
                    return Json(new { success = false, message = "Ngày hẹn không hợp lệ" });
                }

                // Create appointment
                var appointment = new Appointment
                {
                    PatientName = existingUser.FullName,
                    Phone = existingUser.Phone,
                    Email = existingUser.Email,
                    ServiceId = service_id,
                    AppointmentDate = parsedDate,
                    Status = string.IsNullOrWhiteSpace(status) ? "booked" : status,
                    Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
                };

                _logger.LogInformation("Creating Appointment with existing user info...");
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Appointment created with ID: {appointment.AppointmentId}");

                return Json(new 
                { 
                    success = true, 
                    message = "Đăng ký thành công!", 
                    userId = existingUser.UserId,
                    appointmentId = appointment.AppointmentId,
                    appointmentDate = parsedDate.ToString("dd/MM/yyyy HH:mm"),
                    userInfo = new 
                    {
                        fullName = existingUser.FullName,
                        phone = existingUser.Phone,
                        email = existingUser.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR booking with existing user: {Message}", ex.Message);
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        // API: Lấy thông tin user theo email
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Json(new { success = false, message = "Email không được để trống" });
                }

                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email.Trim());
                if (user == null)
                {
                    return Json(new { success = false, exists = false });
                }

                return Json(new 
                { 
                    success = true, 
                    exists = true,
                    user = new 
                    {
                        fullName = user.FullName,
                        phone = user.Phone,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Message}", ex.Message);
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

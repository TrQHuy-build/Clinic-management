using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientBookAppointment : UserControl
    {
        // Panel hiển thị trạng thái bác sĩ trực
        private Panel panelDoctorsOnDuty;
        private Label lblDoctorsTitle;
        private FlowLayoutPanel flowDoctors;
        
        public PatientBookAppointment()
        {
            InitializeComponent();
            InitializeDoctorOnDutyPanel();
            this.Load += PatientBookAppointment_Load;
        }

        private void PatientBookAppointment_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            LoadServices();
            InitializeTimeSelectors();
            LoadDoctorsOnDuty(); // Load bác sĩ trực khi chọn ngày mặc định
        }
        
        #region Doctor On Duty Panel
        
        private void InitializeDoctorOnDutyPanel()
        {
            // Panel chính hiển thị trạng thái bác sĩ trực
            panelDoctorsOnDuty = new Panel
            {
                Location = new Point(420, 65),
                Size = new Size(350, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Tiêu đề
            lblDoctorsTitle = new Label
            {
                Text = "TRẠNG THÁI LỊCH KHÁM",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                Location = new Point(15, 15),
                AutoSize = true
            };
            
            // Panel hiển thị trạng thái
            flowDoctors = new FlowLayoutPanel
            {
                Location = new Point(15, 50),
                Size = new Size(320, 130),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(240, 255, 240)
            };
            
            panelDoctorsOnDuty.Controls.Add(lblDoctorsTitle);
            panelDoctorsOnDuty.Controls.Add(flowDoctors);
            
            this.Controls.Add(panelDoctorsOnDuty);
            panelDoctorsOnDuty.BringToFront();
            
            // Event khi chọn ngày khác
            if (dtpDate != null)
            {
                dtpDate.ValueChanged += (s, e) => LoadDoctorsOnDuty();
            }
        }
        
        private void LoadDoctorsOnDuty()
        {
            if (dtpDate == null || flowDoctors == null) return;
            
            DateTime selectedDate = dtpDate.Value.Date;
            
            flowDoctors.Controls.Clear();
            
            try
            {
                // Query kiểm tra có bác sĩ trực không
                string query = @"
                    SELECT COUNT(DISTINCT st.staff_id) AS doctor_count,
                           COUNT(CASE WHEN s.shift_type = 'AM' THEN 1 END) AS am_shifts,
                           COUNT(CASE WHEN s.shift_type = 'PM' THEN 1 END) AS pm_shifts
                    FROM Shift s
                    INNER JOIN Staff st ON s.staff_id = st.staff_id
                    INNER JOIN UserAccount u ON st.user_id = u.user_id
                    WHERE s.shift_date = @selectedDate
                    AND u.role = 'doctor'";
                
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new[]
                {
                    new SqlParameter("@selectedDate", selectedDate)
                });
                
                int doctorCount = 0;
                int amShifts = 0;
                int pmShifts = 0;
                
                if (dt.Rows.Count > 0)
                {
                    doctorCount = Convert.ToInt32(dt.Rows[0]["doctor_count"]);
                    amShifts = Convert.ToInt32(dt.Rows[0]["am_shifts"]);
                    pmShifts = Convert.ToInt32(dt.Rows[0]["pm_shifts"]);
                }
                
                // Hiển thị kết quả
                Label lblDateInfo = new Label
                {
                    Text = $"📅 Ngày: {selectedDate:dd/MM/yyyy}",
                    Font = new Font("Segoe UI", 11),
                    Size = new Size(300, 30),
                    Padding = new Padding(10, 8, 0, 0)
                };
                flowDoctors.Controls.Add(lblDateInfo);
                
                if (doctorCount == 0)
                {
                    // Không có bác sĩ trực
                    flowDoctors.BackColor = Color.FromArgb(255, 235, 235);
                    
                    Label lblStatus = new Label
                    {
                        Text = "❌ KHÔNG CÓ BÁC SĨ TRỰC",
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.Red,
                        Size = new Size(300, 35),
                        Padding = new Padding(10, 8, 0, 0)
                    };
                    flowDoctors.Controls.Add(lblStatus);
                    
                    Label lblNote = new Label
                    {
                        Text = "⚠️ Vui lòng chọn ngày khác để đặt lịch",
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.OrangeRed,
                        Size = new Size(300, 30),
                        Padding = new Padding(10, 5, 0, 0)
                    };
                    flowDoctors.Controls.Add(lblNote);
                }
                else
                {
                    // Có bác sĩ trực
                    flowDoctors.BackColor = Color.FromArgb(235, 255, 235);
                    
                    Label lblStatus = new Label
                    {
                        Text = "✅ CÓ BÁC SĨ TRỰC",
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.Green,
                        Size = new Size(300, 35),
                        Padding = new Padding(10, 8, 0, 0)
                    };
                    flowDoctors.Controls.Add(lblStatus);
                    
                    // Thông tin ca trực
                    string shiftInfo = "";
                    if (amShifts > 0) shiftInfo += "☀️ Sáng  ";
                    if (pmShifts > 0) shiftInfo += "🌙 Chiều";
                    
                    Label lblShiftInfo = new Label
                    {
                        Text = $"Ca trực: {shiftInfo}",
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.FromArgb(0, 122, 204),
                        Size = new Size(300, 30),
                        Padding = new Padding(10, 5, 0, 0)
                    };
                    flowDoctors.Controls.Add(lblShiftInfo);
                }
            }
            catch (Exception ex)
            {
                flowDoctors.BackColor = Color.FromArgb(255, 245, 238);
                Label lblError = new Label
                {
                    Text = "⚠️ Không thể kiểm tra lịch trực",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.OrangeRed,
                    Size = new Size(300, 30),
                    Padding = new Padding(10, 8, 0, 0)
                };
                flowDoctors.Controls.Add(lblError);
            }
        }
        
        #endregion

        private void InitializeTimeSelectors()
        {
            if (cboHour != null && cboHour.Items.Count > 0)
            {
                cboHour.SelectedIndex = 0;
            }

            if (cboMinute != null && cboMinute.Items.Count > 0)
            {
                cboMinute.SelectedIndex = 0;
            }
        }

        private void LoadUserInfo()
        {
            if (txtPatientName == null || txtPhone == null || txtEmail == null) return;

            if (!Auth.CurrentUserId.HasValue)
            {
                MessageBoxHelper.ShowError("Không tìm thấy thông tin người dùng!");
                return;
            }

            try
            {
                string query = "SELECT fullname, phone, email FROM UserAccount WHERE user_id = @userId";
                SqlParameter[] parameters = { new SqlParameter("@userId", Auth.CurrentUserId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    txtPatientName.Text = dt.Rows[0]["fullname"]?.ToString() ?? "";
                    txtPhone.Text = dt.Rows[0]["phone"]?.ToString() ?? "";
                    txtEmail.Text = dt.Rows[0]["email"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải thông tin: {ex.Message}");
            }
        }

        private void LoadServices()
        {
            if (cboService == null) return;

            try
            {
                string query = @"
                    SELECT service_id, service_name + ' - ' + CAST(price AS VARCHAR) + 'đ' AS display_name
                    FROM Service
                    WHERE status = N'available'
                    ORDER BY service_name";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                cboService.DisplayMember = "display_name";
                cboService.ValueMember = "service_id";
                cboService.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dịch vụ: {ex.Message}");
            }
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (txtPatientName == null || cboService == null) return;

            if (!Validator.IsNotEmpty(txtPatientName.Text.Trim()))
            {
                MessageBoxHelper.ShowValidationError("Họ và tên");
                txtPatientName.Focus();
                return;
            }

            if (!Validator.IsValidPhone(txtPhone.Text.Trim()))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ!");
                txtPhone.Focus();
                return;
            }

            if (cboService.SelectedValue == null)
            {
                MessageBoxHelper.ShowValidationError("Dịch vụ");
                cboService.Focus();
                return;
            }

            if (cboHour.SelectedItem == null || cboMinute.SelectedItem == null)
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn giờ hẹn!");
                cboHour.Focus();
                return;
            }

            DateTime appointmentDate = dtpDate.Value.Date
                .AddHours(int.Parse(cboHour.SelectedItem.ToString()))
                .AddMinutes(int.Parse(cboMinute.SelectedItem.ToString()));

            if (!Validator.IsValidFutureDateTime(appointmentDate))
            {
                MessageBoxHelper.ShowValidationError("Giờ hẹn phải là thời gian trong tương lai!");
                return;
            }
            
            // Kiểm tra có bác sĩ trực ngày đó không
            DateTime selectedDate = dtpDate.Value.Date;
            string checkQuery = @"
                SELECT COUNT(*) FROM Shift s
                INNER JOIN Staff st ON s.staff_id = st.staff_id
                INNER JOIN UserAccount u ON st.user_id = u.user_id
                WHERE s.shift_date = @selectedDate AND u.role = 'doctor'";
            
            object checkResult = DatabaseHelper.ExecuteScalar(checkQuery, new[] 
            { 
                new SqlParameter("@selectedDate", selectedDate) 
            });
            
            if (checkResult == null || Convert.ToInt32(checkResult) == 0)
            {
                MessageBoxHelper.ShowWarning("⚠️ Không có bác sĩ trực ngày này!\n\nVui lòng chọn ngày khác.");
                return;
            }

            try
            {
                // Lấy patient_id từ user_id
                int? patientId = null;
                if (Auth.CurrentUserId.HasValue)
                {
                    string getPatientQuery = "SELECT patient_id FROM Patient WHERE user_id = @userId";
                    object patientResult = DatabaseHelper.ExecuteScalar(getPatientQuery, new[] 
                    { 
                        new SqlParameter("@userId", Auth.CurrentUserId.Value) 
                    });
                    if (patientResult != null)
                        patientId = Convert.ToInt32(patientResult);
                }
                
                string query = @"
                    INSERT INTO Appointment (patient_id, patient_name, phone, email, service_id, 
                                            appointment_date, status, notes)
                    VALUES (@patientId, @name, @phone, @email, @serviceId, @date, N'Booked', @notes)";

                SqlParameter[] parameters = {
                    new SqlParameter("@patientId", patientId.HasValue ? (object)patientId.Value : DBNull.Value),
                    new SqlParameter("@name", txtPatientName.Text.Trim()),
                    new SqlParameter("@phone", txtPhone.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@serviceId", cboService.SelectedValue),
                    new SqlParameter("@date", appointmentDate),
                    new SqlParameter("@notes", txtNotes.Text.Trim())
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess(
                        $"✅ Đặt lịch hẹn thành công!\n\n" +
                        $"🗓️ Thời gian: {appointmentDate:dd/MM/yyyy HH:mm}\n" +
                        $"🏥 Dịch vụ: {cboService.Text}\n\n" +
                        $"Vui lòng đến đúng giờ hẹn!");

                    Logger.LogBookAppointment(txtPatientName.Text, appointmentDate);
                    ClearForm();
                    LoadDoctorsOnDuty();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi đặt lịch: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            if (txtNotes != null) txtNotes.Clear();
            if (dtpDate != null) dtpDate.Value = DateTime.Today.AddDays(1);
            if (cboHour != null) cboHour.SelectedIndex = 0;
            if (cboMinute != null) cboMinute.SelectedIndex = 0;
        }
    }
}
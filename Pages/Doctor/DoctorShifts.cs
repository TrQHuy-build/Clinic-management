using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorShifts : UserControl
    {
        private SimplePagination pagination;
        
        // Đăng ký lịch trực
        private Panel panelRegister;
        private Label lblWeekInfo;
        private Label lblRegistrationStatus;
        private CheckBox[,] shiftCheckboxes; // [7 ngày, 2 ca]
        private DateTime weekStartDate;
        private const int MIN_SHIFTS_PER_WEEK = 3;

        public DoctorShifts()
        {
            InitializeComponent();
            InitializeShiftRegistration();
            InitializeEvents();
            InitializePagination();
            LoadShifts();
            LoadRegisteredShifts();
        }

        #region Shift Registration UI
        
        private void InitializeShiftRegistration()
        {
            // Tính ngày đầu tuần tiếp theo (Thứ 2)
            weekStartDate = GetNextWeekMonday();
            
            // Panel đăng ký lịch trực
            panelRegister = new Panel
            {
                Location = new Point(660, 90),
                Size = new Size(520, 350),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Tiêu đề
            Label lblRegisterTitle = new Label
            {
                Text = "📅 ĐĂNG KÝ LỊCH TRỰC TUẦN TỚI",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                Location = new Point(15, 10),
                AutoSize = true
            };
            
            // Thông tin tuần
            lblWeekInfo = new Label
            {
                Text = $"Tuần: {weekStartDate:dd/MM/yyyy} - {weekStartDate.AddDays(6):dd/MM/yyyy}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 45),
                AutoSize = true
            };
            
            // Trạng thái đăng ký
            lblRegistrationStatus = new Label
            {
                Text = "⚠️ Bạn cần đăng ký ít nhất 3 buổi trực. Đã đăng ký: 0/3 buổi",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.OrangeRed,
                Location = new Point(15, 70),
                Size = new Size(520, 25),
                BackColor = Color.FromArgb(255, 243, 205)
            };
            
            // Tạo grid đăng ký
            Panel gridPanel = CreateShiftGrid();
            gridPanel.Location = new Point(15, 105);
            
            // Nút Lưu
            Button btnSaveRegistration = new Button
            {
                Text = "💾 Lưu đăng ký",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(15, 265),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSaveRegistration.FlatAppearance.BorderSize = 0;
            btnSaveRegistration.Click += BtnSaveRegistration_Click;
            
            // Nút Reset
            Button btnReset = new Button
            {
                Text = "🔄 Reset",
                Font = new Font("Segoe UI", 11),
                Size = new Size(100, 40),
                Location = new Point(175, 265),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => ResetCheckboxes();
            
            panelRegister.Controls.Add(lblRegisterTitle);
            panelRegister.Controls.Add(lblWeekInfo);
            panelRegister.Controls.Add(lblRegistrationStatus);
            panelRegister.Controls.Add(gridPanel);
            panelRegister.Controls.Add(btnSaveRegistration);
            panelRegister.Controls.Add(btnReset);
            
            this.Controls.Add(panelRegister);
            panelRegister.BringToFront();
        }
        
        private Panel CreateShiftGrid()
        {
            Panel gridPanel = new Panel
            {
                Size = new Size(520, 150),
                BackColor = Color.White
            };
            
            string[] dayNames = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            shiftCheckboxes = new CheckBox[7, 2]; // 7 ngày x 2 ca
            
            int cellWidth = 65;
            int cellHeight = 45;
            int startX = 60;
            
            // Header row (ngày)
            for (int i = 0; i < 7; i++)
            {
                DateTime day = weekStartDate.AddDays(i);
                Label lblDay = new Label
                {
                    Text = $"{dayNames[i]}\n{day:dd/MM}",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Size = new Size(cellWidth, 40),
                    Location = new Point(startX + i * cellWidth, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White
                };
                gridPanel.Controls.Add(lblDay);
            }
            
            // Label "Sáng"
            Label lblMorning = new Label
            {
                Text = "☀️ Sáng",
                Font = new Font("Segoe UI", 9),
                Size = new Size(55, cellHeight),
                Location = new Point(0, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };
            gridPanel.Controls.Add(lblMorning);
            
            // Label "Chiều"
            Label lblAfternoon = new Label
            {
                Text = "🌙 Chiều",
                Font = new Font("Segoe UI", 9),
                Size = new Size(55, cellHeight),
                Location = new Point(0, 45 + cellHeight),
                TextAlign = ContentAlignment.MiddleLeft
            };
            gridPanel.Controls.Add(lblAfternoon);
            
            // Checkboxes
            for (int day = 0; day < 7; day++)
            {
                for (int shift = 0; shift < 2; shift++)
                {
                    CheckBox cb = new CheckBox
                    {
                        Size = new Size(cellWidth, cellHeight),
                        Location = new Point(startX + day * cellWidth, 45 + shift * cellHeight),
                        Appearance = Appearance.Button,
                        TextAlign = ContentAlignment.MiddleCenter,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    cb.FlatAppearance.BorderColor = Color.LightGray;
                    cb.CheckedChanged += ShiftCheckbox_CheckedChanged;
                    
                    shiftCheckboxes[day, shift] = cb;
                    gridPanel.Controls.Add(cb);
                }
            }
            
            return gridPanel;
        }
        
        private DateTime GetNextWeekMonday()
        {
            DateTime today = DateTime.Today;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0) daysUntilMonday = 7; // Nếu hôm nay là thứ 2, lấy thứ 2 tuần sau
            return today.AddDays(daysUntilMonday);
        }
        
        private void ShiftCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked)
            {
                cb.BackColor = Color.FromArgb(40, 167, 69);
                cb.ForeColor = Color.White;
                cb.Text = "✓";
            }
            else
            {
                cb.BackColor = Color.White;
                cb.ForeColor = Color.Black;
                cb.Text = "";
            }
            
            UpdateRegistrationStatus();
        }
        
        private int CountCheckedShifts()
        {
            int count = 0;
            for (int day = 0; day < 7; day++)
            {
                for (int shift = 0; shift < 2; shift++)
                {
                    if (shiftCheckboxes[day, shift].Checked)
                        count++;
                }
            }
            return count;
        }
        
        private void UpdateRegistrationStatus()
        {
            int count = CountCheckedShifts();
            bool isValid = count >= MIN_SHIFTS_PER_WEEK;
            
            lblRegistrationStatus.Text = isValid 
                ? $"✅ Đã đăng ký: {count}/{MIN_SHIFTS_PER_WEEK} buổi (Hợp lệ)"
                : $"⚠️ Bạn cần đăng ký ít nhất {MIN_SHIFTS_PER_WEEK} buổi. Đã đăng ký: {count}/{MIN_SHIFTS_PER_WEEK} buổi";
            
            lblRegistrationStatus.ForeColor = isValid ? Color.Green : Color.OrangeRed;
            lblRegistrationStatus.BackColor = isValid ? Color.FromArgb(212, 237, 218) : Color.FromArgb(255, 243, 205);
        }
        
        private void ResetCheckboxes()
        {
            for (int day = 0; day < 7; day++)
            {
                for (int shift = 0; shift < 2; shift++)
                {
                    shiftCheckboxes[day, shift].Checked = false;
                }
            }
        }
        
        #endregion

        #region Save Registration
        
        private void BtnSaveRegistration_Click(object sender, EventArgs e)
        {
            if (!Auth.CurrentStaffId.HasValue)
            {
                MessageBoxHelper.ShowError("Không xác định được bác sĩ đăng nhập!");
                return;
            }
            
            int count = CountCheckedShifts();
            if (count < MIN_SHIFTS_PER_WEEK)
            {
                MessageBoxHelper.ShowWarning($"Bạn cần đăng ký ít nhất {MIN_SHIFTS_PER_WEEK} buổi trực!\nHiện tại: {count} buổi");
                return;
            }
            
            try
            {
                // Xóa đăng ký cũ cho tuần này
                string deleteQuery = @"
                    DELETE FROM Shift 
                    WHERE staff_id = @staffId 
                    AND shift_date BETWEEN @startDate AND @endDate";
                
                DatabaseHelper.ExecuteNonQuery(deleteQuery, new[]
                {
                    new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                    new SqlParameter("@startDate", weekStartDate),
                    new SqlParameter("@endDate", weekStartDate.AddDays(6))
                });
                
                // Thêm đăng ký mới
                for (int day = 0; day < 7; day++)
                {
                    for (int shift = 0; shift < 2; shift++)
                    {
                        if (shiftCheckboxes[day, shift].Checked)
                        {
                            DateTime shiftDate = weekStartDate.AddDays(day);
                            TimeSpan startTime = shift == 0 ? new TimeSpan(8, 0, 0) : new TimeSpan(13, 30, 0);
                            TimeSpan endTime = shift == 0 ? new TimeSpan(12, 0, 0) : new TimeSpan(17, 30, 0);
                            string shiftType = shift == 0 ? "AM" : "PM";
                            
                            string insertQuery = @"
                                INSERT INTO Shift (staff_id, shift_date, start_time, end_time, shift_type, max_patients)
                                VALUES (@staffId, @shiftDate, @startTime, @endTime, @shiftType, 10)";
                            
                            DatabaseHelper.ExecuteNonQuery(insertQuery, new[]
                            {
                                new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                                new SqlParameter("@shiftDate", shiftDate),
                                new SqlParameter("@startTime", startTime),
                                new SqlParameter("@endTime", endTime),
                                new SqlParameter("@shiftType", shiftType)
                            });
                        }
                    }
                }
                
                MessageBoxHelper.ShowSuccess($"✅ Đã lưu đăng ký {count} buổi trực cho tuần {weekStartDate:dd/MM} - {weekStartDate.AddDays(6):dd/MM/yyyy}");
                LoadShifts();
                
                Logger.LogAction("REGISTER_SHIFT", $"Doctor {Auth.CurrentStaffId} registered {count} shifts for week {weekStartDate:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi lưu đăng ký: {ex.Message}");
            }
        }
        
        private void LoadRegisteredShifts()
        {
            if (!Auth.CurrentStaffId.HasValue) return;
            
            try
            {
                string query = @"
                    SELECT shift_date, 
                           CASE WHEN start_time < '12:00:00' THEN 0 ELSE 1 END AS shift_index
                    FROM Shift
                    WHERE staff_id = @staffId 
                    AND shift_date BETWEEN @startDate AND @endDate";
                
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new[]
                {
                    new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                    new SqlParameter("@startDate", weekStartDate),
                    new SqlParameter("@endDate", weekStartDate.AddDays(6))
                });
                
                // Reset trước
                ResetCheckboxes();
                
                // Đánh dấu các ca đã đăng ký
                foreach (DataRow row in dt.Rows)
                {
                    DateTime shiftDate = Convert.ToDateTime(row["shift_date"]);
                    int shiftIndex = Convert.ToInt32(row["shift_index"]);
                    int dayIndex = (shiftDate - weekStartDate).Days;
                    
                    if (dayIndex >= 0 && dayIndex < 7)
                    {
                        shiftCheckboxes[dayIndex, shiftIndex].Checked = true;
                    }
                }
                
                UpdateRegistrationStatus();
            }
            catch (Exception ex)
            {
                // Ignore - might be first time
            }
        }
        
        #endregion

        private void InitializeEvents()
        {
            dtpMonth.ValueChanged += (s, e) => LoadShifts();
        }

        private void InitializePagination()
        {
            pagination = new SimplePagination(dgvShifts, 12);
            Panel paginationPanel = pagination.GetPaginationPanel();
            paginationPanel.Dock = DockStyle.Bottom;
            this.Controls.Add(paginationPanel);
        }

        private void LoadShifts()
        {
            if (!Auth.CurrentStaffId.HasValue) return;

            try
            {
                DateTime startDate = new DateTime(dtpMonth.Value.Year, dtpMonth.Value.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                string query = @"
                    SELECT
                        shift_id AS [ID],
                        FORMAT(shift_date, 'dd/MM/yyyy') AS [Ngày],
                        DATENAME(WEEKDAY, shift_date) AS [Thứ],
                        CASE WHEN shift_type = 'AM' THEN N'☀️ Sáng' ELSE N'🌙 Chiều' END AS [Ca trực],
                        CONVERT(VARCHAR(5), start_time, 108) AS [Giờ bắt đầu],
                        CONVERT(VARCHAR(5), end_time, 108) AS [Giờ kết thúc],
                        ISNULL(max_patients, 10) AS [Tối đa BN],
                        (SELECT COUNT(*) FROM Appointment a WHERE a.assigned_doctor_id = s.staff_id 
                         AND CAST(a.appointment_date AS DATE) = s.shift_date 
                         AND a.status = 'Booked') AS [Đã đặt]
                    FROM Shift s
                    WHERE staff_id = @staffId AND shift_date BETWEEN @start AND @end
                    ORDER BY shift_date, start_time";

                var parameters = new[]
                {
                    new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                    new SqlParameter("@start", startDate),
                    new SqlParameter("@end", endDate)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                pagination.SetDataSource(dt);
                
                if (dgvShifts.Columns["ID"] != null)
                    dgvShifts.Columns["ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch trực: {ex.Message}");
            }
        }
    }
}
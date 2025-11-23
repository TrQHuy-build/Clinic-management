using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminShifts : UserControl
    {
        public AdminShifts()
        {
            InitializeComponent();
            LoadShifts();
        }

        private void LoadShifts()
        {
            try
            {
                string query = @"
                    SELECT
                        s.shift_id AS [ID],
                        u.fullname AS [Nhân viên],
                        st.position AS [Chức vụ],
                        FORMAT(s.shift_date, 'dd/MM/yyyy') AS [Ngày],
                        CONVERT(VARCHAR(5), s.start_time, 108) AS [Giờ bắt đầu],
                        CONVERT(VARCHAR(5), s.end_time, 108) AS [Giờ kết thúc]
                    FROM Shift s
                    INNER JOIN Staff st ON s.staff_id = st.staff_id
                    INNER JOIN UserAccount u ON st.user_id = u.user_id
                    WHERE s.shift_date = @date
                    ORDER BY s.start_time";

                DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter[] { new SqlParameter("@date", dtpDate.Value.Date) });

                // ✅ FIX: Kiểm tra nếu không có dữ liệu
                if (dt == null || dt.Rows.Count == 0)
                {
                    // Hiển thị thông báo không có lịch trực
                    string selectedDate = dtpDate.Value.ToString("dd/MM/yyyy");
                    MessageBoxHelper.ShowInfo($"Không có lịch trực ngày {selectedDate}");

                    // Vẫn gán DataSource rỗng để DataGridView hiển thị header
                    dgvShifts.DataSource = dt;
                }
                else
                {
                    dgvShifts.DataSource = dt;
                }

                // ✅ FIX: Cấu hình DataGridView
                if (dgvShifts.Columns.Count > 0)
                {
                    // Ẩn ID
                    if (dgvShifts.Columns["ID"] != null)
                        dgvShifts.Columns["ID"].Visible = false;

                    // ✅ FIX: Tự động điều chỉnh độ rộng cột
                    dgvShifts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Set độ rộng cụ thể cho từng cột
                    if (dgvShifts.Columns["Nhân viên"] != null)
                    {
                        dgvShifts.Columns["Nhân viên"].FillWeight = 150;
                        dgvShifts.Columns["Nhân viên"].MinimumWidth = 150;
                    }

                    if (dgvShifts.Columns["Chức vụ"] != null)
                    {
                        dgvShifts.Columns["Chức vụ"].FillWeight = 80;
                        dgvShifts.Columns["Chức vụ"].MinimumWidth = 80;
                    }

                    if (dgvShifts.Columns["Ngày"] != null)
                    {
                        dgvShifts.Columns["Ngày"].FillWeight = 100;
                        dgvShifts.Columns["Ngày"].MinimumWidth = 100;
                        dgvShifts.Columns["Ngày"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    if (dgvShifts.Columns["Giờ bắt đầu"] != null)
                    {
                        dgvShifts.Columns["Giờ bắt đầu"].FillWeight = 80;
                        dgvShifts.Columns["Giờ bắt đầu"].MinimumWidth = 80;
                        dgvShifts.Columns["Giờ bắt đầu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    if (dgvShifts.Columns["Giờ kết thúc"] != null)
                    {
                        dgvShifts.Columns["Giờ kết thúc"].FillWeight = 80;
                        dgvShifts.Columns["Giờ kết thúc"].MinimumWidth = 80;
                        dgvShifts.Columns["Giờ kết thúc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Format chức vụ và highlight
                    foreach (DataGridViewRow row in dgvShifts.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Format chức vụ
                        if (row.Cells["Chức vụ"].Value is string position)
                        {
                            switch (position.ToLower())
                            {
                                case "doctor":
                                    row.Cells["Chức vụ"].Value = "Bác sĩ";
                                    row.Cells["Chức vụ"].Style.ForeColor = Color.Blue;
                                    row.Cells["Chức vụ"].Style.Font = new Font(dgvShifts.Font, FontStyle.Bold);
                                    break;
                                case "staff":
                                    row.Cells["Chức vụ"].Value = "Nhân viên";
                                    row.Cells["Chức vụ"].Style.ForeColor = Color.Green;
                                    break;
                            }
                        }

                        // Highlight ca trực hiện tại
                        if (row.Cells["Giờ bắt đầu"].Value != null &&
                            row.Cells["Giờ kết thúc"].Value != null &&
                            row.Cells["Ngày"].Value != null)
                        {
                            try
                            {
                                string dateStr = row.Cells["Ngày"].Value.ToString();
                                string startTimeStr = row.Cells["Giờ bắt đầu"].Value.ToString();
                                string endTimeStr = row.Cells["Giờ kết thúc"].Value.ToString();

                                DateTime shiftDate = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);
                                TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                                TimeSpan endTime = TimeSpan.Parse(endTimeStr);

                                DateTime now = DateTime.Now;

                                if (shiftDate.Date == now.Date &&
                                    now.TimeOfDay >= startTime &&
                                    now.TimeOfDay <= endTime)
                                {
                                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
                                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 230, 153);
                                }
                            }
                            catch
                            {
                                // Ignore parse errors
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddShiftForm();
        }

        // ✅ HOÀN THIỆN: Chức năng thêm ca trực với validation đầy đủ
        private void ShowAddShiftForm()
        {
            using (Form form = new Form
            {
                Text = "Thêm ca trực mới",
                Size = new Size(550, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                // ComboBox chọn nhân viên
                ComboBox cboStaff = new ComboBox
                {
                    Location = new Point(150, 30),
                    Size = new Size(350, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 9)
                };
                Label lblStaffError = CreateErrorLabel(150, 57);

                // DateTimePicker chọn ngày
                DateTimePicker dtpShiftDate = new DateTimePicker
                {
                    Location = new Point(150, 90),
                    Size = new Size(350, 25),
                    Format = DateTimePickerFormat.Short,
                    MinDate = DateTime.Today, // ✅ Không cho chọn ngày quá khứ
                    Value = DateTime.Today
                };
                Label lblDateError = CreateErrorLabel(150, 117);

                // DateTimePicker chọn giờ bắt đầu
                DateTimePicker dtpStartTime = new DateTimePicker
                {
                    Location = new Point(150, 150),
                    Size = new Size(350, 25),
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Value = DateTime.Today.AddHours(8) // Mặc định 08:00
                };
                Label lblStartTimeError = CreateErrorLabel(150, 177);

                // DateTimePicker chọn giờ kết thúc
                DateTimePicker dtpEndTime = new DateTimePicker
                {
                    Location = new Point(150, 210),
                    Size = new Size(350, 25),
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Value = DateTime.Today.AddHours(17) // Mặc định 17:00
                };
                Label lblEndTimeError = CreateErrorLabel(150, 237);

                // Load danh sách nhân viên
                try
                {
                    string query = @"
                        SELECT 
                            st.staff_id,
                            u.fullname + ' (' + st.position + ')' AS display_name
                        FROM Staff st
                        INNER JOIN UserAccount u ON st.user_id = u.user_id
                        WHERE u.status = 'active'
                        ORDER BY u.fullname";

                    DataTable dtStaff = DatabaseHelper.ExecuteQuery(query);

                    if (dtStaff == null || dtStaff.Rows.Count == 0)
                    {
                        MessageBoxHelper.ShowError("Không có nhân viên nào trong hệ thống!");
                        form.Close();
                        return;
                    }

                    cboStaff.DisplayMember = "display_name";
                    cboStaff.ValueMember = "staff_id";
                    cboStaff.DataSource = dtStaff;
                    cboStaff.SelectedIndex = -1; // Không chọn gì mặc định
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError($"Lỗi tải danh sách nhân viên: {ex.Message}");
                    form.Close();
                    return;
                }

                // ✅ Real-time validation
                cboStaff.SelectedIndexChanged += (s, e) =>
                {
                    if (cboStaff != null && lblStaffError != null)
                        ValidateStaffRealtime(cboStaff, lblStaffError);
                };

                dtpShiftDate.ValueChanged += (s, e) =>
                {
                    if (dtpShiftDate != null && lblDateError != null)
                        ValidateShiftDateRealtime(dtpShiftDate, lblDateError);
                };

                EventHandler validateTimes = (s, e) =>
                {
                    if (dtpStartTime != null && dtpEndTime != null &&
                        lblStartTimeError != null && lblEndTimeError != null)
                        ValidateTimesRealtime(dtpStartTime, dtpEndTime, lblStartTimeError, lblEndTimeError);
                };
                dtpStartTime.ValueChanged += validateTimes;
                dtpEndTime.ValueChanged += validateTimes;

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Nhân viên:", 30, 33), cboStaff, lblStaffError,
                    CreateLabel("Ngày trực:", 30, 93), dtpShiftDate, lblDateError,
                    CreateLabel("Giờ bắt đầu:", 30, 153), dtpStartTime, lblStartTimeError,
                    CreateLabel("Giờ kết thúc:", 30, 213), dtpEndTime, lblEndTimeError
                });

                Button btnSave = CreateButton("Lưu", 225, 280, "#007ACC");
                btnSave.Cursor = Cursors.Hand;

                btnSave.Click += (s, ev) =>
                {
                    // ✅ Disable button để tránh double-click
                    btnSave.Enabled = false;
                    btnSave.Text = "Đang lưu...";

                    try
                    {
                        // Validate đầy đủ
                        if (!ValidateShiftInput(cboStaff, dtpShiftDate, dtpStartTime, dtpEndTime))
                        {
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                            return;
                        }

                        int staffId = Convert.ToInt32(cboStaff.SelectedValue);
                        DateTime shiftDate = dtpShiftDate.Value.Date;
                        TimeSpan startTime = dtpStartTime.Value.TimeOfDay;
                        TimeSpan endTime = dtpEndTime.Value.TimeOfDay;

                        // ✅ Kiểm tra trùng lịch trực
                        object existingShift = DatabaseHelper.ExecuteScalar(@"
                            SELECT COUNT(*) 
                            FROM Shift 
                            WHERE staff_id = @staff_id 
                              AND shift_date = @shift_date
                              AND (
                                  (@start_time >= start_time AND @start_time < end_time)
                                  OR (@end_time > start_time AND @end_time <= end_time)
                                  OR (@start_time <= start_time AND @end_time >= end_time)
                              )",
                            new SqlParameter[] {
                                new SqlParameter("@staff_id", staffId),
                                new SqlParameter("@shift_date", shiftDate),
                                new SqlParameter("@start_time", startTime),
                                new SqlParameter("@end_time", endTime)
                            });

                        if (Convert.ToInt32(existingShift) > 0)
                        {
                            MessageBoxHelper.ShowValidationError("Nhân viên đã có ca trực trùng giờ trong ngày này!");
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                            return;
                        }

                        // Insert ca trực mới
                        string insertQuery = @"
                            INSERT INTO Shift (staff_id, shift_date, start_time, end_time)
                            VALUES (@staff_id, @shift_date, @start_time, @end_time)";

                        int result = DatabaseHelper.ExecuteNonQuery(insertQuery,
                            new SqlParameter[] {
                                new SqlParameter("@staff_id", staffId),
                                new SqlParameter("@shift_date", shiftDate),
                                new SqlParameter("@start_time", startTime),
                                new SqlParameter("@end_time", endTime)
                            });

                        if (result > 0)
                        {
                            MessageBoxHelper.ShowSaveSuccess();
                            string staffName = cboStaff.Text;
                            Logger.LogAction("CREATE_SHIFT", $"{staffName} - {shiftDate:dd/MM/yyyy}");
                            form.Close();
                            LoadShifts();
                        }
                        else
                        {
                            MessageBoxHelper.ShowError("Không thể lưu ca trực!");
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                        btnSave.Enabled = true;
                        btnSave.Text = "Lưu";
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                        btnSave.Enabled = true;
                        btnSave.Text = "Lưu";
                    }
                };

                form.Controls.Add(btnSave);
                form.ShowDialog();
            }
        }

        // ✅ VALIDATION METHODS

        private bool ValidateShiftInput(ComboBox cboStaff, DateTimePicker dtpShiftDate,
            DateTimePicker dtpStartTime, DateTimePicker dtpEndTime)
        {
            // Kiểm tra nhân viên
            if (cboStaff.SelectedIndex < 0)
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn nhân viên!");
                cboStaff.Focus();
                return false;
            }

            // Kiểm tra ngày trực
            if (dtpShiftDate.Value.Date < DateTime.Today)
            {
                MessageBoxHelper.ShowValidationError("Ngày trực không được là ngày quá khứ!");
                dtpShiftDate.Focus();
                return false;
            }

            // Kiểm tra ngày trực không quá xa (ví dụ: không quá 1 năm)
            if (dtpShiftDate.Value.Date > DateTime.Today.AddYears(1))
            {
                MessageBoxHelper.ShowValidationError("Ngày trực không được quá 1 năm kể từ hôm nay!");
                dtpShiftDate.Focus();
                return false;
            }

            // Kiểm tra giờ
            TimeSpan startTime = dtpStartTime.Value.TimeOfDay;
            TimeSpan endTime = dtpEndTime.Value.TimeOfDay;

            if (startTime >= endTime)
            {
                MessageBoxHelper.ShowValidationError("Giờ kết thúc phải sau giờ bắt đầu!");
                dtpEndTime.Focus();
                return false;
            }

            // Kiểm tra ca trực không quá ngắn (ít nhất 1 giờ)
            TimeSpan duration = endTime - startTime;
            if (duration.TotalHours < 1)
            {
                MessageBoxHelper.ShowValidationError("Ca trực phải có ít nhất 1 giờ!");
                dtpEndTime.Focus();
                return false;
            }

            // Cảnh báo ca trực quá dài (> 12 giờ)
            if (duration.TotalHours > 12)
            {
                var result = MessageBox.Show(
                    $"Ca trực rất dài ({duration.TotalHours:F1} giờ). Bạn có chắc chắn muốn tiếp tục?",
                    "Cảnh báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    dtpEndTime.Focus();
                    return false;
                }
            }

            // Kiểm tra giờ hợp lý (không nên trực vào giữa đêm)
            if (startTime.Hours < 6 || startTime.Hours > 22)
            {
                var result = MessageBox.Show(
                    "Giờ bắt đầu không thông thường (ngoài 06:00 - 22:00). Bạn có chắc chắn?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    dtpStartTime.Focus();
                    return false;
                }
            }

            return true;
        }

        // ✅ REAL-TIME VALIDATION

        private void ValidateStaffRealtime(ComboBox cbo, Label lblError)
        {
            if (cbo == null || lblError == null) return;

            if (cbo.SelectedIndex < 0)
            {
                ShowFieldError(null, lblError, "Vui lòng chọn nhân viên");
            }
            else
            {
                ClearFieldError(null, lblError);
            }
        }

        private void ValidateShiftDateRealtime(DateTimePicker dtp, Label lblError)
        {
            if (dtp == null || lblError == null) return;

            if (dtp.Value.Date < DateTime.Today)
            {
                ShowFieldError(null, lblError, "Ngày trực không được là ngày quá khứ");
            }
            else if (dtp.Value.Date > DateTime.Today.AddYears(1))
            {
                ShowFieldError(null, lblError, "Ngày trực không được quá 1 năm kể từ hôm nay");
            }
            else
            {
                ClearFieldError(null, lblError);
            }
        }

        private void ValidateTimesRealtime(DateTimePicker dtpStart, DateTimePicker dtpEnd,
            Label lblStartError, Label lblEndError)
        {
            if (dtpStart == null || dtpEnd == null || lblStartError == null || lblEndError == null)
                return;

            TimeSpan startTime = dtpStart.Value.TimeOfDay;
            TimeSpan endTime = dtpEnd.Value.TimeOfDay;

            if (startTime >= endTime)
            {
                ShowFieldError(null, lblEndError, "Giờ kết thúc phải sau giờ bắt đầu");
                ShowFieldError(null, lblStartError, "Giờ bắt đầu phải trước giờ kết thúc");
            }
            else
            {
                TimeSpan duration = endTime - startTime;

                if (duration.TotalHours < 1)
                {
                    ShowFieldError(null, lblEndError, "Ca trực phải có ít nhất 1 giờ");
                }
                else if (duration.TotalHours > 12)
                {
                    ShowFieldError(null, lblEndError, $"⚠ Ca trực rất dài ({duration.TotalHours:F1} giờ)");
                }
                else
                {
                    ClearFieldError(null, lblEndError);
                }

                // Check giờ bắt đầu hợp lý
                if (startTime.Hours < 6 || startTime.Hours > 22)
                {
                    ShowFieldError(null, lblStartError, "⚠ Giờ bắt đầu không thông thường");
                }
                else
                {
                    ClearFieldError(null, lblStartError);
                }
            }
        }

        private void ShowFieldError(Control ctrl, Label lblError, string message)
        {
            if (ctrl != null)
            {
                if (message.StartsWith("⚠"))
                    ctrl.BackColor = Color.FromArgb(255, 243, 205);
                else
                    ctrl.BackColor = Color.FromArgb(255, 235, 238);
            }
            lblError.Text = message;
            lblError.ForeColor = message.StartsWith("⚠") ? Color.Orange : Color.Red;
            lblError.Visible = true;
        }

        private void ClearFieldError(Control ctrl, Label lblError)
        {
            if (ctrl != null)
                ctrl.BackColor = Color.White;
            lblError.Text = "";
            lblError.Visible = false;
        }

        private Label CreateErrorLabel(int x, int y)
        {
            return new Label
            {
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Visible = false,
                MaximumSize = new Size(350, 0)
            };
        }

        // Helper methods
        private Label CreateLabel(string text, int x, int y) => new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9)
        };

        private Button CreateButton(string text, int x, int y, string backColor) => new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(100, 35),
            BackColor = ColorTranslator.FromHtml(backColor),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        private void dtpDate_ValueChanged(object sender, EventArgs e) => LoadShifts();
    }
}
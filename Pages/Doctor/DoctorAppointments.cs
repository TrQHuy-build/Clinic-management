using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorAppointments : UserControl
    {
        private TabControl tabControl;
        private TabPage tabPending;
        private TabPage tabConfirmed;
        private TabPage tabHistory;
        private DataGridView dgvPending;
        private DataGridView dgvConfirmed;
        private DataGridView dgvHistory;

        public DoctorAppointments()
        {
            InitializeComponent();
            InitializeTabs();
            LoadAllAppointments();
        }

        private void InitializeTabs()
        {
            // Create TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // Tab 1: Yêu cầu khám mới
            tabPending = new TabPage("🔔 Yêu cầu khám mới");
            dgvPending = CreateDataGridView();
            tabPending.Controls.Add(dgvPending);

            // Tab 2: Đã xác nhận
            tabConfirmed = new TabPage("✅ Đã xác nhận");
            dgvConfirmed = CreateDataGridView();
            tabConfirmed.Controls.Add(dgvConfirmed);

            // Tab 3: Lịch sử
            tabHistory = new TabPage("📋 Lịch sử");
            dgvHistory = CreateDataGridView();
            tabHistory.Controls.Add(dgvHistory);

            tabControl.TabPages.AddRange(new TabPage[] { tabPending, tabConfirmed, tabHistory });

            // Replace existing dgvAppointments with tabControl
            this.Controls.Clear();

            // Add date filter panel at top
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };

            Label lblDate = new Label
            {
                Text = "Ngày:",
                Location = new Point(20, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            dtpDate.Location = new Point(70, 15);
            dtpDate.Size = new Size(200, 25);

            btnToday.Location = new Point(280, 15);
            btnToday.Size = new Size(100, 30);
            btnToday.Text = "Hôm nay";

            panelTop.Controls.AddRange(new Control[] { lblDate, dtpDate, btnToday });

            this.Controls.Add(tabControl);
            this.Controls.Add(panelTop);

            tabControl.SelectedIndexChanged += (s, e) => LoadAllAppointments();
        }

        private DataGridView CreateDataGridView()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            dgv.CellClick += Dgv_CellClick;
            return dgv;
        }

        private void LoadAllAppointments()
        {
            LoadPendingAppointments();
            LoadConfirmedAppointments();
            LoadHistoryAppointments();
        }

        private void LoadPendingAppointments()
        {
            try
            {
                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        CASE 
                            WHEN p.patient_id IS NOT NULL THEN u.fullname
                            ELSE a.patient_name
                        END AS [Bệnh nhân],
                        a.phone AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    LEFT JOIN Patient p ON a.patient_id = p.patient_id
                    LEFT JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE a.assigned_doctor_id = @doctorId
                    AND a.status = 'pending'
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0)
                });

                // ✅ FIX: Check if DataTable and DataGridView are not null
                if (dt == null || dgvPending == null)
                {
                    MessageBoxHelper.ShowError("Không thể tải dữ liệu lịch hẹn chờ xác nhận!");
                    return;
                }

                dgvPending.DataSource = dt;
                
                // ✅ FIX: Check if column exists before accessing
                if (dgvPending.Columns.Count > 0 && dgvPending.Columns["ID"] != null)
                {
                    dgvPending.Columns["ID"].Visible = false;
                }

                // Add action buttons
                if (!dgvPending.Columns.Contains("Accept"))
                {
                    dgvPending.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Accept",
                        HeaderText = "",
                        Text = "✅ Đồng ý",
                        UseColumnTextForButtonValue = true,
                        Width = 100
                    });

                    dgvPending.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Reject",
                        HeaderText = "",
                        Text = "❌ Từ chối",
                        UseColumnTextForButtonValue = true,
                        Width = 100
                    });
                }

                // Highlight rows
                foreach (DataGridViewRow row in dgvPending.Rows)
                {
                    if (!row.IsNewRow)
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF3CD");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải yêu cầu khám: {ex.Message}");
            }
        }

        private void LoadConfirmedAppointments()
        {
            try
            {
                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        CASE 
                            WHEN p.patient_id IS NOT NULL THEN u.fullname
                            ELSE a.patient_name
                        END AS [Bệnh nhân],
                        a.phone AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    LEFT JOIN Patient p ON a.patient_id = p.patient_id
                    LEFT JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE a.assigned_doctor_id = @doctorId
                    AND a.status IN ('confirmed', 'in_progress')
                    AND CAST(a.appointment_date AS DATE) = @date
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0),
                    new SqlParameter("@date", dtpDate.Value.Date)
                });

                // ✅ FIX: Check if DataTable and DataGridView are not null
                if (dt == null || dgvConfirmed == null)
                {
                    MessageBoxHelper.ShowError("Không thể tải dữ liệu lịch hẹn đã xác nhận!");
                    return;
                }

                dgvConfirmed.DataSource = dt;
                
                // ✅ FIX: Check if column exists before accessing
                if (dgvConfirmed.Columns.Count > 0 && dgvConfirmed.Columns["ID"] != null)
                {
                    dgvConfirmed.Columns["ID"].Visible = false;
                }

                if (!dgvConfirmed.Columns.Contains("StartExam"))
                {
                    dgvConfirmed.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "StartExam",
                        HeaderText = "",
                        Text = "🩺 Bắt đầu khám",
                        UseColumnTextForButtonValue = true,
                        Width = 120
                    });
                }

                foreach (DataGridViewRow row in dgvConfirmed.Rows)
                {
                    if (!row.IsNewRow)
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D1ECF1");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch đã xác nhận: {ex.Message}");
            }
        }

        private void LoadHistoryAppointments()
        {
            try
            {
                // ✅ FIX: Use TRY_CONVERT and check if column exists
                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        CASE 
                            WHEN p.patient_id IS NOT NULL THEN u.fullname
                            ELSE a.patient_name
                        END AS [Bệnh nhân],
                        a.phone AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
                        CASE 
                            WHEN a.status = 'completed' THEN N'Hoàn thành'
                            WHEN a.status = 'rejected' THEN N'Từ chối'
                            WHEN a.status = 'cancelled' THEN N'Đã hủy'
                            ELSE a.status
                        END AS [Trạng thái]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    LEFT JOIN Patient p ON a.patient_id = p.patient_id
                    LEFT JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE a.assigned_doctor_id = @doctorId
                    AND a.status IN ('completed', 'rejected', 'cancelled')
                    AND CAST(a.appointment_date AS DATE) = @date
                    ORDER BY a.appointment_date DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0),
                    new SqlParameter("@date", dtpDate.Value.Date)
                });

                // ✅ FIX: Check if DataTable and DataGridView are not null
                if (dt == null || dgvHistory == null)
                {
                    MessageBoxHelper.ShowError("Không thể tải lịch sử khám!");
                    return;
                }

                // ✅ FIX: Add reject_reason column to DataTable if it exists in database
                try
                {
                    string checkColumnQuery = @"
                        SELECT ISNULL(reject_reason, N'Không có lý do') AS [Lý do từ chối]
                        FROM Appointment 
                        WHERE appointment_id = @id";
                    
                    // Add reject_reason to each row
                    if (!dt.Columns.Contains("Lý do từ chối"))
                    {
                        dt.Columns.Add("Lý do từ chối", typeof(string));
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        int appointmentId = Convert.ToInt32(row["ID"]);
                        try
                        {
                            object reasonResult = DatabaseHelper.ExecuteScalar(checkColumnQuery, 
                                new SqlParameter[] { new SqlParameter("@id", appointmentId) });
                            row["Lý do từ chối"] = reasonResult?.ToString() ?? "Không có lý do";
                        }
                        catch
                        {
                            // Column doesn't exist yet, use default value
                            row["Lý do từ chối"] = "N/A";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If reject_reason column doesn't exist, just skip it
                    Logger.LogAction("HISTORY_LOAD_WARNING", $"reject_reason column not found: {ex.Message}");
                }

                dgvHistory.DataSource = dt;
                
                // ✅ FIX: Check if column exists before accessing
                if (dgvHistory.Columns.Count > 0 && dgvHistory.Columns["ID"] != null)
                {
                    dgvHistory.Columns["ID"].Visible = false;
                }

                foreach (DataGridViewRow row in dgvHistory.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string status = row.Cells["Trạng thái"].Value?.ToString();
                        if (status == "Hoàn thành")
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                        else if (status == "Từ chối")
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch sử: {ex.Message}");
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            int appointmentId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value);
            string columnName = dgv.Columns[e.ColumnIndex].Name;

            if (columnName == "Accept")
            {
                AcceptAppointment(appointmentId);
            }
            else if (columnName == "Reject")
            {
                RejectAppointment(appointmentId);
            }
            else if (columnName == "StartExam")
            {
                StartExamination(appointmentId);
            }
        }

        private void AcceptAppointment(int appointmentId)
        {
            if (!MessageBoxHelper.ShowConfirm("Xác nhận đồng ý khám bệnh nhân này?"))
                return;

            try
            {
                // ✅ FIX: Check if reject_reason column exists before updating
                string query = @"UPDATE Appointment 
                               SET status = 'confirmed'
                               WHERE appointment_id = @id";

                // Try to clear reject_reason if column exists
                try
                {
                    string checkColumnQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Appointment' 
                        AND COLUMN_NAME = 'reject_reason'";
                    
                    object columnExists = DatabaseHelper.ExecuteScalar(checkColumnQuery, null);
                    
                    if (Convert.ToInt32(columnExists ?? 0) > 0)
                    {
                        query = @"UPDATE Appointment 
                                SET status = 'confirmed',
                                    reject_reason = NULL
                                WHERE appointment_id = @id";
                    }
                }
                catch
                {
                    // If check fails, use simple query without reject_reason
                }

                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đã xác nhận lịch hẹn!");
                    Logger.LogAction("ACCEPT_APPOINTMENT", $"Bác sĩ đồng ý khám #{appointmentId}");
                    LoadAllAppointments();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void RejectAppointment(int appointmentId)
        {
            // Show dialog to input reject reason
            using (Form form = new Form())
            {
                form.Text = "Lý do từ chối";
                form.Size = new Size(450, 250);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = "Vui lòng nhập lý do từ chối:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10)
                };

                TextBox txtReason = new TextBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(390, 80),
                    Multiline = true,
                    Font = new Font("Segoe UI", 10)
                };

                Button btnConfirm = new Button
                {
                    Text = "Xác nhận từ chối",
                    Location = new Point(155, 150),
                    Size = new Size(140, 35),
                    BackColor = ColorTranslator.FromHtml("#DC3545"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                btnConfirm.Click += (s, ev) =>
                {
                    string reason = txtReason.Text.Trim();
                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        MessageBoxHelper.ShowValidationError("Vui lòng nhập lý do từ chối!");
                        return;
                    }

                    try
                    {
                        // ✅ FIX: Check if reject_reason column exists
                        string query = "UPDATE Appointment SET status = 'rejected' WHERE appointment_id = @id";
                        SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@id", appointmentId) };

                        try
                        {
                            string checkColumnQuery = @"
                                SELECT COUNT(*) 
                                FROM INFORMATION_SCHEMA.COLUMNS 
                                WHERE TABLE_NAME = 'Appointment' 
                                AND COLUMN_NAME = 'reject_reason'";
                            
                            object columnExists = DatabaseHelper.ExecuteScalar(checkColumnQuery, null);
                            
                            if (Convert.ToInt32(columnExists ?? 0) > 0)
                            {
                                query = @"UPDATE Appointment 
                                        SET status = 'rejected',
                                            reject_reason = @reason
                                        WHERE appointment_id = @id";
                                parameters = new SqlParameter[] {
                                    new SqlParameter("@id", appointmentId),
                                    new SqlParameter("@reason", reason)
                                };
                            }
                            else
                            {
                                // Log to AuditLog instead if column doesn't exist
                                Logger.LogAction("REJECT_APPOINTMENT", $"Bác sĩ từ chối #{appointmentId}: {reason}");
                            }
                        }
                        catch
                        {
                            // If check fails, log to AuditLog
                            Logger.LogAction("REJECT_APPOINTMENT", $"Bác sĩ từ chối #{appointmentId}: {reason}");
                        }

                        int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                        if (result > 0)
                        {
                            MessageBoxHelper.ShowSuccess("Đã từ chối lịch hẹn!");
                            form.Close();
                            LoadAllAppointments();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                    }
                };

                form.Controls.AddRange(new Control[] { lbl, txtReason, btnConfirm });
                form.ShowDialog();
            }
        }

        private void StartExamination(int appointmentId)
        {
            // Update status to in_progress
            try
            {
                string query = "UPDATE Appointment SET status = 'in_progress' WHERE appointment_id = @id";
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                MessageBoxHelper.ShowInfo("Vui lòng chuyển sang tab 'Khám bệnh' để tiếp tục!");
                Logger.LogAction("START_EXAMINATION", $"Bắt đầu khám #{appointmentId}");
                LoadAllAppointments();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // Event handlers
        private void dtpDate_ValueChanged(object sender, EventArgs e) => LoadAllAppointments();

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpDate.Value = DateTime.Today;
            LoadAllAppointments();
        }
    }
}
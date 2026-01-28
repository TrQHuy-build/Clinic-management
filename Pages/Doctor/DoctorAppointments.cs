using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Forms;
using DentalClinicManagement.Services;
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
        
        // ✅ Pagination for 3 tabs
        private SimplePagination paginationPending;
        private SimplePagination paginationConfirmed;
        private SimplePagination paginationHistory;
  
        // ✅ Panel thông báo số lịch hẹn mới
        private Panel notificationPanel;
  private Label lblNotificationCount;

        // ✅ Advanced search controls
        private TextBox txtSearchPatient;
    private ComboBox cboServiceFilter;

public DoctorAppointments()
        {
   InitializeComponent();
InitializeNotificationPanel();
      InitializeTabs();
       InitializePagination(); // ✅ Add pagination
       LoadAllAppointments();
    
  // ✅ Bắt đầu theo dõi thông báo
    StartNotificationMonitoring();
   }

      /// <summary>
        /// ✅ TÍNH NĂNG 1: Panel hiển thị số thông báo lịch hẹn mới
  /// </summary>
  private void InitializeNotificationPanel()
    {
            notificationPanel = new Panel
     {
         Dock = DockStyle.Top,
     Height = 50,
   BackColor = ColorTranslator.FromHtml("#FFF3CD"),
  Visible = false,
              Padding = new Padding(15, 10, 15, 10)
     };

     lblNotificationCount = new Label
      {
   Text = "🔔 Bạn có 0 lịch hẹn mới đang chờ xác nhận",
       Dock = DockStyle.Fill,
       Font = new Font("Segoe UI", 11, FontStyle.Bold),
      ForeColor = ColorTranslator.FromHtml("#856404"),
   TextAlign = ContentAlignment.MiddleLeft
        };

   Button btnDismiss = new Button
     {
    Text = "✕",
      Dock = DockStyle.Right,
   Width = 40,
           FlatStyle = FlatStyle.Flat,
    BackColor = Color.Transparent,
       ForeColor = ColorTranslator.FromHtml("#856404"),
       Font = new Font("Segoe UI", 12, FontStyle.Bold),
     Cursor = Cursors.Hand
 };
            btnDismiss.FlatAppearance.BorderSize = 0;
      btnDismiss.Click += (s, e) => notificationPanel.Visible = false;

    notificationPanel.Controls.Add(lblNotificationCount);
      notificationPanel.Controls.Add(btnDismiss);
            this.Controls.Add(notificationPanel);
        }

        /// <summary>
     /// ✅ TÍNH NĂNG 1: Bắt đầu theo dõi thông báo realtime
        /// </summary>
    private void StartNotificationMonitoring()
  {
    NotificationService.Instance.OnNewAppointment += (s, e) =>
    {
    if (this.InvokeRequired)
      {
           this.Invoke(new Action(() => HandleNewAppointment(e)));
     }
      else
     {
    HandleNewAppointment(e);
        }
     };

            NotificationService.Instance.OnNotificationCountChanged += (s, count) =>
     {
     if (this.InvokeRequired)
  {
       this.Invoke(new Action(() => UpdateNotificationBadge(count)));
     }
     else
      {
UpdateNotificationBadge(count);
          }
  };

         NotificationService.Instance.StartMonitoring(30); // Check mỗi 30 giây
    }

 private void HandleNewAppointment(NewAppointmentEventArgs e)
    {
         // Hiển thị notification panel
   notificationPanel.Visible = true;
  lblNotificationCount.Text = $"🔔 Bạn có {e.NewCount} lịch hẹn mới đang chờ!";
     
         // Hiển thị popup notification
    NotificationService.ShowNotificationPopup(
    this.FindForm(),
      "Lịch hẹn mới!",
   $"Bạn có {e.NewCount} lịch hẹn mới được gán.",
      5000);

 // Refresh danh sách
    LoadAllAppointments();
 }

        private void UpdateNotificationBadge(int count)
        {
       if (count > 0)
  {
       notificationPanel.Visible = true;
        lblNotificationCount.Text = $"🔔 Bạn có {count} lịch hẹn đang chờ xử lý";
      }
       else
      {
        notificationPanel.Visible = false;
     }

    // Update tab title
     if (tabPending != null)
    {
      tabPending.Text = count > 0 ? $"🔔 Yêu cầu khám ({count})" : "🔔 Yêu cầu khám mới";
    }
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
      tabConfirmed = new TabPage("✅ Lịch hẹn hôm nay");
            dgvConfirmed = CreateDataGridView();
  tabConfirmed.Controls.Add(dgvConfirmed);

  // Tab 3: Lịch sử
    tabHistory = new TabPage("📋 Lịch sử");
 dgvHistory = CreateDataGridView();
    tabHistory.Controls.Add(dgvHistory);

       tabControl.TabPages.AddRange(new TabPage[] { tabPending, tabConfirmed, tabHistory });

     // Clear existing controls
       this.Controls.Clear();

            // ✅ TÍNH NĂNG 4: Panel tìm kiếm nâng cao
   Panel panelSearch = new Panel
         {
   Dock = DockStyle.Top,
       Height = 65,
  BackColor = Color.White,
    Padding = new Padding(15)
    };

     // Search by patient name
   Label lblSearch = new Label
            {
           Text = "🔍 Tìm bệnh nhân:",
  Location = new Point(15, 20),
    AutoSize = true,
   Font = new Font("Segoe UI", 9)
         };

  txtSearchPatient = new TextBox
            {
      Location = new Point(130, 17),
 Size = new Size(180, 25),
          Font = new Font("Segoe UI", 9)
       };
  txtSearchPatient.TextChanged += (s, e) => LoadAllAppointments();

  // Filter by service
   Label lblService = new Label
       {
 Text = "Dịch vụ:",
      Location = new Point(330, 20),
   AutoSize = true,
        Font = new Font("Segoe UI", 9)
     };

  cboServiceFilter = new ComboBox
   {
         Location = new Point(390, 17),
     Size = new Size(180, 25),
 DropDownStyle = ComboBoxStyle.DropDownList,
       Font = new Font("Segoe UI", 9)
      };
       LoadServiceFilter();
    cboServiceFilter.SelectedIndexChanged += (s, e) => LoadAllAppointments();

    // Date filter
     Label lblDate = new Label
   {
    Text = "Ngày:",
    Location = new Point(590, 20),
    AutoSize = true,
      Font = new Font("Segoe UI", 9)
     };

      dtpDate.Location = new Point(635, 17);
  dtpDate.Size = new Size(120, 25);

  // Today button
   btnToday.Location = new Point(765, 15);
      btnToday.Size = new Size(80, 28);
  btnToday.Text = "Hôm nay";

         panelSearch.Controls.AddRange(new Control[] {
     lblSearch, txtSearchPatient,
        lblService, cboServiceFilter,
     lblDate, dtpDate, btnToday
    });

      this.Controls.Add(tabControl);
       this.Controls.Add(panelSearch);
   this.Controls.Add(notificationPanel);

         tabControl.SelectedIndexChanged += (s, e) => LoadAllAppointments();
        }

        /// <summary>
        /// ✅ Initialize pagination for all 3 tabs (12 rows per page)
        /// </summary>
        private void InitializePagination()
        {
            // Pagination for Tab 1: Yêu cầu khám mới
            paginationPending = new SimplePagination(dgvPending, 12);
            Panel paginationPanelPending = paginationPending.GetPaginationPanel();
            paginationPanelPending.Dock = DockStyle.Bottom;
            tabPending.Controls.Add(paginationPanelPending);

            // Pagination for Tab 2: Đã xác nhận
            paginationConfirmed = new SimplePagination(dgvConfirmed, 12);
            Panel paginationPanelConfirmed = paginationConfirmed.GetPaginationPanel();
            paginationPanelConfirmed.Dock = DockStyle.Bottom;
            tabConfirmed.Controls.Add(paginationPanelConfirmed);

            // Pagination for Tab 3: Lịch sử
            paginationHistory = new SimplePagination(dgvHistory, 12);
            Panel paginationPanelHistory = paginationHistory.GetPaginationPanel();
            paginationPanelHistory.Dock = DockStyle.Bottom;
            tabHistory.Controls.Add(paginationPanelHistory);
        }

      private void LoadServiceFilter()
    {
        try
      {
       cboServiceFilter.Items.Clear();
 cboServiceFilter.Items.Add("-- Tất cả dịch vụ --");

  string query = "SELECT service_name FROM Service WHERE status = 'available' ORDER BY service_name";
    DataTable dt = DatabaseHelper.ExecuteQuery(query);

   foreach (DataRow row in dt.Rows)
     {
       cboServiceFilter.Items.Add(row["service_name"].ToString());
   }

     cboServiceFilter.SelectedIndex = 0;
   }
  catch { }
 }

        private DataGridView CreateDataGridView()
        {
        var dgv = new DataGridView
            {
     Dock = DockStyle.Fill,
     BackgroundColor = Color.White,
     AllowUserToAddRows = false,
   ReadOnly = true,
     AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
         SelectionMode = DataGridViewSelectionMode.FullRowSelect,
    BorderStyle = BorderStyle.None,
    RowHeadersVisible = false
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

   /// <summary>
   /// Lấy điều kiện tìm kiếm chung
        /// </summary>
  private string GetSearchFilter(out SqlParameter[] extraParams)
  {
    string filter = "";
        var paramList = new System.Collections.Generic.List<SqlParameter>();

   // Search by patient name
       if (txtSearchPatient != null && !string.IsNullOrWhiteSpace(txtSearchPatient.Text))
       {
    filter += " AND (a.patient_name LIKE @search OR u.fullname LIKE @search OR a.phone LIKE @search)";
     paramList.Add(new SqlParameter("@search", $"%{txtSearchPatient.Text.Trim()}%"));
    }

     // Filter by service
     if (cboServiceFilter != null && cboServiceFilter.SelectedIndex > 0)
  {
    filter += " AND s.service_name = @serviceName";
  paramList.Add(new SqlParameter("@serviceName", cboServiceFilter.SelectedItem.ToString()));
   }

  extraParams = paramList.ToArray();
   return filter;
        }

      private void LoadPendingAppointments()
        {
       try
  {
       string searchFilter = GetSearchFilter(out SqlParameter[] extraParams);

             string query = $@"
       SELECT
    a.appointment_id AS [ID],
     a.patient_id AS [PatientID],
          CASE 
    WHEN p.patient_id IS NOT NULL THEN u.fullname
       ELSE a.patient_name
     END AS [Bệnh nhân],
  a.phone AS [SĐT],
            ISNULL(s.service_name, 'N/A') AS [Dịch vụ],
     FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
   ISNULL(a.notes, '') AS [Ghi chú]
              FROM Appointment a
      LEFT JOIN Service s ON a.service_id = s.service_id
  LEFT JOIN Patient p ON a.patient_id = p.patient_id
  LEFT JOIN UserAccount u ON p.user_id = u.user_id
   WHERE a.assigned_doctor_id = @doctorId
    AND a.status = 'Booked'
    AND a.appointment_date >= GETDATE()
  {searchFilter}
    ORDER BY a.appointment_date";

     var allParams = new System.Collections.Generic.List<SqlParameter>
       {
    new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0)
     };
     allParams.AddRange(extraParams);

  DataTable dt = DatabaseHelper.ExecuteQuery(query, allParams.ToArray());

      if (dt == null || dgvPending == null) return;

  paginationPending.SetDataSource(dt); // ✅ Use pagination
      
  if (dgvPending.Columns.Count > 0)
     {
       if (dgvPending.Columns["ID"] != null) dgvPending.Columns["ID"].Visible = false;
   if (dgvPending.Columns["PatientID"] != null) dgvPending.Columns["PatientID"].Visible = false;
       
       // Set column widths
       if (dgvPending.Columns["Bệnh nhân"] != null) dgvPending.Columns["Bệnh nhân"].Width = 180;
       if (dgvPending.Columns["SĐT"] != null) dgvPending.Columns["SĐT"].Width = 120;
       if (dgvPending.Columns["Dịch vụ"] != null) dgvPending.Columns["Dịch vụ"].Width = 200;
       if (dgvPending.Columns["Thời gian"] != null) dgvPending.Columns["Thời gian"].Width = 150;
       if (dgvPending.Columns["Ghi chú"] != null) dgvPending.Columns["Ghi chú"].Width = 250;
      }

     // Add action buttons
     if (!dgvPending.Columns.Contains("StartExam"))
{
         dgvPending.Columns.Add(new DataGridViewButtonColumn
  {
   Name = "StartExam",
   HeaderText = "",
              Text = "🩺 Khám ngay",
    UseColumnTextForButtonValue = true,
           Width = 120
        });

         dgvPending.Columns.Add(new DataGridViewButtonColumn
  {
   Name = "Reject",
    HeaderText = "",
   Text = "❌ Hủy",
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

    // Update notification count
  UpdateNotificationBadge(dt.Rows.Count);
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
    string searchFilter = GetSearchFilter(out SqlParameter[] extraParams);

        string query = $@"
            SELECT
    a.appointment_id AS [ID],
    a.patient_id AS [PatientID],
             CASE 
    WHEN p.patient_id IS NOT NULL THEN u.fullname
   ELSE a.patient_name
         END AS [Bệnh nhân],
              a.phone AS [SĐT],
  ISNULL(s.service_name, 'N/A') AS [Dịch vụ],
   FORMAT(a.appointment_date, 'HH:mm') AS [Giờ hẹn],
  ISNULL(a.notes, '') AS [Ghi chú]
      FROM Appointment a
   LEFT JOIN Service s ON a.service_id = s.service_id
    LEFT JOIN Patient p ON a.patient_id = p.patient_id
       LEFT JOIN UserAccount u ON p.user_id = u.user_id
       WHERE a.assigned_doctor_id = @doctorId
       AND a.status = 'Booked'
         AND CAST(a.appointment_date AS DATE) = @date
     {searchFilter}
     ORDER BY a.appointment_date";

  var allParams = new System.Collections.Generic.List<SqlParameter>
        {
       new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0),
  new SqlParameter("@date", dtpDate.Value.Date)
 };
   allParams.AddRange(extraParams);

   DataTable dt = DatabaseHelper.ExecuteQuery(query, allParams.ToArray());

   if (dt == null || dgvConfirmed == null) return;

    paginationConfirmed.SetDataSource(dt); // ✅ Use pagination
   
if (dgvConfirmed.Columns.Count > 0)
     {
         if (dgvConfirmed.Columns["ID"] != null) dgvConfirmed.Columns["ID"].Visible = false;
if (dgvConfirmed.Columns["PatientID"] != null) dgvConfirmed.Columns["PatientID"].Visible = false;
         
         // Set column widths
         if (dgvConfirmed.Columns["Bệnh nhân"] != null) dgvConfirmed.Columns["Bệnh nhân"].Width = 200;
         if (dgvConfirmed.Columns["SĐT"] != null) dgvConfirmed.Columns["SĐT"].Width = 120;
         if (dgvConfirmed.Columns["Dịch vụ"] != null) dgvConfirmed.Columns["Dịch vụ"].Width = 220;
         if (dgvConfirmed.Columns["Giờ hẹn"] != null) dgvConfirmed.Columns["Giờ hẹn"].Width = 100;
         if (dgvConfirmed.Columns["Ghi chú"] != null) dgvConfirmed.Columns["Ghi chú"].Width = 280;
      }

     if (!dgvConfirmed.Columns.Contains("StartExam"))
    {
dgvConfirmed.Columns.Add(new DataGridViewButtonColumn
        {
          Name = "StartExam",
 HeaderText = "",
 Text = "🩺 Khám ngay",
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
             string searchFilter = GetSearchFilter(out SqlParameter[] extraParams);

      string query = $@"
     SELECT
   a.appointment_id AS [ID],
   CASE 
                 WHEN p.patient_id IS NOT NULL THEN u.fullname
             ELSE a.patient_name
           END AS [Bệnh nhân],
       a.phone AS [SĐT],
          ISNULL(s.service_name, 'N/A') AS [Dịch vụ],
    FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
         CASE 
       WHEN a.status = 'Completed' THEN N'✅ Hoàn thành'
   WHEN a.status = 'Cancelled' THEN N'❌ Đã hủy'
       ELSE a.status
      END AS [Trạng thái],
 ISNULL(a.reject_reason, '') AS [Ghi chú]
       FROM Appointment a
          LEFT JOIN Service s ON a.service_id = s.service_id
             LEFT JOIN Patient p ON a.patient_id = p.patient_id
        LEFT JOIN UserAccount u ON p.user_id = u.user_id
    WHERE a.assigned_doctor_id = @doctorId
 AND a.status IN ('Completed', 'Cancelled')
        AND CAST(a.appointment_date AS DATE) = @date
   {searchFilter}
              ORDER BY a.appointment_date DESC";

   var allParams = new System.Collections.Generic.List<SqlParameter>
   {
      new SqlParameter("@doctorId", Auth.CurrentStaffId ?? 0),
         new SqlParameter("@date", dtpDate.Value.Date)
   };
      allParams.AddRange(extraParams);

   DataTable dt = DatabaseHelper.ExecuteQuery(query, allParams.ToArray());

      if (dt == null || dgvHistory == null) return;

  paginationHistory.SetDataSource(dt); // ✅ Use pagination
  
       if (dgvHistory.Columns.Count > 0)
        {
         if (dgvHistory.Columns["ID"] != null) dgvHistory.Columns["ID"].Visible = false;
         
         // Set column widths
         if (dgvHistory.Columns["Bệnh nhân"] != null) dgvHistory.Columns["Bệnh nhân"].Width = 180;
         if (dgvHistory.Columns["SĐT"] != null) dgvHistory.Columns["SĐT"].Width = 120;
         if (dgvHistory.Columns["Dịch vụ"] != null) dgvHistory.Columns["Dịch vụ"].Width = 200;
         if (dgvHistory.Columns["Thời gian"] != null) dgvHistory.Columns["Thời gian"].Width = 150;
         if (dgvHistory.Columns["Trạng thái"] != null) dgvHistory.Columns["Trạng thái"].Width = 130;
         if (dgvHistory.Columns["Ghi chú"] != null) dgvHistory.Columns["Ghi chú"].Width = 250;
        }

       foreach (DataGridViewRow row in dgvHistory.Rows)
    {
             if (!row.IsNewRow)
          {
 string status = row.Cells["Trạng thái"].Value?.ToString() ?? "";
       if (status.Contains("Hoàn thành"))
          row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
 else if (status.Contains("Đã hủy"))
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
       
  if (!dgv.Columns.Contains("ID") || dgv.Rows[e.RowIndex].Cells["ID"].Value == null)
            return;
      
    int appointmentId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value);
     
            // Lấy PatientID nếu có
   int? patientId = null;
   if (dgv.Columns.Contains("PatientID") && dgv.Rows[e.RowIndex].Cells["PatientID"].Value != DBNull.Value)
 {
    patientId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["PatientID"].Value);
            }
  
       string columnName = dgv.Columns[e.ColumnIndex].Name;

          if (columnName == "Reject")
     {
             RejectAppointment(appointmentId);
  }
          else if (columnName == "StartExam")
   {
  // ✅ TÍNH NĂNG 6: Link trực tiếp đến DoctorExamine với data
     StartExaminationWithPatient(appointmentId, patientId);
        }
        }

   /// <summary>
   /// ✅ TÍNH NĂNG 6: Mở DoctorExamine và tự động chọn bệnh nhân
       /// </summary>
        private void StartExaminationWithPatient(int appointmentId, int? patientId)
        {
            if (!patientId.HasValue)
 {
           MessageBoxHelper.ShowWarning(
    "Bệnh nhân này chưa được đăng ký trong hệ thống.\n\n" +
   "Vui lòng yêu cầu nhân viên lễ tân kê khai thông tin trước khi khám.");
           return;
      }

            // Lưu thông tin appointment đang xử lý vào context
   DoctorExamineContext.CurrentAppointmentId = appointmentId;
DoctorExamineContext.CurrentPatientId = patientId.Value;
            DoctorExamineContext.ShouldAutoSelect = true;
            
            // Tìm frmMain và chuyển sang tab Khám bệnh
            Form mainForm = this.FindForm();
            if (mainForm != null && mainForm is frmMain frmMain)
            {
                frmMain.NavigateToExamine();
            }
            else
            {
                MessageBoxHelper.ShowInfo(
                    $"📋 Lịch hẹn #{appointmentId}\n\n" +
                    "Vui lòng chuyển sang tab 'Khám bệnh' để tiếp tục.\n" +
                    "Bệnh nhân sẽ được chọn tự động.");
            }
     
  Logger.LogAction("START_EXAMINATION", $"Doctor starting exam for appointment #{appointmentId}, patient #{patientId}");
   }

        /// <summary>
     /// Event để yêu cầu navigate đến trang khám bệnh
        /// </summary>
        public static event EventHandler<int> OnNavigateToExamine;

  private void RejectAppointment(int appointmentId)
    {
    using (Form form = new Form())
     {
     form.Text = "Lý do hủy lịch";
               form.Size = new Size(450, 250);
   form.StartPosition = FormStartPosition.CenterParent;
      form.FormBorderStyle = FormBorderStyle.FixedDialog;
       form.MaximizeBox = false;
         form.MinimizeBox = false;

          Label lbl = new Label
 {
    Text = "Vui lòng nhập lý do hủy lịch hẹn:",
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
        Text = "Xác nhận hủy",
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
     MessageBoxHelper.ShowValidationError("Vui lòng nhập lý do!");
         return;
  }

  try
           {
            // Lấy email bệnh nhân để gửi thông báo
               string getEmailQuery = @"SELECT a.email, a.patient_name, a.appointment_date 
           FROM Appointment a WHERE a.appointment_id = @id";
    DataTable dtEmail = DatabaseHelper.ExecuteQuery(getEmailQuery, new SqlParameter[] {
        new SqlParameter("@id", appointmentId)
     });

  string query = @"UPDATE Appointment 
       SET status = 'Cancelled', reject_reason = @reason
      WHERE appointment_id = @id";

      int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
   new SqlParameter("@id", appointmentId),
      new SqlParameter("@reason", reason)
        });

   if (result > 0)
          {
   // ✅ TÍNH NĂNG 5: Gửi email thông báo hủy lịch
         if (dtEmail.Rows.Count > 0)
   {
          string email = dtEmail.Rows[0]["email"]?.ToString();
           string patientName = dtEmail.Rows[0]["patient_name"]?.ToString();
   DateTime appointmentDate = Convert.ToDateTime(dtEmail.Rows[0]["appointment_date"]);

      if (!string.IsNullOrEmpty(email))
          {
   // Gửi email async
         _ = EmailService.SendAppointmentCancellationAsync(
  email, patientName, appointmentDate, reason);
         }
}

          MessageBoxHelper.ShowSuccess("Đã hủy lịch hẹn!");
            Logger.LogAction("CANCEL_APPOINTMENT", $"Doctor cancelled appointment #{appointmentId}: {reason}");
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
           form.ShowDialog(this.FindForm());
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

    /// <summary>
    /// Context để truyền dữ liệu giữa DoctorAppointments và DoctorExamine
    /// </summary>
    public static class DoctorExamineContext
    {
        public static int? CurrentAppointmentId { get; set; }
  public static int? CurrentPatientId { get; set; }
        public static bool ShouldAutoSelect { get; set; }

  public static void Clear()
        {
      CurrentAppointmentId = null;
            CurrentPatientId = null;
 ShouldAutoSelect = false;
}
    }
}
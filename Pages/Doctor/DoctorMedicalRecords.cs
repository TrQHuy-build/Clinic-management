using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Services;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorMedicalRecords : UserControl
    {
   // ✅ Cho phép chỉnh sửa trong vòng 24 giờ
        private const int EDIT_ALLOWED_HOURS = 24;
        private SimplePagination pagination;

        public DoctorMedicalRecords()
        {
            InitializeComponent();
   InitializeAdvancedSearch();
    InitializeEvents();
    InitializePagination();
            LoadRecords();
        }
        
        private void InitializePagination()
        {
            pagination = new SimplePagination(dgvRecords, 12);
            Panel paginationPanel = pagination.GetPaginationPanel();
            paginationPanel.Dock = DockStyle.None;
            paginationPanel.Height = 45;
            
            // Đặt pagination panel ở dưới cùng, điều chỉnh theo dgvRecords
            this.Resize += (s, e) => PositionPaginationPanel(paginationPanel);
            this.Load += (s, e) => PositionPaginationPanel(paginationPanel);
            
            this.Controls.Add(paginationPanel);
            paginationPanel.BringToFront();
        }
        
        private void PositionPaginationPanel(Panel paginationPanel)
        {
            // Đặt pagination panel ngay dưới dgvRecords
            paginationPanel.Location = new Point(dgvRecords.Left, dgvRecords.Bottom + 5);
            paginationPanel.Width = dgvRecords.Width;
            
            // Giảm chiều cao dgvRecords để có chỗ cho pagination
            int availableHeight = this.Height - 170 - paginationPanel.Height - 30; // 170 = top area
            if (availableHeight > 100)
            {
                dgvRecords.Height = availableHeight;
                paginationPanel.Location = new Point(dgvRecords.Left, dgvRecords.Bottom + 5);
            }
        }

        /// <summary>
      /// Khởi tạo bộ lọc tìm kiếm nâng cao
        /// </summary>
        private void InitializeAdvancedSearch()
        {
            // Thêm panel filter nếu chưa có
            if (this.Controls.Find("panelFilter", false).Length == 0)
            {
              Panel panelFilter = new Panel
   {
              Name = "panelFilter",
Dock = DockStyle.Top,
          Height = 80,
         BackColor = Color.White,
        Padding = new Padding(10)
    };

       // Row 1: Search + Date filters
         Label lblSearch = new Label
           {
  Text = "🔍 Tìm kiếm:",
   Location = new Point(10, 15),
    AutoSize = true,
          Font = new Font("Segoe UI", 9)
       };

     TextBox txtAdvSearch = new TextBox
 {
            Name = "txtAdvSearch",
       Location = new Point(90, 12),
      Size = new Size(200, 25),
   Font = new Font("Segoe UI", 9)
                };
          txtAdvSearch.TextChanged += (s, e) => LoadRecords();

      Label lblFrom = new Label
      {
         Text = "Từ ngày:",
        Location = new Point(310, 15),
        AutoSize = true,
         Font = new Font("Segoe UI", 9)
                };

       DateTimePicker dtpFrom = new DateTimePicker
      {
         Name = "dtpFrom",
        Location = new Point(370, 12),
            Size = new Size(120, 25),
          Format = DateTimePickerFormat.Short,
            Value = DateTime.Today.AddMonths(-1),
    Font = new Font("Segoe UI", 9)
 };
    dtpFrom.ValueChanged += (s, e) => LoadRecords();

              Label lblTo = new Label
      {
 Text = "Đến ngày:",
          Location = new Point(500, 15),
      AutoSize = true,
   Font = new Font("Segoe UI", 9)
           };

        DateTimePicker dtpTo = new DateTimePicker
        {
    Name = "dtpTo",
             Location = new Point(570, 12),
         Size = new Size(120, 25),
         Format = DateTimePickerFormat.Short,
         Value = DateTime.Today,
    Font = new Font("Segoe UI", 9)
   };
           dtpTo.ValueChanged += (s, e) => LoadRecords();

         // Row 2: Buttons
          Button btnReset = new Button
    {
        Text = "🔄 Reset",
        Location = new Point(10, 45),
        Size = new Size(80, 28),
    BackColor = Color.LightGray,
                 FlatStyle = FlatStyle.Flat,
         Font = new Font("Segoe UI", 9),
     Cursor = Cursors.Hand
        };
       btnReset.FlatAppearance.BorderSize = 0;
           btnReset.Click += (s, e) =>
     {
     txtAdvSearch.Clear();
          dtpFrom.Value = DateTime.Today.AddMonths(-1);
      dtpTo.Value = DateTime.Today;
       LoadRecords();
      };

    Button btnExportCsv = new Button
              {
   Name = "btnExportCsv",
        Text = "📊 Xuất Excel",
  Location = new Point(100, 45),
   Size = new Size(110, 28),
   BackColor = ColorTranslator.FromHtml("#28A745"),
  ForeColor = Color.White,
 FlatStyle = FlatStyle.Flat,
          Font = new Font("Segoe UI", 9),
   Cursor = Cursors.Hand
       };
     btnExportCsv.FlatAppearance.BorderSize = 0;
 btnExportCsv.Click += BtnExportCsv_Click;

         Button btnExportHtml = new Button
            {
              Name = "btnExportHtml",
     Text = "📄 Xuất PDF/HTML",
     Location = new Point(220, 45),
 Size = new Size(130, 28),
 BackColor = ColorTranslator.FromHtml("#17A2B8"),
   ForeColor = Color.White,
  FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
    Cursor = Cursors.Hand
                };
      btnExportHtml.FlatAppearance.BorderSize = 0;
                btnExportHtml.Click += BtnExportHtml_Click;

      // Label hiển thị tổng số
         Label lblCount = new Label
     {
  Name = "lblCount",
          Text = "Tổng: 0 hồ sơ",
          Location = new Point(570, 50),
         AutoSize = true,
   Font = new Font("Segoe UI", 9, FontStyle.Bold),
           ForeColor = ColorTranslator.FromHtml("#667eea")
 };

    panelFilter.Controls.AddRange(new Control[] {
        lblSearch, txtAdvSearch, lblFrom, dtpFrom, lblTo, dtpTo,
   btnReset, btnExportCsv, btnExportHtml, lblCount
                });

     this.Controls.Add(panelFilter);
        panelFilter.BringToFront();
      }
      }

      private void InitializeEvents()
      {
          // ✅ FIX: Remove existing event handler trước khi đăng ký mới để tránh duplicate
          dgvRecords.CellClick -= DgvRecords_CellClick;
          dgvRecords.CellClick += DgvRecords_CellClick;
        }

        private void LoadRecords()
        {
            if (!Auth.CurrentStaffId.HasValue) return;

            try
   {
          // Lấy giá trị từ controls
       string search = "";
           DateTime fromDate = DateTime.Today.AddMonths(-1);
    DateTime toDate = DateTime.Today;

      var txtAdvSearch = this.Controls.Find("txtAdvSearch", true);
  var dtpFrom = this.Controls.Find("dtpFrom", true);
    var dtpTo = this.Controls.Find("dtpTo", true);

           if (txtAdvSearch.Length > 0)
       search = ((TextBox)txtAdvSearch[0]).Text.Trim();

          if (dtpFrom.Length > 0)
   fromDate = ((DateTimePicker)dtpFrom[0]).Value.Date;

 if (dtpTo.Length > 0)
    toDate = ((DateTimePicker)dtpTo[0]).Value.Date;

              // Thêm placeholder check nếu dùng txtSearch cũ
           if (txtSearch != null)
     {
      const string placeholder = "Tìm theo tên bệnh nhân...";
    if (!string.IsNullOrEmpty(txtSearch.Text) && 
          txtSearch.Text != placeholder && 
          txtSearch.ForeColor != Color.Gray)
   {
     search = txtSearch.Text.Trim();
   }
        }

                string query = @"
    SELECT
 m.record_id AS [ID],
 u.fullname AS [Bệnh nhân],
           u.phone AS [SĐT],
          m.diagnosis AS [Chẩn đoán],
    m.treatment AS [Điều trị],
             FORMAT(m.record_date, 'dd/MM/yyyy HH:mm') AS [Ngày khám],
  m.record_date AS [record_date_raw]
         FROM MedicalRecord m
      INNER JOIN Patient p ON m.patient_id = p.patient_id
  INNER JOIN UserAccount u ON p.user_id = u.user_id
      WHERE m.staff_id = @staffId 
        AND (u.fullname LIKE @search OR m.diagnosis LIKE @search OR u.phone LIKE @search)
 AND CAST(m.record_date AS DATE) BETWEEN @fromDate AND @toDate
     ORDER BY m.record_date DESC";

          var parameters = new[]
         {
      new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
         new SqlParameter("@search", $"%{search}%"),
  new SqlParameter("@fromDate", fromDate),
         new SqlParameter("@toDate", toDate)
       };

        DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
        
        // ✅ Sử dụng pagination thay vì bind trực tiếp
        pagination.SetDataSource(dt);

      // Ẩn cột không cần thiết
 if (dgvRecords.Columns["ID"] != null)
       dgvRecords.Columns["ID"].Visible = false;

             if (dgvRecords.Columns["record_date_raw"] != null)
   dgvRecords.Columns["record_date_raw"].Visible = false;

  // Thêm các nút action
    AddActionButtons();

  // Cập nhật label count
     var lblCount = this.Controls.Find("lblCount", true);
     if (lblCount.Length > 0)
            ((Label)lblCount[0]).Text = $"Tổng: {dt.Rows.Count} hồ sơ";
            }
       catch (Exception ex)
    {
      MessageBoxHelper.ShowError($"Lỗi tải hồ sơ: {ex.Message}");
    }
        }

   private void AddActionButtons()
   {
            // Nút Xem chi tiết
        if (!dgvRecords.Columns.Contains("View"))
     {
           var btnViewCol = new DataGridViewButtonColumn
  {
          Name = "View",
            HeaderText = "",
Text = "👁 Xem",
    UseColumnTextForButtonValue = true,
    Width = 70
         };
     dgvRecords.Columns.Add(btnViewCol);
         }

     // Nút Sửa (chỉ hiển thị cho HSBA trong 24h)
            if (!dgvRecords.Columns.Contains("Edit"))
            {
 var btnEditCol = new DataGridViewButtonColumn
   {
       Name = "Edit",
 HeaderText = "",
Text = "✏ Sửa",
       UseColumnTextForButtonValue = true,
       Width = 70
           };
      dgvRecords.Columns.Add(btnEditCol);
     }

            // Cập nhật trạng thái nút Sửa dựa trên thời gian
            foreach (DataGridViewRow row in dgvRecords.Rows)
            {
    if (row.IsNewRow) continue;

                var dateCell = row.Cells["record_date_raw"];
     if (dateCell?.Value != null && DateTime.TryParse(dateCell.Value.ToString(), out DateTime recordDate))
 {
        bool canEdit = (DateTime.Now - recordDate).TotalHours <= EDIT_ALLOWED_HOURS;
        
    if (row.Cells["Edit"] is DataGridViewButtonCell editCell)
          {
            if (!canEdit)
         {
editCell.Value = "🔒 Khóa";
         editCell.Style.BackColor = Color.LightGray;
    editCell.Style.ForeColor = Color.Gray;
           }
else
         {
            // Highlight row có thể sửa
                  row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                  }
                }
    }
            }
        }

    private void DgvRecords_CellClick(object sender, DataGridViewCellEventArgs e)
      {
       if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var idCell = dgvRecords.Rows[e.RowIndex].Cells["ID"];
    if (idCell?.Value == null) return;

            int recordId = Convert.ToInt32(idCell.Value);
            string columnName = dgvRecords.Columns[e.ColumnIndex].Name;

     if (columnName == "View")
     {
            ShowRecordDetail(recordId);
            }
 else if (columnName == "Edit")
            {
     // Kiểm tra thời gian
    var dateCell = dgvRecords.Rows[e.RowIndex].Cells["record_date_raw"];
     if (dateCell?.Value != null && DateTime.TryParse(dateCell.Value.ToString(), out DateTime recordDate))
     {
       if ((DateTime.Now - recordDate).TotalHours > EDIT_ALLOWED_HOURS)
           {
                   MessageBoxHelper.ShowWarning(
  $"Không thể chỉnh sửa hồ sơ này!\n\n" +
$"Hồ sơ chỉ có thể chỉnh sửa trong vòng {EDIT_ALLOWED_HOURS} giờ sau khi tạo.\n" +
            $"Hồ sơ này được tạo lúc: {recordDate:dd/MM/yyyy HH:mm}");
     return;
         }
    }

         ShowEditRecordForm(recordId);
            }
     }

     /// <summary>
        /// ✅ TÍNH NĂNG MỚI: Form chỉnh sửa hồ sơ bệnh án
      /// </summary>
 private void ShowEditRecordForm(int recordId)
  {
        // Load dữ liệu hiện tại
          string query = @"
  SELECT m.*, u.fullname AS patient_name
                FROM MedicalRecord m
      INNER JOIN Patient p ON m.patient_id = p.patient_id
INNER JOIN UserAccount u ON p.user_id = u.user_id
         WHERE m.record_id = @id";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new[] { new SqlParameter("@id", recordId) });
            if (dt.Rows.Count == 0)
        {
     MessageBoxHelper.ShowError("Không tìm thấy hồ sơ!");
                return;
   }

            var dr = dt.Rows[0];
            string patientName = dr["patient_name"]?.ToString() ?? "N/A";
    DateTime recordDate = Convert.ToDateTime(dr["record_date"]);
       string currentDiagnosis = dr["diagnosis"]?.ToString() ?? "";
            string currentTreatment = dr["treatment"]?.ToString() ?? "";

      using (Form form = new Form())
            {
         form.Text = $"✏ Chỉnh sửa hồ sơ #{recordId}";
          form.Size = new Size(600, 500);
      form.StartPosition = FormStartPosition.CenterParent;
  form.FormBorderStyle = FormBorderStyle.FixedDialog;
 form.MaximizeBox = false;
    form.MinimizeBox = false;

       // Header info
     Label lblHeader = new Label
           {
           Text = $"🏥 Bệnh nhân: {patientName}\n📅 Ngày khám: {recordDate:dd/MM/yyyy HH:mm}",
  Location = new Point(20, 20),
           Size = new Size(550, 50),
         Font = new Font("Segoe UI", 11),
    BackColor = ColorTranslator.FromHtml("#E3F2FD"),
         Padding = new Padding(10)
     };

           // Countdown timer
    TimeSpan remaining = TimeSpan.FromHours(EDIT_ALLOWED_HOURS) - (DateTime.Now - recordDate);
      Label lblTimer = new Label
   {
      Text = $"⏱ Thời gian còn lại để sửa: {remaining.Hours}h {remaining.Minutes}m",
    Location = new Point(20, 80),
   AutoSize = true,
    Font = new Font("Segoe UI", 9, FontStyle.Italic),
        ForeColor = remaining.TotalHours < 2 ? Color.Red : Color.Green
 };

   // Diagnosis
    Label lblDiagnosis = new Label
 {
             Text = "Chẩn đoán:",
Location = new Point(20, 115),
 AutoSize = true,
          Font = new Font("Segoe UI", 10, FontStyle.Bold)
   };

           TextBox txtDiagnosis = new TextBox
    {
        Text = currentDiagnosis,
   Location = new Point(20, 140),
           Size = new Size(540, 100),
    Multiline = true,
  ScrollBars = ScrollBars.Vertical,
    Font = new Font("Segoe UI", 10)
 };

    // Treatment
      Label lblTreatment = new Label
                {
        Text = "Điều trị:",
        Location = new Point(20, 255),
      AutoSize = true,
             Font = new Font("Segoe UI", 10, FontStyle.Bold)
  };

          TextBox txtTreatment = new TextBox
       {
                Text = currentTreatment,
     Location = new Point(20, 280),
         Size = new Size(540, 100),
          Multiline = true,
              ScrollBars = ScrollBars.Vertical,
  Font = new Font("Segoe UI", 10)
     };

     // Buttons
Button btnSave = new Button
      {
      Text = "💾 Lưu thay đổi",
      Location = new Point(150, 400),
   Size = new Size(140, 40),
        BackColor = ColorTranslator.FromHtml("#28A745"),
      ForeColor = Color.White,
          FlatStyle = FlatStyle.Flat,
       Font = new Font("Segoe UI", 10, FontStyle.Bold),
        Cursor = Cursors.Hand
         };
   btnSave.FlatAppearance.BorderSize = 0;

   Button btnCancel = new Button
   {
       Text = "❌ Hủy",
 Location = new Point(310, 400),
      Size = new Size(100, 40),
         BackColor = Color.Gray,
       ForeColor = Color.White,
         FlatStyle = FlatStyle.Flat,
        Font = new Font("Segoe UI", 10),
             Cursor = Cursors.Hand
    };
                btnCancel.FlatAppearance.BorderSize = 0;
    btnCancel.Click += (s, ev) => form.Close();

                btnSave.Click += (s, ev) =>
    {
         string newDiagnosis = txtDiagnosis.Text.Trim();
     string newTreatment = txtTreatment.Text.Trim();

     if (string.IsNullOrWhiteSpace(newDiagnosis))
     {
            MessageBoxHelper.ShowValidationError("Chẩn đoán không được để trống!");
 txtDiagnosis.Focus();
    return;
     }

        // Kiểm tra lại thời gian trước khi lưu
           if ((DateTime.Now - recordDate).TotalHours > EDIT_ALLOWED_HOURS)
         {
            MessageBoxHelper.ShowError("Hết thời gian cho phép chỉnh sửa!");
            form.Close();
    return;
             }

            try
        {
     string updateQuery = @"
      UPDATE MedicalRecord 
        SET diagnosis = @diagnosis, 
     treatment = @treatment,
         updated_at = GETDATE()
      WHERE record_id = @id";

               int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[] {
     new SqlParameter("@diagnosis", newDiagnosis),
       new SqlParameter("@treatment", string.IsNullOrWhiteSpace(newTreatment) ? (object)DBNull.Value : newTreatment),
           new SqlParameter("@id", recordId)
 });

    if (result > 0)
                   {
                MessageBoxHelper.ShowSuccess("Cập nhật hồ sơ thành công!");
           Logger.LogAction("UPDATE_MEDICAL_RECORD", $"Updated record #{recordId}");
      form.Close();
          LoadRecords();
     }
      else
   {
        MessageBoxHelper.ShowError("Không thể cập nhật hồ sơ!");
       }
       }
          catch (Exception ex)
        {
   MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
    }
   };

   form.Controls.AddRange(new Control[] {
          lblHeader, lblTimer, lblDiagnosis, txtDiagnosis,
        lblTreatment, txtTreatment, btnSave, btnCancel
  });

       form.ShowDialog(this.FindForm());
            }
    }

        private void ShowRecordDetail(int recordId)
 {
     var form = new Form
            {
         Text = $"Chi tiết hồ sơ #{recordId}",
     Size = new Size(750, 650),
 StartPosition = FormStartPosition.CenterParent,
    FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
        MinimizeBox = false
       };

          string query = @"
                SELECT m.*, u.fullname AS patient_name, u.phone, u.email
    FROM MedicalRecord m
       INNER JOIN Patient p ON m.patient_id = p.patient_id
      INNER JOIN UserAccount u ON p.user_id = u.user_id
     WHERE m.record_id = @id";

         DataTable dt = DatabaseHelper.ExecuteQuery(query, new[] { new SqlParameter("@id", recordId) });
     if (dt.Rows.Count == 0)
 {
              form.Dispose();
                return;
         }

            var dr = dt.Rows[0];

   var lblInfo = new Label
            {
      Text = $"👤 Bệnh nhân: {dr["patient_name"]}\n" +
    $"📞 SĐT: {dr["phone"]}\n" +
           $"📅 Ngày khám: {Convert.ToDateTime(dr["record_date"]):dd/MM/yyyy HH:mm}\n\n" +
      $"🔬 Chẩn đoán:\n{dr["diagnosis"]}\n\n" +
    $"💊 Điều trị:\n{dr["treatment"]}",
    Location = new Point(20, 20),
 Size = new Size(700, 180),
         Font = new Font("Segoe UI", 11),
           AutoSize = false
            };

       var lblPres = new Label
            {
     Text = "💊 Đơn thuốc:",
      Font = new Font("Segoe UI", 12, FontStyle.Bold),
     Location = new Point(20, 210),
        AutoSize = true
    };

            var dgvPres = new DataGridView
       {
                Location = new Point(20, 240),
             Size = new Size(700, 150),
         BackgroundColor = Color.White,
    AllowUserToAddRows = false,
         ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None
          };

            string presQuery = @"
 SELECT
 m.name AS [Tên thuốc],
                    p.dosage AS [Liều lượng],
  p.quantity AS [Số lượng],
     p.notes AS [Ghi chú]
            FROM Prescription p
             INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
                WHERE p.record_id = @id";

          DataTable dtPres = DatabaseHelper.ExecuteQuery(presQuery, new[] { new SqlParameter("@id", recordId) });
       dgvPres.DataSource = dtPres;

       // Services used
         var lblServices = new Label
{
      Text = "🏥 Dịch vụ đã sử dụng:",
      Font = new Font("Segoe UI", 12, FontStyle.Bold),
      Location = new Point(20, 400),
      AutoSize = true
    };

     var dgvServices = new DataGridView
         {
 Location = new Point(20, 430),
        Size = new Size(700, 120),
  BackgroundColor = Color.White,
            AllowUserToAddRows = false,
       ReadOnly = true,
           AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
              BorderStyle = BorderStyle.None
          };

   string serviceQuery = @"
       SELECT 
           s.service_name AS [Dịch vụ],
           su.quantity AS [Số lượng],
           FORMAT(s.price, 'N0') + ' VNĐ' AS [Đơn giá]
       FROM ServiceUsage su
       INNER JOIN Service s ON su.service_id = s.service_id
       INNER JOIN Invoice i ON su.invoice_id = i.invoice_id
       INNER JOIN MedicalRecord m ON i.patient_id = m.patient_id
       WHERE m.record_id = @id
       AND CAST(i.invoice_date AS DATE) = CAST(m.record_date AS DATE)";

     DataTable dtServices = DatabaseHelper.ExecuteQuery(serviceQuery, new[] { new SqlParameter("@id", recordId) });
            dgvServices.DataSource = dtServices;

         // Close button
            Button btnClose = new Button
            {
    Text = "Đóng",
     Location = new Point(325, 565),
            Size = new Size(100, 35),
          BackColor = Color.Gray,
     ForeColor = Color.White,
          FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
          };
 btnClose.FlatAppearance.BorderSize = 0;
   btnClose.Click += (s, e) => form.Close();

 form.Controls.AddRange(new Control[] { lblInfo, lblPres, dgvPres, lblServices, dgvServices, btnClose });

       IWin32Window owner = this.FindForm() ?? Form.ActiveForm;
            try
      {
    form.ShowDialog(owner);
    }
            finally
     {
        form.Dispose();
         }
 }

        #region Export Handlers

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
if (!Auth.CurrentStaffId.HasValue) return;

  try
      {
        DateTime fromDate = DateTime.Today.AddMonths(-1);
     DateTime toDate = DateTime.Today;

         var dtpFrom = this.Controls.Find("dtpFrom", true);
         var dtpTo = this.Controls.Find("dtpTo", true);

             if (dtpFrom.Length > 0)
         fromDate = ((DateTimePicker)dtpFrom[0]).Value.Date;

   if (dtpTo.Length > 0)
       toDate = ((DateTimePicker)dtpTo[0]).Value.Date;

       DataTable dt = ReportExportService.GetDoctorExaminationReport(
    Auth.CurrentStaffId.Value, fromDate, toDate);

     string title = $"Báo cáo khám bệnh - BS. {Auth.CurrentUserName}";
       ReportExportService.ShowExportDialog(dt, $"BaoCaoKham_{Auth.CurrentUserName}", title);
         }
            catch (Exception ex)
          {
      MessageBoxHelper.ShowError($"Lỗi xuất báo cáo: {ex.Message}");
}
    }

        private void BtnExportHtml_Click(object sender, EventArgs e)
        {
  if (!Auth.CurrentStaffId.HasValue) return;

        try
            {
             DateTime fromDate = DateTime.Today.AddMonths(-1);
    DateTime toDate = DateTime.Today;

    var dtpFrom = this.Controls.Find("dtpFrom", true);
          var dtpTo = this.Controls.Find("dtpTo", true);

  if (dtpFrom.Length > 0)
  fromDate = ((DateTimePicker)dtpFrom[0]).Value.Date;

if (dtpTo.Length > 0)
    toDate = ((DateTimePicker)dtpTo[0]).Value.Date;

    DataTable dt = ReportExportService.GetDoctorExaminationReport(
     Auth.CurrentStaffId.Value, fromDate, toDate);

     using (SaveFileDialog sfd = new SaveFileDialog())
           {
             sfd.Title = "Xuất báo cáo HTML";
      sfd.Filter = "HTML Files (*.html)|*.html";
        sfd.FileName = $"BaoCaoKham_{Auth.CurrentUserName}_{DateTime.Now:yyyyMMdd}";

        if (sfd.ShowDialog() == DialogResult.OK)
     {
          string title = $"Báo cáo khám bệnh - BS. {Auth.CurrentUserName}";
      string subtitle = $"Từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}";

               if (ReportExportService.ExportToHtml(dt, sfd.FileName, title, subtitle))
         {
         var result = MessageBox.Show(
       "Xuất báo cáo thành công!\n\nMở file để xem và in PDF?",
     "Thành công",
         MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

      if (result == DialogResult.Yes)
     {
             System.Diagnostics.Process.Start(sfd.FileName);
     }
         }
        }
   }
            }
     catch (Exception ex)
       {
       MessageBoxHelper.ShowError($"Lỗi xuất báo cáo: {ex.Message}");
            }
        }

        #endregion

        private void DoctorMedicalRecords_Load(object sender, EventArgs e)
        {
        }
    }
}
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminInvoices : UserControl
    {
        // ✅ PHÂN TRANG
        private int currentPage = 1;
        private int pageSize = 50; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private int totalPages = 0;

        public AdminInvoices()
        {
            InitializeComponent();
            InitializeControls();
            LoadInvoices();
        }

        private void InitializeControls()
        {
            try
            {
                // ✅ Kiểm tra controls tồn tại
                if (cboStatus == null)
                {
                    MessageBoxHelper.ShowError("Lỗi khởi tạo controls!");
                    return;
                }

                // Mặc định chọn "Tất cả" nếu chưa chọn gì
                if (cboStatus.SelectedIndex < 0)
                {
                    cboStatus.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khởi tạo: {ex.Message}");
            }
        }

        private void LoadInvoices()
        {
            // ✅ FIX: Kiểm tra null đầy đủ
            if (cboStatus == null || dtpFrom == null || dtpTo == null || dgvInvoices == null)
            {
                MessageBoxHelper.ShowError("Lỗi: Controls chưa được khởi tạo!");
                return;
            }

            try
            {
                string selectedStatus = cboStatus.SelectedItem?.ToString() ?? "Tất cả";
                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;

                // ✅ FIX: Validate ngày
                if (fromDate > toDate)
                {
                    MessageBoxHelper.ShowValidationError("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                    return;
                }

                // ✅ FIX: Cảnh báo nếu khoảng thời gian quá dài (> 1 năm)
                if ((toDate - fromDate).TotalDays > 365)
                {
                    var result = MessageBox.Show(
                        "Khoảng thời gian tra cứu quá dài (> 1 năm). Có thể ảnh hưởng đến hiệu suất.\nBạn có muốn tiếp tục?",
                        "Cảnh báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;
                }

                // ✅ TÍNH TOÁN OFFSET CHO PHÂN TRANG
                int offset = (currentPage - 1) * pageSize;

                // ✅ QUERY CHỈ LẤY DỮ LIỆU CẦN HIỂN THỊ CHO TRANG HIỆN TẠI
                string query = @"
                    SELECT
                        i.invoice_id AS [Mã HĐ],
                        u.fullname AS [Bệnh nhân],
                        FORMAT(i.invoice_date, 'dd/MM/yyyy') AS [Ngày],
                        i.total_amount AS [Tổng tiền],
                        i.status AS [Trạng thái]
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE CAST(i.invoice_date AS DATE) BETWEEN @from AND @to";

                // Chỉ thêm điều kiện status khi KHÔNG phải "Tất cả"
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@from", fromDate),
                    new SqlParameter("@to", toDate)
                };

                // Map Vietnamese status to English
                string statusFilter = selectedStatus;
                if (selectedStatus == "Đã thanh toán")
                    statusFilter = "paid";
                else if (selectedStatus == "Chưa thanh toán")
                    statusFilter = "unpaid";
                else if (selectedStatus == "Đã hủy")
                    statusFilter = "cancelled";

                if (selectedStatus != "Tất cả")
                {
                    query += " AND i.status = @status";
                    parameters.Add(new SqlParameter("@status", statusFilter));
                }

                query += @" ORDER BY i.invoice_date DESC
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                parameters.Add(new SqlParameter("@offset", offset));
                parameters.Add(new SqlParameter("@pageSize", pageSize));

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());

                // ✅ LẤY TỔNG SỐ BẢN GHI ĐỂ TÍNH TỔNG SỐ TRANG
                string countQuery = @"
                    SELECT COUNT(*)
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE CAST(i.invoice_date AS DATE) BETWEEN @from AND @to";

                var countParams = new List<SqlParameter>
                {
                    new SqlParameter("@from", fromDate),
                    new SqlParameter("@to", toDate)
                };

                if (selectedStatus != "Tất cả")
                {
                    countQuery += " AND i.status = @status";
                    countParams.Add(new SqlParameter("@status", statusFilter));
                }

                object countResult = DatabaseHelper.ExecuteScalar(countQuery, countParams.ToArray());
                totalRecords = countResult != null ? Convert.ToInt32(countResult) : 0;
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // ✅ KIỂM TRA DỮ LIỆU
                if (dt == null || dt.Rows.Count == 0)
                {
                    if (currentPage == 1)
                    {
                        string statusText = selectedStatus == "Tất cả" ? "" : $" với trạng thái '{Formatter.FormatStatus(selectedStatus)}'";
                        MessageBoxHelper.ShowInfo($"Không có hóa đơn nào từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}{statusText}");
                    }
                    else
                    {
                        MessageBoxHelper.ShowInfo($"Không có dữ liệu ở trang {currentPage}.");
                        // Quay về trang trước
                        if (currentPage > 1)
                        {
                            currentPage--;
                            LoadInvoices();
                            return;
                        }
                    }

                    dgvInvoices.DataSource = dt;
                    UpdatePaginationControls();
                    return;
                }

                dgvInvoices.DataSource = dt;

                // ✅ CẬP NHẬT PAGINATION CONTROLS
                UpdatePaginationControls();

                // ✅ FIX: Cấu hình DataGridView
                if (dgvInvoices.Columns.Count > 0)
                {
                    // Ẩn Mã HĐ nếu muốn (hoặc giữ lại)
                    // if (dgvInvoices.Columns["Mã HĐ"] != null)
                    //     dgvInvoices.Columns["Mã HĐ"].Visible = false;

                    // ✅ FIX: Tự động điều chỉnh độ rộng cột
                    dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Set độ rộng cho từng cột
                    if (dgvInvoices.Columns["Mã HĐ"] != null)
                    {
                        dgvInvoices.Columns["Mã HĐ"].FillWeight = 60;
                        dgvInvoices.Columns["Mã HĐ"].MinimumWidth = 60;
                        dgvInvoices.Columns["Mã HĐ"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    if (dgvInvoices.Columns["Bệnh nhân"] != null)
                    {
                        dgvInvoices.Columns["Bệnh nhân"].FillWeight = 150;
                        dgvInvoices.Columns["Bệnh nhân"].MinimumWidth = 150;
                    }

                    if (dgvInvoices.Columns["Ngày"] != null)
                    {
                        dgvInvoices.Columns["Ngày"].FillWeight = 90;
                        dgvInvoices.Columns["Ngày"].MinimumWidth = 90;
                        dgvInvoices.Columns["Ngày"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Định dạng cột tiền tệ
                    if (dgvInvoices.Columns["Tổng tiền"] != null)
                    {
                        dgvInvoices.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";
                        dgvInvoices.Columns["Tổng tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvInvoices.Columns["Tổng tiền"].FillWeight = 110;
                        dgvInvoices.Columns["Tổng tiền"].MinimumWidth = 110;
                    }

                    if (dgvInvoices.Columns["Trạng thái"] != null)
                    {
                        dgvInvoices.Columns["Trạng thái"].FillWeight = 90;
                        dgvInvoices.Columns["Trạng thái"].MinimumWidth = 90;
                        dgvInvoices.Columns["Trạng thái"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Thêm cột nút Xem (nếu chưa có)
                    if (!dgvInvoices.Columns.Contains("View"))
                    {
                        var btnColumn = new DataGridViewButtonColumn
                        {
                            Name = "View",
                            HeaderText = "",
                            Text = "Xem",
                            UseColumnTextForButtonValue = true,
                            Width = 70,
                            FlatStyle = FlatStyle.Flat
                        };
                        dgvInvoices.Columns.Add(btnColumn);
                    }

                    // Định dạng trạng thái + màu nền
                    foreach (DataGridViewRow row in dgvInvoices.Rows)
                    {
                        if (row.IsNewRow) continue;

                        if (row.Cells["Trạng thái"].Value is string status)
                        {
                            // Format trạng thái
                            row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);

                            // ✅ FIX: Highlight màu nền theo trạng thái
                            if (status.Equals("unpaid", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE"); // Đỏ nhạt
                                row.Cells["Trạng thái"].Style.ForeColor = Color.Red;
                                row.Cells["Trạng thái"].Style.Font = new Font(dgvInvoices.Font, FontStyle.Bold);
                            }
                            else if (status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9"); // Xanh nhạt
                                row.Cells["Trạng thái"].Style.ForeColor = Color.Green;
                                row.Cells["Trạng thái"].Style.Font = new Font(dgvInvoices.Font, FontStyle.Bold);
                            }
                            else if (status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F5F5F5"); // Xám nhạt
                                row.Cells["Trạng thái"].Style.ForeColor = Color.Gray;
                                row.Cells["Trạng thái"].Style.Font = new Font(dgvInvoices.Font, FontStyle.Italic);
                            }
                        }

                        // ✅ FIX: Highlight số tiền lớn (> 10 triệu)
                        if (row.Cells["Tổng tiền"].Value != null && row.Cells["Tổng tiền"].Value != DBNull.Value)
                        {
                            decimal totalAmount = Convert.ToDecimal(row.Cells["Tổng tiền"].Value);
                            if (totalAmount > 10000000)
                            {
                                row.Cells["Tổng tiền"].Style.ForeColor = Color.DarkRed;
                                row.Cells["Tổng tiền"].Style.Font = new Font(dgvInvoices.Font, FontStyle.Bold);
                            }
                            else if (totalAmount == 0)
                            {
                                row.Cells["Tổng tiền"].Style.ForeColor = Color.Gray;
                                row.Cells["Tổng tiền"].Style.Font = new Font(dgvInvoices.Font, FontStyle.Italic);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
            }
            catch (InvalidCastException castEx)
            {
                MessageBoxHelper.ShowError($"Lỗi định dạng dữ liệu: {castEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải hóa đơn: {ex.Message}");
            }
        }

        // ✅ CẬP NHẬT CONTROLS PHÂN TRANG
        private void UpdatePaginationControls()
        {
            try
            {
                // Kiểm tra controls tồn tại
                if (lblPageInfo == null || btnPrevious == null || btnNext == null ||
                    btnFirst == null || btnLast == null)
                {
                    return;
                }

                // Update label thông tin trang
                int fromRecord = totalRecords > 0 ? (currentPage - 1) * pageSize + 1 : 0;
                int toRecord = Math.Min(currentPage * pageSize, totalRecords);

                lblPageInfo.Text = $"Trang {currentPage}/{totalPages} (Hiển thị {fromRecord}-{toRecord} / {totalRecords} bản ghi)";

                // Enable/Disable buttons
                btnFirst.Enabled = currentPage > 1;
                btnPrevious.Enabled = currentPage > 1;
                btnNext.Enabled = currentPage < totalPages;
                btnLast.Enabled = currentPage < totalPages;

                // Đổi màu button khi disabled
                Color enabledColor = ColorTranslator.FromHtml("#007ACC");
                Color disabledColor = Color.LightGray;

                btnFirst.BackColor = btnFirst.Enabled ? enabledColor : disabledColor;
                btnPrevious.BackColor = btnPrevious.Enabled ? enabledColor : disabledColor;
                btnNext.BackColor = btnNext.Enabled ? enabledColor : disabledColor;
                btnLast.BackColor = btnLast.Enabled ? enabledColor : disabledColor;
            }
            catch (Exception ex)
            {
                // Silent fail - không hiện lỗi cho phần UI này
                Console.WriteLine($"Error updating pagination: {ex.Message}");
            }
        }

        // ✅ EVENT HANDLERS CHO PHÂN TRANG
        private void BtnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentPage != 1)
                {
                    currentPage = 1;
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentPage < totalPages)
                {
                    currentPage++;
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentPage != totalPages && totalPages > 0)
                {
                    currentPage = totalPages;
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // ✅ THAY ĐỔI KÍCH THƯỚC TRANG
        private void CboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPageSize == null || cboPageSize.SelectedItem == null)
                    return;

                int newPageSize = Convert.ToInt32(cboPageSize.SelectedItem);
                if (newPageSize != pageSize)
                {
                    pageSize = newPageSize;
                    currentPage = 1; // Reset về trang 1
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // ✅ NHẢY ĐẾN TRANG CỤ THỂ
        private void BtnGoToPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPageNumber == null || string.IsNullOrWhiteSpace(txtPageNumber.Text))
                {
                    MessageBoxHelper.ShowValidationError("Vui lòng nhập số trang!");
                    return;
                }

                if (!int.TryParse(txtPageNumber.Text, out int pageNumber))
                {
                    MessageBoxHelper.ShowValidationError("Số trang phải là số nguyên!");
                    txtPageNumber.Focus();
                    return;
                }

                if (pageNumber < 1)
                {
                    MessageBoxHelper.ShowValidationError("Số trang phải lớn hơn 0!");
                    txtPageNumber.Focus();
                    return;
                }

                if (pageNumber > totalPages)
                {
                    MessageBoxHelper.ShowValidationError($"Số trang không vượt quá {totalPages}!");
                    txtPageNumber.Focus();
                    return;
                }

                currentPage = pageNumber;
                txtPageNumber.Clear();
                LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                if (dgvInvoices.Columns["View"] is DataGridViewColumn viewCol && e.ColumnIndex == viewCol.Index)
                {
                    // ✅ FIX: Kiểm tra cell value trước khi convert
                    if (dgvInvoices.Rows[e.RowIndex].Cells["Mã HĐ"].Value == null ||
                        dgvInvoices.Rows[e.RowIndex].Cells["Mã HĐ"].Value == DBNull.Value)
                    {
                        MessageBoxHelper.ShowError("Không thể xác định mã hóa đơn!");
                        return;
                    }

                    int invoiceId = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["Mã HĐ"].Value);
                    ShowInvoiceDetail(invoiceId);
                }
            }
            catch (InvalidCastException)
            {
                MessageBoxHelper.ShowError("Lỗi định dạng mã hóa đơn!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void ShowInvoiceDetail(int invoiceId)
        {
            try
            {
                string query = @"
                    SELECT i.*, u.fullname AS patient_name, u2.fullname AS staff_name
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    LEFT JOIN Staff s ON i.staff_id = s.staff_id
                    LEFT JOIN UserAccount u2 ON s.user_id = u2.user_id
                    WHERE i.invoice_id = @id";

                DataTable dtInvoice = DatabaseHelper.ExecuteQuery(
                    query,
                    new SqlParameter[] { new SqlParameter("@id", invoiceId) }
                );

                // ✅ FIX: Kiểm tra dữ liệu
                if (dtInvoice == null || dtInvoice.Rows.Count == 0)
                {
                    MessageBoxHelper.ShowError("Không tìm thấy hóa đơn!");
                    return;
                }

                var row = dtInvoice.Rows[0];

                using (Form form = new Form
                {
                    Text = $"Chi tiết hóa đơn #{invoiceId}",
                    Size = new Size(700, 650), // ✅ FIX: Tăng chiều cao
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    MaximizeBox = false,
                    MinimizeBox = false
                })
                {
                    // ✅ FIX: Thêm icon và màu sắc theo trạng thái
                    string statusIcon = "";
                    Color statusColor = Color.Black;
                    string status = row["status"]?.ToString() ?? "";

                    if (status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                    {
                        statusIcon = "✓";
                        statusColor = Color.Green;
                    }
                    else if (status.Equals("unpaid", StringComparison.OrdinalIgnoreCase))
                    {
                        statusIcon = "⚠";
                        statusColor = Color.Red;
                    }
                    else if (status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        statusIcon = "✗";
                        statusColor = Color.Gray;
                    }

                    Label lblInfo = new Label
                    {
                        Text = $"Bệnh nhân: {row["patient_name"]}\n" +
                               $"Ngày: {Convert.ToDateTime(row["invoice_date"]):dd/MM/yyyy HH:mm}\n" +
                               $"Nhân viên: {(row["staff_name"] == DBNull.Value ? "N/A" : row["staff_name"])}\n" +
                               $"Trạng thái: {statusIcon} {Formatter.FormatStatus(status)}",
                        Location = new Point(20, 20),
                        Size = new Size(650, 100),
                        Font = new Font("Segoe UI", 11)
                    };

                    // ✅ FIX: Panel cho phần thông tin
                    Panel pnlInfo = new Panel
                    {
                        Location = new Point(20, 20),
                        Size = new Size(650, 100),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(245, 245, 245)
                    };
                    pnlInfo.Controls.Add(lblInfo);

                    Label lblServicesTitle = new Label
                    {
                        Text = "Dịch vụ đã sử dụng:",
                        Location = new Point(20, 135),
                        Size = new Size(200, 25),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    };

                    DataGridView dgvDetails = new DataGridView
                    {
                        Location = new Point(20, 165),
                        Size = new Size(650, 350),
                        BackgroundColor = Color.White,
                        AllowUserToAddRows = false,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        RowHeadersVisible = false,
                        BorderStyle = BorderStyle.Fixed3D
                    };

                    // ✅ FIX: Query chi tiết dịch vụ - lấy từ cả ServiceUsage và InvoicePrescription
                    string serviceQuery = @"
                        SELECT 
                            s.service_name AS [Dịch vụ], 
                            su.quantity AS [SL], 
                            s.price AS [Đơn giá],
                            (s.price * su.quantity) AS [Thành tiền]
                        FROM ServiceUsage su
                        INNER JOIN Service s ON su.service_id = s.service_id
                        WHERE su.invoice_id = @id
                        
                        UNION ALL
                        
                        SELECT 
                            m.name AS [Dịch vụ],
                            CAST(p.dosage AS INT) AS [SL],
                            m.price AS [Đơn giá],
                            (m.price * CAST(p.dosage AS INT)) AS [Thành tiền]
                        FROM InvoicePrescription ip
                        INNER JOIN Prescription p ON ip.prescription_id = p.prescription_id
                        INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
                        WHERE ip.invoice_id = @id
                        
                        ORDER BY [Dịch vụ]";

                    DataTable dtServices = DatabaseHelper.ExecuteQuery(
                        serviceQuery,
                        new SqlParameter[] { new SqlParameter("@id", invoiceId) }
                    );

                    // ✅ FIX: Kiểm tra có dịch vụ không
                    if (dtServices == null || dtServices.Rows.Count == 0)
                    {
                        // Tạo DataTable rỗng với cấu trúc
                        dtServices = new DataTable();
                        dtServices.Columns.Add("Dịch vụ", typeof(string));
                        dtServices.Columns.Add("SL", typeof(int));
                        dtServices.Columns.Add("Đơn giá", typeof(decimal));
                        dtServices.Columns.Add("Thành tiền", typeof(decimal));

                        Label lblNoService = new Label
                        {
                            Text = "Chưa có dịch vụ nào trong hóa đơn này",
                            Location = new Point(20, 300),
                            Size = new Size(650, 30),
                            Font = new Font("Segoe UI", 10, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        form.Controls.Add(lblNoService);
                    }

                    dgvDetails.DataSource = dtServices;

                    // Format cột tiền
                    if (dgvDetails.Columns["Đơn giá"] != null)
                    {
                        dgvDetails.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";
                        dgvDetails.Columns["Đơn giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    if (dgvDetails.Columns["Thành tiền"] != null)
                    {
                        dgvDetails.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";
                        dgvDetails.Columns["Thành tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    if (dgvDetails.Columns["SL"] != null)
                    {
                        dgvDetails.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvDetails.Columns["SL"].Width = 50;
                    }

                    // ✅ FIX: Tổng tiền với màu sắc
                    decimal totalAmount = row["total_amount"] != DBNull.Value ? Convert.ToDecimal(row["total_amount"]) : 0;

                    Label lblTotal = new Label
                    {
                        Text = $"TỔNG TIỀN: {totalAmount:N0} VNĐ",
                        Location = new Point(20, 530),
                        Size = new Size(650, 40),
                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                        ForeColor = totalAmount > 10000000 ? Color.DarkRed : ColorTranslator.FromHtml("#DC3545"),
                        TextAlign = ContentAlignment.MiddleRight,
                        BackColor = Color.FromArgb(255, 250, 205)
                    };

                    // ✅ FIX: Button đóng
                    Button btnClose = new Button
                    {
                        Text = "Đóng",
                        Location = new Point(300, 580),
                        Size = new Size(100, 35),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        BackColor = ColorTranslator.FromHtml("#6C757D"),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnClose.Click += (s, e) => form.Close();

                    form.Controls.AddRange(new Control[] {
                        pnlInfo,
                        lblServicesTitle,
                        dgvDetails,
                        lblTotal,
                        btnClose
                    });

                    form.ShowDialog();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBoxHelper.ShowError($"Lỗi SQL khi xem chi tiết: {sqlEx.Message}");
            }
            catch (InvalidCastException castEx)
            {
                MessageBoxHelper.ShowError($"Lỗi định dạng dữ liệu chi tiết: {castEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khi xem chi tiết: {ex.Message}");
            }
        }

        

        // ✅ FIX: Event handlers với try-catch
        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Validate ngày bắt đầu không được > ngày kết thúc
                if (dtpFrom != null && dtpTo != null && dtpFrom.Value.Date > dtpTo.Value.Date)
                {
                    MessageBoxHelper.ShowValidationError("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                    dtpFrom.Value = dtpTo.Value;
                    return;
                }
                currentPage = 1; // Reset trang khi thay đổi filter
                LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Validate ngày kết thúc không được < ngày bắt đầu
                if (dtpFrom != null && dtpTo != null && dtpTo.Value.Date < dtpFrom.Value.Date)
                {
                    MessageBoxHelper.ShowValidationError("Ngày kết thúc không được nhỏ hơn ngày bắt đầu!");
                    dtpTo.Value = dtpFrom.Value;
                    return;
                }
                currentPage = 1; // Reset trang khi thay đổi filter
                LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    btn.Enabled = false;
                    btn.Text = "Đang tải...";
                }
                LoadInvoices();
                if (sender is Button btn2)
                {
                    btn2.Enabled = true;
                    btn2.Text = "🔄 Làm mới";
                }
                MessageBoxHelper.ShowInfo("Dữ liệu đã được làm mới!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi làm mới: {ex.Message}");
                if (sender is Button btn)
                {
                    btn.Enabled = true;
                    btn.Text = "🔄 Làm mới";
                }
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = 1; // Reset trang khi thay đổi filter
                LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}
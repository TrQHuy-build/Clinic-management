using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminLogs : UserControl
    {
        // ✅ Phân trang
        private int currentPage = 1;
        private int pageSize = 50; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private int totalPages = 0;

        public AdminLogs()
        {
            InitializeComponent();
            InitializeControls();
            LoadLogs();
        }

        private void InitializeControls()
        {
            try
            {
                // ✅ Kiểm tra dgvLogs tồn tại
                if (dgvLogs == null)
                {
                    MessageBoxHelper.ShowError("Lỗi khởi tạo DataGridView!");
                    return;
                }

                // Cấu hình DataGridView
                dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvLogs.ReadOnly = true;
                dgvLogs.AllowUserToAddRows = false;
                dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvLogs.RowHeadersVisible = false;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khởi tạo: {ex.Message}");
            }
        }

        private void LoadLogs()
        {
            if (dgvLogs == null)
            {
                MessageBoxHelper.ShowError("Lỗi: DataGridView chưa được khởi tạo!");
                return;
            }

            try
            {
                // ✅ Tính toán offset cho phân trang
                int offset = (currentPage - 1) * pageSize;

                // ✅ Query với phân trang (OFFSET-FETCH)
                string query = @"
                    SELECT
                        u.fullname AS [Người dùng],
                        a.action AS [Hành động],
                        a.description AS [Mô tả],
                        FORMAT(a.timestamp, 'dd/MM/yyyy HH:mm:ss') AS [Thời gian]
                    FROM AuditLog a
                    LEFT JOIN UserAccount u ON a.user_id = u.user_id
                    ORDER BY a.timestamp DESC
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                SqlParameter[] parameters = {
                    new SqlParameter("@offset", offset),
                    new SqlParameter("@pageSize", pageSize)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                // ✅ Lấy tổng số bản ghi để tính tổng số trang
                string countQuery = "SELECT COUNT(*) FROM AuditLog";
                object countResult = DatabaseHelper.ExecuteScalar(countQuery);
                totalRecords = countResult != null ? Convert.ToInt32(countResult) : 0;
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // ✅ Kiểm tra dữ liệu
                if (dt == null || dt.Rows.Count == 0)
                {
                    if (currentPage == 1)
                    {
                        MessageBoxHelper.ShowInfo("Chưa có log nào trong hệ thống.");
                    }
                    else
                    {
                        MessageBoxHelper.ShowInfo($"Không có dữ liệu ở trang {currentPage}.");
                        // Quay về trang trước
                        if (currentPage > 1)
                        {
                            currentPage--;
                            LoadLogs();
                            return;
                        }
                    }

                    dgvLogs.DataSource = dt;
                    UpdatePaginationControls();
                    return;
                }

                dgvLogs.DataSource = dt;

                // ✅ Cấu hình columns
                if (dgvLogs.Columns.Count > 0)
                {
                    // Set độ rộng cho từng cột
                    if (dgvLogs.Columns["Người dùng"] != null)
                    {
                        dgvLogs.Columns["Người dùng"].FillWeight = 120;
                        dgvLogs.Columns["Người dùng"].MinimumWidth = 120;
                    }

                    if (dgvLogs.Columns["Hành động"] != null)
                    {
                        dgvLogs.Columns["Hành động"].FillWeight = 120;
                        dgvLogs.Columns["Hành động"].MinimumWidth = 120;
                        dgvLogs.Columns["Hành động"].DefaultCellStyle.Font = new Font(dgvLogs.Font, FontStyle.Bold);
                    }

                    if (dgvLogs.Columns["Mô tả"] != null)
                    {
                        dgvLogs.Columns["Mô tả"].FillWeight = 200;
                        dgvLogs.Columns["Mô tả"].MinimumWidth = 200;
                    }

                    if (dgvLogs.Columns["Thời gian"] != null)
                    {
                        dgvLogs.Columns["Thời gian"].FillWeight = 130;
                        dgvLogs.Columns["Thời gian"].MinimumWidth = 130;
                        dgvLogs.Columns["Thời gian"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // ✅ Highlight theo loại hành động
                    foreach (DataGridViewRow row in dgvLogs.Rows)
                    {
                        if (row.IsNewRow) continue;

                        if (row.Cells["Hành động"].Value != null)
                        {
                            string action = row.Cells["Hành động"].Value.ToString().ToUpper();

                            // Màu sắc theo loại action
                            if (action.Contains("LOGIN") || action.Contains("LOGOUT"))
                            {
                                row.Cells["Hành động"].Style.ForeColor = Color.Blue;
                            }
                            else if (action.Contains("CREATE") || action.Contains("ADD"))
                            {
                                row.Cells["Hành động"].Style.ForeColor = Color.Green;
                            }
                            else if (action.Contains("UPDATE") || action.Contains("EDIT"))
                            {
                                row.Cells["Hành động"].Style.ForeColor = Color.Orange;
                            }
                            else if (action.Contains("DELETE") || action.Contains("REMOVE"))
                            {
                                row.Cells["Hành động"].Style.ForeColor = Color.Red;
                            }
                            else if (action.Contains("ERROR") || action.Contains("FAIL"))
                            {
                                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238); // Light red
                                row.Cells["Hành động"].Style.ForeColor = Color.DarkRed;
                            }
                        }

                        // Handle NULL người dùng (system actions)
                        if (row.Cells["Người dùng"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["Người dùng"].Value?.ToString()))
                        {
                            row.Cells["Người dùng"].Value = "SYSTEM";
                            row.Cells["Người dùng"].Style.ForeColor = Color.Gray;
                            row.Cells["Người dùng"].Style.Font = new Font(dgvLogs.Font, FontStyle.Italic);
                        }
                    }
                }

                // ✅ Update pagination controls
                UpdatePaginationControls();
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
                MessageBoxHelper.ShowError($"Lỗi tải logs: {ex.Message}");
            }
        }

        // ✅ Cập nhật controls phân trang
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

        // ✅ Event handlers cho phân trang
        private void BtnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentPage != 1)
                {
                    currentPage = 1;
                    LoadLogs();
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
                    LoadLogs();
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
                    LoadLogs();
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
                    LoadLogs();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // ✅ Thay đổi kích thước trang
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
                    LoadLogs();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // ✅ Refresh data
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button khi đang refresh
                if (sender is Button btn)
                {
                    btn.Enabled = false;
                    btn.Text = "Đang tải...";
                }

                LoadLogs();

                if (sender is Button btn2)
                {
                    btn2.Enabled = true;
                    btn2.Text = "Làm mới";
                }

                MessageBoxHelper.ShowInfo("Dữ liệu đã được làm mới!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi làm mới: {ex.Message}");

                if (sender is Button btn)
                {
                    btn.Enabled = true;
                    btn.Text = "Làm mới";
                }
            }
        }

        // ✅ Nhảy đến trang cụ thể
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
                    MessageBoxHelper.ShowValidationError($"Số trang không được vượt quá {totalPages}!");
                    txtPageNumber.Focus();
                    return;
                }

                currentPage = pageNumber;
                LoadLogs();
                txtPageNumber.Clear();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminServices : UserControl
    {
        // ✅ PHÂN TRANG
        private int currentPage = 1;
        private int pageSize = 50; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private int totalPages = 0;

        public AdminServices()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            try
            {
                string search = txtSearch?.Text?.Trim() ?? "";

                // ✅ TÍNH TOÁN OFFSET CHO PHÂN TRANG
                int offset = (currentPage - 1) * pageSize;

                // ✅ QUERY CHỈ LẤY DỮ LIỆU CẦN HIỂN THỊ CHO TRANG HIỆN TẠI
                string query = @"
                    SELECT
                        service_id AS [ID],
                        service_name AS [Tên dịch vụ],
                        description AS [Mô tả],
                        price AS [Giá],
                        status AS [Trạng thái]
                    FROM Service
                    WHERE service_name LIKE @search OR description LIKE @search
                    ORDER BY service_name
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                SqlParameter[] parameters = {
                    new SqlParameter("@search", $"%{search}%"),
                    new SqlParameter("@offset", offset),
                    new SqlParameter("@pageSize", pageSize)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                // ✅ LẤY TỔNG SỐ BẢN GHI ĐỂ TÍNH TỔNG SỐ TRANG
                string countQuery = "SELECT COUNT(*) FROM Service WHERE service_name LIKE @search OR description LIKE @search";
                SqlParameter[] countParams = { new SqlParameter("@search", $"%{search}%") };
                object countResult = DatabaseHelper.ExecuteScalar(countQuery, countParams);
                totalRecords = countResult != null ? Convert.ToInt32(countResult) : 0;
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // ✅ KIỂM TRA DỮ LIỆU
                if (dt == null || dt.Rows.Count == 0)
                {
                    if (currentPage == 1)
                    {
                        MessageBoxHelper.ShowInfo("Chưa có dịch vụ nào trong hệ thống.");
                    }
                    else
                    {
                        MessageBoxHelper.ShowInfo($"Không có dữ liệu ở trang {currentPage}.");
                        // Quay về trang trước
                        if (currentPage > 1)
                        {
                            currentPage--;
                            LoadServices();
                            return;
                        }
                    }

                    dgvServices.DataSource = dt;
                    UpdatePaginationControls();
                    return;
                }

                dgvServices.DataSource = dt;

                // ✅ CẬP NHẬT PAGINATION CONTROLS
                UpdatePaginationControls();

                // Ẩn ID
                if (dgvServices.Columns["ID"] != null)
                    dgvServices.Columns["ID"].Visible = false;

                // ✅ Set column widths for better display
                if (dgvServices.Columns["Tên dịch vụ"] != null)
                    dgvServices.Columns["Tên dịch vụ"].Width = 250;
                if (dgvServices.Columns["Mô tả"] != null)
                    dgvServices.Columns["Mô tả"].Width = 350;
                if (dgvServices.Columns["Giá"] != null)
                    dgvServices.Columns["Giá"].Width = 150;
                if (dgvServices.Columns["Trạng thái"] != null)
                    dgvServices.Columns["Trạng thái"].Width = 120;

                // Định dạng giá
                if (dgvServices.Columns["Giá"] != null)
                {
                    dgvServices.Columns["Giá"].DefaultCellStyle.Format = "N0";
                    dgvServices.Columns["Giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Thêm cột nút
                AddButtonColumn("Edit", "Sửa", 80);
                AddButtonColumn("Delete", "Xóa", 80);

                // Định dạng trạng thái và mô tả
                foreach (DataGridViewRow row in dgvServices.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Format trạng thái
                    if (row.Cells["Trạng thái"].Value is string status)
                    {
                        row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);
                    }

                    // Format mô tả (nếu null)
                    if (row.Cells["Mô tả"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Mô tả"].Value?.ToString()))
                    {
                        row.Cells["Mô tả"].Value = "N/A";
                    }

                    // Highlight giá cao (> 20 triệu)
                    decimal price = Convert.ToDecimal(row.Cells["Giá"].Value);
                    if (price > 20000000)
                    {
                        row.Cells["Giá"].Style.ForeColor = Color.Red;
                        row.Cells["Giá"].Style.Font = new Font(dgvServices.Font, FontStyle.Bold);
                    }
                    // ✅ FIX: Highlight giá = 0 (màu xám)
                    else if (price == 0)
                    {
                        row.Cells["Giá"].Style.ForeColor = Color.Gray;
                        row.Cells["Giá"].Style.Font = new Font(dgvServices.Font, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void AddButtonColumn(string name, string text, int width)
        {
            if (!dgvServices.Columns.Contains(name))
            {
                dgvServices.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = name,
                    HeaderText = "",
                    Text = text,
                    UseColumnTextForButtonValue = true,
                    Width = width
                });
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
                    LoadServices();
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
                    LoadServices();
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
                    LoadServices();
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
                    LoadServices();
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
                    LoadServices();
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
                LoadServices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void DgvServices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int serviceId = Convert.ToInt32(dgvServices.Rows[e.RowIndex].Cells["ID"].Value);
                string serviceName = dgvServices.Rows[e.RowIndex].Cells["Tên dịch vụ"].Value?.ToString() ?? "";

                if (e.ColumnIndex == dgvServices.Columns["Edit"]?.Index)
                    ShowAddEditForm(serviceId);
                else if (e.ColumnIndex == dgvServices.Columns["Delete"]?.Index)
                    DeleteService(serviceId, serviceName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
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

                LoadServices();

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

        private void ShowAddEditForm(int? serviceId)
        {
            using (Form form = new Form
            {
                Text = serviceId.HasValue ? "Sửa dịch vụ" : "Thêm dịch vụ mới",
                Size = new Size(550, 580), // ✅ FIX: Tăng chiều cao để có chỗ cho error labels
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                // ✅ FIX: Tạo controls với spacing tốt hơn cho error labels
                TextBox txtName = CreateTextBox(150, 30);
                Label lblNameError = CreateErrorLabel(150, 57);

                TextBox txtDesc = new TextBox
                {
                    Location = new Point(150, 90),  // ✅ FIX: Tăng Y để có chỗ cho error
                    Size = new Size(350, 80),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Segoe UI", 9)
                };
                Label lblDescError = CreateErrorLabel(150, 172);

                TextBox txtPrice = CreateTextBox(150, 200);  // ✅ FIX: Điều chỉnh Y
                Label lblPriceError = CreateErrorLabel(150, 227);

                ComboBox cboStatus = new ComboBox
                {
                    Location = new Point(150, 255),  // ✅ FIX: Điều chỉnh Y
                    Size = new Size(350, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 9)
                };
                cboStatus.Items.AddRange(new object[] { "available", "unavailable" });
                cboStatus.SelectedIndex = 0;

                // ✅ FIX: Real-time validation với null checks
                txtName.TextChanged += (s, e) =>
                {
                    if (txtName != null && lblNameError != null)
                        ValidateServiceNameRealtime(txtName, lblNameError, serviceId);
                };

                txtDesc.TextChanged += (s, e) =>
                {
                    if (txtDesc != null && lblDescError != null)
                        ValidateDescriptionRealtime(txtDesc, lblDescError);
                };

                txtPrice.TextChanged += (s, e) =>
                {
                    if (txtPrice != null && lblPriceError != null)
                        ValidatePriceRealtime(txtPrice, lblPriceError);
                };

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Tên dịch vụ:", 30, 33), txtName, lblNameError,
                    CreateLabel("Mô tả:", 30, 93), txtDesc, lblDescError,  // ✅ FIX: Điều chỉnh Y
                    CreateLabel("Giá (VNĐ):", 30, 203), txtPrice, lblPriceError,
                    CreateLabel("Trạng thái:", 30, 258), cboStatus
                });

                // Load dữ liệu nếu sửa
                if (serviceId.HasValue)
                {
                    try
                    {
                        string query = "SELECT * FROM Service WHERE service_id = @id";
                        DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", serviceId.Value) });

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtName.Text = row["service_name"]?.ToString() ?? "";
                            txtDesc.Text = row["description"]?.ToString() ?? "";
                            txtPrice.Text = row["price"]?.ToString() ?? "0";

                            string status = row["status"]?.ToString();
                            if (!string.IsNullOrEmpty(status))
                                cboStatus.SelectedItem = status;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
                        form.Close();
                        return;
                    }
                }

                Button btnSave = CreateButton("Lưu", 225, 320, "#007ACC");  // ✅ FIX: Điều chỉnh Y
                btnSave.Click += (s, ev) =>
                {
                    // ✅ FIX: Disable button để tránh double-click
                    btnSave.Enabled = false;
                    btnSave.Text = "Đang lưu...";

                    try
                    {
                        if (!ValidateServiceInput(txtName, txtDesc, txtPrice, serviceId))
                        {
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                            return;
                        }

                        // ✅ FIX: Normalize tên dịch vụ (loại bỏ khoảng trắng thừa)
                        string serviceName = NormalizeWhitespace(txtName.Text.Trim());
                        decimal price = decimal.Parse(txtPrice.Text);
                        string query;
                        SqlParameter[] parameters;

                        if (serviceId.HasValue)
                        {
                            // ✅ FIX: Kiểm tra trùng tên khi UPDATE (ngoại trừ chính service đang edit)
                            object existingService = DatabaseHelper.ExecuteScalar(
                                "SELECT COUNT(*) FROM Service WHERE service_name = @name AND service_id != @id",
                                new SqlParameter[] {
                                    new SqlParameter("@name", serviceName),
                                    new SqlParameter("@id", serviceId.Value)
                                });

                            if (Convert.ToInt32(existingService) > 0)
                            {
                                MessageBoxHelper.ShowValidationError("Tên dịch vụ đã tồn tại!");
                                txtName.Focus();
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            query = @"UPDATE Service SET service_name=@name, description=@desc,
                                    price=@price, status=@status WHERE service_id=@id";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", serviceName),
                                new SqlParameter("@desc", string.IsNullOrWhiteSpace(txtDesc.Text) ? (object)DBNull.Value : txtDesc.Text.Trim()),
                                new SqlParameter("@price", price),
                                new SqlParameter("@status", cboStatus.SelectedItem.ToString()),
                                new SqlParameter("@id", serviceId.Value)
                            };
                        }
                        else
                        {
                            // Kiểm tra tên dịch vụ trùng
                            object existingService = DatabaseHelper.ExecuteScalar(
                                "SELECT COUNT(*) FROM Service WHERE service_name = @name",
                                new SqlParameter[] { new SqlParameter("@name", serviceName) });

                            if (Convert.ToInt32(existingService) > 0)
                            {
                                MessageBoxHelper.ShowValidationError("Tên dịch vụ đã tồn tại!");
                                txtName.Focus();
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            query = @"INSERT INTO Service (service_name, description, price, status)
                                    VALUES (@name, @desc, @price, @status)";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", serviceName),
                                new SqlParameter("@desc", string.IsNullOrWhiteSpace(txtDesc.Text) ? (object)DBNull.Value : txtDesc.Text.Trim()),
                                new SqlParameter("@price", price),
                                new SqlParameter("@status", cboStatus.SelectedItem.ToString())
                            };
                        }

                        int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                        if (result > 0)
                        {
                            MessageBoxHelper.ShowSaveSuccess();
                            Logger.LogAction(serviceId.HasValue ? "UPDATE_SERVICE" : "CREATE_SERVICE", serviceName);
                            form.Close();
                            LoadServices();
                        }
                        else
                        {
                            MessageBoxHelper.ShowError("Không thể lưu dữ liệu!");
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                        {
                            MessageBoxHelper.ShowError("Tên dịch vụ đã tồn tại!");
                        }
                        else
                        {
                            MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                        }
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

        private void DeleteService(int serviceId, string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                MessageBoxHelper.ShowError("Không thể xác định dịch vụ cần xóa!");
                return;
            }

            if (!MessageBoxHelper.ShowDeleteConfirm(serviceName)) return;

            try
            {
                // Kiểm tra dịch vụ có được sử dụng không
                object usageCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM ServiceUsage WHERE service_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", serviceId) });

                if (Convert.ToInt32(usageCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa dịch vụ này vì đã có hóa đơn sử dụng!");
                    return;
                }

                object appointmentCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Appointment WHERE service_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", serviceId) });

                if (Convert.ToInt32(appointmentCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa dịch vụ này vì đã có lịch hẹn liên quan!");
                    return;
                }

                string query = "DELETE FROM Service WHERE service_id = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@id", serviceId) });

                if (result > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(serviceName);
                    Logger.LogDelete("Service", serviceName);
                    LoadServices();
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể xóa dịch vụ!");
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign key constraint
                {
                    MessageBoxHelper.ShowError("Không thể xóa dịch vụ vì có dữ liệu liên quan!");
                }
                else
                {
                    MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi xóa: {ex.Message}");
            }
        }

        // ✅ FIX: Validation methods với serviceId để check duplicate
        private bool ValidateServiceInput(TextBox txtName, TextBox txtDesc, TextBox txtPrice, int? serviceId)
        {
            // ✅ FIX: Normalize trước khi validate
            string name = NormalizeWhitespace(txtName.Text.Trim());

            // Kiểm tra tên dịch vụ
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập tên dịch vụ!");
                txtName.Focus();
                return false;
            }

            if (name.Length < 3)
            {
                MessageBoxHelper.ShowValidationError("Tên dịch vụ phải có ít nhất 3 ký tự!");
                txtName.Focus();
                return false;
            }

            if (name.Length > 100)
            {
                MessageBoxHelper.ShowValidationError("Tên dịch vụ không được vượt quá 100 ký tự!");
                txtName.Focus();
                return false;
            }

            // ✅ FIX: Validate ký tự đặc biệt
            if (!IsValidServiceName(name))
            {
                MessageBoxHelper.ShowValidationError("Tên dịch vụ chỉ được chứa chữ cái, số và các ký tự: - / & ( )");
                txtName.Focus();
                return false;
            }

            // Kiểm tra mô tả
            if (!string.IsNullOrWhiteSpace(txtDesc.Text) && txtDesc.Text.Trim().Length > 255)
            {
                MessageBoxHelper.ShowValidationError("Mô tả không được vượt quá 255 ký tự!");
                txtDesc.Focus();
                return false;
            }

            // Kiểm tra giá
            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập giá dịch vụ!");
                txtPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBoxHelper.ShowValidationError("Giá phải là số!");
                txtPrice.Focus();
                return false;
            }

            if (price < 0)
            {
                MessageBoxHelper.ShowValidationError("Giá không được âm!");
                txtPrice.Focus();
                return false;
            }

            if (price > 999999999)
            {
                MessageBoxHelper.ShowValidationError("Giá không được vượt quá 999,999,999!");
                txtPrice.Focus();
                return false;
            }

            // ✅ FIX: Cảnh báo giá = 0
            if (price == 0)
            {
                var result = MessageBox.Show(
                    "Giá dịch vụ = 0 VNĐ (Miễn phí). Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtPrice.Focus();
                    return false;
                }
            }
            // Cảnh báo giá quá thấp
            else if (price > 0 && price < 10000)
            {
                var result = MessageBox.Show(
                    "Giá dịch vụ rất thấp (< 10,000 VNĐ). Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtPrice.Focus();
                    return false;
                }
            }
            // Cảnh báo giá quá cao
            else if (price > 50000000)
            {
                var result = MessageBox.Show(
                    "Giá dịch vụ rất cao (> 50,000,000 VNĐ). Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtPrice.Focus();
                    return false;
                }
            }

            return true;
        }

        // ✅ FIX: Real-time validation methods với serviceId
        private void ValidateServiceNameRealtime(TextBox txt, Label lblError, int? serviceId)
        {
            if (txt == null || lblError == null) return;

            string value = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                ShowFieldError(txt, lblError, "Tên dịch vụ không được để trống");
            }
            else if (value.Length < 3)
            {
                ShowFieldError(txt, lblError, "Tên phải có ít nhất 3 ký tự");
            }
            else if (value.Length > 100)
            {
                ShowFieldError(txt, lblError, "Tên không được vượt quá 100 ký tự");
            }
            // ✅ FIX: Validate ký tự đặc biệt
            else if (!IsValidServiceName(value))
            {
                ShowFieldError(txt, lblError, "Chỉ được chứa chữ cái, số và các ký tự: - / & ( )");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidateDescriptionRealtime(TextBox txt, Label lblError)
        {
            if (txt == null || lblError == null) return;

            if (!string.IsNullOrWhiteSpace(txt.Text) && txt.Text.Trim().Length > 255)
            {
                ShowFieldError(txt, lblError, "Mô tả không được vượt quá 255 ký tự");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidatePriceRealtime(TextBox txt, Label lblError)
        {
            if (txt == null || lblError == null) return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Giá không được để trống");
            }
            else if (!decimal.TryParse(txt.Text, out decimal price))
            {
                ShowFieldError(txt, lblError, "Giá phải là số");
            }
            else if (price < 0)
            {
                ShowFieldError(txt, lblError, "Giá không được âm");
            }
            else if (price > 999999999)
            {
                ShowFieldError(txt, lblError, "Giá không được vượt quá 999,999,999");
            }
            // ✅ FIX: Cảnh báo giá = 0
            else if (price == 0)
            {
                ShowFieldError(txt, lblError, "⚠ Giá = 0 VNĐ (Miễn phí)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Light orange
            }
            else if (price > 0 && price < 10000)
            {
                ShowFieldError(txt, lblError, "⚠ Giá rất thấp (< 10,000 VNĐ)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Light orange
            }
            else if (price > 50000000)
            {
                ShowFieldError(txt, lblError, "⚠ Giá rất cao (> 50,000,000 VNĐ)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Light orange
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ShowFieldError(TextBox txt, Label lblError, string message)
        {
            if (txt != null)
            {
                if (message.StartsWith("⚠"))
                    txt.BackColor = Color.FromArgb(255, 243, 205); // Warning - light orange
                else
                    txt.BackColor = Color.FromArgb(255, 235, 238); // Error - light red
            }
            lblError.Text = message;
            lblError.ForeColor = message.StartsWith("⚠") ? Color.Orange : Color.Red;
            lblError.Visible = true;
        }

        private void ClearFieldError(TextBox txt, Label lblError)
        {
            if (txt != null)
                txt.BackColor = Color.White;
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

        // ✅ FIX: Helper methods mới

        /// <summary>
        /// Normalize whitespace - loại bỏ khoảng trắng thừa và khoảng trắng liên tiếp
        /// </summary>
        private string NormalizeWhitespace(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Loại bỏ khoảng trắng đầu/cuối và replace nhiều spaces thành 1 space
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Validate tên dịch vụ - chỉ cho phép chữ cái, số và một số ký tự đặc biệt
        /// </summary>
        private bool IsValidServiceName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Cho phép: chữ cái (có dấu), số, dấu cách, -, /, &, (, )
            // Ví dụ hợp lệ: "Nhổ răng khôn", "Điều trị tủy (RCT)", "Implant 3D/4D"
            return Regex.IsMatch(name, @"^[\p{L}\p{Nd}\s\-/&()]+$");
        }

        // Helper methods
        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
        }

        private TextBox CreateTextBox(int x, int y)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(350, 25),
                Font = new Font("Segoe UI", 9)
            };
        }

        private Button CreateButton(string text, int x, int y, string backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 35),
                BackColor = ColorTranslator.FromHtml(backColor),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            currentPage = 1; // Reset về trang 1 khi tìm kiếm
            LoadServices();
        }
    }
}
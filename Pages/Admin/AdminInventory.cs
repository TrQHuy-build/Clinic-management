using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminInventory : System.Windows.Forms.UserControl
    {
        private int? selectedItemId = null;

        public AdminInventory()
        {
            InitializeComponent();
            InitializeComboBox();
            LoadInventory();
        }

        private void InitializeComboBox()
        {
            if (cboType != null)
            {
                cboType.Items.Clear();
                cboType.Items.AddRange(new object[] { "Tất cả", "Material", "Equipment" });
                cboType.SelectedIndex = 0;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void CboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void LoadInventory()
        {
            try
            {
                string search = txtSearch?.Text?.Trim() ?? "";
                string type = cboType?.SelectedItem?.ToString();

                string query = @"
                    SELECT 
                        item_id AS [ID],
                        item_name AS [Tên vật tư/thiết bị],
                        type AS [Loại],
                        quantity AS [Số lượng],
                        unit AS [Đơn vị],
                        supplier AS [Nhà cung cấp]
                    FROM Inventory
                    WHERE 1=1";

                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (item_name LIKE @search OR supplier LIKE @search)";
                    parameterList.Add(new SqlParameter("@search", $"%{search}%"));
                }

                if (type != "Tất cả" && !string.IsNullOrEmpty(type))
                {
                    query += " AND type = @type";
                    parameterList.Add(new SqlParameter("@type", type));
                }

                query += " ORDER BY item_name";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameterList.ToArray());
                dgvInventory.DataSource = dt;

                // Add action columns if not exist
                if (!dgvInventory.Columns.Contains("Edit"))
                {
                    dgvInventory.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Edit",
                        Text = "Sửa",
                        UseColumnTextForButtonValue = true,
                        Width = 70
                    });

                    dgvInventory.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Delete",
                        Text = "Xóa",
                        UseColumnTextForButtonValue = true,
                        Width = 70
                    });

                    dgvInventory.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Export",
                        Text = "Xuất",
                        UseColumnTextForButtonValue = true,
                        Width = 70
                    });
                }

                // Hide ID column
                if (dgvInventory.Columns["ID"] != null)
                {
                    dgvInventory.Columns["ID"].Visible = false;
                }

                // Format type column
                foreach (DataGridViewRow row in dgvInventory.Rows)
                {
                    if (row.IsNewRow) continue;

                    string typeValue = row.Cells["Loại"].Value?.ToString();
                    if (typeValue?.ToLower() == "material")
                        row.Cells["Loại"].Value = "Vật tư";
                    else if (typeValue?.ToLower() == "equipment")
                        row.Cells["Loại"].Value = "Thiết bị";

                    // Format unit (nếu null)
                    if (row.Cells["Đơn vị"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Đơn vị"].Value?.ToString()))
                    {
                        row.Cells["Đơn vị"].Value = "N/A";
                    }

                    // Format supplier (nếu null)
                    if (row.Cells["Nhà cung cấp"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Nhà cung cấp"].Value?.ToString()))
                    {
                        row.Cells["Nhà cung cấp"].Value = "N/A";
                    }

                    // Highlight low stock
                    int quantity = Convert.ToInt32(row.Cells["Số lượng"].Value);
                    if (quantity < 20)
                    {
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                    }
                    else if (quantity == 0)
                    {
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFCDD2");
                        row.DefaultCellStyle.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void DgvInventory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int itemId = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["ID"].Value);
                string itemName = dgvInventory.Rows[e.RowIndex].Cells["Tên vật tư/thiết bị"].Value?.ToString() ?? "";

                if (e.ColumnIndex == dgvInventory.Columns["Edit"]?.Index)
                {
                    EditItem(itemId);
                }
                else if (e.ColumnIndex == dgvInventory.Columns["Delete"]?.Index)
                {
                    DeleteItem(itemId, itemName);
                }
                else if (e.ColumnIndex == dgvInventory.Columns["Export"]?.Index)
                {
                    ExportItem(itemId, itemName);
                }
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

        private void BtnImport_Click(object sender, EventArgs e)
        {
            ShowImportExportForm(true);
        }

        private void EditItem(int itemId)
        {
            ShowAddEditForm(itemId);
        }

        private void DeleteItem(int itemId, string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                MessageBoxHelper.ShowError("Không thể xác định vật tư/thiết bị cần xóa!");
                return;
            }

            if (!MessageBoxHelper.ShowDeleteConfirm(itemName))
                return;

            try
            {
                // Kiểm tra có giao dịch liên quan không
                object transCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM InventoryTransaction WHERE item_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", itemId) });

                if (Convert.ToInt32(transCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa vật tư/thiết bị này vì đã có giao dịch nhập/xuất liên quan!");
                    return;
                }

                string query = "DELETE FROM Inventory WHERE item_id = @itemId";
                SqlParameter[] parameters = { new SqlParameter("@itemId", itemId) };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(itemName);
                    Logger.LogDelete("Inventory", itemName);
                    LoadInventory();
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể xóa vật tư/thiết bị!");
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign key constraint
                {
                    MessageBoxHelper.ShowError("Không thể xóa vì có dữ liệu liên quan!");
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

        private void ExportItem(int itemId, string itemName)
        {
            ShowImportExportForm(false, itemId);
        }

        private void ShowAddEditForm(int? itemId)
        {
            Form form = new Form
            {
                Text = itemId.HasValue ? "Sửa vật tư/thiết bị" : "Thêm vật tư/thiết bị",
                Size = new Size(550, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            TextBox txtName = new TextBox { Location = new Point(150, 30), Size = new Size(350, 25) };
            Label lblNameError = CreateErrorLabel(150, 57);

            ComboBox cboTypeForm = new ComboBox { Location = new Point(150, 85), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboTypeForm.Items.AddRange(new object[] { "Material", "Equipment" });
            cboTypeForm.SelectedIndex = 0;

            TextBox txtQuantity = new TextBox { Location = new Point(150, 125), Size = new Size(350, 25), Text = "0" };
            Label lblQuantityError = CreateErrorLabel(150, 152);

            TextBox txtUnit = new TextBox { Location = new Point(150, 180), Size = new Size(350, 25) };
            Label lblUnitError = CreateErrorLabel(150, 207);

            TextBox txtSupplier = new TextBox { Location = new Point(150, 235), Size = new Size(350, 25) };
            Label lblSupplierError = CreateErrorLabel(150, 262);

            // Real-time validation
            txtName.TextChanged += (s, e) => ValidateItemNameRealtime(txtName, lblNameError);
            txtQuantity.TextChanged += (s, e) => ValidateQuantityRealtime(txtQuantity, lblQuantityError);
            txtUnit.TextChanged += (s, e) => ValidateUnitRealtime(txtUnit, lblUnitError);
            txtSupplier.TextChanged += (s, e) => ValidateSupplierRealtime(txtSupplier, lblSupplierError);

            form.Controls.Add(new Label { Text = "Tên:", Location = new Point(30, 33), AutoSize = true, Font = new Font("Segoe UI", 9) });
            form.Controls.Add(txtName);
            form.Controls.Add(lblNameError);
            form.Controls.Add(new Label { Text = "Loại:", Location = new Point(30, 88), AutoSize = true, Font = new Font("Segoe UI", 9) });
            form.Controls.Add(cboTypeForm);
            form.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(30, 128), AutoSize = true, Font = new Font("Segoe UI", 9) });
            form.Controls.Add(txtQuantity);
            form.Controls.Add(lblQuantityError);
            form.Controls.Add(new Label { Text = "Đơn vị:", Location = new Point(30, 183), AutoSize = true, Font = new Font("Segoe UI", 9) });
            form.Controls.Add(txtUnit);
            form.Controls.Add(lblUnitError);
            form.Controls.Add(new Label { Text = "Nhà cung cấp:", Location = new Point(30, 238), AutoSize = true, Font = new Font("Segoe UI", 9) });
            form.Controls.Add(txtSupplier);
            form.Controls.Add(lblSupplierError);

            // Load data if editing
            if (itemId.HasValue)
            {
                try
                {
                    string query = "SELECT * FROM Inventory WHERE item_id = @itemId";
                    SqlParameter[] parameters = { new SqlParameter("@itemId", itemId.Value) };
                    DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        txtName.Text = row["item_name"]?.ToString() ?? "";

                        string itemType = row["type"]?.ToString();
                        if (!string.IsNullOrEmpty(itemType))
                            cboTypeForm.SelectedItem = itemType;

                        txtQuantity.Text = row["quantity"]?.ToString() ?? "0";
                        txtUnit.Text = row["unit"]?.ToString() ?? "";
                        txtSupplier.Text = row["supplier"]?.ToString() ?? "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
                    form.Close();
                    return;
                }
            }

            Button btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(225, 310),
                Size = new Size(100, 35),
                BackColor = ColorTranslator.FromHtml("#007ACC"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnSave.Click += (s, ev) =>
            {
                if (!ValidateInventoryInput(txtName, txtQuantity, txtUnit, txtSupplier))
                    return;

                try
                {
                    int qty = int.Parse(txtQuantity.Text);
                    string query;
                    SqlParameter[] parameters;

                    if (itemId.HasValue)
                    {
                        query = @"UPDATE Inventory SET item_name=@name, type=@type, quantity=@qty, 
                                unit=@unit, supplier=@supplier WHERE item_id=@id";
                        parameters = new SqlParameter[] {
                            new SqlParameter("@name", txtName.Text.Trim()),
                            new SqlParameter("@type", cboTypeForm.SelectedItem.ToString()),
                            new SqlParameter("@qty", qty),
                            new SqlParameter("@unit", txtUnit.Text.Trim()), // Required field, no DBNull
                            new SqlParameter("@supplier", txtSupplier.Text.Trim()), // Required field, no DBNull
                            new SqlParameter("@id", itemId.Value)
                        };
                    }
                    else
                    {
                        // Kiểm tra tên trùng
                        object existingItem = DatabaseHelper.ExecuteScalar(
                            "SELECT COUNT(*) FROM Inventory WHERE item_name = @name",
                            new SqlParameter[] { new SqlParameter("@name", txtName.Text.Trim()) });

                        if (Convert.ToInt32(existingItem) > 0)
                        {
                            MessageBoxHelper.ShowValidationError("Tên vật tư/thiết bị đã tồn tại!");
                            txtName.Focus();
                            return;
                        }

                        query = @"INSERT INTO Inventory (item_name, type, quantity, unit, supplier) 
                                VALUES (@name, @type, @qty, @unit, @supplier)";
                        parameters = new SqlParameter[] {
                            new SqlParameter("@name", txtName.Text.Trim()),
                            new SqlParameter("@type", cboTypeForm.SelectedItem.ToString()),
                            new SqlParameter("@qty", qty),
                            new SqlParameter("@unit", txtUnit.Text.Trim()), // Required field, no DBNull
                            new SqlParameter("@supplier", txtSupplier.Text.Trim()) // Required field, no DBNull
                        };
                    }

                    int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        MessageBoxHelper.ShowSaveSuccess();
                        Logger.LogAction(itemId.HasValue ? "UPDATE_INVENTORY" : "CREATE_INVENTORY", txtName.Text);
                        form.Close();
                        LoadInventory();
                    }
                    else
                    {
                        MessageBoxHelper.ShowError("Không thể lưu dữ liệu!");
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                    {
                        MessageBoxHelper.ShowError("Tên vật tư/thiết bị đã tồn tại!");
                    }
                    else
                    {
                        MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                }
            };

            form.Controls.Add(btnSave);
            form.ShowDialog();
        }

        private void ShowImportExportForm(bool isImport, int? itemId = null)
        {
            Form form = new Form
            {
                Text = isImport ? "Nhập kho" : "Xuất kho",
                Size = new Size(500, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // ✅ TẠO CONTROLS
            Label lblItem = new Label
            {
                Text = "Vật tư:",
                Location = new Point(30, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            ComboBox cboItem = new ComboBox
            {
                Location = new Point(120, 30),
                Size = new Size(330, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblQty = new Label
            {
                Text = "Số lượng:",
                Location = new Point(30, 73),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            TextBox txtQty = new TextBox
            {
                Location = new Point(120, 70),
                Size = new Size(330, 25),
                Text = "1"
            };

            Label lblQtyError = CreateErrorLabel(120, 97);

            Label lblCurrentStock = new Label
            {
                Location = new Point(120, 110),
                AutoSize = true,
                ForeColor = Color.Blue,
                Font = new Font("Segoe UI", 9)
            };

            // ✅ VALIDATION REAL-TIME
            txtQty.TextChanged += (s, e) => ValidateImportExportQtyRealtime(txtQty, lblQtyError, isImport);

            // ✅ LOAD DỮ LIỆU VÀO COMBOBOX
            try
            {
                string queryItems = "SELECT item_id, item_name, quantity FROM Inventory ORDER BY item_name";
                DataTable dtItems = DatabaseHelper.ExecuteQuery(queryItems);

                if (dtItems == null || dtItems.Rows.Count == 0)
                {
                    MessageBoxHelper.ShowError("Không có vật tư/thiết bị nào trong kho!");
                    form.Close();
                    return;
                }

                cboItem.DataSource = dtItems;
                cboItem.DisplayMember = "item_name";
                cboItem.ValueMember = "item_id";

                // Event hiển thị tồn kho
                cboItem.SelectedIndexChanged += (s, e) =>
                {
                    if (cboItem.SelectedIndex >= 0 && cboItem.SelectedValue != null)
                    {
                        try
                        {
                            DataRowView row = (DataRowView)cboItem.SelectedItem;
                            int currentQty = Convert.ToInt32(row["quantity"]);
                            lblCurrentStock.Text = $"Tồn kho hiện tại: {currentQty}";

                            if (!isImport && currentQty == 0)
                            {
                                lblCurrentStock.Text += " (Hết hàng!)";
                                lblCurrentStock.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblCurrentStock.ForeColor = Color.Blue;
                            }
                        }
                        catch { }
                    }
                };

                // Chọn item
                if (itemId.HasValue)
                {
                    cboItem.SelectedValue = itemId.Value;
                    //cboItem.Enabled = false;
                }
                else if (cboItem.Items.Count > 0)
                {
                    cboItem.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải danh sách: {ex.Message}");
                form.Close();
                return;
            }

            // ✅ THÊM CONTROLS VÀO FORM
            form.Controls.Add(lblItem);
            form.Controls.Add(cboItem);
            form.Controls.Add(lblQty);
            form.Controls.Add(txtQty);
            form.Controls.Add(lblQtyError);
            form.Controls.Add(lblCurrentStock);

            // ✅ NÚT SUBMIT (giữ nguyên code cũ của bạn)
            Button btnSubmit = new Button
            {
                Text = isImport ? "Nhập kho" : "Xuất kho",
                Location = new Point(190, 160),
                Size = new Size(120, 35),
                BackColor = ColorTranslator.FromHtml(isImport ? "#28A745" : "#DC3545"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnSubmit.Click += (s, ev) =>
            {
                if (!int.TryParse(txtQty.Text, out int qty) || qty <= 0)
                {
                    MessageBoxHelper.ShowValidationError("Số lượng phải là số nguyên lớn hơn 0!");
                    txtQty.Focus();
                    return;
                }

                if (qty > 100000)
                {
                    MessageBoxHelper.ShowValidationError("Số lượng không được vượt quá 100,000!");
                    txtQty.Focus();
                    return;
                }

                int selectedItemId = Convert.ToInt32(cboItem.SelectedValue);
                string selectedItemName = cboItem.Text;

                try
                {
                    // === DÙNG TRANSACTION ĐỂ ĐẢM BẢO AN TOÀN ===
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                // 1. LẤY LẠI tồn kho HIỆN TẠI với khóa (SQL Server syntax)
                                string stockQuery = "SELECT quantity FROM Inventory WITH (UPDLOCK, ROWLOCK) WHERE item_id = @id";
                                var cmdStock = new SqlCommand(stockQuery, connection, transaction);
                                cmdStock.Parameters.AddWithValue("@id", selectedItemId);
                                object result = cmdStock.ExecuteScalar();

                                if (result == null || result == DBNull.Value)
                                {
                                    MessageBoxHelper.ShowError("Không tìm thấy vật tư/thiết bị trong kho!");
                                    transaction.Rollback();
                                    return;
                                }

                                int currentStock = Convert.ToInt32(result);

                                if (!isImport && currentStock < qty)
                                {
                                    MessageBoxHelper.ShowError($"Không đủ hàng để xuất!\n" +
                                                            $"Tồn kho hiện tại: {currentStock}\n" +
                                                            $"Yêu cầu xuất: {qty}");
                                    transaction.Rollback();
                                    return;
                                }

                                // 2. Cập nhật số lượng
                                string updateQuery = isImport
                                    ? "UPDATE Inventory SET quantity = quantity + @qty WHERE item_id = @id"
                                    : "UPDATE Inventory SET quantity = quantity - @qty WHERE item_id = @id";

                                var cmdUpdate = new SqlCommand(updateQuery, connection, transaction);
                                cmdUpdate.Parameters.AddWithValue("@qty", qty);
                                cmdUpdate.Parameters.AddWithValue("@id", selectedItemId);
                                int rowsAffected = cmdUpdate.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    MessageBoxHelper.ShowError("Cập nhật kho thất bại! Vui lòng thử lại.");
                                    return;
                                }

                                // 3. Ghi log giao dịch
                                string logQuery = @"INSERT INTO InventoryTransaction 
                          (item_id, quantity, type, staff_id, trans_date) 
                          VALUES (@id, @qty, @type, @staffId, GETDATE())";

                                var cmdLog = new SqlCommand(logQuery, connection, transaction);
                                cmdLog.Parameters.AddWithValue("@id", selectedItemId);
                                cmdLog.Parameters.AddWithValue("@qty", qty);
                                cmdLog.Parameters.AddWithValue("@type", isImport ? "import" : "export");
                                cmdLog.Parameters.AddWithValue("@staffId", Auth.CurrentStaffId ?? (object)DBNull.Value);
                                cmdLog.ExecuteNonQuery();

                                // 4. Commit nếu mọi thứ OK
                                transaction.Commit();

                                MessageBoxHelper.ShowSuccess(isImport ? "Nhập kho thành công!" : "Xuất kho thành công!");
                                if (isImport)
                                    Logger.LogImportInventory(selectedItemName, qty);
                                else
                                    Logger.LogExportInventory(selectedItemName, qty);

                                form.Close();
                                LoadInventory();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                MessageBoxHelper.ShowError($"Lỗi trong quá trình xử lý: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError($"Lỗi kết nối: {ex.Message}");
                }
            };

            form.Controls.Add(btnSubmit);
            form.ShowDialog();
        }

        // Validation methods
        private bool ValidateInventoryInput(TextBox txtName, TextBox txtQuantity, TextBox txtUnit, TextBox txtSupplier)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập tên vật tư/thiết bị!");
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length < 3)
            {
                MessageBoxHelper.ShowValidationError("Tên phải có ít nhất 3 ký tự!");
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length > 100)
            {
                MessageBoxHelper.ShowValidationError("Tên không được vượt quá 100 ký tự!");
                txtName.Focus();
                return false;
            }

            // Parse quantity early so 'qty' exists for subsequent checks
            if (!int.TryParse(txtQuantity.Text, out int qty))
            {
                MessageBoxHelper.ShowValidationError("Số lượng phải là số nguyên!");
                txtQuantity.Focus();
                return false;
            }

            if (qty < 0)
            {
                MessageBoxHelper.ShowValidationError("Số lượng không được âm!");
                txtQuantity.Focus();
                return false;
            }

            // Cảnh báo khi số lượng = 0
            if (qty == 0)
            {
                var result = MessageBox.Show(
                    "Số lượng đang là 0. Vật tư/thiết bị này sẽ ở trạng thái hết hàng. Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtQuantity.Focus();
                    return false;
                }
            }

            if (qty > 1000000)
            {
                MessageBoxHelper.ShowValidationError("Số lượng không được vượt quá 1,000,000!");
                txtQuantity.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtUnit.Text) && txtUnit.Text.Trim().Length > 50)
            {
                MessageBoxHelper.ShowValidationError("Đơn vị không được vượt quá 50 ký tự!");
                txtUnit.Focus();
                return false;
            }

            // Yêu cầu nhập đơn vị + không được là "0"
            if (string.IsNullOrWhiteSpace(txtUnit.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập đơn vị (VD: Cái, Hộp, Chiếc, Lọ, ...)!");
                txtUnit.Focus();
                return false;
            }

            string unit = txtUnit.Text.Trim();

            if (unit.Length < 1)
            {
                MessageBoxHelper.ShowValidationError("Đơn vị phải có ít nhất 1 ký tự!");
                txtUnit.Focus();
                return false;
            }

            if (unit.Length > 50)
            {
                MessageBoxHelper.ShowValidationError("Đơn vị không được vượt quá 50 ký tự!");
                txtUnit.Focus();
                return false;
            }

            // Chặn trường hợp nhập "0", "00", "000", hoặc chỉ toàn số 0
            if (unit.All(c => c == '0' || char.IsWhiteSpace(c)))
            {
                MessageBoxHelper.ShowValidationError("Đơn vị không hợp lệ! Không được nhập chỉ toàn số 0.");
                txtUnit.Focus();
                return false;
            }

            // (Tùy chọn mạnh hơn) Chặn luôn nếu chỉ chứa số (ví dụ: "123", "500")
            if (int.TryParse(unit, out _))
            {
                MessageBoxHelper.ShowValidationError("Đơn vị không được là một con số! Vui lòng nhập chữ (VD: Cái, Hộp, Lọ, Viên, ...)");
                txtUnit.Focus();
                return false;
            }

            return true; // Hợp lệ

            // Yêu cầu nhập nhà cung cấp
            if (string.IsNullOrWhiteSpace(txtSupplier.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập tên nhà cung cấp!");
                txtSupplier.Focus();
                return false;
            }

            if (txtSupplier.Text.Trim().Length < 2)
            {
                MessageBoxHelper.ShowValidationError("Tên nhà cung cấp phải có ít nhất 2 ký tự!");
                txtSupplier.Focus();
                return false;
            }

            if (txtSupplier.Text.Trim().Length > 100)
            {
                MessageBoxHelper.ShowValidationError("Tên nhà cung cấp không được vượt quá 100 ký tự!");
                txtSupplier.Focus();
                return false;
            }

            return true;
        }

        // Real-time validation methods
        private void ValidateItemNameRealtime(TextBox txt, Label lblError)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Tên không được để trống");
            }
            else if (txt.Text.Trim().Length < 3)
            {
                ShowFieldError(txt, lblError, "Tên phải có ít nhất 3 ký tự");
            }
            else if (txt.Text.Trim().Length > 100)
            {
                ShowFieldError(txt, lblError, "Tên không được vượt quá 100 ký tự");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidateQuantityRealtime(TextBox txt, Label lblError)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Số lượng không được để trống");
            }
            else if (!int.TryParse(txt.Text, out int qty))
            {
                ShowFieldError(txt, lblError, "Số lượng phải là số nguyên");
            }
            else if (qty < 0)
            {
                ShowFieldError(txt, lblError, "Số lượng không được âm");
            }
            else if (qty == 0)
            {
                ShowFieldError(txt, lblError, "⚠ Số lượng = 0 (hết hàng)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Warning color
            }
            else if (qty > 1000000)
            {
                ShowFieldError(txt, lblError, "Số lượng không được vượt quá 1,000,000");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidateUnitRealtime(TextBox txt, Label lblError)
        {
            string unit = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Đơn vị không được để trống");
                return;
            }

            if (unit.Length < 1)
            {
                ShowFieldError(txt, lblError, "Đơn vị phải có ít nhất 1 ký tự");
                return;
            }

            if (unit.Length > 50)
            {
                ShowFieldError(txt, lblError, "Đơn vị không được vượt quá 50 ký tự");
                return;
            }

            // Chặn nhập chỉ toàn số 0
            if (unit.All(c => c == '0' || char.IsWhiteSpace(c)))
            {
                ShowFieldError(txt, lblError, "Không được nhập chỉ toàn số 0");
                return;
            }

            // (Tùy chọn) Chặn luôn nếu chỉ chứa số
            if (int.TryParse(unit, out _))
            {
                ShowFieldError(txt, lblError, "Đơn vị không được là số! (VD: dùng 'Cái' thay vì '1')");
                return;
            }

            // Nếu qua hết các kiểm tra → hợp lệ
            ClearFieldError(txt, lblError);
        }

        private void ValidateSupplierRealtime(TextBox txt, Label lblError)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Nhà cung cấp không được để trống");
            }
            else if (txt.Text.Trim().Length < 2)
            {
                ShowFieldError(txt, lblError, "Nhà cung cấp phải có ít nhất 2 ký tự");
            }
            else if (txt.Text.Trim().Length > 100)
            {
                ShowFieldError(txt, lblError, "Nhà cung cấp không được vượt quá 100 ký tự");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidateImportExportQtyRealtime(TextBox txt, Label lblError, bool isImport)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Số lượng không được để trống");
            }
            else if (!int.TryParse(txt.Text, out int qty))
            {
                ShowFieldError(txt, lblError, "Số lượng phải là số nguyên");
            }
            else if (qty <= 0)
            {
                ShowFieldError(txt, lblError, "Số lượng phải lớn hơn 0");
            }
            else if (qty > 100000)
            {
                ShowFieldError(txt, lblError, "Số lượng không được vượt quá 100,000");
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
    }
}
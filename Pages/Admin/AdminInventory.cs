using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminInventory : System.Windows.Forms.UserControl
    {
        private int? selectedItemId = null;

        public AdminInventory()
        {
            InitializeComponent();
            LoadInventory();
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
                string search = txtSearch.Text.Trim();
                string type = cboType.SelectedItem?.ToString();

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

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (item_name LIKE @search OR supplier LIKE @search)";
                }

                if (type != "Tất cả" && !string.IsNullOrEmpty(type))
                {
                    query += " AND type = @type";
                }

                query += " ORDER BY item_name";

                SqlParameter[] parameters = {
                    new SqlParameter("@search", $"%{search}%"),
                    new SqlParameter("@type", type)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
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
                    string typeValue = row.Cells["Loại"].Value?.ToString();
                    if (typeValue?.ToLower() == "material")
                        row.Cells["Loại"].Value = "Vật tư";
                    else if (typeValue?.ToLower() == "equipment")
                        row.Cells["Loại"].Value = "Thiết bị";

                    // Highlight low stock
                    int quantity = Convert.ToInt32(row.Cells["Số lượng"].Value);
                    if (quantity < 20)
                    {
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
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

            int itemId = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["ID"].Value);
            string itemName = dgvInventory.Rows[e.RowIndex].Cells["Tên vật tư/thiết bị"].Value.ToString();

            if (e.ColumnIndex == dgvInventory.Columns["Edit"].Index)
            {
                EditItem(itemId);
            }
            else if (e.ColumnIndex == dgvInventory.Columns["Delete"].Index)
            {
                DeleteItem(itemId, itemName);
            }
            else if (e.ColumnIndex == dgvInventory.Columns["Export"].Index)
            {
                ExportItem(itemId, itemName);
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
            if (!MessageBoxHelper.ShowDeleteConfirm(itemName))
                return;

            try
            {
                string query = "DELETE FROM Inventory WHERE item_id = @itemId";
                SqlParameter[] parameters = { new SqlParameter("@itemId", itemId) };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(itemName);
                    Logger.LogDelete("Inventory", itemName);
                    LoadInventory();
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
                Size = new Size(500, 450),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            TextBox txtName = new TextBox { Location = new Point(150, 30), Size = new Size(300, 25) };
            ComboBox cboTypeForm = new ComboBox { Location = new Point(150, 70), Size = new Size(300, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboTypeForm.Items.AddRange(new object[] { "Material", "Equipment" });
            cboTypeForm.SelectedIndex = 0;

            TextBox txtQuantity = new TextBox { Location = new Point(150, 110), Size = new Size(300, 25), Text = "0" };
            TextBox txtUnit = new TextBox { Location = new Point(150, 150), Size = new Size(300, 25) };
            TextBox txtSupplier = new TextBox { Location = new Point(150, 190), Size = new Size(300, 25) };

            form.Controls.Add(new Label { Text = "Tên:", Location = new Point(30, 33), AutoSize = true });
            form.Controls.Add(txtName);
            form.Controls.Add(new Label { Text = "Loại:", Location = new Point(30, 73), AutoSize = true });
            form.Controls.Add(cboTypeForm);
            form.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(30, 113), AutoSize = true });
            form.Controls.Add(txtQuantity);
            form.Controls.Add(new Label { Text = "Đơn vị:", Location = new Point(30, 153), AutoSize = true });
            form.Controls.Add(txtUnit);
            form.Controls.Add(new Label { Text = "Nhà cung cấp:", Location = new Point(30, 193), AutoSize = true });
            form.Controls.Add(txtSupplier);

            // Load data if editing
            if (itemId.HasValue)
            {
                string query = "SELECT * FROM Inventory WHERE item_id = @itemId";
                SqlParameter[] parameters = { new SqlParameter("@itemId", itemId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    txtName.Text = dt.Rows[0]["item_name"].ToString();
                    cboTypeForm.SelectedItem = dt.Rows[0]["type"].ToString();
                    txtQuantity.Text = dt.Rows[0]["quantity"].ToString();
                    txtUnit.Text = dt.Rows[0]["unit"].ToString();
                    txtSupplier.Text = dt.Rows[0]["supplier"].ToString();
                }
            }

            Button btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(200, 250),
                Size = new Size(100, 35),
                BackColor = ColorTranslator.FromHtml("#007ACC"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += (s, ev) =>
            {
                if (!Validator.IsNotEmpty(txtName.Text))
                {
                    MessageBoxHelper.ShowValidationError("tên");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int qty) || qty < 0)
                {
                    MessageBoxHelper.ShowValidationError("Số lượng phải là số không âm!");
                    return;
                }

                try
                {
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
                            new SqlParameter("@unit", txtUnit.Text.Trim()),
                            new SqlParameter("@supplier", txtSupplier.Text.Trim()),
                            new SqlParameter("@id", itemId.Value)
                        };
                    }
                    else
                    {
                        query = @"INSERT INTO Inventory (item_name, type, quantity, unit, supplier) 
                                VALUES (@name, @type, @qty, @unit, @supplier)";
                        parameters = new SqlParameter[] {
                            new SqlParameter("@name", txtName.Text.Trim()),
                            new SqlParameter("@type", cboTypeForm.SelectedItem.ToString()),
                            new SqlParameter("@qty", qty),
                            new SqlParameter("@unit", txtUnit.Text.Trim()),
                            new SqlParameter("@supplier", txtSupplier.Text.Trim())
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
                Size = new Size(450, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            ComboBox cboItem = new ComboBox { Location = new Point(120, 30), Size = new Size(280, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            TextBox txtQty = new TextBox { Location = new Point(120, 70), Size = new Size(280, 25) };

            // Load items
            DataTable dtItems = DatabaseHelper.ExecuteQuery("SELECT item_id, item_name FROM Inventory ORDER BY item_name");
            cboItem.DisplayMember = "item_name";
            cboItem.ValueMember = "item_id";
            cboItem.DataSource = dtItems;

            if (itemId.HasValue)
            {
                cboItem.SelectedValue = itemId.Value;
                cboItem.Enabled = false;
            }

            form.Controls.Add(new Label { Text = "Vật tư:", Location = new Point(30, 33), AutoSize = true });
            form.Controls.Add(cboItem);
            form.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(30, 73), AutoSize = true });
            form.Controls.Add(txtQty);

            Button btnSubmit = new Button
            {
                Text = isImport ? "Nhập kho" : "Xuất kho",
                Location = new Point(150, 130),
                Size = new Size(120, 35),
                BackColor = ColorTranslator.FromHtml(isImport ? "#28A745" : "#DC3545"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSubmit.Click += (s, ev) =>
            {
                if (!int.TryParse(txtQty.Text, out int qty) || qty <= 0)
                {
                    MessageBoxHelper.ShowValidationError("Số lượng phải lớn hơn 0!");
                    return;
                }

                try
                {
                    int selectedItemId = Convert.ToInt32(cboItem.SelectedValue);
                    string selectedItemName = cboItem.Text;

                    // Update inventory
                    string updateQuery = isImport
                        ? "UPDATE Inventory SET quantity = quantity + @qty WHERE item_id = @id"
                        : "UPDATE Inventory SET quantity = quantity - @qty WHERE item_id = @id";

                    SqlParameter[] updateParams = {
                        new SqlParameter("@qty", qty),
                        new SqlParameter("@id", selectedItemId)
                    };

                    // Insert transaction log
                    string logQuery = @"INSERT INTO InventoryTransaction (item_id, quantity, type, staff_id) 
                                      VALUES (@id, @qty, @type, @staffId)";
                    SqlParameter[] logParams = {
                        new SqlParameter("@id", selectedItemId),
                        new SqlParameter("@qty", qty),
                        new SqlParameter("@type", isImport ? "import" : "export"),
                        new SqlParameter("@staffId", Auth.CurrentStaffId ?? (object)DBNull.Value)
                    };

                    DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
                    DatabaseHelper.ExecuteNonQuery(logQuery, logParams);

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
                    MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                }
            };

            form.Controls.Add(btnSubmit);
            form.ShowDialog();
        }
    }
}
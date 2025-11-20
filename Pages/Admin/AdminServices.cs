using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminServices : UserControl
    {
        public AdminServices()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            try
            {
                string search = txtSearch.Text.Trim();
                string query = @"
                    SELECT
                        service_id AS [ID],
                        service_name AS [Tên dịch vụ],
                        description AS [Mô tả],
                        price AS [Giá],
                        status AS [Trạng thái]
                    FROM Service
                    WHERE service_name LIKE @search OR description LIKE @search
                    ORDER BY service_name";

                SqlParameter[] parameters = { new SqlParameter("@search", $"%{search}%") };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                dgvServices.DataSource = dt;

                // Ẩn ID
                if (dgvServices.Columns["ID"] != null)
                    dgvServices.Columns["ID"].Visible = false;

                // Định dạng giá
                if (dgvServices.Columns["Giá"] != null)
                {
                    dgvServices.Columns["Giá"].DefaultCellStyle.Format = "N0";
                    dgvServices.Columns["Giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Thêm cột nút
                AddButtonColumn("Edit", "Sửa", 70);
                AddButtonColumn("Delete", "Xóa", 70);

                // Định dạng trạng thái
                foreach (DataGridViewRow row in dgvServices.Rows)
                {
                    if (row.Cells["Trạng thái"].Value is string status)
                    {
                        row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);
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

        private void DgvServices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int serviceId = Convert.ToInt32(dgvServices.Rows[e.RowIndex].Cells["ID"].Value);
            string serviceName = dgvServices.Rows[e.RowIndex].Cells["Tên dịch vụ"].Value.ToString();

            if (e.ColumnIndex == dgvServices.Columns["Edit"].Index)
                ShowAddEditForm(serviceId);
            else if (e.ColumnIndex == dgvServices.Columns["Delete"].Index)
                DeleteService(serviceId, serviceName);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
        }

        private void ShowAddEditForm(int? serviceId)
        {
            using (Form form = new Form
            {
                Text = serviceId.HasValue ? "Sửa dịch vụ" : "Thêm dịch vụ mới",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            })
            {
                TextBox txtName = CreateTextBox(150, 30);
                TextBox txtDesc = new TextBox { Location = new Point(150, 70), Size = new Size(300, 60), Multiline = true };
                TextBox txtPrice = CreateTextBox(150, 150);
                ComboBox cboStatus = new ComboBox
                {
                    Location = new Point(150, 190),
                    Size = new Size(300, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cboStatus.Items.AddRange(new object[] { "available", "unavailable" });
                cboStatus.SelectedIndex = 0;

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Tên dịch vụ:", 30, 33), txtName,
                    CreateLabel("Mô tả:", 30, 73), txtDesc,
                    CreateLabel("Giá (VNĐ):", 30, 153), txtPrice,
                    CreateLabel("Trạng thái:", 30, 193), cboStatus
                });

                // Load dữ liệu nếu sửa
                if (serviceId.HasValue)
                {
                    string query = "SELECT * FROM Service WHERE service_id = @id";
                    DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", serviceId.Value) });
                    if (dt.Rows.Count > 0)
                    {
                        txtName.Text = dt.Rows[0]["service_name"].ToString();
                        txtDesc.Text = dt.Rows[0]["description"].ToString();
                        txtPrice.Text = dt.Rows[0]["price"].ToString();
                        cboStatus.SelectedItem = dt.Rows[0]["status"].ToString();
                    }
                }

                Button btnSave = CreateButton("Lưu", 200, 250, "#007ACC");
                btnSave.Click += (s, ev) =>
                {
                    if (!Validator.IsNotEmpty(txtName.Text))
                    {
                        MessageBoxHelper.ShowValidationError("tên dịch vụ");
                        return;
                    }

                    if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                    {
                        MessageBoxHelper.ShowValidationError("Giá phải là số không âm!");
                        return;
                    }

                    try
                    {
                        string query;
                        SqlParameter[] parameters;

                        if (serviceId.HasValue)
                        {
                            query = @"UPDATE Service SET service_name=@name, description=@desc,
                                    price=@price, status=@status WHERE service_id=@id";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", txtName.Text.Trim()),
                                new SqlParameter("@desc", txtDesc.Text.Trim()),
                                new SqlParameter("@price", price),
                                new SqlParameter("@status", cboStatus.SelectedItem.ToString()),
                                new SqlParameter("@id", serviceId.Value)
                            };
                        }
                        else
                        {
                            query = @"INSERT INTO Service (service_name, description, price, status)
                                    VALUES (@name, @desc, @price, @status)";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", txtName.Text.Trim()),
                                new SqlParameter("@desc", txtDesc.Text.Trim()),
                                new SqlParameter("@price", price),
                                new SqlParameter("@status", cboStatus.SelectedItem.ToString())
                            };
                        }

                        if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                        {
                            MessageBoxHelper.ShowSaveSuccess();
                            Logger.LogAction(serviceId.HasValue ? "UPDATE_SERVICE" : "CREATE_SERVICE", txtName.Text);
                            form.Close();
                            LoadServices();
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
        }

        private void DeleteService(int serviceId, string serviceName)
        {
            if (!MessageBoxHelper.ShowDeleteConfirm(serviceName)) return;

            try
            {
                string query = "DELETE FROM Service WHERE service_id = @id";
                if (DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@id", serviceId) }) > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(serviceName);
                    Logger.LogDelete("Service", serviceName);
                    LoadServices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi xóa: {ex.Message}");
            }
        }

        // Helper methods
        private Label CreateLabel(string text, int x, int y)
        {
            return new Label { Text = text, Location = new Point(x, y), AutoSize = true };
        }

        private TextBox CreateTextBox(int x, int y)
        {
            return new TextBox { Location = new Point(x, y), Size = new Size(300, 25) };
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
                FlatStyle = FlatStyle.Flat
            };
        }

        // Event handlers
        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadServices();
    }
}
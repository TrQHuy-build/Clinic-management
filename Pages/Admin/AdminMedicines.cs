using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminMedicines : UserControl
    {
        public AdminMedicines()
        {
            InitializeComponent();
            LoadMedicines();
        }

        private void LoadMedicines()
        {
            try
            {
                string search = txtSearch.Text.Trim();
                string query = @"
                    SELECT
                        medicine_id AS [ID],
                        name AS [Tên thuốc],
                        unit AS [Đơn vị],
                        manufacturer AS [Nhà sản xuất],
                        price AS [Giá]
                    FROM Medicine
                    WHERE name LIKE @search OR manufacturer LIKE @search
                    ORDER BY name";

                SqlParameter[] parameters = { new SqlParameter("@search", $"%{search}%") };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                dgvMedicines.DataSource = dt;

                // Ẩn cột ID
                if (dgvMedicines.Columns["ID"] != null)
                    dgvMedicines.Columns["ID"].Visible = false;

                // Định dạng giá
                if (dgvMedicines.Columns["Giá"] != null)
                    dgvMedicines.Columns["Giá"].DefaultCellStyle.Format = "N0";

                // Thêm cột nút
                AddButtonColumnIfNotExists("Edit", "Sửa", 70);
                AddButtonColumnIfNotExists("Delete", "Xóa", 70);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khi tải thuốc: {ex.Message}");
            }
        }

        private void AddButtonColumnIfNotExists(string name, string text, int width)
        {
            if (!dgvMedicines.Columns.Contains(name))
            {
                dgvMedicines.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = name,
                    HeaderText = "",
                    Text = text,
                    UseColumnTextForButtonValue = true,
                    Width = width
                });
            }
        }

        private void DgvMedicines_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int medicineId = Convert.ToInt32(dgvMedicines.Rows[e.RowIndex].Cells["ID"].Value);
            string name = dgvMedicines.Rows[e.RowIndex].Cells["Tên thuốc"].Value.ToString();

            if (e.ColumnIndex == dgvMedicines.Columns["Edit"].Index)
                ShowAddEditForm(medicineId);
            else if (e.ColumnIndex == dgvMedicines.Columns["Delete"].Index)
                DeleteMedicine(medicineId, name);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
        }

        private void ShowAddEditForm(int? medicineId)
        {
            using (Form form = new Form
            {
                Text = medicineId.HasValue ? "Sửa thuốc" : "Thêm thuốc mới",
                Size = new Size(500, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            })
            {
                TextBox txtName = CreateTextBox(150, 30);
                TextBox txtUnit = CreateTextBox(150, 70);
                TextBox txtManufacturer = CreateTextBox(150, 110);
                TextBox txtPrice = CreateTextBox(150, 150);

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Tên thuốc:", 30, 33),
                    txtName,
                    CreateLabel("Đơn vị:", 30, 73),
                    txtUnit,
                    CreateLabel("Nhà SX:", 30, 113),
                    txtManufacturer,
                    CreateLabel("Giá:", 30, 153),
                    txtPrice
                });

                // Load dữ liệu nếu sửa
                if (medicineId.HasValue)
                {
                    string query = "SELECT * FROM Medicine WHERE medicine_id = @id";
                    DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", medicineId.Value) });
                    if (dt.Rows.Count > 0)
                    {
                        txtName.Text = dt.Rows[0]["name"].ToString();
                        txtUnit.Text = dt.Rows[0]["unit"].ToString();
                        txtManufacturer.Text = dt.Rows[0]["manufacturer"].ToString();
                        txtPrice.Text = dt.Rows[0]["price"].ToString();
                    }
                }

                Button btnSave = new Button
                {
                    Text = "Lưu",
                    Location = new Point(200, 200),
                    Size = new Size(100, 35),
                    BackColor = ColorTranslator.FromHtml("#007ACC"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnSave.Click += (s, ev) =>
                {
                    if (!Validator.IsNotEmpty(txtName.Text))
                    {
                        MessageBoxHelper.ShowValidationError("tên thuốc");
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

                        if (medicineId.HasValue)
                        {
                            query = "UPDATE Medicine SET name=@name, unit=@unit, manufacturer=@manu, price=@price WHERE medicine_id=@id";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", txtName.Text.Trim()),
                                new SqlParameter("@unit", txtUnit.Text.Trim()),
                                new SqlParameter("@manu", txtManufacturer.Text.Trim()),
                                new SqlParameter("@price", price),
                                new SqlParameter("@id", medicineId.Value)
                            };
                        }
                        else
                        {
                            query = "INSERT INTO Medicine (name, unit, manufacturer, price) VALUES (@name, @unit, @manu, @price)";
                            parameters = new SqlParameter[]
                            {
                                new SqlParameter("@name", txtName.Text.Trim()),
                                new SqlParameter("@unit", txtUnit.Text.Trim()),
                                new SqlParameter("@manu", txtManufacturer.Text.Trim()),
                                new SqlParameter("@price", price)
                            };
                        }

                        if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                        {
                            MessageBoxHelper.ShowSaveSuccess();
                            Logger.LogAction(medicineId.HasValue ? "UPDATE_MEDICINE" : "CREATE_MEDICINE", txtName.Text);
                            form.Close();
                            LoadMedicines();
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

        private void DeleteMedicine(int medicineId, string name)
        {
            if (!MessageBoxHelper.ShowDeleteConfirm(name)) return;

            try
            {
                string query = "DELETE FROM Medicine WHERE medicine_id = @id";
                if (DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@id", medicineId) }) > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(name);
                    Logger.LogDelete("Medicine", name);
                    LoadMedicines();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
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

        // Event handlers
        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadMedicines();
    }
}
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminMedicines : UserControl
    {
        public AdminMedicines()
        {
            InitializeComponent();

            // ✅ ĐẢM BẢO KẾT NỐI SỰ KIỆN TÌM KIẾM
            if (txtSearch != null)
            {
                txtSearch.TextChanged -= txtSearch_TextChanged; // Xóa nếu đã có
                txtSearch.TextChanged += txtSearch_TextChanged; // Thêm mới
            }

            LoadMedicines();
        }

        private void LoadMedicines()
        {
            try
            {
                string search = txtSearch?.Text?.Trim() ?? "";

                // ✅ SỬA QUERY - THÊM ĐIỀU KIỆN ĐÚNG
                string query = @"
                    SELECT
                        medicine_id AS [ID],
                        name AS [Tên thuốc],
                        unit AS [Đơn vị],
                        manufacturer AS [Nhà sản xuất],
                        price AS [Giá]
                    FROM Medicine
                    WHERE (@search = '' OR name LIKE @search OR manufacturer LIKE @search)
                    ORDER BY name";

                SqlParameter[] parameters = { new SqlParameter("@search", $"%{search}%") };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvMedicines.DataSource = dt;

                // Ẩn cột ID
                if (dgvMedicines.Columns["ID"] != null)
                    dgvMedicines.Columns["ID"].Visible = false;

                // Định dạng giá
                if (dgvMedicines.Columns["Giá"] != null)
                {
                    dgvMedicines.Columns["Giá"].DefaultCellStyle.Format = "N0";
                    dgvMedicines.Columns["Giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Thêm cột nút
                AddButtonColumnIfNotExists("Edit", "Sửa", 70);
                AddButtonColumnIfNotExists("Delete", "Xóa", 70);

                // Format các cột có thể null + highlight giá cao
                foreach (DataGridViewRow row in dgvMedicines.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (row.Cells["Đơn vị"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Đơn vị"].Value?.ToString()))
                    {
                        row.Cells["Đơn vị"].Value = "N/A";
                    }

                    if (row.Cells["Nhà sản xuất"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Nhà sản xuất"].Value?.ToString()))
                    {
                        row.Cells["Nhà sản xuất"].Value = "N/A";
                    }

                    if (decimal.TryParse(row.Cells["Giá"].Value?.ToString(), out decimal price) && price > 500000)
                    {
                        row.Cells["Giá"].Style.ForeColor = Color.Red;
                        row.Cells["Giá"].Style.Font = new Font(dgvMedicines.Font, FontStyle.Bold);
                    }
                }
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
            try
            {
                int medicineId = Convert.ToInt32(dgvMedicines.Rows[e.RowIndex].Cells["ID"].Value);
                string name = dgvMedicines.Rows[e.RowIndex].Cells["Tên thuốc"].Value?.ToString() ?? "";
                if (e.ColumnIndex == dgvMedicines.Columns["Edit"]?.Index)
                    ShowAddEditForm(medicineId);
                else if (e.ColumnIndex == dgvMedicines.Columns["Delete"]?.Index)
                    DeleteMedicine(medicineId, name);
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
                LoadMedicines();
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
        }

        private void ShowAddEditForm(int? medicineId)
        {
            using (Form form = new Form
            {
                Text = medicineId.HasValue ? "Sửa thuốc" : "Thêm thuốc mới",
                Size = new Size(550, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                TextBox txtName = CreateTextBox(150, 30);
                Label lblNameError = CreateErrorLabel(150, 57);
                TextBox txtUnit = CreateTextBox(150, 85);
                Label lblUnitError = CreateErrorLabel(150, 112);
                TextBox txtManufacturer = CreateTextBox(150, 140);
                Label lblManufacturerError = CreateErrorLabel(150, 167);
                TextBox txtPrice = CreateTextBox(150, 195);
                Label lblPriceError = CreateErrorLabel(150, 222);

                // REALTIME VALIDATION
                txtName.TextChanged += (s, e) => ValidateNameRealtime(txtName, lblNameError, medicineId);
                txtUnit.TextChanged += (s, e) => ValidateUnitRealtime(txtUnit, lblUnitError);
                txtManufacturer.TextChanged += (s, e) => ValidateManufacturerRealtime(txtManufacturer, lblManufacturerError);
                txtPrice.TextChanged += (s, e) => ValidatePriceRealtime(txtPrice, lblPriceError);

                // Chỉ cho nhập số ở ô giá
                txtPrice.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                        e.Handled = true;
                };

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Tên thuốc:", 30, 33), txtName, lblNameError,
                    CreateLabel("Đơn vị:", 30, 88), txtUnit, lblUnitError,
                    CreateLabel("Nhà sản xuất:", 30, 143), txtManufacturer, lblManufacturerError,
                    CreateLabel("Giá (VNĐ):", 30, 198), txtPrice, lblPriceError
                });

                if (medicineId.HasValue)
                {
                    try
                    {
                        string query = "SELECT * FROM Medicine WHERE medicine_id = @id";
                        DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", medicineId.Value) });
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtName.Text = row["name"]?.ToString() ?? "";
                            txtUnit.Text = row["unit"]?.ToString() ?? "";
                            txtManufacturer.Text = row["manufacturer"]?.ToString() ?? "";
                            txtPrice.Text = row["price"]?.ToString() ?? "0";
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
                    Location = new Point(225, 260),
                    Size = new Size(100, 35),
                    BackColor = ColorTranslator.FromHtml("#007ACC"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                btnSave.Click += (s, ev) =>
                {
                    // Normalize khoảng trắng
                    txtName.Text = Normalize(txtName.Text);
                    txtUnit.Text = Normalize(txtUnit.Text);
                    txtManufacturer.Text = Normalize(txtManufacturer.Text);

                    // Kiểm tra lại lần cuối
                    ValidateNameRealtime(txtName, lblNameError, medicineId);
                    ValidateUnitRealtime(txtUnit, lblUnitError);
                    ValidateManufacturerRealtime(txtManufacturer, lblManufacturerError);
                    ValidatePriceRealtime(txtPrice, lblPriceError);

                    if (lblNameError.Visible || lblUnitError.Visible || lblManufacturerError.Visible || lblPriceError.Visible)
                    {
                        MessageBoxHelper.ShowValidationError("Vui lòng sửa các lỗi được đánh dấu trước khi lưu!");
                        return;
                    }

                    if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0 || price > 999999999)
                    {
                        MessageBoxHelper.ShowValidationError("Giá không hợp lệ!");
                        return;
                    }

                    // Xác nhận giá bất thường
                    if (price == 0 || price < 100 || price > 10000000)
                    {
                        string msg = price == 0 ? "Giá thuốc bằng 0 VNĐ" :
                                    price < 100 ? "Giá thuốc rất thấp (dưới 100 VNĐ)" :
                                    "Giá thuốc rất cao (trên 10 triệu VNĐ)";

                        if (MessageBox.Show($"{msg}\nBạn có chắc chắn muốn lưu không?", "Xác nhận giá thuốc",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            return;
                    }

                    try
                    {
                        string query;
                        SqlParameter[] parameters;

                        if (medicineId.HasValue)
                        {
                            query = @"UPDATE Medicine SET name=@name, unit=@unit, manufacturer=@manu, price=@price
                                     WHERE medicine_id=@id";
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

                        int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                        if (result > 0)
                        {
                            MessageBoxHelper.ShowSaveSuccess();
                            Logger.LogAction(medicineId.HasValue ? "UPDATE_MEDICINE" : "CREATE_MEDICINE", txtName.Text);
                            form.Close();
                            LoadMedicines();
                        }
                        else
                        {
                            MessageBoxHelper.ShowError("Không thể lưu dữ liệu!");
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                            MessageBoxHelper.ShowError("Tên thuốc đã tồn tại!");
                        else
                            MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
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
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBoxHelper.ShowError("Không thể xác định thuốc cần xóa!");
                return;
            }
            if (!MessageBoxHelper.ShowDeleteConfirm(name)) return;

            try
            {
                object prescriptionCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Prescription WHERE medicine_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", medicineId) });

                if (Convert.ToInt32(prescriptionCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa thuốc này vì đã có đơn thuốc sử dụng!");
                    return;
                }

                string query = "DELETE FROM Medicine WHERE medicine_id = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@id", medicineId) });

                if (result > 0)
                {
                    MessageBoxHelper.ShowDeleteSuccess(name);
                    Logger.LogDelete("Medicine", name);
                    LoadMedicines();
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể xóa thuốc!");
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547)
                    MessageBoxHelper.ShowError("Không thể xóa thuốc vì có dữ liệu liên quan!");
                else
                    MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
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

        // ✅ SỰ KIỆN TÌM KIẾM - ĐẢM BẢO HOẠT ĐỘNG
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadMedicines();
        }

        // ================================ VALIDATION ===============================

        private readonly HashSet<string> _validUnits = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "viên", "lọ", "ống", "tuýp", "chai", "hộp", "vỉ", "liều", "gói",
            "ml", "mg", "g", "kg", "lít", "litre", "liter", "cc", "gam", "gram"
        };

        private string Normalize(string s) => string.IsNullOrWhiteSpace(s) ? "" : Regex.Replace(s.Trim(), @"\s+", " ");

        private bool NameExists(string name, int? currentId = null)
        {
            string sql = currentId.HasValue
                ? "SELECT COUNT(*) FROM Medicine WHERE name = @name AND medicine_id <> @id"
                : "SELECT COUNT(*) FROM Medicine WHERE name = @name";

            var p = currentId.HasValue
                ? new[] { new SqlParameter("@name", name), new SqlParameter("@id", currentId.Value) }
                : new[] { new SqlParameter("@name", name) };

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, p)) > 0;
        }

        private void ValidateNameRealtime(TextBox txt, Label lbl, int? currentId)
        {
            string s = Normalize(txt.Text);
            if (string.IsNullOrWhiteSpace(s))
                ShowFieldError(txt, lbl, "Tên thuốc không được để trống");
            else if (s.Length < 3)
                ShowFieldError(txt, lbl, "Tên thuốc phải từ 3 ký tự trở lên");
            else if (s.Length > 100)
                ShowFieldError(txt, lbl, "Tên thuốc không quá 100 ký tự");
            else if (!Regex.IsMatch(s, @"^[\p{L}\p{N}\s\/&\(\)\-]+$"))
                ShowFieldError(txt, lbl, "Tên chỉ được dùng chữ, số, khoảng trắng, - / & ( )");
            else if (NameExists(s, currentId))
                ShowFieldError(txt, lbl, "Tên thuốc đã tồn tại!");
            else
                ClearFieldError(txt, lbl);
        }

        private void ValidateUnitRealtime(TextBox txt, Label lbl)
        {
            string s = Normalize(txt.Text);
            if (string.IsNullOrWhiteSpace(s))
                ShowFieldError(txt, lbl, "Đơn vị không được để trống");
            else if (s.Length > 50)
                ShowFieldError(txt, lbl, "Đơn vị không quá 50 ký tự");
            else if (!_validUnits.Contains(s))
                ShowFieldError(txt, lbl, "Đơn vị không hợp lệ (ví dụ: viên, lọ, ống, ml, mg...)");
            else
                ClearFieldError(txt, lbl);
        }

        private void ValidateManufacturerRealtime(TextBox txt, Label lbl)
        {
            string s = Normalize(txt.Text);
            if (string.IsNullOrWhiteSpace(s))
                ShowFieldError(txt, lbl, "Nhà sản xuất không được để trống");
            else if (s.Length > 100)
                ShowFieldError(txt, lbl, "Nhà sản xuất không quá 100 ký tự");
            else if (!Regex.IsMatch(s, @"^[\p{L}\p{N}\s\-]+$"))
                ShowFieldError(txt, lbl, "Chỉ được dùng chữ, số, khoảng trắng và dấu gạch ngang");
            else
                ClearFieldError(txt, lbl);
        }

        private void ValidatePriceRealtime(TextBox txt, Label lbl)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
                ShowFieldError(txt, lbl, "Giá không được để trống");
            else if (!decimal.TryParse(txt.Text, out decimal p))
                ShowFieldError(txt, lbl, "Giá phải là số nguyên");
            else if (p < 0)
                ShowFieldError(txt, lbl, "Giá không được âm");
            else if (p > 999999999)
                ShowFieldError(txt, lbl, "Giá tối đa 999.999.999 VNĐ");
            else if (p == 0)
                ShowFieldError(txt, lbl, "⚠ Giá = 0 VNĐ");
            else if (p < 100)
                ShowFieldError(txt, lbl, "⚠ Giá rất thấp (< 100 VNĐ)");
            else if (p > 10000000)
                ShowFieldError(txt, lbl, "⚠ Giá rất cao (> 10 triệu VNĐ)");
            else
                ClearFieldError(txt, lbl);
        }

        private void ShowFieldError(TextBox txt, Label lbl, string message)
        {
            if (message.StartsWith("⚠"))
            {
                txt.BackColor = Color.FromArgb(255, 243, 205); // Warning - màu cam nhạt
                lbl.ForeColor = Color.Orange;
            }
            else
            {
                txt.BackColor = Color.FromArgb(255, 235, 238); // Error - màu đỏ nhạt
                lbl.ForeColor = Color.Red;
            }

            lbl.Text = message;
            lbl.Visible = true;
        }

        private void ClearFieldError(TextBox txt, Label lbl)
        {
            txt.BackColor = Color.White;
            lbl.Text = "";
            lbl.Visible = false;
        }
    }
}
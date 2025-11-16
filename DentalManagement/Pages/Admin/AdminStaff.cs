using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminStaff : UserControl
    {
        public AdminStaff()
        {
            InitializeComponent();
            LoadStaff();
        }

        private void LoadStaff()
        {
            try
            {
                string search = txtSearch.Text.Trim();
                string position = cboPosition.SelectedItem?.ToString();
                string query = @"
                    SELECT
                        s.staff_id AS [ID],
                        u.fullname AS [Họ tên],
                        u.phone AS [SĐT],
                        u.email AS [Email],
                        s.position AS [Chức vụ],
                        s.specialization AS [Chuyên môn],
                        s.salary AS [Lương],
                        u.status AS [Trạng thái]
                    FROM Staff s
                    INNER JOIN UserAccount u ON s.user_id = u.user_id
                    WHERE (u.fullname LIKE @search OR u.email LIKE @search OR u.phone LIKE @search)";

                var parameters = new[]
                {
                    new SqlParameter("@search", $"%{search}%")
                };

                if (position != "Tất cả" && !string.IsNullOrEmpty(position))
                {
                    query += " AND s.position = @position";
                    parameters = new[]
                    {
                        new SqlParameter("@search", $"%{search}%"),
                        new SqlParameter("@position", position)
                    };
                }

                query += " ORDER BY u.fullname";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvStaff.DataSource = dt;

                // Ẩn ID
                if (dgvStaff.Columns["ID"] != null)
                    dgvStaff.Columns["ID"].Visible = false;

                // Format lương
                if (dgvStaff.Columns["Lương"] != null)
                    dgvStaff.Columns["Lương"].DefaultCellStyle.Format = "N0";

                // Thêm nút Edit/Delete
                AddButtonColumn("Edit", "Sửa", 70);
                AddButtonColumn("Delete", "Xóa", 70);

                // Format trạng thái
                foreach (DataGridViewRow row in dgvStaff.Rows)
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
            if (!dgvStaff.Columns.Contains(name))
            {
                dgvStaff.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = name,
                    HeaderText = "",
                    Text = text,
                    UseColumnTextForButtonValue = true,
                    Width = width
                });
            }
        }

        private void DgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int staffId = Convert.ToInt32(dgvStaff.Rows[e.RowIndex].Cells["ID"].Value);
            string name = dgvStaff.Rows[e.RowIndex].Cells["Họ tên"].Value.ToString();

            if (e.ColumnIndex == dgvStaff.Columns["Edit"].Index)
                ShowAddEditForm(staffId);
            else if (e.ColumnIndex == dgvStaff.Columns["Delete"].Index)
                DeleteStaff(staffId, name);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
        }

        private void ShowAddEditForm(int? staffId)
        {
            using (Form form = new Form
            {
                Text = staffId.HasValue ? "Sửa nhân viên" : "Thêm nhân viên mới",
                Size = new Size(550, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            })
            {
                // Controls
                TextBox txtFullname = CreateTextBox(150, 30);
                TextBox txtPhone = CreateTextBox(150, 70);
                TextBox txtEmail = CreateTextBox(150, 110);
                TextBox txtPassword = CreateTextBox(150, 150);
                ComboBox cboPos = CreateComboBox(150, 190, new[] { "Doctor", "Staff" });
                TextBox txtSpec = CreateTextBox(150, 230);
                TextBox txtSalary = CreateTextBox(150, 270);
                DateTimePicker dtpHireDate = new DateTimePicker { Location = new Point(150, 310), Size = new Size(350, 25) };

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Họ tên:", 30, 33), txtFullname,
                    CreateLabel("SĐT:", 30, 73), txtPhone,
                    CreateLabel("Email:", 30, 113), txtEmail,
                    CreateLabel("Mật khẩu:", 30, 153), txtPassword,
                    CreateLabel("Chức vụ:", 30, 193), cboPos,
                    CreateLabel("Chuyên môn:", 30, 233), txtSpec,
                    CreateLabel("Lương:", 30, 273), txtSalary,
                    CreateLabel("Ngày vào:", 30, 313), dtpHireDate
                });

                int userId = 0;

                // Load data nếu sửa
                if (staffId.HasValue)
                {
                    string query = @"SELECT u.*, s.* FROM Staff s INNER JOIN UserAccount u ON s.user_id = u.user_id WHERE s.staff_id = @id";
                    DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", staffId.Value) });
                    if (dt.Rows.Count > 0)
                    {
                        userId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                        txtFullname.Text = dt.Rows[0]["fullname"].ToString();
                        txtPhone.Text = dt.Rows[0]["phone"].ToString();
                        txtEmail.Text = dt.Rows[0]["email"].ToString();
                        txtEmail.ReadOnly = true;
                        txtPassword.Text = dt.Rows[0]["password_hash"].ToString();
                        cboPos.SelectedItem = dt.Rows[0]["position"].ToString();
                        txtSpec.Text = dt.Rows[0]["specialization"].ToString();
                        txtSalary.Text = dt.Rows[0]["salary"].ToString();
                        if (dt.Rows[0]["hire_date"] != DBNull.Value)
                            dtpHireDate.Value = Convert.ToDateTime(dt.Rows[0]["hire_date"]);
                    }
                }

                Button btnSave = CreateButton("Lưu", 225, 360, "#007ACC");
                btnSave.Click += (s, ev) =>
                {
                    if (!Validator.IsNotEmpty(txtFullname.Text) || !Validator.IsNotEmpty(txtEmail.Text) ||
                        !Validator.IsValidEmail(txtEmail.Text) || !Validator.IsValidPhone(txtPhone.Text))
                    {
                        MessageBoxHelper.ShowValidationError("Vui lòng nhập đầy đủ và đúng định dạng!");
                        return;
                    }

                    if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary < 0)
                    {
                        MessageBoxHelper.ShowValidationError("Lương không hợp lệ!");
                        return;
                    }

                    try
                    {
                        if (staffId.HasValue)
                        {
                            // Update UserAccount
                            DatabaseHelper.ExecuteNonQuery(
                                "UPDATE UserAccount SET fullname=@name, phone=@phone, password_hash=@pass WHERE user_id=@uid",
                                new SqlParameter[] {
                                    new SqlParameter("@name", txtFullname.Text.Trim()),
                                    new SqlParameter("@phone", txtPhone.Text.Trim()),
                                    new SqlParameter("@pass", txtPassword.Text.Trim()),
                                    new SqlParameter("@uid", userId)
                                });

                            // Update Staff
                            DatabaseHelper.ExecuteNonQuery(
                                "UPDATE Staff SET position=@pos, specialization=@spec, salary=@sal, hire_date=@date WHERE staff_id=@id",
                                new SqlParameter[] {
                                    new SqlParameter("@pos", cboPos.SelectedItem.ToString()),
                                    new SqlParameter("@spec", txtSpec.Text.Trim()),
                                    new SqlParameter("@sal", salary),
                                    new SqlParameter("@date", dtpHireDate.Value),
                                    new SqlParameter("@id", staffId.Value)
                                });
                        }
                        else
                        {
                            // Insert UserAccount
                            object newUserId = DatabaseHelper.ExecuteScalar(
                                @"INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status)
                                  OUTPUT INSERTED.user_id
                                  VALUES (@name, @phone, @email, @pass, @role, 'active')",
                                new SqlParameter[] {
                                    new SqlParameter("@name", txtFullname.Text.Trim()),
                                    new SqlParameter("@phone", txtPhone.Text.Trim()),
                                    new SqlParameter("@email", txtEmail.Text.Trim()),
                                    new SqlParameter("@pass", txtPassword.Text.Trim()),
                                    new SqlParameter("@role", cboPos.SelectedItem.ToString().ToLower())
                                });

                            // Insert Staff
                            DatabaseHelper.ExecuteNonQuery(
                                "INSERT INTO Staff (user_id, position, specialization, salary, hire_date) VALUES (@uid, @pos, @spec, @sal, @date)",
                                new SqlParameter[] {
                                    new SqlParameter("@uid", newUserId),
                                    new SqlParameter("@pos", cboPos.SelectedItem.ToString()),
                                    new SqlParameter("@spec", txtSpec.Text.Trim()),
                                    new SqlParameter("@sal", salary),
                                    new SqlParameter("@date", dtpHireDate.Value)
                                });
                        }

                        MessageBoxHelper.ShowSaveSuccess();
                        Logger.LogAction(staffId.HasValue ? "UPDATE_STAFF" : "CREATE_STAFF", txtFullname.Text);
                        form.Close();
                        LoadStaff();
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

        private void DeleteStaff(int staffId, string name)
        {
            if (!MessageBoxHelper.ShowDeleteConfirm(name)) return;

            try
            {
                object userIdObj = DatabaseHelper.ExecuteScalar(
                    "SELECT user_id FROM Staff WHERE staff_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", staffId) });

                if (userIdObj != null)
                {
                    int userId = Convert.ToInt32(userIdObj);
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM Staff WHERE staff_id = @id", new SqlParameter[] { new SqlParameter("@id", staffId) });
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM UserAccount WHERE user_id = @uid", new SqlParameter[] { new SqlParameter("@uid", userId) });

                    MessageBoxHelper.ShowDeleteSuccess(name);
                    Logger.LogDelete("Staff", name);
                    LoadStaff();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi xóa: {ex.Message}");
            }
        }

        // Helper methods
        private Label CreateLabel(string text, int x, int y) => new Label { Text = text, Location = new Point(x, y), AutoSize = true };
        private TextBox CreateTextBox(int x, int y) => new TextBox { Location = new Point(x, y), Size = new Size(350, 25) };
        private ComboBox CreateComboBox(int x, int y, object[] items)
        {
            var cbo = new ComboBox { Location = new Point(x, y), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            return cbo;
        }
        private Button CreateButton(string text, int x, int y, string backColor) => new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(100, 35),
            BackColor = ColorTranslator.FromHtml(backColor),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        // Event handlers
        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadStaff();
        private void cboPosition_SelectedIndexChanged(object sender, EventArgs e) => LoadStaff();
    }
}
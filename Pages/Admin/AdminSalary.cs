using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminSalary : UserControl
    {
        public AdminSalary()
        {
            InitializeComponent();
            InitializeControls();
            LoadSalary();
        }

        private void InitializeControls()
        {
            try
            {
                // ✅ Kiểm tra controls tồn tại
                if (cboMonth == null || cboYear == null)
                {
                    MessageBoxHelper.ShowError("Lỗi khởi tạo controls!");
                    return;
                }

                // ĐIỀN DỮ LIỆU SAU InitializeComponent
                cboMonth.Items.Clear();
                for (int i = 1; i <= 12; i++)
                    cboMonth.Items.Add(i);
                cboMonth.SelectedItem = DateTime.Now.Month;

                cboYear.Items.Clear();
                for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 1; i++)
                    cboYear.Items.Add(i);
                cboYear.SelectedItem = DateTime.Now.Year;

                // GÁN EVENT
                cboMonth.SelectedIndexChanged += cboMonth_SelectedIndexChanged;
                cboYear.SelectedIndexChanged += cboYear_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khởi tạo: {ex.Message}");
            }
        }

        private void LoadSalary()
        {
            // ✅ FIX: Kiểm tra null đầy đủ
            if (cboMonth == null || cboYear == null)
            {
                MessageBoxHelper.ShowError("Lỗi: Controls chưa được khởi tạo!");
                return;
            }

            if (cboMonth.SelectedItem == null || cboYear.SelectedItem == null)
            {
                // Không hiển thị lỗi khi đang khởi tạo
                return;
            }

            try
            {
                int month = (int)cboMonth.SelectedItem;
                int year = (int)cboYear.SelectedItem;

                string query = @"
                    SELECT
                        u.fullname AS [Nhân viên],
                        st.position AS [Chức vụ],
                        ISNULL(s.base_salary, st.salary) AS [Lương cơ bản],
                        ISNULL(s.bonus, 0) AS [Thưởng],
                        ISNULL(s.deduction, 0) AS [Khấu trừ],
                        ISNULL(s.total_salary, st.salary) AS [Thực lĩnh]
                    FROM Staff st
                    INNER JOIN UserAccount u ON st.user_id = u.user_id
                    LEFT JOIN Salary s ON st.staff_id = s.staff_id 
                        AND s.month = @month 
                        AND s.year = @year
                    WHERE u.status = 'active'
                    ORDER BY u.fullname";

                SqlParameter[] parameters = {
                    new SqlParameter("@month", month),
                    new SqlParameter("@year", year)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                // ✅ FIX: Kiểm tra dữ liệu rỗng
                if (dt == null || dt.Rows.Count == 0)
                {
                    // Hiển thị thông báo không có nhân viên
                    MessageBoxHelper.ShowInfo($"Không có thông tin lương tháng {month}/{year}");

                    // Vẫn gán DataSource rỗng để hiển thị header
                    dgvSalary.DataSource = dt;
                }
                else
                {
                    dgvSalary.DataSource = dt;

                    // ✅ FIX: Kiểm tra có nhân viên nhưng chưa có bảng lương
                    bool hasSalaryData = false;
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal bonus = row["Thưởng"] != DBNull.Value ? Convert.ToDecimal(row["Thưởng"]) : 0;
                        decimal deduction = row["Khấu trừ"] != DBNull.Value ? Convert.ToDecimal(row["Khấu trừ"]) : 0;

                        if (bonus > 0 || deduction > 0)
                        {
                            hasSalaryData = true;
                            break;
                        }
                    }

                    if (!hasSalaryData)
                    {
                        MessageBoxHelper.ShowInfo($"Chưa có bảng lương chi tiết cho tháng {month}/{year}.\nHiển thị lương cơ bản từ hợp đồng.");
                    }
                }

                // ✅ FIX: Cấu hình DataGridView
                if (dgvSalary != null && dgvSalary.Columns.Count > 0)
                {
                    // Tự động điều chỉnh độ rộng cột
                    dgvSalary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Set độ rộng cho từng cột
                    if (dgvSalary.Columns["Nhân viên"] != null)
                    {
                        dgvSalary.Columns["Nhân viên"].FillWeight = 150;
                        dgvSalary.Columns["Nhân viên"].MinimumWidth = 150;
                    }

                    if (dgvSalary.Columns["Chức vụ"] != null)
                    {
                        dgvSalary.Columns["Chức vụ"].FillWeight = 80;
                        dgvSalary.Columns["Chức vụ"].MinimumWidth = 80;
                    }

                    // Format số tiền
                    foreach (var col in new[] { "Lương cơ bản", "Thưởng", "Khấu trừ", "Thực lĩnh" })
                    {
                        if (dgvSalary.Columns[col] != null)
                        {
                            dgvSalary.Columns[col].DefaultCellStyle.Format = "N0";
                            dgvSalary.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvSalary.Columns[col].FillWeight = 100;
                            dgvSalary.Columns[col].MinimumWidth = 100;
                        }
                    }

                    // ✅ FIX: Format và highlight rows
                    foreach (DataGridViewRow row in dgvSalary.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Format chức vụ
                        if (row.Cells["Chức vụ"].Value != null)
                        {
                            string position = row.Cells["Chức vụ"].Value.ToString();
                            switch (position.ToLower())
                            {
                                case "doctor":
                                    row.Cells["Chức vụ"].Value = "Bác sĩ";
                                    row.Cells["Chức vụ"].Style.ForeColor = Color.Blue;
                                    row.Cells["Chức vụ"].Style.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                                    break;
                                case "staff":
                                    row.Cells["Chức vụ"].Value = "Nhân viên";
                                    row.Cells["Chức vụ"].Style.ForeColor = Color.Green;
                                    break;
                            }
                        }

                        // Highlight khấu trừ > 0 (màu đỏ nhạt)
                        if (row.Cells["Khấu trừ"].Value != null && row.Cells["Khấu trừ"].Value != DBNull.Value)
                        {
                            decimal deduction = Convert.ToDecimal(row.Cells["Khấu trừ"].Value);
                            if (deduction > 0)
                            {
                                row.Cells["Khấu trừ"].Style.ForeColor = Color.Red;
                                row.Cells["Khấu trừ"].Style.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                            }
                        }

                        // Highlight thưởng > 0 (màu xanh lá)
                        if (row.Cells["Thưởng"].Value != null && row.Cells["Thưởng"].Value != DBNull.Value)
                        {
                            decimal bonus = Convert.ToDecimal(row.Cells["Thưởng"].Value);
                            if (bonus > 0)
                            {
                                row.Cells["Thưởng"].Style.ForeColor = Color.Green;
                                row.Cells["Thưởng"].Style.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                            }
                        }

                        // Highlight thực lĩnh cao (> 30 triệu)
                        if (row.Cells["Thực lĩnh"].Value != null && row.Cells["Thực lĩnh"].Value != DBNull.Value)
                        {
                            decimal totalSalary = Convert.ToDecimal(row.Cells["Thực lĩnh"].Value);
                            if (totalSalary > 30000000)
                            {
                                row.Cells["Thực lĩnh"].Style.ForeColor = Color.DarkGreen;
                                row.Cells["Thực lĩnh"].Style.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                            }
                            else if (totalSalary == 0)
                            {
                                row.Cells["Thực lĩnh"].Style.ForeColor = Color.Gray;
                                row.Cells["Thực lĩnh"].Style.Font = new Font(dgvSalary.Font, FontStyle.Italic);
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
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        // ✅ FIX: Thêm nút Refresh
        

        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSalary();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSalary();
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
                // Hiển thị loading (nếu có)
                if (sender is Button btn)
                {
                    btn.Enabled = false;
                    btn.Text = "Đang tải...";
                }

                LoadSalary();

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
    }
}
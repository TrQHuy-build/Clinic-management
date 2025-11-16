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
            LoadSalary();

            // ĐIỀN DỮ LIỆU SAU InitializeComponent
            for (int i = 1; i <= 12; i++) cboMonth.Items.Add(i);
            cboMonth.SelectedItem = DateTime.Now.Month;

            for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 1; i++) cboYear.Items.Add(i);
            cboYear.SelectedItem = DateTime.Now.Year;

            // GÁN EVENT
            cboMonth.SelectedIndexChanged += cboMonth_SelectedIndexChanged;
            cboYear.SelectedIndexChanged += cboYear_SelectedIndexChanged;
        }

        private void LoadSalary()
        {
            if (cboMonth.SelectedItem == null || cboYear.SelectedItem == null) return;

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
                    LEFT JOIN Salary s ON st.staff_id = s.staff_id AND s.month = @month AND s.year = @year
                    ORDER BY u.fullname";

                SqlParameter[] parameters = {
                    new SqlParameter("@month", month),
                    new SqlParameter("@year", year)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvSalary.DataSource = dt;

                foreach (var col in new[] { "Lương cơ bản", "Thưởng", "Khấu trừ", "Thực lĩnh" })
                {
                    if (dgvSalary.Columns[col] != null)
                        dgvSalary.Columns[col].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e) => LoadSalary();
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e) => LoadSalary();
    }
}
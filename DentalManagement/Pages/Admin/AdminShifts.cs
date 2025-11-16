using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminShifts : UserControl
    {
        public AdminShifts()
        {
            InitializeComponent();
            LoadShifts();
        }

        private void LoadShifts()
        {
            try
            {
                string query = @"
                    SELECT
                        s.shift_id AS [ID],
                        u.fullname AS [Nhân viên],
                        st.position AS [Chức vụ],
                        FORMAT(s.shift_date, 'dd/MM/yyyy') AS [Ngày],
                        CONVERT(VARCHAR(5), s.start_time, 108) AS [Giờ bắt đầu],
                        CONVERT(VARCHAR(5), s.end_time, 108) AS [Giờ kết thúc]
                    FROM Shift s
                    INNER JOIN Staff st ON s.staff_id = st.staff_id
                    INNER JOIN UserAccount u ON st.user_id = u.user_id
                    WHERE s.shift_date = @date
                    ORDER BY s.start_time";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@date", dtpDate.Value.Date) });
                dgvShifts.DataSource = dt;

                if (dgvShifts.Columns["ID"] != null)
                    dgvShifts.Columns["ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddShiftForm();
        }

        private void ShowAddShiftForm()
        {
            MessageBoxHelper.ShowInfo("Chức năng đang phát triển!");
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e) => LoadShifts();
    }
}
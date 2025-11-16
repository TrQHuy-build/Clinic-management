using System;
using System.Data;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminLogs : UserControl
    {
        public AdminLogs()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                string query = @"
                    SELECT TOP 500
                        u.fullname AS [Người dùng],
                        a.action AS [Hành động],
                        a.description AS [Mô tả],
                        FORMAT(a.timestamp, 'dd/MM/yyyy HH:mm:ss') AS [Thời gian]
                    FROM AuditLog a
                    LEFT JOIN UserAccount u ON a.user_id = u.user_id
                    ORDER BY a.timestamp DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLogs.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}
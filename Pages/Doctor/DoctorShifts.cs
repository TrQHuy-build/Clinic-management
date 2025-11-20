using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorShifts : UserControl
    {
        public DoctorShifts()
        {
            InitializeComponent();
            InitializeEvents();
            LoadShifts();
        }

        private void InitializeEvents()
        {
            dtpMonth.ValueChanged += (s, e) => LoadShifts();
        }

        private void LoadShifts()
        {
            if (!Auth.CurrentStaffId.HasValue) return;

            try
            {
                DateTime startDate = new DateTime(dtpMonth.Value.Year, dtpMonth.Value.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                string query = @"
                    SELECT
                        FORMAT(shift_date, 'dd/MM/yyyy') AS [Ngày],
                        DATENAME(WEEKDAY, shift_date) AS [Thứ],
                        CONVERT(VARCHAR(5), start_time, 108) AS [Giờ bắt đầu],
                        CONVERT(VARCHAR(5), end_time, 108) AS [Giờ kết thúc]
                    FROM Shift
                    WHERE staff_id = @staffId AND shift_date BETWEEN @start AND @end
                    ORDER BY shift_date, start_time";

                var parameters = new[]
                {
                    new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                    new SqlParameter("@start", startDate),
                    new SqlParameter("@end", endDate)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvShifts.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch trực: {ex.Message}");
            }
        }
    }
}
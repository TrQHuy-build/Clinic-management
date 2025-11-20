using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminReports : UserControl
    {
        public AdminReports()
        {
            InitializeComponent();
            btnRevenue.Click += (s, e) => ShowRevenueReport();
            btnService.Click += (s, e) => ShowServiceReport();
        }

        private void ShowRevenueReport()
        {
            string query = @"
                SELECT
                    MONTH(invoice_date) AS [Tháng],
                    SUM(total_amount) AS [Doanh thu]
                FROM Invoice
                WHERE YEAR(invoice_date) = YEAR(GETDATE()) AND status = 'paid'
                GROUP BY MONTH(invoice_date)
                ORDER BY MONTH(invoice_date)";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            ShowDataInForm("Báo cáo doanh thu năm " + DateTime.Now.Year, dt);
        }

        private void ShowServiceReport()
        {
            string query = @"
                SELECT TOP 10
                    s.service_name AS [Dịch vụ],
                    COUNT(*) AS [Số lần sử dụng],
                    SUM(s.price * su.quantity) AS [Tổng doanh thu]
                FROM ServiceUsage su
                INNER JOIN Service s ON su.service_id = s.service_id
                GROUP BY s.service_name
                ORDER BY COUNT(*) DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            ShowDataInForm("Dịch vụ phổ biến", dt);
        }

        private void ShowDataInForm(string title, DataTable dt)
        {
            using (Form form = new Form
            {
                Text = title,
                Size = new Size(800, 500),
                StartPosition = FormStartPosition.CenterParent
            })
            {
                DataGridView dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = dt,
                    BackgroundColor = Color.White,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };
                form.Controls.Add(dgv);
                form.ShowDialog();
            }
        }
    }
}
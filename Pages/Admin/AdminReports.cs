using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using DentalClinicManagement.Utils;

// Note: ensure LiveCharts.WinForms is referenced in project packages (already present in /packages)

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminReports : UserControl
    {
        private LiveCharts.WinForms.CartesianChart revenueChartWin;
        private LiveCharts.WinForms.PieChart serviceChartWin;
        private DataTable _revenueDt;
        private DataTable _serviceDt;

        public AdminReports()
        {
            InitializeComponent();

            // Remove button handlers - we'll show charts automatically
            btnRevenue.Visible = false;
            btnService.Visible = false;

            // Set this control to fill parent container (prevents it from influencing parent size)
            this.Dock = DockStyle.Fill;

            // Create a left panel for title and action buttons
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 340,
                Padding = new Padding(10),
                BackColor = this.BackColor
            };

            // Reposition title into leftPanel
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(300, 70);
            lblTitle.TextAlign = ContentAlignment.TopLeft;
            leftPanel.Controls.Add(lblTitle);

            // Create action buttons and stack them vertically
            var btnRefreshAll = new Button
            {
                Text = "Làm mới",
                Size = new Size(300, 40),
                BackColor = ColorTranslator.FromHtml("#007ACC"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 90)
            };

            var btnExportRevenue = new Button
            {
                Text = "Xuất doanh thu",
                Size = new Size(300, 40),
                BackColor = ColorTranslator.FromHtml("#28A745"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 140)
            };

            var btnExportServices = new Button
            {
                Text = "Xuất dịch vụ",
                Size = new Size(300, 40),
                BackColor = ColorTranslator.FromHtml("#28A745"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 190)
            };

            leftPanel.Controls.Add(btnRefreshAll);
            leftPanel.Controls.Add(btnExportRevenue);
            leftPanel.Controls.Add(btnExportServices);

            // Create containers for charts — Dock them so layout is responsive
            revenueChartWin = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 320,
                BackColor = Color.White
            };

            serviceChartWin = new LiveCharts.WinForms.PieChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Add controls: leftPanel first then charts so charts take remaining space
            this.Controls.Add(serviceChartWin);
            this.Controls.Add(revenueChartWin);
            this.Controls.Add(leftPanel);

            // Load charts on control load
            this.Load += (s, e) =>
            {
                LoadRevenueChart();
                LoadServiceChart();
            };

            btnRefreshAll.Click += (s, e) => { LoadRevenueChart(); LoadServiceChart(); };
            btnExportRevenue.Click += (s, e) =>
            {
                var dt = GetRevenueDataTable();
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"revenue_{DateTime.Now:yyyyMMdd}.csv");
                DentalClinicManagement.Utils.CsvExporter.ExportToCsv(dt, path);
                MessageBox.Show($"Đã xuất báo cáo doanh thu ra: {path}");
            };
            btnExportServices.Click += (s, e) =>
            {
                var dt = GetServiceDataTable();
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"services_{DateTime.Now:yyyyMMdd}.csv");
                DentalClinicManagement.Utils.CsvExporter.ExportToCsv(dt, path);
                MessageBox.Show($"Đã xuất báo cáo dịch vụ ra: {path}");
            };
        }

        private void LoadRevenueChart()
        {
            string query = @"
                SELECT
                    MONTH(invoice_date) AS [Month],
                    SUM(total_amount) AS [Revenue]
                FROM Invoice
                WHERE YEAR(invoice_date) = YEAR(GETDATE()) AND status = 'paid'
                GROUP BY MONTH(invoice_date)
                ORDER BY MONTH(invoice_date)";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            // store dt for export
            _revenueDt = dt;

            var months = Enumerable.Range(1, 12).Select(m => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m)).ToArray();
            var values = new ChartValues<decimal>(new decimal[12]);

            foreach (DataRow r in dt.Rows)
            {
                int m = Convert.ToInt32(r["Month"]);
                decimal rev = Convert.ToDecimal(r["Revenue"]);
                values[m - 1] = rev;
            }

            revenueChartWin.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = values
                }
            };

            revenueChartWin.AxisX.Clear();
            revenueChartWin.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Labels = months.ToList()
            });

            revenueChartWin.AxisY.Clear();
            revenueChartWin.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                LabelFormatter = value => value.ToString("N0")
            });
        }

        private void LoadServiceChart()
        {
            string query = @"
                SELECT TOP 10
                    s.service_name AS [Service],
                    COUNT(*) AS [Count],
                    SUM(s.price * su.quantity) AS [Revenue]
                FROM ServiceUsage su
                INNER JOIN Service s ON su.service_id = s.service_id
                GROUP BY s.service_name
                ORDER BY COUNT(*) DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            _serviceDt = dt;

            var pieSeries = new SeriesCollection();
            foreach (DataRow r in dt.Rows)
            {
                string name = r["Service"].ToString();
                decimal rev = Convert.ToDecimal(r["Revenue"]);
                pieSeries.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = name,
                    Values = new ChartValues<decimal> { rev },
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0:N0}", chartPoint.Y)
                });
            }

            serviceChartWin.Series = pieSeries;
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

        private DataTable GetRevenueDataTable()
        {
            if (_revenueDt != null) return _revenueDt;
            string query = @"
                SELECT
                    MONTH(invoice_date) AS [Month],
                    SUM(total_amount) AS [Revenue]
                FROM Invoice
                WHERE YEAR(invoice_date) = YEAR(GETDATE()) AND status = 'paid'
                GROUP BY MONTH(invoice_date)
                ORDER BY MONTH(invoice_date)";
            _revenueDt = DatabaseHelper.ExecuteQuery(query);
            return _revenueDt;
        }

        private DataTable GetServiceDataTable()
        {
            if (_serviceDt != null) return _serviceDt;
            string query = @"
                SELECT TOP 10
                    s.service_name AS [Service],
                    COUNT(*) AS [Count],
                    SUM(s.price * su.quantity) AS [Revenue]
                FROM ServiceUsage su
                INNER JOIN Service s ON su.service_id = s.service_id
                GROUP BY s.service_name
                ORDER BY COUNT(*) DESC";
            _serviceDt = DatabaseHelper.ExecuteQuery(query);
            return _serviceDt;
        }
    }
}
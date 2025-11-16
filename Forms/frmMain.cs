using DentalClinicManagement.Controls;
using DentalClinicManagement.Pages.Admin;
using DentalClinicManagement.Pages.Common;
using DentalClinicManagement.Pages.Doctor;
using DentalClinicManagement.Pages.Patient;
using DentalClinicManagement.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Forms
{
    public partial class frmMain : Form
    {
        private List<SidebarMenuItem> menuItems;
        private UserControl currentPage;

        public frmMain()
        {
            InitializeComponent();

            // Set user info text (không thể set trong Designer vì dùng Auth động)
            lblUserInfo.Text = $"👤 {Auth.CurrentUserName} ({Auth.CurrentUserRole?.ToUpper()})";

            // Position logout button (không thể set trong Designer vì dùng ClientSize động)
            PositionLogoutButton();
            this.Resize += (s, e) => PositionLogoutButton(); // Adjust khi resize

            menuItems = new List<SidebarMenuItem>();
            LoadMenuByRole();
            this.FormClosing += FrmMain_FormClosing;
        }

        private void PositionLogoutButton()
        {
            btnLogout.Location = new Point(this.ClientSize.Width - 140, 15);
        }

        private void LoadMenuByRole()
        {
            // Xóa menu cũ (nếu có)
            foreach (var item in menuItems)
            {
                panelSidebar.Controls.Remove(item);
            }
            menuItems.Clear();

            if (Auth.IsAdmin())
            {
                LoadAdminMenu();
                LoadPage(new AdminDashboard());
            }
            else if (Auth.IsDoctor())
            {
                LoadDoctorMenu();
                LoadPage(new DoctorDashboard());
            }
            else if (Auth.IsPatient())
            {
                LoadPatientMenu();
                LoadPage(new PatientDashboard());
            }
        }

        private void LoadAdminMenu()
        {
            AddMenuItem("📊", "Dashboard", () => LoadPage(new AdminDashboard()));
            AddMenuItem("📦", "Kho vật tư", () => LoadPage(new AdminInventory()));
            AddMenuItem("💼", "Dịch vụ", () => LoadPage(new AdminServices()));
            AddMenuItem("💊", "Thuốc", () => LoadPage(new AdminMedicines()));
            AddMenuItem("👥", "Nhân viên", () => LoadPage(new AdminStaff()));
            AddMenuItem("📅", "Lịch trực", () => LoadPage(new AdminShifts()));
            AddMenuItem("💰", "Lương", () => LoadPage(new AdminSalary()));
            AddMenuItem("🧾", "Hóa đơn", () => LoadPage(new AdminInvoices()));
            AddMenuItem("📈", "Báo cáo", () => LoadPage(new AdminReports()));
            AddMenuItem("📋", "Log hệ thống", () => LoadPage(new AdminLogs()));
            AddMenuItem("⚙️", "Cài đặt", () => LoadPage(new SettingsPage()));

            // Select first item
            if (menuItems.Count > 0)
                menuItems[0].IsSelected = true;
        }

        private void LoadDoctorMenu()
        {
            AddMenuItem("📊", "Dashboard", () => LoadPage(new DoctorDashboard()));
            AddMenuItem("📅", "Lịch hẹn của tôi", () => LoadPage(new DoctorAppointments()));
            AddMenuItem("🩺", "Khám bệnh", () => LoadPage(new DoctorExamine()));
            AddMenuItem("📁", "Hồ sơ bệnh án", () => LoadPage(new DoctorMedicalRecords()));
            AddMenuItem("👥", "Bệnh nhân của tôi", () => LoadPage(new DoctorPatients()));
            AddMenuItem("🕐", "Lịch trực", () => LoadPage(new DoctorShifts()));
            AddMenuItem("⚙️", "Cài đặt", () => LoadPage(new SettingsPage()));

            if (menuItems.Count > 0)
                menuItems[0].IsSelected = true;
        }

        private void LoadPatientMenu()
        {
            AddMenuItem("📊", "Dashboard", () => LoadPage(new PatientDashboard()));
            AddMenuItem("📅", "Đặt lịch hẹn", () => LoadPage(new PatientBookAppointment()));
            AddMenuItem("📋", "Lịch sử hẹn", () => LoadPage(new PatientAppointmentHistory()));
            AddMenuItem("🧾", "Hóa đơn", () => LoadPage(new PatientInvoices()));
            AddMenuItem("📁", "Hồ sơ sức khỏe", () => LoadPage(new PatientMedicalRecords()));
            AddMenuItem("👤", "Thông tin cá nhân", () => LoadPage(new PatientProfile()));
            AddMenuItem("⚙️", "Cài đặt", () => LoadPage(new SettingsPage()));

            if (menuItems.Count > 0)
                menuItems[0].IsSelected = true;
        }

        private void AddMenuItem(string icon, string text, Action onClick)
        {
            SidebarMenuItem item = new SidebarMenuItem
            {
                IconText = icon,
                MenuText = text
            };

            item.Click += (s, e) =>
            {
                // Unselect all
                foreach (var menuItem in menuItems)
                {
                    menuItem.IsSelected = false;
                }
                // Select current
                item.IsSelected = true;
                onClick?.Invoke();
            };

            panelSidebar.Controls.Add(item);
            item.BringToFront();
            panelLogo.BringToFront(); // Keep logo on top

            menuItems.Add(item);
        }

        private void LoadPage(UserControl page)
        {
            // Remove current page
            if (currentPage != null)
            {
                panelContent.Controls.Remove(currentPage);
                currentPage.Dispose();
            }

            // Load new page
            currentPage = page;
            currentPage.Dock = DockStyle.Fill;
            panelContent.Controls.Add(currentPage);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Auth.Logout();
                frmLogin loginForm = new frmLogin();
                loginForm.Show();
                this.Close();
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát ứng dụng?",
                "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
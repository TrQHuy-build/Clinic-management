using System;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class StaffForm : Form
    {
        public StaffForm()
        {
            InitializeComponent();
        }

        private void StaffForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Chào {CurrentUser.Instance.Fullname} (Staff)";
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            using var f = new CheckInForm();
            f.ShowDialog();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            using var f = new InventoryForm();
            f.ShowDialog();
        }

        private void btnExam_Click(object sender, EventArgs e)
        {
            using var f = new ExamForm();
            f.ShowDialog();
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            using var f = new PaymentForm();
            f.ShowDialog();
        }
    }
}

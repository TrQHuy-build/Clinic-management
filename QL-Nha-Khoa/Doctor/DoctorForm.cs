using System;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class DoctorForm : Form
    {
        public DoctorForm()
        {
            InitializeComponent();
        }

        private void DoctorForm_Load(object sender, EventArgs e)
        {

        }

        private void btnExam_Click(object sender, EventArgs e)
        {
            using var f = new DoctorExamForm();
            f.ShowDialog();
        }
    }
}

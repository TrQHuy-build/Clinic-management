using System;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class ShiftEditForm : Form
    {
        public ShiftEditForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

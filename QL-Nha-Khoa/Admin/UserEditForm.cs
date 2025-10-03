using System;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class UserEditForm : Form
    {
        private int? _userId;
        public UserEditForm(int? userId = null)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

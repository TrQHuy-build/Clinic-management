using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class Form1 : Form
    {
        private string connectionString = AppConfig.ConnectionString;
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataAdapter adt;
        private DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(connectionString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT \r\n    a.appointment_id,\r\n    ua.fullname AS patient_name,\r\n    s2.fullname AS doctor_name,\r\n    sv.service_name,\r\n    a.appointment_date,\r\n    a.status,\r\n    a.notes\r\nFROM Appointment a\r\nJOIN Patient p ON a.patient_id = p.patient_id\r\nJOIN UserAccount ua ON p.user_id = ua.user_id\r\nJOIN Staff st ON a.staff_id = st.staff_id\r\nJOIN UserAccount s2 ON st.user_id = s2.user_id\r\nLEFT JOIN Service sv ON a.service_id = sv.service_id;\r\n", con);
                adt = new SqlDataAdapter(cmd);
                adt.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

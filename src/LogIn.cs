using Project;
using System.Data;
using System.Data.SqlClient;

namespace DB_phase2_project
{
    public partial class LogIn : Form
    {
        SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString);
        public static string USER_NAME { get; set; }

        public LogIn()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand cm = new SqlCommand("SP_User_Login", conn);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add(new SqlParameter("@Username", textBox1.Text));
            cm.Parameters.Add(new SqlParameter("@Password", textBox2.Text));

            string loginStatus = string.Empty;

            using (SqlDataReader reader = cm.ExecuteReader())
            {
                if (reader.Read())
                {
                    loginStatus = reader.GetString(0);
                }
            }

            conn.Close();

            switch (loginStatus)
            {
                case "Member":
                    USER_NAME = textBox1.Text;
                    var memberForm = new MemberDashboard();
                    this.Hide();
                    memberForm.Show();
                    break;
                case "Trainer":
                    USER_NAME = textBox1.Text;
                    var trainerForm = new TrainerDashboard();
                    this.Hide();
                    trainerForm.Show();
                    break;
                case "Admin":
                    USER_NAME = textBox1.Text;
                    var adminForm = new AdminDashboard();
                    this.Hide();
                    adminForm.Show();
                    break;
                case "GymOwner":
                    USER_NAME = textBox1.Text;
                    var gymOwnerForm = new GymOwnerDashboard();
                    this.Hide();
                    gymOwnerForm.Show();
                    break;
                case "Incorrect Password":
                    MessageBox.Show("The password you entered is incorrect.");
                    break;
                case "Username does not exist":
                    MessageBox.Show("The username you entered does not exist.");
                    break;
                default:
                    MessageBox.Show("An unexpected error occurred.");
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var signup = new Form2();
            signup.Show();
            //this.Close();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            var Gsignup = new GApplication();
            Gsignup.Show();
            //this.Close();


        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var Tsignup = new Form4();
            Tsignup.Show();
            //this.Close();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexTrainer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var login = new LogIn();
            login.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand("SELECT Gym_ID, Gym_name FROM Gym", conn))
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox1.DataSource = dt;
                    comboBox1.ValueMember = "Gym_ID";
                    comboBox1.DisplayMember = "Gym_name";
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Membership_ID, Membership_name FROM Membership", conn))
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox2.DataSource = dt;
                    comboBox2.ValueMember = "Membership_ID";
                    comboBox2.DisplayMember = "Membership_name";
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string fname = textBox1.Text;
            string lname = textBox2.Text;
            string un = textBox4.Text;  // Check with the previous entries 
            string email = textBox3.Text;
            string pass = textBox5.Text;
            string exp = "";
            string spec = "";
            string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string sques = textBox7.Text;
            string gymname = comboBox1.Text;
            string membershipname = comboBox2.Text;
            string role = "Member";
            int gId, mId;

            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string storedProcedure = "SP_Return_Gym_ID";
                using (SqlCommand cm_gym = new SqlCommand(storedProcedure, conn))
                {
                    cm_gym.CommandType = CommandType.StoredProcedure;
                    cm_gym.Parameters.AddWithValue("@Gym_name", gymname);

                    Object GYM_ID = cm_gym.ExecuteScalar();
                    gId = Convert.ToInt32(GYM_ID);
                    // Use GYM_ID as needed
                }
                storedProcedure = "SP_Return_Membership_ID";
                using (SqlCommand cm_mem = new SqlCommand(storedProcedure, conn))
                {
                    cm_mem.CommandType = CommandType.StoredProcedure;
                    cm_mem.Parameters.AddWithValue("@Membership_name", membershipname);

                    Object MEM_ID = cm_mem.ExecuteScalar();
                    mId = Convert.ToInt32(MEM_ID);

                    // Use GYM_ID as needed
                }

                // Hash password before storing
                string hashedPassword = PasswordHelper.HashPassword(pass);

                storedProcedure = "SP_Add_New_User";
                using (SqlCommand cm = new SqlCommand(storedProcedure, conn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@First_name", fname);
                    cm.Parameters.AddWithValue("@Last_name", lname);
                    cm.Parameters.AddWithValue("@DOB", dob);
                    cm.Parameters.AddWithValue("@Username", un);
                    cm.Parameters.AddWithValue("@Password", hashedPassword);
                    cm.Parameters.AddWithValue("@email", email);
                    cm.Parameters.AddWithValue("@Role", role);
                    cm.Parameters.AddWithValue("@Experience", exp);
                    cm.Parameters.AddWithValue("@Specialization", spec);
                    cm.Parameters.AddWithValue("@Gym_ID", gId);
                    cm.Parameters.AddWithValue("@Membership_ID", mId);

                    cm.ExecuteNonQuery();
                    cm.Dispose();
                }
                var memDashboard = new LogIn();
                memDashboard.Show();
                this.Close();
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

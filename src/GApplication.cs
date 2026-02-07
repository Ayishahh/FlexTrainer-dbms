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
    public partial class GApplication : Form
    {
        public GApplication()
        {
            InitializeComponent();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = new LogIn();
            login.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fname = textBox1.Text;
            string lname = textBox2.Text;
            string un = textBox9.Text;  // Check with the previous entries 
            string email = textBox3.Text;
            string pass = textBox5.Text;
            string exp = textBox4.Text;
            string spec = textBox8.Text;
            string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string sques = textBox7.Text;
            string gymname = textBox11.Text;
            string membershipname = "";
            string role = "GymOwner";
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
                    cm.Parameters.AddWithValue("@Membership_ID", "");

                    cm.ExecuteNonQuery();
                    cm.Dispose();
                }
                MessageBox.Show("Registered Successfully");
                var memDashboard = new LogIn();
                memDashboard.Show();
                this.Close();
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}

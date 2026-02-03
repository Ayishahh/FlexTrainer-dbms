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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DB_phase2_project
{
    public partial class Form4 : Form
    {
        SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString);
        public Form4()
        {
            InitializeComponent();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = new LogIn();
            login.Show();
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            conn.Open();
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
                conn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();

            string fname = textBox1.Text;
            string lname = textBox2.Text;
            string un = textBox3.Text;
            string email = textBox9.Text;
            string pass = textBox5.Text;
            string exp = textBox4.Text;
            string spec = textBox8.Text;
            string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string sques = textBox7.Text;
            string gymname = comboBox1.Text;
            string role = "Trainer";
            int gId;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname) || 
                string.IsNullOrWhiteSpace(un) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Please fill in all required fields.");
                conn.Close();
                return;
            }

            try
            {
                // Get Gym ID
                string storedProcedure = "SP_Return_Gym_ID";
                using (SqlCommand cm_gym = new SqlCommand(storedProcedure, conn))
                {
                    cm_gym.CommandType = CommandType.StoredProcedure;
                    cm_gym.Parameters.AddWithValue("@Gym_name", gymname);
                    Object GYM_ID = cm_gym.ExecuteScalar();
                    gId = Convert.ToInt32(GYM_ID);
                }

                // Register the trainer using Add_New_User stored procedure
                storedProcedure = "SP_Add_New_User";
                using (SqlCommand cm = new SqlCommand(storedProcedure, conn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@First_name", fname);
                    cm.Parameters.AddWithValue("@Last_name", lname);
                    cm.Parameters.AddWithValue("@DOB", dob);
                    cm.Parameters.AddWithValue("@Username", un);
                    cm.Parameters.AddWithValue("@Password", pass);
                    cm.Parameters.AddWithValue("@email", email);
                    cm.Parameters.AddWithValue("@Role", role);
                    cm.Parameters.AddWithValue("@Experience", exp);
                    cm.Parameters.AddWithValue("@Specialization", spec);
                    cm.Parameters.AddWithValue("@Gym_ID", gId);
                    cm.Parameters.AddWithValue("@Membership_ID", DBNull.Value);
                    cm.ExecuteNonQuery();
                }

                // Get the newly created trainer's ID
                string getIdQuery = "SELECT User_ID FROM Users WHERE Username = @Username";
                int trainerId;
                using (SqlCommand cmd = new SqlCommand(getIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", un);
                    trainerId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Create a trainer request for gym owner approval
                string insertRequest = "INSERT INTO Trainer_Requests (Trainer_ID, Gym_ID, Request_status) VALUES (@TrainerID, @GymID, 'Pending')";
                using (SqlCommand cmdReq = new SqlCommand(insertRequest, conn))
                {
                    cmdReq.Parameters.AddWithValue("@TrainerID", trainerId);
                    cmdReq.Parameters.AddWithValue("@GymID", gId);
                    cmdReq.ExecuteNonQuery();
                }

                MessageBox.Show("Trainer registration submitted successfully!\nAwaiting gym owner approval.");
                conn.Close();

                var login = new LogIn();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message);
                conn.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '*';
        }
    }
}

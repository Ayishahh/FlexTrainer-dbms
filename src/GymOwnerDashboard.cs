using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FlexTrainer
{
    public partial class GymOwnerDashboard : Form
    {
        string connection_string = DatabaseHelper.ConnectionString;
        public GymOwnerDashboard()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Gym_ID, Gym_name FROM Gym WHERE GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)", conn)) // previous trainers
                {
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);

                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox5.DataSource = dt;
                    comboBox5.ValueMember = "Gym_ID";
                    comboBox5.DisplayMember = "Gym_name";

                    Gym_name.DataSource = dt;
                    Gym_name.ValueMember = "Gym_ID";
                    Gym_name.DisplayMember = "Gym_name";

                    comboBox7.DataSource = dt;
                    comboBox7.ValueMember = "Gym_ID";
                    comboBox7.DisplayMember = "Gym_name";

                    comboBox9.DataSource = dt;
                    comboBox9.ValueMember = "Gym_ID";
                    comboBox9.DisplayMember = "Gym_name";

                    gym.DataSource = dt;
                    gym.ValueMember = "Gym_ID";
                    gym.DisplayMember = "Gym_name";
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Member_ID FROM Member", conn)) // current trainers
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox1.DataSource = dt;
                    comboBox1.ValueMember = "Member_ID";
                    comboBox1.DisplayMember = "Member_ID";
                }

                string query = "Select* from Users where username = @Username";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    textBox18.Text = textBox25.Text = LogIn.USER_NAME;
                    textBox19.Text = reader1["User_ID"].ToString();
                    textBox24.Text = reader1["email"].ToString();
                    textBox22.Text = reader1["DOB"].ToString();
                    textBox20.Text = reader1["First_name"].ToString();
                    textBox26.Text = reader1["Last_name"].ToString();
                }
                reader1.Close();

                string query1 = "SELECT Gym_name FROM Gym WHERE GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                SqlCommand cm2 = new SqlCommand(query1, conn);
                cm2.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                SqlDataReader reader2 = cm2.ExecuteReader();
                if (reader2.Read())
                {
                    textBox21.Text = reader2["Gym_name"].ToString();
                }
                reader2.Close();
                conn.Close();
            }
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select* from Users where User_ID = @UserID";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@UserID", Convert.ToInt32(comboBox2.Text));
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label73.Text = reader1["Username"].ToString();
                    label72.Text = reader1["email"].ToString();
                    label71.Text = reader1["First_name"].ToString();
                    label76.Text = reader1["Last_name"].ToString();
                }
                reader1.Close();

                query = "Select* from Membership where Membership_ID = (Select Membership_ID from Member where Member_ID = @MemberID)";
                cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@MemberID", Convert.ToInt32(comboBox2.Text));
                SqlDataReader reader2 = cm1.ExecuteReader();
                if (reader2.Read())
                {
                    label70.Text = reader2["Membership_name"].ToString();
                    label75.Text = reader2["Membership_duration"].ToString();
                    label74.Text = reader2["Membership_charges"].ToString();
                }
                reader2.Close();
                conn.Close();
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Trainer_ID FROM Works_For WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)";
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    cm.Parameters.AddWithValue("@GymName", Gym_name.Text);

                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox6.DataSource = dt;
                    comboBox6.ValueMember = "Trainer_ID";
                    comboBox6.DisplayMember = "Trainer_ID";
                }
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            var edit_prof = new GymOwnerEditProfile();
            this.Close();
            edit_prof.Show();
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void label57_Click(object sender, EventArgs e)
        {

        }

        private void label58_Click(object sender, EventArgs e)
        {
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select* from Users where User_ID = @UserID";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@UserID", Convert.ToInt32(comboBox6.Text));
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label60.Text = reader1["Username"].ToString();
                    label61.Text = reader1["email"].ToString();
                    label62.Text = reader1["First_name"].ToString();
                    label69.Text = reader1["Last_name"].ToString();
                    label64.Text = reader1["DOB"].ToString();
                }
                reader1.Close();

                query = "Select* from Trainer where Trainer_ID = @TrainerID";
                cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@TrainerID", Convert.ToInt32(comboBox6.Text));
                SqlDataReader reader2 = cm1.ExecuteReader();
                if (reader2.Read())
                {
                    label63.Text = reader2["Experience"].ToString();
                    label67.Text = reader2["Speciality"].ToString();
                }
                reader2.Close();
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) // add trainer
        {
            string pass = "0927irksldijr3";
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();

                string query = "select Gym_ID from gym where Gym_name = @GymName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GymName", gym.Text);
                string gymid = cmd.ExecuteScalar().ToString();
                cmd.Dispose();


                string storedProcedure = "SP_Add_New_User";
                using (SqlCommand cm = new SqlCommand(storedProcedure, conn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@First_name", First_name.Text);
                    cm.Parameters.AddWithValue("@Last_name", Last_name.Text);
                    cm.Parameters.AddWithValue("@DOB", dob.Value.ToString("yyyy-MM-dd"));
                    cm.Parameters.AddWithValue("@Username", username.Text);
                    cm.Parameters.AddWithValue("@Password", pass);
                    cm.Parameters.AddWithValue("@email", email.Text);
                    cm.Parameters.AddWithValue("@Role", "Trainer");
                    cm.Parameters.AddWithValue("@Experience", experience.Text);
                    cm.Parameters.AddWithValue("@Specialization", speciality.Text);
                    cm.Parameters.AddWithValue("@Gym_ID", gymid);
                    cm.Parameters.AddWithValue("@Membership_ID", "");

                    cm.ExecuteNonQuery();
                    cm.Dispose();

                }
                conn.Close();
            }
            MessageBox.Show("Trainer added to gym");
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Member_ID FROM Member WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)";
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    cm.Parameters.AddWithValue("@GymName", comboBox5.Text);

                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox2.DataSource = dt;
                    comboBox2.ValueMember = "Member_ID";
                    comboBox2.DisplayMember = "Member_ID";

                }
                conn.Close();
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Request_ID FROM Trainer_Requests WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName) AND Request_status = 'Pending'", conn)) // previous trainers
                {
                    cmd.Parameters.AddWithValue("@GymName", comboBox7.Text);

                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox8.DataSource = dt;
                    comboBox8.ValueMember = "Request_ID";
                    comboBox8.DisplayMember = "Request_ID";
                }
                conn.Close();
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Users.First_name, Users.Last_name, Users.email, Users.Username, Users.DOB, Trainer.Experience, Trainer.Speciality FROM Users JOIN Trainer ON Users.User_ID = Trainer.Trainer_ID WHERE Trainer.Trainer_ID = (SELECT Trainer_ID FROM Trainer_Requests WHERE Request_ID = @RequestID)";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@RequestID", Convert.ToInt32(comboBox8.Text));
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    textBox12.Text = reader1["Username"].ToString();
                    textBox10.Text = reader1["email"].ToString();
                    textBox14.Text = reader1["First_name"].ToString();
                    textBox13.Text = reader1["Last_name"].ToString();
                    textBox15.Text = reader1["DOB"].ToString();
                    textBox8.Text = reader1["Experience"].ToString();
                    textBox11.Text = reader1["Speciality"].ToString();
                }
                reader1.Close();
                conn.Close();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e) // accept trainer request
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                SqlCommand cm;
                string query = "UPDATE Trainer_Requests SET Request_status = 'Approved' WHERE Request_ID = @RequestID";
                cm = new SqlCommand(query, conn);
                cm.Parameters.AddWithValue("@RequestID", Convert.ToInt32(comboBox8.Text));
                cm.ExecuteNonQuery();

                string q1 = "SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName";
                SqlCommand cmd = new SqlCommand(q1, conn);
                cmd.Parameters.AddWithValue("@GymName", comboBox7.Text);
                string gymid = cmd.ExecuteScalar().ToString();
                cmd.Dispose();

                string q2 = "SELECT User_ID FROM Users WHERE Username = @Username";
                SqlCommand c2 = new SqlCommand(q2, conn);
                c2.Parameters.AddWithValue("@Username", textBox12.Text);
                string tid = c2.ExecuteScalar().ToString();
                c2.Dispose();

                query = "INSERT INTO Works_for VALUES (@GymID, @TrainerID)";
                SqlCommand c3 = new SqlCommand(query, conn);
                c3.Parameters.AddWithValue("@GymID", Convert.ToInt32(gymid));
                c3.Parameters.AddWithValue("@TrainerID", Convert.ToInt32(tid));
                c3.ExecuteNonQuery();
                c3.Dispose();

                MessageBox.Show("Trainer Request Approved! \n    -- Status updated --  ");
                cm.Dispose();
                conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e) // reject trainer request
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                SqlCommand cm;
                string query = "Update Trainer_Requests Set Request_status = 'Rejected' where Request_ID = @RequestID";
                cm = new SqlCommand(query, conn);
                cm.Parameters.AddWithValue("@RequestID", Convert.ToInt32(comboBox8.Text));
                cm.ExecuteNonQuery();
                MessageBox.Show("Trainer Request Rejected! \n    -- Statud updated -- ");
                cm.Dispose();
                conn.Close();
            }
        }

        private void label69_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnViewReports_Click(object sender, EventArgs e)
        {
            var reportsForm = new GymOwnerReports();
            reportsForm.ShowDialog();
        }
    }
}

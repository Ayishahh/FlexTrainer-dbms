using FlexTrainer;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Project
{
    public partial class AdminDashboard : Form
    {
        string connection_string = DatabaseHelper.ConnectionString;
        public AdminDashboard()
        {
            InitializeComponent();
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string Calories = "SELECT Diet_name FROM Diet_Plan WHERE Diet_ID IN(SELECT Diet_ID FROM Diet_Meal WHERE Meal_ID IN (SELECT Meal_ID FROM Meals WHERE Meal_name LIKE '%Breakfast%' AND(Protein + Carbohydrates + Fats) < 500))";
                SqlDataAdapter adapter1 = new SqlDataAdapter(Calories, conn);
                DataTable dt1 = new DataTable();
                adapter1.Fill(dt1);
                dataGridView3.DataSource = dt1;
                dataGridView3.ReadOnly = true;
                dataGridView3.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView3.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView3.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView3.RowHeadersVisible = false;

                string Carbs = "SELECT Diet_name FROM Diet_Plan WHERE Diet_ID IN (SELECT Diet_ID FROM Diet_Meal WHERE Meal_ID IN (SELECT Meal_ID FROM Meals WHERE Carbohydrates < 300))";
                SqlDataAdapter adapter2 = new SqlDataAdapter(Carbs, conn);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);
                dataGridView4.DataSource = dt2;
                dataGridView4.ReadOnly = true;
                dataGridView4.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView4.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView4.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView4.RowHeadersVisible = false;

                string Machine = "SELECT Plan_name FROM Workout_Plan WHERE Plan_ID NOT IN (SELECT Plan_ID FROM Workout_Exercise WHERE Exercise_ID IN (SELECT Exercise_ID FROM Machine WHERE Machine_name = 'Treadmill'))";
                SqlDataAdapter adapter3 = new SqlDataAdapter(Machine, conn);
                DataTable dt3 = new DataTable();
                adapter3.Fill(dt3);
                dataGridView2.DataSource = dt3;
                dataGridView2.ReadOnly = true;
                dataGridView2.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView2.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView2.RowHeadersVisible = false;

                string Trainer = "SELECT Users.First_name, Users.Last_name, Users.email, Users.DOB, Users.Username, Users.Role, Gym.Gym_name, Membership.Membership_name, Membership.Membership_duration, Membership.Membership_charges FROM Users JOIN Member ON Users.User_ID = Member.Member_ID JOIN Gym ON Member.Gym_ID = Gym.Gym_ID JOIN Membership ON Member.Membership_ID = Membership.Membership_ID WHERE Gym.Gym_name = 'Gym1' AND Member.Member_ID IN (SELECT Member_ID FROM Trains WHERE Trainer_ID = 8)";
                SqlDataAdapter adapter4 = new SqlDataAdapter(Trainer, conn);
                DataTable dt4 = new DataTable();
                adapter4.Fill(dt4);
                dataGridView5.DataSource = dt4;
                dataGridView5.ReadOnly = true;
                dataGridView5.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView5.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView5.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView5.RowHeadersVisible = false;

                string q1 = "SELECT Gym.Gym_name, COUNT(Member.Member_ID) AS Total_Members, SUM(Membership.Membership_charges) AS Total_Revenue FROM Member JOIN Membership ON Member.Membership_ID = Membership.Membership_ID JOIN Member_Joins_Gym MJG on Member.Member_ID = MJG.Member_ID JOIN Gym ON Member.Gym_ID = Gym.Gym_ID GROUP BY Gym.Gym_name";
                SqlDataAdapter adapter5 = new SqlDataAdapter(q1, conn);
                DataTable dt5 = new DataTable();
                adapter5.Fill(dt5);
                dataGridView6.DataSource = dt5;
                dataGridView6.ReadOnly = true;
                dataGridView6.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView6.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView6.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView6.RowHeadersVisible = false;

                string q2 = " SELECT Gym.Gym_name, COUNT(Member.Member_ID) AS Total_Members, COUNT(Member_Joins_Gym.Leave_date) AS Members_Leaving, (COUNT(Member.Member_ID) - COUNT(Member_Joins_Gym.Leave_date)) AS Members_Renewing FROM Member JOIN Member_Joins_Gym ON Member.Member_ID = Member_Joins_Gym.Member_ID JOIN Gym ON Member.Gym_ID = Gym.Gym_ID GROUP BY Gym.Gym_name";
                SqlDataAdapter adapter6 = new SqlDataAdapter(q2, conn);
                DataTable dt6 = new DataTable();
                adapter6.Fill(dt6);
                dataGridView7.DataSource = dt6;
                dataGridView7.ReadOnly = true;
                dataGridView7.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView7.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView7.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView7.RowHeadersVisible = false;

                string q3 = "SELECT CONCAT(Users.First_name, ' ', Users.Last_name) AS Trainer_Name, COUNT(Member.Member_ID) AS Total_Clients, AVG(Feedback.Rating) AS Average_Rating, COUNT(Appointment.Appointment_ID) AS Total_Sessions FROM Trainer JOIN Users ON Trainer.Trainer_ID = Users.User_ID JOIN Appointment ON Trainer.Trainer_ID = Appointment.Trainer_ID JOIN Feedback ON Appointment.Member_ID = Feedback.Member_ID AND Appointment.Trainer_ID = Feedback.Trainer_ID JOIN Member ON Appointment.Member_ID = Member.Member_ID GROUP BY CONCAT(Users.First_name, ' ', Users.Last_name)";
                SqlDataAdapter adapter7 = new SqlDataAdapter(q3, conn);
                DataTable dt7 = new DataTable();
                adapter7.Fill(dt7);
                dataGridView8.DataSource = dt7;
                dataGridView8.ReadOnly = true;
                dataGridView8.DefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView8.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView8.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                dataGridView8.RowHeadersVisible = false;
                conn.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void GymNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GymName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open Excel Import form
            var excelImport = new ExcelImport();
            excelImport.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void GymDetailR_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AdminTab_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RequestRemove_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
        }

        private void GymDetailA_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer5_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void FinancialReport_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_2(object sender, EventArgs e)
        {

        }

        private void splitContainer6_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer6_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click_3(object sender, EventArgs e)
        {

        }

        private void splitContainer6_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void AdminDashboard_Load(object sender, EventArgs e)
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

                    cmd.Dispose();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Request_ID FROM Gym_Request WHERE Request_status = 'Pending'", conn)) // pending gym requests
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox6.DataSource = dt;
                    comboBox6.ValueMember = "Request_ID";
                    comboBox6.DisplayMember = "Request_ID";
                    cmd.Dispose();

                }

                // Assuming 'conn' is an open SqlConnection object
                using (SqlCommand cmd = new SqlCommand("SELECT Gym_ID, Gym_name FROM Gym", conn))

                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox5.DataSource = dt;
                    comboBox5.ValueMember = "Gym_ID";
                    comboBox5.DisplayMember = "Gym_name";

                    comboBox7.DataSource = dt;
                    comboBox7.ValueMember = "Gym_ID";
                    comboBox7.DisplayMember = "Gym_name";

                    comboBox14.DataSource = dt;
                    comboBox14.ValueMember = "Gym_ID";
                    comboBox14.DisplayMember = "Gym_name";
                    cmd.Dispose();
                    conn.Close();

                }


                using (SqlCommand cmd = new SqlCommand("SELECT Meal_ID, Meal_name FROM Meals ", conn)) // previous trainers
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox13.DataSource = dt;
                    comboBox13.ValueMember = "Meal_ID";
                    comboBox13.DisplayMember = "Meal_name";

                    comboBox12.DataSource = dt;
                    comboBox12.ValueMember = "Meal_ID";
                    comboBox12.DisplayMember = "Meal_name";
                    cmd.Dispose();

                }

                using (SqlCommand cmd = new SqlCommand("SELECT Meal_ID, Meal_name FROM Meals ", conn)) // previous trainers
                {
                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox10.DataSource = dt;
                    comboBox10.ValueMember = "Meal_ID";
                    comboBox10.DisplayMember = "Meal_name";
                    cmd.Dispose();

                }
                conn.Close();
            }

            // Load pending gym requests
            LoadPendingGymRequests();
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select* from Gym where Gym_name = @GymName";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@GymName", comboBox18.Text);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label3.Text = comboBox18.Text;
                    label5.Text = reader1["Gym_location"].ToString();
                    label4.Text = reader1["GymOwner_ID"].ToString();
                    cm1.Dispose();
                }
                reader1.Close();

                using (SqlCommand cmd = new SqlCommand("SELECT Member_ID FROM Member WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)", conn)) // previous trainers
                {
                    cmd.Parameters.AddWithValue("@GymName", comboBox18.Text);

                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox2.DataSource = dt;
                    comboBox2.ValueMember = "Member_ID";
                    comboBox2.DisplayMember = "Member_ID";
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Trainer_ID FROM Works_for WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)", conn)) // previous trainers
                {
                    cmd.Parameters.AddWithValue("@GymName", comboBox18.Text);

                    // Initialize SqlDataAdapter and DataTable
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox1.DataSource = dt;
                    comboBox1.ValueMember = "Trainer_ID";
                    comboBox1.DisplayMember = "Trainer_ID";
                }
                conn.Close();
            }
        }

        private void button3_Click_1(object sender, EventArgs e) // remove gym
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string storedProcedure = "SP_Delete_Gym";
                using (SqlCommand cm_mem = new SqlCommand(storedProcedure, conn))
                {
                    cm_mem.CommandType = CommandType.StoredProcedure;
                    cm_mem.Parameters.AddWithValue("@Gym_name", comboBox18.SelectedItem.ToString());

                    cm_mem.ExecuteScalar();
                    cm_mem.Dispose();
                }
                conn.Close();
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string q1 = "SELECT First_name FROM users WHERE User_ID = (SELECT GymOwner_ID FROM Gym WHERE Gym_name = @GymName)";
                SqlCommand c1 = new SqlCommand(q1, conn);
                c1.Parameters.AddWithValue("@GymName", comboBox18.Text);
                string name = (string)c1.ExecuteScalar();

                using (conn)
                {
                    string query = "SELECT * FROM Gym WHERE Gym_name = @GymName";
                    SqlCommand cm1 = new SqlCommand(query, conn);
                    cm1.Parameters.AddWithValue("@GymName", comboBox18.SelectedItem.ToString());
                    SqlDataReader reader1 = cm1.ExecuteReader();
                    if (reader1.Read())
                    {
                        label9.Text = reader1["Gym_location"].ToString();
                        label7.Text = reader1["Gym_location"].ToString();
                        label8.Text = reader1["GymOwner_ID"].ToString() + " --> " + name;
                        cm1.Dispose();
                    }
                    reader1.Close();
                }
                conn.Close();
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox6.SelectedValue == null)
                {
                    MessageBox.Show("Please select a gym request to approve.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();

                    // Get Admin_ID from current logged-in user
                    int adminId = 0;
                    string getAdminIdQuery = "SELECT User_ID FROM Users WHERE Username = @Username AND Role = 'Admin'";
                    using (SqlCommand getAdminCmd = new SqlCommand(getAdminIdQuery, conn))
                    {
                        getAdminCmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                        object result = getAdminCmd.ExecuteScalar();
                        if (result != null)
                        {
                            adminId = Convert.ToInt32(result);
                        }
                    }

                    // Call stored procedure to approve gym request
                    string storedProcedure = "SP_Approve_Gym_Request";
                    using (SqlCommand cm_mem = new SqlCommand(storedProcedure, conn))
                    {
                        cm_mem.CommandType = CommandType.StoredProcedure;
                        cm_mem.Parameters.AddWithValue("@Request_ID", Convert.ToInt32(comboBox6.SelectedValue));

                        if (adminId > 0)
                        {
                            cm_mem.Parameters.AddWithValue("@Admin_ID", adminId);
                        }
                        else
                        {
                            cm_mem.Parameters.AddWithValue("@Admin_ID", DBNull.Value);
                        }

                        cm_mem.ExecuteNonQuery();
                    }

                    MessageBox.Show("Gym registration request approved successfully. Gym has been created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the pending requests list
                    LoadPendingGymRequests();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving gym request: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox6.SelectedValue == null)
                {
                    MessageBox.Show("Please select a gym request to reject.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Are you sure you want to reject this gym registration request?", "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    string q1 = "UPDATE Gym_Request SET Request_status = 'Rejected' WHERE Request_ID = @RequestID";
                    SqlCommand c1 = new SqlCommand(q1, conn);
                    c1.Parameters.AddWithValue("@RequestID", Convert.ToInt32(comboBox6.SelectedValue));
                    c1.ExecuteNonQuery();
                    MessageBox.Show("Gym registration request rejected successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the pending requests list
                    LoadPendingGymRequests();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error rejecting gym request: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Trainer_ID FROM Works_For WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)";
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    cm.Parameters.AddWithValue("@GymName", comboBox7.Text);

                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox4.DataSource = dt;
                    comboBox4.ValueMember = "Trainer_ID";
                    comboBox4.DisplayMember = "Trainer_ID";

                }
                conn.Close();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select* from Users where User_ID = @UserID";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@UserID", Convert.ToInt32(comboBox4.Text));
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label60.Text = reader1["Username"].ToString();
                    label61.Text = reader1["email"].ToString();
                    label62.Text = reader1["First_name"].ToString();
                    label69.Text = reader1["Last_name"].ToString();
                    label64.Text = reader1["DOB"].ToString();
                    cm1.Dispose();
                }
                reader1.Close();

                query = "Select* from Trainer where Trainer_ID = @TrainerID";
                cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@TrainerID", Convert.ToInt32(comboBox4.Text));
                SqlDataReader reader2 = cm1.ExecuteReader();
                if (reader2.Read())
                {
                    label63.Text = reader2["Experience"].ToString();
                    label67.Text = reader2["Speciality"].ToString();
                    cm1.Dispose();
                }
                reader2.Close();
                conn.Close();
            }
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
                    comboBox3.DataSource = dt;
                    comboBox3.ValueMember = "Member_ID";
                    comboBox3.DisplayMember = "Member_ID";

                }
                conn.Close();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Username, email, First_name, Last_name FROM Users WHERE User_ID = @UserID";
                SqlCommand cm1 = new SqlCommand(query, conn);

                cm1.Parameters.AddWithValue("@UserID", comboBox3.SelectedValue);
                SqlDataReader reader1 = cm1.ExecuteReader();


                if (reader1.Read())
                {
                    label73.Text = reader1["Username"].ToString();
                    label72.Text = reader1["email"].ToString();
                    label71.Text = reader1["First_name"].ToString();
                    label76.Text = reader1["Last_name"].ToString();
                    cm1.Dispose();
                }
                reader1.Close();

                query = "SELECT Membership_name, Membership_duration, Membership_charges FROM Membership JOIN Member M on Membership.Membership_ID = M.Membership_ID WHERE M.Member_ID = @UserID";
                cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@UserID", comboBox3.SelectedValue);

                SqlDataReader reader2 = cm1.ExecuteReader();
                if (reader2.Read())
                {
                    label70.Text = reader2["Membership_name"].ToString();
                    label75.Text = reader2["Membership_duration"].ToString();
                    label74.Text = reader2["Membership_charges"].ToString();
                    cm1.Dispose();
                }
                reader2.Close();
                conn.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e) // remove trainer
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string storedProcedure = "SP_Remove_Trainer";
                using (SqlCommand cm_mem = new SqlCommand(storedProcedure, conn))
                {
                    cm_mem.CommandType = CommandType.StoredProcedure;
                    cm_mem.Parameters.AddWithValue("@Trainer_ID", comboBox4.Text);

                    cm_mem.ExecuteScalar();
                    cm_mem.Dispose();
                }
                conn.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e) // remove member
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string storedProcedure = "SP_Remove_Member";
                using (SqlCommand cm_mem = new SqlCommand(storedProcedure, conn))
                {
                    cm_mem.CommandType = CommandType.StoredProcedure;
                    cm_mem.Parameters.AddWithValue("@Member_ID", comboBox3.Text);

                    cm_mem.ExecuteScalar();
                    cm_mem.Dispose();
                }
                conn.Close();
            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage21_Click(object sender, EventArgs e)
        {

        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Trainer_ID FROM Works_For WHERE Gym_ID = (SELECT Gym_ID FROM Gym WHERE Gym_name = @GymName)";
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    cm.Parameters.AddWithValue("@GymName", comboBox14.Text);

                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox11.DataSource = dt;
                    comboBox11.ValueMember = "Trainer_ID";
                    comboBox11.DisplayMember = "Trainer_ID";

                }
                conn.Close();
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_2(object sender, EventArgs e)
        {

        }

        private void LoadPendingGymRequests()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    string query = @"SELECT Request_ID, Request_date, Gym_name, Gym_location,
                                    Request_status, GymOwner_ID
                                    FROM Gym_Request
                                    WHERE Request_status = 'Pending'
                                    ORDER BY Request_date DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Update comboBox6 with pending requests
                    if (comboBox6 != null)
                    {
                        comboBox6.DataSource = dt.Copy();
                        comboBox6.ValueMember = "Request_ID";
                        comboBox6.DisplayMember = "Request_ID";
                    }

                    // Update DataGridView if it exists
                    // Assuming there's a DataGridView for gym requests (dataGridView1 or similar)
                    // If the DataGridView doesn't exist, this will need to be added to the form designer
                    if (this.Controls.Find("dataGridView1", true).Length > 0)
                    {
                        DataGridView dgv = (DataGridView)this.Controls.Find("dataGridView1", true)[0];
                        dgv.DataSource = dt;
                        dgv.ReadOnly = true;
                        dgv.DefaultCellStyle.Font = new Font("Arial", 8);
                        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                        dgv.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                        dgv.RowHeadersVisible = false;
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending gym requests: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewReports_Click(object sender, EventArgs e)
        {
            var reportsForm = new AdminReports();
            reportsForm.ShowDialog();
        }
    }
}
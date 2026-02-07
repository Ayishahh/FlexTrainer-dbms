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

namespace FlexTrainer
{
    public partial class Form4 : Form
    {
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
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
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
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error loading form: {sqlEx.Message}\n\nPlease ensure the database is running.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading registration form: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate all inputs
                string fname = textBox1.Text.Trim();
                string lname = textBox2.Text.Trim();
                string un = textBox3.Text.Trim();
                string email = textBox9.Text.Trim();
                string pass = textBox5.Text;
                string exp = textBox4.Text.Trim();
                string spec = textBox8.Text.Trim();
                string sques = textBox7.Text.Trim();
                string gymname = comboBox1.Text;

                // Validation checks
                if (!ValidationHelper.IsValidName(fname))
                {
                    MessageBox.Show("Please enter a valid first name (at least 2 characters).",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidName(lname))
                {
                    MessageBox.Show("Please enter a valid last name (at least 2 characters).",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidUsername(un))
                {
                    MessageBox.Show("Please enter a valid username (3-50 alphanumeric characters or underscore).",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidEmail(email))
                {
                    MessageBox.Show("Please enter a valid email address.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox9.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidPassword(pass))
                {
                    MessageBox.Show("Password must be at least 8 characters long.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox5.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidAge(dateTimePicker1.Value))
                {
                    MessageBox.Show("You must be at least 13 years old to register.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dateTimePicker1.Focus();
                    return;
                }

                // Validate experience (must be a positive integer or empty)
                int experienceYears;
                if (!string.IsNullOrWhiteSpace(exp))
                {
                    if (!ValidationHelper.IsPositiveInteger(exp, out experienceYears))
                    {
                        MessageBox.Show("Please enter a valid number of years of experience (positive integer).",
                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox4.Focus();
                        return;
                    }
                }

                if (!ValidationHelper.IsValidName(spec))
                {
                    MessageBox.Show("Please enter your specialization (at least 2 characters).",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox8.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(sques))
                {
                    MessageBox.Show("Please enter a security question answer.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox7.Focus();
                    return;
                }

                if (comboBox1.SelectedValue == null)
                {
                    MessageBox.Show("Please select a gym.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox1.Focus();
                    return;
                }

                string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string role = "Trainer";
                int gId;

                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Get Gym ID
                    string storedProcedure = "SP_Return_Gym_ID";
                    using (SqlCommand cm_gym = new SqlCommand(storedProcedure, conn))
                    {
                        cm_gym.CommandType = CommandType.StoredProcedure;
                        cm_gym.Parameters.AddWithValue("@Gym_name", gymname);

                        Object GYM_ID = cm_gym.ExecuteScalar();
                        if (GYM_ID == null)
                        {
                            MessageBox.Show("Selected gym not found. Please try again.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        gId = Convert.ToInt32(GYM_ID);
                    }

                    // Hash password before storing
                    string hashedPassword = PasswordHelper.HashPassword(pass);

                    // Register the trainer using Add_New_User stored procedure
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

                    MessageBox.Show("Trainer registration submitted successfully!\nAwaiting gym owner approval.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var login = new LogIn();
                    login.Show();
                    this.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    MessageBox.Show("This username or email is already registered. Please use a different one.",
                        "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Database error during registration: {sqlEx.Message}\n\nPlease contact support.",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during registration: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

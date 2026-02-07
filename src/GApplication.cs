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
            try
            {
                // Validate all inputs
                string fname = textBox1.Text.Trim();
                string lname = textBox2.Text.Trim();
                string un = textBox9.Text.Trim();
                string email = textBox3.Text.Trim();
                string pass = textBox5.Text;
                string exp = textBox4.Text.Trim();
                string spec = textBox8.Text.Trim();
                string sques = textBox7.Text.Trim();
                string gymname = textBox11.Text.Trim();

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
                    textBox9.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidEmail(email))
                {
                    MessageBox.Show("Please enter a valid email address.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Focus();
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

                // Validate experience (optional but must be valid if provided)
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

                if (string.IsNullOrWhiteSpace(spec))
                {
                    MessageBox.Show("Please enter your specialization/background (at least 2 characters).",
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

                if (string.IsNullOrWhiteSpace(gymname))
                {
                    MessageBox.Show("Please enter the gym name you want to register.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox11.Focus();
                    return;
                }

                string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string role = "GymOwner";
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
                            MessageBox.Show("Selected gym not found. Please verify the gym name.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        gId = Convert.ToInt32(GYM_ID);
                    }

                    // Hash password before storing
                    string hashedPassword = PasswordHelper.HashPassword(pass);

                    // Register gym owner
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

                    MessageBox.Show("Gym Owner registration successful! You can now log in.",
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

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FlexTrainer
{
    public partial class GymOwnerEditProfile : Form
    {
        private int userId;

        public GymOwnerEditProfile()
        {
            InitializeComponent();
            this.Load += GymOwnerEditProfile_Load;
        }

        private void GymOwnerEditProfile_Load(object sender, EventArgs e)
        {
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Get user data including Gym Owner's gym
                    string query = @"SELECT u.User_ID, u.Username, u.First_name, u.Last_name, u.email,
                                    u.Password, u.DOB, g.Gym_name
                                    FROM Users u
                                    LEFT JOIN Gym g ON u.User_ID = g.GymOwner_ID
                                    WHERE u.Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                                textBox1.Text = reader.GetString(1); // Username
                                textBox6.Text = reader.GetString(2) + " " + reader.GetString(3); // Full Name
                                textBox5.Text = reader.GetString(4); // Email
                                textBox2.Text = ""; // Password (leave empty for security)
                                dateTimePicker1.Value = reader.GetDateTime(6); // DOB
                                textBox4.Text = reader.IsDBNull(7) ? "" : reader.GetString(7); // Gym name

                                // Make read-only fields non-editable
                                textBox1.ReadOnly = true; // Username cannot be changed
                                textBox4.ReadOnly = true; // Gym name (read-only)
                            }
                            else
                            {
                                MessageBox.Show("Unable to load user profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate email format (basic)
            if (!textBox5.Text.Contains("@") || !textBox5.Text.Contains("."))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate that name has at least first and last name
            string[] nameParts = textBox6.Text.Trim().Split(' ');
            if (nameParts.Length < 2)
            {
                MessageBox.Show("Please enter both first and last name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Parse name into first and last name
                    string[] nameParts = textBox6.Text.Trim().Split(new[] { ' ' }, 2);
                    string firstName = nameParts[0];
                    string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                    // Call the stored procedure to update user
                    using (SqlCommand cmd = new SqlCommand("SP_Update_Existing_User", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@First_name", firstName);
                        cmd.Parameters.AddWithValue("@Last_name", lastName);
                        cmd.Parameters.AddWithValue("@DOB", dateTimePicker1.Value.Date);

                        // Only update password if it was changed
                        if (!string.IsNullOrWhiteSpace(textBox2.Text))
                        {
                            cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(textBox2.Text));
                        }
                        else
                        {
                            // Get current password
                            string getCurrentPassword = "SELECT Password FROM Users WHERE User_ID = @UserID";
                            using (SqlCommand pwdCmd = new SqlCommand(getCurrentPassword, conn))
                            {
                                pwdCmd.Parameters.AddWithValue("@UserID", userId);
                                string currentPassword = (string)pwdCmd.ExecuteScalar();
                                cmd.Parameters.AddWithValue("@Password", currentPassword);
                            }
                        }

                        cmd.Parameters.AddWithValue("@email", textBox5.Text);
                        cmd.Parameters.AddWithValue("@Experience", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Specialization", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Membership_ID", DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var gymDash = new GymOwnerDashboard();
                    this.Close();
                    gymDash.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

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
    public partial class M_editprofile : Form
    {
        private int userId;

        public M_editprofile()
        {
            InitializeComponent();
            this.Load += M_editprofile_Load;
        }

        private void M_editprofile_Load(object sender, EventArgs e)
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

                    // Get user data including Member-specific information
                    string query = @"SELECT u.User_ID, u.Username, u.email, u.Password, u.DOB,
                                    g.Gym_name, m.Membership_ID,
                                    ms.Membership_name, ms.Membership_duration, ms.Membership_charges
                                    FROM Users u
                                    JOIN Member m ON u.User_ID = m.Member_ID
                                    JOIN Gym g ON m.Gym_ID = g.Gym_ID
                                    JOIN Membership ms ON m.Membership_ID = ms.Membership_ID
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
                                textBox5.Text = reader.GetString(2); // Email
                                textBox4.Text = ""; // Password (leave empty for security)
                                textBox3.Text = reader.GetDateTime(4).ToString("yyyy-MM-dd"); // DOB
                                textBox2.Text = reader.GetString(5); // Gym name
                                textBox6.Text = reader.GetString(7); // Membership name
                                textBox8.Text = reader.GetInt32(8).ToString(); // Membership duration
                                textBox7.Text = reader.GetDecimal(9).ToString(); // Membership charges

                                // Make read-only fields non-editable
                                textBox1.ReadOnly = true; // Username cannot be changed
                                textBox2.ReadOnly = true; // Gym name (read-only)
                                textBox6.ReadOnly = true; // Membership name (read-only)
                                textBox7.ReadOnly = true; // Membership charges (read-only)
                                textBox8.ReadOnly = true; // Membership duration (read-only)
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
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Date of Birth is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate DOB format
            if (!DateTime.TryParse(textBox3.Text, out DateTime dob))
            {
                MessageBox.Show("Invalid Date of Birth format. Please use YYYY-MM-DD.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate email format (basic)
            if (!textBox5.Text.Contains("@") || !textBox5.Text.Contains("."))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
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

                    // Get current data to populate unchanged fields
                    string getCurrentQuery = "SELECT First_name, Last_name, Membership_ID FROM Users u JOIN Member m ON u.User_ID = m.Member_ID WHERE u.User_ID = @UserID";
                    string firstName = "";
                    string lastName = "";
                    int membershipId = 0;

                    using (SqlCommand getCmd = new SqlCommand(getCurrentQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = getCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                firstName = reader.GetString(0);
                                lastName = reader.GetString(1);
                                membershipId = reader.GetInt32(2);
                            }
                        }
                    }

                    // Call the stored procedure to update user
                    using (SqlCommand cmd = new SqlCommand("SP_Update_Existing_User", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@First_name", firstName);
                        cmd.Parameters.AddWithValue("@Last_name", lastName);
                        cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(textBox3.Text));

                        // Only update password if it was changed
                        if (!string.IsNullOrWhiteSpace(textBox4.Text))
                        {
                            cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(textBox4.Text));
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
                        cmd.Parameters.AddWithValue("@Membership_ID", membershipId);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var memberDash = new MemberDashboard();
                    this.Close();
                    memberDash.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

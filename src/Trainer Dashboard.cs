using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DB_phase2_project
{
    public partial class TrainerDashboard : Form
    {
        public TrainerDashboard()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var edit_prof = new T_editprofile();
            this.Hide();
            edit_prof.FormClosed += (s, args) => this.Show();
            edit_prof.Show();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            try
            {
                LoadFormData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFormData()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                // Load meal types
                string query = "SELECT DISTINCT(Meal_type) FROM Meals";
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox4.DataSource = dt;
                    comboBox4.ValueMember = "Meal_type";
                    comboBox4.DisplayMember = "Meal_type";
                }

                // Load allergens
                string query1 = "SELECT Allergen_ID, Allergen_names FROM Allergen";
                using (SqlCommand cm = new SqlCommand(query1, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox5.DataSource = dt;
                    comboBox5.ValueMember = "Allergen_ID";
                    comboBox5.DisplayMember = "Allergen_names";
                }

                // Load user's workout plans
                string query2 = "SELECT Plan_ID, Plan_name FROM Workout_Plan WHERE Creator_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox7.DataSource = dt;
                    comboBox7.ValueMember = "Plan_ID";
                    comboBox7.DisplayMember = "Plan_name";
                }

                // Load workout plan goals
                query2 = "SELECT DISTINCT(Goal) FROM Workout_Plan";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox8.DataSource = dt;
                    comboBox8.ValueMember = "Goal";
                    comboBox8.DisplayMember = "Goal";
                }

                // Load diet plan goals
                query2 = "SELECT DISTINCT(Diet_Goal) FROM Diet_Plan";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox9.DataSource = dt;
                    comboBox9.ValueMember = "Diet_Goal";
                    comboBox9.DisplayMember = "Diet_Goal";
                }

                // Load user's diet plans
                query2 = "SELECT Diet_ID, Diet_name FROM Diet_Plan WHERE Creator_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    comboBox12.DataSource = dt;
                    comboBox12.ValueMember = "Diet_ID";
                    comboBox12.DisplayMember = "Diet_name";
                }

                // Load gyms where trainer works
                query2 = "SELECT Gym.Gym_ID, Gym_name FROM Gym JOIN Works_For ON Gym.Gym_ID = Works_For.Gym_ID WHERE Trainer_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox18.DataSource = dt;
                    comboBox18.ValueMember = "Gym_ID";
                    comboBox18.DisplayMember = "Gym_name";

                    // Clone for other combobox to avoid shared binding issues
                    DataTable dt2 = dt.Copy();
                    comboBox1.DataSource = dt2;
                    comboBox1.ValueMember = "Gym_ID";
                    comboBox1.DisplayMember = "Gym_name";
                }
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.SelectedValue == null || string.IsNullOrEmpty(comboBox7.Text))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Plan_name, Goal, Level, Charges FROM Workout_plan WHERE Plan_name = @PlanName";
                    using (SqlCommand cm1 = new SqlCommand(query, conn))
                    {
                        cm1.Parameters.AddWithValue("@PlanName", comboBox7.Text);
                        using (SqlDataReader reader1 = cm1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                label59.Text = reader1["Plan_name"].ToString();
                                label58.Text = reader1["Goal"].ToString();
                                label57.Text = reader1["Level"].ToString();
                                label56.Text = reader1["Charges"].ToString();
                            }
                        }
                    }

                    query = "SELECT COUNT(Exercise_ID) FROM Workout_Exercise WHERE Plan_ID = (SELECT Plan_ID FROM Workout_Plan WHERE Plan_name = @PlanName)";
                    using (SqlCommand cm2 = new SqlCommand(query, conn))
                    {
                        cm2.Parameters.AddWithValue("@PlanName", label59.Text);
                        var result = cm2.ExecuteScalar();
                        label60.Text = result?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading workout plan details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please enter a plan name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please enter a level.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Please enter charges.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus();
                return;
            }

            if (comboBox8.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a goal.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox8.Focus();
                return;
            }

            if (!int.TryParse(textBox5.Text, out int level) || level < 1 || level > 10)
            {
                MessageBox.Show("Level must be a number between 1 and 10.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }

            if (!decimal.TryParse(textBox6.Text, out decimal charges) || charges < 0)
            {
                MessageBox.Show("Charges must be a valid positive amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT User_ID FROM Users WHERE Username = @Username";
                    int userId;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                        userId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string insertQuery = "INSERT INTO Workout_Plan(Plan_name, Goal, Level, Charges, Creator_ID) VALUES (@PlanName, @Goal, @Level, @Charges, @CreatorID)";
                    using (SqlCommand cm = new SqlCommand(insertQuery, conn))
                    {
                        cm.Parameters.AddWithValue("@PlanName", textBox4.Text.Trim());
                        cm.Parameters.AddWithValue("@Goal", comboBox8.Text);
                        cm.Parameters.AddWithValue("@Level", level);
                        cm.Parameters.AddWithValue("@Charges", charges);
                        cm.Parameters.AddWithValue("@CreatorID", userId);
                        cm.ExecuteNonQuery();
                    }

                    MessageBox.Show("Workout plan created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear form fields
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    
                    // Refresh the workout plans combobox
                    LoadFormData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating workout plan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please enter the workout plan name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }

            if (comboBox3.SelectedIndex < 0 || string.IsNullOrEmpty(comboBox3.Text))
            {
                MessageBox.Show("Please select an exercise.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox3.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox7.Text) || string.IsNullOrWhiteSpace(textBox8.Text) || string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Please fill in Sets, Reps, and Rest fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox7.Text, out int sets) || sets < 1)
            {
                MessageBox.Show("Sets must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox7.Focus();
                return;
            }

            if (!int.TryParse(textBox8.Text, out int reps) || reps < 1)
            {
                MessageBox.Show("Reps must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox8.Focus();
                return;
            }

            if (!int.TryParse(textBox9.Text, out int rest) || rest < 0)
            {
                MessageBox.Show("Rest must be a valid number (seconds).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox9.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                try
                {
                    string name = comboBox3.Text;

                    int exerciseId;
                    using (SqlCommand cm = new SqlCommand("SELECT Exercise_ID FROM Exercise WHERE Exercise_name = @Name", conn))
                    {
                        cm.Parameters.AddWithValue("@Name", name);
                        var result = cm.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Exercise not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        exerciseId = Convert.ToInt32(result);
                    }

                    int planId;
                    using (SqlCommand cm = new SqlCommand("SELECT Plan_ID FROM Workout_Plan WHERE Plan_name = @PlanName", conn))
                    {
                        cm.Parameters.AddWithValue("@PlanName", textBox4.Text.Trim());
                        var result = cm.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Workout plan not found. Please create the plan first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        planId = Convert.ToInt32(result);
                    }

                    using (SqlCommand cm2 = new SqlCommand("SP_Make_Workout_Exercises", conn))
                    {
                        cm2.CommandType = CommandType.StoredProcedure;
                        cm2.Parameters.AddWithValue("@Exercise_ID", exerciseId);
                        cm2.Parameters.AddWithValue("@Sets", sets);
                        cm2.Parameters.AddWithValue("@Reps", reps);
                        cm2.Parameters.AddWithValue("@Rest", rest);
                        cm2.Parameters.AddWithValue("@Plan_ID", planId);
                        cm2.ExecuteNonQuery();
                    }

                    MessageBox.Show("Exercise added successfully to the workout plan!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear only exercise-specific fields
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox9.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding exercise: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox12.SelectedValue == null || string.IsNullOrEmpty(comboBox12.Text))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Diet_name, Diet_type, Diet_goal FROM Diet_plan WHERE Diet_name = @DietName";
                    using (SqlCommand cm1 = new SqlCommand(query, conn))
                    {
                        cm1.Parameters.AddWithValue("@DietName", comboBox12.Text);
                        using (SqlDataReader reader1 = cm1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                label80.Text = reader1["Diet_name"].ToString();
                                label78.Text = reader1["Diet_type"].ToString();
                                label79.Text = reader1["Diet_goal"].ToString();
                            }
                        }
                    }

                    query = "SELECT COUNT(Meal_ID) FROM Diet_Meal WHERE Diet_ID = (SELECT Diet_ID FROM Diet_Plan WHERE Diet_name = @DietName)";
                    using (SqlCommand cm2 = new SqlCommand(query, conn))
                    {
                        cm2.Parameters.AddWithValue("@DietName", label80.Text);
                        var result = cm2.ExecuteScalar();
                        label68.Text = result?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading diet plan details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter a diet plan name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            if (comboBox4.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a diet type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox4.Focus();
                return;
            }

            if (comboBox9.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a diet goal.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox9.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                try
                {
                    int userId;
                    using (SqlCommand cmd = new SqlCommand("SELECT User_ID FROM Users WHERE Username = @Username", conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                        userId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    using (SqlCommand cm = new SqlCommand("INSERT INTO Diet_Plan(Diet_name, Diet_type, Diet_goal, Creator_ID) VALUES (@DietName, @DietType, @DietGoal, @CreatorID)", conn))
                    {
                        cm.Parameters.AddWithValue("@DietName", textBox3.Text.Trim());
                        cm.Parameters.AddWithValue("@DietType", comboBox4.Text);
                        cm.Parameters.AddWithValue("@DietGoal", comboBox9.Text);
                        cm.Parameters.AddWithValue("@CreatorID", userId);
                        cm.ExecuteNonQuery();
                    }

                    MessageBox.Show("Diet Plan created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh diet plans combobox
                    LoadFormData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating diet plan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter a diet plan name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(label48.Text) || label48.Text == "label48")
            {
                MessageBox.Show("Please select a meal first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                try
                {
                    int dietId, mealId;

                    using (SqlCommand c1 = new SqlCommand("SELECT Diet_ID FROM Diet_Plan WHERE Diet_name = @DietName", conn))
                    {
                        c1.Parameters.AddWithValue("@DietName", textBox3.Text.Trim());
                        var result = c1.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Diet plan not found. Please create the diet plan first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        dietId = Convert.ToInt32(result);
                    }

                    using (SqlCommand c2 = new SqlCommand("SELECT Meal_ID FROM Meals WHERE Meal_name = @MealName", conn))
                    {
                        c2.Parameters.AddWithValue("@MealName", label48.Text);
                        var result = c2.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Meal not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        mealId = Convert.ToInt32(result);
                    }

                    using (SqlCommand cm = new SqlCommand("INSERT INTO Diet_meal VALUES (@DietID, @MealID)", conn))
                    {
                        cm.Parameters.AddWithValue("@DietID", dietId);
                        cm.Parameters.AddWithValue("@MealID", mealId);
                        cm.ExecuteNonQuery();
                    }

                    MessageBox.Show("Meal added successfully to the diet plan!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding meal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedValue == null || comboBox4.SelectedValue == null)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    string query2 = "SELECT Meals.Meal_ID, Meal_name FROM Meals JOIN Meal_Contains ON Meals.Meal_ID = Meal_Contains.Meal_ID WHERE Allergen_ID != (SELECT Allergen_ID FROM Allergen WHERE Allergen_names = @AllergenName) AND Meal_type = @MealType";

                    using (SqlCommand cm = new SqlCommand(query2, conn))
                    {
                        cm.Parameters.AddWithValue("@AllergenName", comboBox5.Text);
                        cm.Parameters.AddWithValue("@MealType", comboBox4.Text);

                        SqlDataAdapter sda = new SqlDataAdapter(cm);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        comboBox2.DataSource = dt;
                        comboBox2.ValueMember = "Meal_ID";
                        comboBox2.DisplayMember = "Meal_name";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading meals: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null || string.IsNullOrEmpty(comboBox2.Text))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Meal_name, Fibre, Protein, Carbohydrates, Fats FROM Meals WHERE Meal_name = @MealName";
                    using (SqlCommand cm1 = new SqlCommand(query, conn))
                    {
                        cm1.Parameters.AddWithValue("@MealName", comboBox2.Text);
                        using (SqlDataReader reader1 = cm1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                label48.Text = reader1["Meal_name"].ToString();
                                label51.Text = reader1["Fibre"].ToString();
                                label50.Text = reader1["Protein"].ToString();
                                label53.Text = reader1["Carbohydrates"].ToString();
                                label52.Text = reader1["Fats"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading meal details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox18.SelectedValue == null)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query2 = "SELECT Member.Member_ID, Users.First_name, Users.Last_name FROM Member JOIN Users ON Member.Member_ID = Users.User_ID JOIN Trains ON Member.Member_ID = Trains.Member_ID WHERE Trainer_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                    using (SqlCommand cm = new SqlCommand(query2, conn))
                    {
                        cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                        SqlDataAdapter sda = new SqlDataAdapter(cm);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        comboBox17.DataSource = dt;
                        comboBox17.ValueMember = "Member_ID";
                        comboBox17.DisplayMember = "Member_ID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox19.SelectedValue == null || string.IsNullOrEmpty(comboBox19.Text))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Appointment_Date, Appointment_start_time, Appointment_end_time FROM Appointment WHERE Appointment_ID = @AppointmentID";
                    using (SqlCommand cm1 = new SqlCommand(query, conn))
                    {
                        cm1.Parameters.AddWithValue("@AppointmentID", comboBox19.Text);
                        using (SqlDataReader reader1 = cm1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                label90.Text = reader1["Appointment_Date"].ToString();
                                label89.Text = reader1["Appointment_start_time"].ToString();
                                label88.Text = reader1["Appointment_end_time"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointment details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string query2 = "SELECT Appointment_ID FROM Appointment WHERE Trainer_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    using (SqlCommand cm = new SqlCommand(query2, conn))
                    {
                        cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                        SqlDataAdapter sda = new SqlDataAdapter(cm);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        comboBox19.DataSource = dt;
                        comboBox19.ValueMember = "Appointment_ID";
                        comboBox19.DisplayMember = "Appointment_ID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.SelectedValue == null || string.IsNullOrEmpty(comboBox6.Text))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Appointment_start_time, Appointment_end_time FROM Appointment WHERE Appointment_ID = @AppointmentID";
                    using (SqlCommand cm1 = new SqlCommand(query, conn))
                    {
                        cm1.Parameters.AddWithValue("@AppointmentID", comboBox6.Text);
                        using (SqlDataReader reader1 = cm1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                label100.Text = reader1["Appointment_start_time"].ToString();
                                label101.Text = reader1["Appointment_end_time"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointment details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Empty event handlers - kept for designer compatibility but can be removed from designer if not needed
        private void label42_Click(object sender, EventArgs e) { }
        private void tabPage5_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void splitContainer3_Panel2_Paint(object sender, PaintEventArgs e) { }
        private void button4_Click(object sender, EventArgs e) { }
        private void button3_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label103_Click(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
    }
}


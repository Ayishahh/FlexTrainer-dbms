using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace DB_phase2_project
{
    public partial class MemberDashboard : Form
    {
        string connection_string = DatabaseHelper.ConnectionString;
        public MemberDashboard()
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

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT DISTINCT(Meal_type) FROM Meals"; // select meal type
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox4.DataSource = dt;
                    comboBox4.ValueMember = "Meal_type";
                    comboBox4.DisplayMember = "Meal_type";

                }

                string query1 = "SELECT Allergen_ID, Allergen_names FROM Allergen"; // select allergen type
                using (SqlCommand cm = new SqlCommand(query1, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox5.DataSource = dt;
                    comboBox5.ValueMember = "Allergen_ID";
                    comboBox5.DisplayMember = "Allergen_names";

                }

                string query2 = "Select Plan_ID, Plan_name from Workout_Plan Where Creator_ID = (Select User_ID from Users Where Username = '" + LogIn.USER_NAME + "')"; // display plan names created by this user
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox7.DataSource = dt;
                    comboBox7.ValueMember = "Plan_ID";
                    comboBox7.DisplayMember = "Plan_name";

                }

                query2 = "SELECT DISTINCT(Goal) FROM Workout_Plan"; // display distinct plan goals
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox8.DataSource = dt;
                    comboBox8.ValueMember = "Goal";
                    comboBox8.DisplayMember = "Goal";

                }

                query2 = "SELECT DISTINCT(Diet_Goal) FROM Diet_Plan"; // display distinct diet plan goals
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox9.DataSource = dt;
                    comboBox9.ValueMember = "Diet_Goal";
                    comboBox9.DisplayMember = "Diet_Goal";

                }

                query2 = "SELECT Diet_ID, Diet_name FROM Diet_Plan WHERE Creator_ID = (SELECT User_ID FROM Users WHERE Username = '" + LogIn.USER_NAME + "')"; // display plan names created by this user
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox12.DataSource = dt;
                    comboBox12.ValueMember = "Diet_ID";
                    comboBox12.DisplayMember = "Diet_name";

                }

                query2 = "SELECT Trainer_ID FROM Works_For WHERE Gym_ID = (SELECT Gym_ID FROM Member WHERE Member_ID = (SELECT User_ID FROM Users WHERE Username = '" + LogIn.USER_NAME + "'))"; // display plan names created by this user
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox13.DataSource = dt;
                    comboBox13.ValueMember = "Trainer_ID";
                    comboBox13.DisplayMember = "Trainer_ID";

                    comboBox6.DataSource = dt;
                    comboBox6.ValueMember = "Trainer_ID";
                    comboBox6.DisplayMember = "Trainer_ID";

                    comboBox1.DataSource = dt;
                    comboBox1.ValueMember = "Trainer_ID";
                    comboBox1.DisplayMember = "Trainer_ID";

                }

                string main_q = "SELECT User_ID, First_name, Last_name , email, Password, DOB, Role, Gym.gym_name, Membership.Membership_name, Membership.Membership_duration, Membership.Membership_charges FROM Users JOIN Member ON Users.User_ID = Member.Member_ID JOIN Gym ON Member.Gym_ID = Gym.Gym_ID JOIN Membership ON Member.Membership_ID = Membership.Membership_ID WHERE Username = '" + LogIn.USER_NAME + "'"; // displaying info of meal
                SqlCommand cm1 = new SqlCommand(main_q, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label87.Text = reader1["First_name"].ToString();
                    label88.Text = reader1["Last_name"].ToString();
                    label89.Text = reader1["User_ID"].ToString();
                    label90.Text = LogIn.USER_NAME;
                    label91.Text = reader1["Email"].ToString();
                    label92.Text = reader1["Password"].ToString();
                    label93.Text = reader1["DOB"].ToString();
                    label94.Text = reader1["gym_name"].ToString();
                    label95.Text = reader1["Membership_name"].ToString();
                    label96.Text = reader1["Membership_duration"].ToString();
                    label97.Text = reader1["Membership_charges"].ToString();

                }
                reader1.Close();
                conn.Close();
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e) // Display all the info of a workout plan
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select Plan_name, Goal, Level, Charges from Workout_plan Where Plan_name = (Select Plan_name from Workout_Plan where Creator_ID = (Select User_ID from Users where Username = @un))";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@un", LogIn.USER_NAME);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label59.Text = reader1["Plan_name"].ToString();
                    label58.Text = reader1["Goal"].ToString();
                    label57.Text = reader1["Level"].ToString();
                    label56.Text = reader1["Charges"].ToString();
                }
                reader1.Close();
                //            }
                query = "SELECT COUNT(Exercise_ID) FROM Workout_Exercise WHERE Plan_ID = (SELECT Plan_ID FROM Workout_Plan WHERE Plan_name = '" + label59.Text.ToString() + "') ";
                SqlCommand cm2 = new SqlCommand(query, conn);
                string no_ex = cm2.ExecuteScalar().ToString();
                cm2.Dispose();
                cm1.Dispose();
                label60.Text = no_ex;
                conn.Close();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) // Display all available meals with the specified type and without the specified allergens
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                //            using (SqlConnection conn = new SqlConnection("Data Source=172.16.60.63;Initial Catalog=DB_PROJECT;Persist Security Info=True;User ID=sa;Password=Ali222126"))
                //            {

                string query = "SELECT Meal_ID, Meal_name FROM Meals WHERE Meal_type = '" + comboBox4.Text + "'"; // Display all available meals with the specified type and without the specified allergens
                using (SqlCommand cm = new SqlCommand(query, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Bind data to the ComboBox
                    comboBox2.DataSource = dt;
                    comboBox2.ValueMember = "Meal_ID";
                    comboBox2.DisplayMember = "Meal_name";

                }
                conn.Close();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) // displaying all info of a meal
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                //            using (SqlConnection conn = new SqlConnection("Data Source=172.16.60.63;Initial Catalog=DB_PROJECT;Persist Security Info=True;User ID=sa;Password=Ali222126"))
                //            {
                string query = "Select Meal_name, Protein, Fibre, Fats, Carbohydrates from Meals Where Meal_name = '" + comboBox2.Text.ToString() + "'"; // displaying info of meal
                SqlCommand cm1 = new SqlCommand(query, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label48.Text = reader1["Meal_name"].ToString();
                    label50.Text = reader1["Protein"].ToString();
                    label51.Text = reader1["Fibre"].ToString();
                    label52.Text = reader1["Fats"].ToString();
                    label53.Text = reader1["Carbohydrates"].ToString();
                }
                reader1.Close();
                conn.Close();
                //
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string name = label48.Text;
                string prot = label50.Text;
                string fibre = label51.Text;
                string fats = label52.Text;
                string carbs = label53.Text;

                string q1 = "Select Diet_ID from Diet_Plan where Diet_name = '" + textBox3.Text + "'";
                string q2 = "Select Meal_ID from Meals where Meal_name = '" + name + "'";

                SqlCommand c1 = new SqlCommand(q1, conn);
                SqlCommand c2 = new SqlCommand(q2, conn);

                string query = "Insert into Diet_meal values(" + c1.ExecuteScalar().ToString() + ", " + c2.ExecuteScalar().ToString() + ")";
                SqlCommand cm = new SqlCommand(query, conn);
                cm.ExecuteNonQuery();
                c1.Dispose();
                c2.Dispose();
                cm.Dispose();

                MessageBox.Show("Meal added successfully to plan !");
                conn.Close();
            }
        }

        private void splitContainer4_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query2 = "SELECT Exercise.Exercise_ID, Exercise.Exercise_Name FROM Exercise";
                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox3.DataSource = dt;
                    comboBox3.ValueMember = "Exercise_ID";
                    comboBox3.DisplayMember = "Exercise_name";
                }
                conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e) // add exercise to workout plan
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string name = comboBox3.Text.ToString();
                string sets = textBox7.Text.ToString();
                string reps = textBox8.Text.ToString();
                string rest = textBox9.Text.ToString();
                string id = textBox4.Text.ToString();

                string query = "Select Exercise_ID from Exercise where Exercise_name = '" + name + "'";
                SqlCommand cm;
                cm = new SqlCommand(query, conn);
                Object var = cm.ExecuteScalar();

                query = "Select Plan_ID from Workout_Plan where Plan_name = '" + textBox4.Text.ToString() + "'";
                cm = new SqlCommand(query, conn);
                Object var1 = cm.ExecuteScalar();

                cm.Dispose();
                string storedProcedure = "SP_Make_Workout_Exercises";
                using (SqlCommand cm2 = new SqlCommand(storedProcedure, conn))
                {
                    cm2.CommandType = CommandType.StoredProcedure;
                    cm2.Parameters.AddWithValue("@Exercise_ID", Convert.ToInt32(var));
                    cm2.Parameters.AddWithValue("@Sets", Convert.ToInt32(sets));
                    cm2.Parameters.AddWithValue("@Reps", Convert.ToInt32(reps));
                    cm2.Parameters.AddWithValue("@Rest", Convert.ToInt32(rest));
                    cm2.Parameters.AddWithValue("@Plan_ID", Convert.ToInt32(var1));
                    cm2.ExecuteNonQuery();
                    cm2.Dispose();
                }

                textBox7.Text = textBox8.Text = textBox9.Text = "";
                MessageBox.Show("Workout exercise added successfull to plan !");
                conn.Close();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "SELECT Exercise_name, Focus_muscle FROM Exercise WHERE Exercise_name = @ExerciseName";
                SqlCommand cm1 = new SqlCommand(query, conn);
                cm1.Parameters.AddWithValue("@ExerciseName", comboBox3.Text);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label67.Text = reader1["Exercise_name"].ToString();
                    label66.Text = reader1["Focus_muscle"].ToString();
                }
                reader1.Close();
                conn.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e) // creating a new workout plan
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select User_ID from users where Username = '" + LogIn.USER_NAME + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                string id = cmd.ExecuteScalar().ToString();
                cmd.Dispose();
                string main_q = "INSERT INTO Workout_Plan(Plan_name, Goal, Level, Charges, Creator_ID) VALUES ('" + textBox4.Text.ToString() + "', '" + comboBox8.Text.ToString() + "', " + textBox5.Text.ToString() + ", " + textBox6.Text.ToString() + "," + id + ")";
                SqlCommand cm;
                cm = new SqlCommand(main_q, conn);
                cm.ExecuteNonQuery();
                cm.Dispose();
                conn.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Workout Plan Created ! ");
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select* from Diet_plan Where Diet_name = '" + comboBox12.Text.ToString() + "'"; // displaying info of meal
                SqlCommand cm1 = new SqlCommand(query, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label80.Text = reader1["Diet_name"].ToString();
                    label78.Text = reader1["Type"].ToString();
                    label79.Text = reader1["Goal"].ToString();
                }
                reader1.Close();
                conn.Close();
            }
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string q1 = "SELECT COUNT(Meal_ID) FROM Diet_Meal WHERE Diet_ID = (SELECT Diet_ID FROM Diet_Plan WHERE Diet_name = '" + label80.Text.ToString() + "') ";
                SqlCommand cm2 = new SqlCommand(q1, conn);
                string no_meals = cm2.ExecuteScalar().ToString();
                cm2.Dispose();
                label68.Text = no_meals;
                conn.Close();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                try
                {
                    string query = "SELECT User_ID FROM Users WHERE Username = @Username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    string id = cmd.ExecuteScalar().ToString();
                    cmd.Dispose();

                    string main_q = "INSERT INTO Diet_Plan(Diet_name, Diet_type, Diet_goal, Creator_ID) VALUES (@DietName, @DietType, @DietGoal, @CreatorID)";
                    SqlCommand cm = new SqlCommand(main_q, conn);
                    cm.Parameters.AddWithValue("@DietName", textBox3.Text);
                    cm.Parameters.AddWithValue("@DietType", comboBox4.Text);
                    cm.Parameters.AddWithValue("@DietGoal", comboBox9.Text);
                    cm.Parameters.AddWithValue("@CreatorID", Convert.ToInt32(id));
                    cm.ExecuteNonQuery();
                    cm.Dispose();

                    MessageBox.Show("Diet Plan created successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating diet plan: " + ex.Message);
                }
                conn.Close();
            }
        }

        private void comboBox5_SelectedIndexChanged_1(object sender, EventArgs e) // display meals without allergens
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query2 = "SELECT Meals.Meal_ID, Meal_name FROM Meals JOIN Meal_Contains ON Meals.Meal_ID = Meal_Contains.Meal_ID WHERE Allergen_ID != (Select Allergen_ID FROM Allergen WHERE Allergen_names = @AllergenName) AND Meal_type = @MealType";

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
                conn.Close();
            }

        }

        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select Exercise_name, Focus_muscle from Exercise Where Exercise_name = '" + comboBox3.Text.ToString() + "'"; // displaying info of meal
                SqlCommand cm1 = new SqlCommand(query, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label67.Text = reader1["Exercise_name"].ToString();
                    label66.Text = reader1["Focus_muscle"].ToString();
                }
                reader1.Close();
                conn.Close();
            }
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select Meal_name, Fibre, Protein, Carbohydrates, Fats from Meals Where Meal_name = '" + comboBox2.Text.ToString() + "'"; // displaying info of meal
                SqlCommand cm1 = new SqlCommand(query, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label48.Text = reader1["Meal_name"].ToString();
                    label51.Text = reader1["Fibre"].ToString();
                    label50.Text = reader1["Protein"].ToString();
                    label53.Text = reader1["Carbohydrates"].ToString();
                    label52.Text = reader1["Fats"].ToString();

                }
                reader1.Close();
                conn.Close();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Diet Plan created successfully ! ");
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query2 = "SELECT Appointment_ID FROM Appointment WHERE Trainer_ID = @TrainerID AND Member_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";

                using (SqlCommand cm = new SqlCommand(query2, conn))
                {
                    int trainerId;
                    if (!int.TryParse(comboBox13.Text, out trainerId))
                    {
                        MessageBox.Show("Please select a valid trainer.");
                        return;
                    }

                    cm.Parameters.AddWithValue("@TrainerID", trainerId);
                    cm.Parameters.AddWithValue("@Username", LogIn.USER_NAME);

                    SqlDataAdapter sda = new SqlDataAdapter(cm);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox14.DataSource = dt;
                    comboBox14.ValueMember = "Appointment_ID";
                    comboBox14.DisplayMember = "Appointment_ID";
                }
                conn.Close();
            }


        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string query = "Select Appointment_Date, Appointment_start_time, Appointment_end_time from Appointment Where Appointment_ID = " + comboBox14.Text.ToString(); // displaying info of meal
                SqlCommand cm1 = new SqlCommand(query, conn);
                SqlDataReader reader1 = cm1.ExecuteReader();
                if (reader1.Read())
                {
                    label82.Text = reader1["Appointment_Date"].ToString();
                    label85.Text = reader1["Appointment_start_time"].ToString();
                    label86.Text = reader1["Appointment_end_time"].ToString();
                }
                reader1.Close();
                conn.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e) // submit appointment request
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string q1 = "Select User_ID from users where Username = '" + LogIn.USER_NAME + "'";
                SqlCommand c1 = new SqlCommand(q1, conn);
                string tID = comboBox6.Text;
                string mID = c1.ExecuteScalar().ToString();
                string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string stime = textBox1.Text.ToString();
                string etime = textBox2.Text.ToString();
                using (conn)
                {
                    string storedProcedure = "SP_Request_Appointment";
                    using (SqlCommand cm2 = new SqlCommand(storedProcedure, conn))
                    {
                        cm2.CommandType = CommandType.StoredProcedure;
                        cm2.Parameters.AddWithValue("@Date", date);
                        cm2.Parameters.AddWithValue("@Start", stime);
                        cm2.Parameters.AddWithValue("@End", etime);
                        cm2.Parameters.AddWithValue("@Member_ID", Convert.ToInt32(mID));
                        cm2.Parameters.AddWithValue("@Trainer_ID", Convert.ToInt32(tID));
                        cm2.ExecuteNonQuery();
                        cm2.Dispose();
                    }
                }
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var medit = new M_editprofile();
            this.Close();
            medit.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();
                string q1 = "Select User_ID from users where Username = '" + LogIn.USER_NAME + "'";
                SqlCommand cmd = new SqlCommand(q1, conn);
                string mID = cmd.ExecuteScalar().ToString();
                string tID = comboBox1.Text;
                string rating = numericUpDown1.Value.ToString();
                string review = richTextBox1.Text;

                string query = "INSERT INTO Feedback (Rating, Review, Member_ID, Trainer_ID) VALUES (" + Convert.ToInt32(rating).ToString() + ", '" + review + "', " + mID + ", " + tID + ")";
                SqlCommand cm = new SqlCommand(query, conn);
                cm.ExecuteNonQuery();
                cm.Dispose();

                MessageBox.Show("Feedback submitted successfully !");

                richTextBox1.Text = "";
                conn.Close();
            }

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

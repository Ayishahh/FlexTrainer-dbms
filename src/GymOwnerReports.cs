using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FlexTrainer;

namespace FlexTrainer
{
    public partial class GymOwnerReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private Panel parameterPanel;
        private Button btnLoadReport;
        private ComboBox cmbTrainer;
        private ComboBox cmbDietPlan;
        private NumericUpDown nudMonths;

        public GymOwnerReports()
        {
            InitializeComponent();
            LoadReportTypes();
            LoadDropdownData();
        }

        private void InitializeComponent()
        {
            this.Text = "Gym Owner Reports";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Gym Owner Reports";
            lblTitle.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.Size = new System.Drawing.Size(300, 30);
            this.Controls.Add(lblTitle);

            // Report Type Label
            Label lblReportType = new Label();
            lblReportType.Text = "Select Report:";
            lblReportType.Location = new System.Drawing.Point(20, 70);
            lblReportType.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblReportType);

            // Report Type ComboBox
            cmbReportType = new ComboBox();
            cmbReportType.Location = new System.Drawing.Point(130, 70);
            cmbReportType.Size = new System.Drawing.Size(400, 25);
            cmbReportType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;
            this.Controls.Add(cmbReportType);

            // Parameter Panel
            parameterPanel = new Panel();
            parameterPanel.Location = new System.Drawing.Point(20, 110);
            parameterPanel.Size = new System.Drawing.Size(1140, 80);
            parameterPanel.BorderStyle = BorderStyle.FixedSingle;
            parameterPanel.Visible = false;
            this.Controls.Add(parameterPanel);

            // Load Report Button
            btnLoadReport = new Button();
            btnLoadReport.Text = "Load Report";
            btnLoadReport.Location = new System.Drawing.Point(550, 70);
            btnLoadReport.Size = new System.Drawing.Size(120, 25);
            btnLoadReport.Click += BtnLoadReport_Click;
            this.Controls.Add(btnLoadReport);

            // DataGridView
            dataGridView1 = new DataGridView();
            dataGridView1.Location = new System.Drawing.Point(20, 210);
            dataGridView1.Size = new System.Drawing.Size(1140, 400);
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dataGridView1);

            // Record Count Label
            lblRecordCount = new Label();
            lblRecordCount.Text = "0 records found";
            lblRecordCount.Location = new System.Drawing.Point(20, 620);
            lblRecordCount.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(lblRecordCount);
        }

        private void LoadReportTypes()
        {
            cmbReportType.Items.Add("Members by Trainer and Gym");
            cmbReportType.Items.Add("Members by Diet Plan and Gym");
            cmbReportType.Items.Add("Gym Revenue Analysis");
            cmbReportType.Items.Add("Trainer Performance (My Trainers)");
            cmbReportType.Items.Add("New Memberships (Recent)");
            cmbReportType.Items.Add("Member Activity (My Gym)");
        }

        private void LoadDropdownData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Get Gym Owner's Gym ID
                    string gymIdQuery = "SELECT Gym_ID FROM Gym WHERE GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                    SqlCommand cmdGymId = new SqlCommand(gymIdQuery, conn);
                    cmdGymId.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    object gymIdResult = cmdGymId.ExecuteScalar();

                    if (gymIdResult != null)
                    {
                        int gymId = Convert.ToInt32(gymIdResult);

                        // Load trainers at this gym
                        string trainerQuery = "SELECT Trainer_ID, CONCAT((SELECT First_name FROM Users WHERE User_ID = Trainer_ID), ' ', (SELECT Last_name FROM Users WHERE User_ID = Trainer_ID)) AS Trainer_Name FROM Works_For WHERE Gym_ID = @GymID";
                        SqlCommand cmdTrainer = new SqlCommand(trainerQuery, conn);
                        cmdTrainer.Parameters.AddWithValue("@GymID", gymId);
                        SqlDataAdapter trainerAdapter = new SqlDataAdapter(cmdTrainer);
                        DataTable trainerDt = new DataTable();
                        trainerAdapter.Fill(trainerDt);

                        // Load diet plans
                        string dietQuery = "SELECT Diet_ID, Diet_name FROM Diet_Plan";
                        SqlCommand cmdDiet = new SqlCommand(dietQuery, conn);
                        SqlDataAdapter dietAdapter = new SqlDataAdapter(cmdDiet);
                        DataTable dietDt = new DataTable();
                        dietAdapter.Fill(dietDt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dropdown data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members by Trainer and Gym
                    parameterPanel.Visible = true;
                    Label lblTrainer = new Label();
                    lblTrainer.Text = "Select Trainer:";
                    lblTrainer.Location = new System.Drawing.Point(10, 15);
                    lblTrainer.Size = new System.Drawing.Size(100, 20);
                    parameterPanel.Controls.Add(lblTrainer);

                    cmbTrainer = new ComboBox();
                    cmbTrainer.Location = new System.Drawing.Point(120, 15);
                    cmbTrainer.Size = new System.Drawing.Size(250, 25);
                    cmbTrainer.DropDownStyle = ComboBoxStyle.DropDownList;
                    LoadTrainersForGym();
                    parameterPanel.Controls.Add(cmbTrainer);
                    break;

                case 1: // Members by Diet Plan and Gym
                    parameterPanel.Visible = true;
                    Label lblDietPlan = new Label();
                    lblDietPlan.Text = "Select Diet Plan:";
                    lblDietPlan.Location = new System.Drawing.Point(10, 15);
                    lblDietPlan.Size = new System.Drawing.Size(100, 20);
                    parameterPanel.Controls.Add(lblDietPlan);

                    cmbDietPlan = new ComboBox();
                    cmbDietPlan.Location = new System.Drawing.Point(120, 15);
                    cmbDietPlan.Size = new System.Drawing.Size(250, 25);
                    cmbDietPlan.DropDownStyle = ComboBoxStyle.DropDownList;
                    LoadDietPlans();
                    parameterPanel.Controls.Add(cmbDietPlan);
                    break;

                case 4: // New Memberships
                    parameterPanel.Visible = true;
                    Label lblMonths = new Label();
                    lblMonths.Text = "Months Back:";
                    lblMonths.Location = new System.Drawing.Point(10, 15);
                    lblMonths.Size = new System.Drawing.Size(100, 20);
                    parameterPanel.Controls.Add(lblMonths);

                    nudMonths = new NumericUpDown();
                    nudMonths.Location = new System.Drawing.Point(120, 15);
                    nudMonths.Size = new System.Drawing.Size(100, 25);
                    nudMonths.Minimum = 1;
                    nudMonths.Maximum = 24;
                    nudMonths.Value = 3;
                    parameterPanel.Controls.Add(nudMonths);
                    break;
            }
        }

        private void LoadTrainersForGym()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT t.Trainer_ID, CONCAT(u.First_name, ' ', u.Last_name) AS Trainer_Name
                                    FROM Trainer t
                                    JOIN Users u ON t.Trainer_ID = u.User_ID
                                    JOIN Works_For wf ON t.Trainer_ID = wf.Trainer_ID
                                    JOIN Gym g ON wf.Gym_ID = g.Gym_ID
                                    WHERE g.GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbTrainer.DataSource = dt;
                    cmbTrainer.ValueMember = "Trainer_ID";
                    cmbTrainer.DisplayMember = "Trainer_Name";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading trainers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDietPlans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Diet_ID, Diet_name FROM Diet_Plan";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbDietPlan.DataSource = dt;
                    cmbDietPlan.ValueMember = "Diet_ID";
                    cmbDietPlan.DisplayMember = "Diet_name";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading diet plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetOwnerGymId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Gym_ID FROM Gym WHERE GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private void BtnLoadReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int gymId = GetOwnerGymId();
            if (gymId == 0)
            {
                MessageBox.Show("Could not determine your gym. Please contact administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members by Trainer and Gym
                    if (cmbTrainer.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a trainer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_By_Trainer_And_Gym";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Trainer_ID", cmbTrainer.SelectedValue),
                        new SqlParameter("@Gym_ID", gymId)
                    };
                    break;

                case 1: // Members by Diet Plan and Gym
                    if (cmbDietPlan.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a diet plan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_By_Diet_Plan_And_Gym";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Diet_Plan_ID", cmbDietPlan.SelectedValue),
                        new SqlParameter("@Gym_ID", gymId)
                    };
                    break;

                case 2: // Gym Revenue
                    procedureName = "SP_Report_Gym_Revenue";
                    parameters = new SqlParameter[] { new SqlParameter("@Gym_ID", gymId) };
                    break;

                case 3: // Trainer Performance
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new SqlParameter[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
                    break;

                case 4: // New Memberships
                    procedureName = "SP_Report_New_Memberships_Recent";
                    parameters = new SqlParameter[] { new SqlParameter("@Months_Back", nudMonths.Value) };
                    break;

                case 5: // Member Activity
                    procedureName = "SP_Report_Member_Activity";
                    parameters = new SqlParameter[] { new SqlParameter("@Member_ID", DBNull.Value) };
                    break;
            }

            LoadReport(procedureName, parameters);
        }

        private void LoadReport(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumns();
                        lblRecordCount.Text = $"{dt.Rows.Count} records found";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

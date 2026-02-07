using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FlexTrainer
{
    public partial class TrainerReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private FlowLayoutPanel parameterPanel;
        private Button btnLoadReport;

        // Dynamic parameter controls
        private ComboBox cmbGym;
        private ComboBox cmbDietPlan;
        private ComboBox cmbGoal;
        private ComboBox cmbDietType;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;

        public TrainerReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Trainer Reports";
            this.Size = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lblTitle = new Label
            {
                Text = "Trainer Reports",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 12),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            Label lblReportType = new Label
            {
                Text = "Select Report:",
                Location = new System.Drawing.Point(20, 55),
                AutoSize = true
            };
            this.Controls.Add(lblReportType);

            cmbReportType = new ComboBox
            {
                Location = new System.Drawing.Point(130, 52),
                Size = new System.Drawing.Size(400, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;
            this.Controls.Add(cmbReportType);

            btnLoadReport = new Button
            {
                Text = "Load Report",
                Location = new System.Drawing.Point(550, 50),
                Size = new System.Drawing.Size(120, 28)
            };
            btnLoadReport.Click += BtnLoadReport_Click;
            this.Controls.Add(btnLoadReport);

            parameterPanel = new FlowLayoutPanel
            {
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(1140, 65),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(10, 6, 10, 6),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(parameterPanel);

            dataGridView1 = new DataGridView
            {
                Location = new System.Drawing.Point(20, 155),
                Size = new System.Drawing.Size(1140, 500),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.SystemColors.Window,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(dataGridView1);

            lblRecordCount = new Label
            {
                Text = "Select a report and click Load Report.",
                Location = new System.Drawing.Point(20, 665),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(lblRecordCount);
        }

        private void LoadReportTypes()
        {
            cmbReportType.Items.Add("My Clients at a Gym");                       // 0  -> SP_Report_Members_By_Trainer_And_Gym
            cmbReportType.Items.Add("My Clients Across Gyms (By Diet Plan)");     // 1  -> SP_Report_Members_Cross_Gym_By_Trainer_Diet
            cmbReportType.Items.Add("My Appointment Schedule");                   // 2  -> SP_Report_Trainer_Appointments
            cmbReportType.Items.Add("My Performance Metrics");                    // 3  -> SP_Report_Trainer_Performance
            cmbReportType.Items.Add("Workout Plans by Goal");                     // 4  -> SP_Report_Workout_Plans_By_Goal
            cmbReportType.Items.Add("Diet Plans by Type");                        // 5  -> SP_Report_Diet_Plans_By_Type
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Clients at Gym
                    parameterPanel.Visible = true;
                    AddLabel("Gym:", 10, 15);
                    cmbGym = new ComboBox
                    {
                        Size = new System.Drawing.Size(300, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    LoadGymsForTrainer();
                    parameterPanel.Controls.Add(cmbGym);
                    break;

                case 1: // Cross Gym By Diet
                    parameterPanel.Visible = true;
                    AddLabel("Diet Plan:", 10, 15);
                    cmbDietPlan = new ComboBox
                    {
                        Size = new System.Drawing.Size(300, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    LoadDietPlans();
                    parameterPanel.Controls.Add(cmbDietPlan);
                    break;

                case 2: // Appointment Schedule
                    parameterPanel.Visible = true;
                    AddLabel("Start Date:", 10, 15);
                    dtpStartDate = new DateTimePicker
                    {
                        Size = new System.Drawing.Size(200, 25),
                        Value = DateTime.Now,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(dtpStartDate);

                    AddLabel("End Date:", 320, 15);
                    dtpEndDate = new DateTimePicker
                    {
                        Size = new System.Drawing.Size(200, 25),
                        Value = DateTime.Now.AddDays(30),
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(dtpEndDate);
                    break;

                case 4: // By Goal
                    parameterPanel.Visible = true;
                    AddLabel("Goal (optional):", 10, 15);
                    cmbGoal = new ComboBox
                    {
                        Size = new System.Drawing.Size(250, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    cmbGoal.Items.Add("(All Goals)");
                    LoadGoals();
                    cmbGoal.SelectedIndex = 0;
                    parameterPanel.Controls.Add(cmbGoal);
                    break;

                case 5: // By Type
                    parameterPanel.Visible = true;
                    AddLabel("Diet Type (optional):", 10, 15);
                    cmbDietType = new ComboBox
                    {
                        Size = new System.Drawing.Size(250, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    cmbDietType.Items.Add("(All Types)");
                    LoadDietTypes();
                    cmbDietType.SelectedIndex = 0;
                    parameterPanel.Controls.Add(cmbDietType);
                    break;
            }
        }

        private void AddLabel(string text, int x, int y)
        {
            parameterPanel.Controls.Add(new Label
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            });
        }

        private int GetTrainerId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT User_ID FROM Users WHERE Username = @Username", conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch { return 0; }
        }

        private void LoadGymsForTrainer()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT g.Gym_ID, g.Gym_name
                                     FROM Gym g
                                     JOIN Works_For wf ON g.Gym_ID = wf.Gym_ID
                                     WHERE wf.Trainer_ID = (SELECT User_ID FROM Users WHERE Username = @Username)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    cmbGym.DataSource = dt;
                    cmbGym.ValueMember = "Gym_ID";
                    cmbGym.DisplayMember = "Gym_name";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading gyms: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDietPlans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Diet_ID, Diet_name FROM Diet_Plan", conn);
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

        private void LoadGoals()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT DISTINCT Goal FROM Workout_Plan ORDER BY Goal", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        cmbGoal.Items.Add(reader["Goal"].ToString());
                    reader.Close();
                }
            }
            catch { }
        }

        private void LoadDietTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT DISTINCT Diet_type FROM Diet_Plan ORDER BY Diet_type", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        cmbDietType.Items.Add(reader["Diet_type"].ToString());
                    reader.Close();
                }
            }
            catch { }
        }

        private void BtnLoadReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int trainerId = GetTrainerId();
            if (trainerId == 0)
            {
                MessageBox.Show("Could not determine your trainer ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Clients at Gym
                    if (cmbGym.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a gym.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_By_Trainer_And_Gym";
                    parameters = new[]
                    {
                        new SqlParameter("@Trainer_ID", trainerId),
                        new SqlParameter("@Gym_ID", cmbGym.SelectedValue)
                    };
                    break;

                case 1: // Cross Gym By Diet
                    if (cmbDietPlan.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a diet plan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_Cross_Gym_By_Trainer_Diet";
                    parameters = new[]
                    {
                        new SqlParameter("@Trainer_ID", trainerId),
                        new SqlParameter("@Diet_Plan_ID", cmbDietPlan.SelectedValue)
                    };
                    break;

                case 2: // Appointment Schedule
                    procedureName = "SP_Report_Trainer_Appointments";
                    parameters = new[]
                    {
                        new SqlParameter("@Trainer_ID", trainerId),
                        new SqlParameter("@Start_Date", dtpStartDate.Value.Date),
                        new SqlParameter("@End_Date", dtpEndDate.Value.Date)
                    };
                    break;

                case 3: // Performance Metrics
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new[] { new SqlParameter("@Trainer_ID", trainerId) };
                    break;

                case 4: // By Goal
                    procedureName = "SP_Report_Workout_Plans_By_Goal";
                    object goalVal = (cmbGoal.SelectedIndex <= 0) ? (object)DBNull.Value : cmbGoal.SelectedItem.ToString();
                    parameters = new[] { new SqlParameter("@Goal", goalVal) };
                    break;

                case 5: // By Type
                    procedureName = "SP_Report_Diet_Plans_By_Type";
                    object typeVal = (cmbDietType.SelectedIndex <= 0) ? (object)DBNull.Value : cmbDietType.SelectedItem.ToString();
                    parameters = new[] { new SqlParameter("@Diet_Type", typeVal) };
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
                            cmd.Parameters.AddRange(parameters);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumns();
                        lblRecordCount.Text = $"{dt.Rows.Count} record(s) found";
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

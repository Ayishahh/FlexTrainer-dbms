using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FlexTrainer
{
    public partial class GymOwnerReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private FlowLayoutPanel parameterPanel;
        private Button btnLoadReport;

        // Dynamic parameter controls
        private ComboBox cmbTrainer;
        private ComboBox cmbDietPlan;
        private ComboBox cmbMachine;
        private NumericUpDown nudMonths;
        private DateTimePicker dtpUsageDate;

        public GymOwnerReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Gym Owner Reports";
            this.Size = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lblTitle = new Label
            {
                Text = "Gym Owner Reports",
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
            cmbReportType.Items.Add("Members by Trainer");                    // 0  -> SP_Report_Members_By_Trainer_And_Gym
            cmbReportType.Items.Add("Members by Diet Plan");                  // 1  -> SP_Report_Members_By_Diet_Plan_And_Gym
            cmbReportType.Items.Add("Machine Usage by Day");                  // 2  -> SP_Report_Machine_Usage_By_Day
            cmbReportType.Items.Add("Gym Revenue Analysis");                  // 3  -> SP_Report_Gym_Revenue
            cmbReportType.Items.Add("Trainer Performance");                   // 4  -> SP_Report_Trainer_Performance
            cmbReportType.Items.Add("New Memberships (Recent)");              // 5  -> SP_Report_New_Memberships_Recent
            cmbReportType.Items.Add("Member Activity (All Members)");         // 6  -> SP_Report_Member_Activity
            cmbReportType.Items.Add("Gym Member Comparison");                 // 7  -> SP_Report_Gym_Member_Comparison
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members by Trainer
                    parameterPanel.Visible = true;
                    AddLabel("Trainer:", 10, 15);
                    cmbTrainer = new ComboBox
                    {
                        Size = new System.Drawing.Size(300, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    LoadTrainersForGym();
                    parameterPanel.Controls.Add(cmbTrainer);
                    break;

                case 1: // Members by Diet Plan
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

                case 2: // Machine Usage
                    parameterPanel.Visible = true;
                    AddLabel("Machine:", 10, 15);
                    cmbMachine = new ComboBox
                    {
                        Size = new System.Drawing.Size(250, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    LoadMachines();
                    parameterPanel.Controls.Add(cmbMachine);

                    AddLabel("Date:", 380, 15);
                    dtpUsageDate = new DateTimePicker
                    {
                        Size = new System.Drawing.Size(200, 25),
                        Value = DateTime.Now,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(dtpUsageDate);
                    break;

                case 5: // New Memberships
                    parameterPanel.Visible = true;
                    AddLabel("Months Back:", 10, 15);
                    nudMonths = new NumericUpDown
                    {
                        Size = new System.Drawing.Size(80, 25),
                        Minimum = 1,
                        Maximum = 24,
                        Value = 3,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(nudMonths);
                    break;

                case 7: // Gym Member Comparison
                    parameterPanel.Visible = true;
                    AddLabel("Months Back:", 10, 15);
                    nudMonths = new NumericUpDown
                    {
                        Size = new System.Drawing.Size(80, 25),
                        Minimum = 1,
                        Maximum = 24,
                        Value = 6,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(nudMonths);
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

        private int GetOwnerGymId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Gym_ID FROM Gym WHERE GymOwner_ID = (SELECT User_ID FROM Users WHERE Username = @Username)", conn);
                    cmd.Parameters.AddWithValue("@Username", LogIn.USER_NAME);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch { return 0; }
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

        private void LoadMachines()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Machine_ID, Machine_name FROM Machine ORDER BY Machine_name", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    cmbMachine.DataSource = dt;
                    cmbMachine.ValueMember = "Machine_ID";
                    cmbMachine.DisplayMember = "Machine_name";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading machines: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Could not determine your gym.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members by Trainer
                    if (cmbTrainer.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a trainer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_By_Trainer_And_Gym";
                    parameters = new[]
                    {
                        new SqlParameter("@Trainer_ID", cmbTrainer.SelectedValue),
                        new SqlParameter("@Gym_ID", gymId)
                    };
                    break;

                case 1: // Members by Diet Plan
                    if (cmbDietPlan.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a diet plan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_By_Diet_Plan_And_Gym";
                    parameters = new[]
                    {
                        new SqlParameter("@Diet_Plan_ID", cmbDietPlan.SelectedValue),
                        new SqlParameter("@Gym_ID", gymId)
                    };
                    break;

                case 2: // Machine Usage
                    if (cmbMachine.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a machine.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Machine_Usage_By_Day";
                    parameters = new[]
                    {
                        new SqlParameter("@Machine_ID", cmbMachine.SelectedValue),
                        new SqlParameter("@Usage_Date", dtpUsageDate.Value.Date)
                    };
                    break;

                case 3: // Gym Revenue
                    procedureName = "SP_Report_Gym_Revenue";
                    parameters = new[] { new SqlParameter("@Gym_ID", gymId) };
                    break;

                case 4: // Trainer Performance
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
                    break;

                case 5: // New Memberships
                    procedureName = "SP_Report_New_Memberships_Recent";
                    parameters = new[] { new SqlParameter("@Months_Back", (int)nudMonths.Value) };
                    break;

                case 6: // Member Activity
                    procedureName = "SP_Report_Member_Activity";
                    parameters = new[] { new SqlParameter("@Member_ID", DBNull.Value) };
                    break;

                case 7: // Gym Member Comparison
                    procedureName = "SP_Report_Gym_Member_Comparison";
                    parameters = new[] { new SqlParameter("@Months_Back", (int)nudMonths.Value) };
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

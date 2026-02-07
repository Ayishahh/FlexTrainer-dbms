using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FlexTrainer;

namespace Project
{
    public partial class AdminReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private FlowLayoutPanel parameterPanel;
        private Button btnLoadReport;

        // Dynamic parameter controls
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private ComboBox cmbActivityType;
        private NumericUpDown nudTopN;
        private NumericUpDown nudMonths;

        public AdminReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Reports";
            this.Size = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lblTitle = new Label
            {
                Text = "Admin Reports",
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
                Size = new System.Drawing.Size(1140, 75),
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
                Location = new System.Drawing.Point(20, 175),
                Size = new System.Drawing.Size(1140, 480),
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
            cmbReportType.Items.Add("Trainer Performance (All Trainers)");     // 0  -> SP_Report_Trainer_Performance
            cmbReportType.Items.Add("Gym Member Comparison");                  // 1  -> SP_Report_Gym_Member_Comparison
            cmbReportType.Items.Add("Pending Approval Requests");              // 2  -> SP_Report_Pending_Requests
            cmbReportType.Items.Add("System Audit Log");                       // 3  -> SP_Report_System_Audit_Log
            cmbReportType.Items.Add("Gym Revenue Analysis (All Gyms)");        // 4  -> SP_Report_Gym_Revenue
            cmbReportType.Items.Add("Popular Workout Plans");                  // 5  -> SP_Report_Popular_Workout_Plans
            cmbReportType.Items.Add("Popular Diet Plans");                     // 6  -> SP_Report_Popular_Diet_Plans
            cmbReportType.Items.Add("Member Activity (All Members)");          // 7  -> SP_Report_Member_Activity
            cmbReportType.Items.Add("New Memberships (Recent)");               // 8  -> SP_Report_New_Memberships_Recent
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 1: // Gym Member Comparison
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

                case 3: // System Audit Log
                    parameterPanel.Visible = true;
                    AddLabel("Start Date:", 10, 15);
                    dtpStartDate = new DateTimePicker
                    {
                        Size = new System.Drawing.Size(200, 25),
                        Value = DateTime.Now.AddDays(-30),
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(dtpStartDate);

                    AddLabel("End Date:", 320, 15);
                    dtpEndDate = new DateTimePicker
                    {
                        Size = new System.Drawing.Size(200, 25),
                        Value = DateTime.Now,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(dtpEndDate);

                    AddLabel("Activity Type:", 10, 45);
                    cmbActivityType = new ComboBox
                    {
                        Size = new System.Drawing.Size(150, 25),
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    cmbActivityType.Items.AddRange(new object[] { "All", "INSERT", "UPDATE", "DELETE" });
                    cmbActivityType.SelectedIndex = 0;
                    parameterPanel.Controls.Add(cmbActivityType);
                    break;

                case 5: // Popular Workout Plans
                case 6: // Popular Diet Plans
                    parameterPanel.Visible = true;
                    AddLabel("Top N results:", 10, 15);
                    nudTopN = new NumericUpDown
                    {
                        Size = new System.Drawing.Size(80, 25),
                        Minimum = 1,
                        Maximum = 100,
                        Value = 10,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(nudTopN);
                    break;

                case 8: // New Memberships
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

        private void BtnLoadReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Trainer Performance
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
                    break;

                case 1: // Gym Member Comparison
                    procedureName = "SP_Report_Gym_Member_Comparison";
                    parameters = new[] { new SqlParameter("@Months_Back", (int)nudMonths.Value) };
                    break;

                case 2: // Pending Requests
                    procedureName = "SP_Report_Pending_Requests";
                    break;

                case 3: // System Audit Log
                    string activityType = cmbActivityType.SelectedItem.ToString();
                    procedureName = "SP_Report_System_Audit_Log";
                    parameters = new[]
                    {
                        new SqlParameter("@Start_Date", dtpStartDate.Value.Date),
                        new SqlParameter("@End_Date", dtpEndDate.Value.Date),
                        new SqlParameter("@Activity_Type", activityType == "All" ? (object)DBNull.Value : activityType)
                    };
                    break;

                case 4: // Gym Revenue
                    procedureName = "SP_Report_Gym_Revenue";
                    parameters = new[] { new SqlParameter("@Gym_ID", DBNull.Value) };
                    break;

                case 5: // Popular Workout Plans
                    procedureName = "SP_Report_Popular_Workout_Plans";
                    parameters = new[] { new SqlParameter("@Top_N", (int)nudTopN.Value) };
                    break;

                case 6: // Popular Diet Plans
                    procedureName = "SP_Report_Popular_Diet_Plans";
                    parameters = new[] { new SqlParameter("@Top_N", (int)nudTopN.Value) };
                    break;

                case 7: // Member Activity
                    procedureName = "SP_Report_Member_Activity";
                    parameters = new[] { new SqlParameter("@Member_ID", DBNull.Value) };
                    break;

                case 8: // New Memberships
                    procedureName = "SP_Report_New_Memberships_Recent";
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

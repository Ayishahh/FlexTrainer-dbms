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
        private Panel parameterPanel;
        private Button btnLoadReport;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private ComboBox cmbActivityType;

        public AdminReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Reports";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Admin Reports";
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
            cmbReportType.Items.Add("Trainer Performance (All Trainers)");
            cmbReportType.Items.Add("Gym Member Comparison");
            cmbReportType.Items.Add("Pending Approval Requests");
            cmbReportType.Items.Add("System Audit Log");
            cmbReportType.Items.Add("Gym Revenue Analysis (All Gyms)");
            cmbReportType.Items.Add("Popular Workout Plans");
            cmbReportType.Items.Add("Popular Diet Plans");
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            // Show parameter panel for System Audit Log
            if (cmbReportType.SelectedIndex == 3) // System Audit Log
            {
                parameterPanel.Visible = true;

                Label lblStartDate = new Label();
                lblStartDate.Text = "Start Date:";
                lblStartDate.Location = new System.Drawing.Point(10, 15);
                lblStartDate.Size = new System.Drawing.Size(80, 20);
                parameterPanel.Controls.Add(lblStartDate);

                dtpStartDate = new DateTimePicker();
                dtpStartDate.Location = new System.Drawing.Point(100, 15);
                dtpStartDate.Size = new System.Drawing.Size(200, 25);
                dtpStartDate.Value = DateTime.Now.AddDays(-30);
                parameterPanel.Controls.Add(dtpStartDate);

                Label lblEndDate = new Label();
                lblEndDate.Text = "End Date:";
                lblEndDate.Location = new System.Drawing.Point(320, 15);
                lblEndDate.Size = new System.Drawing.Size(80, 20);
                parameterPanel.Controls.Add(lblEndDate);

                dtpEndDate = new DateTimePicker();
                dtpEndDate.Location = new System.Drawing.Point(410, 15);
                dtpEndDate.Size = new System.Drawing.Size(200, 25);
                dtpEndDate.Value = DateTime.Now;
                parameterPanel.Controls.Add(dtpEndDate);

                Label lblActivityType = new Label();
                lblActivityType.Text = "Activity Type:";
                lblActivityType.Location = new System.Drawing.Point(10, 45);
                lblActivityType.Size = new System.Drawing.Size(80, 20);
                parameterPanel.Controls.Add(lblActivityType);

                cmbActivityType = new ComboBox();
                cmbActivityType.Location = new System.Drawing.Point(100, 45);
                cmbActivityType.Size = new System.Drawing.Size(200, 25);
                cmbActivityType.Items.Add("All");
                cmbActivityType.Items.Add("INSERT");
                cmbActivityType.Items.Add("UPDATE");
                cmbActivityType.Items.Add("DELETE");
                cmbActivityType.SelectedIndex = 0;
                parameterPanel.Controls.Add(cmbActivityType);
            }
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
                    parameters = new SqlParameter[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
                    break;

                case 1: // Gym Member Comparison
                    procedureName = "SP_Report_Gym_Member_Comparison";
                    parameters = new SqlParameter[] { new SqlParameter("@Months_Back", 6) };
                    break;

                case 2: // Pending Requests
                    procedureName = "SP_Report_Pending_Requests";
                    break;

                case 3: // System Audit Log
                    procedureName = "SP_Report_System_Audit_Log";
                    string activityType = cmbActivityType.SelectedItem.ToString();
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Start_Date", dtpStartDate.Value.Date),
                        new SqlParameter("@End_Date", dtpEndDate.Value.Date),
                        new SqlParameter("@Activity_Type", activityType == "All" ? (object)DBNull.Value : activityType)
                    };
                    break;

                case 4: // Gym Revenue
                    procedureName = "SP_Report_Gym_Revenue";
                    parameters = new SqlParameter[] { new SqlParameter("@Gym_ID", DBNull.Value) };
                    break;

                case 5: // Popular Workout Plans
                    procedureName = "SP_Report_Popular_Workout_Plans";
                    parameters = new SqlParameter[] { new SqlParameter("@Top_N", 10) };
                    break;

                case 6: // Popular Diet Plans
                    procedureName = "SP_Report_Popular_Diet_Plans";
                    parameters = new SqlParameter[] { new SqlParameter("@Top_N", 10) };
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

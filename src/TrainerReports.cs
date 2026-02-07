using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FlexTrainer;

namespace FlexTrainer
{
    public partial class TrainerReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private Panel parameterPanel;
        private Button btnLoadReport;
        private ComboBox cmbDietPlan;
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
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Trainer Reports";
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
            cmbReportType.Items.Add("My Clients Across Gyms (By Diet Plan)");
            cmbReportType.Items.Add("My Appointment Schedule");
            cmbReportType.Items.Add("My Performance Metrics");
            cmbReportType.Items.Add("My Clients Activity");
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members Cross Gym By Diet
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

                case 1: // Appointment Schedule
                    parameterPanel.Visible = true;
                    Label lblStartDate = new Label();
                    lblStartDate.Text = "Start Date:";
                    lblStartDate.Location = new System.Drawing.Point(10, 15);
                    lblStartDate.Size = new System.Drawing.Size(80, 20);
                    parameterPanel.Controls.Add(lblStartDate);

                    dtpStartDate = new DateTimePicker();
                    dtpStartDate.Location = new System.Drawing.Point(100, 15);
                    dtpStartDate.Size = new System.Drawing.Size(200, 25);
                    dtpStartDate.Value = DateTime.Now;
                    parameterPanel.Controls.Add(dtpStartDate);

                    Label lblEndDate = new Label();
                    lblEndDate.Text = "End Date:";
                    lblEndDate.Location = new System.Drawing.Point(320, 15);
                    lblEndDate.Size = new System.Drawing.Size(80, 20);
                    parameterPanel.Controls.Add(lblEndDate);

                    dtpEndDate = new DateTimePicker();
                    dtpEndDate.Location = new System.Drawing.Point(410, 15);
                    dtpEndDate.Size = new System.Drawing.Size(200, 25);
                    dtpEndDate.Value = DateTime.Now.AddDays(30);
                    parameterPanel.Controls.Add(dtpEndDate);
                    break;
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

        private int GetTrainerId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT User_ID FROM Users WHERE Username = @Username AND Role = 'Trainer'";
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

            int trainerId = GetTrainerId();
            if (trainerId == 0)
            {
                MessageBox.Show("Could not determine your trainer ID. Please contact administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Members Cross Gym By Diet
                    if (cmbDietPlan.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a diet plan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Members_Cross_Gym_By_Trainer_Diet";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Trainer_ID", trainerId),
                        new SqlParameter("@Diet_Plan_ID", cmbDietPlan.SelectedValue)
                    };
                    break;

                case 1: // Appointment Schedule
                    procedureName = "SP_Report_Trainer_Appointments";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Trainer_ID", trainerId),
                        new SqlParameter("@Start_Date", dtpStartDate.Value.Date),
                        new SqlParameter("@End_Date", dtpEndDate.Value.Date)
                    };
                    break;

                case 2: // Performance Metrics
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new SqlParameter[] { new SqlParameter("@Trainer_ID", trainerId) };
                    break;

                case 3: // Client Activity
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

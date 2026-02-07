using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FlexTrainer;

namespace FlexTrainer
{
    public partial class MemberReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private Panel parameterPanel;
        private Button btnLoadReport;

        public MemberReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Member Reports";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Member Reports";
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
            cmbReportType.Items.Add("My Activity Summary");
            cmbReportType.Items.Add("Popular Workout Plans");
            cmbReportType.Items.Add("Popular Diet Plans");
            cmbReportType.Items.Add("Trainer Performance (My Gym)");
        }

        private int GetMemberId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT User_ID FROM Users WHERE Username = @Username AND Role = 'Member'";
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

            int memberId = GetMemberId();
            if (memberId == 0)
            {
                MessageBox.Show("Could not determine your member ID. Please contact administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // My Activity Summary
                    procedureName = "SP_Report_Member_Activity";
                    parameters = new SqlParameter[] { new SqlParameter("@Member_ID", memberId) };
                    break;

                case 1: // Popular Workout Plans
                    procedureName = "SP_Report_Popular_Workout_Plans";
                    parameters = new SqlParameter[] { new SqlParameter("@Top_N", 10) };
                    break;

                case 2: // Popular Diet Plans
                    procedureName = "SP_Report_Popular_Diet_Plans";
                    parameters = new SqlParameter[] { new SqlParameter("@Top_N", 10) };
                    break;

                case 3: // Trainer Performance
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new SqlParameter[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
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

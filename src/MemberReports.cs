using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FlexTrainer
{
    public partial class MemberReports : Form
    {
        private ComboBox cmbReportType;
        private DataGridView dataGridView1;
        private Label lblRecordCount;
        private Label lblTitle;
        private FlowLayoutPanel parameterPanel;
        private Button btnLoadReport;

        // Dynamic parameter controls
        private NumericUpDown nudMaxCalories;
        private NumericUpDown nudMaxCarbs;
        private NumericUpDown nudTopN;
        private TextBox txtMachineName;
        private ComboBox cmbAllergen;
        private ComboBox cmbGoal;
        private ComboBox cmbDietType;

        public MemberReports()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void InitializeComponent()
        {
            this.Text = "Member Reports";
            this.Size = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lblTitle = new Label
            {
                Text = "Member Reports",
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
            cmbReportType.Items.Add("My Activity Summary");                       // 0  -> SP_Report_Member_Activity
            cmbReportType.Items.Add("Popular Workout Plans");                     // 1  -> SP_Report_Popular_Workout_Plans
            cmbReportType.Items.Add("Popular Diet Plans");                        // 2  -> SP_Report_Popular_Diet_Plans
            cmbReportType.Items.Add("Trainer Performance");                       // 3  -> SP_Report_Trainer_Performance
            cmbReportType.Items.Add("Low-Calorie Breakfast Diet Plans");          // 4  -> SP_Report_Diet_Plans_Low_Calorie_Breakfast
            cmbReportType.Items.Add("Low-Carb Diet Plans");                       // 5  -> SP_Report_Diet_Plans_Low_Carb
            cmbReportType.Items.Add("Workout Plans Without Specific Machine");    // 6  -> SP_Report_Workout_Plans_Without_Machine
            cmbReportType.Items.Add("Diet Plans Without Specific Allergen");      // 7  -> SP_Report_Diet_Plans_Without_Allergen
            cmbReportType.Items.Add("Workout Plans by Goal");                     // 8  -> SP_Report_Workout_Plans_By_Goal
            cmbReportType.Items.Add("Diet Plans by Type");                        // 9  -> SP_Report_Diet_Plans_By_Type
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameterPanel.Controls.Clear();
            parameterPanel.Visible = false;

            switch (cmbReportType.SelectedIndex)
            {
                case 1: // Popular Workout Plans
                case 2: // Popular Diet Plans
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

                case 4: // Low-Calorie Breakfast
                    parameterPanel.Visible = true;
                    AddLabel("Max Calories:", 10, 15);
                    nudMaxCalories = new NumericUpDown
                    {
                        Size = new System.Drawing.Size(100, 25),
                        Minimum = 50,
                        Maximum = 5000,
                        Value = 500,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(nudMaxCalories);
                    break;

                case 5: // Low-Carb
                    parameterPanel.Visible = true;
                    AddLabel("Max Carbs (g):", 10, 15);
                    nudMaxCarbs = new NumericUpDown
                    {
                        Size = new System.Drawing.Size(100, 25),
                        Minimum = 10,
                        Maximum = 5000,
                        Value = 300,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(nudMaxCarbs);
                    break;

                case 6: // Without Machine
                    parameterPanel.Visible = true;
                    AddLabel("Machine Name:", 10, 15);
                    txtMachineName = new TextBox
                    {
                        Size = new System.Drawing.Size(250, 25),
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    parameterPanel.Controls.Add(txtMachineName);
                    break;

                case 7: // Without Allergen
                    parameterPanel.Visible = true;
                    AddLabel("Allergen:", 10, 15);
                    cmbAllergen = new ComboBox
                    {
                        Size = new System.Drawing.Size(250, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 2, 16, 0)
                    };
                    LoadAllergens();
                    parameterPanel.Controls.Add(cmbAllergen);
                    break;

                case 8: // By Goal
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

                case 9: // By Type
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

        private int GetMemberId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT User_ID FROM Users WHERE Username = @Username";
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

        private void LoadAllergens()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Allergen_names FROM Allergen ORDER BY Allergen_names", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        cmbAllergen.Items.Add(reader["Allergen_names"].ToString());
                    reader.Close();
                }
            }
            catch { }
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

            string procedureName = "";
            SqlParameter[] parameters = null;

            switch (cmbReportType.SelectedIndex)
            {
                case 0: // My Activity Summary
                    int memberId = GetMemberId();
                    if (memberId == 0)
                    {
                        MessageBox.Show("Could not determine your member ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    procedureName = "SP_Report_Member_Activity";
                    parameters = new[] { new SqlParameter("@Member_ID", memberId) };
                    break;

                case 1: // Popular Workout Plans
                    procedureName = "SP_Report_Popular_Workout_Plans";
                    parameters = new[] { new SqlParameter("@Top_N", (int)nudTopN.Value) };
                    break;

                case 2: // Popular Diet Plans
                    procedureName = "SP_Report_Popular_Diet_Plans";
                    parameters = new[] { new SqlParameter("@Top_N", (int)nudTopN.Value) };
                    break;

                case 3: // Trainer Performance
                    procedureName = "SP_Report_Trainer_Performance";
                    parameters = new[] { new SqlParameter("@Trainer_ID", DBNull.Value) };
                    break;

                case 4: // Low-Calorie Breakfast
                    procedureName = "SP_Report_Diet_Plans_Low_Calorie_Breakfast";
                    parameters = new[] { new SqlParameter("@Max_Calories", (int)nudMaxCalories.Value) };
                    break;

                case 5: // Low-Carb
                    procedureName = "SP_Report_Diet_Plans_Low_Carb";
                    parameters = new[] { new SqlParameter("@Max_Carbs", (int)nudMaxCarbs.Value) };
                    break;

                case 6: // Without Machine
                    if (string.IsNullOrWhiteSpace(txtMachineName.Text))
                    {
                        MessageBox.Show("Please enter a machine name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Workout_Plans_Without_Machine";
                    parameters = new[] { new SqlParameter("@Machine_Name", txtMachineName.Text.Trim()) };
                    break;

                case 7: // Without Allergen
                    if (cmbAllergen.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select an allergen.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    procedureName = "SP_Report_Diet_Plans_Without_Allergen";
                    parameters = new[] { new SqlParameter("@Allergen_Name", cmbAllergen.SelectedItem.ToString()) };
                    break;

                case 8: // By Goal
                    procedureName = "SP_Report_Workout_Plans_By_Goal";
                    object goalVal = (cmbGoal.SelectedIndex <= 0) ? (object)DBNull.Value : cmbGoal.SelectedItem.ToString();
                    parameters = new[] { new SqlParameter("@Goal", goalVal) };
                    break;

                case 9: // By Type
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

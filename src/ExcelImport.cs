using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace DB_phase2_project
{
    public partial class ExcelImport : Form
    {
        string connection_string = DatabaseHelper.ConnectionString;
        private DataTable excelData;

        public ExcelImport()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Excel Data Import";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Import Users from Excel",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Browse Button
            Button btnBrowse = new Button
            {
                Text = "Browse Excel File",
                Location = new Point(30, 70),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            // File Path TextBox
            TextBox txtFilePath = new TextBox
            {
                Name = "txtFilePath",
                Location = new Point(200, 75),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };
            this.Controls.Add(txtFilePath);

            // DataGridView for preview
            DataGridView dgvPreview = new DataGridView
            {
                Name = "dgvPreview",
                Location = new Point(30, 120),
                Size = new Size(820, 350),
                BackgroundColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            dgvPreview.DefaultCellStyle.BackColor = Color.White;
            dgvPreview.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgvPreview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPreview.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvPreview);

            // Import Button
            Button btnImport = new Button
            {
                Name = "btnImport",
                Text = "Import to Database",
                Location = new Point(30, 490),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(46, 139, 87),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            btnImport.Click += BtnImport_Click;
            this.Controls.Add(btnImport);

            // Status Label
            Label lblStatus = new Label
            {
                Name = "lblStatus",
                Text = "Select an Excel file to preview data",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Location = new Point(230, 500),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(750, 490),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(178, 34, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Select Excel File with User Data"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                TextBox txtFilePath = (TextBox)this.Controls["txtFilePath"];
                txtFilePath.Text = filePath;

                try
                {
                    LoadExcelData(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Excel file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadExcelData(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            string connectionString;

            if (extension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            }
            else
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=YES;'";
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString();

                string query = $"SELECT * FROM [{sheetName}]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                excelData = new DataTable();
                adapter.Fill(excelData);
            }

            DataGridView dgvPreview = (DataGridView)this.Controls["dgvPreview"];
            dgvPreview.DataSource = excelData;

            Button btnImport = (Button)this.Controls["btnImport"];
            btnImport.Enabled = true;

            Label lblStatus = (Label)this.Controls["lblStatus"];
            lblStatus.Text = $"Loaded {excelData.Rows.Count} records. Ready to import.";
            lblStatus.ForeColor = Color.LightGreen;
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (excelData == null || excelData.Rows.Count == 0)
            {
                MessageBox.Show("No data to import.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Label lblStatus = (Label)this.Controls["lblStatus"];
            int successCount = 0;
            int errorCount = 0;

            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                conn.Open();

                foreach (DataRow row in excelData.Rows)
                {
                    try
                    {
                        string storedProcedure = "SP_Add_New_User";
                        using (SqlCommand cm = new SqlCommand(storedProcedure, conn))
                        {
                            cm.CommandType = CommandType.StoredProcedure;

                            // Map Excel columns to stored procedure parameters
                            // Expected columns: First_name, Last_name, DOB, Username, Password, email, Role, Experience, Specialization, Gym_ID, Membership_ID
                            cm.Parameters.AddWithValue("@First_name", GetColumnValue(row, "First_name", "First Name"));
                            cm.Parameters.AddWithValue("@Last_name", GetColumnValue(row, "Last_name", "Last Name"));
                            cm.Parameters.AddWithValue("@DOB", GetColumnValue(row, "DOB", "Date of Birth"));
                            cm.Parameters.AddWithValue("@Username", GetColumnValue(row, "Username", "User Name"));
                            cm.Parameters.AddWithValue("@Password", GetColumnValue(row, "Password"));
                            cm.Parameters.AddWithValue("@email", GetColumnValue(row, "email", "Email"));
                            cm.Parameters.AddWithValue("@Role", GetColumnValue(row, "Role"));
                            cm.Parameters.AddWithValue("@Experience", GetColumnValue(row, "Experience") ?? "");
                            cm.Parameters.AddWithValue("@Specialization", GetColumnValue(row, "Specialization", "Specialty") ?? "");
                            
                            object gymId = GetColumnValue(row, "Gym_ID", "GymID");
                            cm.Parameters.AddWithValue("@Gym_ID", gymId != null ? Convert.ToInt32(gymId) : (object)DBNull.Value);
                            
                            object memId = GetColumnValue(row, "Membership_ID", "MembershipID");
                            cm.Parameters.AddWithValue("@Membership_ID", memId != null ? Convert.ToInt32(memId) : (object)DBNull.Value);

                            cm.ExecuteNonQuery();
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        System.Diagnostics.Debug.WriteLine($"Error importing row: {ex.Message}");
                    }
                }
            }

            lblStatus.Text = $"Import complete! Success: {successCount}, Errors: {errorCount}";
            lblStatus.ForeColor = errorCount > 0 ? Color.Orange : Color.LightGreen;

            MessageBox.Show($"Import completed!\n\nSuccessfully imported: {successCount} users\nErrors: {errorCount}", 
                "Import Results", MessageBoxButtons.OK, 
                errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            Button btnImport = (Button)this.Controls["btnImport"];
            btnImport.Enabled = false;
        }

        private object GetColumnValue(DataRow row, params string[] columnNames)
        {
            foreach (string colName in columnNames)
            {
                if (excelData.Columns.Contains(colName) && row[colName] != DBNull.Value)
                {
                    return row[colName];
                }
            }
            return null;
        }
    }
}

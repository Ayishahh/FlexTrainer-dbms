using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FlexTrainer
{
    /// <summary>
    /// Centralized database connection helper class.
    /// Connection string is loaded from App.config.
    /// </summary>
    public static class DatabaseHelper
    {
        private static string _connectionString;
        private const string ServerPlaceholder = "YOUR_SERVER_IP";

        /// <summary>
        /// Gets the database connection string from App.config.
        /// Falls back to a default connection if config is not available.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    try
                    {
                        // Try to load from App.config
                        var connString = ConfigurationManager.ConnectionStrings["FlexTrainerDB"];
                        if (connString != null && !string.IsNullOrEmpty(connString.ConnectionString))
                        {
                            _connectionString = connString.ConnectionString;
                            System.Diagnostics.Debug.WriteLine("Connection string loaded from App.config");
                        }
                        else
                        {
                            // Default fallback for Docker setup
                            _connectionString = "Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True";
                            System.Diagnostics.Debug.WriteLine("Using default connection string (App.config not found)");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Fallback if ConfigurationManager fails
                        _connectionString = "Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True";
                        System.Diagnostics.Debug.WriteLine($"ConfigurationManager error: {ex.Message}. Using default connection string.");
                    }
                }
                
                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("Connection string could not be initialized. Check App.config or DatabaseHelper configuration.");
                }
                
                return _connectionString;
            }
        }

        public static void EnsureServerConfigured()
        {
            var connString = ConfigurationManager.ConnectionStrings["FlexTrainerDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connString))
            {
                return;
            }

            var builder = new SqlConnectionStringBuilder(connString);
            if (!NeedsServerConfiguration(builder))
            {
                _connectionString = builder.ConnectionString;
                return;
            }

            while (true)
            {
                var serverInput = PromptForServerIp(builder.DataSource);
                if (serverInput == null)
                {
                    throw new InvalidOperationException("Database server configuration was cancelled.");
                }

                if (string.IsNullOrWhiteSpace(serverInput))
                {
                    MessageBox.Show("Please enter a valid server IP or name.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                var dataSource = NormalizeDataSource(serverInput, builder.DataSource);
                builder.DataSource = dataSource;

                UpdateConnectionString(builder.ConnectionString);
                _connectionString = builder.ConnectionString;
                break;
            }
        }

        private static bool NeedsServerConfiguration(SqlConnectionStringBuilder builder)
        {
            return string.IsNullOrWhiteSpace(builder.DataSource) ||
                   builder.DataSource.Contains(ServerPlaceholder, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeDataSource(string input, string existingDataSource)
        {
            var dataSource = input.Trim();
            if (!dataSource.Contains(",") && !dataSource.Contains("\\"))
            {
                var commaIndex = existingDataSource.IndexOf(',');
                if (commaIndex > -1)
                {
                    dataSource = dataSource + existingDataSource.Substring(commaIndex);
                }
            }

            return dataSource;
        }

        private static void UpdateConnectionString(string connectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.ConnectionStrings.ConnectionStrings["FlexTrainerDB"];
            if (settings == null)
            {
                throw new InvalidOperationException("FlexTrainerDB connection string is missing in App.config.");
            }

            settings.ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private static string? PromptForServerIp(string currentValue)
        {
            using var form = new Form
            {
                Text = "Configure Database Server",
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new System.Drawing.Size(540, 180)
            };

            var label = new Label
            {
                Text = "Enter database server IP (optionally with port):",
                AutoSize = true,
                Location = new System.Drawing.Point(12, 12)
            };

            var textBox = new TextBox
            {
                Location = new System.Drawing.Point(15, 45),
                Size = new System.Drawing.Size(505, 27),
                Text = currentValue?.Contains(ServerPlaceholder, StringComparison.OrdinalIgnoreCase) == true
                    ? string.Empty
                    : currentValue
            };

            var okButton = new Button
            {
                Text = "Save",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(345, 110),
                Size = new System.Drawing.Size(85, 32)
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(435, 110),
                Size = new System.Drawing.Size(85, 32)
            };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        /// <summary>
        /// Creates and returns a new SqlConnection using the configured connection string.
        /// </summary>
        /// <returns>A new SqlConnection instance</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Tests the database connection.
        /// </summary>
        /// <returns>True if connection successful, false otherwise</returns>
        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Password hashing helper for secure password storage.
    /// Uses SHA256 hashing algorithm.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Base64 encoded hash</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="hash">Stored hash to compare against</param>
        /// <returns>True if password matches hash</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hash))
                return false;

            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }
    }

    /// <summary>
    /// Input validation helper for form validation.
    /// Provides methods for validating user inputs before database operations.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an email address format.
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if email format is valid</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates password strength (minimum 8 characters).
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password meets minimum requirements</returns>
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }

        /// <summary>
        /// Validates a name field (minimum 2 characters).
        /// </summary>
        /// <param name="name">Name to validate</param>
        /// <returns>True if name is valid</returns>
        public static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2;
        }

        /// <summary>
        /// Validates a positive number.
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <param name="value">Parsed decimal value</param>
        /// <returns>True if input is a positive number</returns>
        public static bool IsPositiveNumber(string input, out decimal value)
        {
            return decimal.TryParse(input, out value) && value > 0;
        }

        /// <summary>
        /// Validates a positive integer.
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <param name="value">Parsed integer value</param>
        /// <returns>True if input is a positive integer</returns>
        public static bool IsPositiveInteger(string input, out int value)
        {
            return int.TryParse(input, out value) && value > 0;
        }

        /// <summary>
        /// Validates a username (alphanumeric, underscore, 3-50 characters).
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <returns>True if username is valid</returns>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length < 3 || username.Length > 50)
                return false;

            // Allow alphanumeric and underscore
            return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        /// <summary>
        /// Validates a date is not in the future.
        /// </summary>
        /// <param name="date">Date to validate</param>
        /// <returns>True if date is not in the future</returns>
        public static bool IsNotFutureDate(DateTime date)
        {
            return date <= DateTime.Now;
        }

        /// <summary>
        /// Validates a date of birth (must be at least 13 years old).
        /// </summary>
        /// <param name="dob">Date of birth to validate</param>
        /// <returns>True if age is at least 13</returns>
        public static bool IsValidAge(DateTime dob)
        {
            int age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now < dob.AddYears(age))
                age--;

            return age >= 13;
        }
    }
}

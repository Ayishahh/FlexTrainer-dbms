using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace FlexTrainer
{
    /// <summary>
    /// Centralized database connection helper class.
    /// Connection string is loaded from App.config.
    /// </summary>
    public static class DatabaseHelper
    {
        private static string _connectionString;

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

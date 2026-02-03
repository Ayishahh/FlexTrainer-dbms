using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DB_phase2_project
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
                        if (connString != null)
                        {
                            _connectionString = connString.ConnectionString;
                        }
                        else
                        {
                            // Default fallback for Docker setup
                            _connectionString = "Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True";
                        }
                    }
                    catch
                    {
                        // Fallback if ConfigurationManager fails
                        _connectionString = "Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True";
                    }
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
}

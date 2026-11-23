using Microsoft.Data.SqlClient;

namespace StudentManagementAPI.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize(
            string? connectionString,
            ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is missing.");
            }

            using var connection =
                new SqlConnection(connectionString);

            connection.Open();

            string sql = @"
                IF NOT EXISTS (
                    SELECT * FROM sys.tables
                    WHERE name = 'Students'
                )
                BEGIN
                    CREATE TABLE Students (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(100) NOT NULL,
                        Email NVARCHAR(100) NOT NULL UNIQUE,
                        Course NVARCHAR(100) NOT NULL,
                        CreatedDate DATETIME DEFAULT GETUTCDATE(),
                        UpdatedDate DATETIME DEFAULT GETUTCDATE()
                    );
                END";

            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();

            logger.LogInformation(
                "Student database initialized successfully.");
        }
    }
}

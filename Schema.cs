IF NOT EXISTS (
    SELECT * FROM sys.databases
    WHERE name = 'StudentAPI'
)
BEGIN
    CREATE DATABASE StudentAPI;
END
GO

USE StudentAPI;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'Students'
)
BEGIN
    CREATE TABLE Students
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Course NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME DEFAULT GETUTCDATE()
    );

    CREATE NONCLUSTERED INDEX IX_Students_Email
    ON Students(Email);

    INSERT INTO Students (Name, Email, Course)
    VALUES
    ('John Doe', 'john.doe@example.com', 'Computer Science'),
    ('Jane Smith', 'jane.smith@example.com', 'Software Engineering'),
    ('Mike Johnson', 'mike.johnson@example.com', 'Data Science'),
    ('Sarah Williams', 'sarah.williams@example.com', 'Web Development');
END
GO

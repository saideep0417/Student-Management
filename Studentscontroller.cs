using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentsController(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not configured.");
        }

        // GET: api/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<Student>();

            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "SELECT Id, Name, Email, Course FROM Students",
                connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new Student
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Course = reader.GetString(3)
                });
            }

            return Ok(students);
        }

        // GET: api/students/1
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "SELECT Id, Name, Email, Course " +
                "FROM Students WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return NotFound("Student not found.");

            var student = new Student
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Course = reader.GetString(3)
            };

            return Ok(student);
        }

        // POST: api/students
        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"INSERT INTO Students (Name, Email, Course)
                  VALUES (@Name, @Email, @Course);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);",
                connection);

            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Email", student.Email);
            command.Parameters.AddWithValue("@Course", student.Course);

            connection.Open();

            student.Id = (int)command.ExecuteScalar()!;

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.Id },
                student);
        }

        // PUT: api/students/1
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"UPDATE Students
                  SET Name = @Name,
                      Email = @Email,
                      Course = @Course,
                      UpdatedDate = GETUTCDATE()
                  WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Email", student.Email);
            command.Parameters.AddWithValue("@Course", student.Course);

            connection.Open();

            int rows = command.ExecuteNonQuery();

            if (rows == 0)
                return NotFound("Student not found.");

            return Ok(new
            {
                Message = "Student updated successfully."
            });
        }

        // DELETE: api/students/1
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "DELETE FROM Students WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();

            int rows = command.ExecuteNonQuery();

            if (rows == 0)
                return NotFound("Student not found.");

            return Ok(new
            {
                Message = "Student deleted successfully."
            });
        }
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cumulative1.Models;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Cumulative1.Controllers
{
    /// <summary>
    /// API Controller for managing teacher-related operations including CRUD functionality
    /// and teacher-course relationships.
    /// </summary>
    public class TeacherAPIController : ApiController
    {
        // Database context for MySQL connection
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Retrieves a list of teachers with optional filtering capabilities
        /// </summary>
        /// <param name="SearchKey">Optional search term to filter by name, hire date or salary</param>
        /// <param name="MinHireDate">Optional minimum hire date for filtering</param>
        /// <param name="MaxHireDate">Optional maximum hire date for filtering</param>
        /// <returns>
        /// HTTP response containing:
        /// - 200 OK with list of teachers when successful
        /// - 400 Bad Request if parameters are invalid
        /// </returns>
        /// <example>
        /// GET: /api/TeacherPage/ListTeachers
        /// GET: /api/TeacherPage/ListTeachers/Smith
        /// GET: /api/TeacherPage/ListTeachers?MinHireDate=2015-01-01&MaxHireDate=2020-12-31
        /// </example>
        [HttpGet]
        [Route("api/TeacherPage/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null, DateTime? MinHireDate = null, DateTime? MaxHireDate = null)
        {
            // Establish database connection
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Initialize SQL command builder
            MySqlCommand cmd = Conn.CreateCommand();
            string query = "SELECT * FROM Teachers";
            bool hasWhere = false;

            // Build dynamic WHERE clause based on provided filters
            if (!string.IsNullOrEmpty(SearchKey))
            {
                query += " WHERE ";
                hasWhere = true;
                query += "(LOWER(teacherfname) LIKE LOWER(@Key) OR " +
                         "LOWER(teacherlname) LIKE LOWER(@Key) OR " +
                         "LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) OR " +
                         "hiredate LIKE @Key OR " +
                         "DATE_FORMAT(hiredate, '%d-%m-%Y') LIKE @Key OR " +
                         "salary LIKE @Key)";
                cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");
            }

            // Add date range filtering if provided
            if (MinHireDate.HasValue)
            {
                query += hasWhere ? " AND " : " WHERE ";
                query += "hiredate >= @MinHireDate";
                cmd.Parameters.AddWithValue("@MinHireDate", MinHireDate.Value);
                hasWhere = true;
            }

            if (MaxHireDate.HasValue)
            {
                query += hasWhere ? " AND " : " WHERE ";
                query += "hiredate <= @MaxHireDate";
                cmd.Parameters.AddWithValue("@MaxHireDate", MaxHireDate.Value);
            }

            // Execute the query
            cmd.CommandText = query;
            cmd.Prepare();

            // Process results
            MySqlDataReader ResultSet = cmd.ExecuteReader();
            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                Teachers.Add(new Teacher
                {
                    TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                    TeacherFname = ResultSet["teacherFname"].ToString(),
                    TeacherLname = ResultSet["teacherLname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                });
            }

            // Clean up resources
            Conn.Close();
            return Teachers;
        }

        /// <summary>
        /// Retrieves details for a specific teacher by ID
        /// </summary>
        /// <param name="id">The unique identifier of the teacher</param>
        /// <returns>
        /// HTTP response containing:
        /// - 200 OK with teacher details when found
        /// - 400 Bad Request for invalid ID
        /// - 404 Not Found if teacher doesn't exist
        /// </returns>
        /// <example>
        /// GET: /api/TeacherPage/FindTeacher/5
        /// </example>
        [HttpGet]
        [Route("api/TeacherPage/FindTeacher/{id}")]
        public IHttpActionResult FindTeacher(int id)
        {
            // Validate input
            if (id <= 0)
                return BadRequest("Teacher ID must be a positive integer");

            using (MySqlConnection Conn = School.AccessDatabase())
            {
                Conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Teachers WHERE teacherid = @id", Conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Prepare();

                    using (MySqlDataReader ResultSet = cmd.ExecuteReader())
                    {
                        if (!ResultSet.HasRows)
                            return NotFound();

                        ResultSet.Read();
                        return Ok(new Teacher
                        {
                            TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                            TeacherFname = ResultSet["teacherFname"].ToString(),
                            TeacherLname = ResultSet["teacherLname"].ToString(),
                            EmployeeNumber = ResultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                            Salary = Convert.ToDecimal(ResultSet["salary"])
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all courses taught by a specific teacher
        /// </summary>
        /// <param name="teacherId">The unique identifier of the teacher</param>
        /// <returns>
        /// HTTP response containing:
        /// - 200 OK with list of courses
        /// - 400 Bad Request for invalid teacher ID
        /// - 404 Not Found if teacher doesn't exist
        /// </returns>
        /// <example>
        /// GET: /api/TeacherPage/GetTeacherCourses/3
        /// </example>
        [HttpGet]
        [Route("api/TeacherPage/GetTeacherCourses/{teacherId}")]
        public IHttpActionResult GetTeacherCourses(int teacherId)
        {
            // Validate input
            if (teacherId <= 0)
                return BadRequest("Teacher ID must be a positive integer");

            using (MySqlConnection Conn = School.AccessDatabase())
            {
                Conn.Open();

                // Verify teacher exists first
                using (MySqlCommand teacherCmd = new MySqlCommand("SELECT 1 FROM Teachers WHERE teacherid = @id", Conn))
                {
                    teacherCmd.Parameters.AddWithValue("@id", teacherId);
                    teacherCmd.Prepare();

                    using (MySqlDataReader teacherResult = teacherCmd.ExecuteReader())
                    {
                        if (!teacherResult.HasRows)
                            return NotFound();
                    }
                }

                // Retrieve courses for the teacher
                List<Course> Courses = new List<Course>();
                using (MySqlCommand courseCmd = new MySqlCommand("SELECT * FROM Courses WHERE teacherid = @id", Conn))
                {
                    courseCmd.Parameters.AddWithValue("@id", teacherId);
                    courseCmd.Prepare();

                    using (MySqlDataReader CourseResultSet = courseCmd.ExecuteReader())
                    {
                        while (CourseResultSet.Read())
                        {
                            Courses.Add(new Course
                            {
                                CourseId = Convert.ToInt32(CourseResultSet["CourseId"]),
                                CourseCode = CourseResultSet["CourseCode"].ToString(),
                                CourseName = CourseResultSet["CourseName"].ToString()
                            });
                        }
                    }
                }

                return Ok(Courses);
            }
        }

        /// <summary>
        /// Adds a new teacher to the system with comprehensive validation
        /// </summary>
        /// <param name="newTeacher">Teacher object containing all required properties</param>
        /// <returns>
        /// HTTP response containing:
        /// - 200 OK with success message when created
        /// - 400 Bad Request with validation errors
        /// - 500 Internal Server Error for database issues
        /// </returns>
        /// <example>
        /// POST: /api/TeacherPage/AddTeacher
        /// Request Body: JSON representation of Teacher object
        /// </example>
        [HttpPost]
        [Route("api/TeacherPage/AddTeacher")]
        public IHttpActionResult AddTeacher([FromBody] Teacher newTeacher)
        {
            // Validate employee number format (T followed by digits)
            if (string.IsNullOrWhiteSpace(newTeacher.EmployeeNumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(newTeacher.EmployeeNumber, @"^T\d+$"))
            {
                return BadRequest("Employee number must start with 'T' followed by digits (e.g., T123)");
            }

            // Validate teacher name fields
            if (string.IsNullOrWhiteSpace(newTeacher.TeacherFname) ||
                string.IsNullOrWhiteSpace(newTeacher.TeacherLname))
            {
                return BadRequest("Both first and last name are required");
            }

            // Validate hire date is not in the future
            if (newTeacher.HireDate > DateTime.Today)
            {
                return BadRequest("Hire date cannot be in the future");
            }

            // Validate salary is positive
            if (newTeacher.Salary <= 0)
            {
                return BadRequest("Salary must be a positive value");
            }

            try
            {
                using (MySqlConnection Conn = School.AccessDatabase())
                {
                    Conn.Open();

                    // Check for duplicate employee number
                    using (MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM teachers WHERE employeenumber = @empnum", Conn))
                    {
                        checkCmd.Parameters.AddWithValue("@empnum", newTeacher.EmployeeNumber);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            return BadRequest("Employee number already exists");
                        }
                    }

                    // Insert new teacher record
                    using (MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                        "VALUES (@fname, @lname, @empnum, @hiredate, @salary);", Conn))
                    {
                        cmd.Parameters.AddWithValue("@fname", newTeacher.TeacherFname);
                        cmd.Parameters.AddWithValue("@lname", newTeacher.TeacherLname);
                        cmd.Parameters.AddWithValue("@empnum", newTeacher.EmployeeNumber);
                        cmd.Parameters.AddWithValue("@hiredate", newTeacher.HireDate);
                        cmd.Parameters.AddWithValue("@salary", newTeacher.Salary);

                        cmd.Prepare();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return InternalServerError(new Exception("Failed to add teacher"));
                        }

                        // Retrieve the new teacher ID
                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        newTeacher.TeacherId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                return Ok(new
                {
                    Message = "Teacher added successfully",
                    TeacherId = newTeacher.TeacherId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes a teacher from the system after validation checks
        /// </summary>
        /// <param name="id">The ID of the teacher to delete</param>
        /// <returns>
        /// HTTP response containing:
        /// - 200 OK with success message when deleted
        /// - 400 Bad Request for invalid ID
        /// - 404 Not Found if teacher doesn't exist
        /// - 500 Internal Server Error for database issues
        /// </returns>
        /// <example>
        /// DELETE: /api/TeacherPage/DeleteTeacher/5
        /// </example>
        [HttpDelete]
        [Route("api/TeacherPage/DeleteTeacher/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            // Validate ID
            if (id <= 0)
                return BadRequest("Teacher ID must be a positive integer");

            try
            {
                using (MySqlConnection Conn = School.AccessDatabase())
                {
                    Conn.Open();

                    // Verify teacher exists first
                    using (MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT teacherid FROM teachers WHERE teacherid = @id", Conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);

                        if (checkCmd.ExecuteScalar() == null)
                        {
                            return NotFound();
                        }
                    }

                    // Delete the teacher record
                    using (MySqlCommand deleteCmd = new MySqlCommand(
                        "DELETE FROM teachers WHERE teacherid = @id", Conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        deleteCmd.Prepare();
                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return InternalServerError(new Exception("Failed to delete teacher"));
                        }
                    }
                }

                return Ok($"Teacher {id} deleted successfully");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

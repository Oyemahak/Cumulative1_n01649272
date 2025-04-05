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
    public class TeacherAPIController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        [HttpGet]
        [Route("api/TeacherPage/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null, DateTime? MinHireDate = null, DateTime? MaxHireDate = null)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();
            string query = "SELECT * FROM Teachers";
            bool hasWhere = false;

            // Build WHERE clause if any conditions exist
            if (!string.IsNullOrEmpty(SearchKey) || MinHireDate.HasValue || MaxHireDate.HasValue)
            {
                query += " WHERE ";
                hasWhere = true;
            }

            // Add search key condition
            if (!string.IsNullOrEmpty(SearchKey))
            {
                query += "(LOWER(teacherfname) LIKE LOWER(@Key) OR " +
                         "LOWER(teacherlname) LIKE LOWER(@Key) OR " +
                         "LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) OR " +
                         "hiredate LIKE @Key OR " +
                         "DATE_FORMAT(hiredate, '%d-%m-%Y') LIKE @Key OR " +
                         "salary LIKE @Key)";
                cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");
            }

            // Add date range conditions
            if (MinHireDate.HasValue)
            {
                if (hasWhere && !string.IsNullOrEmpty(SearchKey))
                    query += " AND ";
                query += "hiredate >= @MinHireDate";
                cmd.Parameters.AddWithValue("@MinHireDate", MinHireDate.Value);
                hasWhere = true;
            }

            if (MaxHireDate.HasValue)
            {
                if (hasWhere && (!string.IsNullOrEmpty(SearchKey) || MinHireDate.HasValue))
                    query += " AND ";
                query += "hiredate <= @MaxHireDate";
                cmd.Parameters.AddWithValue("@MaxHireDate", MaxHireDate.Value);
            }

            cmd.CommandText = query;
            cmd.Prepare();

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

            Conn.Close();
            return Teachers;
        }

        [HttpGet]
        [Route("api/TeacherPage/FindTeacher/{id}")]
        public IHttpActionResult FindTeacher(int id)
        {
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

        [HttpGet]
        [Route("api/TeacherPage/GetTeacherCourses/{teacherId}")]
        public IHttpActionResult GetTeacherCourses(int teacherId)
        {
            if (teacherId <= 0)
                return BadRequest("Teacher ID must be a positive integer");

            using (MySqlConnection Conn = School.AccessDatabase())
            {
                Conn.Open();

                // Verify teacher exists
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

                // Get courses
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

        // Add these new methods to your existing TeacherAPIController.cs

        /// <summary>
        /// Adds a new teacher to the system
        /// </summary>
        [HttpPost]
        [Route("api/TeacherPage/AddTeacher")]
        public IHttpActionResult AddTeacher([FromBody] Teacher newTeacher)
        {
            // =================================================================
            // VALIDATION 1: Ensure Employee Number follows format "T" + digits
            // - Uses regex pattern ^T\d+$:
            //   - ^T     : Must start with "T"
            //   - \d+    : Followed by one or more digits
            //   - $      : No extra characters allowed
            // - Returns 400 Bad Request if format is invalid
            // =================================================================

            if (string.IsNullOrWhiteSpace(newTeacher.EmployeeNumber) ||
                string.IsNullOrWhiteSpace(newTeacher.EmployeeNumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(newTeacher.EmployeeNumber, @"^T\d+$"))
            {
                return BadRequest("Employee number cannot be empty");
            }

            // =================================================================
            // VALIDATION 2: Teacher name cannot be empty
            // =================================================================

            if (string.IsNullOrWhiteSpace(newTeacher.TeacherFname) ||
                string.IsNullOrWhiteSpace(newTeacher.TeacherLname))
            {
                return BadRequest("Teacher first and last name cannot be empty");
            }

            // =================================================================
            // VALIDATION 3: Hire date cannot be in the future
            // =================================================================

            if (newTeacher.HireDate > DateTime.Today)
            {
                return BadRequest("Hire date cannot be in the future");
            }

            using (MySqlConnection Conn = School.AccessDatabase())
            {
                Conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                    "VALUES (@fname, @lname, @empnum, @hiredate, @salary);", Conn);

                cmd.Parameters.AddWithValue("@fname", newTeacher.TeacherFname);
                cmd.Parameters.AddWithValue("@lname", newTeacher.TeacherLname);
                cmd.Parameters.AddWithValue("@empnum", newTeacher.EmployeeNumber);
                cmd.Parameters.AddWithValue("@hiredate", newTeacher.HireDate);
                cmd.Parameters.AddWithValue("@salary", newTeacher.Salary);

                cmd.Prepare();
                cmd.ExecuteNonQuery();

                // Get the ID of the newly inserted teacher
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                newTeacher.TeacherId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return Ok("Teacher Added Successfully!");
        }

        /// <summary>
        /// Deletes a teacher from the system
        /// </summary>
        [HttpDelete]
        [Route("api/TeacherPage/DeleteTeacher/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            // Initiative #1: Validate ID
            if (id <= 0) return BadRequest("Invalid Teacher ID");

            using (MySqlConnection Conn = School.AccessDatabase())
            {
                Conn.Open();

                // Check if teacher exists first
                MySqlCommand checkCmd = new MySqlCommand(
                    "SELECT teacherid FROM teachers WHERE teacherid = @id", Conn);
                checkCmd.Parameters.AddWithValue("@id", id);

                if (checkCmd.ExecuteScalar() == null)
                {
                    return NotFound();
                }

                // Delete the teacher
                MySqlCommand deleteCmd = new MySqlCommand(
                    "DELETE FROM teachers WHERE teacherid = @id", Conn);
                deleteCmd.Parameters.AddWithValue("@id", id);
                deleteCmd.Prepare();
                deleteCmd.ExecuteNonQuery();
            }

            return Ok($"Teacher {id} deleted successfully");
        }
    }
}
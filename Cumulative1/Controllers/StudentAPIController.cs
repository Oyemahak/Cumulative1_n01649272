using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cumulative1.Models;
using MySql.Data.MySqlClient;

namespace Cumulative1.Controllers
{
    /// <summary>
    /// API Controller for managing student-related operations including CRUD functionality
    /// </summary>
    public class StudentAPIController : ApiController
    {
        // Database context for MySQL connection
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Retrieves a list of all students
        /// </summary>
        /// <returns>List of Student objects</returns>
        [HttpGet]
        [Route("api/StudentPage/ListStudents")]
        public HttpResponseMessage ListStudents()
        {
            try
            {
                List<Student> Students = new List<Student>();
                MySqlConnection Conn = School.AccessDatabase();
                Conn.Open();

                MySqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM students";

                MySqlDataReader ResultSet = cmd.ExecuteReader();

                while (ResultSet.Read())
                {
                    Student NewStudent = new Student
                    {
                        StudentId = Convert.ToInt32(ResultSet["studentid"]),
                        StudentFname = ResultSet["studentfname"].ToString(),
                        StudentLname = ResultSet["studentlname"].ToString(),
                        StudentNumber = ResultSet["studentnumber"].ToString(),
                        EnrolDate = Convert.ToDateTime(ResultSet["enroldate"])
                    };
                    Students.Add(NewStudent);
                }

                Conn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, Students);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Retrieves details for a specific student by ID
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>Student object</returns>
        [HttpGet]
        [Route("api/StudentPage/FindStudent/{id}")]
        public HttpResponseMessage FindStudent(int id)
        {
            if (id <= 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Student ID must be a positive integer");
            }

            try
            {
                Student NewStudent = new Student();
                MySqlConnection Conn = School.AccessDatabase();
                Conn.Open();

                MySqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM students WHERE studentid = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();

                MySqlDataReader ResultSet = cmd.ExecuteReader();

                if (!ResultSet.HasRows)
                {
                    Conn.Close();
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found");
                }

                while (ResultSet.Read())
                {
                    NewStudent.StudentId = Convert.ToInt32(ResultSet["studentid"]);
                    NewStudent.StudentFname = ResultSet["studentfname"].ToString();
                    NewStudent.StudentLname = ResultSet["studentlname"].ToString();
                    NewStudent.StudentNumber = ResultSet["studentnumber"].ToString();
                    NewStudent.EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);
                }

                Conn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, NewStudent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/StudentPage/AddStudent")]
        public HttpResponseMessage AddStudent([FromBody] Student newStudent)
        {
            try
            {
                MySqlConnection Conn = School.AccessDatabase();
                Conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO students (studentfname, studentlname, studentnumber, enroldate) " +
                    "VALUES (@fname, @lname, @number, @enrol)", Conn);

                cmd.Parameters.AddWithValue("@fname", newStudent.StudentFname);
                cmd.Parameters.AddWithValue("@lname", newStudent.StudentLname);
                cmd.Parameters.AddWithValue("@number", newStudent.StudentNumber);
                cmd.Parameters.AddWithValue("@enrol", newStudent.EnrolDate);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to add student");
                }

                // Get the new ID
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                newStudent.StudentId = Convert.ToInt32(cmd.ExecuteScalar());

                Conn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, newStudent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/StudentPage/UpdateStudent/{id}")]
        public HttpResponseMessage UpdateStudent(int id, [FromBody] Student studentInfo)
        {
            if (id <= 0 || id != studentInfo.StudentId)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Student ID");

            try
            {
                MySqlConnection Conn = School.AccessDatabase();
                Conn.Open();

                // Check if student exists
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM students WHERE studentid = @id", Conn);
                checkCmd.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found");

                // Update student
                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE students SET studentfname=@fname, studentlname=@lname, " +
                    "studentnumber=@number, enroldate=@enrol WHERE studentid=@id", Conn);

                cmd.Parameters.AddWithValue("@fname", studentInfo.StudentFname);
                cmd.Parameters.AddWithValue("@lname", studentInfo.StudentLname);
                cmd.Parameters.AddWithValue("@number", studentInfo.StudentNumber);
                cmd.Parameters.AddWithValue("@enrol", studentInfo.EnrolDate);
                cmd.Parameters.AddWithValue("@id", id);

                if (cmd.ExecuteNonQuery() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Update failed");

                Conn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, "Student updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/StudentPage/DeleteStudent/{id}")]
        public HttpResponseMessage DeleteStudent(int id)
        {
            if (id <= 0)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Student ID");

            try
            {
                MySqlConnection Conn = School.AccessDatabase();
                Conn.Open();

                // Check if student exists
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM students WHERE studentid = @id", Conn);
                checkCmd.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found");

                // Delete student
                MySqlCommand cmd = new MySqlCommand("DELETE FROM students WHERE studentid = @id", Conn);
                cmd.Parameters.AddWithValue("@id", id);

                if (cmd.ExecuteNonQuery() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Delete failed");

                Conn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, "Student deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
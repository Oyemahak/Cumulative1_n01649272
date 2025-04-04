using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cumulative1.Models;
using MySql.Data.MySqlClient;

namespace Cumulative1.Controllers
{
    public class StudentAPIController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        // GET: api/StudentPage/ListStudents
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

        // GET: api/StudentPage/FindStudent/5
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
    }
}
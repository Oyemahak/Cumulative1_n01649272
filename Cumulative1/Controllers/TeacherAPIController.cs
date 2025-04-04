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
    }
}
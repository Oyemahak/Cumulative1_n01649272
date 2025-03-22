using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cumulative1.Models;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;
using Mysqlx.Datatypes;

namespace Cumulative1.Controllers
{
    public class TeacherAPIController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of teachers in the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter teachers by first name, last name, full name, hire date, or salary.</param>
        /// <returns>
        /// A list of teacher objects.
        /// Each teacher object contains the following properties:
        /// - TeacherId (int): The unique identifier of the teacher.
        /// - TeacherFname (string): The first name of the teacher.
        /// - TeacherLname (string): The last name of the teacher.
        /// - EmployeeNumber (string): The employee number of the teacher.
        /// - HireDate (DateTime): The date when the teacher was hired.
        /// - Salary (decimal): The salary of the teacher.
        /// </returns>
        /// <example>
        /// Example of GET request:
        /// GET api/TeacherPage/ListTeachers?SearchKey=Mahak
        /// GET api/TeacherPage/ListTeachers?SearchKey=06-05
        /// GET api/TeacherPage/ListTeachers?SearchKey=66
        /// </example>
        [HttpGet]
        [Route("api/TeacherPage/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query with optional search key
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");

            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create a list to hold teacher objects
            List<Teacher> Teachers = new List<Teacher>();

            // Loop through each row in the result set
            while (ResultSet.Read())
            {
                // Retrieve column information
                int TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                string TeacherFname = ResultSet["teacherFname"].ToString();
                string TeacherLname = ResultSet["teacherLname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                // Create a new Teacher object and populate its properties
                Teacher NewTeacher = new Teacher
                {
                    TeacherId = TeacherId,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                };

                // Add the teacher to the list
                Teachers.Add(NewTeacher);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of teachers
            return Teachers;
        }

        /// <summary>
        /// Finds a teacher in the system given an ID and retrieves associated classes.
        /// </summary>
        /// <param name="id">The teacher's primary key.</param>
        /// <returns>
        /// A teacher object with associated classes.
        /// The teacher object contains the following properties:
        /// - TeacherId (int): The unique identifier of the teacher.
        /// - TeacherFname (string): The first name of the teacher.
        /// - TeacherLname (string): The last name of the teacher.
        /// - EmployeeNumber (string): The employee number of the teacher.
        /// - HireDate (DateTime): The date when the teacher was hired.
        /// - Salary (decimal): The salary of the teacher.
        /// - Courses (List<Class>): A list of class objects associated with the teacher.
        /// </returns>
        /// <example>
        /// Example of GET request:
        /// GET api/TeacherPage/FindTeacher/12
        /// </example>
        [HttpGet]
        [Route("api/TeacherPage/FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // Create a new Teacher object
            Teacher NewTeacher = new Teacher();

            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query to retrieve teacher information
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Populate the teacher object with information from the result set
            while (ResultSet.Read())
            {
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                NewTeacher.TeacherFname = ResultSet["teacherFname"].ToString();
                NewTeacher.TeacherLname = ResultSet["teacherLname"].ToString();
                NewTeacher.EmployeeNumber = ResultSet["employeenumber"].ToString();
                NewTeacher.HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                NewTeacher.Salary = Convert.ToDecimal(ResultSet["salary"]);
            }
            ResultSet.Close(); // Close the result set

            // Prepare SQL query to retrieve classes associated with the teacher
            MySqlCommand courseCmd = Conn.CreateCommand();
            courseCmd.CommandText = "SELECT * FROM Courses WHERE teacherid = @id";
            courseCmd.Parameters.AddWithValue("@id", id);
            courseCmd.Prepare();

            // Execute the query
            MySqlDataReader CourseResultSet = courseCmd.ExecuteReader();

            // Create a list to hold course objects
            List<Course> Courses = new List<Course>();

            // Loop through each row in the course result set
            while (CourseResultSet.Read())
            {
                // Retrieve column information
                int CourseId = Convert.ToInt32(CourseResultSet["CourseId"]);
                string CourseCode = CourseResultSet["CourseCode"].ToString();
                string CourseName = CourseResultSet["CourseName"].ToString();

                // Create a new Course object and populate its properties
                //note: Id needed for a link to course page. 
                Course NewCourse = new Course
                {
                    CourseId = CourseId,
                    CourseCode = CourseCode,
                    CourseName = CourseName
                };

                // Add the course to the list
                Courses.Add(NewCourse);
            }

            // Add the list of coursees to the teacher object
            NewTeacher.Courses = Courses;

            // Close the database connection
            Conn.Close();

            // Return the teacher object
            return NewTeacher;
        }
    }
}

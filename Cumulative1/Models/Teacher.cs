using System;
using System.Collections.Generic;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a teacher in the school system
    /// </summary>
    public class Teacher
    {
        // The unique identifier for the teacher
        public int TeacherId { get; set; }

        // The first name of the teacher
        public string TeacherFname { get; set; }

        // The last name of the teacher
        public string TeacherLname { get; set; }

        // The employee number (format: T followed by digits)
        public string EmployeeNumber { get; set; }

        // The date when the teacher was hired
        public DateTime HireDate { get; set; }

        // The teacher's salary
        public decimal Salary { get; set; }

        // List of courses taught by this teacher
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
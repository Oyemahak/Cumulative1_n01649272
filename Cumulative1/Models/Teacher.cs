using System;
using System.Collections.Generic;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a teacher in the school system.
    /// </summary>
    public class Teacher
    {
        // Properties of the Teacher entity
        public int TeacherId;
        public string TeacherFname;
        public string TeacherLname;
        public string EmployeeNumber;
        public DateTime HireDate;
        public decimal Salary;
        public List<Course> Courses;
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cumulative1.Models
{
    [DataContract]
    public class Teacher
    {
        [DataMember]
        public int TeacherId { get; set; }

        [DataMember]
        public string TeacherFname { get; set; }

        [DataMember]
        public string TeacherLname { get; set; }

        [DataMember]
        public string EmployeeNumber { get; set; }

        [DataMember]
        public DateTime HireDate { get; set; }

        [DataMember]
        public decimal Salary { get; set; }

        [DataMember]
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
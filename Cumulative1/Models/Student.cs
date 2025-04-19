using System;
using System.Runtime.Serialization;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a student in the school system
    /// </summary>
    [DataContract]
    public class Student
    {
        [DataMember]
        public int StudentId { get; set; }

        [DataMember]
        public string StudentFname { get; set; }

        [DataMember]
        public string StudentLname { get; set; }

        [DataMember]
        public string StudentNumber { get; set; }

        [DataMember]
        public DateTime EnrolDate { get; set; }
    }
}
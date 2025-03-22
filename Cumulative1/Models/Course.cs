using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a Course in the school system.
    /// </summary>
    public class Course
    {
        // Properties of the Course entity
        public int CourseId;
        public string CourseCode;
        public int TeacherId;
        public DateTime StartDate;
        public DateTime FinishDate;
        public string CourseName;
    }
}
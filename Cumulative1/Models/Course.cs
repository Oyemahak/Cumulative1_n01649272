using System;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a course in the school system
    /// </summary>
    public class Course
    {
        // The unique identifier for the course
        public int CourseId { get; set; }

        // The course code (e.g., COMP1001)
        public string CourseCode { get; set; }

        // The name of the course
        public string CourseName { get; set; }

        // The ID of the teacher who teaches this course
        public int? TeacherId { get; set; }
    }
}
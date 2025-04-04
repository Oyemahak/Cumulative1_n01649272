using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        /// <summary>
        /// Displays a list of teachers optionally filtered by search key or hire date range.
        /// </summary>
        /// <param name="SearchKey">The search key to filter teachers.</param>
        /// <param name="MinHireDate">Minimum hire date (format: yyyy-MM-dd)</param>
        /// <param name="MaxHireDate">Maximum hire date (format: yyyy-MM-dd)</param>
        /// <returns>The view displaying the list of teachers.</returns>
        /// <example>
        /// GET: /Teacher/List
        /// GET: /Teacher/List?SearchKey=John
        /// GET: /Teacher/List?MinHireDate=2000-01-01&MaxHireDate=2010-12-31
        /// </example>
        public ActionResult List(string SearchKey = null, string MinHireDate = null, string MaxHireDate = null)
        {
            TeacherAPIController controller = new TeacherAPIController();

            // Parse date parameters if provided
            DateTime? minDate = null;
            DateTime? maxDate = null;

            if (!string.IsNullOrEmpty(MinHireDate) && DateTime.TryParse(MinHireDate, out DateTime parsedMinDate))
            {
                minDate = parsedMinDate;
            }

            if (!string.IsNullOrEmpty(MaxHireDate) && DateTime.TryParse(MaxHireDate, out DateTime parsedMaxDate))
            {
                maxDate = parsedMaxDate;
            }

            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey, minDate, maxDate);
            return View(Teachers);
        }

        /// <summary>
        /// Displays details of a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>
        /// The view displaying the details of the teacher.
        /// Returns error view if teacher is not found.
        /// </returns>
        /// <example>
        /// GET: /Teacher/Show/12
        /// </example>
        public ActionResult Show(int id)
        {
            // Validate ID
            if (id <= 0)
            {
                return View("Error", new HandleErrorInfo(
                    new ArgumentException("Invalid Teacher ID - must be positive number"),
                    "TeacherPage",
                    "Show"));
            }

            TeacherAPIController controller = new TeacherAPIController();

            // Get teacher details
            var teacherResponse = controller.FindTeacher(id);

            // Check if teacher was found
            if (teacherResponse is System.Web.Http.Results.NotFoundResult)
            {
                return View("Error", new HandleErrorInfo(
                    new KeyNotFoundException($"Teacher with ID {id} not found"),
                    "TeacherPage",
                    "Show"));
            }

            // Get the teacher object from the response
            var teacherContent = teacherResponse as System.Web.Http.Results.OkNegotiatedContentResult<Teacher>;
            Teacher teacher = teacherContent.Content;

            // Get courses for this teacher
            var coursesResponse = controller.GetTeacherCourses(id);
            if (coursesResponse is System.Web.Http.Results.OkNegotiatedContentResult<List<Course>>)
            {
                var coursesContent = coursesResponse as System.Web.Http.Results.OkNegotiatedContentResult<List<Course>>;
                teacher.Courses = coursesContent.Content;
            }

            return View(teacher);
        }
    }
}
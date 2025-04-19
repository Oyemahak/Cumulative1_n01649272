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

        // ============== NEW METHODS FOR PART 2 ==============
        public ActionResult New()
        {
            return View();
        }

        public ActionResult DeleteConfirm(int id)
        {
            TeacherAPIController api = new TeacherAPIController();
            var result = api.FindTeacher(id);

            if (result is System.Web.Http.Results.NotFoundResult)
            {
                return View("Error", new HandleErrorInfo(
                    new Exception($"Teacher with ID {id} not found"),
                    "TeacherPage",
                    "DeleteConfirm"));
            }

            var contentResult = result as System.Web.Http.Results.OkNegotiatedContentResult<Teacher>;
            return View(contentResult.Content);
        }

        // ============== PART 3: UPDATE FUNCTIONALITY ==============

        /// <summary>
        /// Displays the edit form for a specific teacher
        /// </summary>
        /// <param name="id">The ID of the teacher to edit</param>
        /// <returns>
        /// The edit view pre-populated with teacher data
        /// Returns error view if teacher is not found
        /// </returns>
        /// <example>
        /// GET: /Teacher/Edit/5
        /// </example>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Validate ID
            if (id <= 0)
            {
                return View("Error", new HandleErrorInfo(
                    new ArgumentException("Invalid Teacher ID - must be positive number"),
                    "TeacherPage",
                    "Edit"));
            }

            TeacherAPIController api = new TeacherAPIController();
            var result = api.FindTeacher(id);

            // Check if teacher was found
            if (result is System.Web.Http.Results.NotFoundResult)
            {
                return View("Error", new HandleErrorInfo(
                    new KeyNotFoundException($"Teacher with ID {id} not found"),
                    "TeacherPage",
                    "Edit"));
            }

            // Get the teacher object from the response
            var contentResult = result as System.Web.Http.Results.OkNegotiatedContentResult<Teacher>;
            Teacher teacher = contentResult.Content;

            return View(teacher);
        }

        /// <summary>
        /// Processes the submitted edit form (AJAX)
        /// </summary>
        /// <param name="id">The ID of the teacher being updated</param>
        /// <param name="TeacherInfo">The updated teacher information</param>
        /// <returns>
        /// JSON result indicating success or failure
        /// </returns>
        /// <example>
        /// POST: /Teacher/Update/5
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, Teacher TeacherInfo)
        {
            // This method is primarily for server-side validation in case JS is disabled
            // The actual update will be handled via AJAX from the client side

            if (id <= 0 || id != TeacherInfo.TeacherId)
            {
                return Json(new { success = false, message = "Invalid Teacher ID" });
            }

            TeacherAPIController api = new TeacherAPIController();
            var result = api.UpdateTeacher(id, TeacherInfo);

            if (result is System.Web.Http.Results.BadRequestErrorMessageResult badRequest)
            {
                return Json(new { success = false, message = badRequest.Message });
            }
            else if (result is System.Web.Http.Results.NotFoundResult)
            {
                return Json(new { success = false, message = "Teacher not found" });
            }
            else if (result is System.Web.Http.Results.InternalServerErrorResult)
            {
                return Json(new { success = false, message = "Server error occurred" });
            }

            return Json(new { success = true, message = "Teacher updated successfully" });
        }

    }
}
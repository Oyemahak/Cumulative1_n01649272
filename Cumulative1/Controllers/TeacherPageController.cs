using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        /// <summary>
        /// Displays a list of teachers optionally filtered by search key.
        /// </summary>
        /// <param name="SearchKey">The search key to filter teachers.</param>
        /// <returns>The view displaying the list of teachers.</returns>
        /// <example>
        /// GET: /Teacher/List
        /// </example>
        public ActionResult List(string SearchKey = null)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        /// <summary>
        /// Displays details of a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>The view displaying the details of the teacher.</returns>
        /// <example>
        /// GET: /Teacher/Show/{id}
        /// </example>
        public ActionResult Show(int id)
        {
            TeacherAPIController controller = new TeacherAPIController();
            Teacher NewTeacher = controller.FindTeacher(id);

            return View(NewTeacher);
        }
    }
}

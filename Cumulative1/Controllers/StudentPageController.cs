using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class StudentPageController : Controller
    {
        // GET: Student/List
        public ActionResult List()
        {
            try
            {
                StudentAPIController controller = new StudentAPIController();
                var response = controller.ListStudents();

                if (response.IsSuccessStatusCode)
                {
                    var students = response.Content.ReadAsAsync<IEnumerable<Student>>().Result;
                    return View(students);
                }

                ViewBag.ErrorMessage = "Error loading students: " + response.ReasonPhrase;
                return View(new List<Student>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(new List<Student>());
            }
        }

        // GET: Student/Show/5
        public ActionResult Show(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Error", new { message = "Invalid Student ID" });
            }

            try
            {
                StudentAPIController controller = new StudentAPIController();
                var response = controller.FindStudent(id);

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Error", new
                    {
                        message = response.StatusCode == HttpStatusCode.NotFound ?
                        "Student not found" : "Error loading student information"
                    });
                }

                var student = response.Content.ReadAsAsync<Student>().Result;
                if (student == null)
                {
                    return RedirectToAction("Error", new { message = "Student data could not be loaded" });
                }

                return View(student);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", new { message = "An error occurred" });
            }
        }

        // GET: Student/Error
        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
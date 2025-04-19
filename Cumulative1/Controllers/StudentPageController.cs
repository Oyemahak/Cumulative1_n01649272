using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class StudentPageController : Controller
    {
        private HttpClient client = new HttpClient();

        public StudentPageController()
        {
            client.BaseAddress = new Uri("https://localhost:44394/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: StudentPage/List
        public ActionResult List()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync("api/StudentPage/ListStudents").Result;
                if (response.IsSuccessStatusCode)
                {
                    var students = response.Content.ReadAsAsync<List<Student>>().Result;
                    return View(students);
                }
                else
                {
                    ViewBag.ErrorMessage = "Error: " + response.ReasonPhrase;
                    return View(new List<Student>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
                return View(new List<Student>());
            }
        }

        // GET: StudentPage/Show/5
        public ActionResult Show(int id)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"api/StudentPage/FindStudent/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var student = response.Content.ReadAsAsync<Student>().Result;
                    return View(student);
                }
                else
                {
                    return RedirectToAction("Error", new
                    {
                        message = response.StatusCode == HttpStatusCode.NotFound ?
                        "Student not found" : "Error loading student"
                    });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }

        // GET: StudentPage/New
        public ActionResult New()
        {
            return View();
        }

        // GET: StudentPage/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"api/StudentPage/FindStudent/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var student = response.Content.ReadAsAsync<Student>().Result;
                    return View(student);
                }
                else
                {
                    return View("Error", new HandleErrorInfo(
                        new Exception(response.ReasonPhrase),
                        "StudentPage",
                        "Edit"));
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "StudentPage", "Edit"));
            }
        }

        // GET: StudentPage/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"api/StudentPage/FindStudent/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var student = response.Content.ReadAsAsync<Student>().Result;
                    return View(student);
                }
                else
                {
                    return View("Error", new HandleErrorInfo(
                        new Exception(response.ReasonPhrase),
                        "StudentPage",
                        "DeleteConfirm"));
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "StudentPage", "DeleteConfirm"));
            }
        }

        // POST: StudentPage/Create
        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync("api/StudentPage/AddStudent", student).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    ViewBag.ErrorMessage = "Error creating student: " + response.ReasonPhrase;
                    return View("New", student);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
                return View("New", student);
            }
        }

        // POST: StudentPage/Update/5
        [HttpPost]
        public ActionResult Update(int id, Student student)
        {
            try
            {
                HttpResponseMessage response = client.PutAsJsonAsync($"api/StudentPage/UpdateStudent/{id}", student).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    ViewBag.ErrorMessage = "Error updating student: " + response.ReasonPhrase;
                    return View("Edit", student);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
                return View("Edit", student);
            }
        }

        // POST: StudentPage/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync($"api/StudentPage/DeleteStudent/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return RedirectToAction("Error", new { message = "Error deleting student: " + response.ReasonPhrase });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = "Exception: " + ex.Message });
            }
        }

        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
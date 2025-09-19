using Microsoft.AspNetCore.Mvc;
using QBCA.Models;
using System.Collections.Generic;

namespace CNPM_QBCA.Web.Controllers
{
    public class LecturerAssignmentController : Controller
    {
        private static List<LecturerAssignment> _lecturerTasks = new List<LecturerAssignment>();

        // GET: /Exam/AssignToLecturers
        public IActionResult AssignToLecturers()
        {
            return View(_lecturerTasks);
        }

        // GET: /Exam/CreateLecturerTask
        public IActionResult CreateLecturerTask()
        {
            return View();
        }

        // POST: /Exam/CreateLecturerTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLecturerTask(LecturerAssignment task)
        {
            if (ModelState.IsValid)
            {
                task.AssignmentID = _lecturerTasks.Count + 1;
                task.CreatedAt = DateTime.Now;
                _lecturerTasks.Add(task);
                return RedirectToAction("AssignToLecturers");
            }

            return View(task);
        }
        public IActionResult Create()
        {
            return View(); // Views/LecturerAssignment/Create.cshtml
        }

        // POST: /LecturerAssignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LecturerAssignment model)
        {
            if (ModelState.IsValid)
            {
               
                return RedirectToAction("Index"); // hoặc "AssignToLecturers"
            }

            return View(model);
        }
    }
}

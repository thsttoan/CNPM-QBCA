using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QBCA.Models;
using QBCA.Data;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace QBCA.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Subject/Subjects
        public IActionResult Subjects()
        {
            var subjects = _context.Subjects.ToList();
            return View(subjects);
        }

        // GET: /Subject/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.Status = "Active";
                // Assuming you have a claim for UserID
                var userIdClaim = User.FindFirst("UserID")?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                    model.CreatedBy = userId;

                _context.Subjects.Add(model);
                await _context.SaveChangesAsync();

                // Notification logic here (optional)

                TempData["Success"] = "Subject created successfully!";
                return RedirectToAction("Subjects");
            }
            return View(model);
        }
    }
}
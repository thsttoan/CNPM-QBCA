using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QBCA.Models;
using QBCA.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

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

        private void AddNotification(int userId, string message, string relatedType, int? relatedId, int createdBy)
        {
            var noti = new Notification
            {
                UserID = userId,
                Message = message,
                Status = "unread",
                RelatedEntityType = relatedType,
                RelatedEntityID = relatedId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
            _context.Notifications.Add(noti);
        }

        // GET: /Subject/Subjects
        public IActionResult Subjects()
        {
            var subjects = _context.Subjects
                .Include(s => s.Creator)
                .ToList();
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
                try
                {
                    model.CreatedAt = DateTime.UtcNow;
                    var userIdClaim = User.FindFirst("UserID")?.Value;
                    int createdBy = 0;
                    if (int.TryParse(userIdClaim, out int userId))
                        model.CreatedBy = createdBy = userId;

                    _context.Subjects.Add(model);
                    await _context.SaveChangesAsync();

                    var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
                    foreach (var user in notifyUsers)
                    {
                        AddNotification(
                            user.UserID,
                            $"Subject \"{model.SubjectName}\" has been created.",
                            "Subject",
                            model.SubjectID,
                            createdBy
                        );
                    }
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Subject created successfully!";
                    return RedirectToAction("Subjects");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.ToString();
                    return View(model);
                }
            }
            else
            {
                ViewBag.Error = "ModelState invalid: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return View(model);
        }

        // GET: /Subject/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        // POST: /Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject model)
        {
            if (id != model.SubjectID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = await _context.Subjects.FindAsync(id);
                    if (subject == null)
                        return NotFound();

                    subject.SubjectName = model.SubjectName;
                    subject.Status = model.Status;

                    _context.Subjects.Update(subject);
                    await _context.SaveChangesAsync();

                    var userIdClaim = User.FindFirst("UserID")?.Value;
                    int.TryParse(userIdClaim, out int createdBy);

                    var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
                    foreach (var user in notifyUsers)
                    {
                        AddNotification(
                            user.UserID,
                            $"Subject \"{subject.SubjectName}\" has been updated.",
                            "Subject",
                            subject.SubjectID,
                            createdBy
                        );
                    }
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Subject updated successfully!";
                    return RedirectToAction("Subjects");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.ToString();
                    return View(model);
                }
            }
            else
            {
                ViewBag.Error = "ModelState invalid: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return View(model);
        }

        // POST: /Subject/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            var cloExists = await _context.CLOs.AnyAsync(c => c.SubjectID == id);
            if (cloExists)
            {
                TempData["Error"] = "Cannot delete this subject because there are related CLOs (Course Learning Outcomes). Please delete or reassign the CLOs before deleting the subject.";
                return RedirectToAction("Subjects");
            }

            string subjectName = subject.SubjectName;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            var userIdClaim = User.FindFirst("UserID")?.Value;
            int.TryParse(userIdClaim, out int createdBy);

            var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
            foreach (var user in notifyUsers)
            {
                AddNotification(
                    user.UserID,
                    $"Subject \"{subjectName}\" has been deleted.",
                    "Subject",
                    id,
                    createdBy
                );
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Subject deleted successfully!";
            return RedirectToAction("Subjects");
        }
    }
}
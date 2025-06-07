using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QBCA.Controllers
{
    public class CLOController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CLOController(ApplicationDbContext context)
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

        // GET: /CLO/CLOs
        public async Task<IActionResult> CLOs()
        {
            var list = await _context.CLOs
                .Include(c => c.Subject)
                .Include(c => c.Questions)
                .ToListAsync();
            return View(list);
        }

        // GET: /CLO/Create
        public IActionResult Create()
        {
            var vm = new CLOCreateViewModel
            {
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };
            return View(vm);
        }

        // POST: /CLO/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CLOCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var clo = new CLO
                {
                    Code = vm.Code,
                    Description = vm.Description,
                    SubjectID = vm.SubjectID.Value,
                    Questions = new List<Question>()
                };

                if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
                {
                    clo.Questions = await _context.Questions
                        .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                        .ToListAsync();
                }

                _context.CLOs.Add(clo);
                await _context.SaveChangesAsync();

                var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
                string subjectName = _context.Subjects.Find(clo.SubjectID)?.SubjectName;

                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var user in notifyUsers)
                {
                    AddNotification(
                        user.UserID,
                        $"CLO \"{clo.Code}\" for subject \"{subjectName}\" has been created.",
                        "CLO",
                        clo.CLOID,
                        createdBy
                    );
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "CLO created successfully!";
                return RedirectToAction("CLOs");
            }

            vm.AllSubjects = _context.Subjects.ToList();
            vm.AllQuestions = _context.Questions.ToList();
            return View(vm);
        }

        // GET: /CLO/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var clo = await _context.CLOs
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.CLOID == id);

            if (clo == null)
            {
                return NotFound();
            }

            var vm = new CLOCreateViewModel
            {
                CLOID = clo.CLOID,
                Code = clo.Code,
                Description = clo.Description,
                SubjectID = clo.SubjectID,
                SelectedQuestionIDs = clo.Questions?.Select(q => q.QuestionID).ToList() ?? new List<int>(),
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };

            return View(vm);
        }

        // POST: /CLO/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CLOCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllSubjects = _context.Subjects.ToList();
                vm.AllQuestions = _context.Questions.ToList();
                return View(vm);
            }

            var clo = await _context.CLOs
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.CLOID == vm.CLOID);

            if (clo == null)
            {
                return NotFound();
            }

            clo.Code = vm.Code;
            clo.Description = vm.Description;
            clo.SubjectID = vm.SubjectID.Value;

            // Update 
            clo.Questions.Clear();
            if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
            {
                var selectedQuestions = await _context.Questions
                    .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                    .ToListAsync();
                foreach (var q in selectedQuestions)
                {
                    clo.Questions.Add(q);
                }
            }

            await _context.SaveChangesAsync();

            var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
            string subjectName = _context.Subjects.Find(clo.SubjectID)?.SubjectName;

            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            int.TryParse(userIdClaim, out int createdBy);

            foreach (var user in notifyUsers)
            {
                AddNotification(
                    user.UserID,
                    $"CLO \"{clo.Code}\" for subject \"{subjectName}\" has been updated.",
                    "CLO",
                    clo.CLOID,
                    createdBy
                );
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "CLO updated successfully!";
            return RedirectToAction("CLOs");
        }

        // POST: /CLO/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var clo = await _context.CLOs
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.CLOID == id);

            if (clo == null)
                return NotFound();

            string cloCode = clo.Code;
            string subjectName = _context.Subjects.Find(clo.SubjectID)?.SubjectName;

            clo.Questions?.Clear();

            try
            {
                _context.CLOs.Remove(clo);
                await _context.SaveChangesAsync();

                var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();

                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var user in notifyUsers)
                {
                    AddNotification(
                        user.UserID,
                        $"CLO \"{cloCode}\" for subject \"{subjectName}\" has been deleted.",
                        "CLO",
                        id,
                        createdBy
                    );
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "CLO deleted successfully!";
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = "Cannot delete this CLO because there are related data or foreign key constraints. Please check and remove all related questions or dependent records before deleting!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the CLO: " + ex.Message;
            }

            return RedirectToAction("CLOs");
        }
    }
}
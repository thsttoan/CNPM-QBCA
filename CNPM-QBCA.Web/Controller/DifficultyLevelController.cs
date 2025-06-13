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
    public class DifficultyLevelController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DifficultyLevelController(ApplicationDbContext context)
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

        // GET: /DifficultyLevel/DLs
        public async Task<IActionResult> DLs()
        {
            var list = await _context.DifficultyLevels
                .Include(dl => dl.Subject)
                .Include(dl => dl.Questions)
                .ToListAsync();
            return View(list);
        }

        // GET: /DifficultyLevel/Create
        public IActionResult Create()
        {
            var vm = new DifficultyLevelCreateViewModel
            {
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };
            return View(vm);
        }

        // POST: /DifficultyLevel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DifficultyLevelCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var dl = new DifficultyLevel
                {
                    LevelName = vm.LevelName,
                    SubjectID = vm.SubjectID.Value,
                    Questions = new List<Question>()
                };

                if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
                {
                    dl.Questions = await _context.Questions
                        .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                        .ToListAsync();
                }

                _context.DifficultyLevels.Add(dl);
                await _context.SaveChangesAsync();

                var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
                string subjectName = _context.Subjects.Find(dl.SubjectID)?.SubjectName;

                // LẤY userID NGƯỜI TẠO
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var user in notifyUsers)
                {
                    AddNotification(
                        user.UserID,
                        $"Difficulty level \"{dl.LevelName}\" for subject \"{subjectName}\" has been created.",
                        "DifficultyLevel",
                        dl.DifficultyLevelID,
                        createdBy
                    );
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "DifficultyLevel created successfully!";
                return RedirectToAction("DLs");
            }

            vm.AllSubjects = _context.Subjects.ToList();
            vm.AllQuestions = _context.Questions.ToList();
            return View(vm);
        }

        // GET: /DifficultyLevel/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dl = await _context.DifficultyLevels
                .Include(d => d.Questions)
                .FirstOrDefaultAsync(d => d.DifficultyLevelID == id);

            if (dl == null)
            {
                return NotFound();
            }

            var vm = new DifficultyLevelCreateViewModel
            {
                DifficultyLevelID = dl.DifficultyLevelID,
                LevelName = dl.LevelName,
                SubjectID = dl.SubjectID,
                SelectedQuestionIDs = dl.Questions?.Select(q => q.QuestionID).ToList() ?? new List<int>(),
                AllSubjects = _context.Subjects.ToList(),
                AllQuestions = _context.Questions.ToList()
            };

            return View(vm);
        }

        // POST: /DifficultyLevel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DifficultyLevelCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllSubjects = _context.Subjects.ToList();
                vm.AllQuestions = _context.Questions.ToList();
                return View(vm);
            }

            var dl = await _context.DifficultyLevels
                .Include(d => d.Questions)
                .FirstOrDefaultAsync(d => d.DifficultyLevelID == vm.DifficultyLevelID);

            if (dl == null)
            {
                return NotFound();
            }

            dl.LevelName = vm.LevelName;
            dl.SubjectID = vm.SubjectID.Value;

            // Update questions
            dl.Questions.Clear();
            if (vm.SelectedQuestionIDs != null && vm.SelectedQuestionIDs.Count > 0)
            {
                var selectedQuestions = await _context.Questions
                    .Where(q => vm.SelectedQuestionIDs.Contains(q.QuestionID))
                    .ToListAsync();
                foreach (var q in selectedQuestions)
                {
                    dl.Questions.Add(q);
                }
            }

            await _context.SaveChangesAsync();

            var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();
            string subjectName = _context.Subjects.Find(dl.SubjectID)?.SubjectName;

            // LẤY userID NGƯỜI SỬA
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            int.TryParse(userIdClaim, out int createdBy);

            foreach (var user in notifyUsers)
            {
                AddNotification(
                    user.UserID,
                    $"Difficulty level \"{dl.LevelName}\" for subject \"{subjectName}\" has been updated.",
                    "DifficultyLevel",
                    dl.DifficultyLevelID,
                    createdBy
                );
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Difficulty Level updated successfully!";
            return RedirectToAction("DLs");
        }

        // POST: /DifficultyLevel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dl = await _context.DifficultyLevels
                .Include(d => d.Questions)
                .FirstOrDefaultAsync(d => d.DifficultyLevelID == id);

            if (dl == null)
                return NotFound();

            string levelName = dl.LevelName;
            string subjectName = _context.Subjects.Find(dl.SubjectID)?.SubjectName;

            dl.Questions?.Clear();

            try
            {
                _context.DifficultyLevels.Remove(dl);
                await _context.SaveChangesAsync();

                var notifyUsers = _context.Users.Where(u => u.RoleID == 1 || u.RoleID == 3).ToList();

                // LẤY userID NGƯỜI XÓA
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var user in notifyUsers)
                {
                    AddNotification(
                        user.UserID,
                        $"Difficulty level \"{levelName}\" for subject \"{subjectName}\" has been deleted.",
                        "DifficultyLevel",
                        id,
                        createdBy
                    );
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Difficulty Level deleted successfully!";
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = "Cannot delete this Difficulty Level because there are related data or foreign key constraints. Please check and remove all related questions or dependent records before deleting!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the Difficulty Level: " + ex.Message;
            }

            return RedirectToAction("DLs");
        }
    }
}
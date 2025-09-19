﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;

namespace QBCA.Controllers
{
    public class ExamPlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly List<int> ManagerRoleIds = new List<int> { 3, 4, 5 };

        public ExamPlanController(ApplicationDbContext context)
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
                CreatedAt = System.DateTime.UtcNow,
                CreatedBy = createdBy
            };
            _context.Notifications.Add(noti);
        }

        // GET: /ExamPlan/ExamPlans
        [HttpGet]
        public async Task<IActionResult> ExamPlans()
        {
            var plans = await _context.ExamPlans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManagerRole)
                .Include(p => p.CreatedByUser)
                .ToListAsync();
            return View("ExamPlans", plans);
        }

        // GET: /ExamPlan/Create
        public IActionResult Create()
        {
            var subjects = _context.Subjects.ToList();
            var firstSubjectId = subjects.Any() ? subjects.First().SubjectID : 0;

            var vm = new ExamPlanCreateViewModel
            {
                AllSubjects = subjects,
                AllDifficultyLevels = firstSubjectId > 0
                    ? _context.DifficultyLevels.Where(dl => dl.SubjectID == firstSubjectId).ToList()
                    : new List<DifficultyLevel>(),
                AllManagerRoles = _context.Roles
                    .Where(r => ManagerRoleIds.Contains(r.RoleID))
                    .ToList(),
                Distributions = new List<PlanDistributionViewModel> { new PlanDistributionViewModel() },
                StatusOptions = new List<string> { "Pending", "Approved", "Rejected" }
            };
            return View(vm);
        }

        // POST: /ExamPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamPlanCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int createdBy = 0;
                int.TryParse(userIdClaim, out createdBy);

                if (createdBy == 0 || !_context.Users.Any(u => u.UserID == createdBy))
                {
                    ModelState.AddModelError("", "Invalid or missing user. Please login again.");
                    model.AllSubjects = _context.Subjects.ToList();
                    model.AllDifficultyLevels = model.SubjectID > 0
                        ? _context.DifficultyLevels.Where(dl => dl.SubjectID == model.SubjectID).ToList()
                        : new List<DifficultyLevel>();
                    model.AllManagerRoles = _context.Roles.Where(r => ManagerRoleIds.Contains(r.RoleID)).ToList();
                    model.StatusOptions = new List<string> { "Pending", "Approved", "Rejected" };
                    return View(model);
                }

                var examPlan = new ExamPlan
                {
                    Name = model.PlanName,
                    SubjectID = model.SubjectID,
                    Status = model.Status,
                    CreatedAt = System.DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                _context.ExamPlans.Add(examPlan);
                await _context.SaveChangesAsync();

                if (model.Distributions != null)
                {
                    foreach (var dist in model.Distributions)
                    {
                        var entity = new ExamPlanDistribution
                        {
                            ExamPlanID = examPlan.ExamPlanID,
                            DifficultyLevelID = dist.DifficultyLevelID,
                            NumberOfQuestions = dist.NumberOfQuestions,
                            AssignedManagerRoleID = dist.AssignedManagerRoleID ?? 0
                        };
                        _context.ExamPlanDistributions.Add(entity);
                    }
                    await _context.SaveChangesAsync();
                }

                var subjectName = _context.Subjects.Find(examPlan.SubjectID)?.SubjectName;
                foreach (var dist in model.Distributions)
                {
                    if (dist.AssignedManagerRoleID.HasValue)
                    {
                        var users = _context.Users.Where(u => u.RoleID == dist.AssignedManagerRoleID.Value && u.IsActive).ToList();
                        foreach (var user in users)
                        {
                            AddNotification(
                                user.UserID,
                                $"Exam Plan \"{examPlan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been created.",
                                "ExamPlan",
                                examPlan.ExamPlanID,
                                createdBy
                            );
                        }
                    }
                    var rds = _context.Users.Where(u => u.RoleID == 1 && u.IsActive).ToList();
                    foreach (var rd in rds)
                    {
                        AddNotification(
                            rd.UserID,
                            $"Exam Plan \"{examPlan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been created.",
                            "ExamPlan",
                            examPlan.ExamPlanID,
                            createdBy
                        );
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "ExamPlan & Distribution created successfully!";
                return RedirectToAction(nameof(ExamPlans));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = model.SubjectID > 0
                ? _context.DifficultyLevels.Where(dl => dl.SubjectID == model.SubjectID).ToList()
                : new List<DifficultyLevel>();
            model.AllManagerRoles = _context.Roles.Where(r => ManagerRoleIds.Contains(r.RoleID)).ToList();
            model.StatusOptions = new List<string> { "Pending", "Approved", "Rejected" };
            return View(model);
        }

        // GET: /ExamPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examPlan = await _context.ExamPlans
                .Include(p => p.Distributions)
                .FirstOrDefaultAsync(p => p.ExamPlanID == id);
            if (examPlan == null) return NotFound();

            var vm = new ExamPlanCreateViewModel
            {
                ExamPlanID = examPlan.ExamPlanID,
                PlanName = examPlan.Name,
                SubjectID = examPlan.SubjectID,
                Status = examPlan.Status,
                AllSubjects = _context.Subjects.ToList(),
                AllDifficultyLevels = _context.DifficultyLevels
                    .Where(dl => dl.SubjectID == examPlan.SubjectID)
                    .ToList(),
                AllManagerRoles = _context.Roles
                    .Where(r => ManagerRoleIds.Contains(r.RoleID))
                    .ToList(),
                Distributions = examPlan.Distributions.Select(d => new PlanDistributionViewModel
                {
                    DistributionID = d.DistributionID,
                    DifficultyLevelID = d.DifficultyLevelID,
                    NumberOfQuestions = d.NumberOfQuestions,
                    AssignedManagerRoleID = d.AssignedManagerRoleID
                }).ToList(),
                StatusOptions = new List<string> { "Pending", "Approved", "Rejected" }
            };
            return View(vm);
        }

        // POST: /ExamPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExamPlanCreateViewModel model)
        {
            if (model.ExamPlanID == null) return NotFound();

            if (ModelState.IsValid)
            {
                var examPlan = await _context.ExamPlans
                    .Include(p => p.Distributions)
                    .FirstOrDefaultAsync(p => p.ExamPlanID == model.ExamPlanID);

                if (examPlan == null) return NotFound();

                examPlan.Name = model.PlanName;
                examPlan.SubjectID = model.SubjectID;
                examPlan.Status = model.Status;

                _context.ExamPlanDistributions.RemoveRange(examPlan.Distributions);
                if (model.Distributions != null)
                {
                    foreach (var dist in model.Distributions)
                    {
                        var entity = new ExamPlanDistribution
                        {
                            ExamPlanID = examPlan.ExamPlanID,
                            DifficultyLevelID = dist.DifficultyLevelID,
                            NumberOfQuestions = dist.NumberOfQuestions,
                            AssignedManagerRoleID = dist.AssignedManagerRoleID ?? 0
                        };
                        _context.ExamPlanDistributions.Add(entity);
                    }
                }
                await _context.SaveChangesAsync();

                var oldNotifications = _context.Notifications
                    .Where(n => n.RelatedEntityType == "ExamPlan" && n.RelatedEntityID == examPlan.ExamPlanID)
                    .ToList();
                _context.Notifications.RemoveRange(oldNotifications);
                await _context.SaveChangesAsync();

                var subjectName = _context.Subjects.Find(examPlan.SubjectID)?.SubjectName;
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int createdBy = 0;
                int.TryParse(userIdClaim, out createdBy);

                foreach (var dist in model.Distributions)
                {
                    if (dist.AssignedManagerRoleID.HasValue)
                    {
                        var users = _context.Users.Where(u => u.RoleID == dist.AssignedManagerRoleID.Value && u.IsActive).ToList();
                        foreach (var user in users)
                        {
                            AddNotification(
                                user.UserID,
                                $"Exam Plan \"{examPlan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been updated.",
                                "ExamPlan",
                                examPlan.ExamPlanID,
                                createdBy
                            );
                        }
                    }
                    var rds = _context.Users.Where(u => u.RoleID == 1 && u.IsActive).ToList();
                    foreach (var rd in rds)
                    {
                        AddNotification(
                            rd.UserID,
                            $"Exam Plan \"{examPlan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been updated.",
                            "ExamPlan",
                            examPlan.ExamPlanID,
                            createdBy
                        );
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "ExamPlan & Distribution updated successfully!";
                return RedirectToAction(nameof(ExamPlans));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = model.SubjectID > 0
                ? _context.DifficultyLevels.Where(dl => dl.SubjectID == model.SubjectID).ToList()
                : new List<DifficultyLevel>();
            model.AllManagerRoles = _context.Roles.Where(r => ManagerRoleIds.Contains(r.RoleID)).ToList();
            model.StatusOptions = new List<string> { "Pending", "Approved", "Rejected" };
            return View(model);
        }

        // POST: /ExamPlan/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var examPlan = await _context.ExamPlans
                .Include(p => p.Distributions)
                .FirstOrDefaultAsync(p => p.ExamPlanID == id);

            if (examPlan != null)
            {
                _context.ExamPlanDistributions.RemoveRange(examPlan.Distributions);
                _context.ExamPlans.Remove(examPlan);

                var notifications = _context.Notifications
                    .Where(n => n.RelatedEntityType == "ExamPlan" && n.RelatedEntityID == examPlan.ExamPlanID)
                    .ToList();
                _context.Notifications.RemoveRange(notifications);

                await _context.SaveChangesAsync();
                TempData["Success"] = "ExamPlan & Distribution deleted successfully!";
            }
            return RedirectToAction(nameof(ExamPlans));
        }

        // GET: /ExamPlan/ExamPlan
        public async Task<IActionResult> ExamPlan()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            ViewBag.CurrentUserId = userId;

            var plans = await _context.ExamPlans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManagerRole)
                .Include(p => p.CreatedByUser)
                .Where(p => p.Distributions.Any(d => d.AssignedManagerRoleID == userId))
                .ToListAsync();

            return View("ExamPlan", plans);
        }

        [HttpGet]
        public JsonResult GetDifficultyLevelsBySubject(int subjectId)
        {
            var difficulties = _context.DifficultyLevels
                .Where(dl => dl.SubjectID == subjectId)
                .Select(dl => new { dl.DifficultyLevelID, dl.LevelName })
                .ToList();
            return Json(difficulties);
        }
    }
}

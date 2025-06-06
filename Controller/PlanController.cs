using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;

namespace QBCA.Controllers
{
    public class PlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly List<int> ManagerRoleIds = new List<int> { 3, 4, 5 }; // Các Role Manager

        public PlanController(ApplicationDbContext context)
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

        // GET: /Plan/Plan
        public async Task<IActionResult> Plan()
        {
            var plans = await _context.Plans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManagerRole)
                .ToListAsync();
            return View("Plans", plans);
        }

        // GET: /Plan/Create
        public IActionResult Create()
        {
            var vm = new PlanCreateViewModel
            {
                AllSubjects = _context.Subjects.ToList(),
                AllDifficultyLevels = _context.DifficultyLevels.ToList(),
                AllManagerRoles = _context.Roles
                    .Where(r => ManagerRoleIds.Contains(r.RoleID))
                    .ToList(),
                Distributions = new List<PlanDistributionViewModel> { new PlanDistributionViewModel() }
            };
            return View(vm);
        }

        // POST: /Plan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var plan = new Plan
                {
                    Name = model.PlanName,
                    SubjectID = model.SubjectID,
                    CreatedAt = System.DateTime.UtcNow
                };
                _context.Plans.Add(plan);
                await _context.SaveChangesAsync();

                // Add distributions
                if (model.Distributions != null)
                {
                    foreach (var dist in model.Distributions)
                    {
                        var entity = new PlanDistribution
                        {
                            PlanID = plan.PlanID,
                            DifficultyLevelID = dist.DifficultyLevelID,
                            NumberOfQuestions = dist.NumberOfQuestions,
                            AssignedManagerRoleID = dist.AssignedManagerRoleID.HasValue ? dist.AssignedManagerRoleID.Value : 0
                        };
                        _context.PlanDistributions.Add(entity);
                    }
                    await _context.SaveChangesAsync();
                }

                // Gửi thông báo cho các user thuộc từng role đã chọn trong từng distribution
                var subjectName = _context.Subjects.Find(plan.SubjectID)?.SubjectName;
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var dist in model.Distributions)
                {
                    // Gửi cho tất cả Manager được chọn
                    if (dist.AssignedManagerRoleID.HasValue)
                    {
                        var users = _context.Users.Where(u => u.RoleID == dist.AssignedManagerRoleID.Value && u.IsActive).ToList();
                        foreach (var user in users)
                        {
                            AddNotification(
                                user.UserID,
                                $"Plan \"{plan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been created.",
                                "Plan",
                                plan.PlanID,
                                createdBy
                            );
                        }
                    }

                    // Gửi cho tất cả RD (RoleID == 1)
                    var rds = _context.Users.Where(u => u.RoleID == 1 && u.IsActive).ToList();
                    foreach (var rd in rds)
                    {
                        AddNotification(
                            rd.UserID,
                            $"Plan \"{plan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been created.",
                            "Plan",
                            plan.PlanID,
                            createdBy
                        );
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plan & Distribution created successfully!";
                return RedirectToAction(nameof(Plan));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = _context.DifficultyLevels.ToList();
            model.AllManagerRoles = _context.Roles.Where(r => ManagerRoleIds.Contains(r.RoleID)).ToList();
            return View(model);
        }

        // GET: /Plan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.Plans
                .Include(p => p.Distributions)
                .FirstOrDefaultAsync(p => p.PlanID == id);
            if (plan == null) return NotFound();

            var vm = new PlanCreateViewModel
            {
                PlanID = plan.PlanID,
                PlanName = plan.Name,
                SubjectID = plan.SubjectID,
                AllSubjects = _context.Subjects.ToList(),
                AllDifficultyLevels = _context.DifficultyLevels.ToList(),
                AllManagerRoles = _context.Roles
                    .Where(r => ManagerRoleIds.Contains(r.RoleID))
                    .ToList(),
                Distributions = plan.Distributions.Select(d => new PlanDistributionViewModel
                {
                    DistributionID = d.DistributionID,
                    DifficultyLevelID = d.DifficultyLevelID,
                    NumberOfQuestions = d.NumberOfQuestions,
                    AssignedManagerRoleID = d.AssignedManagerRoleID
                }).ToList()
            };
            return View(vm);
        }

        // POST: /Plan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlanCreateViewModel model)
        {
            if (model.PlanID == null) return NotFound();

            if (ModelState.IsValid)
            {
                var plan = await _context.Plans
                    .Include(p => p.Distributions)
                    .FirstOrDefaultAsync(p => p.PlanID == model.PlanID);

                if (plan == null) return NotFound();

                plan.Name = model.PlanName;
                plan.SubjectID = model.SubjectID;

                _context.PlanDistributions.RemoveRange(plan.Distributions);
                if (model.Distributions != null)
                {
                    foreach (var dist in model.Distributions)
                    {
                        var entity = new PlanDistribution
                        {
                            PlanID = plan.PlanID,
                            DifficultyLevelID = dist.DifficultyLevelID,
                            NumberOfQuestions = dist.NumberOfQuestions,
                            AssignedManagerRoleID = dist.AssignedManagerRoleID.HasValue ? dist.AssignedManagerRoleID.Value : 0
                        };
                        _context.PlanDistributions.Add(entity);
                    }
                }
                await _context.SaveChangesAsync();

                // Xóa thông báo cũ liên quan đến plan này
                var oldNotifications = _context.Notifications
                    .Where(n => n.RelatedEntityType == "Plan" && n.RelatedEntityID == plan.PlanID)
                    .ToList();
                _context.Notifications.RemoveRange(oldNotifications);
                await _context.SaveChangesAsync();

                // Gửi lại thông báo cho các user thuộc từng role đã chọn trong từng distribution
                var subjectName = _context.Subjects.Find(plan.SubjectID)?.SubjectName;
                var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                int.TryParse(userIdClaim, out int createdBy);

                foreach (var dist in model.Distributions)
                {
                    // Gửi cho tất cả Manager được chọn
                    if (dist.AssignedManagerRoleID.HasValue)
                    {
                        var users = _context.Users.Where(u => u.RoleID == dist.AssignedManagerRoleID.Value && u.IsActive).ToList();
                        foreach (var user in users)
                        {
                            AddNotification(
                                user.UserID,
                                $"Plan \"{plan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been updated.",
                                "Plan",
                                plan.PlanID,
                                createdBy
                            );
                        }
                    }

                    // Gửi cho tất cả RD (RoleID == 1)
                    var rds = _context.Users.Where(u => u.RoleID == 1 && u.IsActive).ToList();
                    foreach (var rd in rds)
                    {
                        AddNotification(
                            rd.UserID,
                            $"Plan \"{plan.Name}\" for subject \"{subjectName}\" with distribution '{_context.DifficultyLevels.Find(dist.DifficultyLevelID)?.LevelName}' has been updated.",
                            "Plan",
                            plan.PlanID,
                            createdBy
                        );
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plan & Distribution updated successfully!";
                return RedirectToAction(nameof(Plan));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = _context.DifficultyLevels.ToList();
            model.AllManagerRoles = _context.Roles.Where(r => ManagerRoleIds.Contains(r.RoleID)).ToList();
            return View(model);
        }

        // POST: /Plan/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.Plans
                .Include(p => p.Distributions)
                .FirstOrDefaultAsync(p => p.PlanID == id);

            if (plan != null)
            {
                _context.PlanDistributions.RemoveRange(plan.Distributions);
                _context.Plans.Remove(plan);

                // Xóa notification liên quan đến plan này
                var notifications = _context.Notifications
                    .Where(n => n.RelatedEntityType == "Plan" && n.RelatedEntityID == plan.PlanID)
                    .ToList();
                _context.Notifications.RemoveRange(notifications);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Plan & Distribution deleted successfully!";
            }
            return RedirectToAction(nameof(Plan));
        }
    }
}
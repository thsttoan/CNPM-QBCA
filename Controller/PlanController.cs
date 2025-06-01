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
        private static readonly List<int> ManagerRoleIds = new List<int> { 3, 4, 5 };

        public PlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Plan/Plan
        public async Task<IActionResult> Plan()
        {
            var plans = await _context.Plans
                .Include(p => p.Subject)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.Distributions)
                    .ThenInclude(d => d.AssignedManager)
                .ToListAsync();
            return View("Plans", plans); // View: Views/Plan/Plans.cshtml
        }

        // GET: /Plan/Create
        public IActionResult Create()
        {
            var vm = new PlanCreateViewModel
            {
                AllSubjects = _context.Subjects.ToList(),
                AllDifficultyLevels = _context.DifficultyLevels.ToList(),
                AllManagers = _context.Users.Where(u => ManagerRoleIds.Contains(u.RoleID) && u.IsActive).ToList(),
                Distributions = new List<PlanDistributionViewModel>
        {
            new PlanDistributionViewModel()
        }
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
                            AssignedManagerID = dist.AssignedManagerID
                        };
                        _context.PlanDistributions.Add(entity);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Plan & Distribution created successfully!";
                return RedirectToAction(nameof(Plan));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = _context.DifficultyLevels.ToList();
            model.AllManagers = _context.Users.Where(u => ManagerRoleIds.Contains(u.RoleID) && u.IsActive).ToList();
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
                AllManagers = _context.Users.Where(u => ManagerRoleIds.Contains(u.RoleID) && u.IsActive).ToList(),
                Distributions = plan.Distributions.Select(d => new PlanDistributionViewModel
                {
                    DistributionID = d.DistributionID,
                    DifficultyLevelID = d.DifficultyLevelID,
                    NumberOfQuestions = d.NumberOfQuestions,
                    AssignedManagerID = d.AssignedManagerID
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
                            AssignedManagerID = dist.AssignedManagerID
                        };
                        _context.PlanDistributions.Add(entity);
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plan & Distribution updated successfully!";
                return RedirectToAction(nameof(Plan));
            }
            model.AllSubjects = _context.Subjects.ToList();
            model.AllDifficultyLevels = _context.DifficultyLevels.ToList();
            model.AllManagers = _context.Users.Where(u => ManagerRoleIds.Contains(u.RoleID) && u.IsActive).ToList();
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
                await _context.SaveChangesAsync();
                TempData["Success"] = "Plan & Distribution deleted successfully!";
            }
            return RedirectToAction(nameof(Plan));
        }
    }
}
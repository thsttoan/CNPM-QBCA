using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using QBCA.ViewModels;
using System;
using System.Linq;

namespace QBCA.Controllers
{
    public class AssignedPlanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignedPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssignedPlan/Create
        public IActionResult Create()
        {
            var model = new AssignedPlanViewModel
            {
                AssignedDate = DateTime.Now,
                Deadline = DateTime.Now.AddDays(7)
            };
            LoadDropdowns(model);
            return View(model);
        }

        // POST: AssignedPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AssignedPlanViewModel model)
        {
            if (model.Deadline <= DateTime.Now)
            {
                ModelState.AddModelError("Deadline", "Deadline must be a future date.");
                LoadDropdowns(model);
                return View(model);
            }

            if (!_context.ExamPlanDistributions.Any(d => d.DistributionID == model.DistributionID))
            {
                ModelState.AddModelError("DistributionID", "Selected distribution does not exist.");
                LoadDropdowns(model);
                return View(model);
            }

            var plan = new AssignedPlan
            {
                ExamPlanID = model.ExamPlanID,
                DistributionID = model.DistributionID,
                AssignedToID = model.AssignedToID,
                AssignedByID = 1, // TODO: Replace with logged-in user
                AssignedDate = DateTime.Now,
                Deadline = model.Deadline,
                Notes = model.Notes,
                TaskType = model.TaskType,
                Status = AssignedPlanStatus.Assigned
            };

            _context.AssignPlans.Add(plan);
            _context.SaveChanges(); // Lưu trước để lấy ID

            var subjectName = _context.ExamPlans
                .Include(e => e.Subject)
                .FirstOrDefault(e => e.ExamPlanID == model.ExamPlanID)
                ?.Subject?.SubjectName;

            var notification = new Notification
            {
                UserID = model.AssignedToID,
                Message = $"You have been assigned a task for subject: {subjectName} ({model.TaskType})",
                CreatedAt = DateTime.Now,
                CreatedBy = plan.AssignedByID,
                Status = "Unread",
                RelatedEntityType = "AssignedPlan",
                RelatedEntityID = plan.ID
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            return RedirectToAction("Plans");

        }

        // GET: AssignedPlan/Plans
        public IActionResult Plans(string subject, string status)
        {
            var query = _context.AssignPlans
                .Include(t => t.ExamPlan).ThenInclude(p => p.Subject)
                .Include(t => t.AssignedTo)
                .Include(t => t.Distribution).ThenInclude(d => d.DifficultyLevel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(subject))
                query = query.Where(p => p.ExamPlan.Subject.SubjectName == subject);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status.ToString() == status);

            var assignedPlans = query
                .OrderByDescending(p => p.AssignedDate)
                .Select(t => new AssignedPlanViewModel
                {
                    AssignedPlanID = t.ID,
                    SubjectName = t.ExamPlan.Subject.SubjectName,
                    AssignedToName = t.AssignedTo.FullName,
                    DistributionStatus = $"{t.Distribution.Status} - {t.Distribution.DifficultyLevel.LevelName}",
                    Status = t.Status,
                    TaskType = t.TaskType,
                    AssignedDate = t.AssignedDate,
                    Deadline = t.Deadline,
                    Notes = t.Notes
                }).ToList();

            ViewBag.Subjects = _context.Subjects.Select(s => s.SubjectName).Distinct().ToList();
            ViewBag.Statuses = Enum.GetNames(typeof(AssignedPlanStatus)).ToList();

            return View(assignedPlans);
        }

        // GET: AssignedPlan/Edit/{id}
        public IActionResult Edit(int id)
        {
            var plan = _context.AssignPlans.Find(id);
            if (plan == null) return NotFound();

            var model = new AssignedPlanViewModel
            {
                AssignedPlanID = plan.ID,
                ExamPlanID = plan.ExamPlanID,
                DistributionID = plan.DistributionID,
                AssignedToID = plan.AssignedToID,
                AssignedByID = plan.AssignedByID,
                AssignedDate = plan.AssignedDate,
                Deadline = plan.Deadline,
                Notes = plan.Notes,
                Status = plan.Status,
                TaskType = plan.TaskType
            };

            LoadDropdowns(model);
            return View(model);
        }

        // POST: AssignedPlan/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AssignedPlanViewModel model)
        {
            if (model.Deadline <= DateTime.Now)
            {
                ModelState.AddModelError("Deadline", "Deadline must be a future date.");
                LoadDropdowns(model);
                return View(model);
            }

            var plan = _context.AssignPlans.Find(id);
            if (plan == null) return NotFound();

            plan.ExamPlanID = model.ExamPlanID;
            plan.DistributionID = model.DistributionID;
            plan.AssignedToID = model.AssignedToID;
            plan.Deadline = model.Deadline;
            plan.Notes = model.Notes;
            plan.Status = (AssignedPlanStatus)model.Status;
            plan.TaskType = model.TaskType;

            _context.SaveChanges();
            return RedirectToAction("Plans");
        }

        // GET: AssignedPlan/Details/{id}
        public IActionResult Details(int id)
        {
            var plan = _context.AssignPlans
                .Include(p => p.ExamPlan).ThenInclude(p => p.Subject)
                .Include(p => p.Distribution).ThenInclude(d => d.DifficultyLevel)
                .Include(p => p.AssignedTo)
                .Include(p => p.AssignedBy)
                .FirstOrDefault(p => p.ID == id);

            if (plan == null) return NotFound();

            var viewModel = new AssignedPlanViewModel
            {
                AssignedPlanID = plan.ID,
                ExamPlanID = plan.ExamPlanID,
                SubjectName = plan.ExamPlan.Subject.SubjectName,
                AssignedToName = plan.AssignedTo.FullName,
                AssignedByID = plan.AssignedByID,
                AssignedByName = plan.AssignedBy.FullName,
                AssignedDate = plan.AssignedDate,
                Deadline = plan.Deadline,
                DistributionStatus = $"{plan.Distribution.Status} - {plan.Distribution.DifficultyLevel.LevelName}",
                Notes = plan.Notes,
                TaskType = plan.TaskType,
                Status = plan.Status
            };

            return View(viewModel);
        }

        // GET: AssignedPlan/Delete/{id}
        public IActionResult Delete(int id)
        {
            var plan = _context.AssignPlans
                .Include(p => p.ExamPlan).ThenInclude(e => e.Subject)
                .Include(p => p.AssignedTo)
                .FirstOrDefault(p => p.ID == id);

            if (plan == null) return NotFound();

            var model = new AssignedPlanViewModel
            {
                AssignedPlanID = plan.ID,
                SubjectName = plan.ExamPlan.Subject.SubjectName,
                AssignedToName = plan.AssignedTo.FullName,
                Deadline = plan.Deadline,
                Status = plan.Status
            };

            return View(model);
        }

        // POST: AssignedPlan/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(AssignedPlanViewModel model)
        {
            var plan = _context.AssignPlans.Find(model.AssignedPlanID);
            if (plan == null) return NotFound();

            _context.AssignPlans.Remove(plan);
            _context.SaveChanges();
            return RedirectToAction("Plans");
        }

        // Helper dropdowns
        private void LoadDropdowns(AssignedPlanViewModel model)
        {
            model.AllExamPlans = _context.ExamPlans.Include(e => e.Subject).ToList();

            model.DisplayDistributions = _context.ExamPlanDistributions
                .Include(d => d.DifficultyLevel)
                .Include(d => d.ExamPlan).ThenInclude(p => p.Subject)
                .Select(d => new DistributionDisplayViewModel
                {
                    DistributionID = d.DistributionID,
                    DisplayName = $"Plan: {d.ExamPlan.Title} - {d.DifficultyLevel.LevelName} ({d.Status})"
                }).ToList();

            model.AllLecturers = _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Lecturer")
                .ToList();
        }
    }
}

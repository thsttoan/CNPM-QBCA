using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QBCA.Controllers
{

    public class SubmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Submission/SubmissionTable
        public async Task<IActionResult> SubmissionTable()
        {
            var submissions = await _context.SubmissionTables
                .Include(s => s.ExamPlan)  
                .Include(s => s.Creator)
                .Include(s => s.Approver)
                .Include(s => s.Questions)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(submissions);
        }

        // GET: /Submission/Create
        public IActionResult Create()
        {
            ViewBag.Plans = new SelectList(_context.ExamPlans.Where(p => p.Status == "ACTIVE").ToList(), "ExamPlanID", "Name");
            return View();
        }

        // POST: /Submission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmissionTable model)
        {
            if (ModelState.IsValid)
            {
                int.TryParse(User.FindFirst("UserID")?.Value, out int userId);
                
                model.CreatedBy = userId;
                model.CreatedAt = DateTime.UtcNow;
                model.FinalStatus = "Pending";
                // Hoặc status tương tự

                _context.SubmissionTables.Add(model);
                await _context.SaveChangesAsync();
                
                // Lấy ExamPlan để tìm AssignedPlanID tương ứng
                var assignedPlan = await _context.AssignPlans
                    .Where(ap => ap.ExamPlanID == model.PlanID && ap.AssignedToID == userId)
                    .OrderByDescending(ap => ap.CreatedDate)
                    .FirstOrDefaultAsync();

                int assignedPlanId = assignedPlan != null ? assignedPlan.ID : 1; // Fallback to 1 if not found for testing

                // Assign approval to all Head of Departments
                var headOfDepts = await _context.Users
                    .Where(u => u.RoleID == 3 && u.IsActive)
                    .ToListAsync();

                foreach (var hod in headOfDepts)
                {
                    var approval = new SubmissionApproval
                    {
                        SubmissionTableID = model.SubmissionID,
                        AssignedPlanID = assignedPlanId,
                        ApprovedBy = hod.UserID,
                        Status = "Pending",
                        ApprovedDate = DateTime.Now
                    };
                    _context.SubmissionApprovals.Add(approval);
                }
                
                await _context.SaveChangesAsync();

                TempData["Success"] = "Submission table created and sent for approval successfully.";
                return RedirectToAction(nameof(SubmissionTable));
            }

            ViewBag.Plans = new SelectList(_context.ExamPlans.Where(p => p.Status == "ACTIVE").ToList(), "ExamPlanID", "Name", model.PlanID);
            return View(model);
        }

    }
}
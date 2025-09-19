using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using QBCA.ViewModels;
using System;
using System.Linq;

namespace QBCA.Controllers
{
    public class SubmissionApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubmissionApproval/Approve
        public IActionResult Approve()
        {
            var model = new SubmissionApprovalViewModel
            {
                AllSubmissions = _context.SubmissionTables
                    .Include(s => s.ExamPlan).ThenInclude(p => p.Subject)
                    .ToList(),

                AllApprovers = _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "Head of Department")
                    .ToList()
            };

            return View(model);
        }

        // POST: SubmissionApproval/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(SubmissionApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var approval = new SubmissionApproval
                {
                    SubmissionTableID = model.SubmissionTableID,
                    ApprovedBy = model.ApprovedByID,
                    ApprovedDate = DateTime.Now,
                    Status = model.Status
                };

                _context.SubmissionApprovals.Add(approval);

                var submission = _context.SubmissionTables
                    .FirstOrDefault(s => s.SubmissionID == model.SubmissionTableID);

                if (submission != null)
                {
                    submission.Status = model.Status;
                }

                _context.SaveChanges();
                return RedirectToAction("SubmissionApproval");
            }

            model.AllSubmissions = _context.SubmissionTables
                .Include(s => s.ExamPlan).ThenInclude(p => p.Subject)
                .ToList();

            model.AllApprovers = _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Head of Department")
                .ToList();

            return View(model);
        }

        public IActionResult SubmissionApproval()
        {
            var approvals = _context.SubmissionApprovals
                .Include(a => a.SubmissionTable).ThenInclude(s => s.ExamPlan).ThenInclude(e => e.Subject)
                .Include(a => a.Approver)
                .Select(a => new SubmissionApprovalViewModel
                {
                    SubmissionApprovalID = a.SubmissionApprovalID,
                    SubmissionTitle = a.SubmissionTable.ExamPlan.Name,
                    SubjectName = a.SubmissionTable.ExamPlan.Subject.SubjectName,
                    Status = a.Status,
                    ApprovedByName = a.Approver.FullName,
                    ApprovedDate = a.ApprovedDate
                }).ToList();

            return View("SubmissionApproval", approvals);
        }
    }
}

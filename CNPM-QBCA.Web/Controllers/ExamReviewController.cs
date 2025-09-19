using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using QBCA.ViewModels.ReviewExam;
using System.Threading.Tasks;

namespace QBCA.Controllers
{
    public class ExamReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ExamReview/ReviewExam
        public async Task<IActionResult> ReviewExam()
        {
            var reviews = await _context.ReviewExams
                .Include(r => r.Reviewer)
                .Include(r => r.ExamPlan)
                    .ThenInclude(p => p.Subject)
                .Include(r => r.Distribution)
                .ToListAsync();

            var viewModels = reviews.Select(r => new ReviewExamDetailViewModel
            {
                ReviewID = r.ReviewID,
                ExamTitle = r.ExamPlan?.Name,
                LecturerName = r.Reviewer?.FullName,
                SubjectName = r.ExamPlan?.Subject?.SubjectName,
                SubmittedDate = r.ReviewedAt,
                Status = r.Status,
                Comment = r.Comment,
                DueDate = r.DueDate,
                ReviewedAt = r.ReviewedAt
            }).ToList();

            return View(viewModels); // View: ReviewExam.cshtml
        }

        // GET: /ExamReview/ReviewDetails/5
        public async Task<IActionResult> ReviewDetails(int id)
        {
            var review = await _context.ReviewExams
                .Include(r => r.Reviewer)
                .Include(r => r.ExamPlan)
                    .ThenInclude(p => p.Subject)
                .Include(r => r.Distribution)
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null)
            {
                return NotFound();
            }

            var vm = new ReviewExamDetailViewModel
            {
                ReviewID = review.ReviewID,
                ExamTitle = review.ExamPlan?.Name,
                LecturerName = review.Reviewer?.FullName,
                SubjectName = review.ExamPlan?.Subject?.SubjectName,
                SubmittedDate = review.ReviewedAt,
                Comment = review.Comment,
                Status = review.Status,
                DueDate = review.DueDate,
                ReviewedAt = review.ReviewedAt
            };

            return View(vm); // View: ReviewDetails.cshtml
        }
    }
}

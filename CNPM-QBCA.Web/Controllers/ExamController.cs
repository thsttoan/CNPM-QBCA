using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;

namespace QBCA.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExamController(ApplicationDbContext context)
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
        // GET: Exam
        public async Task<IActionResult> SubmittedExams()
        {
            var exams = await _context.Exams
                .Include(e => e.ExamPlanDistribution)
                    .ThenInclude(d => d.ExamPlan)
                .Include(e => e.Submitter)
                .ToListAsync();
            return View(exams);
        }

        // GET: Exam/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var exam = await _context.Exams
                    .Include(e => e.ExamPlanDistribution)
                    .Include(e => e.Submitter)
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                    .FirstOrDefaultAsync(e => e.ExamID == id);
            if (exam == null)
            {
                return NotFound();
            }
            return View(exam);
        }

        // GET: Exam/Create
        public IActionResult SubmitExam()
        {
            // Lọc distribution của Subject Leader
            var distributions = _context.ExamPlanDistributions
                                .Where(d => d.AssignedManagerRoleID == 4)
                                .Select(d => new
                                {
                                    d.DistributionID,
                                    d.NumberOfQuestions
                                }).ToList();

            ViewBag.Distributions = distributions;

            ViewBag.Questions = _context.Questions
                .Select(q => new { q.QuestionID, q.Content })
                .ToList();

            return View();
        }
        // POST: Exam/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(ExamCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst("UserID")?.Value;
                int submittedBy = 0;
                if (int.TryParse(userIdClaim, out int userId))
                    submittedBy = userId;

                var distribution = await _context.ExamPlanDistributions
                    .FirstOrDefaultAsync(d => d.DistributionID == model.DistributionID);
                
                if (distribution == null)
                {
                    ModelState.AddModelError("DistributionID", "Invalid Distribution selected.");
                    ViewBag.Distributions = _context.ExamPlanDistributions.ToList();
                    return View(model);
                }

                var exam = new Exam
                {
                    Title = model.Title,
                    DistributionID = model.DistributionID,
                    ExamPlanID = distribution.ExamPlanID,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    SubmitDate = DateTime.Now,
                    SubmittedBy = submittedBy,
                    Status = "Submitted", // hoặc "Pending", tùy quy định
                    Description = model.Description,
                };

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                // Thêm câu hỏi nếu có
                foreach (var qid in model.QuestionIDs)
                {
                    _context.ExamQuestions.Add(new ExamQuestion
                    {
                        ExamID = exam.ExamID,
                        QuestionID = qid,
                        ExamPlanID = distribution.ExamPlanID
                    });
                }

                // Gửi thông báo
                var rndUsers = _context.Users.Where(u => u.RoleID == 1).ToList();
                foreach (var rnd in rndUsers)
                {
                    AddNotification(
                        userId: rnd.UserID,
                        message: $"Đề thi '{exam.Title}' đã được nộp.",
                        relatedType: "Exam",
                        relatedId: exam.ExamID,
                        createdBy: exam.SubmittedBy
                    );
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SubmittedExams));
            }

            // Gửi lại dữ liệu xuống view nếu lỗi
            ViewBag.Distributions = _context.ExamPlanDistributions
                .Where(d => d.AssignedManagerRoleID == 4)
                .Select(d => new
                {
                    d.DistributionID,
                    d.NumberOfQuestions
                }).ToList();

            ViewBag.Questions = _context.Questions
                .Select(q => new { q.QuestionID, q.Content })
                .ToList();

            return View(model);
        }

        // GET: Exam/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.ExamID == id);

            if (exam == null)
            {
                return NotFound();
            }

            var model = new ExamCreateViewModel
            {
                ExamID = exam.ExamID,
                Title = exam.Title,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                DistributionID = exam.DistributionID,
                Description = exam.Description,
                QuestionIDs = exam.ExamQuestions.Select(eq => eq.QuestionID).ToList()
            };

            var distributions = _context.ExamPlanDistributions
                                .Where(d => d.AssignedManagerRoleID == 4)
                                .Select(d => new
                                {
                                    d.DistributionID,
                                    d.NumberOfQuestions
                                }).ToList();

            ViewBag.Distributions = distributions;

            ViewBag.Questions = _context.Questions
                .Select(q => new { q.QuestionID, q.Content })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamCreateViewModel model)
        {
            if (id != model.ExamID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var exam = await _context.Exams
                    .Include(e => e.ExamQuestions)
                    .FirstOrDefaultAsync(e => e.ExamID == id);

                if (exam == null)
                {
                    return NotFound();
                }

                var userIdClaim = User.FindFirst("UserID")?.Value;
                int submittedBy = 0;
                if (int.TryParse(userIdClaim, out int userId))
                    submittedBy = userId;

                var distribution = await _context.ExamPlanDistributions
                    .FirstOrDefaultAsync(d => d.DistributionID == model.DistributionID);

                if (distribution == null)
                {
                    ModelState.AddModelError("DistributionID", "Invalid Distribution selected.");
                    ViewBag.Distributions = _context.ExamPlanDistributions.ToList();
                    ViewBag.Questions = _context.Questions.ToList();
                    return View(model);
                }

                // Cập nhật thông tin đề thi
                exam.Title = model.Title;
                exam.StartDate = model.StartDate;
                exam.EndDate = model.EndDate;
                exam.DistributionID = model.DistributionID;
                exam.SubmitDate = DateTime.Now;
                exam.SubmittedBy = submittedBy;
                exam.Status = "Updated"; // hoặc giữ là "Submitted" nếu không có trạng thái khác
                exam.Description = model.Description;

                // Xoá các câu hỏi cũ
                var oldQuestions = _context.ExamQuestions.Where(eq => eq.ExamID == exam.ExamID);
                _context.ExamQuestions.RemoveRange(oldQuestions);

                // Thêm câu hỏi mới
                foreach (var qid in model.QuestionIDs)
                {
                    _context.ExamQuestions.Add(new ExamQuestion
                    {
                        ExamID = exam.ExamID,
                        QuestionID = qid,
                        ExamPlanID = distribution.ExamPlanID
                    });
                }

                // Gửi thông báo
                var rndUsers = _context.Users.Where(u => u.RoleID == 1).ToList();
                foreach (var rnd in rndUsers)
                {
                    AddNotification(
                        userId: rnd.UserID,
                        message: $"Exam '{exam.Title}' has been updated.",
                        relatedType: "Exam",
                        relatedId: exam.ExamID,
                        createdBy: submittedBy
                    );
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exam updated successfully.";
                return RedirectToAction(nameof(SubmittedExams));
            }

            // Gửi lại dữ liệu nếu lỗi
            ViewBag.Distributions = _context.ExamPlanDistributions.ToList();
            ViewBag.Questions = _context.Questions.ToList();
            return View(model);
        }

        // GET: Exam/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            return View(exam);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SubmittedExams));
        }
        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.ExamID == id);
        }
    }
}

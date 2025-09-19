using CNPM_QBCA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QBCA.Data;
using QBCA.Models;

namespace CNPM_QBCA.Controllers
{
    [Route("Exam")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Exam/MockExam
        [HttpGet("MockExam")]
        public IActionResult MockExam()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var assignment = _context.TaskAssignments
                .Include(t => t.ExamPlan)
                .Include(t => t.Distribution)
                    .ThenInclude(d => d.Questions)
                .Where(t => t.AssignedTo == userId)
                .OrderByDescending(t => t.AssignmentID)
                .FirstOrDefault();

            if (assignment == null)
                return NotFound("Bạn chưa được giao đề thi thử nào.");

            var vm = new MockExamViewModel
            {
                AssignmentID = assignment.AssignmentID,
                ExamTitle = assignment.ExamPlan?.Title ?? "Untitled Exam",
                Questions = assignment.Distribution?.Questions?.ToList() ?? new List<Question>(),
                Answers = new Dictionary<int, string>()
            };

            return View(vm); // Views/Exam/MockExam.cshtml
        }

        // POST: /Exam/MockExam
        [HttpPost("MockExam")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MockExam(MockExamViewModel vm)
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // ✅ Kiểm tra assignment có tồn tại và thuộc về giảng viên hiện tại không
            var assignment = await _context.TaskAssignments
                .FirstOrDefaultAsync(a => a.AssignmentID == vm.AssignmentID && a.AssignedTo == userId);

            if (assignment == null)
            {
                ModelState.AddModelError(string.Empty, "Assignment không tồn tại hoặc không hợp lệ.");
                return RedirectToAction("MockExam");
            }

            var mockExam = new MockExam
            {
                AssignmentID = assignment.AssignmentID,
                LecturerID = userId,
                SubmittedAt = DateTime.UtcNow,
                AnswersJson = JsonConvert.SerializeObject(vm.Answers),
                Feedback = vm.Feedback
            };

            try
            {
                _context.MockExam.Add(mockExam);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu bài thi thử. Hãy thử lại sau.");
                return RedirectToAction("MockExam");
            }

            // Gửi thông báo cho người giao đề
            var notification = new Notification
            {
                UserID = assignment.AssignedBy,
                Message = $"Giảng viên đã hoàn thành đề thi thử cho Assignment #{assignment.AssignmentID}",
                Status = "unread",
                RelatedEntityType = "MockExam",
                RelatedEntityID = mockExam.MockExamID,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bạn đã hoàn thành đề thi thử và gửi nhận xét.";
            return RedirectToAction("TeamTasks", "Task");
        }
    }
}

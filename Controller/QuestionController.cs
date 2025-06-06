using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;

namespace QBCA.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CHO R&D: chỉ xem
        public async Task<IActionResult> QuestionBank()
        {
            var questions = await _context.Questions
                .Include(q => q.Subject)
                .Include(q => q.CLO)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Creator)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return View("QuestionBank", questions); // Views/Question/QuestionBank.cshtml
        }

        // CHO SUBJECT LEADER: chỉ xem
        public async Task<IActionResult> ReviewQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Subject)
                .Include(q => q.CLO)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Creator)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return View("ReviewQuestions", questions); // Views/Question/ReviewQuestions.cshtml
        }

        // CHO LECTURER: xem + CRUD
        public async Task<IActionResult> Questions()
        {
            var questions = await _context.Questions
                .Include(q => q.Subject)
                .Include(q => q.CLO)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Creator)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return View("Questions", questions); // Views/Question/Questions.cshtml
        }

        // GET: /Question/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: /Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            question.CreatedAt = System.DateTime.UtcNow;
            question.CreatedBy = 1; // Thay bằng lấy từ user đăng nhập nếu có

            if (!ModelState.IsValid)
            {
                LoadDropdowns();

                // XÓA giá trị navigation property để tránh lỗi required navigation
                question.Subject = null;
                question.CLO = null;
                question.DifficultyLevel = null;
                question.Creator = null;
                question.ExamQuestions = null;
                question.DuplicateCheckResults = null;
                question.SimilarQuestions = null;

                return View(question);
            }

            try
            {
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question added successfully!";
                return RedirectToAction(nameof(Questions));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);

                LoadDropdowns();

                question.Subject = null;
                question.CLO = null;
                question.DifficultyLevel = null;
                question.Creator = null;
                question.ExamQuestions = null;
                question.DuplicateCheckResults = null;
                question.SimilarQuestions = null;

                return View(question);
            }
        }

        // GET: /Question/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            LoadDropdowns();
            return View(question);
        }

        // POST: /Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Question question)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();

                question.Subject = null;
                question.CLO = null;
                question.DifficultyLevel = null;
                question.Creator = null;
                question.ExamQuestions = null;
                question.DuplicateCheckResults = null;
                question.SimilarQuestions = null;

                return View(question);
            }
            try
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question updated successfully!";
                return RedirectToAction(nameof(Questions));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);

                LoadDropdowns();

                question.Subject = null;
                question.CLO = null;
                question.DifficultyLevel = null;
                question.Creator = null;
                question.ExamQuestions = null;
                question.DuplicateCheckResults = null;
                question.SimilarQuestions = null;

                return View(question);
            }
        }

        // POST: /Question/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question deleted successfully!";
            }
            return RedirectToAction(nameof(Questions));
        }

        /// <summary>
        /// Nạp lại dữ liệu cho các dropdown để tránh NullReferenceException trong View
        /// </summary>
        private void LoadDropdowns()
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            ViewBag.CLOs = _context.CLOs.ToList();
            ViewBag.DifficultyLevels = _context.DifficultyLevels.ToList();
        }
    }
}

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

        // R&D: Only view
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

        // SUBJECT LEADER: Only view
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

        // LECTURER: View + CRUD
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
            question.CreatedBy = 1; // Chage this to the actual user ID

            if (!ModelState.IsValid)
            {
                LoadDropdowns();

                // Delete navigation properties to avoid circular references
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

        private void LoadDropdowns()
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            ViewBag.CLOs = _context.CLOs.ToList();
            ViewBag.DifficultyLevels = _context.DifficultyLevels.ToList();
        }

        // GET: /Question/ApproveQuestionList
        public async Task<IActionResult> ApproveQuestionList()
        {
            var questions = await _context.Questions
                .Where(q => q.Status == "Inactive")
                .Include(q => q.Subject)
                .Include(q => q.CLO)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Creator)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        
            return View("ApproveQuestionList", questions);
        }
        
        // POST: /Question/ApproveQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveQuestionList(List<int> selectedQuestionIds)
        {
            if (selectedQuestionIds == null || selectedQuestionIds.Count == 0)
            {
                TempData["Error"] = "No question was selected.";
                return RedirectToAction(nameof(ApproveQuestionList));
            }
        
            var questions = await _context.Questions
                .Where(q => selectedQuestionIds.Contains(q.QuestionID))
                .ToListAsync();
        
            foreach (var question in questions)
            {
                question.Status = "Active";
            }
        
            await _context.SaveChangesAsync();
            TempData["Success"] = "Approved questions successfully!";
            return RedirectToAction(nameof(ApproveQuestionList));
        }
    }
}

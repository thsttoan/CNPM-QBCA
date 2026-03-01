using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
            return View("QuestionBank", questions);
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
            return View("ReviewQuestions", questions);
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
            return View("Questions", questions);
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
        public async Task<IActionResult> Create(Question question, bool forceCreate = false)
        {
            question.CreatedAt = System.DateTime.UtcNow;
            int.TryParse(User.FindFirst("UserID")?.Value, out int createdBy);
            question.CreatedBy = createdBy;

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                ClearNavProperties(question);
                return View(question);
            }

            // ── AUTO DUPLICATE CHECK ─────────────────────────────────────
            if (!forceCreate)
            {
                try
                {
                    var config = HttpContext.RequestServices
                        .GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                    var jinaKey = config["ApiKeys:Jina"];

                    if (!string.IsNullOrEmpty(jinaKey))
                    {
                        var jinaService = new JinaService(jinaKey);

                        // Gọi Jina API 1 lần cho câu hỏi mới
                        var newEmbedding = await jinaService.GetEmbeddingAsync(question.Content);
                        // Cache embedding vào DB luôn
                        question.Embedding = Newtonsoft.Json.JsonConvert.SerializeObject(newEmbedding);

                        // Lấy các câu hỏi cùng môn có embedding sẵn trong DB (0 API call thêm)
                        var existingQuestions = await _context.Questions
                            .Where(q => q.SubjectID == question.SubjectID && q.Embedding != null)
                            .Select(q => new { q.QuestionID, q.Content, q.Embedding })
                            .ToListAsync();

                        var duplicates = new List<(int id, string content, double score)>();
                        foreach (var eq in existingQuestions)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(eq.Embedding)) continue;
                                var storedEmb = Newtonsoft.Json.JsonConvert.DeserializeObject<float[]>(eq.Embedding);
                                if (storedEmb == null) continue;
                                var sim = JinaService.CosineSimilarity(newEmbedding, storedEmb);
                                if (sim > 0.92)
                                    duplicates.Add((eq.QuestionID, eq.Content, sim));
                            }
                            catch { /* bỏ qua câu có embedding bị lỗi */ }
                        }

                        if (duplicates.Count > 0)
                        {
                            LoadDropdowns();
                            ClearNavProperties(question);
                            ViewBag.DuplicateWarning = duplicates
                                .OrderByDescending(d => d.score)
                                .Select(d => new DuplicateHint
                                {
                                    Id      = d.id,
                                    Preview = d.content.Length > 120
                                                ? d.content.Substring(0, 120) + "..."
                                                : d.content,
                                    Score   = System.Math.Round(d.score * 100, 1)
                                })
                                .ToList();
                            return View(question);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Nếu Jina API lỗi vẫn cho phép lưu, hiện warning nhẹ
                    ViewBag.ApiWarning = $"Không thể kiểm tra trùng lặp: {ex.Message}";
                }
            }
            // ────────────────────────────────────────────────────────────

            try
            {
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Câu hỏi đã được thêm thành công!";
                return RedirectToAction(nameof(Questions));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);
                LoadDropdowns();
                ClearNavProperties(question);
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
                ClearNavProperties(question);
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
                ClearNavProperties(question);
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
            ViewBag.Subjects         = _context.Subjects.ToList();
            ViewBag.CLOs             = _context.CLOs.ToList();
            ViewBag.DifficultyLevels = _context.DifficultyLevels.ToList();
        }

        private static void ClearNavProperties(Question question)
        {
            question.Subject               = null;
            question.CLO                   = null;
            question.DifficultyLevel       = null;
            question.Creator               = null;
            question.ExamQuestions         = new List<ExamQuestion>();
            question.DuplicateCheckResults = new List<DuplicateCheckResult>();
            question.SimilarQuestions      = new List<DuplicateCheckResult>();
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
                question.Status = "Active";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Approved questions successfully!";
            return RedirectToAction(nameof(ApproveQuestionList));
        }
    }

    // DTO truyền thông tin câu hỏi trùng sang View
    public class DuplicateHint
    {
        public int     Id      { get; set; }
        public string? Preview { get; set; }
        public double  Score   { get; set; }
    }
}

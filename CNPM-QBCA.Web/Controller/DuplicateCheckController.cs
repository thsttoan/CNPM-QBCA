using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBCA.Data;
using QBCA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System;

namespace QBCA.Controllers
{
    public class DuplicateCheckController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _aiService;

        public DuplicateCheckController(ApplicationDbContext context)
        {
            _context = context;
            _aiService = new GeminiService("AIzaSyC4ZDWUHt2e-BuZ169dIpW6O2rPQ2FmY2M");
        }

        // GET: DuplicateCheck/HomeCheck
        public async Task<IActionResult> HomeCheck()
        {
            var history = await _context.DuplicateCheckResults
                .Include(r => r.Question)
                .Include(r => r.SimilarQuestion)
                .OrderByDescending(r => r.CheckedAt)
                .ToListAsync();
            return View(history);
        }

        // POST: DuplicateCheck/DeleteHistory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var record = await _context.DuplicateCheckResults.FindAsync(id);
            if (record != null)
            {
                _context.DuplicateCheckResults.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(HomeCheck));
        }

        // GET: DuplicateCheck/Check
        public async Task<IActionResult> Check()
        {
            var questions = await _context.Questions
                .Include(q => q.Subject)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View(questions);
        }

        // POST: DuplicateCheck/Check
        [HttpPost]
        public async Task<IActionResult> Check(int id)
        {
            var target = await _context.Questions
                .Include(q => q.Subject)
                .FirstOrDefaultAsync(q => q.QuestionID == id);
            if (target == null) return NotFound();

            var others = await _context.Questions
                .Where(q => q.QuestionID != id && q.SubjectID == target.SubjectID)
                .ToListAsync();

            var duplicates = new List<(Question, double)>();
            var results = new List<DuplicateCheckResult>();

            try
            {
                var targetEmbedding = await _aiService.GetEmbeddingAsync(target.Content);
                var targetTopic = ExtractMainTopic(target.Content);

                foreach (var q in others)
                {
                    var qTopic = ExtractMainTopic(q.Content);

                    // Continue if the main topics do not match
                    if (!string.IsNullOrWhiteSpace(targetTopic) && !string.IsNullOrWhiteSpace(qTopic) && targetTopic != qTopic)
                    {
                        continue;
                    }

                    var emb = await _aiService.GetEmbeddingAsync(q.Content);
                    var sim = GeminiService.CosineSimilarity(targetEmbedding, emb);
                    if (sim > 0.92)
                    {
                        duplicates.Add((q, sim));

                        // Check if this duplicate check result already exists
                        bool exists = await _context.DuplicateCheckResults.AnyAsync(r =>
                            r.QuestionID == target.QuestionID &&
                            r.SimilarQuestionID == q.QuestionID &&
                            Math.Abs(r.SimilarityScore - sim) < 0.0001
                        );
                        DuplicateCheckResult result;
                        if (!exists)
                        {
                            result = new DuplicateCheckResult
                            {
                                QuestionID = target.QuestionID,
                                SimilarQuestionID = q.QuestionID,
                                SimilarityScore = sim,
                                CheckType = "Gemini",
                                CheckedAt = DateTime.UtcNow,
                                Question = target,
                                SimilarQuestion = q
                            };
                            _context.DuplicateCheckResults.Add(result);
                        }
                        else
                        {
                            result = await _context.DuplicateCheckResults
                                .Include(r => r.Question)
                                .Include(r => r.SimilarQuestion)
                                .FirstOrDefaultAsync(r =>
                                    r.QuestionID == target.QuestionID &&
                                    r.SimilarQuestionID == q.QuestionID &&
                                    Math.Abs(r.SimilarityScore - sim) < 0.0001
                                );
                        }
                        if (result != null)
                        {
                            results.Add(result);
                        }
                    }
                }
                await _context.SaveChangesAsync();

                var model = new DuplicateResultViewModel
                {
                    Question = target,
                    Duplicates = duplicates.OrderByDescending(x => x.Item2).ToList()
                };

                return View("CheckResult", model);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Check");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Check");
            }
        }

        public string ExtractMainTopic(string content)
        {
            content = content.ToLowerInvariant();
            var keyword = "";

            if (content.Contains("công thức tính"))
            {
                var parts = content.Split(new[] { "công thức tính" }, StringSplitOptions.None);
                if (parts.Length > 1) keyword = parts[1].Trim(new[] { '.', ' ', ':' });
            }
            else if (content.Contains("tính"))
            {
                var parts = content.Split(new[] { "tính" }, StringSplitOptions.None);
                if (parts.Length > 1) keyword = parts[1].Trim(new[] { '.', ' ', ':' });
            }
            return keyword;
        }
    }

    // Service for calling Gemini API
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _http;

        public GeminiService(string apiKey)
        {
            _apiKey = apiKey;
            _http = new HttpClient();
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/embedding-001:embedContent?key={_apiKey}";
            var body = new
            {
                content = new
                {
                    parts = new[] { new { text = text } }
                }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            var resp = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(result);
            var arr = obj["embedding"]?["values"];
            if (arr == null) throw new System.Exception("No embedding received from Gemini API.");
            return arr.Select(v => (float)v).ToArray();
        }

        public static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new System.Exception("Embeddings have different lengths.");

            double dot = 0.0;
            double magA = 0.0;
            double magB = 0.0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dot += vectorA[i] * vectorB[i];
                magA += vectorA[i] * vectorA[i];
                magB += vectorB[i] * vectorB[i];
            }
            return dot / (System.Math.Sqrt(magA) * System.Math.Sqrt(magB));
        }
    }

    // ViewModel for duplicate result
    public class DuplicateResultViewModel
    {
        public Question Question { get; set; }
        public List<(Question, double)> Duplicates { get; set; } = new();
    }
}
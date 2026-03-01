using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QBCA.Data;
using QBCA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using System;

namespace QBCA.Controllers
{
    public class DuplicateCheckController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JinaService _aiService;

        public DuplicateCheckController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            var jinaKey = configuration["ApiKeys:Jina"]
                ?? throw new InvalidOperationException("Jina API key not found in configuration.");
            _aiService = new JinaService(jinaKey);
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

                    // Bỏ qua nếu chủ đề chính không khớp
                    if (!string.IsNullOrWhiteSpace(targetTopic) && !string.IsNullOrWhiteSpace(qTopic) && targetTopic != qTopic)
                    {
                        continue;
                    }

                    var emb = await _aiService.GetEmbeddingAsync(q.Content);
                    var sim = JinaService.CosineSimilarity(targetEmbedding, emb);
                    if (sim > 0.92)
                    {
                        duplicates.Add((q, sim));

                        // Kiểm tra xem kết quả đã tồn tại chưa
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
                                CheckType = "Jina",
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
                TempData["Error"] = $"Lỗi kết nối Jina API: {ex.Message}";
                return RedirectToAction("Check");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}";
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

    // Service gọi Jina Embeddings API
    public class JinaService
    {
        private readonly string _apiKey;
        private static readonly HttpClient _http = new HttpClient();

        // Jina embeddings-v3: 1024 chiều, hỗ trợ đa ngôn ngữ (kể cả tiếng Việt)
        private const string JinaModel = "jina-embeddings-v3";
        private const string JinaEndpoint = "https://api.jina.ai/v1/embeddings";

        public JinaService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var body = new
            {
                input = new[] { text },
                model = JinaModel,
                task = "text-matching"   // tối ưu cho so sánh độ tương đồng
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            var request = new HttpRequestMessage(HttpMethod.Post, JinaEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(request);

            if (!resp.IsSuccessStatusCode)
            {
                var errBody = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Jina API lỗi {(int)resp.StatusCode}: {errBody}");
            }

            var result = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(result);
            var arr = obj["data"]?[0]?["embedding"];
            if (arr == null) throw new Exception("Không nhận được embedding từ Jina API.");
            return arr.Select(v => (float)v).ToArray();
        }

        public static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new Exception("Hai vector embedding có độ dài khác nhau.");

            double dot = 0.0, magA = 0.0, magB = 0.0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dot  += vectorA[i] * vectorB[i];
                magA += vectorA[i] * vectorA[i];
                magB += vectorB[i] * vectorB[i];
            }
            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }
    }

    // ViewModel cho kết quả kiểm tra trùng lặp
    public class DuplicateResultViewModel
    {
        public Question Question { get; set; }
        public List<(Question, double)> Duplicates { get; set; } = new();
    }
}
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace QBCA.Services
{
    public class GeminiDuplicateService
    {
        private readonly string _apiKey;
        private readonly HttpClient _http;

        public GeminiDuplicateService(string apiKey)
        {
            _apiKey = apiKey;
            _http = new HttpClient();
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            // Cập nhật model lên text-embedding-004 để có kết quả tốt hơn
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={_apiKey}";

            var body = new
            {
                model = "models/text-embedding-004", // Khai báo model rõ ràng trong body
                content = new
                {
                    parts = new[] { new { text = text } }
                }
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            var resp = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

            // Đọc nội dung lỗi nếu có trước khi ném Exception
            if (!resp.IsSuccessStatusCode)
            {
                var errorContent = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error: {resp.StatusCode} - {errorContent}");
            }

            var result = await resp.Content.ReadAsStringAsync();
            var obj = Newtonsoft.Json.Linq.JObject.Parse(result);

            // Lưu ý: Kết quả trả về của endpoint embedContent nằm trong object "embedding"
            var arr = obj["embedding"]?["values"];

            if (arr == null) throw new Exception("Không nhận được dữ liệu embedding từ Gemini API.");

            return arr.Select(v => (float)v).ToArray();
        }

        public static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new System.Exception("Embeddings come in different lengths.");

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
}
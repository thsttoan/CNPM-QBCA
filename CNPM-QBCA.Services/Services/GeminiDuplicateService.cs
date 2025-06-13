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
            if (arr == null) throw new System.Exception("Không nhận được embedding từ Gemini API.");
            return arr.Select(v => (float)v).ToArray();
        }

        public static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new System.Exception("Embedding có độ dài khác nhau.");

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
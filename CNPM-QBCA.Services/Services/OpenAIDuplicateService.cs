using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace QBCA.Services
{
    public class OpenAIDuplicateService
    {
        private readonly string _apiKey;

        public OpenAIDuplicateService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var endpoint = "https://api.openai.com/v1/embeddings";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var content = new StringContent(
                JsonConvert.SerializeObject(new { input = text, model = "text-embedding-ada-002" }),
                Encoding.UTF8, "application/json"
            );

            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(json);
            var arr = obj.data[0].embedding;
            return ((IEnumerable<dynamic>)arr).Select(x => (float)x).ToArray();
        }

        public double CosineSimilarity(float[] vec1, float[] vec2)
        {
            double dot = 0, mag1 = 0, mag2 = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                dot += vec1[i] * vec2[i];
                mag1 += vec1[i] * vec1[i];
                mag2 += vec2[i] * vec2[i];
            }
            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }
}
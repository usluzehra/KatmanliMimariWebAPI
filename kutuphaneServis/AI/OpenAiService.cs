using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace kutuphaneServis.AI
{
    // sealed: miras alınmaz
    public sealed class OpenAiService : IAIService
    {
        private readonly HttpClient _http;
        private readonly string _model;
        private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public OpenAiService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _model = cfg["AI:Model"] ?? "gpt-4o-mini";
        }

        public async Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default)
        {
            var req = new
            {
                model = _model,
                temperature = 0.35,
                max_tokens = 700,
                messages = new object[]
                {
                    new { role = "system", content = "Türkçe konuş. Kısa ve anlaşılır cevaplar ver." },
                    new { role = "user", content = prompt }
                }
            };

            using var resp = await _http.PostAsJsonAsync("chat/completions", req, _json, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new ApplicationException($"OpenAI hata: {resp.StatusCode} - {body}");

            // OpenAI yanıt formatı:
            // { choices: [ { message: { content: "..." } } ] }
            var root = JsonNode.Parse(body)!;
            var content = root["choices"]?[0]?["message"]?["content"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(content))
                throw new ApplicationException("OpenAI boş yanıt döndürdü.");

            return content.Trim();
        }
    }
}

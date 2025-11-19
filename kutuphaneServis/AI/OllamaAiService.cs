using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kutuphaneServis.AI
{
    //sealed: miras alınamaz
    public sealed class OllamaAiService : IAIService
    {
        private readonly HttpClient _http;
        private readonly string _model;
        // buradaki json llm'in iç yanıt cevabını deserialize etmek için
        private readonly JsonSerializerOptions _json = new (JsonSerializerDefaults.Web);

        public OllamaAiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _model = configuration["AI:Model"] ?? "llama3";
          
        }
        public async Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default)
        {
            var req = new 
            { 
                model = _model, 
                prompt, 
                stream = false , 
                options = new { 
                    temperature = 0.2,
                    num_predict =128,
                    top_p= 0.9
                } };
            using var resp = await _http.PostAsJsonAsync("/api/generate", req, _json, ct);
           
            var body = await resp.Content.ReadAsStringAsync(ct);
            
            if (!resp.IsSuccessStatusCode)
            {
                throw new ApplicationException($"AI servis hatası: {resp.StatusCode} {body}");
            }

            using var doc= await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            return doc.RootElement.GetProperty("response").GetString()!;
        }
    }
}

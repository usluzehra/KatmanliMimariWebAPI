using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace UdemyKutuphaneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController(IAIService ai, IBookRepository repo) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Recommend([FromBody] RecommendRequest req, CancellationToken ct)
        {
            var max = Math.Clamp(req.MaxResults ?? 5, 1, 10);
            var books = await repo.GetAllAsync(ct);
            if (books.Count == 0)
            {
                return Ok(new RecommendationResponse("Kütüphaneye kayıtlı kitap bulunamadı.", new()));
            }

            var prompt = BuildPrompt(req.Query, books, max);


            try
            {
            
                var aiTask = ai.GenerateTextAsync(prompt, ct);
                var finished = await Task.WhenAny(aiTask, Task.Delay(TimeSpan.FromSeconds(300), ct));
                if (finished != aiTask)
                    return Problem("LLM yanıtı zaman aşımına uğradı.", statusCode: 504);

                var text = (await aiTask)?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                    return Problem("LLM boş yanıt döndürdü.", statusCode: 502);

                // Düz metin olarak geri ver
                return Content(text!, "text/plain; charset=utf-8");
            }
            catch (OperationCanceledException)
            {
                return Problem("İstek iptal edildi.", statusCode: 499);
            }
            catch (Exception ex)
            {
                return Problem($"AI hata: {ex.Message}", statusCode: 502);
            }
        }

        private static string BuildPrompt(string userQuery, IEnumerable<BookCandidate> candidates, int maxResults)
        {
            var list = string.Join("\n", candidates.Select(c =>
                $"- id:{c.Id}, başlık:{c.Title}, sayfa:{c.CountOfPage}, açıklama:{(c.Description ?? "").Replace("\n", " ")}"));

            return $@"
            Rolün: Bir kütüphane asistanısın.
            Kullanıcı isteği: ""{userQuery}""

           
                Aşağıda KÜTÜPHANE KATALOĞU var. Bu listedeki bilgiler doğrudur:
                {list}

                GÖREVİN (adım adım ve sırasıyla):
                1) Kullanıcı isteğini analiz et. İstek bir **yazar** (örn. Tolstoy) veya **kategori/tür** (örn. psikoloji, tarih, polisiye, aşk) içeriyorsa algıla.
                   - Yazım hatalarına toleranslı ol (örn. ""toystoy"" → ""Tolstoy"").
                2) Önce KATALOĞU tara:
                   - Eğer istek **yazar odaklı** ise: aynı yazara ait kitaplar varsa en alakalılarını öner.
                   - Eğer istek **kategori odaklı** ise: aynı kategoriye ait kitaplar varsa  en alakalılarını öner.
                3) Katalogda uygun sonuç **yoksa**, bunu açıkça bir cümleyle belirt ve **kütüphane dışından** gerçek, bilinen kitapları öner.
                   - Dış önerilerin sonuna mutlaka **(Kütüphane Dışı)** etiketi koy.
                   - Dış öneriler istenen yazarın gerçek eserleri (örn. Tolstoy → Savaş ve Barış, Anna Karenina) ya da istenen kategoriyle gerçekten ilişkili eserler olmalıdır.
                4) **Asla uydurma bilgi yazma.** Katalogda sayfa sayısı yoksa “– sayfa sayısı: bilinmiyor” de veya hiç yazma.
                5) En fazla {maxResults} öneri ver.
                6) ÇIKTI BİÇİMİ  şu şablona tam uysun (markdown/JSON kullanma):
                   - Giriş cümlesi (varsa “kütüphanede bulunamadı” bilgisini içerir).
                   - Ardından numaralı liste:
                     1) Kitap Adı – çok kısa açıklama – Yazar [– sayfa sayısı: N] [(Kütüphane Dışı) varsa]
                     2) ...
                7) Katalogdan seçtiğin kitaplarda **(Kütüphane Dışı)** etiketi KULLANMA. Bu etiket sadece dış önerilerde kullanılacak.

                ÖRNEK 1 (Yazar var, katalogda yok):
                Kütüphanemizde Tolstoy'a ait kitap bulunmamaktadır. Ancak yazarın bilinen eserlerinden bazılarını önerebilirim:
                1) Savaş ve Barış – savaşın gölgesinde insan doğası – Lev Tolstoy (Kütüphane Dışı)
                2) Anna Karenina – trajik bir aşk ve toplum baskısı – Lev Tolstoy (Kütüphane Dışı)

                ÖRNEK 2 (Kategori var, katalogda var):
                Psikoloji kategorisinde kütüphanemizde bulunan kitaplardan öneriler:
                1) kitap adı – insan davranışlarına kısa bir bakış – Yazar Adı
                 Şimdi yukarıdaki kurallara % 100 uyarak yanıt ver
";
        }

    }
}

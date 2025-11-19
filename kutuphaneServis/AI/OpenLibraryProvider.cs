using System.Text.Json;

namespace kutuphaneServis.AI;

public record ExternalBook(string Title, string Author, string? Category, string? InfoUrl);

public interface IExternalBookProvider
{
    Task<IReadOnlyList<ExternalBook>> SearchAsync(string query, int take, CancellationToken ct);
}

public sealed class OpenLibraryProvider(HttpClient http) : IExternalBookProvider
{
    private readonly HttpClient _http = http;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<ExternalBook>> SearchAsync(string query, int take, CancellationToken ct)
    {
        using var resp = await _http.GetAsync($"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}&limit={take}", ct);
        resp.EnsureSuccessStatusCode();
        using var s = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(s, cancellationToken: ct);

        var docs = doc.RootElement.TryGetProperty("docs", out var arr) ? arr : default;
        if (docs.ValueKind != JsonValueKind.Array) return Array.Empty<ExternalBook>();

        return docs.EnumerateArray()
            .Select(d => new ExternalBook(
                Title: d.GetProperty("title").GetString() ?? "Bilinmiyor",
                Author: d.TryGetProperty("author_name", out var a) && a.ValueKind == JsonValueKind.Array
                    ? a[0].GetString() ?? "Bilinmiyor"
                    : "Bilinmiyor",
                Category: d.TryGetProperty("subject", out var subs) && subs.ValueKind == JsonValueKind.Array
                    ? subs[0].GetString()
                    : null,
                InfoUrl: d.TryGetProperty("key", out var key)
                    ? $"https://openlibrary.org{key.GetString()}"
                    : null))
            .Take(take)
            .ToList();
    }
}

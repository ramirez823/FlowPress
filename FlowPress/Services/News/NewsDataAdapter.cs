using System.Text.Json;
using FlowPress.Models;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services.News;

public class NewsDataAdapter : INewsAdapter
{
    private readonly HttpClient _http;

    public string SourceName => "NewsData";
    public string SourceUrl => "https://newsdata.io/api/1/latest";

    public NewsDataAdapter(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SourceItemViewModel>> FetchAsync(string apiKey, int count)
    {
        var url = $"https://newsdata.io/api/1/latest?apikey={apiKey}&language=es&size={count}";
        var response = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var articles = doc.RootElement.GetProperty("results");

        var source = new Source
        {
            Name = SourceName,
            Url = SourceUrl,
            RequiresSecret = true
        };

        var results = new List<SourceItemViewModel>();

        foreach (var article in articles.EnumerateArray())
        {
            string? author = null;
            if (article.TryGetProperty("creator", out var creators) &&
                creators.ValueKind == JsonValueKind.Array &&
                creators.GetArrayLength() > 0)
            {
                author = creators[0].GetString();
            }

            string? primaryCategory = null;
            if (article.TryGetProperty("category", out var categories) &&
                categories.ValueKind == JsonValueKind.Array &&
                categories.GetArrayLength() > 0)
            {
                primaryCategory = categories[0].GetString();
            }

            var normalized = new NormalizedContent
            {
                ExternalId = article.TryGetProperty("article_id", out var id) ? id.GetString() : null,
                Title = article.TryGetProperty("title", out var t) ? t.GetString() : null,
                Summary = article.TryGetProperty("description", out var d) ? d.GetString() : null,
                Content = article.TryGetProperty("content", out var c) ? c.GetString() : null,
                Url = article.TryGetProperty("link", out var link) ? link.GetString() : null,
                Author = author,
                PublishedAt = article.TryGetProperty("pubDate", out var pub)
                    ? DateTime.Parse(pub.GetString()!)
                    : DateTime.UtcNow,
                Language = "es",
                Category = new CategoryInfo { Primary = primaryCategory ?? "general" }
            };

            var raw = new RawContent
            {
                Format = "json",
                Data = article
            };

            results.Add(new SourceItemViewModel(source, normalized, raw));
        }

        return results;
    }
}
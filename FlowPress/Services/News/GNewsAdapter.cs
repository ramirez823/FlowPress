using System.Text.Json;
using FlowPress.Models;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services.News;

public class GNewsAdapter : INewsAdapter
{
    private readonly HttpClient _http;

    public string SourceName => "GNews";
    public string SourceUrl => "https://gnews.io/api/v4/top-headlines";

    public GNewsAdapter(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SourceItemViewModel>> FetchAsync(string apiKey, int count)
    {
        var url = $"https://gnews.io/api/v4/top-headlines?token={apiKey}&lang=es&max={count}";
        var response = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var articles = doc.RootElement.GetProperty("articles");

        var source = new Source
        {
            Name = SourceName,
            Url = SourceUrl,
            RequiresSecret = true
        };

        var results = new List<SourceItemViewModel>();

        foreach (var article in articles.EnumerateArray())
        {
            var normalized = new NormalizedContent
            {
                ExternalId = article.TryGetProperty("url", out var u) ? u.GetString() : null,
                Title = article.TryGetProperty("title", out var t) ? t.GetString() : null,
                Summary = article.TryGetProperty("description", out var d) ? d.GetString() : null,
                Content = article.TryGetProperty("content", out var c) ? c.GetString() : null,
                Url = article.TryGetProperty("url", out var url2) ? url2.GetString() : null,
                PublishedAt = article.TryGetProperty("publishedAt", out var pub)
                    ? DateTime.Parse(pub.GetString()!)
                    : DateTime.UtcNow,
                Language = "es",
                Category = new CategoryInfo { Primary = "general" }
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
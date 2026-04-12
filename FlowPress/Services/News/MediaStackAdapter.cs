using System.Text.Json;
using FlowPress.Models;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services.News;

public class MediaStackAdapter : INewsAdapter
{
    private readonly HttpClient _http;

    public string SourceName => "MediaStack";
    public string SourceUrl => "http://api.mediastack.com/v1/news";

    public MediaStackAdapter(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SourceItemViewModel>> FetchAsync(string apiKey, int count)
    {
        var url = $"http://api.mediastack.com/v1/news?access_key={apiKey}&languages=es&limit={count}";
        var response = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var articles = doc.RootElement.GetProperty("data");

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
                Content = article.TryGetProperty("description", out var c) ? c.GetString() : null,
                Url = article.TryGetProperty("url", out var url2) ? url2.GetString() : null,
                Author = article.TryGetProperty("author", out var a) ? a.GetString() : null,
                PublishedAt = article.TryGetProperty("published_at", out var pub)
                    ? DateTime.Parse(pub.GetString()!)
                    : DateTime.UtcNow,
                Language = "es",
                Category = new CategoryInfo
                {
                    Primary = article.TryGetProperty("category", out var cat)
                        ? cat.GetString() ?? "general"
                        : "general"
                },
                ImageUrl = article.TryGetProperty("image", out var img) ? img.GetString() : null
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
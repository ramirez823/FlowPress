using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowPress.Repositories.Interfaces;
using FlowPress.Models;

namespace FlowPress.Pages.Articles;

public class IndexModel : PageModel
{
    private readonly ISourceItemRepository _sourceItemRepository;

    public IndexModel(ISourceItemRepository sourceItemRepository)
    {
        _sourceItemRepository = sourceItemRepository;
    }

    public List<ArticleViewModel> Articles { get; set; } = new();

    public async Task OnGetAsync()
    {
        var items = await _sourceItemRepository.GetAllAsync();

        Articles = items.Select(item =>
        {
            string title = "Sin título";
            string content = "Sin contenido";
            string publishedAt = "Sin fecha";
            string sourceName = "Sin fuente";

            try
            {
                using var doc = JsonDocument.Parse(item.Json);
                var root = doc.RootElement;

                if (root.TryGetProperty("normalized", out var normalized))
                {
                    if (normalized.TryGetProperty("title", out var titleProp))
                        title = titleProp.GetString() ?? "Sin título";

                    if (normalized.TryGetProperty("content", out var contentProp))
                        content = contentProp.GetString() ?? "Sin contenido";

                    if (normalized.TryGetProperty("publishedAt", out var publishedProp))
                        publishedAt = publishedProp.GetString() ?? "Sin fecha";
                }

                if (root.TryGetProperty("source", out var source))
                {
                    if (source.TryGetProperty("name", out var sourceNameProp))
                        sourceName = sourceNameProp.GetString() ?? "Sin fuente";
                }
            }
            catch
            {
                title = "JSON inválido";
                content = "No se pudo leer el contenido";
                publishedAt = "-";
                sourceName = "-";
            }

            return new ArticleViewModel
            {
                Id = item.Id,
                Title = title,
                Content = content,
                PublishedAt = publishedAt,
                SourceName = sourceName
            };
        }).ToList();
    }

    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string PublishedAt { get; set; } = "";
        public string SourceName { get; set; } = "";
    }
}
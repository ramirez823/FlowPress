using FlowPress.Models;
using FlowPress.Services.Interfaces;
using FlowPress.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FlowPress.Pages;

public class IndexModel : PageModel
{
    private readonly IContentService _contentService;
    private readonly INewsAggregatorService _newsAggregator;
    private readonly ISourceItemRepository _sourceItemRepo;

    public IEnumerable<SourceItemViewModel> Items { get; set; } = [];
    public IEnumerable<Source> Sources { get; set; } = [];
    public List<SourceItemViewModel> NewsItems { get; set; } = [];
    public List<SourceItemViewModel> FeaturedItems { get; set; } = [];

    public IndexModel(
        IContentService contentService,
        INewsAggregatorService newsAggregator,
        ISourceItemRepository sourceItemRepo)
    {
        _contentService = contentService;
        _newsAggregator = newsAggregator;
        _sourceItemRepo = sourceItemRepo;
    }

    public async Task OnGetAsync()
    {
        Sources = await _contentService.GetAllSourcesAsync();
        var rawItems = await _contentService.GetAllItemsAsync();
        Items = rawItems.Select(i => new SourceItemViewModel(i)).ToList();
        NewsItems = await _newsAggregator.GetLatestNewsAsync(12);
        var featuredRaw = await _sourceItemRepo.GetFeaturedAsync();
        FeaturedItems = featuredRaw.Select(i => new SourceItemViewModel(i)).ToList();
    }

    // Handler para exportar un artículo destacado completo (tiene ID en BD)
    public async Task<IActionResult> OnGetExportArticleAsync(int id)
    {
        var item = await _sourceItemRepo.GetByIdAsync(id);
        if (item is null)
            return new JsonResult(new { error = "Artículo no encontrado" });

        var vm = new SourceItemViewModel(item);

        // Parsear el raw original para incluirlo tal cual
        JsonElement? rawData = null;
        try
        {
            using var doc = JsonDocument.Parse(item.Json);
            if (doc.RootElement.TryGetProperty("raw", out var raw) &&
                raw.TryGetProperty("data", out var data))
            {
                rawData = data.Clone();
            }
        }
        catch { }

        var payload = new
        {
            schemaVersion = "edu.univ.ingest.v1",
            exportedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            source = new
            {
                id = vm.Source?.Id.ToString(),
                name = vm.Source?.Name,
                type = vm.Schema?.Type ?? "api",
                url = vm.Source?.Url,
                requiresSecret = vm.Source?.RequiresSecret ?? false
            },
            normalized = new
            {
                id = vm.Normalized?.Id,
                externalId = vm.Normalized?.ExternalId,
                title = vm.Normalized?.Title,
                content = vm.Normalized?.Content,
                summary = vm.Normalized?.Summary,
                publishedAt = vm.Normalized?.PublishedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                url = vm.Normalized?.Url,
                author = vm.Normalized?.Author,
                language = vm.Normalized?.Language,
               imageUrl = vm.Normalized?.ImageUrl,   // [JsonIgnore] en el modelo, pero aquí lo incluimos explícitamente
                category = new
                {
                    primary = vm.Normalized?.Category?.Primary,
                    secondary = vm.Normalized?.Category?.Secondary
                }
            },
            raw = new
            {
                format = vm.Raw?.Format ?? "json",
                data = rawData
            }
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var date = DateTime.UtcNow.ToString("yyyyMMdd");

        return File(bytes, "application/json", $"article_{id}_{date}.json");
    }
}

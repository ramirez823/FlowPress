using FlowPress.Models;
using FlowPress.Services.Interfaces;
using FlowPress.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
}
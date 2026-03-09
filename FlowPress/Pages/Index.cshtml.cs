using FlowPress.Models;
using FlowPress.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowPress.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IContentService _contentService;

        public IEnumerable<SourceItemViewModel> Items { get; set; } = [];
        public IEnumerable<Source> Sources { get; set; } = [];

        public IndexModel(IContentService contentService)
        {
            _contentService = contentService;
        }

        public async Task OnGetAsync()
        {
            Sources = await _contentService.GetAllSourcesAsync();

            var rawItems = await _contentService.GetAllItemsAsync();
            Items = rawItems.Select(i => new SourceItemViewModel(i)).ToList();
        }
    }
}
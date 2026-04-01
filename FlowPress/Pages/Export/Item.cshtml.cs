using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowPress.Repositories.Interfaces;

namespace FlowPress.Pages.Export;

public class ItemModel : PageModel
{
    private readonly ISourceItemRepository _sourceItemRepository;

    public ItemModel(ISourceItemRepository sourceItemRepository)
    {
        _sourceItemRepository = sourceItemRepository;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var items = await _sourceItemRepository.GetAllAsync();
        var item = items.FirstOrDefault(x => x.Id == id);

        if (item == null)
            return NotFound();

        var fileName = $"flowpress-item-{id}.json";
        var bytes = System.Text.Encoding.UTF8.GetBytes(item.Json);

        return File(bytes, "application/json", fileName);
    }
}
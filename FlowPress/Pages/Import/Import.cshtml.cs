using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowPress.Services;

namespace FlowPress.Pages.Import;

public class ImportModel : PageModel
{
    private readonly JsonImportService _importService;

    public ImportModel(JsonImportService importService)
    {
        _importService = importService;
    }

    [BindProperty]
    public IFormFile? JsonFile { get; set; }

    public string? Message { get; set; }
    public string? Error { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (JsonFile == null || JsonFile.Length == 0)
        {
            Error = "Debe seleccionar un archivo.";
            return Page();
        }

        try
        {
            using var reader = new StreamReader(JsonFile.OpenReadStream());
            var json = await reader.ReadToEndAsync();

            Message = await _importService.ImportAsync(json);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }

        return Page();
    }
}
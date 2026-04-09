using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlowPress.Models;
using FlowPress.Services.Interfaces;
using FlowPress.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FlowPress.Pages.Configuration;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ISecretService _secretService;
    private readonly ISourceRepository _sourceRepo;
    private readonly ISourceItemRepository _sourceItemRepo;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ISecretService secretService,
        ISourceRepository sourceRepo,
        ISourceItemRepository sourceItemRepo)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _secretService = secretService;
        _sourceRepo = sourceRepo;
        _sourceItemRepo = sourceItemRepo;
    }



    // Propiedades para la vista
    public List<UserWithRoles> Users { get; set; } = new();
    public List<Secret> Secrets { get; set; } = new();
    public List<Source> Sources { get; set; } = new();
    public List<SourceItem> AllItems { get; set; } = new();

    [TempData]
    public string? ImportMessage { get; set; }

    [TempData]
    public bool ImportSuccess { get; set; }

    // Inputs para asignación de roles
    [BindProperty]
    public string? SelectedUserId { get; set; }

    [BindProperty]
    public bool IsAdmin { get; set; }

    // Inputs para Secrets
    [BindProperty]
    public SecretInputModel SecretInput { get; set; } = new();

    public class SecretInputModel
    {
        public int? Id { get; set; }
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
        public int? SourceId { get; set; }
    }

    public class UserWithRoles
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostToggleRoleAsync()
    {
        if (string.IsNullOrEmpty(SelectedUserId))
            return RedirectToPage();

        var user = await _userManager.FindByIdAsync(SelectedUserId);
        if (user == null)
            return RedirectToPage();

        if (IsAdmin)
        {
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveSecretAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
            return Page();
        }

        if (SecretInput.Id.HasValue)
        {
            var existing = await _secretService.GetByIdAsync(SecretInput.Id.Value);
            if (existing != null)
            {
                await _secretService.UpdateAsync(existing, SecretInput.Value);
            }
        }
        else
        {
            var secret = new Secret
            {
                Key = SecretInput.Key,
                SourceId = SecretInput.SourceId
            };
            await _secretService.CreateAsync(secret, SecretInput.Value);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteSecretAsync(int id)
    {
        await _secretService.DeleteAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleFeaturedAsync(int id)
    {
        var item = await _sourceItemRepo.GetByIdAsync(id);
        if (item is null)
            return RedirectToPage(null, null, "destacados");

        item.IsFeatured = !item.IsFeatured;
        _sourceItemRepo.MarkAsModified(item);
        await _sourceItemRepo.SaveChangesAsync();

        return RedirectToPage(null, null, "destacados");
    }

    public async Task<IActionResult> OnPostImportJsonAsync(IFormFile? jsonFile)
    {
        if (jsonFile is null || jsonFile.Length == 0)
        {
            ImportMessage = "No se seleccionó ningún archivo.";
            ImportSuccess = false;
            return RedirectToPage(null, null, "importar");
        }

        if (!jsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            ImportMessage = "El archivo debe tener extensión .json";
            ImportSuccess = false;
            return RedirectToPage(null, null, "importar");
        }

        try
        {
            using var stream = jsonFile.OpenReadStream();
            using var doc = await System.Text.Json.JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            // Validar schemaVersion
            if (!root.TryGetProperty("schemaVersion", out var schemaVersion) ||
                schemaVersion.GetString() != "edu.univ.ingest.v1")
            {
                ImportMessage = "El archivo no tiene el esquema esperado (edu.univ.ingest.v1).";
                ImportSuccess = false;
                return RedirectToPage(null, null, "importar");
            }

            // Leer source
            if (!root.TryGetProperty("source", out var sourceEl))
            {
                ImportMessage = "El archivo no contiene el nodo 'source'.";
                ImportSuccess = false;
                return RedirectToPage(null, null, "importar");
            }

            var sourceUrl = sourceEl.TryGetProperty("url", out var u) ? u.GetString() : null;
            var sourceName = sourceEl.TryGetProperty("name", out var n) ? n.GetString() : null;
            var sourceType = sourceEl.TryGetProperty("type", out var t) ? t.GetString() : null;

            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                ImportMessage = "El nodo 'source' no contiene una URL válida.";
                ImportSuccess = false;
                return RedirectToPage(null, null, "importar");
            }

            // Buscar o crear Source
            var source = await _sourceRepo.GetByUrlAsync(sourceUrl);
            if (source is null)
            {
                source = new Source
                {
                    Url = sourceUrl,
                    Name = sourceName ?? sourceUrl,
                    ComponentType = string.IsNullOrWhiteSpace(sourceType) ? "API" : sourceType,
                    RequiresSecret = false
                };
                await _sourceRepo.AddAsync(source);
            }

            // Guardar SourceItem
            var item = new SourceItem
            {
                SourceId = source.Id,
                Json = root.GetRawText(),
                CreatedAt = DateTime.UtcNow
            };
            await _sourceItemRepo.AddAsync(item);

            ImportMessage = $"Artículo importado correctamente desde '{source.Name}'.";
            ImportSuccess = true;
            return RedirectToPage(null, null, "importar");
        }
        catch (System.Text.Json.JsonException)
        {
            ImportMessage = "El archivo no es un JSON válido.";
            ImportSuccess = false;
            return RedirectToPage(null, null, "importar");
        }
        catch (Exception ex)
        {
            ImportMessage = $"Error inesperado: {ex.Message}";
            ImportSuccess = false;
            return RedirectToPage(null, null, "importar");
        }
    }

    public async Task<IActionResult> OnGetExportAsync(int id)
    {
        var item = await _sourceItemRepo.GetByIdAsync(id);
        if (item is null)
            return NotFound();

        var bytes = System.Text.Encoding.UTF8.GetBytes(item.Json);
        var fileName = $"article_{id}_{DateTime.UtcNow:yyyyMMdd}.json";
        return File(bytes, "application/json", fileName);
    }

    private async Task LoadDataAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        Users = new List<UserWithRoles>();
        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            Users.Add(new UserWithRoles
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsAdmin = isAdmin
            });
        }

        Secrets = (await _secretService.GetAllAsync()).ToList();
        Sources = (await _sourceRepo.GetAllAsync()).ToList();
        AllItems = (await _sourceItemRepo.GetAllAsync()).ToList();


    }



}
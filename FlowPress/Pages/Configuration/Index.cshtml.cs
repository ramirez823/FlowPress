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

    public IndexModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ISecretService secretService,
        ISourceRepository sourceRepo)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _secretService = secretService;
        _sourceRepo = sourceRepo;
    }

    // Propiedades para la vista
    public List<UserWithRoles> Users { get; set; } = new();
    public List<Secret> Secrets { get; set; } = new();
    public List<Source> Sources { get; set; } = new();

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
    }
}
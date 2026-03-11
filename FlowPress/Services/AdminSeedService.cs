using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlowPress.Services;

public class AdminSeedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AdminSeedService> _logger;

    public AdminSeedService(IServiceProvider serviceProvider, ILogger<AdminSeedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
            _logger.LogInformation("Rol 'Admin' creado.");
        }

        // Datos del admin inicial (podrías leer de configuración)
        string adminEmail = "admin@flowpress.com";
        string adminUserName = "admin";
        string adminPassword = "Admin123!"; // Cámbialo por algo seguro

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario admin creado.");
            }
            else
            {
                _logger.LogError("Error creando usuario admin: {Errors}", string.Join(", ", result.Errors));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            _logger.LogInformation("Rol Admin asignado al usuario.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
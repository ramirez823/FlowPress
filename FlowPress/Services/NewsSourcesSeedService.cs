using FlowPress.Data;
using FlowPress.Models;
using FlowPress.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowPress.Services;

public class NewsSourcesSeedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NewsSourcesSeedService> _logger;

    public NewsSourcesSeedService(IServiceProvider services, ILogger<NewsSourcesSeedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var encryption = scope.ServiceProvider.GetRequiredService<IEncryptionService>();

        var seeds = new[]
        {
            new
            {
                Source = new Source
                {
                    Name = "GNews",
                    Url = "https://gnews.io/api/v4/top-headlines",
                    Description = "GNews top headlines API",
                    ComponentType = "API",
                    RequiresSecret = true
                },
                ApiKey = "afa9c70734f9d8f227a76bcdeaad9cff"
            },
            new
            {
                Source = new Source
                {
                    Name = "NewsAPI",
                    Url = "https://newsapi.org/v2/top-headlines",
                    Description = "NewsAPI top headlines",
                    ComponentType = "API",
                    RequiresSecret = true
                },
                ApiKey = "e7b96e976f744bb8b109bf8f8412ee2f"
            },
            new
            {
                Source = new Source
                {
                    Name = "NewsData",
                    Url = "https://newsdata.io/api/1/latest",
                    Description = "NewsData.io latest news",
                    ComponentType = "API",
                    RequiresSecret = true
                },
                ApiKey = "pub_4105e78854f545e68c65df96184ac2ba"
            },
            new
            {
                Source = new Source
                {
                    Name = "MediaStack",
                    Url = "http://api.mediastack.com/v1/news",
                    Description = "MediaStack news API (HTTP)",
                    ComponentType = "API",
                    RequiresSecret = true
                },
                ApiKey = "351636d4c0f841a62ebaaea8e257adef"
            }
        };

        foreach (var seed in seeds)
        {
            var exists = await context.Sources
                .AnyAsync(s => s.Url == seed.Source.Url, cancellationToken);

            if (exists)
            {
                _logger.LogInformation("NewsSourcesSeed: Source '{Name}' ya existe, omitiendo.", seed.Source.Name);
                continue;
            }

            await context.Sources.AddAsync(seed.Source, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var secret = new Secret
            {
                Key = "ApiKey",
                Value = encryption.Encrypt(seed.ApiKey),
                SourceId = seed.Source.Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.Secrets.AddAsync(secret, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("NewsSourcesSeed: Source '{Name}' creado con su Secret.", seed.Source.Name);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
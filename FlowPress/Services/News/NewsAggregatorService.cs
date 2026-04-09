using FlowPress.Models;
using FlowPress.Repositories.Interfaces;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services.News;

public class NewsAggregatorService : INewsAggregatorService
{
    private readonly IEnumerable<INewsAdapter> _adapters;
    private readonly ISourceRepository _sourceRepo;
    private readonly ISecretService _secretService;
    private readonly IEncryptionService _encryption;
    private readonly ILogger<NewsAggregatorService> _logger;

    public NewsAggregatorService(
        IEnumerable<INewsAdapter> adapters,
        ISourceRepository sourceRepo,
        ISecretService secretService,
        IEncryptionService encryption,
        ILogger<NewsAggregatorService> logger)
    {
        _adapters = adapters;
        _sourceRepo = sourceRepo;
        _secretService = secretService;
        _encryption = encryption;
        _logger = logger;
    }

    public async Task<List<SourceItemViewModel>> GetLatestNewsAsync(int totalCount = 12)
    {
        var results = new List<SourceItemViewModel>();
        int perAdapter = (int)Math.Ceiling((double)totalCount / _adapters.Count());

        foreach (var adapter in _adapters)
        {
            try
            {
                var source = await _sourceRepo.GetByUrlAsync(adapter.SourceUrl);
                if (source is null)
                {
                    _logger.LogWarning("NewsAggregator: no existe Source con URL {Url}", adapter.SourceUrl);
                    continue;
                }

                var secret = await _secretService.GetBySourceIdAsync(source.Id);
                if (secret is null)
                {
                    _logger.LogWarning("NewsAggregator: no hay Secret para Source {Name}", source.Name);
                    continue;
                }

                var apiKey = _encryption.Decrypt(secret.Value);
                var items = await adapter.FetchAsync(apiKey, perAdapter);
                results.AddRange(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NewsAggregator: falló el adapter {Adapter}", adapter.SourceName);
            }
        }

        return results.Take(totalCount).ToList();
    }
}
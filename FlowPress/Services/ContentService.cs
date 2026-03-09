using FlowPress.Models;
using FlowPress.Repositories.Interfaces;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services;

public class ContentService : IContentService
{
    private readonly ISourceItemRepository _sourceItemRepository;
    private readonly ISourceRepository _sourceRepository;

    public ContentService(ISourceItemRepository sourceItemRepository, ISourceRepository sourceRepository)
    {
        _sourceItemRepository = sourceItemRepository;
        _sourceRepository = sourceRepository;
    }

    public async Task<IEnumerable<SourceItem>> GetAllItemsAsync() =>
        await _sourceItemRepository.GetAllAsync();

    public async Task<IEnumerable<Source>> GetAllSourcesAsync() =>
        await _sourceRepository.GetAllAsync();

    public async Task SaveItemAsync(SourceItem item) =>
        await _sourceItemRepository.AddAsync(item);
}
using FlowPress.Models;

namespace FlowPress.Services.Interfaces;

public interface IContentService
{
    Task<IEnumerable<SourceItem>> GetAllItemsAsync();
    Task<IEnumerable<IngestModels>> GetAllSourcesAsync();
    Task SaveItemAsync(SourceItem item);
}
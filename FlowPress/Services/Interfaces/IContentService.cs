using FlowPress.Models;

namespace FlowPress.Services.Interfaces;

public interface IContentService
{
    Task<IEnumerable<SourceItem>> GetAllItemsAsync();
    Task<IEnumerable<Source>> GetAllSourcesAsync();
    Task SaveItemAsync(SourceItem item);
}
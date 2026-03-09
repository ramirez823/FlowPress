using FlowPress.Models;

namespace FlowPress.Repositories.Interfaces;

public interface ISourceItemRepository
{
    Task<IEnumerable<SourceItem>> GetAllAsync();
    Task<SourceItem?> GetByIdAsync(int id);
    Task AddAsync(SourceItem item);
    Task DeleteAsync(int id);
}
using FlowPress.Models;

namespace FlowPress.Repositories.Interfaces;

public interface ISourceRepository
{
    Task<IEnumerable<IngestModels>> GetAllAsync();
    Task<IngestModels?> GetByIdAsync(int id);
    Task AddAsync(IngestModels source);
    Task DeleteAsync(int id);
}
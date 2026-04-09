using FlowPress.Models;
namespace FlowPress.Repositories.Interfaces;

public interface ISourceRepository
{
    Task<IEnumerable<Source>> GetAllAsync();
    Task<Source?> GetByIdAsync(int id);
    Task<Source?> GetByUrlAsync(string url);
    Task AddAsync(Source source);
    Task DeleteAsync(int id);
}
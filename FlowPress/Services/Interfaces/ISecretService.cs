using FlowPress.Models;

namespace FlowPress.Services.Interfaces;

public interface ISecretService
{
    Task<IEnumerable<Secret>> GetAllAsync();
    Task<Secret?> GetByIdAsync(int id);
    Task<Secret?> GetBySourceIdAsync(int sourceId);
    Task CreateAsync(Secret secret, string plainValue);
    Task UpdateAsync(Secret secret, string? newPlainValue = null);
    Task DeleteAsync(int id);
}
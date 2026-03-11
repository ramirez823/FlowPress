using FlowPress.Models;

namespace FlowPress.Repositories.Interfaces;

public interface ISecretRepository
{
    Task<IEnumerable<Secret>> GetAllAsync();
    Task<Secret?> GetByIdAsync(int id);
    Task<Secret?> GetBySourceIdAsync(int sourceId);
    Task<IEnumerable<Secret>> GetAllWithSourceAsync(); // Incluir datos de la fuente
    Task AddAsync(Secret secret);
    void Update(Secret secret);
    Task DeleteAsync(int id);
    Task SaveChangesAsync(); // Para guardar cambios después de Update
}
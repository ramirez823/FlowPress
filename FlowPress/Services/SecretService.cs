using FlowPress.Models;
using FlowPress.Repositories.Interfaces;
using FlowPress.Services.Interfaces;

namespace FlowPress.Services;

public class SecretService : ISecretService
{
    private readonly ISecretRepository _secretRepo;
    private readonly IEncryptionService _encryption;

    public SecretService(ISecretRepository secretRepo, IEncryptionService encryption)
    {
        _secretRepo = secretRepo;
        _encryption = encryption;
    }

    public async Task<IEnumerable<Secret>> GetAllAsync()
    {
        return await _secretRepo.GetAllWithSourceAsync();
    }

    public async Task<Secret?> GetByIdAsync(int id)
    {
        return await _secretRepo.GetByIdAsync(id);
    }

    public async Task<Secret?> GetBySourceIdAsync(int sourceId)
    {
        return await _secretRepo.GetBySourceIdAsync(sourceId);
    }

    public async Task CreateAsync(Secret secret, string plainValue)
    {
        secret.Value = _encryption.Encrypt(plainValue);
        secret.CreatedAt = DateTime.UtcNow;
        await _secretRepo.AddAsync(secret);
        // No llamar a SaveChangesAsync aquí porque el repositorio ya lo hace en AddAsync
    }

    public async Task UpdateAsync(Secret secret, string? newPlainValue = null)
    {
        if (!string.IsNullOrEmpty(newPlainValue))
        {
            secret.Value = _encryption.Encrypt(newPlainValue);
        }
        secret.UpdatedAt = DateTime.UtcNow;
        _secretRepo.Update(secret);
        await _secretRepo.SaveChangesAsync(); // Necesitamos un método para guardar cambios
    }

    public async Task DeleteAsync(int id)
    {
        await _secretRepo.DeleteAsync(id);
    }
}
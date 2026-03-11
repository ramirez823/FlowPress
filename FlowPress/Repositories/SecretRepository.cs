using Microsoft.EntityFrameworkCore;
using FlowPress.Data;
using FlowPress.Models;
using FlowPress.Repositories.Interfaces;

namespace FlowPress.Repositories;

public class SecretRepository : ISecretRepository
{
    private readonly ApplicationDbContext _context;

    public SecretRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Secret>> GetAllAsync()
    {
        return await _context.Secrets.ToListAsync();
    }

    public async Task<Secret?> GetByIdAsync(int id)
    {
        return await _context.Secrets.FindAsync(id);
    }

    public async Task<Secret?> GetBySourceIdAsync(int sourceId)
    {
        return await _context.Secrets.FirstOrDefaultAsync(s => s.SourceId == sourceId);
    }

    public async Task<IEnumerable<Secret>> GetAllWithSourceAsync()
    {
        return await _context.Secrets.Include(s => s.Source).ToListAsync();
    }

    public async Task AddAsync(Secret secret)
    {
        await _context.Secrets.AddAsync(secret);
        await _context.SaveChangesAsync();
    }

    public void Update(Secret secret)
    {
        _context.Secrets.Update(secret);
    }

    public async Task DeleteAsync(int id)
    {
        var secret = await _context.Secrets.FindAsync(id);
        if (secret != null)
        {
            _context.Secrets.Remove(secret);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
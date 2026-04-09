using FlowPress.Data;
using FlowPress.Models;
using FlowPress.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowPress.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly ApplicationDbContext _context;

    public SourceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Source>> GetAllAsync() =>
        await _context.Sources.ToListAsync();

    public async Task<Source?> GetByIdAsync(int id) =>
        await _context.Sources.FindAsync(id);

    public async Task AddAsync(Source source)
    {
        await _context.Sources.AddAsync(source);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var source = await _context.Sources.FindAsync(id);
        if (source != null)
        {
            _context.Sources.Remove(source);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Source?> GetByUrlAsync(string url) =>
    await _context.Sources.FirstOrDefaultAsync(s => s.Url == url);


}
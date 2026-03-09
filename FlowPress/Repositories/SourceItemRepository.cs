using FlowPress.Data;
using FlowPress.Models;
using FlowPress.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowPress.Repositories;


public class SourceItemRepository : ISourceItemRepository
{
    private readonly ApplicationDbContext _context;

    public SourceItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SourceItem>> GetAllAsync() =>
        await _context.SourceItems.Include(i => i.Source).ToListAsync();

    public async Task<SourceItem?> GetByIdAsync(int id) =>
        await _context.SourceItems.Include(i => i.Source).FirstOrDefaultAsync(i => i.Id == id);

    public async Task AddAsync(SourceItem item)
    {
        await _context.SourceItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.SourceItems.FindAsync(id);
        if (item != null)
        {
            _context.SourceItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
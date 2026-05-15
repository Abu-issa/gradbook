using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
using GradBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradBook.Infrastructure.Services;

public class MemoryService : IMemoryService
{
    private readonly GradBookDbContext _context;

    public MemoryService(GradBookDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Memory>> GetAllMemoriesAsync()
        => await _context.Memories.OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<Memory?> GetMemoryByIdAsync(int id)
        => await _context.Memories.FindAsync(id);

    public async Task<Memory> CreateMemoryAsync(Memory memory)
    {
        memory.CreatedAt = DateTime.UtcNow;
        _context.Memories.Add(memory);
        await _context.SaveChangesAsync();
        return memory;
    }

    public async Task UpdateMemoryAsync(Memory memory)
    {
        _context.Memories.Update(memory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMemoryAsync(int id)
    {
        var memory = await _context.Memories.FindAsync(id);
        if (memory != null)
        {
            _context.Memories.Remove(memory);
            await _context.SaveChangesAsync();
        }
    }
}

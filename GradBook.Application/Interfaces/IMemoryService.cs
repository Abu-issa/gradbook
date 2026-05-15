using GradBook.Domain.Entities;

namespace GradBook.Application.Interfaces;

public interface IMemoryService
{
    Task<IEnumerable<Memory>> GetAllMemoriesAsync();
    Task<Memory?> GetMemoryByIdAsync(int id);
    Task<Memory> CreateMemoryAsync(Memory memory);
    Task UpdateMemoryAsync(Memory memory);
    Task DeleteMemoryAsync(int id);
}

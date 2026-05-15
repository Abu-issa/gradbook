using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
using GradBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace GradBook.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly CloudinaryService _cloudinary;
    private readonly GradBookDbContext _context;

    public MessageService(GradBookDbContext context, CloudinaryService cloudinary )
    {
        _context = context;
        _cloudinary = cloudinary;
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        => await _context.Messages.OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Message>> GetApprovedMessagesAsync()
        => await _context.Messages.Where(m => m.IsApproved).OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Message>> GetPendingMessagesAsync()
        => await _context.Messages.Where(m => !m.IsApproved).OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<Message?> GetMessageByIdAsync(int id)
        => await _context.Messages.FindAsync(id);

    public async Task<Message> CreateMessageAsync(Message message)
    {
        message.CreatedAt = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task ApproveMessageAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            message.IsApproved = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RejectMessageAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            message.IsApproved = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteMessageAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalCountAsync()
        => await _context.Messages.CountAsync();

    public async Task<int> GetPendingCountAsync()
        => await _context.Messages.CountAsync(m => !m.IsApproved);
}

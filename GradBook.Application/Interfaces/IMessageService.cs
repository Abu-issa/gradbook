using GradBook.Domain.Entities;

namespace GradBook.Application.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetAllMessagesAsync();
    Task<IEnumerable<Message>> GetApprovedMessagesAsync();
    Task<IEnumerable<Message>> GetPendingMessagesAsync();
    Task<Message?> GetMessageByIdAsync(int id);
    Task<Message> CreateMessageAsync(Message message);
    Task ApproveMessageAsync(int id);
    Task RejectMessageAsync(int id);
    Task DeleteMessageAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<int> GetPendingCountAsync();
}

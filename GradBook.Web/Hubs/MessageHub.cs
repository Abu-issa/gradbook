using Microsoft.AspNetCore.SignalR;

namespace GradBook.Web.Hubs;

public class MessageHub : Hub
{
    public async Task SendNewMessage(string senderName, string content, string reactionType, string? imageUrl, string createdAt)
    {
        await Clients.All.SendAsync("ReceiveMessage", senderName, content, reactionType, imageUrl, createdAt);
    }

    public async Task UpdateVisitorCount(int count)
    {
        await Clients.All.SendAsync("UpdateVisitorCount", count);
    }
}

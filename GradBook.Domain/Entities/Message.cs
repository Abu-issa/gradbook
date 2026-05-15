namespace GradBook.Domain.Entities;

public class Message
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
    public string ReactionType { get; set; } = "🎓";
}

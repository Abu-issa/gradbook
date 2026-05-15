namespace GradBook.Application.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }
    public string ReactionType { get; set; } = "🎓";
}

public class CreateMessageDto
{
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ReactionType { get; set; } = "🎓";
}

public class MemoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardStatsDto
{
    public int TotalMessages { get; set; }
    public int PendingMessages { get; set; }
    public int TotalVisitors { get; set; }
    public int TodayVisitors { get; set; }
    public int TotalMemories { get; set; }
}

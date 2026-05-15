using System.ComponentModel.DataAnnotations;
using GradBook.Domain.Entities;

namespace GradBook.Web.ViewModels;

public class HomeViewModel
{
    public Graduate? Graduate { get; set; }
    public IEnumerable<Message> RecentMessages { get; set; } = new List<Message>();
    public IEnumerable<Memory> RecentMemories { get; set; } = new List<Memory>();
    public int TotalMessages { get; set; }
    public int TotalVisitors { get; set; }
}

public class MessageWallViewModel
{
    public IEnumerable<Message> Messages { get; set; } = new List<Message>();
}

public class CreateMessageViewModel
{
    [Required(ErrorMessage = "Please enter your name")]
    [StringLength(150, MinimumLength = 2)]
    public string SenderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please write a message")]
    [StringLength(2000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }

    public string ReactionType { get; set; } = "🎓";
}

public class MemoriesViewModel
{
    public IEnumerable<Memory> Memories { get; set; } = new List<Memory>();
}

public class AdminDashboardViewModel
{
    public int TotalMessages { get; set; }
    public int PendingMessages { get; set; }
    public int TotalVisitors { get; set; }
    public int TodayVisitors { get; set; }
    public int TotalMemories { get; set; }
    public IEnumerable<Message> PendingMessagesList { get; set; } = new List<Message>();
    public IEnumerable<Message> RecentApprovedMessages { get; set; } = new List<Message>();
}

public class AdminLoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class CreateMemoryViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}

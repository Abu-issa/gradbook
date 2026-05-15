namespace GradBook.Domain.Entities;

public class Visitor
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
}

namespace GradBook.Domain.Entities;

public class Graduate
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public DateTime GraduationDate { get; set; }
}

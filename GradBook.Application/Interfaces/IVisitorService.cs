using GradBook.Domain.Entities;

namespace GradBook.Application.Interfaces;

public interface IVisitorService
{
    Task TrackVisitorAsync(string ipAddress);
    Task<int> GetTotalVisitorCountAsync();
    Task<int> GetTodayVisitorCountAsync();
}

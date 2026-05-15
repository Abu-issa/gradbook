using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
using GradBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradBook.Infrastructure.Services;

public class VisitorService : IVisitorService
{
    private readonly GradBookDbContext _context;

    public VisitorService(GradBookDbContext context)
    {
        _context = context;
    }

    public async Task TrackVisitorAsync(string ipAddress)
    {
        _context.Visitors.Add(new Visitor
        {
            IpAddress = ipAddress,
            VisitedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalVisitorCountAsync()
        => await _context.Visitors.CountAsync();

    public async Task<int> GetTodayVisitorCountAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Visitors.CountAsync(v => v.VisitedAt.Date == today);
    }
}

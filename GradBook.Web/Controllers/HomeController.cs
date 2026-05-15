using GradBook.Application.Interfaces;
using GradBook.Infrastructure.Data;
using GradBook.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradBook.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IMemoryService _memoryService;
    private readonly IVisitorService _visitorService;
    private readonly GradBookDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IMessageService messageService, IMemoryService memoryService,
        IVisitorService visitorService, GradBookDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _messageService = messageService;
        _memoryService = memoryService;
        _visitorService = visitorService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index()
    {
        // Track visitor
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        await _visitorService.TrackVisitorAsync(ip);

        var graduate = await _context.Graduates.FirstOrDefaultAsync();
        var recentMessages = await _messageService.GetApprovedMessagesAsync();
        var memories = await _memoryService.GetAllMemoriesAsync();

        var vm = new HomeViewModel
        {
            Graduate = graduate,
            RecentMessages = recentMessages.Take(6),
            RecentMemories = memories.Take(4),
            TotalMessages = await _messageService.GetTotalCountAsync(),
            TotalVisitors = await _visitorService.GetTotalVisitorCountAsync()
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}

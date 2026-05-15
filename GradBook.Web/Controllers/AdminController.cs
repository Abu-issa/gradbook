using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
using GradBook.Infrastructure.Services;
using GradBook.Web.Hubs;
using GradBook.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GradBook.Web.Controllers;

public class AdminController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IMemoryService _memoryService;
    private readonly IVisitorService _visitorService;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IConfiguration _config;
    private readonly CloudinaryService _cloudinary;

    public AdminController(IMessageService messageService, IMemoryService memoryService,
        IVisitorService visitorService, IHubContext<MessageHub> hubContext,
        IConfiguration config, CloudinaryService cloudinary)
    {
        _messageService = messageService;
        _memoryService = memoryService;
        _visitorService = visitorService;
        _hubContext = hubContext;
        _config = config;
        _cloudinary = cloudinary;
    }

    public IActionResult Login() => View(new AdminLoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var username = _config["AdminCredentials:Username"];
        var password = _config["AdminCredentials:Password"];

        if (vm.Username == username && vm.Password == password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, vm.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Dashboard");
        }

        ModelState.AddModelError("", "Invalid credentials");
        return View(vm);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var vm = new AdminDashboardViewModel
        {
            TotalMessages = await _messageService.GetTotalCountAsync(),
            PendingMessages = await _messageService.GetPendingCountAsync(),
            TotalVisitors = await _visitorService.GetTotalVisitorCountAsync(),
            TodayVisitors = await _visitorService.GetTodayVisitorCountAsync(),
            TotalMemories = (await _memoryService.GetAllMemoriesAsync()).Count(),
            PendingMessagesList = await _messageService.GetPendingMessagesAsync(),
            RecentApprovedMessages = (await _messageService.GetApprovedMessagesAsync()).Take(5)
        };
        return View(vm);
    }

    [Authorize]
    public async Task<IActionResult> Messages()
    {
        var messages = await _messageService.GetAllMessagesAsync();
        return View(messages);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ApproveMessage(int id)
    {
        var message = await _messageService.GetMessageByIdAsync(id);
        await _messageService.ApproveMessageAsync(id);

        if (message != null)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                message.SenderName, message.Content, message.ReactionType,
                message.ImageUrl, message.CreatedAt.ToString("MMM dd, yyyy"));
        }

        TempData["Success"] = "Message approved successfully!";
        return RedirectToAction("Messages");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        await _messageService.DeleteMessageAsync(id);
        TempData["Success"] = "Message deleted.";
        return RedirectToAction("Messages");
    }

    [Authorize]
    public async Task<IActionResult> Memories()
    {
        var memories = await _memoryService.GetAllMemoriesAsync();
        return View(memories);
    }

    [Authorize]
    public IActionResult CreateMemory() => View(new CreateMemoryViewModel());

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMemory(CreateMemoryViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        string? imageUrl = null;
        if (vm.Image != null && vm.Image.Length > 0)
        {
            imageUrl = await _cloudinary.UploadImageAsync(vm.Image);
        }

        await _memoryService.CreateMemoryAsync(new Memory
        {
            Title = vm.Title,
            Description = vm.Description,
            ImageUrl = imageUrl
        });

        TempData["Success"] = "Memory added!";
        return RedirectToAction("Memories");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteMemory(int id)
    {
        await _memoryService.DeleteMemoryAsync(id);
        TempData["Success"] = "Memory deleted.";
        return RedirectToAction("Memories");
    }

    [Authorize]
    public IActionResult QrCode()
    {
        return View();
    }
}
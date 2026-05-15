using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
using GradBook.Infrastructure.Services;
using GradBook.Web.Hubs;
using GradBook.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GradBook.Web.Controllers;

public class MessagesController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IWebHostEnvironment _environment;
    private readonly CloudinaryService _cloudinary;

    public MessagesController(IMessageService messageService, IHubContext<MessageHub> hubContext,
        IWebHostEnvironment environment, CloudinaryService cloudinary)
    {
        _messageService = messageService;
        _hubContext = hubContext;
        _environment = environment;
        _cloudinary = cloudinary;       
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _messageService.GetApprovedMessagesAsync();
        return View(new MessageWallViewModel { Messages = messages });
    }

    public IActionResult Create() => View(new CreateMessageViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public async Task<IActionResult> Create(CreateMessageViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // ← احذف كود الـ uploads القديم واستبدله بهاد
        string? imageUrl = null;
        if (vm.Image != null && vm.Image.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(vm.Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("Image", "Only image files are allowed.");
                return View(vm);
            }
            imageUrl = await _cloudinary.UploadImageAsync(vm.Image);
        }

        var message = new Message
        {
            SenderName = vm.SenderName,
            Content = vm.Content,
            ReactionType = vm.ReactionType,
            ImageUrl = imageUrl,
            IsApproved = false
        };
        await _messageService.CreateMessageAsync(message);
        TempData["Success"] = "Your message has been submitted! 🎓";
        return RedirectToAction("Thanks");
    }

    public IActionResult Thanks() => View();
}

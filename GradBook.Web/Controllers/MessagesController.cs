using GradBook.Application.Interfaces;
using GradBook.Domain.Entities;
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

    public MessagesController(IMessageService messageService, IHubContext<MessageHub> hubContext,
        IWebHostEnvironment environment)
    {
        _messageService = messageService;
        _hubContext = hubContext;
        _environment = environment;
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
        if (!ModelState.IsValid)
            return View(vm);

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

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.Image.CopyToAsync(stream);
            imageUrl = $"/uploads/{fileName}";
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

        TempData["Success"] = "Your message has been submitted and is awaiting approval. Thank you! 🎓";
        return RedirectToAction("Thanks");
    }

    public IActionResult Thanks() => View();
}

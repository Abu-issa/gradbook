using GradBook.Application.Interfaces;
using GradBook.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GradBook.Web.Controllers;

public class MemoriesController : Controller
{
    private readonly IMemoryService _memoryService;

    public MemoriesController(IMemoryService memoryService)
    {
        _memoryService = memoryService;
    }

    public async Task<IActionResult> Index()
    {
        var memories = await _memoryService.GetAllMemoriesAsync();
        return View(new MemoriesViewModel { Memories = memories });
    }
}

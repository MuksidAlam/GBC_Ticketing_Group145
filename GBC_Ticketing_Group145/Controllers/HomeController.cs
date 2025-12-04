using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBC_Ticketing_Group145.Models;

namespace GBC_Ticketing_Group145.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Status handler used by UseStatusCodePagesWithReExecute
    [Route("Home/Status/{code}")]
    public IActionResult Status(int code)
    {
        // Log status with structured data
        _logger.LogWarning("Status code {StatusCode} returned for path {Path}", code, HttpContext.Request.Path);

        Response.StatusCode = code;
        return View(model: code);
    }
}
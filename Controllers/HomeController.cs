using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CliCarProject.Models;
using CliCarProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CliCarProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.Marcas = _context.Marcas.ToList();
        ViewBag.Modelos = _context.Modelos.ToList();
        ViewBag.Classes = _context.Classes.ToList();    
        ViewBag.Combustiveis = _context.Combustivels.ToList();

        ViewBag.Caixas = new List<SelectListItem>
        {
            new SelectListItem("Todas", ""),
            new SelectListItem("Manual", "Manual"),
            new SelectListItem("Automática", "Automática")
        };

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
}

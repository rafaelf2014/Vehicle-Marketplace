using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CliCarProject.Models;
using CliCarProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using CliCarProject.Models.Classes;
using Microsoft.EntityFrameworkCore;


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

    public async Task<IActionResult> Index()
    {
        // 1. Carregar Dados para as Dropdowns (ViewBag)
        ViewBag.Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();
        ViewBag.Classes = await _context.Classes.OrderBy(c => c.Nome).ToListAsync();
        ViewBag.Combustiveis = await _context.Combustivels.OrderBy(c => c.Tipo).ToListAsync();

        // Inicializamos modelos como lista vazia (será preenchida via AJAX no browser)
        ViewBag.Modelos = new List<Modelo>();

        // 2. Carregar Anúncios em Destaque para o Model
        // Ordenamos por visualizações e pegamos nos 6 principais
        var destaques = await _context.Anuncios
            .Where(a => a.Estado == "Ativo")
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.Imagems)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.IdModeloNavigation)
            .OrderByDescending(a => a.Visualizacoes)
            .Take(4)
            .ToListAsync();

        return View(destaques);
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

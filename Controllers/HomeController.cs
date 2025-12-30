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

        //var todosAtivos = await _context.Anuncios.Where(a => a.Estado == "Ativo").CountAsync();

        // 2. Carregar Anúncios em Destaque para o Model
        var destaques = await _context.Anuncios
            .Where(a => a.Estado == "Ativo")
            .Where(a => a.IdVeiculoNavigation != null && a.IdVeiculoNavigation.Disponivel)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.Imagems)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.IdModeloNavigation)
            .OrderByDescending(a => a.Visualizacoes)
            .Take(4)
            .ToListAsync();

        //System.Diagnostics.Debug.WriteLine($"DEBUG: Anúncios Ativos: {todosAtivos} | Destaques encontrados: {destaques.Count}");

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

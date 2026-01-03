using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CliCarProject.Models;
using CliCarProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using CliCarProject.Models.Classes;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Identity;


namespace CliCarProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
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

        var meusFavoritos = await _context.Favoritos
            .Where(f => f.IdUtilizador == userId) 
            .Include(f => f.Anuncio)              
                .ThenInclude(a => a.IdVeiculoNavigation) 
                    .ThenInclude(v => v.Imagems)         
            .Include(f => f.Anuncio.IdVeiculoNavigation.IdMarcaNavigation) 
            .Include(f => f.Anuncio.IdVeiculoNavigation.IdModeloNavigation)
            .Select(f => f.Anuncio)
            .Take(4)
            .ToListAsync();

        ViewBag.MeusFavoritos = meusFavoritos;

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

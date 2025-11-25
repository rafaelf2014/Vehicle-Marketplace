using CliCarProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CliCarProject.Models.Classes;
using System.Diagnostics;

namespace CliCarProject.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CarSearch(string searchBox, int? marcaId, int? modeloId, int? categoriaId, int? combustivelId, string caixa, int? priceRange)
        {
            // Se o utilizador escolheu um modelo mas não especificou marca,
            // inferimos a marca a partir do modelo (evita estado inconsistente).
            if (modeloId.HasValue && !marcaId.HasValue)
            {
                var modelo = _context.Modelos.Find(modeloId.Value);
                if (modelo != null)
                {
                    marcaId = modelo.IdMarca;
                }
            }

            // Se foram passados ambos, garantimos consistência:
            if (modeloId.HasValue && marcaId.HasValue)
            {
                var modelo = _context.Modelos.Find(modeloId.Value);
                if (modelo != null && modelo.IdMarca != marcaId.Value)
                {
                    // modelo não pertence à marca selecionada → ignorar o modelo
                    modeloId = null;
                }
            }

            Debug.WriteLine($"CarSearch called: searchBox={searchBox}, marcaId={marcaId}, modeloId={modeloId}, categoriaId={categoriaId}, combustivelId={combustivelId}, caixa={caixa}, priceRange={priceRange}");

            var query = _context.Veiculos
                .Include(v => v.IdModeloNavigation).ThenInclude(m => m.IdMarcaNavigation)
                .Include(v => v.Imagems)
                .Include(v => v.Anuncios)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                query = query.Where(v =>
                    v.IdModeloNavigation.Nome.Contains(searchBox) ||
                    v.IdModeloNavigation.IdMarcaNavigation.Nome.Contains(searchBox));
            }

            if (marcaId.HasValue)
                query = query.Where(v => v.IdModeloNavigation.IdMarca == marcaId.Value);

            if (modeloId.HasValue)
                query = query.Where(v => v.IdModelo == modeloId.Value);

            if (categoriaId.HasValue)
                query = query.Where(v => v.IdClasse == categoriaId.Value);

            if (combustivelId.HasValue)
                query = query.Where(v => v.IdCombustivel == combustivelId.Value);

            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        query = query.Where(v => v.Anuncios.Any(a => a.Preco <= 10000));
                        break;
                    case 2:
                        query = query.Where(v => v.Anuncios.Any(a => a.Preco > 10000 && a.Preco <= 30000));
                        break;
                    case 3:
                        query = query.Where(v => v.Anuncios.Any(a => a.Preco > 30000));
                        break;
                }
            }

            try
            {
                Debug.WriteLine("Generated SQL: " + query.ToQueryString());
            }
            catch { }

            var resultados = query.ToList();

            // popular dados para a view
            ViewBag.Resultados = resultados;
            ViewBag.Marcas = _context.Marcas.ToList();

            // Se houver marca selecionada, devolve só os modelos dessa marca.
            ViewBag.Modelos = marcaId.HasValue
                ? _context.Modelos.Where(m => m.IdMarca == marcaId.Value).ToList()
                : _context.Modelos.ToList();

            ViewBag.Classes = _context.Classes.ToList();
            ViewBag.Combustiveis = _context.Combustivels.ToList();
            ViewBag.Caixas = new List<SelectListItem>
            {
                new SelectListItem("Todas", ""),
                new SelectListItem("Manual", "Manual"),
                new SelectListItem("Automática", "Automática")
            };

            return View("CarSearch");
        }

        // Endpoint para popular os modelos via AJAX quando muda a marca
        [HttpGet]
        public IActionResult GetModelos(int marcaId)
        {
            var modelos = _context.Modelos
                .Where(m => m.IdMarca == marcaId)
                .Select(m => new { m.IdModelo, m.Nome })
                .ToList();

            return Json(modelos);
        }
    }
}

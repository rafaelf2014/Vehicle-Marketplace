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
                   
                    modeloId = null;
                }
            }

            if (modeloId.HasValue && !categoriaId.HasValue)
            {
                var existe = _context.Veiculos.Any(v => v.IdModelo == modeloId.Value && v.IdClasse == categoriaId.Value);
                if (!existe)
                {
               
            {
                var existe = _context.Veiculos.Any(v => v.IdModelo == modeloId.Value && v.IdClasse == categoriaId.Value);
                if (!existe)
                {
                   
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

            // Determina se o utilizador não aplicou filtros:
            bool noFilters = string.IsNullOrWhiteSpace(searchBox)
                             && !marcaId.HasValue
                             && !modeloId.HasValue
                             && !categoriaId.HasValue
                             && !combustivelId.HasValue
                             && string.IsNullOrWhiteSpace(caixa)
                             && !priceRange.HasValue;

            List<Veiculo> resultados;

            if (noFilters)
            {
                // busca todos e escolhe um subconjunto aleatório (ex.: 12)
                var all = query.ToList();
                resultados = all
                    .OrderBy(x => Guid.NewGuid())
                    .Take(12)
                    .ToList();
            }
            else
            {
                resultados = query.ToList();
            }

            // popular dados para a view
            ViewBag.Resultados = resultados;
            ViewBag.Marcas = _context.Marcas.ToList();

            // Se houver marca selecionada, devolve só os modelos dessa marca.
            // Melhorado: também considera categoriaId para evitar modelos que não existam nessa categoria.
            if (marcaId.HasValue && categoriaId.HasValue)
            {
                ViewBag.Modelos = _context.Modelos
                    .Where(m => m.IdMarca == marcaId.Value
                                && _context.Veiculos.Any(v => v.IdModelo == m.IdModelo && v.IdClasse == categoriaId.Value))
                    .ToList();
            }
            else if (marcaId.HasValue)
            {
                ViewBag.Modelos = _context.Modelos.Where(m => m.IdMarca == marcaId.Value).ToList();
            }
            else if (categoriaId.HasValue)
            {
                ViewBag.Modelos = _context.Modelos
                    .Where(m => _context.Veiculos.Any(v => v.IdModelo == m.IdModelo && v.IdClasse == categoriaId.Value))
                    .ToList();
            }
            else
            {
                ViewBag.Modelos = _context.Modelos.ToList();
            }

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

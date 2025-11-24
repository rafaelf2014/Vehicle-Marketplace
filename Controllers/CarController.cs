using CliCarProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CliCarProject.Models.Classes;

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
            // base query com includes necessários
            var query = _context.Veiculos
                .Include(v => v.IdModeloNavigation).ThenInclude(m => m.IdMarcaNavigation)
                .Include(v => v.Imagems)
                .Include(v => v.Anuncios)
                .AsQueryable();

            // filtros
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

            // caixa: só aplica se tiveres essa propriedade no modelo (exemplo comentado)
            // if (!string.IsNullOrEmpty(caixa))
            //     query = query.Where(v => v.Caixa == caixa);

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

            var resultados = query.ToList();

            // popular dados para a view (podes trocar por ViewModel se preferires)
            ViewBag.Resultados = resultados;
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

            return View("CarSearch");
        }
    }
}

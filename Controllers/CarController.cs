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
        public IActionResult CarSearch(string searchBox, int? marcaId, int? modeloId, int? categoriaId, int? combustivelId, string caixa, decimal? minPrice,decimal? maxPrice, string sortOrder, int? minYear, int? maxYear, int? localizacaoId, int? minKm, int? maxKm, int page = 1)
        {
            if (modeloId.HasValue && !marcaId.HasValue)
            {
                var modelo = _context.Modelos.Find(modeloId.Value);
                if (modelo != null)
                    marcaId = modelo.IdMarca;
            }

            if (modeloId.HasValue && marcaId.HasValue)
            {
                var modelo = _context.Modelos.Find(modeloId.Value);
                if (modelo != null && modelo.IdMarca != marcaId.Value)
                    modeloId = null;
            }

            var query = _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)!.ThenInclude(v => v!.Imagems)
                .Include(a => a.IdVeiculoNavigation)!.ThenInclude(v => v!.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation)!.ThenInclude(v => v!.IdModeloNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                query = query.Where(a =>
                    a.Titulo.Contains(searchBox) ||
                    a.IdVeiculoNavigation!.IdModeloNavigation!.Nome.Contains(searchBox) ||
                    a.IdVeiculoNavigation!.IdMarcaNavigation!.Nome.Contains(searchBox));
            }

            if (marcaId.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation!.IdMarca == marcaId.Value);

            if (modeloId.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation!.IdModelo == modeloId.Value);

            if (categoriaId.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation!.IdClasse == categoriaId.Value);

            if (combustivelId.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation!.IdCombustivel == combustivelId.Value);

            if (!string.IsNullOrWhiteSpace(caixa))
                query = query.Where(a => a.IdVeiculoNavigation!.Caixa.ToLower() == caixa.ToLower());

             //PREÇO MINIMO
             if (minPrice.HasValue)
             {
                query = query.Where(a => a.Preco >= minPrice.Value);
             }

             //PREÇO MAXIMO
             if (maxPrice.HasValue)
             {
                query = query.Where(a => a.Preco <= maxPrice.Value);
             }

            // FILTRO DE ANO MÍNIMO 
            if (minYear.HasValue)
            {
                query = query.Where(a => a.IdVeiculoNavigation!.Ano >= minYear.Value);
            }

            // FILTRO DE ANO MÁXIMO 
            if (maxYear.HasValue)
            {
                query = query.Where(a => a.IdVeiculoNavigation!.Ano <= maxYear.Value);
            }

            if (localizacaoId.HasValue)
            {
                // Nota: Confirma se na tua classe Anuncio o campo é 'IdLocalizacao'
                query = query.Where(a => a.IdLocalizacao == localizacaoId.Value);
            }

            if (minKm.HasValue)
            {
               
                query = query.Where(a => a.IdVeiculoNavigation!.Quilometragem >= minKm.Value);
            }

            if (maxKm.HasValue)
            {
                query = query.Where(a => a.IdVeiculoNavigation!.Quilometragem <= maxKm.Value);
            }

            query = sortOrder switch
            {
                "ano_asc" => query.OrderBy(a => a.IdVeiculoNavigation!.Ano),
                "ano_desc" => query.OrderByDescending(a => a.IdVeiculoNavigation!.Ano),

                "km_asc" => query.OrderBy(a => a.IdVeiculoNavigation!.Quilometragem),
                "km_desc" => query.OrderByDescending(a => a.IdVeiculoNavigation!.Quilometragem),

                "modelo_asc" => query.OrderBy(a => a.IdVeiculoNavigation!.IdModeloNavigation!.Nome),
                "modelo_desc" => query.OrderByDescending(a => a.IdVeiculoNavigation!.IdModeloNavigation!.Nome),

                "preco_asc" => query.OrderBy(a => a.Preco),
                "preco_desc" => query.OrderByDescending(a => a.Preco),

                "data_asc" => query.OrderBy(a => a.DataCriacao),
                _ => query.OrderByDescending(a => a.DataCriacao),
            };

            const int pageSize = 9;
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var anuncios = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;

            ViewBag.CurrentSort = sortOrder ?? "";
            ViewBag.Page = page;
            ViewBag.TotalPages = Math.Max(1, totalPages);

            // ... código de paginação ...

            // PREENCHER OS DADOS PARA OS DROPDOWNS LATERAIS
            ViewBag.Marcas = _context.Marcas.OrderBy(m => m.Nome).ToList();
            ViewBag.Classes = _context.Classes.OrderBy(c => c.Nome).ToList(); // Categoria
            ViewBag.Combustiveis = _context.Combustivels.OrderBy(c => c.Tipo).ToList();
            ViewBag.Localizacoes = _context.Localizacaos.OrderBy(l => l.Distrito).ToList();

            // Lista manual para as Caixas (se não vier da BD)
            ViewBag.Caixas = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Manual" },
                new SelectListItem { Value = "A", Text = "Automática" }
            };

            // Se tiveres uma marca selecionada, carrega os modelos dela para o filtro não ficar vazio
            if (marcaId.HasValue)
            {
                ViewBag.Modelos = _context.Modelos.Where(m => m.IdMarca == marcaId).OrderBy(m => m.Nome).ToList();
            }
            else
            {
                ViewBag.Modelos = new List<Modelo>();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AnunciosGrid", anuncios);
            }

            return View("CarSearch", anuncios);

            
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

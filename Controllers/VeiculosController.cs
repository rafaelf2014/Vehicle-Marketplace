using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Data;
using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CliCarProject.Services;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CliCarProject.Controllers
{
    [Authorize]
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IVeiculoService _veiculoService;

        public VeiculosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IVeiculoService veiculoService)
        {
            _context = context;
            _userManager = userManager;
            _veiculoService = veiculoService;
        }

        private void CarregarDropdowns()
        {
            ViewBag.IdMarca = new SelectList(_context.Marcas.OrderBy(m => m.Nome), "IdMarca", "Nome");
            ViewBag.IdCombustivel = new SelectList(_context.Combustivels.OrderBy(c => c.Tipo), "IdCombustivel", "Tipo");
            ViewBag.IdClasse = new SelectList(_context.Classes.OrderBy(c => c.Nome), "IdClasse", "Nome");
            ViewBag.IdModelo = new SelectList(_context.Modelos, "IdModelo", "Nome");
        }

        public async Task<IActionResult> Index(string sortOrder, int page = 1, int pageSize = 8)
        {
            ViewBag.CurrentSort = sortOrder;
            var userId = _userManager.GetUserId(User);

            var query = _context.Veiculos
                .Where(v => v.IdVendedor == userId && v.Disponivel == true) 
                .Include(v => v.Imagems)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdMarcaNavigation)
                .AsQueryable();

            // Lógica de Ordenação
            query = sortOrder switch
            {
                "ano_asc" => query.OrderBy(v => v.Ano),
                "ano_desc" => query.OrderByDescending(v => v.Ano),
                "km_asc" => query.OrderBy(v => v.Quilometragem),
                "km_desc" => query.OrderByDescending(v => v.Quilometragem),
                "modelo_asc" => query.OrderBy(v => v.IdModeloNavigation.Nome),
                "modelo_desc" => query.OrderByDescending(v => v.IdModeloNavigation.Nome),
                _ => query.OrderByDescending(v => v.IdVeiculo)
            };

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page > totalPages && totalPages > 0) page = totalPages;

            var veiculos = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            // Retorna PartialView se o pedido for AJAX para evitar duplicação de Layout
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(veiculos);
            }

            return View(veiculos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var veiculo = await _context.Veiculos
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdMarcaNavigation)
                .Include(v => v.IdClasseNavigation)
                .Include(v => v.IdCombustivelNavigation)
                .Include(v => v.Imagems)
                .FirstOrDefaultAsync(v => v.IdVeiculo == id);

            if (veiculo == null) return NotFound();
            return View(veiculo);
        }

        public IActionResult Create()
        {
            CarregarDropdowns();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veiculo veiculo, List<IFormFile> Imagens,int? modeloId, int? marcaId)
        {
            if (ModelState.IsValid)
            {
                veiculo.IdVendedor = _userManager.GetUserId(User);
                _context.Add(veiculo);
                await _context.SaveChangesAsync();

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

                if(veiculo.Quilometragem < 0)
                {
                    ModelState.AddModelError("Quilometragem","Quilometragem não pode ser negativa");
                }

                if(veiculo.Ano < 1886 || veiculo.Ano > DateTime.Now.Year + 1)
                {
                    ModelState.AddModelError("Ano", "Ano inválido");
                }

                if (Imagens != null && Imagens.Count > 0)
                {
                    var pastaVeiculo = Path.Combine("wwwroot/uploads/veiculos", veiculo.IdVeiculo.ToString());
                    if (!Directory.Exists(pastaVeiculo)) Directory.CreateDirectory(pastaVeiculo);

                    foreach (var file in Imagens)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(pastaVeiculo, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _context.Imagems.Add(new Imagem { IdVeiculo = veiculo.IdVeiculo, Nome = fileName });
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            CarregarDropdowns();
            return View(veiculo);
        }

        [HttpGet]
        public IActionResult GetModelos(int marcaId)
        {
            var modelos = _context.Modelos
                .Where(m => m.IdMarca == marcaId)
                .Select(m => new { idModelo = m.IdModelo, nome = m.Nome })
                .ToList();

            return Json(modelos);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(User);

            if (id == null) return NotFound();
            var veiculo = await _context.Veiculos.Include(v => v.Imagems).FirstOrDefaultAsync(v => v.IdVeiculo == id);
            if (veiculo == null) return NotFound();
            CarregarDropdowns();

            if (userId != veiculo.IdVendedor)
            {
                return Unauthorized();
            }
            return View(veiculo);
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.IdVeiculo) return NotFound();

            var veiculoOriginal = await _context.Veiculos
        .AsNoTracking()
        .FirstOrDefaultAsync(v => v.IdVeiculo == id);

            if (id != veiculo.IdVeiculo) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    veiculo.IdVendedor = veiculoOriginal.IdVendedor;
                    veiculo.Disponivel = veiculoOriginal.Disponivel;

                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                
            }
            CarregarDropdowns();
            return View(veiculo);
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImages(int id, int[] RemoverIds, List<IFormFile> NovasImagens)
        {
            var veiculo = await _context.Veiculos.Include(v => v.Imagems).FirstOrDefaultAsync(v => v.IdVeiculo == id);
            if (veiculo == null) return NotFound();

            // 1. Remover Imagens Selecionadas
            if (RemoverIds != null && RemoverIds.Length > 0)
            {
                var imagensParaApagar = veiculo.Imagems.Where(img => RemoverIds.Contains(img.IdImagem)).ToList();
                foreach (var img in imagensParaApagar)
                {
                    // Apagar ficheiro físico
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/veiculos", id.ToString(), img.Nome);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                    _context.Imagems.Remove(img);
                }
            }

            // 2. Adicionar Novas Imagens
            if (NovasImagens != null && NovasImagens.Any())
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/veiculos", id.ToString());
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                foreach (var file in NovasImagens)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(path, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    veiculo.Imagems.Add(new Imagem { Nome = fileName });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        [HttpPost]
        [Route("Veiculos/DeleteMultiple")]
        [Authorize(Roles = "Vendedor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest();

            var userId = _userManager.GetUserId(User);

            // Procuramos os veículos
            var veiculos = await _context.Veiculos
                .Where(v => ids.Contains(v.IdVeiculo) && v.IdVendedor == userId)
                .ToListAsync();

            var temReserva = await _context.Anuncios
                .AnyAsync(a => ids.Contains(a.IdVeiculo) && a.Estado == "Reservado");

            if (temReserva)
            {
                TempData["Error"] = "Não pode eliminar veículos que têm reservas ativas nos anúncios.";
                return RedirectToAction(nameof(Index));
            }

            if (veiculos.Any())
            {
                foreach (var v in veiculos)
                {
                    v.Disponivel = false;

                    // Procura anúncios ativos deste veículo e desativa-os também
                    var anunciosRelacionados = _context.Anuncios.Where(a => a.IdVeiculo == v.IdVeiculo);
                    foreach (var a in anunciosRelacionados)
                    {
                        a.Estado = "Inativo";
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            return NotFound();
        }
    }
}
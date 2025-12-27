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
                .Where(v => v.IdVendedor == userId) // Apenas os veículos do dono logado
                .Include(v => v.Imagems)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdMarcaNavigation)
                .AsQueryable();

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

            // Suporte para AJAX Unobtrusive
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veiculo veiculo, List<IFormFile> Imagens)
        {
            if (ModelState.IsValid)
            {
                veiculo.IdVendedor = _userManager.GetUserId(User);
                _context.Add(veiculo);
                await _context.SaveChangesAsync();

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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var veiculo = await _context.Veiculos.Include(v => v.Imagems).FirstOrDefaultAsync(v => v.IdVeiculo == id);
            if (veiculo == null) return NotFound();
            CarregarDropdowns();
            return View(veiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.IdVeiculo) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            CarregarDropdowns();
            return View(veiculo);
        }

        [HttpPost]
        [Route("Veiculos/DeleteMultiple")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest(new { success = false, error = "Nenhum ID enviado." });

            var userId = _userManager.GetUserId(User);
            var veiculos = await _context.Veiculos
                .Where(v => ids.Contains(v.IdVeiculo) && v.IdVendedor == userId)
                .ToListAsync();

            if (!veiculos.Any()) return NotFound(new { success = false, error = "Veículos não encontrados." });

            try
            {
                _context.Veiculos.RemoveRange(veiculos);
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null) _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
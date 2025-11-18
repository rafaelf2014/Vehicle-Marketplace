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


namespace CliCarProject.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public VeiculosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Veiculos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Veiculos.Include(v => v.IdClasseNavigation).Include(v => v.IdCombustivelNavigation).Include(v => v.IdModeloNavigation).Include(v => v.IdVendedorNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.IdClasseNavigation)
                .Include(v => v.IdCombustivelNavigation)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdVeiculo == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // GET: Veiculos/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["IdClasse"] = new SelectList(_context.Classes, "IdClasse", "IdClasse");
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "IdCombustivel");
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "IdModelo");
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreateConfirmed([Bind("Ano,Quilometragem,Condicao,IdModelo,IdCombustivel,IdClasse")] Veiculo veiculo)
        {
            Console.WriteLine("🔥 POST chegou ao método Create");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState inválido!");
                foreach (var e in ModelState)
                {
                    foreach (var err in e.Value.Errors)
                        Console.WriteLine($"Erro em {e.Key}: {err.ErrorMessage}");
                }
                return View(veiculo);

            }
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"👤 Utilizador autenticado: {userId ?? "NENHUM"}");

            veiculo.IdVendedor = userId;

            try
            {
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("✅ Veículo guardado na base de dados!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Erro ao guardar veículo: {ex.Message}");
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }
            return RedirectToAction(nameof(Index));

            //ViewData["IdClasse"] = new SelectList(_context.Classes, "IdClasse", "IdClasse", veiculo.IdClasse);
            //ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "IdCombustivel", veiculo.IdCombustivel);
            //ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "IdModelo", veiculo.IdModelo);
            //return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }
            ViewData["IdClasse"] = new SelectList(_context.Classes, "IdClasse", "IdClasse", veiculo.IdClasse);
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "IdCombustivel", veiculo.IdCombustivel);
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "IdModelo", veiculo.IdModelo);
            ViewData["IdVendedor"] = new SelectList(_context.Users, "Id", "Id", veiculo.IdVendedor);
            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVeiculo,IdVendedor,Ano,Quilometragem,Condicao,IdModelo,IdCombustivel,IdClasse")] Veiculo veiculo)
        {
            if (id != veiculo.IdVeiculo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.IdVeiculo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClasse"] = new SelectList(_context.Classes, "IdClasse", "IdClasse", veiculo.IdClasse);
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "IdCombustivel", veiculo.IdCombustivel);
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "IdModelo", veiculo.IdModelo);
            ViewData["IdVendedor"] = new SelectList(_context.Users, "Id", "Id", veiculo.IdVendedor);
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.IdClasseNavigation)
                .Include(v => v.IdCombustivelNavigation)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdVeiculo == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null)
            {
                _context.Veiculos.Remove(veiculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.IdVeiculo == id);
        }
    }
}

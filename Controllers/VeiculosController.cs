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
using static System.Net.Mime.MediaTypeNames;


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
        public async Task<IActionResult> Index(string sortOrder, int page = 1, int pageSize = 6)
        {
            // ViewBag para manter seleção atual
            ViewBag.CurrentSort = sortOrder;

            // opções de ordenação
            ViewBag.SortAnoAsc = "ano_asc";
            ViewBag.SortAnoDesc = "ano_desc";
            ViewBag.SortKmAsc = "km_asc";
            ViewBag.SortKmDesc = "km_desc";
            ViewBag.SortModeloAsc = "modelo_asc";
            ViewBag.SortModeloDesc = "modelo_desc";
            ViewBag.SortDataAsc = "data_asc";
            ViewBag.SortDataDesc = "data_desc";

            var query = _context.Veiculos
                .Include(v => v.Imagems)
                .Include(v => v.IdModeloNavigation)
                .OrderBy(v => v.IdVeiculo)
                .AsQueryable();

            // ORDENAR
            query = sortOrder switch
            {
                "ano_asc" => query.OrderBy(v => v.Ano),
                "ano_desc" => query.OrderByDescending(v => v.Ano),

                "km_asc" => query.OrderBy(v => v.Quilometragem),
                "km_desc" => query.OrderByDescending(v => v.Quilometragem),

                "modelo_asc" => query.OrderBy(v => v.IdModeloNavigation.Nome),
                "modelo_desc" => query.OrderByDescending(v => v.IdModeloNavigation.Nome),

                "data_asc" => query.OrderBy(v => v.IdVeiculo),
                "data_desc" => query.OrderByDescending(v => v.IdVeiculo),

                _ => query.OrderBy(v => v.IdVeiculo)
            };

            // PAGINAÇÃO
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var veiculos = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(veiculos);
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculos = await _context.Veiculos
        .Include(v => v.IdModeloNavigation)
            .Include(v => v.IdMarcaNavigation)
        .Include(v => v.IdClasseNavigation)
        .Include(v => v.IdCombustivelNavigation)
        .Include(v => v.Imagems)
        .FirstOrDefaultAsync(v => v.IdVeiculo == id);


            if (veiculos == null)
            {
                return NotFound();
            }

            return View(veiculos);
        }

        // GET: Veiculos/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["IdClasse"] = new SelectList(_context.Classes, "IdClasse", "Nome");
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo");
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "Nome");
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "Nome");
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreateConfirmed([Bind("Ano,Quilometragem,Condicao,Caixa,IdModelo,IdMarca,IdCombustivel,IdClasse")] Veiculo veiculo, List<IFormFile> Imagens)
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
                
                

                // (4) Guardar cada imagem
                if (Imagens != null && Imagens.Count > 0)
                {
                    // Criar pasta do veiculo
                    var pastaVeiculo = Path.Combine("wwwroot/uploads/veiculos", veiculo.IdVeiculo.ToString());

                    if (!Directory.Exists(pastaVeiculo))
                        Directory.CreateDirectory(pastaVeiculo);


                    foreach (var file in Imagens)
                    {
                        if (file.Length > 0)
                        {
                            // Gerar nome único do ficheiro
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                            // Caminho COMPLETO para guardar no disco
                            var filePath = Path.Combine(pastaVeiculo, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Guardar só o NOME no SQL
                            var imagem = new Imagem
                            {
                                IdVeiculo = veiculo.IdVeiculo,
                                Nome = fileName
                            };

                            _context.Imagems.Add(imagem);
                        }
                    }
                }
                await _context.SaveChangesAsync(); // salvar imagens
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
                return NotFound();

            var veiculo = await _context.Veiculos
                .Include(v => v.Imagems)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdMarcaNavigation)
                .Include(v => v.IdCombustivelNavigation)
                .Include(v => v.IdClasseNavigation)
                .FirstOrDefaultAsync(v => v.IdVeiculo == id);

            if (veiculo == null)
                return NotFound();

            // Popular dropdowns
            ViewBag.IdMarca = new SelectList(_context.Marcas, "IdMarca", "Nome", veiculo.IdMarca);
            ViewBag.IdModelo = new SelectList(_context.Modelos.Where(m => m.IdMarca == veiculo.IdMarca), "IdModelo", "Nome", veiculo.IdModelo);
            ViewBag.IdClasse = new SelectList(_context.Classes, "IdClasse", "Nome", veiculo.IdClasse);
            ViewBag.IdCombustivel = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo", veiculo.IdCombustivel);

            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.IdVeiculo)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // repopular dropdowns em caso de erro
                ViewBag.IdMarca = new SelectList(_context.Marcas, "IdMarca", "Nome", veiculo.IdMarca);
                ViewBag.IdModelo = new SelectList(_context.Modelos, "IdModelo", "Nome", veiculo.IdModelo);
                ViewBag.IdClasse = new SelectList(_context.Classes, "IdClasse", "Nome", veiculo.IdClasse);
                ViewBag.IdCombustivel = new SelectList(_context.Combustivels, "IdCombustivel", "Nome", veiculo.IdCombustivel);

                return View(veiculo);
            }

            _context.Update(veiculo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = veiculo.IdVeiculo });
        }

        // POST: Veiculos/EditImages/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImages(int id, IFormFile[] NovasImagens, int[] RemoverIds)
        {
            if (RemoverIds != null && RemoverIds.Length > 0)
            {
                foreach (var imgId in RemoverIds)
                {
                    var imagem = await _context.Imagems.FirstOrDefaultAsync(i => i.IdImagem == imgId);

                    if (imagem != null)
                    {

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/veiculos", id.ToString(), imagem.Nome);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        _context.Imagems.Remove(imagem);
                        await _context.SaveChangesAsync();

                    }
                }
                
            }
            if (NovasImagens != null && NovasImagens.Length > 0)
            {
                // 1. Caminho da pasta do veículo
                var pastaVeiculo = Path.Combine("wwwroot/uploads/veiculos", id.ToString());

                // 2. Criar se não existir
                if (!Directory.Exists(pastaVeiculo))
                    Directory.CreateDirectory(pastaVeiculo);

                // 3. Loop pelas novas imagens
                foreach (var file in NovasImagens)
                {
                    if (file.Length > 0)
                    {
                        var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                        var filePath = Path.Combine(pastaVeiculo, filename);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var novaImagem = new Imagem
                        {
                            IdVeiculo = id,
                            Nome = filename
                        };

                        _context.Imagems.Add(novaImagem);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Edit), new { id });
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
                .Include(v => v.IdMarcaNavigation)
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

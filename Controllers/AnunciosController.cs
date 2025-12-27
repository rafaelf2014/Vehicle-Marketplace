using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Data;
using CliCarProject.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using CliCarProject.Models.Classes;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Data.SqlClient;

namespace CliCarProject.Controllers
{
    [Authorize]
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AnunciosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Anuncios
        public async Task<IActionResult> Index(string sortOrder)
        {
            var userId = _userManager.GetUserId(User); //Obtém o ID do usuário atualmente autenticado

<<<<<<< Updated upstream
            ViewBag.CurrentSort = sortOrder;

            var query = _context.Anuncios
        .Include(a => a.IdVeiculoNavigation)
        .ThenInclude(v => v.Imagems)
        .Where(a => a.Estado == "Ativo") // Apenas anúncios ativos
    .Include(a => a.IdVeiculoNavigation)
        .ThenInclude(v => v.IdMarcaNavigation)
        .Include(v => v.IdVeiculoNavigation)
        .Where(a => a.IdVendedor == userId) // Exclui anúncios do vendedor autenticado
    .Include(a => a.IdVeiculoNavigation)
        .ThenInclude(v => v.IdModeloNavigation)
    .Include(a => a.IdVeiculoNavigation)
        .ThenInclude(v => v.IdCombustivelNavigation)
    //.Include(a => a.IdLocalizacaoNavigation) // podes manter se usares depois
    .AsQueryable();
=======
            var query = _context.Anuncios
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.Imagems)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.IdMarcaNavigation)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.IdModeloNavigation)
            .Include(a => a.IdVeiculoNavigation)
                .ThenInclude(v => v.IdCombustivelNavigation)
            .Where(User != null ? a => a.IdVendedor == userId : a => true)
            //.Include(a => a.IdLocalizacaoNavigation)
            .AsQueryable();
>>>>>>> Stashed changes


            query = sortOrder switch
            {
                "ano_asc" => query.OrderBy(a => a.DataCriacao),
                "ano_desc" => query.OrderByDescending(a => a.DataCriacao),
                _ => query.OrderByDescending(a => a.DataCriacao), // Padrão: mais recentes primeiro
            };
            var anuncios = await query.ToListAsync();

            var destaques = await _context.Anuncios
            .OrderByDescending(a => a.NVisitas)
            .Take(10)
            .ToListAsync();



            return View(anuncios);
        }


        // GET: Anuncios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.Imagems)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdModeloNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdCombustivelNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdClasseNavigation)
                .Include(a => a.IdLocalizacaoNavigation)
                .Include(a => a.IdVendedorNavigation) // IdentityUser do vendedor
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            var currentUserId = _userManager.GetUserId(User);

            // Só conta se o visitante não for o dono do anúncio
            if (anuncio.IdVendedor != currentUserId)
            {
                anuncio.Visualizacoes += 1;
                await _context.SaveChangesAsync();
            }

            if (anuncio == null)
            {
                return NotFound();
            }
            anuncio.NVisitas++;
            await _context.SaveChangesAsync();


            return View(anuncio);
        }


        // GET: Anuncios/Create
        [Authorize] //Quando tivermos sistema de Roles trocamos por [Athorize(Roles="Vendedor")];
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User); //Obtém o ID do usuário atualmente autenticado 

            //Garantimos que os veículos disponiveis para escolha são apenas os do vendedor autenticado no momento;
            //Para isso, comparamos o IdVendedor do veículo com o userId obtido acima
            ViewData["IdVeiculo"] = new SelectList(
    _context.Veiculos
        .Where(v => v.IdVendedor == userId)
        .Select(v => new
        {
            v.IdVeiculo,
            Nome = v.IdModeloNavigation.Nome + " (" + v.Ano + ")"
        }),
    "IdVeiculo",
    "Nome"
);


            ViewData["IdLocalizacao"] = new SelectList(_context.Localizacaos, "IdLocalizacao", "Distrito");
  
            return View();
        }

        // POST: Anuncios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //Quando tivermos sistema de Roles trocamos por [Athorize(Roles="Vendedor")];
        public async Task<IActionResult> Create([Bind("Titulo,Descricao,Preco,IdVeiculo,IdLocalizacao")] Anuncio anuncio)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); //Redireciona para a página de login se o usuário não estiver autenticado
            }
            //Garantimos que os veículos disponiveis para escolha são apenas os do vendedor autenticado no momento;

            var veiculo = _context.Veiculos
                .FirstOrDefault(v => v.IdVeiculo == anuncio.IdVeiculo && v.IdVendedor == userId);

            if (veiculo == null)
            {
                return Unauthorized();
            }

            anuncio.IdVendedor = userId; //Atribui o Id do vendedor ao anúncio

            anuncio.DataCriacao = DateTime.Now;
            anuncio.DataAtualizacao = DateTime.Now;
            anuncio.Estado = "Ativo"; //Definimos o estado inicial do anúncio como "Ativo"

            if (!ModelState.IsValid)
            {
                //Console.WriteLine("❌ ModelState inválido!");

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Campo: {entry.Key} → ERRO: {error.ErrorMessage}");
                    }
                }   //Percorre os modelsState e imprime os erros no console

                // Recarregar dropdowns
                ViewData["IdVeiculo"] = new SelectList(
                    _context.Veiculos.Where(v => v.IdVendedor == userId),
                    "IdVeiculo",
                    "IdVeiculo",
                    anuncio.IdVeiculo
                );

                ViewData["IdLocalizacao"] = new SelectList(
                    _context.Localizacaos,
                    "IdLocalizacao",
                    "Distrito",
                    anuncio.IdLocalizacao
                );

                return View(anuncio);
            }

            //Console.WriteLine("✅ Anúncio válido");
            _context.Anuncios.Add(anuncio);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Anuncios");
        }

        // GET: Anuncios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }
            ViewData["IdLocalizacao"] = new SelectList(_context.Localizacaos, "IdLocalizacao", "IdLocalizacao", anuncio.IdLocalizacao);
            ViewData["IdVeiculo"] = new SelectList(_context.Veiculos, "IdVeiculo", "IdVeiculo", anuncio.IdVeiculo);
            return View(anuncio);
        }

        // POST: Anuncios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Titulo,Descricao,Preco,IdVeiculo,IdVendedor,IdLocalizacao")] Anuncio anuncio)
        {
            if (id != anuncio.IdAnuncio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.IdAnuncio))
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
            ViewData["IdLocalizacao"] = new SelectList(_context.Localizacaos, "IdLocalizacao", "IdLocalizacao", anuncio.IdLocalizacao);
            ViewData["IdVeiculo"] = new SelectList(_context.Veiculos, "IdVeiculo", "IdVeiculo", anuncio.IdVeiculo);
            return View(anuncio);
        }

        // GET: Anuncios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.IdLocalizacaoNavigation)
                .Include(a => a.IdVeiculoNavigation)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio != null)
            {
                _context.Anuncios.Remove(anuncio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.IdAnuncio == id);
        }
    }
}

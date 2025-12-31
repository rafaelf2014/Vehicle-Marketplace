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
            var userId = _userManager.GetUserId(User);
            ViewBag.CurrentSort = sortOrder;

            var query = _context.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .Include(a => a.IdLocalizacaoNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.Imagems)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdModeloNavigation)
                .Where(a => a.Estado == "Ativo")
                .Where(a => a.IdVeiculoNavigation.Disponivel == true)
                .Where(a => a.IdVendedor == userId)
                .AsQueryable();

            // Lógica de ordenação (switch)
            query = sortOrder switch
            {
                "ano_asc" => query.OrderBy(a => a.IdVeiculoNavigation.Ano),
                "ano_desc" => query.OrderByDescending(a => a.IdVeiculoNavigation.Ano),
                _ => query.OrderByDescending(a => a.DataCriacao)
            };

            return View(await query.ToListAsync());
        }

        // GET: Anuncios/Details/5 (Inclui incremento de visualizações)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.Imagems)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdModeloNavigation)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdCombustivelNavigation)
                .Include(a => a.IdLocalizacaoNavigation)
                .Include(a => a.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);

            if (anuncio == null) return NotFound();

            // Lógica de Favoritos
            var userId = _userManager.GetUserId(User);
            ViewBag.IsFavorito = false;

            if (userId != null)
            {
                ViewBag.IsFavorito = await _context.Favoritos
                    .AnyAsync(f => f.IdAnuncio == id && f.IdUtilizador == userId);
            }

            // Incremento de visualizações
            if (anuncio.IdVendedor != userId)
            {
                anuncio.Visualizacoes++;
                await _context.SaveChangesAsync();
            }

            return View(anuncio);
        }

        
        // GET: Anuncios/Create
        [Authorize(Roles ="Vendedor")] 
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User); //Obtém o ID do usuário atualmente autenticado 

            //Garantimos que os veículos disponiveis para escolha são apenas os do vendedor autenticado no momento;
            //Para isso, comparamos o IdVendedor do veículo com o userId obtido acima
            ViewData["IdVeiculo"] = new SelectList(
            _context.Veiculos
                .Where(v => v.IdVendedor == userId && v.Disponivel == true)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Vendedor")] //Quando tivermos sistema de Roles trocamos por [Athorize(Roles="Vendedor")];
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

            return RedirectToAction("Index", "Anuncios");
        }

        // GET: Anuncios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdModeloNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            if (anuncio == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            // Carrega o veículo com o Nome formatado para o Select
            ViewData["IdVeiculo"] = new SelectList(
                _context.Veiculos
                    .Where(v => v.IdVendedor == userId)
                    .Select(v => new
                    {
                        v.IdVeiculo,
                        Nome = v.IdModeloNavigation.Nome + " (" + v.Ano + ")"
                    }),
                "IdVeiculo", "Nome", anuncio.IdVeiculo);

            // Carrega as localizações mostrando o nome do Distrito
            ViewData["IdLocalizacao"] = new SelectList(_context.Localizacaos, "IdLocalizacao", "Distrito", anuncio.IdLocalizacao);

            return View(anuncio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnuncio,Titulo,Descricao,Preco,IdLocalizacao,Estado")] Anuncio anuncio)
        {
            if (id != anuncio.IdAnuncio) return NotFound();

            // 1. Carregamos o anúncio original da BD para manter o IdVendedor e IdVeiculo intactos
            var anuncioOriginal = await _context.Anuncios
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            if (anuncioOriginal == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 2. Reatribuímos os valores que não podem mudar ou que não vieram do formulário
                    anuncio.IdVendedor = anuncioOriginal.IdVendedor;
                    anuncio.IdVeiculo = anuncioOriginal.IdVeiculo;
                    anuncio.DataCriacao = anuncioOriginal.DataCriacao;
                    anuncio.DataAtualizacao = DateTime.Now;

                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();

                    // 3. Redirecionamento explícito para os Detalhes do anúncio acabado de editar
                    return RedirectToAction(nameof(Details), new { id = anuncio.IdAnuncio });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.IdAnuncio)) return NotFound();
                    else throw;
                }
            }

            // Se houver erro de validação, recarregamos as dropdowns para a View não quebrar
            var userId = _userManager.GetUserId(User);
            ViewData["IdVeiculo"] = new SelectList(_context.Veiculos.Where(v => v.IdVendedor == userId), "IdVeiculo", "IdVeiculo", anuncio.IdVeiculo);
            ViewData["IdLocalizacao"] = new SelectList(_context.Localizacaos, "IdLocalizacao", "Distrito", anuncio.IdLocalizacao);

            return View(anuncio);
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.IdAnuncio == id);
        }

        [Authorize]
        public async Task<IActionResult> Favoritos()
        {
            var userId = _userManager.GetUserId(User);

            var favoritos = await _context.Favoritos
                .Where(f => f.IdUtilizador == userId)
                // FILTRO IMPORTANTE: Só traz favoritos cujo veículo associado esteja disponível
                .Where(f => f.Anuncio.IdVeiculoNavigation.Disponivel == true)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.IdVeiculoNavigation)
                        .ThenInclude(v => v.Imagems)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.IdLocalizacaoNavigation)
                .Select(f => f.Anuncio)
                .ToListAsync();

            return View(favoritos);
        }

        // POST: Anuncios/ToggleFavorito
        // Método chamado via AJAX para adicionar/remover favorito
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorito(int idAnuncio)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Json(new { success = false, message = "Sessão expirada" });

            var favoritoExistente = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.IdAnuncio == idAnuncio && f.IdUtilizador == userId);

            bool isFavorito;

            if (favoritoExistente != null)
            {
                _context.Favoritos.Remove(favoritoExistente);
                isFavorito = false;
            }
            else
            {
                _context.Favoritos.Add(new Favorito { IdAnuncio = idAnuncio, IdUtilizador = userId });
                isFavorito = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isFavorito = isFavorito });
        }


        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Procuramos o anúncio na base de dados
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio != null)
            {
                // Verificar se quem está a tentar eliminar é o dono do anúncio
                var currentUserId = _userManager.GetUserId(User);
                if (anuncio.IdVendedor != currentUserId)
                {
                    return Unauthorized();
                }

                // EM VEZ DE: _context.Anuncios.Remove(anuncio);
                // FAZEMOS: Alterar o estado para Inativo
                anuncio.Estado = "Inativo";
                anuncio.DataAtualizacao = DateTime.Now;

                _context.Update(anuncio);
                await _context.SaveChangesAsync();
            }

            // Retorna para a página Index dos anúncios
            return RedirectToAction(nameof(Index));
        }
    }
}

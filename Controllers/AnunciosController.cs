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
using NuGet.Packaging.Signing;

namespace CliCarProject.Controllers
{
    
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
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            var userId = _userManager.GetUserId(User);
            int pageSize = 5;

            // Base query com todos os Includes necessários para a Partial View
            var baseQuery = _context.Anuncios
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.Imagems)
                .Include(a => a.VisitaReservas) 
                .Include(a => a.Compra)         
                .Where(a => a.IdVendedor == userId && a.Estado != "Inativo");

            baseQuery = sortOrder switch
            {
                "ano_asc" => baseQuery.OrderBy(a => a.IdVeiculoNavigation.Ano),
                "ano_desc" => baseQuery.OrderByDescending(a => a.IdVeiculoNavigation.Ano),
                _ => baseQuery.OrderByDescending(a => a.IdAnuncio)
            };

            // 1. ATIVOS:
            var ativosLista = await baseQuery
                .Where(a => a.Estado == "Ativo" && !a.VisitaReservas.Any(r => r.Estado == "Pendente"))
                .ToListAsync();
            ViewBag.Ativos = ativosLista.Take(pageSize).ToList();
            ViewBag.TotalAtivos = ativosLista.Count;
            ViewBag.HasNextAtivos = ativosLista.Count > pageSize;

            // 2. PENDENTES: Anúncios que aguardam ação do vendedor
            var pendentesLista = await baseQuery
                .Where(a => a.Estado == "Pendente")
                .ToListAsync();
            ViewBag.Pendentes = pendentesLista.Take(pageSize).ToList();
            ViewBag.TotalPendentes = pendentesLista.Count;
            ViewBag.HasNextPendentes = pendentesLista.Count > pageSize;

            //var totalPendentesNaBD = await _context.Anuncios.CountAsync(a => a.Estado == "Pendente");
            //Console.WriteLine($"DEBUG: Total de anúncios com Estado 'Pendente' na BD: {totalPendentesNaBD}");

            //foreach (var a in pendentesLista)
            //{
            //    Console.WriteLine($"Anúncio ID: {a.IdAnuncio} | Estado: {a.Estado} | Reservas: {a.VisitaReservas?.Count ?? 0}");
            //}

            // 3. RESERVADOS: Vendedor já confirmou
            var reservadosLista = await baseQuery
                .Where(a => a.Estado == "Reservado")
                .ToListAsync();
            ViewBag.Reservados = reservadosLista.Take(pageSize).ToList();
            ViewBag.TotalReservados = reservadosLista.Count;
            ViewBag.HasNextReservados = reservadosLista.Count > pageSize;

            // 4. VENDIDOS: Ciclo finalizado
            var vendidosLista = await baseQuery
                .Where(a => a.Estado == "Vendido")
                .ToListAsync();
            ViewBag.Vendidos = vendidosLista.Take(pageSize).ToList();
            ViewBag.TotalVendidos = vendidosLista.Count;
            ViewBag.HasNextVendidos = vendidosLista.Count > pageSize;

            return View();
        }
        [Authorize]
        public async Task<IActionResult> GetAnunciosPaginados(string tipo, int pagina = 1)
        {
            var userId = _userManager.GetUserId(User);
            int pageSize = 5;

            // 1. Base Query com TODOS os Includes necessários para a Partial View
            var query = _context.Anuncios
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.Imagems)
                .Include(a => a.VisitaReservas) // ESSENCIAL para os botões de reserva aparecerem
                .Where(a => a.IdVendedor == userId && a.Estado != "Inativo");


            switch (tipo.ToLower())
            {
                case "ativos":
                    // Apenas anúncios que não têm qualquer processo de reserva iniciado
                    query = query.Where(a => a.Estado == "Ativo" &&
                                !a.VisitaReservas.Any(r => r.Estado == "Pendente"));
                    break;

                case "pendentes":
                    // IMPORTANTE: Alinhado com o método Reservar que define Estado = "Pendente"
                    query = query.Where(a => a.Estado == "Pendente")
                                .Include(a => a.VisitaReservas);
                    break;

                case "reservados":
                    // Requisito: Listagens de veículos reservados 
                    query = query.Where(a => a.Estado == "Reservado")
                                .Include(a => a.Compra) // Necessário para validar se está PAGO na Partial
                                .Include(a => a.VisitaReservas);
                    break;

                case "vendidos":
                    // Requisito: Listagens de veículos vendidos 
                    query = query.Where(a => a.Estado == "Vendido")
                                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.Imagems);
                    break;
            }

            // 3. Ordenação (Importante para o novo anúncio não "saltar" de página)
            query = query.OrderByDescending(a => a.IdAnuncio);

            var totalItens = await query.CountAsync();
            var lista = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. Cabeçalho para o JavaScript gerir os botões Next/Prev
            Response.Headers.Add("X-Has-Next", (totalItens > pagina * pageSize).ToString().ToLower());

            return PartialView("~/Views/Anuncios/PartialViews/_CardsPartial.cshtml", lista);
        }

        // GET: Anuncios/Details/5 (Inclui incremento de visualizações)
        public async Task<IActionResult> Details(int? id)
        {
            var userId = _userManager.GetUserId(User);

            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.Imagems)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdModeloNavigation)
                .Include(a => a.IdVeiculoNavigation).ThenInclude(v => v.IdCombustivelNavigation)
                .Include(a => a.IdLocalizacaoNavigation)
                .Include(a => a.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);

            var reservaAtiva = await _context.VisitaReservas
                .FirstOrDefaultAsync(r => r.IdAnuncio == id && r.Estado == "Pendente" && r.DataExpiracao > DateTime.Now);

            if (reservaAtiva != null)
            {
                ViewBag.IsReservado = true;
                ViewBag.ReservadoPorMim = (reservaAtiva.IdComprador == userId);
                ViewBag.DataExpiracao = reservaAtiva.DataExpiracao;
            }
            else
            {
                ViewBag.IsReservado = false;
                if (anuncio.Estado == "Reservado")
                {
                    anuncio.Estado = "Ativo";
                    
                    var reservaObsoleta = await _context.VisitaReservas
                        .FirstOrDefaultAsync(r => r.IdAnuncio == id && r.Estado == "Pendente");
                    if (reservaObsoleta != null) reservaObsoleta.Estado = "Expirada";

                    await _context.SaveChangesAsync();
                }
            }

            if (anuncio == null) return NotFound();

            // Lógica de Favoritos
            
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
        [Authorize(Roles ="Vendedor")]
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
            if(userId != anuncio.IdVendedor)
            {
                return Unauthorized();
            }
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

            if (anuncioOriginal.Estado == "Reservado")
            {
                // Permite editar apenas título e descrição, mas não o preço
                if (anuncio.Preco != anuncioOriginal.Preco)
                {
                    ModelState.AddModelError("Preco", "Não pode alterar o preço de um veículo com reserva ativa.");
                    return View(anuncio);
                }
            }

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
        [Authorize(Roles = "Vendedor")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Carregamos o anúncio garantindo que pertence ao utilizador logado
            var anuncio = await _context.Anuncios
                .Include(a => a.VisitaReservas)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            // Lógica de Integridade: Só desativamos se não houver um processo de compra em curso
            // Verificamos se há reservas que NÃO estejam canceladas nem concluídas
            bool temProcessoAtivo = anuncio.VisitaReservas.Any(r => r.Estado == "Pendente" || r.Estado == "Confirmada");

            if (anuncio.Estado != "Vendido" && !temProcessoAtivo)
            {
                anuncio.Estado = "Inativo";
                anuncio.DataAtualizacao = DateTime.Now;

                TempData["Success"] = "Anúncio removido com sucesso.";
            }
            else
            {
                TempData["Error"] = "Não pode eliminar um anúncio que tenha reservas ativas ou que já tenha sido vendido.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles ="Comprador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int idAnuncio, DateTime? dataSugerida)
        {
            if (!ModelState.IsValid)
            {
                // Se entrar aqui, a data ou o ID vieram em formato errado
                var erros = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Erro de validação: " + erros;
                return RedirectToAction(nameof(Details), new { id = idAnuncio });
            }

            var userId = _userManager.GetUserId(User);

            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == idAnuncio);

            if (anuncio == null) return NotFound();
            //Verifica se o usuário é o vendedor do anúncio
            if (anuncio.IdVendedor == userId)
            {
                TempData["Error"] = "Não pode reservar o seu próprio veículo.";
                return RedirectToAction(nameof(Details), new { id = idAnuncio });
            }
            //Verifica se o anúncio já está vendido ou se o veículo não está disponível
            if (anuncio.Estado == "Vendido" || anuncio.IdVeiculoNavigation.Disponivel == false)
            {
                TempData["Error"] = "Este veículo já não está disponível para reserva.";
                return RedirectToAction(nameof(Details), new { id = idAnuncio });
            }
            //Verifica se já existe uma reserva pendente para este anúncio
            var reservaExistente = await _context.VisitaReservas
                    .FirstOrDefaultAsync(r => r.IdAnuncio == idAnuncio &&
                                 r.Estado == "Pendente" &&
                                 r.DataExpiracao > DateTime.Now);

            if (reservaExistente != null)
            {
                TempData["Error"] = "Este veículo acabou de ser reservado por outro utilizador.";
                return RedirectToAction(nameof(Details), new { id = idAnuncio });
            }
            //Cria uma nova reserva
            var novaReserva = new VisitaReserva
            {
                IdAnuncio = idAnuncio,
                IdComprador = userId,
                DataVisita = dataSugerida ?? DateTime.Now.AddDays(1),
                DataExpiracao = (dataSugerida ?? DateTime.Now).AddDays(2),
                Estado = "Pendente"
            };

            anuncio.Estado = "Pendente";
            anuncio.Notificacao = true;

            try
            {
                _context.VisitaReservas.Add(novaReserva);
                _context.Update(anuncio);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Veículo reservado! Tem 24h para contactar o vendedor.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocorreu um erro ao processar a reserva. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id = idAnuncio });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(int idAnuncio)
        {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios
                .Include(a => a.VisitaReservas)
                .FirstOrDefaultAsync(a => a.IdAnuncio == idAnuncio && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            // 1. Atualizar o estado do anúncio
            anuncio.Estado = "Reservado";

            // 2. Atualizar o estado da visita/reserva na tabela relacionada
            var reserva = anuncio.VisitaReservas.FirstOrDefault(r => r.Estado == "Pendente");
            if (reserva != null)
            {
                reserva.Estado = "Confirmada";
                reserva.DataExpiracao = DateTime.Now.AddDays(2); // Dá 48h após confirmação
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Reserva confirmada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

       
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteSelected(int[] ids)
        //{
        //    if (ids == null || ids.Length == 0) return RedirectToAction(nameof(Index));

        //    try
        //    {
        //        var anunciosParaApagar = await _context.Anuncios
        //            .Where(a => ids.Contains(a.IdAnuncio) && a.Estado == "Ativo")
        //            .ToListAsync();

        //        foreach (var anuncio in anunciosParaApagar)
        //        {
        //            // Verifica se realmente não tem reservas antes de apagar
        //            var temReserva = await _context.VisitaReservas.AnyAsync(r => r.IdAnuncio == anuncio.IdAnuncio);
        //            if (!temReserva)
        //            {
        //                anuncio.Estado = "Inativo";
        //            }
        //        }
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "Anúncios eliminados com sucesso.";
        //    }
        //    catch (Exception)
        //    {
        //        TempData["Error"] = "Erro ao eliminar anúncios.";
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelarReserva(int idAnuncio)
        {
            var userId = _userManager.GetUserId(User);

            var anuncio = await _context.Anuncios.FindAsync(idAnuncio);
            if (anuncio == null) return NotFound();
            // Verifica se é o comprador da reserva ou o vendedor do anúncio
            var reserva = await _context.VisitaReservas
                .FirstOrDefaultAsync(r => r.IdAnuncio == idAnuncio &&
                                 (r.Estado == "Pendente" || r.Estado == "Confirmada"));

            if (reserva == null) return NotFound();

            if (reserva.IdComprador != userId && anuncio.IdVendedor != userId)
                return Unauthorized();

            // Reset do estado
            reserva.Estado = "Cancelada";
            if (reserva.IdAnuncioNavigation != null)
            {
                reserva.IdAnuncioNavigation.Estado = "Ativo";
            }
            anuncio.Notificacao = false;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reserva cancelada. O veículo está novamente disponível.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Erro ao atualizar a base de dados.";
            }

            if (anuncio.IdVendedor == userId)
                return RedirectToAction("Index");

            return RedirectToAction(nameof(Details), new { id = idAnuncio });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmarVenda(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Procura o anúncio e inclui o veículo para o podermos atualizar
            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            var compraPaga = await _context.Compras
                .AnyAsync(c => c.IdAnuncio == id && c.Estado == "Pago");

            if (!compraPaga)
            {
                TempData["Error"] = "Não pode confirmar a venda antes do pagamento ser registado.";
                return RedirectToAction(nameof(Index));
            }

            if (anuncio == null) return NotFound();

            // 1. Atualizar o estado do Anúncio
            anuncio.Estado = "Vendido";
            anuncio.DataAtualizacao = DateTime.Now;

            // 2. Retirar o veículo de circulação (Disponibilidade = false ou 0)
            if (anuncio.IdVeiculoNavigation != null)
            {
                anuncio.IdVeiculoNavigation.Disponivel = false;
            }

            // 3. Fechar a reserva na tabela VisitaReserva
            var reservaAtiva = await _context.VisitaReservas
                .FirstOrDefaultAsync(v => v.IdAnuncio == id && v.Estado == "Pendente");

            var veiculo = anuncio.IdVeiculoNavigation;

            if (veiculo != null)
            {
                // O veículo agora pertence ao comprador que fez a reserva
                veiculo.IdVendedor = reservaAtiva.IdComprador;

                // Marcamos como Disponível para que apareça na "Garagem" do novo dono, 
                veiculo.Disponivel = true;
            }

            reservaAtiva.Estado = "Concluida";

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Venda concluída! O veículo foi transferido para o novo proprietário.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Erro ao processar a transferência do veículo.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> MinhasReservas()
        {
            var userId = _userManager.GetUserId(User);

            // Listar todas as visitas/reservas feitas pelo utilizador logado
            var reservas = await _context.VisitaReservas
        .Include(v => v.IdAnuncioNavigation)
            .ThenInclude(a => a.IdVeiculoNavigation)
        .Include(v => v.IdAnuncioNavigation)
            .ThenInclude(a => a.Compra)
        .Where(v => v.IdComprador == userId)
        .OrderByDescending(v => v.DataVisita)
        .ToListAsync();

            return View(reservas);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PagarEncomenda(int idCompra)
        {
            var userId = _userManager.GetUserId(User);

            // 1. Carregar a Compra E o Anúncio relacionado (Eager Loading)
            var compra = await _context.Compras
                .Include(c => c.IdAnuncioNavigation)
                .FirstOrDefaultAsync(c => c.IdAnuncio == idCompra && c.IdComprador == userId);

            if (compra == null || compra.Estado != "Pendente")
                return BadRequest("Encomenda não encontrada ou já processada.");

            // 2. Atualizar o estado da Compra para Pago
            compra.Estado = "Pago";

            // 3. Importante: Atualizar o estado do Anúncio para indicar que aguarda confirmação final
            if (compra.IdAnuncioNavigation != null)
            {
                // Opcional: podes definir um estado intermédio ou manter "Reservado" 
                // mas garantir que a propriedade de navegação não é nula.
                compra.IdAnuncioNavigation.Estado = "Reservado";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Pagamento simulado com sucesso! O vendedor já pode confirmar a entrega.";
            return RedirectToAction("MinhasReservas");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelarEncomenda(int idAnuncio)
        {
            var userId = _userManager.GetUserId(User);
            var compra = await _context.Compras
                .FirstOrDefaultAsync(c => c.IdAnuncio == idAnuncio && c.IdComprador == userId);

            if (compra != null)
            {
                compra.Estado = "Cancelado";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Compra cancelada com sucesso.";
            }

            return RedirectToAction(nameof(MinhasReservas));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DetalhesCheckout(int idAnuncio)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == idAnuncio);

            if (anuncio == null || anuncio.Estado != "Reservado")
                return NotFound();

            return View(anuncio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int idAnuncio)
        {
            var userId = _userManager.GetUserId(User);

            // 1. Procuramos a reserva DIRETAMENTE na tabela para evitar problemas de Include
            var reservaAtiva = await _context.VisitaReservas
                .FirstOrDefaultAsync(r => r.IdAnuncio == idAnuncio && r.IdComprador == userId);

            // Se não encontrar a reserva para este user, aí sim damos o 401
            if (reservaAtiva == null)
            {
                return Unauthorized("Não tem uma reserva ativa para este veículo.");
            }

            // 2. Procuramos o anúncio para validar o estado e o preço
            var anuncio = await _context.Anuncios.FindAsync(idAnuncio);

            if (anuncio == null || anuncio.Estado != "Reservado")
            {
                return BadRequest("O anúncio não está disponível para checkout (não está reservado).");
            }

            // 3. Registo da Encomenda (Protocolo: simulação de checkout)
            var compra = await _context.Compras
                .FirstOrDefaultAsync(c => c.IdAnuncio == idAnuncio && c.IdComprador == userId);

            if (compra == null)
            {
                compra = new Compra
                {
                    IdAnuncio = idAnuncio,
                    IdComprador = userId,
                    DataCompra = DateTime.Now,
                    Estado = "Pendente",
                    Preco = anuncio.Preco 
                };
                _context.Compras.Add(compra);
            }
            else
            {
                compra.Estado = "Pendente";
                _context.Update(compra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MinhasReservas");
        }
    }
}

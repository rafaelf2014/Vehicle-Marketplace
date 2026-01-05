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
        public IActionResult CarSearch(string searchBox, int? marcaId, int? modeloId, int? categoriaId, int? combustivelId, string caixa, decimal? minPrice,decimal? maxPrice, string sortOrder, int? minYear, int? maxYear, int? localizacaoId, int? minKm, int? maxKm, string condicao ="",bool ocultarReservados = false , int page = 1)
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
                .Include(a => a.VisitaReservas)
                .AsQueryable();


            query = query.Where(a =>
                  (a.Estado == "Ativo" ||
                  a.Estado == "Reservado" ||
                  // Se estiver com reserva, mostra se tiver reserva "Reservada", "Confirmada" ou "Pendente"
                  a.VisitaReservas.Any(v =>
                      v.Estado.Contains("Reservada") ||
                      v.Estado.Contains("Confirmada") ||
                      v.Estado.Contains("Pendente")
                  )) &&
                  // Exclui explicitamente anúncios Inativo, Indisponivel e Vendido
                  a.Estado != "Inativo" &&
                  a.Estado != "Indisponivel" &&
                  a.Estado != "Vendido"
              );

            if (ocultarReservados)
            {
                // Garante que esconde tudo o que tenha qualquer tipo de reserva
                query = query.Where(a => !a.VisitaReservas.Any(v => v.Estado.Contains("Reservada") || v.Estado.Contains("Confirmada")));
            }

            if (!string.IsNullOrEmpty(condicao))
            {
                // Nota: Verifica se na tua BD o valor é "Novo"/"Usado" ou "New"/"Used"
                query = query.Where(a => a.IdVeiculoNavigation.Condicao == condicao);
            }
       
            ViewBag.CurrentCondicao = condicao;

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

        [HttpPost]
        public async Task<IActionResult> GuardarFiltro(string nome, string filtrosJson)
        {
            // 1. Simular Utilizador Logado (Ajusta depois para User.Identity.GetUserId())
            // Tens de garantir que este ID existe na tabela Comprador/User
            var userId = "e797aeee-bf4c-4235-8664-000000000000";

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(filtrosJson))
            {
                return Json(new { success = false, message = "Dados inválidos." });
            }

            // 2. Criar o objeto com os dados
            var novoFiltro = new FiltrosFavorito
            {
                IdComprador = userId,
                Nome = nome,
                FiltrosJson = filtrosJson, // Guardamos tudo aqui!

                // Opcional: Se quiseres preencher as colunas antigas para estatística
                // podes tentar extrair do JSON, mas não é obrigatório para a pesquisa funcionar.
                IdMarca = null,
                IdCombustivel = null
            };

            _context.FiltrosFavoritos.Add(novoFiltro);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ObterMeusFiltros()
        {
            var userId = "e797aeee-bf4c-4235-8664-000000000000"; // ID fixo para teste

            var filtros = _context.FiltrosFavoritos
                .Where(f => f.IdComprador == userId)
                .Select(f => new {
                    id = f.IdFiltroFavorito,
                    nome = f.Nome,
                    json = f.FiltrosJson
                })
                .ToList();

            return Json(filtros);
        }

        [HttpPost]
        public async Task<IActionResult> ApagarFiltro(int id)
        {
            // Verifica se o filtro existe e pertence ao user (usando o ID de teste que definimos)
            var userId = "e797aeee-bf4c-4235-8664-000000000000";

            var filtro = await _context.FiltrosFavoritos.FindAsync(id);

            if (filtro == null || filtro.IdComprador != userId)
            {
                return Json(new { success = false, message = "Filtro não encontrado." });
            }

            _context.FiltrosFavoritos.Remove(filtro);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}

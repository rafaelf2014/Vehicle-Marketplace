using System;
using System.Linq;
using System.Threading.Tasks;
using CliCarProject.Data;
using CliCarProject.Models;
using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CliCarProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Painel de Controlo";
            ViewData["AdminActive"] = "Dashboard";

            var totalVisits = await _context.SitePageViews.CountAsync();
            var totalAccounts = await _context.Users.CountAsync();
            var totalAnuncios = await _context.Anuncios.CountAsync();
            var totalVeiculos = await _context.Veiculos.CountAsync();

            var weekThreshold = DateTime.UtcNow.AddDays(-7);
            var weeklyVisits = await _context.SitePageViews.CountAsync(s => s.VisitTime >= weekThreshold);

            const int days = 30;
            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            // VISITAS
            var visitsGrouped = await _context.SitePageViews
                .Where(s => s.VisitTime >= startDate)
                .GroupBy(s => s.VisitTime.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            var visitLabels = new List<string>(days);
            var visitCounts = new List<int>(days);

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                visitLabels.Add(date.ToString("yyyy-MM-dd"));
                var found = visitsGrouped.FirstOrDefault(v => v.Date == date);
                visitCounts.Add(found?.Count ?? 0);
            }

            // VENDAS → usa tabela Compra
            var totalVendas = await _context.Compras.CountAsync();

            var vendasGrouped = await _context.Compras
                .Where(c => c.DataCompra.HasValue && c.DataCompra.Value.Date >= startDate)
                .GroupBy(c => c.DataCompra.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            var salesLabels = new List<string>(days);
            var salesCounts = new List<int>(days);

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                salesLabels.Add(date.ToString("yyyy-MM-dd"));
                var foundSale = vendasGrouped.FirstOrDefault(v => v.Date == date);
                salesCounts.Add(foundSale?.Count ?? 0);
            }

            var model = new AdminDashboardViewModel
            {
                TotalSiteVisits = (int)totalVisits,
                TotalAccountsCreated = (int)totalAccounts,
                TotalUsers = (int)totalAccounts,
                TotalAnuncios = (int)totalAnuncios,
                TotalVeiculos = (int)totalVeiculos,
                WeeklySiteVisits = (int)weeklyVisits,
                VisitLabels = visitLabels,
                VisitCounts = visitCounts,
                TotalVendas = totalVendas,
                SalesLabels = salesLabels,
                SalesCounts = salesCounts
            };

            return View(model);
        }

        // GET: /Admin/UserManage
        public async Task<IActionResult> UserManage(string searchTerm, string sortBy, string sortOrder)
        {
            ViewData["AdminActive"] = "Users";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;

            ViewBag.IdSortOrder = sortBy == "id" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.EmailSortOrder = sortBy == "email" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.UsernameSortOrder = sortBy == "username" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.CreatedAtSortOrder = sortBy == "createdAt" && sortOrder == "asc" ? "desc" : "asc";

            var totalAccounts = await _context.Users.CountAsync();
            var totalCompradores = await _context.Compradors.CountAsync();
            var totalVendedores = await _context.Vendedors.CountAsync();

            var compradoresIds = await _context.Compradors.Select(c => c.IdUtilizador).ToListAsync();
            var vendedoresIds = await _context.Vendedors.Select(v => v.IdUtilizador).ToListAsync();
            var blockedIds = await _context.UserBlocks.Select(b => b.UserId).ToListAsync();

            var compradoresSet = compradoresIds.ToHashSet();
            var vendedoresSet = vendedoresIds.ToHashSet();
            var blockedSet = blockedIds.ToHashSet();

            var identityUsersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                identityUsersQuery = identityUsersQuery.Where(u =>
                    u.Id.ToLower().Contains(searchTerm) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(searchTerm)));
            }

            var identityUsers = await identityUsersQuery.ToListAsync();
            var users = new List<UsersViewModel.UserListItem>(identityUsers.Count);

            foreach (var iu in identityUsers)
            {
                var claims = await _userManager.GetClaimsAsync(iu);
                var createdAtClaim = claims.FirstOrDefault(c => c.Type == "CreatedAt");
                DateTime? createdAt = null;
                if (createdAtClaim != null && DateTime.TryParse(createdAtClaim.Value, null,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                {
                    createdAt = dt;
                }

                var isSuperAdmin = string.Equals(iu.Email, "superadmin@clicar.local", StringComparison.OrdinalIgnoreCase);

                users.Add(new UsersViewModel.UserListItem
                {
                    Id = iu.Id,
                    Email = iu.Email ?? string.Empty,
                    UserName = iu.UserName ?? string.Empty,
                    Role = (compradoresSet.Contains(iu.Id) ? "Comprador" : string.Empty)
                           + (compradoresSet.Contains(iu.Id) && vendedoresSet.Contains(iu.Id) ? ", " : string.Empty)
                           + (vendedoresSet.Contains(iu.Id) ? "Vendedor" : string.Empty),
                    CreatedAt = createdAt,
                    IsBlocked = blockedSet.Contains(iu.Id),
                    IsSuperAdmin = isSuperAdmin
                });
            }

            // Aplicar ordenação
            users = sortBy switch
            {
                "id" => sortOrder == "desc"
                    ? users.OrderByDescending(u => u.Id).ToList()
                    : users.OrderBy(u => u.Id).ToList(),
                "email" => sortOrder == "desc"
                    ? users.OrderByDescending(u => u.Email).ToList()
                    : users.OrderBy(u => u.Email).ToList(),
                "username" => sortOrder == "desc"
                    ? users.OrderByDescending(u => u.UserName).ToList()
                    : users.OrderBy(u => u.UserName).ToList(),
                "createdAt" => sortOrder == "desc"
                    ? users.OrderByDescending(u => u.CreatedAt ?? DateTime.MinValue).ToList()
                    : users.OrderBy(u => u.CreatedAt ?? DateTime.MinValue).ToList(),
                _ => users.OrderBy(u => u.Email).ToList()
            };

            var model = new UsersViewModel
            {
                TotalAccountsCreated = totalAccounts,
                TotalCompradores = totalCompradores,
                TotalVendedores = totalVendedores,
                Users = users
            };

            return View("~/Views/Admin/ManageUsers.cshtml", model);
        }

        // GET: /Admin/Veiculos
        public async Task<IActionResult> Veiculos(string searchTerm, string sortBy, string sortOrder)
        {
            ViewData["AdminActive"] = "Veiculos";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;

            ViewBag.IdSortOrder = sortBy == "id" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.UsernameSortOrder = sortBy == "username" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.AnoSortOrder = sortBy == "ano" && sortOrder == "asc" ? "desc" : "asc";

            var total = await _context.Veiculos.CountAsync();

            var query = _context.Veiculos
                .Include(v => v.IdMarcaNavigation)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdVendedorNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(v =>
                    v.IdVeiculo.ToString().Contains(searchTerm) ||
                    (v.IdVendedorNavigation != null &&
                     v.IdVendedorNavigation.UserName != null &&
                     v.IdVendedorNavigation.UserName.ToLower().Contains(searchTerm)));
            }

            query = sortBy switch
            {
                "id" => sortOrder == "desc"
                    ? query.OrderByDescending(v => v.IdVeiculo)
                    : query.OrderBy(v => v.IdVeiculo),
                "username" => sortOrder == "desc"
                    ? query.OrderByDescending(v => v.IdVendedorNavigation.UserName)
                    : query.OrderBy(v => v.IdVendedorNavigation.UserName),
                "ano" => sortOrder == "desc"
                    ? query.OrderByDescending(v => v.Ano)
                    : query.OrderBy(v => v.Ano),
                _ => query.OrderByDescending(v => v.IdVeiculo)
            };

            var list = await query
                .Select(v => new AdminVehiclesViewModel.VehicleItem
                {
                    IdVeiculo = v.IdVeiculo,
                    Marca = v.IdMarcaNavigation != null ? v.IdMarcaNavigation.Nome : string.Empty,
                    Modelo = v.IdModeloNavigation != null ? v.IdModeloNavigation.Nome : string.Empty,
                    Ano = v.Ano,
                    Proprietario = v.IdVendedorNavigation != null ? v.IdVendedorNavigation.UserName ?? string.Empty : string.Empty,
                    Disponivel = v.Disponivel
                })
                .ToListAsync();

            var vm = new AdminVehiclesViewModel
            {
                TotalVeiculos = total,
                Vehicles = list
            };

            return View(vm);
        }

        // GET: /Admin/Anuncios
        public async Task<IActionResult> Anuncios(string searchTerm, string sortBy, string sortOrder)
        {
            ViewData["AdminActive"] = "Anuncios";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;

            ViewBag.IdSortOrder = sortBy == "id" && sortOrder == "asc" ? "desc" : "asc";
            ViewBag.UsernameSortOrder = sortBy == "username" && sortOrder == "asc" ? "desc" : "asc";

            var total = await _context.Anuncios
                .Where(a => a.Estado == "Ativo")
                .CountAsync();

            var query = _context.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdMarcaNavigation)
                .Include(a => a.IdVeiculoNavigation)
                    .ThenInclude(v => v.IdModeloNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(a =>
                    a.IdAnuncio.ToString().Contains(searchTerm) ||
                    (a.IdVendedorNavigation != null &&
                     a.IdVendedorNavigation.UserName != null &&
                     a.IdVendedorNavigation.UserName.ToLower().Contains(searchTerm)));
            }

            query = sortBy switch
            {
                "id" => sortOrder == "desc"
                    ? query.OrderByDescending(a => a.IdAnuncio)
                    : query.OrderBy(a => a.IdAnuncio),
                "username" => sortOrder == "desc"
                    ? query.OrderByDescending(a => a.IdVendedorNavigation.UserName)
                    : query.OrderBy(a => a.IdVendedorNavigation.UserName),
                _ => query.OrderByDescending(a => a.IdAnuncio)
            };

            var list = await query
                .Select(a => new AdminAnnouncementsViewModel.AnnouncementItem
                {
                    IdAnuncio = a.IdAnuncio,
                    Criador = a.IdVendedorNavigation != null ? a.IdVendedorNavigation.UserName ?? string.Empty : string.Empty,
                    Estado = a.Estado,
                    DataCriacao = a.DataCriacao,
                    Marca = a.IdVeiculoNavigation != null && a.IdVeiculoNavigation.IdMarcaNavigation != null
                        ? a.IdVeiculoNavigation.IdMarcaNavigation.Nome
                        : string.Empty,
                    Modelo = a.IdVeiculoNavigation != null && a.IdVeiculoNavigation.IdModeloNavigation != null
                        ? a.IdVeiculoNavigation.IdModeloNavigation.Nome
                        : string.Empty
                })
                .ToListAsync();

            string? topMarca = null;
            if (list.Any())
            {
                topMarca = list
                    .Where(a => !string.IsNullOrEmpty(a.Marca))
                    .GroupBy(a => a.Marca)
                    .Select(g => new { Marca = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ThenBy(x => x.Marca)
                    .FirstOrDefault()
                    ?.Marca;
            }

            var vm = new AdminAnnouncementsViewModel
            {
                TotalAnuncios = total,
                Announcements = list,
                MarcaMaisPresente = topMarca
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVeiculo(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Anuncios)
                .FirstOrDefaultAsync(v => v.IdVeiculo == id);

            if (veiculo == null)
            {
                TempData["AdminError"] = "Viatura não encontrada.";
                return RedirectToAction(nameof(Veiculos));
            }

            veiculo.Disponivel = false;
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Viatura marcada como indisponível.";
            return RedirectToAction(nameof(Veiculos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == id);
            if (anuncio == null)
            {
                TempData["AdminError"] = "Anúncio não encontrado.";
                return RedirectToAction(nameof(Anuncios));
            }

            anuncio.Estado = "Inativo";
            anuncio.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Anúncio marcado como inativo.";
            return RedirectToAction(nameof(Anuncios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdminUser(string userName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["AdminError"] = "Todos os campos são obrigatórios.";
                return RedirectToAction("Index");
            }

            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                TempData["AdminError"] = "A password e a confirmação não coincidem.";
                return RedirectToAction("Index");
            }

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                TempData["AdminError"] = "Já existe um utilizador com esse email.";
                return RedirectToAction("Index");
            }

            var user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                TempData["AdminError"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Index");
            }

            // Claim CreatedAt, tal como no registo normal
            var createdAtClaim = new Claim("CreatedAt", DateTime.UtcNow.ToString("o"));
            await _userManager.AddClaimAsync(user, createdAtClaim);

            await _userManager.AddToRoleAsync(user, "Admin");

            // --------- Registo no Histórico de Ações (Criação conta admin) ---------
            // Garante que existe o TipoAcao para criação de conta admin
            var tipoAcaoCriacaoAdmin = await _context.TipoAcaos
                .FirstOrDefaultAsync(t => t.Nome == "Criação de conta admin");

            if (tipoAcaoCriacaoAdmin == null)
            {
                tipoAcaoCriacaoAdmin = new TipoAcao
                {
                    Nome = "Criação de conta admin"
                };

                _context.TipoAcaos.Add(tipoAcaoCriacaoAdmin);
                await _context.SaveChangesAsync();
            }

            // Garante que existe a Acao associada
            var acaoCriacaoContaAdmin = await _context.Acaos
                .FirstOrDefaultAsync(a => a.Nome == "Criação de conta admin");

            if (acaoCriacaoContaAdmin == null)
            {
                acaoCriacaoContaAdmin = new Acao
                {
                    IdTipoAcao = tipoAcaoCriacaoAdmin.IdTipoAcao,
                    Nome = "Criação de conta admin",
                    Descricao = "Registo de nova conta de administrador",
                    TipoAlvo = "Utilizador"
                };

                _context.Acaos.Add(acaoCriacaoContaAdmin);
                await _context.SaveChangesAsync();
            }

            // Registo no histórico
            var historicoAdmin = new HistoricoAco
            {
                IdAcao = acaoCriacaoContaAdmin.IdAcao,
                IdUtilizador = user.Id,
                IdAlvo = null,
                TipoAlvo = "Utilizador",
                Razao = "Conta criada com role Admin",
                DataHora = DateTime.UtcNow
            };

            _context.HistoricoAcoes.Add(historicoAdmin);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Conta admin criada com sucesso.";

            return RedirectToAction("Index");
        }

        // GET: /Admin/Acoes
        [HttpGet]
        public async Task<IActionResult> HistoricoAcoes(string? search)
        {
            ViewData["Title"] = "Histórico de Ações";
            ViewData["AdminActive"] = "HistoricoAcoes";

            var query = _context.HistoricoAcoes
                .Include(h => h.IdAcaoNavigation)
                .AsQueryable();

            var historico = await query
                .Join(_userManager.Users,
                    h => h.IdUtilizador,
                    u => u.Id,
                    (h, u) => new
                    {
                        Historico = h,
                        User = u
                    })
                .Where(x =>
                    string.IsNullOrWhiteSpace(search)
                    || x.User.UserName!.Contains(search)
                    || x.User.Id.Contains(search))
                .OrderByDescending(x => x.Historico.DataHora)
                .Select(x => new HistoricoAdminViewModel
                {
                    IdHistorico = x.Historico.IdHistorico,
                    IdUtilizador = x.User.Id,
                    UserName = x.User.UserName!,
                    NomeAcao = x.Historico.IdAcaoNavigation.Nome,
                    TipoAcao = x.Historico.IdAcaoNavigation.IdTipoAcaoNavigation.Nome,
                    IdAlvo = x.Historico.IdAlvo,
                    TipoAlvo = x.Historico.TipoAlvo,
                    Razao = x.Historico.Razao,
                    DataHora = x.Historico.DataHora
                })
                .ToListAsync();

            ViewBag.Search = search;

            return View(historico);
        }
    }
}
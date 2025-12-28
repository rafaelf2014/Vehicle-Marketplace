using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CliCarProject.Data;
using CliCarProject.Models;
using CliCarProject.Models.Classes;
using Microsoft.EntityFrameworkCore;

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
            var visitsGrouped = await _context.SitePageViews
                .Where(s => s.VisitTime >= startDate)
                .GroupBy(s => s.VisitTime.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            var labels = new List<string>(days);
            var counts = new List<int>(days);
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                labels.Add(date.ToString("yyyy-MM-dd"));
                var found = visitsGrouped.FirstOrDefault(v => v.Date == date);
                counts.Add(found?.Count ?? 0);
            }

            var model = new AdminDashboardViewModel
            {
                TotalSiteVisits = (int)totalVisits,
                TotalAccountsCreated = (int)totalAccounts,
                TotalUsers = (int)totalAccounts,
                TotalAnuncios = (int)totalAnuncios,
                TotalVeiculos = (int)totalVeiculos,
                WeeklySiteVisits = (int)weeklyVisits,
                VisitLabels = labels,
                VisitCounts = counts
            };

            return View(model);
        }

        // GET: /Admin/UserManage
        public async Task<IActionResult> UserManage()
        {
            ViewData["AdminActive"] = "Users";

            var totalAccounts = await _context.Users.CountAsync();
            var totalCompradores = await _context.Compradors.CountAsync();
            var totalVendedores = await _context.Vendedors.CountAsync();

            var compradoresIds = await _context.Compradors.Select(c => c.IdUtilizador).ToListAsync();
            var vendedoresIds = await _context.Vendedors.Select(v => v.IdUtilizador).ToListAsync();

            var compradoresSet = compradoresIds.ToHashSet();
            var vendedoresSet = vendedoresIds.ToHashSet();

            var identityUsers = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

            var users = new List<UsersViewModel.UserListItem>(identityUsers.Count);

            foreach (var iu in identityUsers)
            {
                var claims = await _userManager.GetClaimsAsync(iu);
                var createdAtClaim = claims.FirstOrDefault(c => c.Type == "CreatedAt");
                DateTime? createdAt = null;
                if (createdAtClaim != null && DateTime.TryParse(createdAtClaim.Value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                {
                    createdAt = dt;
                }

                users.Add(new UsersViewModel.UserListItem
                {
                    Id = iu.Id,
                    Email = iu.Email ?? string.Empty,
                    UserName = iu.UserName ?? string.Empty,
                    Role = (compradoresSet.Contains(iu.Id) ? "Comprador" : "")
                           + (compradoresSet.Contains(iu.Id) && vendedoresSet.Contains(iu.Id) ? ", " : "")
                           + (vendedoresSet.Contains(iu.Id) ? "Vendedor" : ""),
                    CreatedAt = createdAt
                });
            }

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
        // Página que mostra o número total de viaturas e a lista (Id, Marca, Modelo, Ano, Proprietário)
        public async Task<IActionResult> Veiculos()
        {
            ViewData["AdminActive"] = "Veiculos";

            var total = await _context.Veiculos.CountAsync();

            var list = await _context.Veiculos
                .Include(v => v.IdMarcaNavigation)
                .Include(v => v.IdModeloNavigation)
                .Include(v => v.IdVendedorNavigation)
                .OrderByDescending(v => v.IdVeiculo)
                .Select(v => new AdminVehiclesViewModel.VehicleItem
                {
                    IdVeiculo = v.IdVeiculo,
                    Marca = v.IdMarcaNavigation != null ? v.IdMarcaNavigation.Nome : string.Empty,
                    Modelo = v.IdModeloNavigation != null ? v.IdModeloNavigation.Nome : string.Empty,
                    Ano = v.Ano,
                    Proprietario = v.IdVendedorNavigation != null ? v.IdVendedorNavigation.UserName ?? string.Empty : string.Empty
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
        // Página que mostra o número total de anúncios e a lista (Id, Criador, Estado, DataCriação)
        public async Task<IActionResult> Anuncios()
        {
            ViewData["AdminActive"] = "Anuncios";

            var total = await _context.Anuncios.CountAsync();

            var list = await _context.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .OrderByDescending(a => a.IdAnuncio)
                .Select(a => new AdminAnnouncementsViewModel.AnnouncementItem
                {
                    IdAnuncio = a.IdAnuncio,
                    Criador = a.IdVendedorNavigation != null ? a.IdVendedorNavigation.UserName ?? string.Empty : string.Empty,
                    Estado = a.Estado,
                    DataCriacao = a.DataCriacao
                })
                .ToListAsync();

            var vm = new AdminAnnouncementsViewModel
            {
                TotalAnuncios = total,
                Announcements = list
            };

            return View(vm);
        }
    }
}
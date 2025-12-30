using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Data;
using CliCarProject.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CliCarProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Mantém a view existente UserProfile (não alterada)
        public IActionResult UserProfile()
        {
            return View();
        }

        // Nova action que devolve a view de gestão/listagem de utilizadores
        public async Task<IActionResult> Manage()
        {
            ViewData["AdminActive"] = "Users";

            var totalAccounts = await _context.Users.CountAsync();
            var totalCompradores = await _context.Compradors.CountAsync();
            var totalVendedores = await _context.Vendedors.CountAsync();

            var compradoresIds = await _context.Compradors.Select(c => c.IdUtilizador).ToListAsync();
            var vendedoresIds = await _context.Vendedors.Select(v => v.IdUtilizador).ToListAsync();

            var compradoresSet = compradoresIds.ToHashSet();
            var vendedoresSet = vendedoresIds.ToHashSet();

            // Obtemos a lista completa de utilizadores via UserManager (permite depois ler claims)
            var identityUsers = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

            var users = new List<UsersViewModel.UserListItem>(identityUsers.Count);

            foreach (var iu in identityUsers)
            {
                // Ler claims do utilizador (procuramos CreatedAt)
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

            // Renderiza a view que fica dentro de Views/Admin para ficar integrada no painel
            return View("~/Views/Admin/ManageUsers.cshtml", model);
        }
    }
}

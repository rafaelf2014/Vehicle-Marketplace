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

namespace CliCarProject.Controllers
{
    [Authorize] // perfil é para qualquer utilizador autenticado
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /User/UserProfile
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Foto de perfil via claim (se usares esse esquema)
            var claims = await _userManager.GetClaimsAsync(user);
            var imgClaim = claims.FirstOrDefault(c => c.Type == "ProfileImagePath");
            if (imgClaim != null)
            {
                ViewBag.ProfileImagePath = imgClaim.Value;
            }

            // Carregar vendedor/comprador
            Vendedor? vendedor = null;
            Comprador? comprador = null;

            if (User.IsInRole("Vendedor"))
            {
                vendedor = await _context.Vendedors
                    .FirstOrDefaultAsync(v => v.IdUtilizador == user.Id);
            }

            if (User.IsInRole("Comprador"))
            {
                comprador = await _context.Compradors
                    .FirstOrDefaultAsync(c => c.IdUtilizador == user.Id);
            }

            ViewBag.Vendedor = vendedor;
            ViewBag.Comprador = comprador;

            return View();
        }

        // POST: /User/UploadProfileImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile fotoPerfil, [FromServices] IWebHostEnvironment env)
        {
            if (fotoPerfil == null || fotoPerfil.Length == 0)
            {
                return RedirectToAction(nameof(UserProfile));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(UserProfile));
            }

            var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", "perfil", user.Id);
            Directory.CreateDirectory(uploadsRoot);

            var fileName = Path.GetRandomFileName() + Path.GetExtension(fotoPerfil.FileName);
            var fullPath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await fotoPerfil.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/perfil/{user.Id}/{fileName}";

            // guardar caminho numa claim
            var claims = await _userManager.GetClaimsAsync(user);
            var old = claims.FirstOrDefault(c => c.Type == "ProfileImagePath");
            if (old != null)
            {
                await _userManager.RemoveClaimAsync(user, old);
            }
            await _userManager.AddClaimAsync(
                user,
                new System.Security.Claims.Claim("ProfileImagePath", relativePath));

            return RedirectToAction(nameof(UserProfile));
        }

        // POST: /User/UpdateAccountData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccountData(
            string userName,
            string email,
            string contacto,
            string codigoPostal,
            string? nif,
            string? morada)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ProfileError"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(UserProfile));
            }

            // validações simples de números
            if (!string.IsNullOrWhiteSpace(contacto) && !contacto.All(char.IsDigit))
            {
                TempData["ProfileError"] = "O contacto deve conter apenas dígitos.";
                return RedirectToAction(nameof(UserProfile));
            }

            if (!string.IsNullOrWhiteSpace(codigoPostal) && !codigoPostal.All(char.IsDigit))
            {
                TempData["ProfileError"] = "O código postal deve conter apenas dígitos (sem hífen).";
                return RedirectToAction(nameof(UserProfile));
            }

            if (!string.IsNullOrWhiteSpace(nif) && !nif.All(char.IsDigit))
            {
                TempData["ProfileError"] = "O NIF deve conter apenas dígitos.";
                return RedirectToAction(nameof(UserProfile));
            }

            // Atualizar username/email do AspNetUsers
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(userName) && !string.Equals(user.UserName, userName, StringComparison.Ordinal))
            {
                user.UserName = userName;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(email) && !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = email;
                user.NormalizedEmail = _userManager.NormalizeEmail(email);
                hasChanges = true;
            }

            if (hasChanges)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    TempData["ProfileError"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(UserProfile));
                }
            }

            // Atualizar Vendedor / Comprador
            if (User.IsInRole("Vendedor"))
            {
                var vendedor = await _context.Vendedors
                    .FirstOrDefaultAsync(v => v.IdUtilizador == user.Id);

                if (vendedor == null)
                {
                    vendedor = new Vendedor { IdUtilizador = user.Id };
                    _context.Vendedors.Add(vendedor);
                }

                vendedor.Contacto = string.IsNullOrWhiteSpace(contacto) ? null : contacto;
                vendedor.CodigoPostal = string.IsNullOrWhiteSpace(codigoPostal) ? null : codigoPostal;
                vendedor.Nif = string.IsNullOrWhiteSpace(nif) ? null : nif;
            }

            if (User.IsInRole("Comprador"))
            {
                var comprador = await _context.Compradors
                    .FirstOrDefaultAsync(c => c.IdUtilizador == user.Id);

                if (comprador == null)
                {
                    comprador = new Comprador { IdUtilizador = user.Id };
                    _context.Compradors.Add(comprador);
                }

                comprador.Contacto = string.IsNullOrWhiteSpace(contacto) ? null : contacto;
                comprador.CodigoPostal = string.IsNullOrWhiteSpace(codigoPostal) ? null : codigoPostal;
                comprador.Morada = string.IsNullOrWhiteSpace(morada) ? comprador.Morada : morada;
            }

            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] = "Dados da conta atualizados com sucesso.";
            return RedirectToAction(nameof(UserProfile));
        }

        // POST: /User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ProfileError"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(UserProfile));
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["ProfileError"] = "A nova palavra‑passe e a confirmação não coincidem.";
                return RedirectToAction(nameof(UserProfile));
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                TempData["ProfileError"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(UserProfile));
            }

            TempData["ProfileSuccess"] = "Palavra‑passe alterada com sucesso.";
            return RedirectToAction(nameof(UserProfile));
        }

        // GET: /User/AdminViewProfile/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminViewProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // claims para foto
            var claims = await _userManager.GetClaimsAsync(user);
            var imgClaim = claims.FirstOrDefault(c => c.Type == "ProfileImagePath");
            ViewBag.ProfileImagePath = imgClaim?.Value;

            // roles
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            // comprador/vendedor do utilizador em questão
            var vendedor = await _context.Vendedors
                .FirstOrDefaultAsync(v => v.IdUtilizador == user.Id);
            var comprador = await _context.Compradors
                .FirstOrDefaultAsync(c => c.IdUtilizador == user.Id);

            ViewBag.Vendedor = vendedor;
            ViewBag.Comprador = comprador;

            return View("AdminUserProfile", user); // usa view específica
        }

        // Se ainda precisares de ações de admin neste controller, coloca-as aqui com [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUser(string userId, string reason)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(reason))
            {
                TempData["ProfileError"] = "É necessário indicar o utilizador e a razão.";
                return RedirectToAction("UserManage", "Admin");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ProfileError"] = "Utilizador não encontrado.";
                return RedirectToAction("UserManage", "Admin");
            }

            if (string.Equals(user.Email, "superadmin@clicar.local", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ProfileError"] = "Não é permitido bloquear a conta superadmin.";
                return RedirectToAction("UserManage", "Admin");
            }

            // registar bloqueio (cria/atualiza)
            var existing = await _context.UserBlocks.FirstOrDefaultAsync(b => b.UserId == userId);
            if (existing == null)
            {
                existing = new UserBlock
                {
                    UserId = userId,
                    Reason = reason.Trim(),
                    BlockedAt = DateTime.UtcNow
                };
                _context.UserBlocks.Add(existing);
            }
            else
            {
                existing.Reason = reason.Trim();
                existing.BlockedAt = DateTime.UtcNow;
            }

            // Lockout opcional
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);

            // ------- Histórico: Bloqueio de utilizador -------
            // TipoAcao = "Gestão de bloqueios"
            var tipoAcaoBloqueio = await _context.TipoAcaos
                .FirstOrDefaultAsync(t => t.Nome == "Gestão de bloqueios");
            if (tipoAcaoBloqueio == null)
            {
                tipoAcaoBloqueio = new TipoAcao
                {
                    Nome = "Gestão de bloqueios"
                };
                _context.TipoAcaos.Add(tipoAcaoBloqueio);
                await _context.SaveChangesAsync();
            }

            // Acao = "Bloqueio de utilizador"
            var acaoBloqueio = await _context.Acaos
                .FirstOrDefaultAsync(a => a.Nome == "Bloqueio de utilizador");
            if (acaoBloqueio == null)
            {
                acaoBloqueio = new Acao
                {
                    IdTipoAcao = tipoAcaoBloqueio.IdTipoAcao,
                    Nome = "Bloqueio de utilizador",
                    Descricao = "Utilizador bloqueado por administrador",
                    TipoAlvo = "Utilizador"
                };
                _context.Acaos.Add(acaoBloqueio);
                await _context.SaveChangesAsync();
            }

            var adminId = _userManager.GetUserId(User);

            var historicoBloqueio = new HistoricoAco
            {
                IdAcao = acaoBloqueio.IdAcao,
                IdUtilizador = adminId!,
                IdAlvo = null, // alvo identificado via Razao
                TipoAlvo = "Utilizador",
                Razao = $"Utilizador alvo: {user.UserName}. Razão: {reason.Trim()}",
                DataHora = DateTime.UtcNow
            };

            _context.HistoricoAcoes.Add(historicoBloqueio);
            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] = "Utilizador bloqueado com sucesso.";
            return RedirectToAction("UserManage", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["ProfileError"] = "Utilizador inválido.";
                return RedirectToAction("UserManage", "Admin");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ProfileError"] = "Utilizador não encontrado.";
                return RedirectToAction("UserManage", "Admin");
            }

            var block = await _context.UserBlocks.FirstOrDefaultAsync(b => b.UserId == userId);
            var reason = block?.Reason ?? "Motivo não especificado.";

            if (block != null)
            {
                _context.UserBlocks.Remove(block);
                await _context.SaveChangesAsync();
            }

            if (user.LockoutEnabled || user.LockoutEnd != null)
            {
                user.LockoutEnd = null;
                user.LockoutEnabled = false;
                await _userManager.UpdateAsync(user);
            }

            // ------- Histórico: Desbloqueio de utilizador -------
            var tipoAcaoBloqueio = await _context.TipoAcaos
                .FirstOrDefaultAsync(t => t.Nome == "Gestão de bloqueios");
            if (tipoAcaoBloqueio == null)
            {
                tipoAcaoBloqueio = new TipoAcao
                {
                    Nome = "Gestão de bloqueios"
                };
                _context.TipoAcaos.Add(tipoAcaoBloqueio);
                await _context.SaveChangesAsync();
            }

            var acaoDesbloqueio = await _context.Acaos
                .FirstOrDefaultAsync(a => a.Nome == "Desbloqueio de utilizador");
            if (acaoDesbloqueio == null)
            {
                acaoDesbloqueio = new Acao
                {
                    IdTipoAcao = tipoAcaoBloqueio.IdTipoAcao,
                    Nome = "Desbloqueio de utilizador",
                    Descricao = "Utilizador desbloqueado por administrador",
                    TipoAlvo = "Utilizador"
                };
                _context.Acaos.Add(acaoDesbloqueio);
                await _context.SaveChangesAsync();
            }

            var adminId = _userManager.GetUserId(User);

            var historicoDesbloqueio = new HistoricoAco
            {
                IdAcao = acaoDesbloqueio.IdAcao,
                IdUtilizador = adminId!,
                IdAlvo = null,
                TipoAlvo = "Utilizador",
                Razao = $"Utilizador alvo: {user.UserName}. Desbloqueado. Última razão conhecida: {reason}",
                DataHora = DateTime.UtcNow
            };

            _context.HistoricoAcoes.Add(historicoDesbloqueio);
            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] = "Utilizador desbloqueado com sucesso.";
            return RedirectToAction("UserManage", "Admin");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Blocked(string? userId = null)
        {
            string? reason = null;

            // 1) se vier userId do querystring, usa-o
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var blockById = await _context.UserBlocks
                    .FirstOrDefaultAsync(b => b.UserId == userId);
                reason = blockById?.Reason;
            }
            // 2) se estiver autenticado, tenta novamente com o utilizador da sessão
            else if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var block = await _context.UserBlocks
                        .FirstOrDefaultAsync(b => b.UserId == user.Id);
                    reason = block?.Reason;
                }
            }

            ViewBag.Reason = reason;
            return View();
        }
    }
}

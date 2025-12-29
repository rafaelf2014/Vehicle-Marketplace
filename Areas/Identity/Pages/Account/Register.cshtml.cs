// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using CliCarProject.Models;
using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims; // adicionado

namespace CliCarProject.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CliCarProject.Data.ApplicationDbContext _dbContext;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            Data.ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Required(ErrorMessage = "Campo Obrigatório!!")]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!!")]
            [Display(Name = "Phone Number")]
            [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Formato inválido (ex.: 912867424)")]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage ="Campo Obrigatório!!")]
            [Display(Name = "Código Postal")]
            [RegularExpression(@"^[0-9]{4}-[0-9]{3}$", ErrorMessage = "Formato inválido (ex.: 1234-567)")]
            public string CodigoPostal { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!!")]
            [Display(Name = "NIF")]
            [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Formato inválido (ex.: 123456789)")]
            public string NIF { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!!")]
            [Display(Name = "Tipo de Vendedor")]
            public string TypeSeller { get; set; }

            // Tipo de Conta (role) selecionada na view
            [Required]
            [Display(Name = "Tipo de Conta")]
            public string Role { get; set; }

            [Required(ErrorMessage = "Campo Obligatório!!")]
            [Display(Name = "Morada")]
            [StringLength(100, ErrorMessage = "A {0} deve ter no máximo {1} caracteres.")]
            public string Morada { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!!")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input != null)
            {
                var role = Input.Role ?? string.Empty;
                if (string.Equals(role, "Comprador", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.Remove("Input.NIF");
                    ModelState.Remove("Input.TypeSeller");
                }
                else if (string.Equals(role, "Vendedor", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.Remove("Input.Morada");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            var user = CreateUser();

            await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Adiciona claim com a data de criação do utilizador (opção escolhida)
            var createdAtClaim = new Claim("CreatedAt", DateTime.UtcNow.ToString("o"));
            await _userManager.AddClaimAsync(user, createdAtClaim);

            _logger.LogInformation("Usuário criado com sucesso: {UserName}", Input.UserName);
            _logger.LogInformation("Usuário criado com sucesso: {Id}", user.Id);


            if (!string.IsNullOrEmpty(Input.Role))
            {
                var roleManager = HttpContext?.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
                if (roleManager != null && !await roleManager.RoleExistsAsync(Input.Role))
                {
                    var createRoleResult = await roleManager.CreateAsync(new IdentityRole(Input.Role));
                    if (!createRoleResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user); // rollback
                        foreach (var error in createRoleResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return Page();
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, Input.Role);
                if (!addRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user); // rollback
                    foreach (var error in addRoleResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
 
            }

            // ----------------- Criar Contas Vendedor e Comprador --------------------

            var userId = user.Id;

            if (Input.Role == "Vendedor")
            {
                var vendedor = new Vendedor
                {
                    IdUtilizador = userId,
                    Contacto = Input.PhoneNumber,
                    CodigoPostal = Input.CodigoPostal,
                    Nif = Input.NIF,
                    Tipo = Input.TypeSeller
                };
                
                _dbContext.Vendedors.Add(vendedor);
                await _dbContext.SaveChangesAsync();
                SuccessMessage = "Conta criada com sucesso";
            }

            if (Input.Role == "Comprador")
            {
                var comprador = new Comprador
                {
                    IdUtilizador = userId,
                    Contacto = Input.PhoneNumber,
                    CodigoPostal = Input.CodigoPostal,
                    Morada = Input.Morada
                };
                _dbContext.Compradors.Add(comprador);
                await _dbContext.SaveChangesAsync();
                SuccessMessage = "Conta criada com sucesso";

            }

            // ----------------- EMAIL DE CONFIRMAÇÃO --------------------
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirme o seu email",
                $"Por favor confirme a sua conta clicando aqui: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirmar Email</a>");

            // Redireciona para página automática de confirmação
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

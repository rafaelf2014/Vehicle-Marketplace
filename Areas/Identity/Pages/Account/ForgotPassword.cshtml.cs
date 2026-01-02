// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CliCarProject.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code , email = user.Email },
                    protocol: Request.Scheme);

                string mensagemHtml = $@"
            <div style='font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
                
                <div style='background-color: #d90429; padding: 40px 20px; text-align: center;'>
                    <h1 style='color: #ffffff; margin: 0; font-size: 28px; text-transform: uppercase; letter-spacing: 2px;'>CliCar</h1>
                    <p style='color: #ffffff; opacity: 0.9; margin: 5px 0 0 0;'>Recuperação de Acesso</p>
                </div>

                <div style='padding: 40px; background-color: #ffffff; color: #2b2d42;'>
                    <p style='font-size: 18px; margin-top: 0;'>Olá!</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Recebemos um pedido para redefinir a palavra-passe da tua conta no <strong>CliCar Stand Virtual</strong>.</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Se foste tu que solicitaste esta alteração, clica no botão abaixo para escolher uma nova senha:</p>
                    
                    <div style='text-align: center; margin: 45px 0;'>
                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                           style='background-color: #d90429; color: #ffffff; padding: 18px 35px; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px; display: inline-block;'>
                           REDEFINIR PALAVRA-PASSE
                        </a>
                    </div>
                    
                    <p style='font-size: 14px; color: #8d99ae; text-align: center;'>
                        Se não solicitaste esta redefinição, podes ignorar este e-mail. A tua senha atual permanecerá segura.
                    </p>
                </div>

                <div style='background-color: #edf2f4; padding: 20px; text-align: center; font-size: 12px; color: #2b2d42; border-top: 1px solid #d8e2dc;'>
                    <strong>CliCar Project</strong><br>
                    O teu Stand Virtual de confiança &copy; {DateTime.Now.Year}
                </div>
            </div>";

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Recuperar Palavra-passe - CliCar",
                    mensagemHtml);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}

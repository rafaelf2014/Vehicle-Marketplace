using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CliCarProject.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Configuramos o cliente SMTP com os dados do Mailtrap que enviaste
            using (var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525))
            {
                client.Credentials = new NetworkCredential("11893ed2b2091d", "58e8b46b9fc726");
                client.EnableSsl = true;

                // Criamos a mensagem
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("suporte@clicarproject.com", "CliCar Support"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true // Importante para o link de confirmação funcionar
                };

                mailMessage.To.Add(email);

                // Enviamos o email de forma assíncrona
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
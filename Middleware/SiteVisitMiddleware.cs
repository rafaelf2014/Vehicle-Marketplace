using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CliCarProject.Data;
using CliCarProject.Models.Classes;

namespace CliCarProject.Middleware
{
    /// <summary>
    /// Regista uma entrada em SitePageView para cada pedido GET "válido".
    /// Filtra pedidos de assets (css/js/img/fonts) e AJAX para evitar ruído.
    /// Usa session para evitar contagens repetidas dentro de uma janela de tempo.
    /// </summary>
    public class SiteVisitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SiteVisitMiddleware> _logger;
        private static readonly string[] _excludedPrefixes = new[]
        {
            "/lib", "/css", "/js", "/images", "/img", "/uploads", "/favicon", "/_framework", "/_blazor", "/swagger", "/health"
        };
        private static readonly string[] _excludedExtensions = new[]
        {
            ".css", ".js", ".map", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".eot", ".webmanifest"
        };

        // janela de deduplicação em minutos (não contar várias visitas para o mesmo path dentro deste intervalo)
        private const double DeduplicationWindowMinutes = 5;

        public SiteVisitMiddleware(RequestDelegate next, ILogger<SiteVisitMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider services)
        {
            try
            {
                // Só contar GETs (evita POST/PUT/etc)
                if (string.Equals(context.Request.Method, HttpMethods.Get, StringComparison.OrdinalIgnoreCase))
                {
                    var path = context.Request.Path.Value ?? "/";

                    // Filtrar prefixes e extensões óbvias (assets)
                    if (!_excludedPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    {
                        var hasExt = _excludedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

                        if (!hasExt)
                        {
                            // Filtrar requests que não são páginas HTML (ex.: fetch/ajax, imagens, APIs)
                            var accept = context.Request.Headers["Accept"].ToString();
                            var xRequestedWith = context.Request.Headers["X-Requested-With"].ToString();

                            // Se o cliente não aceitar HTML ou for um XHR, ignorar
                            if (!accept.Contains("text/html", StringComparison.OrdinalIgnoreCase)) { await _next(context); return; }
                            if (!string.IsNullOrEmpty(xRequestedWith) && xRequestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase)) { await _next(context); return; }

                            // Verificar sessão para deduplicação (garantir que app.UseSession() é chamado antes deste middleware)
                            bool shouldCount = true;
                            try
                            {
                                var session = context.Session;
                                if (session != null)
                                {
                                    var key = $"LastVisit:{path}";
                                    var last = session.GetString(key);
                                    if (!string.IsNullOrEmpty(last) && DateTime.TryParse(last, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastDt))
                                    {
                                        if ((DateTime.UtcNow - lastDt).TotalMinutes < DeduplicationWindowMinutes)
                                        {
                                            shouldCount = false;
                                        }
                                    }

                                    if (shouldCount)
                                    {
                                        session.SetString(key, DateTime.UtcNow.ToString("o"));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // se sessão não estiver disponível ou falhar, não bloqueia a contagem — apenas loga
                                _logger.LogDebug(ex, "Não foi possível usar session para dedupe (pode ser que UseSession não esteja registado antes do middleware).");
                            }

                            if (shouldCount)
                            {
                                // Criar scope para obter ApplicationDbContext (scoped)
                                using var scope = services.CreateScope();
                                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                                var ip = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                                var ua = context.Request.Headers["User-Agent"].ToString() ?? string.Empty;

                                var view = new SitePageView
                                {
                                    Path = path,

                                    VisitTime = DateTime.UtcNow
                                };

                                try
                                {
                                    db.SitePageViews.Add(view);
                                    await db.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                    // Não impedir request por erro de gravação; registar para diagnóstico
                                    _logger.LogWarning(ex, "Falha ao gravar SitePageView para path {Path}", path);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no SiteVisitMiddleware");
                // continuar pipeline mesmo em erro
            }

            await _next(context);
        }
    }

    public static class SiteVisitMiddlewareExtensions
    {
        public static IApplicationBuilder UseSiteVisitMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SiteVisitMiddleware>();
        }
    }
}
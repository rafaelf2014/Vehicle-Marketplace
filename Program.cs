using CliCarProject.Data;
using CliCarProject.Models.Classes;
using CliCarProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar encoding UTF-8 para toda a aplicação
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(60); // commands
       
        }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Adicionar o serviço de Sessões
builder.Services.AddSession(options =>
{
    // Define o tempo que a sessão pode ficar inativa antes de ser limpa
    // 30 minutos é um valor padrão comum
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Assegura que o cookie de sessão é essencial para o funcionamento do site
    options.Cookie.IsEssential = true;

    // Nome do cookie que armazena o ID da sessão (opcional, mas bom para clareza)
    options.Cookie.Name = "CliCar.Session";
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar codificação UTF-8 para Web
builder.Services.AddWebEncoders(options =>
{
    options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(
        System.Text.Unicode.UnicodeRanges.All
    );
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IFileService, FileService>(); // Registar o FileService para injeção de dependência
builder.Services.AddScoped<IVeiculoService, VeiculoService>(); // Registar o VeiculoService para injeção de dependência

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Seed roles and superadmin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles.SeedAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Valores padrão; preferir configurar em appsettings.json ou variáveis de ambiente
    var adminEmail = config["AdminUser:Email"] ?? "superadmin@clicar.local";
    var adminUserName = config["AdminUser:UserName"] ?? "superadmin";
    var adminPassword = config["AdminUser:Password"] ?? "Admin@123"; // altere em produção

    // Garante que o role "Admin" existe (SeedRoles já cria) e cria um superadmin se não existir
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var adminUser = new IdentityUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            // Usa "Admin" (consistente com SeedRoles). Roles são normalizadas internamente.
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            // opcional: logar erros
            foreach (var err in createResult.Errors) Console.WriteLine(err.Description);
        }
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT: garantir que Session está registado antes do middleware de visitas
app.UseSession();

// Registar o middleware de visitas imediatamente após o Session
app.UseSiteVisitMiddleware();

app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var db = context.RequestServices.GetRequiredService<ApplicationDbContext>();

        var user = await userManager.GetUserAsync(context.User);
        if (user != null)
        {
            var blocked = await db.UserBlocks.AnyAsync(b => b.UserId == user.Id);
            if (blocked)
            {
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
                await signInManager.SignOutAsync();

                context.Response.Redirect("/User/Blocked");
                return;
            }
        }
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

await SeedData.InitializeAsync(app.Services);

app.Run();

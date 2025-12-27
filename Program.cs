using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Data;
using CliCarProject.Services;
using CliCarProject.Models.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.CommandTimeout(60) // em segundos
    ));

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
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IFileService, FileService>(); // Registar o FileService para injeção de dependência
builder.Services.AddScoped<IVeiculoService, VeiculoService>(); // Registar o VeiculoService para injeção de dependência

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
app.UseSession();

app.UseAuthentication();
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

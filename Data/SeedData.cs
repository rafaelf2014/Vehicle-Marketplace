using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CliCarProject.Data;
using CliCarProject.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace CliCarProject.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Aplicar migrações pendentes (opcional)
            await context.Database.MigrateAsync();

            if (!context.Classes.Any())
            {
                context.Classes.AddRange(
                    new Classe { Nome = "SUV" },
                    new Classe { Nome = "Compacto" },
                    new Classe { Nome = "Sedan" },
                    new Classe { Nome = "Hatchback" },
                    new Classe { Nome = "Carrinha" },
                    new Classe { Nome = "Desportivo" },
                    new Classe { Nome = "Pickup" },
                    new Classe { Nome = "Elétrico" }
                );
            }

            if (!context.Combustivels.Any())
            {
                context.Combustivels.AddRange(
                    new Combustivel { Tipo = "Gasolina" },
                    new Combustivel { Tipo = "Diesel" },
                    new Combustivel { Tipo = "Elétrico" },
                    new Combustivel { Tipo = "Híbrido" },
                    new Combustivel { Tipo = "GPL" }
                );
            }

            if (!context.Marcas.Any())
            {
                context.Marcas.AddRange(
                    new Marca { Nome = "Honda" },
                    new Marca { Nome = "Toyota" },
                    new Marca { Nome = "BMW" },
                    new Marca { Nome = "Mercedes" },
                    new Marca { Nome = "Volkswagen" }
                );
            }

            await context.SaveChangesAsync();

            if (!context.Modelos.Any())
            {
                // Obter marcas já persistidas
                var honda = await context.Marcas.FirstOrDefaultAsync(m => m.Nome == "Honda");
                var toyota = await context.Marcas.FirstOrDefaultAsync(m => m.Nome == "Toyota");
                var bmw = await context.Marcas.FirstOrDefaultAsync(m => m.Nome == "BMW");
                var mercedes = await context.Marcas.FirstOrDefaultAsync(m => m.Nome == "Mercedes");
                var vw = await context.Marcas.FirstOrDefaultAsync(m => m.Nome == "Volkswagen");

                if (honda != null) context.Modelos.AddRange(
                    new Modelo { Nome = "Civic", IdMarca = honda.IdMarca },
                    new Modelo { Nome = "Accord", IdMarca = honda.IdMarca }
                );
                if (toyota != null) context.Modelos.AddRange(
                    new Modelo { Nome = "Corolla", IdMarca = toyota.IdMarca },
                    new Modelo { Nome = "Yaris", IdMarca = toyota.IdMarca }
                );
                if (bmw != null) context.Modelos.AddRange(
                    new Modelo { Nome = "320d", IdMarca = bmw.IdMarca },
                    new Modelo { Nome = "X5", IdMarca = bmw.IdMarca }
                );
                if (mercedes != null) context.Modelos.AddRange(
                    new Modelo { Nome = "A-Class", IdMarca = mercedes.IdMarca },
                    new Modelo { Nome = "C-Class", IdMarca = mercedes.IdMarca }
                );
                if (vw != null) context.Modelos.AddRange(
                    new Modelo { Nome = "Golf", IdMarca = vw.IdMarca },
                    new Modelo { Nome = "Polo", IdMarca = vw.IdMarca }
                );
            }

            if (!context.Localizacaos.Any())
            {
                context.Localizacaos.AddRange(
                    new Localizacao { Distrito = "Aveiro" },
                    new Localizacao { Distrito = "Beja" },
                    new Localizacao { Distrito = "Braga" },
                    new Localizacao { Distrito = "Bragança" },
                    new Localizacao { Distrito = "Castelo Branco" },
                    new Localizacao { Distrito = "Coimbra" },
                    new Localizacao { Distrito = "Évora" },
                    new Localizacao { Distrito = "Faro" },
                    new Localizacao { Distrito = "Guarda" },
                    new Localizacao { Distrito = "Leiria" },
                    new Localizacao { Distrito = "Lisboa" },
                    new Localizacao { Distrito = "Portalegre" },
                    new Localizacao { Distrito = "Porto" },
                    new Localizacao { Distrito = "Santarém" },
                    new Localizacao { Distrito = "Setúbal" },
                    new Localizacao { Distrito = "Viana do Castelo" },
                    new Localizacao { Distrito = "Vila Real" },
                    new Localizacao { Distrito = "Viseu" },
                    new Localizacao { Distrito = "Madeira" },
                    new Localizacao { Distrito = "Açores" }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
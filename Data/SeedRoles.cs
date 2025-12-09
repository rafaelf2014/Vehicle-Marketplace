using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Data
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {

            string[] roles = { "Admin", "Vendedor", "Comprador" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }
    }
}

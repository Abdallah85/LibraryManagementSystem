using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace Persistence.Identity
{
    public static class IdentitySeeder
    {
        private static readonly string[] RoleNames = { "Administrator", "Librarian", "Staff","Member" };


        public static async Task InitIdentityAsync(WebApplication app)
        {
            await SeedRolesAsync(app);
        }

        public static async Task SeedRolesAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            foreach (var roleName in RoleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }
    }
}

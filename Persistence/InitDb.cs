using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;


namespace Persistence
{
    public static class InitDb
    {
        public static async Task InitDbAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if(context.Database.GetPendingMigrations().Any()) {
                await context.Database.MigrateAsync();
            }
        }
    }
}

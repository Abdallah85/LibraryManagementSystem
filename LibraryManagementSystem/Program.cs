
using LibraryManagementSystemApi;
using LibraryManagementSystemApi.Middelwares;
using Microsoft.OpenApi;
using Persistence;
using Persistence.Identity;
using Presentation;
using Services.Extension;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Add infrastructure services
            builder.Services.AddWebApplicationServices(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApplicationServices(builder.Configuration);


            var app = builder.Build();

            await IdentitySeeder.InitIdentityAsync(app);

            app.UseSwagger();    
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.Run();
        }
    }
}

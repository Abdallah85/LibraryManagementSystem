
using LibraryManagementSystemApi.Middelwares;
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

            // Add services to the container.

            builder.Services.AddControllers()
            .AddApplicationPart(typeof(AuthController).Assembly);
            builder.Services.AddSwaggerGen();

            // Add infrastructure services
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);


            var app = builder.Build();

            await IdentitySeeder.InitIdentityAsync(app);

            app.UseSwagger();    
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();


            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.Run();
        }
    }
}

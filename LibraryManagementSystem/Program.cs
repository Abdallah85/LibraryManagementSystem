
using LibraryManagementSystemApi.Middelwares;
using Persistence;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            // Add infrastructure services
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();
            
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

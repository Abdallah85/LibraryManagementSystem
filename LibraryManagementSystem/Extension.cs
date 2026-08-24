using Microsoft.OpenApi;
using Presentation;

namespace LibraryManagementSystemApi
{
    public static class Extension
    {
        public static IServiceCollection AddWebApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(AuthController).Assembly);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Library Management System Api",
                        Version = "v1"
                    });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] =
                            new List<string>()
                    });
            });

            return services;
        }
    }
}
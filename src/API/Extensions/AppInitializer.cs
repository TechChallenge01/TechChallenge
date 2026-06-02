using API.EndPoints;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace API.Extensions
{
    public static class AppInitializer
    {
        public static async Task<WebApplication> UseScalarDocumentation(this WebApplication app)
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Tech Challenge")
                .AddPreferredSecuritySchemes("Bearer")
                .AddHttpAuthentication("Bearer", options =>
                {
                    options.Token = "teste";
                });
            });

            return app;
        }

        public static async Task<WebApplication> InitializeDb(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        db.Database.Migrate();
                        break;
                    }
                    catch
                    {
                        retries--;
                        Thread.Sleep(3000);
                    }
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                //await DbSeeds.Seed(context);
            }

            return app;
        }
        public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
        {
            IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }

            return app;
        }
    }
}

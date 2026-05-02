<<<<<<< HEAD
﻿using Application.Auth.Services;
using Application.EmailServices;
=======
﻿using Application.EmailServices;
using Application.PasswordsServices;
>>>>>>> 87031395c4d2393cb8f3fe7c2cdeffbe6d3dba83
using Application.UnitOfWork;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Domain.Aggregates.OrdemServicoAggregates.Repositories;
using Domain.Entities.Repositories;
using Infra.Context;
using Infra.PasswordServices;
using Infra.Repositories;
using Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            services.AddScoped<IEmailService, EmailService>();
<<<<<<< HEAD
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
=======
            services.AddScoped<IPasswordHasher, PasswordHasher>();
>>>>>>> 87031395c4d2393cb8f3fe7c2cdeffbe6d3dba83

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                );
            }));
            return services;
        }
    }
}

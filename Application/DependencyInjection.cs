using Application.Clientes.Services;
using Application.Veiculos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IVeiculoService, VeiculoService>();

            return services;
        }
    }
}

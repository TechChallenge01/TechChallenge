using Application.Clientes.Services;
using Application.Estoques.Services;
using Application.OrdemServicos.Services;
using Application.Pecas.Services;
using Application.Servicos.Services;
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
            services.AddScoped<IOrdemServicoService, OrdemServicoService>();
            services.AddScoped<IPecaService, PecaService>();
            services.AddScoped<IServicoService, ServicoService>();
            services.AddScoped<IEstoqueService, EstoqueService>();

            return services;
        }
    }
}

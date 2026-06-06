using Application.Auth.Services;
using Application.Estoques.Services;
using Application.Insumos.Services;
using Application.OrdemServicos.Services;
using Application.Pecas.Services;
using Application.Servicos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IInsumoService, InsumoService>();
            services.AddScoped<IOrdemServicoService, OrdemServicoService>();
            services.AddScoped<IPecaService, PecaService>();
            services.AddScoped<IServicoService, ServicoService>();
            services.AddScoped<IEstoqueService, EstoqueService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}

using Application.Gateways.Veiculos;
using Domain.Entities;

namespace Application.UseCases.Veiculos
{
    public class GetByIdUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;

        private GetByIdUseCase(VeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public static GetByIdUseCase Create(VeiculoGateway veiculoGateway)
        {
            return new GetByIdUseCase(veiculoGateway);
        }

        public async Task<Veiculo>? Run(Guid id, CancellationToken ct)
        {
            try
            {
                var veiculo = await _veiculoGateway.GetById(id, ct);
                return veiculo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}

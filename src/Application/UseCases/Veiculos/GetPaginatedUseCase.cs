using Application.Gateways.Veiculos;
using Domain.Entities;

namespace Application.UseCases.Veiculos
{
    public class GetPaginatedUseCase
    {
        private readonly VeiculoGateway _veiculoGateway;

        private GetPaginatedUseCase(VeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public static GetPaginatedUseCase Create(VeiculoGateway veiculoGateway)
        {
            return new GetPaginatedUseCase(veiculoGateway);
        }

        public async Task<(List<Veiculo> veiculos, int total)> Run(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var response = await _veiculoGateway.GetPaginated(page, pageSize, ct);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}

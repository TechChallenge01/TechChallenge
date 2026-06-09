using Application.Gateways.Clientes;
using Domain.Aggregates.ClienteAggregates;

namespace Application.UseCases.Clientes
{
    public class GetPaginatedUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private GetPaginatedUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static GetPaginatedUseCase Create(ClienteGateway clienteGateway)
        {
            return new GetPaginatedUseCase(clienteGateway);
        }

        public async Task<(List<Cliente> Clientes, int total)> Run(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var response = await _clienteGateway.GetPaginated(page, pageSize, ct);

                return response;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

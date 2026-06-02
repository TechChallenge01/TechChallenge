using Application.Gateways.Clientes;
using Domain.Aggregates.ClienteAggregates;

namespace Application.UseCases.Clientes
{
    public class GetByIdUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private GetByIdUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }
        public static GetByIdUseCase Create(ClienteGateway clienteGateway)
        {
            return new GetByIdUseCase(clienteGateway);
        }

        public async Task<Cliente?> Run(Guid id, CancellationToken ct)
        {
            try
            {
                var response = await _clienteGateway.GetById(id, ct);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}

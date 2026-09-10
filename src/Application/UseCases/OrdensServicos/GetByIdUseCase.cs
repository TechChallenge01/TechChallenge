using Application.Gateways.OrdemServicos;
using Domain.Aggregates.OrdemServicoAggregates;

namespace Application.UseCases.OrdensServicos
{
    public class GetByIdUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;

        private GetByIdUseCase(OrdemServicoGateway ordemServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
        }
        public static GetByIdUseCase Create(OrdemServicoGateway ordemServicoGateway)
        {
            return new GetByIdUseCase(ordemServicoGateway);
        }

        public async Task<OrdemServico?> Run(Guid id, CancellationToken ct, Guid? clienteIdSolicitante = null)
        {
            try
            {
                var response = await _ordemServicoGateway.GetById(id, ct);

                if (response is not null && clienteIdSolicitante.HasValue && response.ClienteId != clienteIdSolicitante.Value)
                    throw new UnauthorizedAccessException("Você não tem permissão para acessar esta ordem de serviço.");

                return response;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
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

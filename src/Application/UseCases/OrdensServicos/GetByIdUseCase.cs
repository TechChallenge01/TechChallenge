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

        public async Task<OrdemServico?> Run(Guid id, CancellationToken ct)
        {
            try
            {
                var response = await _ordemServicoGateway.GetById(id, ct);

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

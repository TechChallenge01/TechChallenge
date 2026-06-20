using Application.Gateways.OrdemServicos;
using Domain.Aggregates.OrdemServicoAggregates;

namespace Application.UseCases.OrdensServicos
{
    public class GetPaginatedUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;

        private GetPaginatedUseCase(OrdemServicoGateway ordemServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
        }

        public static GetPaginatedUseCase Create(OrdemServicoGateway ordemServicoGateway)
        {
            return new GetPaginatedUseCase(ordemServicoGateway);
        }

        public async Task<(List<OrdemServico> ordensServicos, int total)> Run(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var response = await _ordemServicoGateway.GetPaginated(page, pageSize, ct);

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

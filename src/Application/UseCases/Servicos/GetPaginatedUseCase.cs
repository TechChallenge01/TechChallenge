using Application.Gateways.Servicos;
using Domain.Entities;

namespace Application.UseCases.Servicos
{
    public class GetPaginatedUseCase
    {
        private readonly ServicoGateway _servicoGateway;

        private GetPaginatedUseCase(ServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public static GetPaginatedUseCase Create(ServicoGateway servicoGateway)
        {
            return new GetPaginatedUseCase(servicoGateway);
        }

        public async Task<(List<Servico> servicos, int total)> Run(int page, int pageSize, CancellationToken ct)
        {
            try
            {
                var servicos = await _servicoGateway.GetPaginated(page, pageSize, ct);

                return servicos;
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

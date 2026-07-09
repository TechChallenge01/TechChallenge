using Application.Gateways.OrdemServicos;

namespace Application.UseCases.OrdensServicos
{
    public class RegistrarEntregaUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;

        private RegistrarEntregaUseCase(OrdemServicoGateway ordemServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
        }

        public static RegistrarEntregaUseCase Create(OrdemServicoGateway ordemServicoGateway)
        {
            return new RegistrarEntregaUseCase(ordemServicoGateway);
        }

        public async Task Run(Guid id, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var ordemServico = await _ordemServicoGateway.GetById(id, ct);

                if (ordemServico is null) throw new KeyNotFoundException("ordem de serviço informada não existe!");

                ordemServico.Entregar();
                ordemServico.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _ordemServicoGateway.Update(ordemServico, ct);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

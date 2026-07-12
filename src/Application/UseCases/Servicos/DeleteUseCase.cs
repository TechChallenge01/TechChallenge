using Application.Gateways.Servicos;

namespace Application.UseCases.Servicos
{
    public class DeleteUseCase
    {
        private readonly ServicoGateway _servicoGateway;

        private DeleteUseCase(ServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public static DeleteUseCase Create(ServicoGateway servicoGateway)
        {
            return new DeleteUseCase(servicoGateway);
        }

        public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
        {
            try
            {
                var servico = await _servicoGateway.GetById(id, ct);

                if (servico is null)
                    throw new KeyNotFoundException("Serviço não encontrado!");

                servico.Inativar();
                servico.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _servicoGateway.Update(servico, ct);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

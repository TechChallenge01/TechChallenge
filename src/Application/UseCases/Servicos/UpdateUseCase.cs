using Application.Gateways.Servicos;
using Shared.DTOs.Servicos.Requests;

namespace Application.UseCases.Servicos
{
    public class UpdateUseCase
    {
        private readonly ServicoGateway _servicoGateway;

        private UpdateUseCase(ServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public static UpdateUseCase Create(ServicoGateway servicoGateway)
        {
            return new UpdateUseCase(servicoGateway);
        }

        public async Task Run(Guid idUsuario, Guid id, ServicoRequestDTO servicoRequest, CancellationToken ct)
        {
            try
            {
                var servico = await _servicoGateway.GetById(id, ct);

                if (servico is null)
                    throw new KeyNotFoundException("Serviço não encontrado!");

                servico.AlterarNome(servicoRequest.Nome);
                servico.AlterarDescricao(servicoRequest.Descricao);
                servico.AlterarPrecoVenda(servicoRequest.PrecoVenda);
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

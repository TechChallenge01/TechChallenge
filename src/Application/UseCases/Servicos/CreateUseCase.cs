using Application.Gateways.Servicos;
using Domain.Entities;
using Shared.DTOs.Servicos.Requests;

namespace Application.UseCases.Servicos
{
    public class CreateUseCase
    {
        private readonly ServicoGateway _servicoGateway;

        private CreateUseCase(ServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public static CreateUseCase Create(ServicoGateway servicoGateway)
        {
            return new CreateUseCase(servicoGateway);
        }

        public async Task<Guid> Run(ServicoRequestDTO servico, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var servicoEntity = new Servico(servico.Nome, servico.Descricao, servico.PrecoVenda, idUsuario, DateTime.UtcNow);

                await _servicoGateway.Create(servicoEntity, ct);

                return servicoEntity.Id;
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

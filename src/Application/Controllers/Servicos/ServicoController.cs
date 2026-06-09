using Application.Gateways.Servicos;
using Application.Interfaces;
using Application.Presenters.Servicos;
using Application.UseCases.Servicos;
using Microsoft.Extensions.Caching.Memory;
using Shared.DTOs;
using Shared.DTOs.Servicos.Output;
using Shared.Result;

namespace Application.Controllers.Servicos
{
    public class ServicoController
    {
        private readonly IServicoDataSource _dataSource;

        public ServicoController(IServicoDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICommandResult<PagedResultDTO<ServicoOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct) 
        {
            var presenter = new ServicoPresenter("Pesquisa de serviços retornada com sucesso!");
            try
            {
                var servicoGateway = ServicoGateway.Create(_dataSource);
                var useCase = GetPaginatedUseCase.Create(servicoGateway);
                var servicos = await useCase.Run(page, pageSize, ct);

                return presenter.TransformPaged(servicos.servicos, page, servicos.total);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<PagedResultDTO<ServicoOutputDTO>>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<PagedResultDTO<ServicoOutputDTO>>(ex.Message);
            }
        }
        public async Task<ICommandResult<ServicoOutputDTO>> GetById(Guid id, CancellationToken ct)
        {
            var presenter = new ServicoPresenter("Serviço retornado com sucesso");
            try
            {
                var servicoGateway = ServicoGateway.Create(_dataSource);
                var useCase = GetByIdUseCase.Create(servicoGateway);
                var servico = await useCase.Run(id, ct);

                if(servico is null)
                    presenter.NoContent();

                return presenter.TransformObject(servico);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<ServicoOutputDTO>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<ServicoOutputDTO>(ex.Message);
            }
        }
    }
}

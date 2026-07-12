using Application.Gateways.Servicos;
using Application.Interfaces;
using Application.Presenters.Servicos;
using Application.UseCases.Servicos;
using Shared.DTOs;
using Shared.DTOs.Servicos.Output;
using Shared.DTOs.Servicos.Requests;
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
                    return presenter.NotFound<ServicoOutputDTO>("Serviço não encontrado");

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

        public async Task<ICommandResult<Guid>> Create(ServicoRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            var presenter = new ServicoPresenter("Servico criado com sucesso!");
            try
            {
                var servicoGateway = ServicoGateway.Create(_dataSource);
                var useCase = CreateUseCase.Create(servicoGateway);
                var response = await useCase.Run(request, idUsuario, ct);

                return presenter.Created<Guid>(response);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }

        }

        public async Task<ICommandResult> Update(Guid id, ServicoRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            var presenter = new ServicoPresenter("Serviço atualizado com sucesso!");
            try
            {
                var servicoGateway = ServicoGateway.Create(_dataSource);
                var useCase = UpdateUseCase.Create(servicoGateway);
                await useCase.Run(idUsuario, id, request, ct);

                return presenter.NoContent();
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError(ex.Message);
            }
        }

        public async Task<ICommandResult> Delete(Guid idUsuario, Guid id, CancellationToken ct)
        {
            var presenter = new ServicoPresenter("Serviço deletado com sucesso!");
            try
            {
                var servicoGateway = ServicoGateway.Create(_dataSource);
                var useCase = DeleteUseCase.Create(servicoGateway);
                await useCase.Run(idUsuario, id, ct);

                return presenter.NoContent();
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError(ex.Message);
            }
        }
    }
}

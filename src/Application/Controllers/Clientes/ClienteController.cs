using Application.Gateways.Clientes;
using Application.Interfaces;
using Application.Presenters.Clientes;
using Application.UseCases.Clientes;
using Shared.DTOs;
using Shared.DTOs.Cliente.Output;
using Shared.DTOs.Cliente.Request;
using Shared.Result;

namespace Application.Controllers.Clientes
{
    public class ClienteController
    {
        private readonly IClienteDataSource _dataSource;

        public ClienteController(IClienteDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICommandResult<PagedResultDTO<ClienteOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var presenter = new ClientePresenter("Pesquisa de clientes retornada com sucesso!");
            try
            {
                var clienteGateway = ClienteGateway.Create(_dataSource);
                var useCase = GetPaginatedUseCase.Create(clienteGateway);

                var clientes = await useCase.Run(page, pageSize, ct);

                return presenter.TransformPaged(clientes.Clientes, page, clientes.total);
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest<PagedResultDTO<ClienteOutputDTO>>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<PagedResultDTO<ClienteOutputDTO>>(ex.Message);
            }
        }
        public async Task<ICommandResult<ClienteOutputDTO>> GetById(Guid Id, CancellationToken ct)
        {
            var presenter = new ClientePresenter("Pesquisa de cliente retornada com sucesso!");
            try
            {
                var clienteGateway = ClienteGateway.Create(_dataSource);
                var useCase = GetByIdUseCase.Create(clienteGateway);

                var cliente = await useCase.Run(Id, ct);

                if(cliente is null)
                    return presenter.NotFound<ClienteOutputDTO>("Cliente não encontrado!");

                return presenter.TransformObject(cliente);
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest<ClienteOutputDTO>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<ClienteOutputDTO>(ex.Message);
            }
        }
        public async Task<ICommandResult<Guid>> Create(ClienteRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            var presenter = new ClientePresenter("Cliente criado com sucesso!");
            try
            {
                var clienteGateway = ClienteGateway.Create(_dataSource);
                var useCase = CreateUseCase.Create(clienteGateway);
                var idCliente = await useCase.Run(request, idUsuario, ct);
                return presenter.Created(idCliente);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }
            ;
        }
    }
}

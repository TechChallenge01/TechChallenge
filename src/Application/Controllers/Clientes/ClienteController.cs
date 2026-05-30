using Application.Gateways.Clientes;
using Application.Interfaces;
using Application.Presenters.Clientes;
using Application.UseCases.Clientes;
using Domain.Aggregates.ClienteAggregates;
using Shared.DTOs;
using Shared.DTOs.Cliente.Output;
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


    }
}

using Application.Gateways.Clientes;
using Application.Interfaces;
using Application.Presenters.Clientes;
using Application.UseCases.Clientes;
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
                var clienteGatewat = ClienteGateway.Create(_dataSource);
                var useCase = GetPaginatedUseCase.Create(clienteGatewat);

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
    }
}

using Application.Gateways.Clientes;
using Application.Interfaces;
using Application.UseCases.Clientes;
using Shared.DTOs;
using Shared.DTOs.Cliente.Output;
using Shared.Result;
using System;
using System.Collections.Generic;
using System.Text;

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
            var clienteGatewat = ClienteGateway.Create(_dataSource);
            var useCase = GetPaginatedUseCase.Create(clienteGatewat);

            var clientes = await useCase.Run(page, pageSize, ct);


        }
    }
}

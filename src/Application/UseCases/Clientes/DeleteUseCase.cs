using Application.Gateways.Clientes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Clientes
{
    public class DeleteUseCase
    {
        private readonly ClienteGateway _clienteGateway;

        private DeleteUseCase(ClienteGateway clienteGateway)
        {
            _clienteGateway = clienteGateway;
        }

        public static DeleteUseCase Create(ClienteGateway clienteGateway)
        {
            return new DeleteUseCase(clienteGateway);
        }

        public async Task Run(Guid id, CancellationToken ct)
        {
            var cliente = await _clienteGateway.GetById(id, ct);

            if (cliente is null)
                throw new ArgumentException("Cliente não encontrado!");

            await _clienteGateway.Delete(id, ct);
        }
    }
}

using Application.Interfaces;
using Domain.Aggregates.ClienteAggregates;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Gateways.Clientes
{
    public class ClienteGateway
    {
        private readonly IClienteDataSource _dataSource;

        private ClienteGateway(IClienteDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public static ClienteGateway Create(IClienteDataSource dataSource)
        {
            return new ClienteGateway(dataSource);
        }

        public async Task<(List<Cliente> Clientes, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var response = await _dataSource.GetPaginated(page, pageSize, ct);

            var clientes = response.clientes.Select(c => new Cliente(c.Id, c.Nome, new Cpf(c.Cpf), new Cnpj(c.Cnpj), new Email(c.Email), 
                                                   new Telefone(c.Telefone.DDD,c.Telefone.DDI, c.Telefone.Numero), 
                                                   new Endereco(c.Endereco.Logradouro, c.Endereco.Numero, c.Endereco.Complemento, c.Endereco.Bairro, c.Endereco.Cep, c.Endereco.Cidade, c.Endereco.Uf), 
                                                   c.Veiculos.Select(v => new Veiculo(v)).ToList())).ToList();

            return (clientes, response.total);

        }
    }
}

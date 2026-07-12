using Application.Interfaces;
using Domain.Aggregates.ClienteAggregates;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.DTOs.Clientes.Input;
using Shared.DTOs.Clientes.Shared;

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

            var clientes = response.clientes.Select(c => new Cliente(c.Id, c.Nome, c.Cpf == null ? null : new Cpf(c.Cpf), c.Cnpj == null ? null : new Cnpj(c.Cnpj), new Email(c.Email), 
                                                   new Telefone(c.Telefone.DDD,c.Telefone.DDI, c.Telefone.Numero), 
                                                   new Endereco(c.Endereco.Logradouro, c.Endereco.Numero, c.Endereco.Complemento, c.Endereco.Bairro, c.Endereco.Cidade, c.Endereco.Uf, c.Endereco.Cep), 
                                                   c.Veiculos.Select(v => new Veiculo(v)).ToList())).ToList();

            return (clientes, response.total);
        }
        public async Task<Cliente?> GetById(Guid id, CancellationToken ct)
        {
            var response = await _dataSource.GetById(id, ct);

            if (response == null)
                return null;

            var cliente = new Cliente(response.Id, response.Nome, response.Cpf == null ? null : new Cpf(response.Cpf), response.Cnpj == null ? null : new Cnpj(response.Cnpj), new Email(response.Email), 
                                      new Telefone(response.Telefone.DDD, response.Telefone.DDI, response.Telefone.Numero), 
                                      new Endereco(response.Endereco.Logradouro, response.Endereco.Numero, response.Endereco.Complemento, response.Endereco.Bairro, response.Endereco.Cidade, response.Endereco.Uf, response.Endereco.Cep), 
                                      response.Veiculos.Select(v => new Veiculo(v)).ToList());

            return cliente;
        }
        public async Task<Cliente?> GetByCpf(Cpf cpf, CancellationToken ct)
        {
            var response = await _dataSource.GetByCpf(cpf.Valor, ct);

            if (response == null)
                return null;

            var cliente = new Cliente(response.Id, response.Nome, response.Cpf == null ? null : new Cpf(response.Cpf), response.Cnpj == null ? null : new Cnpj(response.Cnpj), new Email(response.Email), 
                                      new Telefone(response.Telefone.DDD, response.Telefone.DDI, response.Telefone.Numero),
                                      new Endereco(response.Endereco.Logradouro, response.Endereco.Numero, response.Endereco.Complemento, response.Endereco.Bairro, response.Endereco.Cidade, response.Endereco.Uf, response.Endereco.Cep),
                                      response.Veiculos.Select(v => new Veiculo(v)).ToList());

            return cliente;
        }
        public async Task<Cliente?> GetByCnpj(Cnpj cnpj, CancellationToken ct)
        {
            var response = await _dataSource.GetByCnpj(cnpj.Valor, ct);

            if (response == null)
                return null;

            var cliente = new Cliente(response.Id, response.Nome, response.Cpf == null ? null : new Cpf(response.Cpf), response.Cnpj == null ? null : new Cnpj(response.Cnpj), new Email(response.Email), 
                                      new Telefone(response.Telefone.DDD, response.Telefone.DDI, response.Telefone.Numero),
                                      new Endereco(response.Endereco.Logradouro, response.Endereco.Numero, response.Endereco.Complemento, response.Endereco.Bairro, response.Endereco.Cidade, response.Endereco.Uf, response.Endereco.Cep),
                                      response.Veiculos.Select(v => new Veiculo(v)).ToList());

            return cliente;
        }
        public async Task Create(Cliente cliente, CancellationToken ct)
        {
            var clienteDTO = new ClienteInputDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf?.Valor,
                Cnpj = cliente.Cnpj?.Valor,
                Email = cliente.Email.EnderecoEmail,
                Telefone = new TelefoneDTO
                {
                    DDD = cliente.Telefone.DDD,
                    DDI = cliente.Telefone.DDI,
                    Numero = cliente.Telefone.Numero
                },
                Endereco = new EnderecoDTO
                {
                    Logradouro = cliente.Endereco.Logradouro,
                    Numero = cliente.Endereco.Numero,
                    Complemento = cliente.Endereco.Complemento,
                    Bairro = cliente.Endereco.Bairro,
                    Cep = cliente.Endereco.Cep,
                    Cidade = cliente.Endereco.Cidade,
                    Uf = cliente.Endereco.Uf
                },
                Veiculos = cliente.Veiculos?.Select(v => v.Id).ToList(),
                DataCriacao = cliente.DataCriacao,
                IdUsuarioCriacao = cliente.IdUsuarioCriacao
            };

            await _dataSource.Create(clienteDTO, ct);
        }
        public async Task Update (Cliente cliente, CancellationToken ct)
        {
            var clienteDTO = new ClienteInputDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf?.Valor,
                Cnpj = cliente.Cnpj?.Valor,
                Email = cliente.Email.EnderecoEmail,
                Telefone = new TelefoneDTO
                {
                    DDD = cliente.Telefone.DDD,
                    DDI = cliente.Telefone.DDI,
                    Numero = cliente.Telefone.Numero
                },
                Endereco = new EnderecoDTO
                {
                    Logradouro = cliente.Endereco.Logradouro,
                    Numero = cliente.Endereco.Numero,
                    Complemento = cliente.Endereco.Complemento,
                    Bairro = cliente.Endereco.Bairro,
                    Cep = cliente.Endereco.Cep,
                    Cidade = cliente.Endereco.Cidade,
                    Uf = cliente.Endereco.Uf
                },
                Veiculos = cliente.Veiculos?.Select(v => v.Id).ToList(),
                DataAtualizacao = cliente.DataAtualizacao,
                Ativo = cliente.Ativo
            };

            await _dataSource.Update(clienteDTO, ct);
        }
    }
}

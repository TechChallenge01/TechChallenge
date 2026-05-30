using Shared.DTOs.Cliente.Shared;
using Domain.Aggregates.ClienteAggregates;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Cliente.Input;
using Application.Interfaces;

namespace Infra.DataSources
{
    public class ClienteDataSource : IClienteDataSource
    {
        private readonly AppDbContext _appDbContext;

        public ClienteDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ClienteInputDTO?> GetById(Guid id, CancellationToken ct)
        {
            var cliente = await _appDbContext.Clientes
                .Include(c => c.Veiculos.Where(v => v.Ativo))
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo, ct);
            
            if (cliente == null)
                return null;

            var clienteResponse = new ClienteInputDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf.Valor,
                Cnpj = cliente.Cnpj.Valor,
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
                Veiculos = cliente.Veiculos.Select(v => v.Id).ToList()
            };

            return clienteResponse;
        }

        public async Task<(List<ClienteInputDTO> clientes, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<Cliente> query = _appDbContext.Clientes.Where(c => c.Ativo);

            var clientes = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .Include(c => c.Veiculos.Where(v => v.Ativo))
                                      .ToListAsync(ct);

            var clientesResponse = clientes.Select(c => new ClienteInputDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Cpf = c.Cpf.Valor,
                Cnpj = c.Cnpj.Valor,
                Email = c.Email.EnderecoEmail,
                Telefone = new TelefoneDTO
                {
                    DDD = c.Telefone.DDD,
                    DDI = c.Telefone.DDI,
                    Numero = c.Telefone.Numero
                },
                Endereco = new EnderecoDTO
                {
                    Logradouro = c.Endereco.Logradouro,
                    Numero = c.Endereco.Numero,
                    Complemento = c.Endereco.Complemento,
                    Bairro = c.Endereco.Bairro,
                    Cep = c.Endereco.Cep,
                    Cidade = c.Endereco.Cidade,
                    Uf = c.Endereco.Uf
                },
                Veiculos = c.Veiculos.Select(v => v.Id).ToList()
            }).ToList();

            var total = await query.CountAsync(ct);

            return (clientesResponse, total);
        }
    }
}

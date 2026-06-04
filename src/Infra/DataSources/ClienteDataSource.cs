using Shared.DTOs.Cliente.Shared;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Cliente.Input;
using Application.Interfaces;
using Infra.DataModel;

namespace Infra.DataSources
{
    public class ClienteDataSource : IClienteDataSource
    {
        private readonly AppDbContext _appDbContext;

        public ClienteDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(ClienteInputDTO cliente, CancellationToken ct)
        {
            var clienteDbModel = new ClienteDbModel(cliente.Id, cliente.Nome, cliente.Cpf ?? null, cliente.Cnpj ?? null, cliente.Email, cliente.Telefone.DDD, cliente.Telefone.DDI, cliente.Telefone.Numero,
                cliente.Endereco.Logradouro, cliente.Endereco.Numero, cliente.Endereco.Complemento, cliente.Endereco.Bairro,
                cliente.Endereco.Cep, cliente.Endereco.Cidade, cliente.Endereco.Uf, cliente.IdUsuarioCriacao, cliente.DataCriacao, cliente.IdUsuarioAtualizacao, cliente.DataAtualizacao);

            await _appDbContext.Clientes.AddAsync(clienteDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<ClienteInputDTO?> GetByCnpj(string cnpj, CancellationToken ct)
        {
            return await _appDbContext.Clientes
                .Where(c => c.Cnpj == cnpj && c.Ativo)
                .Select(c => new ClienteInputDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cpf = c.Cpf,
                    Cnpj = c.Cnpj,
                    Email = c.Email,
                    Telefone = new TelefoneDTO
                    {
                        DDD = c.DDD,
                        DDI = c.DDI,
                        Numero = c.NumeroTelefone
                    },
                    Endereco = new EnderecoDTO
                    {
                        Logradouro = c.Logradouro,
                        Numero = c.Numero,
                        Complemento = c.Complemento,
                        Bairro = c.Bairro,
                        Cep = c.Cep,
                        Cidade = c.Cidade,
                        Uf = c.Uf
                    },
                    Veiculos = _appDbContext.Veiculos.Where(v => v.ClienteId == c.Id && v.Ativo).Select(v => v.Id).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public Task<ClienteInputDTO?> GetByCpf(string cpf, CancellationToken ct)
        {
            throw new NotImplementedException();
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
                Cpf = cliente.Cpf,
                Cnpj = cliente.Cnpj,
                Email = cliente.Email,
                Telefone = new TelefoneDTO
                {
                    DDD = cliente.DDD,
                    DDI = cliente.DDI,
                    Numero = cliente.Numero
                },
                Endereco = new EnderecoDTO
                {
                    Logradouro = cliente.Logradouro,
                    Numero = cliente.Numero,
                    Complemento = cliente.Complemento,
                    Bairro = cliente.Bairro,
                    Cep = cliente.Cep,
                    Cidade = cliente.Cidade,
                    Uf = cliente.Uf
                },
                Veiculos = cliente.Veiculos.Select(v => v.Id).ToList()
            };

            return clienteResponse;
        }

        public async Task<(List<ClienteInputDTO> clientes, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<ClienteDbModel> query = _appDbContext.Clientes.Where(c => c.Ativo);

            var clientes = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .Include(c => c.Veiculos.Where(v => v.Ativo))
                                      .ToListAsync(ct);

            var clientesResponse = clientes.Select(c => new ClienteInputDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Cpf = c.Cpf,
                Cnpj = c.Cnpj,
                Email = c.Email,
                Telefone = new TelefoneDTO
                {
                    DDD = c.DDD,
                    DDI = c.DDI,
                    Numero = c.Numero
                },
                Endereco = new EnderecoDTO
                {
                    Logradouro = c.Logradouro,
                    Numero = c.Numero,
                    Complemento = c.Complemento,
                    Bairro = c.Bairro,
                    Cep = c.Cep,
                    Cidade = c.Cidade,
                    Uf = c.Uf
                },
                Veiculos = c.Veiculos.Select(v => v.Id).ToList()
            }).ToList();

            var total = await query.CountAsync(ct);

            return (clientesResponse, total);
        }

        public async Task Update(ClienteInputDTO cliente, CancellationToken ct)
        {
            var dbModel = _appDbContext.Clientes.FirstOrDefault(c => c.Id == cliente.Id && c.Ativo);

            if (dbModel is null)
                throw new ArgumentNullException("Cliente não encontrado!");

            dbModel.Nome = cliente.Nome;
            dbModel.Email = cliente.Email;
            dbModel.DDD = cliente.Telefone.DDD;
            dbModel.DDI = cliente.Telefone.DDI;
            dbModel.Numero = cliente.Telefone.Numero;
            dbModel.Logradouro = cliente.Endereco.Logradouro;
            dbModel.Numero = cliente.Endereco.Numero;
            dbModel.Complemento = cliente.Endereco.Complemento;
            dbModel.Bairro = cliente.Endereco.Bairro;
            dbModel.Cep = cliente.Endereco.Cep;
            dbModel.Cidade = cliente.Endereco.Cidade;
            dbModel.Uf = cliente.Endereco.Uf;
            dbModel.Ativo = cliente.Ativo;
            dbModel.IdUsuarioAtualizacao = cliente.IdUsuarioAtualizacao;
            dbModel.DataAtualizacao = cliente.DataAtualizacao;

            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}

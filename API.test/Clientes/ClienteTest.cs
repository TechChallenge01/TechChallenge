using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Shared;
using Domain.Aggregates.ClienteAggregates;
using Domain.ValueObjects;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Clientes
{
    public class ClienteTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
    {
        const string ApiKey = "api/Cliente";
        private readonly HttpClient _client;
        private readonly ApiWebApplicationFactory _factory;

        public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        public ClienteTest(IntegrationTestFixture fixture)
        {
            _client = fixture.Client;
            _factory = fixture.App;
        }

        [Fact]
        public async Task Cliente_Get_GetPaginated_PartialContent()
        {
            //arrange

            //act
            var result = await _client.GetAsync(ApiKey);

            //assert
            Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Get_GetPaginated_Unauthorized()
        {
            //arrange
            var app = new ApiWebApplicationFactory();
            using var client = app.CreateClient();

            //act
            var result = await client.GetAsync(ApiKey);

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Post_Create_cpf_correto_Created()
        {
            //arrange
            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "72814249061",
                Cnpj = "",
                Email = "testecpf@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };

            //act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            //assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Post_Create_cnpj_correto_Created()
        {
            //arrange
            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "",
                Cnpj = "54635822000178",
                Email = "testecpnj@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };

            //act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            //assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Post_Create_Unautorized()
        {
            //arrange
            var app = new ApiWebApplicationFactory();            

            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "45073010094",
                Cnpj = "",
                Email = "teste@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };

            using var client = app.CreateClient();

            //act
            var result = await client.PostAsJsonAsync(ApiKey, cliente);

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Post_Create_CPF_invalido_BadRequest()
        {
            //arrange
            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "111111111111", //valida cpf
                Cnpj = "",
                Email = "teste@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };

            //act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Post_Create_cnpj_invalido_BadRequest()
        {
            //arrange
            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "", //valida cpf
                Cnpj = "111111111111111",
                Email = "teste@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };

            //act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_get_GetById_OK()
        {
            // arrange
            var app = new ApiWebApplicationFactory();

            Guid clienteId;

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cliente = new Cliente(
                    "Teste",
                    new Cpf("52998224725"),
                    Guid.NewGuid(),
                    new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "999999999"),
                    new Email("teste@email.com")
                );

                context.Clientes.Add(cliente);
                context.SaveChanges();

                clienteId = cliente.Id;
            }

            // act
            var result = await _client.GetAsync($"{ApiKey}/{clienteId}");

            // assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Put_Update_NoContent()
        {
            // arrange
            var app = new ApiWebApplicationFactory();

            Guid clienteId;

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cliente = new Cliente(
                    "Teste",
                    new Cpf("52998224725"),
                    Guid.NewGuid(),
                    new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "999999999"),
                    new Email("teste@email.com")
                );

                context.Clientes.Add(cliente);
                context.SaveChanges();

                clienteId = cliente.Id;
            }
            var clienteDTO = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "52998224725", 
                Cnpj = "",
                Email = "teste@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro test",
                    Cep = "04349000",
                    Cidade = "são paulo",
                    Logradouro = "Rua 1",
                    Complemento = "",
                    Numero = "5",
                    Uf = "SP"
                },
                Telefone = new TelefoneDTO
                {
                    DDD = "11",
                    DDI = "55",
                    Numero = "959972016"
                }
            };


            // act
            var result = await _client.PutAsJsonAsync($"{ApiKey}/{clienteId}", clienteDTO);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
        [Fact]
        public async Task Cliente_Delete_Delete_NoContent()
        {
            // arrange
            var app = new ApiWebApplicationFactory();

            Guid clienteId;

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cliente = new Cliente(
                    "Teste",
                    new Cpf("52998224725"),
                    Guid.NewGuid(),
                    new Endereco("Rua A", "123", "", "Centro", "São Paulo", "SP", "01310100"),
                    new Telefone("11", "55", "999999999"),
                    new Email("teste@email.com")
                );

                context.Clientes.Add(cliente);
                context.SaveChanges();

                clienteId = cliente.Id;
            }

            // act
            var result = await _client.DeleteAsync($"{ApiKey}/{clienteId}");

            // assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}

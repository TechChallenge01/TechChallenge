using Bogus;
using Bogus.Extensions.Brazil;
using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Clientes.Request;
using Shared.DTOs.Clientes.Shared;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Clientes
{
    public class ClienteTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
    {
        const string ApiKey = "api/clientes";
        private readonly HttpClient _client;
        private readonly ApiWebApplicationFactory _factory;
        private readonly IntegrationTestFixture _fixture;

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
            _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public ClienteTest(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
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
            using var client = _factory.CreateClient();

            //act
            var result = await client.GetAsync(ApiKey);

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Post_Create_cpf_correto_Created()
        {
            //arrange
            var faker = new Faker("pt_BR");

            var cpf = faker.Person.Cpf(includeFormatSymbols: false); 
            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = cpf,
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
            var faker = new Faker("pt_BR");
            var cnpj = faker.Company.Cnpj(includeFormatSymbols: false);

            var cliente = new ClienteRequestDTO
            {
                Nome = "Test test",
                Cpf = "",
                Cnpj = cnpj,
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

            using var client = _factory.CreateClient();

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
            Guid clienteId;

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var admin = context.Usuarios.First();

                var clienteDbModel = new ClienteDbModel(
                    Guid.NewGuid(), "Teste", "52998224725", null, "teste@email.com",
                    "11", "55", "999999999", "Rua A", "123", null, "Centro", "01310100", "São Paulo", "SP",
                    admin.Id, DateTime.UtcNow, null, null
                );

                context.Clientes.Add(clienteDbModel);
                await context.SaveChangesAsync();

                clienteId = clienteDbModel.Id;
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
            Guid clienteId;

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var admin = context.Usuarios.First();

                var clienteDbModel = new ClienteDbModel(
                    Guid.NewGuid(), "Teste", "52998224725", null, "teste@email.com",
                    "11", "55", "999999999", "Rua A", "123", null, "Centro", "01310100", "São Paulo", "SP",
                    admin.Id, DateTime.UtcNow, null, null
                );

                context.Clientes.Add(clienteDbModel);
                await context.SaveChangesAsync();

                clienteId = clienteDbModel.Id;
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
            Guid clienteId;

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var admin = context.Usuarios.First();

                var clienteDbModel = new ClienteDbModel(
                    Guid.NewGuid(), "Teste", "52998224725", null, "teste@email.com",
                    "11", "55", "999999999", "Rua A", "123", null, "Centro", "01310100", "São Paulo", "SP",
                    admin.Id, DateTime.UtcNow, null, null
                );

                context.Clientes.Add(clienteDbModel);
                await context.SaveChangesAsync();

                clienteId = clienteDbModel.Id;
            }

            // act
            var result = await _client.DeleteAsync($"{ApiKey}/{clienteId}");

            // assert
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Post_Create_CpfDuplicado_Conflict()
        {
            // arrange
            var cpf = "79171883029";
            var clienteOriginal = new ClienteRequestDTO
            {
                Nome = "Primeiro Cadastro",
                Cpf = cpf,
                Cnpj = "", 
                Email = "sucesso@email.com",
                Endereco = new EnderecoDTO
                {
                    Bairro = "Bairro",
                    Cep = "04349000",
                    Cidade = "SP",
                    Logradouro = "Rua",
                    Numero = "1",
                    Uf = "SP",
                    Complemento = "SN" 
                },
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" }
            };

            // Act 1
            var resp1 = await _client.PostAsJsonAsync(ApiKey, clienteOriginal);

            // Act 2
            var clienteDuplicado = clienteOriginal with { Email = "outro@email.com" };
            var result = await _client.PostAsJsonAsync(ApiKey, clienteDuplicado);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Get_GetById_Inexistente_NotFound()
        {
            // act
            var result = await _client.GetAsync($"{ApiKey}/{Guid.NewGuid()}");

            // assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Delete_Inexistente_NotFound()
        {
            // act
            var result = await _client.DeleteAsync($"{ApiKey}/{Guid.NewGuid()}");

            // assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Post_Create_AmbosCpfCnpjPreenchidos_BadRequest()
        {
            // arrange
            var faker = new Faker("pt_BR");
            var cliente = new ClienteRequestDTO
            {
                Nome = "Erro Teste",
                Cpf = faker.Person.Cpf(includeFormatSymbols: false),
                Cnpj = faker.Company.Cnpj(includeFormatSymbols: false), 
                Email = "erro@email.com",
                Endereco = new EnderecoDTO { Bairro = "Bairro", Cep = "04349000", Cidade = "SP", Logradouro = "Rua", Numero = "1", Uf = "SP" },
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" }
            };

            // act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Post_Create_SemCpfESemCnpj_BadRequest()
        {
            // arrange
            var cliente = new ClienteRequestDTO
            {
                Nome = "Erro Teste",
                Cpf = "", 
                Cnpj = "",
                Email = "erro@email.com",
                Endereco = new EnderecoDTO { Bairro = "Bairro", Cep = "04349000", Cidade = "SP", Logradouro = "Rua", Numero = "1", Uf = "SP" },
                Telefone = new TelefoneDTO { DDD = "11", DDI = "55", Numero = "999999999" }
            };

            // act
            var result = await _client.PostAsJsonAsync(ApiKey, cliente);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Cliente_Get_GetPaginated_PaginaInvalida_BadRequest()
        {
            // act
            var result = await _client.GetAsync($"{ApiKey}?page=0&pageSize=10");

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}

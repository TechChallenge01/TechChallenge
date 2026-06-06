using Application.Gateways.Clientes;
using Application.Gateways.Veiculos;
using Application.Interfaces;
using Application.Presenters.Veiculos;
using Application.UseCases.Veiculos;
using Shared.DTOs;
using Shared.DTOs.Veiculos.Output;
using Shared.DTOs.Veiculos.Requests;
using Shared.Result;

namespace Application.Controllers.Veiculos
{
    public class VeiculoController
    {
        private readonly IVeiculoDataSource _dataSource;

        public VeiculoController(IVeiculoDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICommandResult<PagedResultDTO<VeiculoOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var presenter = new VeiculoPresenter("Pesquisa de veiculos retornada com sucesso!");
            try
            {
                var veiculoGateway = VeiculoGateway.Create(_dataSource);
                var useCase = GetPaginatedUseCase.Create(veiculoGateway);

                var veiculos = await useCase.Run(page, pageSize, ct);

                return presenter.TransformPaged(veiculos.veiculos, page, veiculos.total);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<PagedResultDTO<VeiculoOutputDTO>>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<PagedResultDTO<VeiculoOutputDTO>>(ex.Message);
            }
        }

        public async Task<ICommandResult<VeiculoOutputDTO>> GetById(Guid id, CancellationToken ct)
        {
            var presenter = new VeiculoPresenter("Veiculo retornado com sucesso!");
            try
            {
                var veiculoGateway = VeiculoGateway.Create(_dataSource);
                var useCase = GetByIdUseCase.Create(veiculoGateway);

                var response = await useCase.Run(id, ct);

                if (response is null)
                    presenter.NotFound<VeiculoOutputDTO>("Veiculo não encontrado!");

                return presenter.TransformObject(response);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<VeiculoOutputDTO>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<VeiculoOutputDTO>(ex.Message);
            }
        }

        public async Task<ICommandResult<Guid>> Create(VeiculoRequestDTO request, Guid UsuarioId, IClienteDataSource clienteDataSource, CancellationToken ct)
        {
            var presenter = new VeiculoPresenter("Veiculo criado com sucesso!");
            try
            {
                var clienteGateway = ClienteGateway.Create(clienteDataSource);
                var veiculoGateway = VeiculoGateway.Create(_dataSource);
                var useCase = CreateUseCase.Create(veiculoGateway, clienteGateway);
                var response = await useCase.Run(request, UsuarioId, ct);

                return presenter.Created<Guid>(response);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }
        }

        public async Task<ICommandResult> Delete(Guid id, Guid usuarioId, CancellationToken ct)
        {
            var presenter = new VeiculoPresenter("Veiculo deletado com sucesso!");

            try
            {
                var veiculoGateway = VeiculoGateway.Create(_dataSource);
                var useCase = DeleteUseCase.Create(veiculoGateway);
                await useCase.Run(id, usuarioId, ct);

                return presenter.NoContent();
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }
        }

        public async Task<ICommandResult> Update(Guid id, Guid usuarioId, VeiculoRequestDTO request, CancellationToken ct)
        {
            var presenter = new VeiculoPresenter("Veiculo Atualizado com sucesso!");
            try
            {
                var veiculoGateway = VeiculoGateway.Create(_dataSource);
                var useCase = UpdateUseCase.Create(veiculoGateway);
                await useCase.Run(id, usuarioId, request, ct);

                return presenter.NoContent();

            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }
        }
    }
}

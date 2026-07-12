using Application.Auth.DTOs.Requests;
using Application.Gateways.Usuarios;
using Application.Interfaces;
using Application.Presenters.Usuarios;
using Application.UseCases.Usuarios;
using Shared.DTOs.Usuarios.Output;
using Shared.Result;

namespace Application.Controllers.Usuarios
{
    public class UsuarioController
    {
        private readonly IUsuarioDataSource _dataSource;

        public UsuarioController(IUsuarioDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICommandResult<LoginOutputDTO>> Login(LoginRequestDTO request, IJwtService jwtService, CancellationToken ct)
        {
            var presenter = new UsuarioPresenter("Login realizado com sucesso!");
            try
            {
                var usuarioGateway = UsuarioGateway.Create(_dataSource);
                var useCase = LoginUseCase.Create(usuarioGateway, jwtService);

                var loginOutput = await useCase.Run(request, ct);

                return presenter.TransformLogin(loginOutput);
            }
            catch (UnauthorizedAccessException ex)
            {
                return presenter.Unauthorized<LoginOutputDTO>(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<LoginOutputDTO>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<LoginOutputDTO>(ex.Message);
            }
        }

        public async Task<ICommandResult<Guid>> CriarUsuario(CriarUsuarioRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            var presenter = new UsuarioPresenter("Usuário criado com sucesso!");
            try
            {
                var usuarioGateway = UsuarioGateway.Create(_dataSource);
                var useCase = new CriarUsuarioUseCase(usuarioGateway);

                var idNovoUsuario = await useCase.Run(request, idUsuario, ct);

                return presenter.Created(idNovoUsuario);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }
        }
    }
}

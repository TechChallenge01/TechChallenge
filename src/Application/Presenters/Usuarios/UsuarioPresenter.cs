using Shared.DTOs.Usuarios.Output;
using Shared.Result;
using System.Net;

namespace Application.Presenters.Usuarios
{
    public class UsuarioPresenter
    {
        private string _message;

        public UsuarioPresenter(string? message = null)
        {
            _message = message ?? string.Empty;
        }

        public ICommandResult<LoginOutputDTO> TransformLogin(LoginOutputDTO loginOutput)
        {
            return new CommandResult<LoginOutputDTO>
            {
                Data = loginOutput,
                Message = _message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public ICommandResult<T> Created<T>(T data)
        {
            return new CommandResult<T>
            {
                Message = _message,
                StatusCode = HttpStatusCode.Created,
                Data = data
            };
        }

        public ICommandResult NoContent()
        {
            return new CommandResult
            {
                Message = _message,
                StatusCode = HttpStatusCode.NoContent
            };
        }

        public ICommandResult<T> InternalError<T>(string message)
        {
            return new CommandResult<T>
            {
                Message = message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        public ICommandResult<T> BadRequest<T>(string message)
        {
            return new CommandResult<T>
            {
                Message = message,
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public ICommandResult<T> Unauthorized<T>(string message)
        {
            return new CommandResult<T>
            {
                Message = message,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        public ICommandResult InternalError(string message)
        {
            return new CommandResult
            {
                Message = message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        public ICommandResult BadRequest(string message)
        {
            return new CommandResult
            {
                Message = message,
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public ICommandResult Unauthorized(string message)
        {
            return new CommandResult
            {
                Message = message,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        public ICommandResult NotFound(string message)
        {
            return new CommandResult
            {
                Message = message,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public ICommandResult<T> NotFound<T>(string message)
        {
            return new CommandResult<T>
            {
                Message = message,
                StatusCode = HttpStatusCode.NotFound
            };
        }
    }
}

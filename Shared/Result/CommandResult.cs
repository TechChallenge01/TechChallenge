using System.Net;

namespace Shared.Result
{
    public class CommandResult : ICommandResult
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
    public class  CommandResult<T> : CommandResult, ICommandResult<T>
    {
        public T Data { get; set; }
    }
}

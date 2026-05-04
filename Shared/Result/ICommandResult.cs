using System.Net;
using System.Text.Json.Serialization;

namespace Shared.Result
{
    public interface ICommandResult
    {
        public string Message { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
    public interface ICommandResult<T> : ICommandResult
    {
        public T Data { get; set; }
    }
}

using Shared.DTOs;

namespace Application.EmailServices
{
    public interface IEmailService
    {
        Task Send(EmailPayloadDTO payload, CancellationToken cancellationToken);
    }
}

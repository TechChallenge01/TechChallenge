using Shared.DTOs;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task Send(EmailPayloadDTO payload, CancellationToken cancellationToken);
    }
}

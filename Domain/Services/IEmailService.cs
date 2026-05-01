using Shared.DTOs;

namespace Domain.Services
{
    public interface IEmailService
    {
        Task Send(EmailPayloadDTO payload, CancellationToken cancellationToken);
    }
}

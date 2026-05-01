using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public interface IEmailService
    {
        Task Send(EmailPayloadDTO payload, CancellationToken cancellationToken);
    }
}

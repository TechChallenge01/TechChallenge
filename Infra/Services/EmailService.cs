using Application.EmailServices;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System.Net;
using System.Net.Mail;

namespace Infra.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;
        private readonly string _fromAddress;

        public EmailService(IConfiguration config)
        {
            _fromAddress = config["Smtp:From"]!;

            _client = new SmtpClient(config["Smtp:Host"])
            {
                Port = int.Parse(config["Smtp:Port"]!),
                Credentials = new NetworkCredential(
                    config["Smtp:User"],
                    config["Smtp:Pass"]
                ),
                EnableSsl = true,
            };
        }
        public async Task Send(EmailPayloadDTO payload, CancellationToken cancellationToken)
        {
            bool mock = true;

            if (mock)
            {
                Console.WriteLine("Email enviado com sucesso!");
            }
            else
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_fromAddress),
                    Subject = payload.Subject,
                    Body = payload.Body,
                    IsBodyHtml = payload.IsHtml
                };

                await _client.SendMailAsync(message, cancellationToken);
            }
        }
    }
}

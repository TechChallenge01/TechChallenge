using Application.Gateways.Clientes;
using Application.Interfaces;
using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.OrdemServicoAggregates;
using Shared.DTOs;

namespace Application.UseCases.OrdensServicos
{
    public class EnviarOrcamentoUseCase
    {
        private readonly ClienteGateway _clienteGateway;
        private readonly IEmailService _emailService;

        private EnviarOrcamentoUseCase(ClienteGateway clienteGateway, IEmailService emailService)
        {
            _clienteGateway = clienteGateway;
            _emailService = emailService;
        }

        public static EnviarOrcamentoUseCase Create(ClienteGateway clienteGateway, IEmailService emailService)
        {
            return new EnviarOrcamentoUseCase(clienteGateway, emailService);
        }

        public async Task Run(OrdemServico ordemServico, CancellationToken ct)
        {
            try
            {
                Cliente cliente = await _clienteGateway.GetById(ordemServico.ClienteId, ct);

                var orcamento = $"""
                                ORÇAMENTO

                                PEÇAS:
                                {string.Join("\n", ordemServico.Pecas.Select(p =>
                                        $"  {p.Quantidade}x {p.Peca.Nome} | Unit: {p.ValorUnitario:C} | Total: {p.ValorTotal:C}"))}

                                SERVIÇOS:
                                {string.Join("\n", ordemServico.Servicos.Select(s =>
                                        $"  {s.Quantidade}x {s.Servico.Nome} | Unit: {s.ValorUnitario:C} | Total: {s.ValorTotal:C}"))}

                                Desconto:    {ordemServico.ValorDesconto:C}
                                Valor Total: {ordemServico.ValorTotal:C}
                                """;

                var payloadEmail = new EmailPayloadDTO
                {
                    To = cliente.Email.EnderecoEmail,
                    Body = $"Olá {cliente.Nome}, o diagnóstico da sua ordem de serviço (ID: {ordemServico.Id}) foi realizado. segue o orçamento:\n{orcamento}",
                    Subject = "Orçamento da Ordem de Serviço"
                };

                await _emailService.Send(payloadEmail, ct);
            }
            catch (Exception ex)
            {
                // Log do erro de envio de email, mas não interrompe o fluxo principal
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
            }
        }

    }
}

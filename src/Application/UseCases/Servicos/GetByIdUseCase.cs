using Application.Gateways.Servicos;
using Domain.Entities;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Servicos
{
    public class GetByIdUseCase
    {
        private readonly ServicoGateway _servicoGatweay;

        private GetByIdUseCase(ServicoGateway servicoGatweay)
        {
            _servicoGatweay = servicoGatweay;
        }

        public static GetByIdUseCase Create(ServicoGateway servicoGatweay)
        {
            return new GetByIdUseCase(servicoGatweay);
        }

        public async Task<Servico>? Run(Guid id, CancellationToken ct)
        {
            try
            {
                var servico = await _servicoGatweay.GetById(id, ct);

                return servico;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

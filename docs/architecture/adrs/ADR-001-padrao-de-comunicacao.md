# ADR-001 — Padrão de comunicação: REST síncrono, sem mensageria

**Status:** Aceito

## Contexto

O sistema tem 3 pontos de comunicação entre componentes: Cliente ↔ borda (API Gateway), borda ↔ API principal (via VPC Link/NLB) e API principal ↔ banco (EF Core). Era preciso decidir o padrão de comunicação entre esses componentes.

## Decisão

**REST síncrono (requisição/resposta) em todos os pontos**, sem fila/broker de mensagens (SQS, SNS, EventBridge, Kafka) entre os componentes da aplicação.

## Justificativa

- O domínio (CRUD de clientes/veículos/OS/peças + transições de status de OS) é fundamentalmente request/response: o cliente que abre uma OS espera uma resposta imediata de sucesso/erro, não um processamento assíncrono em background.
- Introduzir um broker adicionaria um componente de infraestrutura extra (custo, operação, mais um ponto de falha) sem um caso de uso real que justifique desacoplamento assíncrono neste momento (não há, por exemplo, um requisito de "notificar N sistemas externos quando uma OS muda de status").
- O único ponto que já é naturalmente assíncrono/desacoplado — a ingestão de logs para o Datadog — é resolvido de forma nativa pela própria AWS (CloudWatch Logs Subscription Filter → Lambda forwarder), sem precisar de um broker genérico de aplicação.

## Consequências

- Simplicidade operacional: nenhum broker para provisionar, monitorar ou pagar.
- Acoplamento temporal entre borda e API principal — se o cluster estiver indisponível, a chamada falha na hora (mitigado pelo HPA + múltiplas réplicas do Deployment).
- Se o domínio evoluir para precisar de processamento assíncrono real (ex.: notificações por e-mail/SMS quando a OS muda de status, mencionado como possível extensão), a decisão deve ser revisitada — um novo ADR deve documentar a escolha do broker e o desenho do desacoplamento.

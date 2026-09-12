# ADR-004 — Observabilidade híbrida: agente no EKS + Log Forwarder para a camada serverless

**Status:** Aceito

## Contexto

A rubrica exige integração com Datadog (ou New Relic) cobrindo latência de APIs, consumo de recursos do Kubernetes, healthchecks/uptime, alertas de falha no processamento de OS e logs estruturados correlacionados — tanto para a API principal (em EKS) quanto para a camada serverless (Lambdas em `TechChallenger.auth`). A Lambda de autenticação roda **dentro da VPC** (precisa alcançar o RDS por IP privado) e as subnets usadas **não têm NAT Gateway** (decisão de custo — ver `RFC-001`), portanto a Lambda não tem rota de saída para a internet e não consegue enviar dados diretamente para a API da Datadog.

## Decisão

Duas peças complementares, cada uma resolvendo a parte do sistema onde efetivamente tem acesso:

1. **Datadog Agent (DaemonSet) dentro do cluster EKS** — um pod do agente por node, com acesso direto (rede do cluster) aos pods da API principal. Cobre: APM (traces via `Datadog.Trace.Bundle`), métricas de infraestrutura (CPU/memória via kubelet), métricas de negócio customizadas (DogStatsD — `techchallenger.os.criadas/status_alterado/erros/tempo_execucao_segundos`) e logs estruturados com correlação trace↔log (`DD_LOGS_INJECTION`).
2. **Datadog Log Forwarder** — uma Lambda **fora da VPC** (portanto com egress normal), inscrita via CloudWatch Logs Subscription Filter nos log groups da Lambda de auth, do Lambda authorizer e do access log do API Gateway. Ela lê os eventos do CloudWatch (que já recebe logs de qualquer Lambda, dentro ou fora de VPC, através do serviço da AWS — não da rede do cliente) e os encaminha para a Datadog.

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Adicionar NAT Gateway às subnets privadas** | Resolveria o problema de rede, mas tem custo fixo por hora (~US$32/mês) só para permitir telemetria — desproporcional ao orçamento do AWS Academy, e não muda a necessidade do Log Forwarder para as outras Lambdas que também precisariam de rota de saída. |
| **Extensão Datadog embutida na imagem da Lambda** (Datadog Lambda Extension) | Testada inicialmente; também depende de egress da própria Lambda para a API da Datadog — não resolve o problema de a subnet não ter NAT. Removida da imagem em favor do Forwarder. |
| **Mover a Lambda de auth para fora da VPC / tornar o RDS público sem restrição** | Rebaixaria a postura de segurança (Lambda perderia acesso privado ao banco, ou o RDS ficaria exposto sem o controle de rede atual) só para resolver observabilidade — trade-off não aceito. |
| **Só CloudWatch nativo, sem Datadog para a camada serverless** | Atenderia parcialmente (métricas de invocação/erro/duração do Lambda existem nativamente), mas deixaria logs da camada serverless fora do mesmo painel usado para o resto do sistema, e sem correlação com os traces da API principal. |

## Consequências

- O Forwarder é publicado a partir do release oficial da Datadog no GitHub (baixado pela esteira de CD e implantado por pacote local) em vez do bucket S3 público da própria Datadog — o AWS Academy bloqueia `GetObject` cross-account em buckets de outras contas, então o caminho documentado pela Datadog (apontar a Lambda direto para `s3://datadog-cloudformation-template/...`) não funciona neste ambiente.
- Duas fontes de verdade para "observabilidade" (agente no cluster + forwarder para a Lambda), mas um único destino (Datadog) — dashboards e monitors enxergam o sistema como um todo, apesar da diferença de mecanismo de coleta.
- `techchallenger.os.tempo_execucao_segundos` está instrumentado no código (`DatadogMetricsService`) mas, até o momento desta ADR, nenhum caso de uso o invoca — pendência conhecida para fechar 100% o item "tempo médio de execução por status" do dashboard exigido pela rubrica.

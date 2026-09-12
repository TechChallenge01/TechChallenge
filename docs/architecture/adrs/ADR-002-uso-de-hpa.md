# ADR-002 — Uso de HPA (Horizontal Pod Autoscaler)

**Status:** Aceito

## Contexto

A rubrica exige um "Cluster Kubernetes com escalabilidade". A carga de OS varia por horário de pico (mencionado explicitamente no objetivo da Fase 2/3 — "suportar alta disponibilidade e maiores volumes de OS em horários de pico"), o que pede escala automática do número de réplicas da aplicação, não apenas um número fixo de pods.

## Decisão

Usar **Horizontal Pod Autoscaler (HPA)** no `Deployment` da API principal, baseado em métricas de CPU/memória coletadas pelo **`metrics-server`** (instalado via Helm no próprio Terraform do cluster, repositório `TechChallenger.k8s`).

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Número fixo de réplicas** | Não atende ao requisito de escalabilidade nem responde a picos de carga reais — ou super-provisiona (custo desnecessário no AWS Academy) ou sub-provisiona (indisponibilidade em pico). |
| **KEDA (autoscaling orientado a eventos)** | Mais adequado para escalar com base em filas/eventos externos — não há fila neste desenho (ver [ADR-001](ADR-001-padrao-de-comunicacao.md)); adicionaria um operador extra ao cluster sem necessidade real. |
| **Cluster Autoscaler (escala os *nodes*, não os pods)** | Complementar, não substituto — resolve escala de infraestrutura (nodes do EKS), não de réplicas da aplicação. Não adotado nesta fase por orçamento/tempo; o node group tem `min=1`/`max=2` fixo via Terraform. |

## Consequências

- Requisitos essenciais nos manifestos: `requests`/`limits` de CPU definidos no `Deployment` (o HPA precisa de uma base para calcular percentual de uso) e o `metrics-server` funcional no cluster.
- `metrics-server` precisa da flag `--kubelet-insecure-tls` no ambiente do Learner Lab, por causa do certificado do kubelet em clusters de laboratório — documentado no `eks.tf`.
- Sem Cluster Autoscaler, o HPA tem um teto prático: se todos os nodes do node group (`max_size = 2`) já estiverem saturados, novas réplicas ficam `Pending` até haver capacidade — aceitável para o escopo e orçamento desta fase, mas um limite conhecido a documentar para produção real.

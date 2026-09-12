# RFC-001 — Escolha do provedor de nuvem

**Status:** Aceito

## Contexto

A Fase 3 exige elevar a aplicação a um nível de operação corporativa: API Gateway, Function Serverless, banco gerenciado, cluster Kubernetes escalável e Terraform, tudo com deploy automatizado e observabilidade. Era preciso escolher um provedor de nuvem que oferecesse todos esses serviços gerenciados e que fosse viável dentro do orçamento e prazo do grupo (sem custo direto do bolso do time).

## Decisão

**AWS**, via **AWS Academy Learner Lab** (crédito educacional, ~US$50 de orçamento por sessão de lab).

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Azure** / **GCP** | Sem crédito educacional já disponível para o grupo; exigiria cartão de crédito pessoal ou trial com limitações de tempo mais curtas que o necessário para todas as fases. |
| **Infra própria (VPS + k3s)** | Não atende ao requisito de "banco de dados gerenciado" nem oferece Function Serverless / API Gateway gerenciado — o grupo teria que construir e manter esses componentes manualmente, na contramão do objetivo da fase (usar serviços gerenciados de nuvem). |

## Consequências

- Restrições específicas do **AWS Academy Learner Lab** passaram a moldar decisões técnicas subsequentes: a conta bloqueia `iam:CreateRole`/`iam:AttachRolePolicy` (daí o uso da `LabRole` pronta em todos os repositórios — ver [`data.tf`](https://github.com/TechChallenge01/TechChallenger.auth/blob/main/terraform/data.tf)), as credenciais são temporárias (expiram a cada sessão do lab, exigindo atualização de secrets do GitHub a cada rodada de deploy) e o orçamento limitado motivou escolhas como nodes do EKS em subnets públicas (evitar custo de NAT Gateway) e `terraform destroy` sistemático entre sessões de trabalho.
- Região única (`us-east-1`) — sem necessidade de multi-região para o escopo do desafio.

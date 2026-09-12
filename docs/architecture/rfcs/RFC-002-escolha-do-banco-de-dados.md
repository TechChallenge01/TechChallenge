# RFC-002 — Escolha do banco de dados

**Status:** Aceito

## Contexto

A rubrica pede um "Banco de Dados Gerenciado (PostgreSQL, MySQL, SQL Server, etc.)", de livre escolha, provisionado em Terraform em um repositório próprio. O domínio (Cliente, Veículo, Ordem de Serviço, Peça, Insumo, Serviço, Estoque) e as migrations do EF Core já existiam de fases anteriores, construídos sobre SQL Server.

## Decisão

Manter **SQL Server**, agora como **Amazon RDS (edição Express, `sqlserver-ex`)**.

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **PostgreSQL (Amazon RDS)** | Motor mais barato e popular em cenários cloud-native, mas exigiria recriar/validar as migrations do EF Core (tipos como `nvarchar`, `time`, `decimal(10,2)` têm equivalentes, mas o esforço de migração não traz nenhum benefício funcional para o que esta fase avalia — infraestrutura, CI/CD e observabilidade, não modelagem de dados). |
| **MySQL (Amazon RDS)** | Mesma consideração do PostgreSQL — trocar o motor sem necessidade, com risco de regressão no domínio já testado. |
| **DynamoDB / banco não-relacional** | O domínio é fortemente relacional (Cliente→Veículo→Ordem de Serviço com múltiplas associações N-N com peças/insumos/serviços e regras de integridade referencial) — modelar isso em um banco NoSQL exigiria duplicar dados e reimplementar checagens de integridade na aplicação, sem ganho de performance ou escala que o volume do desafio justifique. |

## Consequências

- O modelo de dados em si (ver [`03-modelo-dados.md`](../03-modelo-dados.md)) não mudou nesta fase.
- O que mudou foi **onde e como o banco é provisionado**: passou a viver em Terraform próprio, no repositório [`TechChallenger.db`](https://github.com/TechChallenge01/TechChallenger.db), separado da infraestrutura de rede/cluster ([`TechChallenger.k8s`](https://github.com/TechChallenge01/TechChallenger.k8s)) — os dois se descobrem por `data source` (tag/nome), sem acoplamento de state.
- SQL Server Express não suporta Multi-AZ (`multi_az = false`) — aceitável para o contexto acadêmico; documentado como ponto de atenção para produção real.
- `publicly_accessible = true`, restrito por security group a IPs específicos — decisão pragmática para permitir acesso via SSMS durante o desenvolvimento (ver [`ADR` correspondente](../adrs) e notas no README do `TechChallenger.db`); para produção, o caminho documentado é subnet privada + `publicly_accessible = false`.

# Documentação Arquitetural — Tech Challenge (Fase 3)

Índice da documentação exigida pela rubrica da Fase 3. Cobre a solução como um todo — os 4 repositórios — não só este repo de aplicação.

| Documento | Conteúdo |
|---|---|
| [`01-diagrama-componentes.md`](01-diagrama-componentes.md) | Diagrama de componentes: nuvem, APIs, banco, observabilidade |
| [`02-diagramas-sequencia.md`](02-diagramas-sequencia.md) | Fluxo de autenticação por CPF e de abertura de Ordem de Serviço |
| [`03-modelo-dados.md`](03-modelo-dados.md) | Diagrama ER + justificativa da escolha do banco e do modelo relacional |
| [`rfcs/`](rfcs) | RFCs — decisões técnicas (nuvem, banco, estratégia de autenticação) |
| [`adrs/`](adrs) | ADRs — decisões arquiteturais permanentes |

## Os 4 repositórios

| Repositório | Papel |
|---|---|
| [TechChallenge](https://github.com/TechChallenge01/TechChallenge) | Aplicação principal (API em Kubernetes) |
| [TechChallenger.auth](https://github.com/TechChallenge01/TechChallenger.auth) | API Gateway + Function Serverless de autenticação por CPF |
| [TechChallenger.db](https://github.com/TechChallenge01/TechChallenger.db) | Infraestrutura do banco de dados gerenciado (Terraform) |
| [TechChallenger.k8s](https://github.com/TechChallenge01/TechChallenger.k8s) | Infraestrutura do cluster Kubernetes e rede (Terraform) |

## Vídeo de demonstração

_(link a preencher quando gravado — deve cobrir: autenticação por CPF, pipeline de CI/CD, deploy automatizado, consumo das APIs protegidas, dashboard de monitoramento e logs/traces em execução)_

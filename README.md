# TechChallenge API

## Descrição
API REST para gerenciamento de oficina mecânica (clientes, veículos, ordens de serviço, peças e estoque), desenvolvida em .NET 10 seguindo Clean Architecture (Domain, Application, Infrastructure, API), com cobertura de testes automatizados nos fluxos críticos.

## Repositórios do Tech Challenge
Este é um dos quatro repositórios que compõem a solução:

| Repositório | Papel |
|---|---|
| **TechChallenge** (este) | Aplicação principal (API em Kubernetes) |
| [TechChallenge.auth](https://github.com/TechChallenge01/TechChallenger.auth) | Function Serverless de autenticação por CPF |
| [TechChallenge.db](https://github.com/TechChallenge01/TechChallenge.db) | Infraestrutura do banco de dados gerenciado (Terraform) |
| [TechChallenge.k8s](https://github.com/TechChallenge01/TechChallenge.k8s) | Infraestrutura do cluster Kubernetes (Terraform) |

### Objetivo da Fase 2
Na Fase 1 o sistema entregou a gestão de ordens de serviço, veículos, clientes e peças. A Fase 2 evolui essa base para suportar alta disponibilidade e maiores volumes de OS em horários de pico, incorporando:
- Refino do código sob Clean Architecture, com Clean Code e testes automatizados cobrindo os fluxos críticos (abertura, diagnóstico, aprovação, execução e entrega de OS).
- Conteinerização revisada (Dockerfile multi-stage + Docker Compose para desenvolvimento local).
- Orquestração via Kubernetes (Deployments, Services, ConfigMaps/Secrets e Horizontal Pod Autoscaler) — manifestos em [`k8s/`](k8s).
- Infraestrutura como Código com Terraform, provisionando VPC, cluster EKS e banco de dados (RDS SQL Server) — scripts em [`infra/terraform/`](infra/terraform).
- Pipeline de CI/CD (GitHub Actions) fazendo build, testes, build/push da imagem Docker, migration do banco e deploy no Kubernetes — workflows em [`.github/workflows/`](.github/workflows).

## Pré-requisitos

### Execução local
- Docker
- Docker Compose
- (Opcional) .NET 10 SDK para desenvolvimento local

### Deploy em Kubernetes / Infraestrutura (opcional)
- kubectl
- Terraform >= 1.5
- AWS CLI configurado (credenciais AWS Academy Learner Lab / conta AWS com acesso a EKS, ECR e RDS)
- Helm (usado internamente pelo Terraform para instalar o metrics-server no cluster)

## Como Executar

### Usando Docker Compose
1. Clone o repositório:
   ```bash
   git clone https://github.com/TechChallenge01/TechChallenge.git
   ```

2. Navegue até o diretório do projeto:
   ```bash
   cd TechChallenge
   ```

3. Inicie a aplicação:
   ```bash
   docker-compose up
   ```

4. A aplicação estará disponível em:
   ```
   http://localhost:8080
   ```

---

## Provisionamento da Infraestrutura (Terraform)

Os scripts em [`infra/terraform/`](infra/terraform) provisionam, na AWS, toda a infraestrutura necessária para rodar a aplicação em produção:

| Recurso | Arquivo | Descrição |
|---------|---------|-----------|
| VPC, subnets públicas/privadas, Internet Gateway, route tables | `vpc.tf` | Rede da aplicação — subnets públicas para os nodes do EKS, privadas para o banco |
| Cluster EKS + Node Group + metrics-server (via Helm) | `eks.tf` | Cluster Kubernetes gerenciado e o componente necessário para o HPA funcionar |
| Repositório ECR | `ecr.tf` | Registry das imagens Docker publicadas pela pipeline de CD |
| Banco de dados (RDS SQL Server Express) | `sqlserver.tf` | Banco relacional gerenciado, na mesma VPC do cluster |
| IAM (LabRole) | `iam.tf` | Reaproveita a role padrão do AWS Academy Learner Lab (ambiente não permite criar roles/policies) |
| Variáveis e outputs | `variables.tf`, `outputs.tf`, `provider.tf` | Parametrização (região, nomes, CIDRs, tamanhos de instância) e valores expostos após o apply |

### Como aplicar

```bash
cd infra/terraform
terraform init
terraform plan -var="db_password=<SENHA_FORTE_DO_BANCO>"
terraform apply -var="db_password=<SENHA_FORTE_DO_BANCO>"
```

Ao final, os outputs trazem o endpoint do cluster EKS, a URL do repositório ECR e o endpoint do banco (`terraform output`). Esses valores alimentam a pipeline de CD e o Secret do Kubernetes.

> Ambiente pensado para o AWS Academy Learner Lab: usa a `LabRole` já existente (sem criação de IAM roles), nodes do EKS em subnets públicas (evita custo de NAT Gateway) e RDS com `skip_final_snapshot` para permitir destruir/recriar o lab livremente. Para destruir os recursos: `terraform destroy`.

---

## Deploy em Kubernetes

Os manifestos em [`k8s/`](k8s) descrevem o deploy da aplicação no cluster:

| Manifesto | Recurso | Descrição |
|-----------|---------|-----------|
| `namespace.yaml` | Namespace | Isola os recursos da aplicação (`techchallenger-ns`) |
| `configmap.yaml` | ConfigMap | Variáveis não sensíveis (ambiente, porta, log level) |
| `secret.yaml` | Secret | Connection string do banco (valor sensível, em base64) |
| `deployment.yaml` | Deployment | 2 réplicas da API, com readiness/liveness probes em `/health` |
| `service.yaml` | Service (LoadBalancer) | Expõe a aplicação externamente na porta 80 |
| `hpa.yaml` | HorizontalPodAutoscaler | Escala entre 2 e 6 réplicas por uso de CPU (70%) ou memória (80%) |
| `migration-job.yaml` | Job | Executa o EF Core migration bundle contra o banco antes do deploy |
| `datadog-agent.yaml` | DaemonSet | Agente do Datadog (APM, DogStatsD, métricas de infraestrutura e logs) — um por node |
| `datadog-secret.yaml` | Secret | Template da API Key do Datadog (valor sensível, em base64) — a esteira de CD gera a versão real a partir do GitHub Secret `DATADOG_API_KEY` |

### Aplicando manualmente

```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml   # ajuste a connection string em base64 antes

# Substitua ${IMAGE} pela tag da imagem publicada no ECR
export IMAGE="<ecr_repository_url>:<tag>"
envsubst < k8s/deployment.yaml | kubectl apply -f -
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/hpa.yaml

kubectl rollout status deployment/techchallenger -n techchallenger-ns
kubectl get service techchallenger-service -n techchallenger-ns
```

Em produção esse fluxo é automatizado pela pipeline de CD (veja a seção abaixo).

---

## Integração e Entrega Contínua (CI/CD)

A pipeline roda no GitHub Actions, em dois workflows complementares ([`.github/workflows/`](.github/workflows)):

### `ci.yml` — Integração Contínua
Disparado em Pull Requests para `release`/`main` e em push para `release`. Executa build, testes automatizados e build (sem push) da imagem Docker, validando o código antes do merge.

### `cd.yml` — Entrega Contínua
Disparado em push para `main`. Encadeia:
1. **build-test** — build e execução dos testes automatizados.
2. **build-push-image** — build da imagem Docker e push para o ECR.
3. **migrate-database** — aplica namespace/ConfigMap/Secret no cluster e roda o `migration-job.yaml` (EF Core migration bundle) contra o banco.
4. **deploy** — aplica `deployment.yaml`, `service.yaml` e `hpa.yaml` no EKS e aguarda o rollout.

As credenciais AWS (temporárias, do AWS Academy), a connection string do banco e a API Key do Datadog são injetadas via GitHub Secrets (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_SESSION_TOKEN`, `RDS_CONNECTION_STRING`, `DATADOG_API_KEY`).

---

## Observabilidade / Datadog

A aplicação é instrumentada com o Datadog em três frentes, todas configuradas via variáveis de ambiente (nenhuma delas exige recompilar a imagem):

1. **APM (traces)** — `Datadog.Trace.Bundle` (referenciado no `src/API/API.csproj`) faz auto-instrumentação de requisições HTTP, EF Core/SQL Server e chamadas externas. Ativado pelas variáveis `CORECLR_ENABLE_PROFILING`/`CORECLR_PROFILER`/`CORECLR_PROFILER_PATH`/`DD_DOTNET_TRACER_HOME` no `k8s/configmap.yaml`.
2. **Logs estruturados** — `Logging:Console:FormatterName=json` (também no ConfigMap) faz o `Microsoft.Extensions.Logging` nativo emitir logs em JSON. Com `DD_LOGS_INJECTION=true`, o tracer injeta `dd.trace_id`/`dd.span_id` em cada log, permitindo pular do log direto para o trace correspondente no Datadog (correlação entre requisições).
3. **Métricas de negócio (DogStatsD)** — `IMetricsService`/`DatadogMetricsService` ([`src/Application/Interfaces/IMetricsService.cs`](src/Application/Interfaces/IMetricsService.cs), [`src/Infra/Services/DatadogMetricsService.cs`](src/Infra/Services/DatadogMetricsService.cs)) publicam contadores/histogramas customizados a cada transição de status de uma ordem de serviço:
   - `techchallenger.os.criadas` — volume de OS criadas
   - `techchallenger.os.status_alterado` (tag `status`) — volume por status (Diagnóstico, Execução, Finalização, etc.)
   - `techchallenger.os.tempo_execucao_segundos` (tag `status`) — histograma do tempo médio de execução
   - `techchallenger.os.erros` (tag `operacao`) — falhas por ação (Create, Aprovar, RealizarDiagnostico, etc.)

O [Datadog Agent](k8s/datadog-agent.yaml) roda como `DaemonSet` (um por node do EKS) e expõe DogStatsD (8125/UDP) e o receptor de traces do APM (8126/TCP) via `hostPort`, além de coletar métricas de CPU/memória dos pods (`kubelet`) e logs de todos os containers do namespace. O pod da API aponta para o agente do seu próprio node via `DD_AGENT_HOST` (`status.hostIP`, injetado em `k8s/deployment.yaml`).

### Aplicando

```bash
kubectl create secret generic datadog-secret \
  --namespace techchallenger-ns \
  --from-literal=api-key=<SUA_DATADOG_API_KEY>
kubectl apply -f k8s/datadog-agent.yaml
```

Em produção isso é feito automaticamente pelo `cd.yml`, a partir do secret `DATADOG_API_KEY` configurado no GitHub.

### Dashboards e alertas sugeridos
- **Healthcheck/uptime**: monitor de disponibilidade sobre `GET /health` (Synthetics ou monitor HTTP do Datadog) + os `readinessProbe`/`livenessProbe` do `k8s/deployment.yaml`.
- **Latência das APIs**: `trace.aspnet_core.request` (p50/p95/p99) vindo do APM, por rota.
- **CPU/memória do Kubernetes**: `kubernetes.cpu.usage.total` / `kubernetes.memory.usage`, comparado com os `requests`/`limits` do `deployment.yaml` e os alvos do `k8s/hpa.yaml`.
- **Volume diário de OS**: soma de `techchallenger.os.criadas` por dia.
- **Tempo médio de execução por status**: média/percentis de `techchallenger.os.tempo_execucao_segundos`, agrupado pela tag `status`.
- **Erros e falhas nas integrações**: soma de `techchallenger.os.erros` por `operacao`, mais os erros 5xx capturados automaticamente pelo APM.
- **Alerta de falha no processamento de OS**: monitor de anomalia/threshold sobre `techchallenger.os.erros` e sobre a taxa de erro do APM no serviço `techchallenger-api`.

---

## Documentação da API

A API utiliza o **Scalar** como interface de documentação interativa. Com a aplicação em execução, acesse:
```
http://localhost:8080/scalar
```

Também está disponível uma collection do **Postman** com todos os endpoints e exemplos de request prontos para uso:

📄 [`docs/API - v1 - Completa.postman_collection.json`](docs/API%20-%20v1%20-%20Completa.postman_collection.json)

Importe o arquivo no Postman e configure a variável `baseUrl` como `http://localhost:8080` (ou a URL pública do serviço no Kubernetes) para começar a usar.

## Vídeo Demonstrativo

🎥 [Assista à demonstração do ambiente em execução](https://www.youtube.com/watch?v=a9y73qtfT-E) — deploy da aplicação, execução do CI/CD, consumo das APIs e escalabilidade automática (HPA).

---

## Autenticação
Todos os endpoints (exceto login) requerem um token JWT no header:
```
Authorization: Bearer <seu_token_jwt>
```

Existem duas formas de obter esse token, dependendo de quem está se autenticando:

| Quem | Como | Onde |
|---|---|---|
| Funcionários/equipe interna (`Administrador`, `Funcionario`, `Mecanico`, `Almoxarifado`) | Login com e-mail/senha | `POST /api/login`, nesta API |
| Cliente final (dono do veículo) | Autenticação por CPF | `POST /auth/cpf`, na Function Serverless [TechChallenge.auth](https://github.com/TechChallenge01/TechChallenger.auth) |

Os dois emissores assinam o token com a **mesma chave simétrica** (`Jwt:Key`/`Jwt:Issuer`/`Jwt:Audience`) — esta API só valida a assinatura e as claims (`sub`, `role`), sem saber qual serviço emitiu o token. Isso significa que qualquer alteração em `Jwt:Key` precisa ser replicada nos dois repositórios ao mesmo tempo (ver `Jwt__Key` no `docker-compose.yml`/`k8s/secret.yaml` aqui e a variável `jwt_key` no Terraform do `TechChallenge.auth`).

Um token emitido pela `TechChallenge.auth` carrega `role=Cliente` e `sub=<ClienteId>`. As rotas que aceitam esse perfil (`GET /api/ordemServico/{id}` e `PUT /api/ordemServico/{id}/Aprovar`) verificam que o `ClienteId` do token é o dono da ordem de serviço consultada/aprovada — um cliente autenticado não consegue ver ou aprovar ordens de serviço de outro cliente (retorna `403 Forbidden`).

---

## Endpoints da API

### 🩺 Health Check
**Base URL:** `/`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Verifica status da aplicação | Público | 200 |

---

### 🔐 AuthController - Autenticação
**Base URL:** `/api/auth`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| POST | `/login` | Realiza login do usuário | Público | 200, 400, 401, 500 |

**Request (Login):**
```json
{
  "email": "Admin@email.com",
  "senha": "12345678"
}
```

**Response (Login):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### 👤 UsuarioController - Gerenciamento de Usuários
**Base URL:** `/api/usuario`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| POST | `/` | Cria novo usuário | Administrador | 201, 400, 401, 403, 500 |

**Request (Criar Usuário):**
```json
{
  "nome": "Administrador Sistema",
  "email": "admin@sistema.com.br",
  "senha": "SenhaSegura123!",
  "perfil": 1
}
```

> **Perfis disponíveis (campo `perfil`):**
> `1` = Administrador, `2` = Funcionário, `3` = Mecânico, `4` = Almoxarifado, `5` = Cliente

---

### 👥 ClienteController - Gerenciamento de Clientes
**Base URL:** `/api/cliente`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista clientes (paginado) | Administrador, Funcionário | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém cliente por ID | Administrador, Funcionário | 200, 400, 401, 404, 500 |
| POST | `/` | Cria novo cliente | Administrador, Funcionário | 201, 400, 401, 500 |
| PUT | `/{id}` | Atualiza cliente | Administrador, Funcionário | 204, 400, 401, 404, 500 |
| DELETE | `/{id}` | Deleta cliente | Administrador, Funcionário | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar/Atualizar):**
```json
{
  "nome": "João da Silva",
  "cpf": "50872558843",
  "cnpj": "",
  "email": "joao.silva@email.com",
  "telefone": {
    "ddd": "11",
    "ddi": "55",
    "numero": "999998888"
  },
  "endereco": {
    "logradouro": "Rua das Flores",
    "numero": "123",
    "complemento": "Apto 42",
    "bairro": "Centro",
    "cep": "01000000",
    "cidade": "São Paulo",
    "uf": "SP"
  }
}
```

---

### 🚗 VeiculoController - Gerenciamento de Veículos
**Base URL:** `/api/veiculo`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista veículos (paginado) | Administrador, Funcionário | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém veículo por ID | Administrador, Funcionário | 200, 400, 401, 404, 500 |
| POST | `/` | Cria novo veículo | Administrador, Funcionário | 201, 400, 401, 404, 500 |
| PUT | `/{id}` | Atualiza veículo | Administrador, Funcionário | 204, 400, 401, 404, 500 |
| DELETE | `/{id}` | Deleta veículo | Administrador, Funcionário | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar/Atualizar):**
```json
{
  "modelo": "Civic EXL",
  "marcaVeiculo": "Honda",
  "clienteId": "550e8400-e29b-41d4-a716-446655440000",
  "ano": 2020,
  "placa": "ABC1234",
  "cor": "Prata"
}
```

---

### 🔧 PecaController - Gerenciamento de Peças
**Base URL:** `/api/peca`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista peças (paginado) | Administrador, Mecânico, Almoxarifado | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém peça por ID | Administrador, Mecânico, Almoxarifado | 200, 400, 401, 404, 500 |
| POST | `/` | Cria nova peça | Administrador, Almoxarifado | 201, 400, 401, 500 |
| PUT | `/{id}` | Atualiza peça | Administrador, Almoxarifado | 204, 400, 401, 404, 500 |
| DELETE | `/{id}` | Deleta peça | Administrador, Almoxarifado | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar/Atualizar):**
```json
{
  "nome": "Pastilha de Freio",
  "descricao": "Jogo de pastilhas de freio dianteiras",
  "marcaPeca": "Bosch",
  "precoVenda": 120.50
}
```

---

### 📦 InsumoController - Gerenciamento de Insumos
**Base URL:** `/api/insumo`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista insumos (paginado) | Administrador, Mecânico, Almoxarifado | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém insumo por ID | Administrador, Mecânico, Almoxarifado | 200, 400, 401, 404, 500 |
| POST | `/` | Cria novo insumo | Administrador, Almoxarifado | 201, 400, 401, 500 |
| PUT | `/{id}` | Atualiza insumo | Administrador, Almoxarifado | 204, 400, 401, 404, 500 |
| DELETE | `/{id}` | Deleta insumo | Administrador, Almoxarifado | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar/Atualizar):**
```json
{
  "nome": "Óleo de Motor 5W40",
  "descricao": "Óleo sintético para motor",
  "custoUnitario": 45.90
}
```

---

### 🛠️ ServicoController - Gerenciamento de Serviços
**Base URL:** `/api/servico`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista serviços (paginado) | Administrador, Funcionário, Mecânico | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém serviço por ID | Administrador, Funcionário, Mecânico | 200, 400, 401, 404, 500 |
| POST | `/` | Cria novo serviço | Administrador, Funcionário | 201, 400, 401, 500 |
| PUT | `/{id}` | Atualiza serviço | Administrador, Funcionário | 204, 400, 401, 404, 500 |
| DELETE | `/{id}` | Deleta serviço | Administrador, Funcionário | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar/Atualizar):**
```json
{
  "nome": "Troca de Óleo",
  "descricao": "Serviço completo de troca de óleo e filtro",
  "precoVenda": 80.00
}
```

---

### 📋 OrdemServicoController - Gerenciamento de Ordens de Serviço
**Base URL:** `/api/ordemservico`

| Método | Endpoint | Descrição | Status Exigido | Autorização | Status HTTP |
|--------|----------|-----------|----------------|-------------|-------------|
| GET | `/` | Lista ordens de serviço (paginado) | — | Administrador, Funcionário, Mecânico | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém ordem de serviço por ID | — | Administrador, Funcionário, Mecânico, Cliente | 200, 400, 401, 404, 500 |
| POST | `/` | Cria nova ordem de serviço | — | Administrador, Funcionário | 201, 400, 401, 404, 500 |
| PUT | `/{id}/IniciarDiagnostico` | Inicia o diagnóstico da OS | `Recebida` | Administrador, Mecânico | 204, 400, 401, 404, 500 |
| PUT | `/{id}/RealizarDiagnostico` | Registra peças/serviços e aguarda aprovação | `EmDiagnostico` | Administrador, Mecânico | 204, 400, 401, 404, 500 |
| PUT | `/{id}/Aprovar` | Aprova a OS e inicia a execução | `AguardandoAprovacao` | Administrador, Funcionário, Cliente | 204, 400, 401, 404, 500 |
| PUT | `/{id}/FinalizarServico` | Conclui os serviços executados | `EmExecucao` | Administrador, Mecânico | 204, 400, 401, 404, 500 |
| PUT | `/{id}/RegistrarEntrega` | Registra a entrega do veículo ao cliente | `Finalizada` | Administrador, Funcionário | 204, 400, 401, 404, 500 |
| PUT | `/{id}/Cancelar` | Cancela a ordem de serviço | `AguardandoAprovacao` | Administrador, Funcionário | 204, 400, 401, 404, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Criar OS):**
```json
{
  "veiculoId": "550e8400-e29b-41d4-a716-446655440000",
  "cpf": "51922594016",
  "cnpj": null,
  "observacao": "Cliente relatou barulho no freio",
  "valorDesconto": 0.0,
  "pecas": [
    {
      "pecaId": "e44ef901-efa5-4310-9297-00f58ab567c1",
      "quantidade": 1
    }
  ],
  "servicos": [
    {
      "servicoId": "e88776e6-b5f7-477c-aa90-031b05a8584d",
      "quantidade": 1
    }
  ]
}
```

**Request (Realizar Diagnóstico):**

> Além de registrar peças e serviços, este endpoint exige uma `observacao` com o resultado do diagnóstico. Substitui os itens da OS e move o status para `AguardandoAprovacao`.

```json
{
  "observacao": "Identificado desgaste nas pastilhas dianteiras e necessidade de troca de óleo",
  "pecas": [
    {
      "pecaId": "e44ef901-efa5-4310-9297-00f58ab567c1",
      "quantidade": 3
    }
  ],
  "servicos": [
    {
      "servicoId": "d122e4ab-5a8c-4805-b49e-02e84fcbdb93",
      "quantidade": 1
    }
  ]
}
```

**Request (Finalizar Serviço):**
```json
{
  "servicosId": [
    "e88776e6-b5f7-477c-aa90-031b05a8584d"
  ]
}
```

> **Fluxo de status da OS:**
>
> ```
> Recebida → EmDiagnostico → AguardandoAprovacao → EmExecucao → Finalizada → Entregue
>                                      ↓
>                                  Cancelada
> ```
>
> | Ação | Status exigido | Próximo status |
> |------|---------------|----------------|
> | `IniciarDiagnostico` | `Recebida` | `EmDiagnostico` |
> | `RealizarDiagnostico` | `EmDiagnostico` | `AguardandoAprovacao` |
> | `Aprovar` | `AguardandoAprovacao` | `EmExecucao` |
> | `Cancelar` | `AguardandoAprovacao` | `Cancelada` |
> | `FinalizarServico` | `EmExecucao` | `Finalizada` |
> | `RegistrarEntrega` | `Finalizada` | `Entregue` |
>
> Itens (peças, serviços e insumos) só podem ser editados enquanto a OS estiver nos status `Recebida` ou `EmDiagnostico`.

---

### 📊 EstoqueController - Gerenciamento de Estoque
**Base URL:** `/api/estoque`

| Método | Endpoint | Descrição | Autorização | Status |
|--------|----------|-----------|-------------|--------|
| GET | `/` | Lista movimentações de estoque (paginado) | Administrador, Funcionário, Mecânico, Almoxarifado | 206, 400, 401, 500 |
| GET | `/{id}` | Obtém movimentação de estoque por ID | Administrador, Funcionário, Mecânico, Almoxarifado | 200, 400, 401, 404, 500 |
| POST | `/` | Realiza movimentação de estoque | Administrador, Funcionário, Almoxarifado | 201, 400, 401, 500 |

**Query Parameters (Listagem):**
- `page` (int, default: 1): Número da página
- `pageSize` (int, default: 10): Quantidade de itens por página

**Request (Movimentação):**
```json
{
  "pecaId": null,
  "insumoId": "9e686be2-788f-4770-8698-02ef8ad72b52",
  "tipoMovimentacao": "Entrada",
  "quantidade": 10
}
```

> Os campos `pecaId` e `insumoId` são mutuamente exclusivos: informe apenas um deles por movimentação. O campo `tipoMovimentacao` aceita `"Entrada"` ou `"Saida"`.

---

## Perfis/Roles

Os seguintes perfis estão disponíveis no sistema:

| Valor | Nome (JWT) | Exibição | Descrição |
|-------|-----------|----------|-----------|
| 1 | `Administrador` | Administrador | Acesso total a todas as funcionalidades |
| 2 | `Funcionario` | Funcionário | Gerenciamento de clientes, veículos, serviços e ordens |
| 3 | `Mecanico` | Mecânico | Diagnóstico, finalização de serviços; visualização de peças, insumos, estoque e OS |
| 4 | `Almoxarifado` | Almoxarifado | Gerenciamento de peças, insumos e estoque |
| 5 | `Cliente` | Cliente | Visualização e aprovação de suas próprias ordens de serviço |

> **Atenção:** os nomes das roles no token JWT não possuem acentuação (`Funcionario`, `Mecanico`). Utilize exatamente esses valores ao configurar políticas de autorização ou ao inspecionar o token.

---

## Estrutura do Projeto

```
TechChallenge/
├── src/
│   ├── API/                    # Camada de apresentação (Controllers, DTOs, Dockerfile)
│   ├── Application/            # Camada de aplicação (Services, DTOs)
│   ├── Domain/                 # Camada de domínio (Entities, ValueObjects)
│   ├── Infra/                  # Camada de infraestrutura (Data, External Services)
│   └── Shared/                 # Código compartilhado (Utilities, Constants)
├── test/                       # Projetos de testes (Unit, Integration)
├── k8s/                        # Manifestos Kubernetes (Deployment, Service, HPA, ConfigMap, Secret, Job, Datadog Agent)
├── infra/terraform/            # Infraestrutura como Código (VPC, EKS, ECR, RDS)
├── .github/workflows/          # Pipelines de CI (ci.yml) e CD (cd.yml)
├── docs/                       # Site de documentação, collection Postman
└── docker-compose.yml          # Configuração do Docker Compose
```

---

## Notas Importantes

- A porta `8080` deve estar disponível para executar a aplicação
- Todos os endpoints requerem autenticação via JWT (exceto `GET /` e `POST /api/auth/login`)
- As senhas são armazenadas de forma segura (hash)
- Os campos `cpf` e `cnpj` em Cliente e OS são mutuamente exclusivos — informe apenas o documento aplicável
- A aplicação utiliza banco de dados relacional (verifique o `docker-compose.yml` para detalhes de conexão)
    -   Foi escolhido o SQL server por ser um dos bancos de dados relacionais mais robustos do mercado, e também pela alta sinergia que ele tem com o .Net 10, por conta das duas tecnologias serem da mesma empresa (Microsoft).
- Para testes, use as credenciais padrão: `Admin@email.com` / `12345678`
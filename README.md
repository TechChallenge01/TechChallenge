# TechChallenge API

## Descrição
API REST para gerenciamento de oficina mecânica, desenvolvida em .NET 10 com arquitetura em camadas (Domain, Application, Infrastructure, API).

## Pré-requisitos
- Docker
- Docker Compose
- (Opcional) .NET 10 SDK para desenvolvimento local

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

## Documentação da API

A API utiliza o **Scalar** como interface de documentação interativa. Com a aplicação em execução, acesse:
```
http://localhost:8080/scalar
```

Também está disponível uma collection do **Postman** com todos os endpoints e exemplos de request prontos para uso:
```
docs/collection/
```

Importe o arquivo no Postman e configure a variável `baseUrl` como `http://localhost:8080` para começar a usar.

---

## Autenticação
Todos os endpoints (exceto login) requerem um token JWT no header:
```
Authorization: Bearer <seu_token_jwt>
```

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
│   ├── API/                    # Camada de apresentação (Controllers, DTOs)
│   ├── Application/            # Camada de aplicação (Services, DTOs)
│   ├── Domain/                 # Camada de domínio (Entities, ValueObjects)
│   ├── Infra/                  # Camada de infraestrutura (Data, External Services)
│   └── Shared/                 # Código compartilhado (Utilities, Constants)
├── test/                       # Projetos de testes (Unit, Integration)
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
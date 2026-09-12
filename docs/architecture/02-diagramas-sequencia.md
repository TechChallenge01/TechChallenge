# Diagramas de Sequência

## 1. Autenticação por CPF

```mermaid
sequenceDiagram
    actor Cliente
    participant GW as API Gateway
    participant LA as Lambda auth
    participant RDS as RDS SQL Server

    Cliente->>GW: POST /auth/cpf { cpf }
    GW->>LA: invoca (rota publica, sem authorizer)
    LA->>RDS: SELECT Cliente WHERE Cpf = :cpf AND Ativo = 1
    alt CPF nao encontrado ou inativo
        RDS-->>LA: 0 linhas
        LA-->>GW: 404 / 401
        GW-->>Cliente: erro
    else CPF valido
        RDS-->>LA: dados do cliente (Id, Nome, Email)
        LA->>LA: gera JWT HS256\n(sub=ClienteId, role=Cliente,\niss/aud=TechChallenger, exp=+Nh)
        LA-->>GW: 200 { token, expiracao, clienteId, nome }
        GW-->>Cliente: 200 { token, ... }
    end
```

Chave simétrica (`Jwt:Key`) compartilhada entre a Lambda de auth e a API principal — ambas validam o mesmo token sem se comunicarem diretamente (ver [RFC-003](rfcs/RFC-003-estrategia-de-autenticacao.md)).

## 2. Abertura de Ordem de Serviço (rota protegida)

```mermaid
sequenceDiagram
    actor Cliente
    participant GW as API Gateway
    participant AUTHZ as Lambda Authorizer
    participant NLB as NLB interno
    participant POD as API principal (pod EKS)
    participant RDS as RDS SQL Server
    participant DD as Datadog Agent

    Cliente->>GW: POST /api/ordemServico\nAuthorization: Bearer <token>
    GW->>AUTHZ: REQUEST authorizer (identity source = header Authorization)
    AUTHZ->>AUTHZ: valida assinatura HS256,\niss/aud, expiracao
    alt token invalido/expirado
        AUTHZ-->>GW: isAuthorized=false
        GW-->>Cliente: 401 Unauthorized
    else token valido
        AUTHZ-->>GW: isAuthorized=true\ncontext={sub, cpf, role, name}
        GW->>NLB: proxy (VPC Link), header x-request-id
        NLB->>POD: NodePort 30080
        POD->>POD: RequireRole("Administrador","Funcionario")
        alt role sem permissao
            POD-->>Cliente: 403 Forbidden
        else role permitida
            POD->>RDS: valida Cliente/Veiculo/Pecas/Servicos,\nINSERT OrdemServico (EF Core)
            RDS-->>POD: OK
            POD->>DD: DogStatsD techchallenger.os.criadas++
            POD-->>NLB: 201 Created { id }
            NLB-->>GW: 201
            GW-->>Cliente: 201 Created
        end
    end
    POD-)DD: trace APM + log JSON (dd.trace_id)
```

Pontos-chave:
- O **Lambda authorizer** é o único ponto que valida o JWT — a API principal confia no `role`/`sub` que chegam via claims do próprio token (também validado independentemente pelo middleware de auth do ASP.NET Core, com a mesma chave).
- Toda requisição que chega ao pod gera **trace de APM** e **log estruturado em JSON** correlacionados (`dd.trace_id`), e falhas nos casos de uso de OS emitem a métrica `techchallenger.os.erros` (ver [ADR-004](adrs/ADR-004-observabilidade-hibrida.md)).

# RFC-003 — Estratégia de autenticação

**Status:** Aceito

## Contexto

A rubrica exige: (1) proteger rotas sensíveis com autenticação via CPF, e (2) uma Function Serverless que valide o CPF, consulte o status do cliente no banco e devolva um JWT válido para consumo das APIs protegidas — tudo atrás de um API Gateway. Havia ainda uma segunda fonte de identidade já existente: o login interno de staff (e-mail/senha) na API principal, usado por `Administrador`/`Funcionario`/`Mecanico`.

## Decisão

- **JWT assinado com HS256 (segredo simétrico compartilhado)**, com o mesmo `Jwt:Key`/`Issuer`/`Audience` usados tanto pela Lambda de autenticação por CPF quanto pelo login interno da API principal — qualquer um dos dois emissores gera um token que a API principal aceita sem precisar saber qual serviço o emitiu.
- **Validação na borda via Lambda authorizer customizado** (Node.js, sem dependências, usando `node:crypto`) no API Gateway, em vez do authorizer JWT nativo do API Gateway.

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Authorizer JWT nativo do API Gateway** | Suporta apenas RS256/JWKS (chave assimétrica com endpoint de chaves públicas). Adotar RS256 exigiria gerenciar um par de chaves e um endpoint JWKS só para satisfazer a ferramenta, sem benefício de segurança adicional dado que os dois emissores de token (Lambda e API principal) já confiam um no outro por estarem no mesmo domínio de aplicação — HS256 com o segredo como GitHub Secret/Terraform var é suficiente e mais simples de operar nos dois repositórios. |
| **OAuth2/OIDC com provedor externo (Cognito, Auth0)** | Adicionaria um serviço externo e um fluxo de autenticação completo (login, refresh token, etc.) para um requisito que é, na prática, "validar CPF e devolver token" — sobre-engenharia para o escopo pedido. |
| **Sessão/cookie no lugar de JWT** | Incompatível com a natureza stateless de Lambda + API Gateway e com múltiplos serviços validando a mesma identidade sem estado compartilhado. |

## Consequências

- Qualquer alteração em `Jwt:Key` precisa ser replicada nos dois repositórios (`TechChallenge` e `TechChallenger.auth`) ao mesmo tempo — documentado nos READMEs de ambos.
- O authorizer customizado só valida assinatura/`iss`/`aud`/expiração e repassa o `role`/`sub`/`cpf` como contexto; a **autorização por papel** (quem pode fazer o quê) continua sendo responsabilidade da API principal (`RequireRole(...)` por endpoint), não do authorizer — separação clara entre "quem é você" (borda) e "o que você pode fazer" (aplicação).
- Um cliente autenticado por CPF só enxerga/aprova as próprias ordens de serviço (checagem de `ClienteId` do token contra o dono do recurso) — ver [02-diagramas-sequencia.md](../02-diagramas-sequencia.md).

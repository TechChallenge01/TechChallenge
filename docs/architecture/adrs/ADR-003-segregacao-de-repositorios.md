# ADR-003 — Segregação em 4 repositórios com Terraform por domínio de infraestrutura

**Status:** Aceito (revisado)

## Contexto

A rubrica exige 4 repositórios separados, cada um com CI/CD próprio: (1) Function Serverless, (2) infraestrutura Kubernetes, (3) infraestrutura do banco de dados, (4) aplicação principal. Numa primeira iteração, por conveniência durante o desenvolvimento inicial, o Terraform de rede + cluster + banco + ECR + IAM foi escrito uma única vez e **copiado** para os repositórios `TechChallenger.k8s` e `TechChallenger.db` (e temporariamente também para dentro de `TechChallenge/infra/terraform`) — os três provisionavam, na prática, o mesmo conjunto de recursos.

## Decisão

Segregar de fato o Terraform por responsabilidade:

- **`TechChallenger.k8s`**: só rede (VPC, subnets, Internet Gateway) e cluster (EKS + node group + `metrics-server`) + ECR + IAM (`LabRole`).
- **`TechChallenger.db`**: só o RDS — subnet group, security group e a instância — descobrindo a VPC/EKS do repositório acima via `data source` (filtro por tag `Name` / nome do cluster), **sem** remote state nem acoplamento de backend entre os dois repositórios.
- **`TechChallenge`**: remove por completo o `infra/terraform/` duplicado — este repositório passa a conter só aplicação + manifestos Kubernetes.

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Manter o monólito duplicado** (como estava) | Viola diretamente o requisito da rubrica de repositórios segregados; além disso, gerava risco real de aplicar o mesmo Terraform em dois lugares e criar recursos duplicados/conflitantes na mesma conta AWS (o que de fato ocorreu durante o desenvolvimento). |
| **`terraform_remote_state` entre os repositórios** | Acopla os dois repos ao mesmo backend/formato de state — qualquer mudança estrutural em um poderia quebrar o outro. O padrão de `data source` por tag (já usado pelo `TechChallenger.auth` para achar VPC/EKS/RDS) é mais desacoplado: cada repo só depende de nomes/tags estáveis, não da estrutura interna do state alheio. |
| **Um único repositório de infraestrutura para k8s + db** | Não atenderia à rubrica, que pede explicitamente repositórios separados para "Infraestrutura Kubernetes" e "Infraestrutura do Banco de Dados Gerenciado". |

## Consequências

- Ordem de apply passa a ser explícita e documentada nos READMEs: `k8s` → `db` → (`TechChallenge` e `auth`, em qualquer ordem entre si, ambos dependendo dos dois primeiros).
- Cada repositório ganhou seu próprio backend S3 (mesmo bucket, `key` diferente por repositório) e pipeline de CI/CD (`ci.yml`/`cd.yml`) — item que também estava pendente e foi resolvido junto desta segregação.
- Recursos órfãos da era do monólito (ECR, subnet groups, security groups criados antes da segregação) precisaram ser importados (`terraform import`) ou limpos manualmente na primeira rodada de deploy pós-segregação — comportamento esperado ao migrar de um modelo para o outro, não uma falha recorrente.

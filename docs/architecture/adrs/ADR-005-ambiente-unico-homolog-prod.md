# ADR-005 — Ambiente único para a infraestrutura base (`k8s`/`db`); homologação e produção reais só na aplicação e no serverless

**Status:** Aceito

## Contexto

A rubrica pede "deploy automático das branches de homologação e produção" nos 4 repositórios. Nos repositórios de **aplicação** (`TechChallenge`) e **serverless** (`TechChallenger.auth`), isso foi implementado literalmente: o `cd.yml` dispara tanto em push na `release` (homologação) quanto na `main` (produção). Já nos repositórios de **infraestrutura base** (`TechChallenger.k8s` e `TechChallenger.db` — VPC, cluster EKS, RDS), o `cd.yml` dispara **só** em push na `main`.

## Decisão

`TechChallenger.k8s` e `TechChallenger.db` mantêm **um único ambiente** (só `main`, sem trigger de `release`). `TechChallenge` e `TechChallenger.auth` mantêm os dois gatilhos (`release` → homologação, `main` → produção), compartilhando esse único cluster/RDS de base.

## Justificativa

- Duplicar VPC + EKS (control plane cobrado por hora) + RDS para ter um ambiente de homologação separado do de produção dobraria o consumo do orçamento fixo do AWS Academy Learner Lab (~US$50), inviabilizando múltiplas rodadas de teste ao longo do projeto.
- A infraestrutura **base** (rede, cluster, banco) muda raramente e de forma estrutural (adicionar um recurso, ajustar uma variável) — o risco de uma mudança direto em produção é mitigado por PR obrigatório + `terraform plan` revisado no CI, não por um ambiente de homologação espelhado.
- Onde a rubrica mais se beneficia de homologação real — testar uma nova versão da **aplicação** ou da **função serverless** antes de ir para produção — já está coberto: `TechChallenge` e `TechChallenger.auth` de fato promovem `release → main` como estágios distintos.

## Alternativas consideradas

| Opção | Por que não |
|---|---|
| **Segundo cluster/RDS completo para homologação** | Dobra o custo de infraestrutura de base rodando o tempo todo — incompatível com o orçamento fixo do lab. |
| **Namespace separado no mesmo cluster para "homolog"** | Resolveria parcialmente para a aplicação (já não é o caso, pois o app tem seu próprio `release`/`main`), mas não faz sentido para VPC/EKS/RDS — não há "namespace" de rede ou de banco gerenciado nesse nível. |

## Consequências

- Documentado explicitamente nos READMEs de `TechChallenger.k8s` e `TechChallenger.db` e nos comentários dos respectivos `cd.yml`, para não passar como uma omissão silenciosa da rubrica.
- Qualquer alteração na infraestrutura base afeta imediatamente o único ambiente existente — reforça a necessidade de `terraform plan` sempre revisado antes do merge (branch `main` protegida, PR obrigatório).

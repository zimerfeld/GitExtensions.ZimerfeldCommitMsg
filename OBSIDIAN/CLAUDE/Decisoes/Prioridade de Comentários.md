---
tipo: decisão
tags: [decisão, comentários, prioridade, diff, ranqueamento]
status: estável
---

# Decisão: Prioridade de Comentários por Tipo de Arquivo

## Contexto

O plugin extrai comentários das linhas do `git diff --cached`. Quando há múltiplos arquivos com comentários, qual deve aparecer no título do commit?

## Decisão

**Ranquear os comentários por tipo de arquivo (categoria semântica), priorizando código-fonte sobre config/docs.**

## Sistema de prioridade

| Categoria | Prioridade | Raciocínio |
|---|---|---|
| Testes (qualquer) | 0 | Comentários de teste raramente descrevem a mudança de negócio |
| `config` / `docs` | 1 | Mudanças de suporte, não o core da alteração |
| `build` | 2 | Infraestrutura, geralmente não é o foco |
| `web` (.js, .ts, .html…) | 3 | Frontend — relevante, mas abaixo de lógica de negócio |
| `source` (.cs, .py, .go…) | 4 | Maior prioridade — onde a lógica de negócio vive |

**Linhas removidas** têm prioridade -1 em relação às adicionadas do mesmo nível (contexto do que mudou, não o que foi adicionado).

## Implementação

```csharp
// Detecta arquivo atual no diff:
if (line.StartsWith("+++ b/"))
    filePriority = CommentFilePriority(line[6..].Trim());

// Comentário adicionado vs. removido:
int priority = isAdded ? filePriority : Math.Max(0, filePriority - 1);
```

`SortedDictionary<int, List<string>>` em ordem decrescente → os buckets de maior prioridade vêm primeiro.

Dentro do mesmo bucket: **comentários mais longos primeiro** (comprimento como proxy de valor descritivo).

## Limites

- Máx. 15 linhas processadas por diff
- Máx. 5 comentários no resultado final
- Comentário mínimo: 10 chars e pelo menos 1 espaço (deve ser frase, não código)

## Exemplo

Staged: `PaymentService.cs` (source, prio=4) + `appsettings.json` (config, prio=1)

```
+++ b/src/Services/PaymentService.cs
+ // Processa pagamentos com validação de fraude
+++ b/appsettings.json
+ # URL do gateway de pagamento
```

Resultado: comentário do `.cs` vence → `fix, chore\n\nprocessa pagamentos com validação de fraude`

## Relacionado

- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[Estratégia de Detecção de Idioma]]

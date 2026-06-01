---
tipo: decisão
tags: [decisão, título, conventional-commits, types, branch]
branch: feature/titulo
data: 2026-05-22
status: implementada
---

# Decisão: Título como Lista de CC Types

## Contexto

O formato original do commit gerado era:

```
<type>: <descrição em pt-BR>

[corpo]
```

Exemplos anteriores:
```
feat: adicionar gerenciamento de usuários
fix: corrigir processamento de pagamento
```

## Decisão

**Substituir o título pela lista de todos os Conventional Commit types envolvidos nas mudanças, separados por vírgula.**

Novo formato:
```
<type1>, <type2>, ...

[descrição, se houver]

[corpo arquitetural]
```

Exemplos após a mudança:
```
feat, docs, chore

filtrar stems com ponto

Abrange autenticação nas camadas de serviço e repositório.
```

## Motivação

Quando um commit envolve múltiplos tipos de mudanças (ex: novo código + documentação + config), o formato `<single_type>: <desc>` forçava escolher um único tipo dominante, perdendo informação. A lista comunica com precisão o que foi alterado.

## Implementação

### Novo método: `DetermineAllTypes(List<FileChange>) → List<string>`

```
Para cada arquivo:
  IsTestPath?         → "test"
  categoria "docs"    → "docs"
  categoria "build"   → "build"
  categoria "config"  → "chore"
  status A/C          → "feat"
  status D            → "chore"
  status M/R/T        → "fix"
  outro               → "refactor"

Deduplicar + ordenar por prioridade convencional:
feat → fix → refactor → perf → test → build → ci → chore → docs → style
```

### Mudança em `Generate()`

```csharp
// ANTES:
var type   = DetermineType(changes);
var header = TruncateTitle($"{type}: {desc}");

// DEPOIS:
var types  = DetermineAllTypes(changes);
var header = TruncateTitle(string.Join(", ", types));
```

A `desc` (quando vinda de comentários do diff ou README) migra para o corpo, não para o título.

No fallback (sem comentários, sem README): `desc = string.Empty`, apenas o `BuildBody()` vai para o corpo.

### Preservação do tipo primário

`var type = types[0]` — mantido para `BuildSubject()` (verbo em pt-BR), mas não mais usado no header.

## Trade-offs

| Aspecto | Antes | Depois |
|---|---|---|
| Informação no título | Único type + descrição | Todos os types |
| Descrição | No título | No corpo |
| Commits simples (1 type) | `feat: adicionar X` | `feat` |
| Commits mistos | `feat: adicionar X` (info perdida) | `feat, docs, chore` |

## Arquivos modificados

- `CommitMessageGenerator.cs` — método `DetermineAllTypes` adicionado (~L698), `Generate()` alterado

## Relacionado

- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]

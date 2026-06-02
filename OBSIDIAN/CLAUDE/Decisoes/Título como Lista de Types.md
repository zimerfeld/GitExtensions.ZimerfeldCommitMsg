---
tipo: decisão
tags: [decisão, título, conventional-commits, types, branch]
branch: feature/titulo
data: 2026-05-22
revisado: 2026-06-02
status: revisada
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
feat, docs, chore: filtrar stems com ponto

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

### Mudança em `Generate()` (implementação original)

```csharp
// ANTES (type único):
var type   = DetermineType(changes);
var header = TruncateTitle($"{type}: {desc}");

// DEPOIS (lista de types, desc no corpo):
var types  = DetermineAllTypes(changes);
var header = TruncateTitle(string.Join(", ", types));
```

### ⚠️ Revisão 2026-06-02 — Descrição de volta à primeira linha

A `desc` voltou para a primeira linha junto com os types, separada por `: `, seguindo o padrão Conventional Commits:

```csharp
var typeStr = string.Join(", ", types);
var title   = TruncateTitle(desc.Length > 0 ? $"{typeStr}: {desc}" : typeStr);
return body.Length > 0 ? $"{title}\n\n{body}" : title;
```

**Motivação da revisão:** a spec em `FUNCIONALIDADES.md` sempre definiu `<tipo>: <descrição>` na primeira linha. A versão anterior criava uma linha só com os types e a descrição num parágrafo separado, quebrando o formato Conventional Commits.

**Fallback corrigido:** quando não há comentários nem README, `desc = BuildSubject(type, changes)` — antes ficava vazio, gerando commits sem descrição.

## Trade-offs

| Aspecto | Antes (type único) | Implementação original | Estado atual (2026-06-02) |
|---|---|---|---|
| Tipos no título | Único | Todos, separados por `, ` | Todos, separados por `, ` |
| Descrição | No título | Parágrafo separado | Na primeira linha (após `: `) |
| Commits simples | `feat: adicionar X` | `feat\n\nadicionarX` | `feat: adicionar X` |
| Commits mistos | `feat: adicionar X` (info perdida) | `feat, docs, chore\n\ndescrição` | `feat, docs, chore: descrição` |

## Arquivos modificados

- `CommitMessageGenerator.cs` — método `DetermineAllTypes` adicionado (~L698), `Generate()` alterado

## Relacionado

- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]

---
tipo: fluxo
tags: [fluxo, generate, pipeline, conventional-commits]
atualizado: 2026-06-02
---

# Fluxo: Geração da Mensagem de Commit

Pipeline executado por `CommitMessageGenerator.Generate()`.

## Visão macro

```
git diff --cached --name-status
        │
        ▼
ParseChanges() → List<FileChange>
        │
        ├──► DetermineAllTypes()     → List<string> types   (ex: ["feat","docs"])
        │
        ├──► ReadStagedReadmeTitle() → string? titulo do README
        │
        ├──► ExtractDiffComments()   → comentários do diff ranqueados
        │       └──► TranslateToPortuguese()
        │
        └──► Montagem
                title  = TruncateTitle(FormatTitle(type, changes, desc))  // <Verbo> <desc> — SEM "tipo:"
                output = body.Length > 0 ? title + "\n\n" + body : title
```

> ⚠️ **Sem prefixo `tipo:`.** Apesar de `DetermineAllTypes` retornar uma lista, só `types[0]`
> é usado — e apenas para escolher o **verbo** em `FormatTitle`/`TypeVerb`. Os tipos não são
> impressos (a decisão [[../Decisoes/Título como Lista de Types]] foi superada em 2026-06-05).

## Passo 1 — ParseChanges

Lê `git diff --cached --name-status`. Cada linha tem formato:
```
M    src/Auth/UserService.cs
A    src/Auth/TokenService.cs
D    src/Old/LegacyService.cs
R100 src/Old.cs\tsrc/New.cs
```

Produz `List<FileChange(char Status, string Path)>`.

Status relevantes: `A`=added, `M`=modified, `D`=deleted, `C`=copied, `R`=renamed, `T`=type-changed.

## Passo 2 — DetermineAllTypes *(branch feature/titulo)*

Para **cada** arquivo, classifica em um CC type:

```
IsTestPath?         → "test"
categoria "docs"    → "docs"
categoria "build"   → "build"
categoria "config"  → "chore"
status A/C          → "feat"
status D            → "chore"
status M/R/T        → "fix"
outro               → "refactor"
```

Retorna lista de types únicos, ordenados:
`feat → fix → refactor → perf → test → build → ci → chore → docs → style`

Exemplo: `["feat", "docs", "chore"]` → **só `types[0]` (`feat`) é usado**, para escolher o verbo.

## Passo 3 — Estratégia de descrição (prioridade decrescente)

### 3a. README staged com título `#`
Se `README.md` está staged e tem linha `# Título`, usa o título como `desc`.

### 3b. Comentários do diff (estratégia principal)
`git diff --cached --no-color` → linhas `+` (added) e `-` (removed) que são comentários.

**Ranqueamento por tipo de arquivo:**
| Categoria | Prioridade |
|---|---|
| `source` (`.cs`, `.py`, etc.) | 4 (maior) |
| `web` (`.js`, `.ts`, `.html`) | 3 |
| `build` / `config` / `docs`   | 1–2 |
| Arquivos de teste              | 0 (menor) |

Linhas removidas têm prioridade -1 em relação às adicionadas do mesmo arquivo.

**Filtros de comentário rejeitado:**
- Separadores visuais (`─────`, `===`)
- Tags XML (`<summary>`, `<param>`)
- Código comentado (tem `{` `}` ou chamada de método)
- Muito curto (< 10 chars) ou sem espaço

Máximo: 5 comentários, 15 linhas processadas.

**Tradução en→pt-BR:**
1. Frases compostas (longest-match first): `"doesn't match"` → `"não corresponde"`
2. Padrões estruturais: `X-based` → `baseado em X`
3. Palavras individuais: `returns` → `retorna`
4. Se ainda >25% inglês após tradução → descarta (qualidade insuficiente)

O primeiro comentário (mais impactante) vira `desc`. Os demais vão para o body como marcadores `- item`.

**ExtractMainClause:** separa `desc` no primeiro conector de propósito (` para `, ` pois `, ` porque `, ` — `) para manter só a cláusula principal.

### 3c. Fallback (sem comentários, sem README)
`desc = BuildSubject(type, changes)` → frase funcional do conceito dominante, **sem verbo** (ex.: `"autenticação"`, `"gerenciamento de usuários"`). O verbo é adicionado depois por `FormatTitle` → `"Implementa autenticação"`, `"Corrige gerenciamento de usuários"`.

## Passo 4 — Corpo em bullets (BuildBody)

Ativo quando há **2+ arquivos**. Ordena por relevância do arquivo (`CommentFilePriority`) e gera até **5 bullets** `- <StatusVerb> <conceito>`, com `Distinct`.

**Extração de conceito por arquivo:** filename sem extensão → remove prefixo `I` de interface → remove sufixo arquitetural (`Service`, `Repository`, `Controller`…) → `MapConcept` para frase no idioma ativo.

**StatusVerb:** `A`/`C` → Adiciona/Add · `D` → Remove/Remove · `R` → Renomeia/Rename · demais → Atualiza/Update.

Exemplo de body gerado:
```
- Adiciona autenticação
- Adiciona gerenciamento de usuários
```
*(O antigo corpo em prosa "Abrange … nas camadas …" — e `ArchLayers`/`JoinPhrases` — foram removidos.)*

## Passo 5 — Montagem final

```
title  = TruncateTitle(FormatTitle(type, changes, desc))   → ex: "Implementa autenticação"
output = body.Length > 0 ? title + "\n\n" + body : title
```

- `FormatTitle`: se `desc` começa com verbo conhecido (`LeadingVerb`), normaliza-o; senão prefixa `TypeVerb(type, …)`. **Não** acrescenta `tipo:`.
- `TruncateTitle` limita a 72 chars, cortando no último espaço e adicionando `…`.

Formato final (verbo + objeto, sem prefixo de tipo):
```
Implementa autenticação

- Adiciona autenticação
- Adiciona gerenciamento de token
```

## Exemplos de saída

| Staged | Saída |
|---|---|
| `AuthService.cs` adicionado | `Implementa autenticação` |
| `UserService.cs` modificado | `Corrige gerenciamento de usuários` |
| `README.md` modificado | `Atualiza documentação` |
| `UserService.cs` + `TokenService.cs` novos | `Implementa gerenciamento de usuários`<br>`- Adiciona gerenciamento de usuários`<br>`- Adiciona gerenciamento de token` |
| `.cs` com comentário `// filtra stems com ponto` | `Filtra stems com ponto` |

## Relacionado

- [[../Sistema/Arquitetura]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[../Decisoes/Título como Lista de Types]]
- [[../Decisoes/Prioridade de Comentários]]

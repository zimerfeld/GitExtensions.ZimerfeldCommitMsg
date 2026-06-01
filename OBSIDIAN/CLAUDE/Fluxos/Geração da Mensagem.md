---
tipo: fluxo
tags: [fluxo, generate, pipeline, conventional-commits]
atualizado: 2026-05-22
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
                header = Join(types, ", ")   ← título é a lista de types
                body   = desc + corpo arquitetural
```

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

Exemplo: `["feat", "docs", "chore"]`

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
`desc = ""` — o body é gerado só com a análise arquitetural.

## Passo 4 — Corpo arquitetural (BuildBody)

Ativo quando há 2+ arquivos com conceitos/camadas distintos.

**Extração de conceitos:** filename sem extensão → remove prefixo `I` de interface → remove sufixo arquitetural (`Service`, `Repository`, `Controller`…) → mapeia para frase pt-BR.

**Extração de camadas:** sufixos como `Service`→`serviço`, `Repository`→`repositório`, `Controller`→`controlador`.

Exemplo de body gerado:
```
Abrange autenticação e gerenciamento de usuários nas camadas de serviço e repositório.
```

## Passo 5 — Montagem final

```
header  = TruncateTitle( Join(types, ", ") )   → ex: "feat, docs, chore"
fullBody = desc + "\n\n" + body                → ex: "filtrar stems com ponto\n\nAbrange..."
resultado = header + "\n\n" + fullBody
```

`TruncateTitle` limita a 72 chars, cortando no último espaço e adicionando `…`.

## Exemplos de saída

| Staged | Saída |
|---|---|
| `UserService.cs` adicionado | `feat` |
| `README.md` modificado | `docs` |
| `.cs` modificado + `.yml` alterado | `fix, chore` |
| `.cs` novo + `.md` + `appsettings.json` | `feat, docs, chore` |
| `.cs` com comentário `// filtrar stems` staged | `feat\n\nfiltrar stems` |

## Relacionado

- [[../Sistema/Arquitetura]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[../Decisoes/Título como Lista de Types]]
- [[../Decisoes/Prioridade de Comentários]]

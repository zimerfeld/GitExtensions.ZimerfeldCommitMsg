---
tipo: conhecimento
criado: 2026-06-01
atualizado: 2026-06-13
tags: [conhecimento, conventional-commits, git, algoritmo, i18n]
---

# Geração de mensagem — verbo guiado por Conventional Commits

> Lógica em `CommitMessageGenerator.cs` do projeto [[GitExtensions.ZimerfeldCommitMsg]]. Espelho do README em [[README — Instalação, Uso e Build]]. Detalhes de fluxo em [[Geração da Mensagem]].

## Formato gerado
```
<Verbo> <descrição no idioma ativo>

- <bullet 1>
- <bullet 2>
```
- **Sem prefixo `tipo:`** — `Generate()` retorna `FormatTitle(type, changes, desc)` = `<Verbo> <descrição>`. O tipo CC **só escolhe o verbo**; não é impresso. (A decisão [[Título como Lista de Types]] que imprimia `feat, docs: …` foi **superada** em 2026-06-05 — ver [[2026-06-05 - Formato imperativo pt-BR]].)
- **Sem scope**; **sem cor** (`git diff --no-color`).
- Primeira linha ≤ 72 chars (`TruncateTitle`).
- Descrição, verbos e bullets no idioma resolvido (pt-BR ou inglês), via `LanguagePack`.
- **Nunca vazio com stage não-vazio:** `Generate` garante ao menos a linha-resumo (`<verbo> N arquivos`) caso todo o resto falhe. Stage realmente vazio → `string.Empty` (correto).

## Tipos detectados (`DetermineAllTypes`) — definem o verbo
Cada arquivo recebe um tipo; o verbo vem do tipo de maior prioridade (`types[0]`):
| Tipo | Critério (por arquivo) |
|---|---|
| `test` | caminho de teste (pasta `test`/`tests`/`spec`/`__tests__` ou sufixo `Test`/`Spec`) |
| `docs` | categoria docs (`.md`, `.txt`, `.rst`, `.adoc`) |
| `build` | categoria build (`.csproj`, `.sln`, `Dockerfile`, …) |
| `chore` | categoria config (`.json`, `.yml`, …) **ou** status `D` (deletado) |
| `feat` | status `A`/`C` (adicionado) em código |
| `fix` | status `M`/`R`/`T` (modificado/renomeado) em código |
| `refactor` | demais casos |

Ordem de prioridade: `feat → fix → refactor → perf → test → build → ci → chore → docs → style`.

### Verbo por tipo (`LanguagePack.TypeVerb`)
| Tipo | pt-BR | en |
|---|---|---|
| feat (só adições / misto) | Implementa / Adiciona | Implement / Add |
| fix | Corrige | Fix |
| refactor | Refatora | Refactor |
| docs (com adições / só mod.) | Documenta / Atualiza | Document / Update |
| build · ci | Configura | Configure |
| chore (com deleções / sem) | Remove / Configura | Remove / Configure |
| test | Adiciona | Add |
| perf | Otimiza | Optimize |
| style | Padroniza | Standardize |

Se a `desc` já começa com verbo conhecido (`LeadingVerb`), ele é normalizado (pt-BR: 3ª pessoa `filtrar`→`Filtra`; en: imperativo) em vez de prefixar outro.

## Estratégia 1 — comentários do diff (principal)
1. `git diff --cached --no-color`.
2. Coleta linhas de **comentário adicionadas (`+`) e removidas (`-`)** (`//`, `///`; `#` fora de `.md`). Removidas entram com prioridade um grau menor.
3. **Rejeita** ruído: < 10 chars, sem espaço, separadores visuais (`// ─────`), tags XML (`<summary>`), código comentado (`{`/`}` ou chamada `metodo(arg)`), **delimitadores desbalanceados** (`DelimitersBalanced`: `()`/`[]`/`{}`/aspas `"` `'` `` ` ``/`<>`) e frases que **terminam em palavra de ligação solta** (`IsCleanSentence` + `DanglingTrailingWords`).
4. Prioriza por categoria do arquivo (source=4 > web=3 > build=2 / config=1 / docs=1; teste=0); até 5 comentários (um por arquivo).
5. Por arquivo, escolhe o comentário de maior **score** (`ScoreCandidate`: comprimento ~20–72 + início por verbo; penaliza resíduo de código) — não mais o simplesmente mais longo. Esse vira a `desc`; os demais viram bullets `- <FormatTitle>`.

```
Valida o token antes de processar a requisição

- Filtra requisições sem cabeçalho de autenticação
```

> Se o corte de `ExtractMainClause` encurtou o 1º comentário, a `desc` passa a usar a frase funcional da Estratégia 2 e **todos** os comentários vão para os bullets.

## Estratégia 2 — nomes de arquivo (fallback)
Quando não há comentário válido:
1. Stem com `.` ou não-ASCII → ignorado.
2. Remove prefixo de interface (`IUserService` → `UserService`).
3. Remove sufixo arquitetural (maior primeiro): `ServiceTests`, `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator`, `Helper`, `Provider`, `Factory`, `Builder`, `Middleware`, `Validator`, `Mapper`, `Dto`, `ViewModel`, `Config`, `Settings`, `Adapter`, `Client`, `Impl`, …
4. < 2 chars → rejeitado; fora do dicionário e > 2 palavras PascalCase → rejeitado.
5. `MapConcept` → frase pt-BR/EN (`Auth`→autenticação, `User`→gerenciamento de usuários, `Payment`→processamento de pagamento, `CommitMessage`→mensagem de commit, …).

Subject = conceito **dominante** (mais frequente). 

## Corpo (`BuildBody`)
Com 2+ arquivos, até 5 bullets `- <StatusVerb> <conceito>`, ordenados por relevância do arquivo, `Distinct`:
```
- Adiciona autenticação
- Adiciona gerenciamento de token
```
`StatusVerb`: `A`/`C` → Adiciona/Add · `D` → Remove/Remove · `R` → Renomeia/Rename · demais → Atualiza/Update.
*(O antigo corpo em prosa "Abrange … nas camadas …" foi removido quando o corpo virou bullets.)*

## Tradução EN→PT
Só roda quando o idioma de saída é **pt-BR**; em inglês os comentários passam intactos. Descarta a tradução se restar > 25% de inglês (usa fallback). **Nomes de branch** (gitflow) e **tipos Conventional Commits** são preservados intactos. Ver [[Geração da Mensagem]] e `Decisoes/Preservação de Branches e Tipos CC`.

## 🔗 Relacionado
- [[GitExtensions.ZimerfeldCommitMsg]]
- [[README — Instalação, Uso e Build]]
- [[Plugin MEF para GitExtensions]]
- [[Título como Lista de Types]] — decisão superada

---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [fluxo, geração, diff, conventional-commits, etapa2]
---

# ⚙️ Flow: Message generation

> 🇧🇷 Português → [[⚙️ 2 - Geração da mensagem]] · 🇪🇸 Español → [[⚙️ 2 - Geração da mensagem (ES)]]

What `CommitMessageGenerator.Generate()` does to turn the staged diff into a message.

## 🪜 Steps

```
Generate()
        │
        ├─ 1. Reads the staged diff: `git diff --cached` in the working dir
        │       (nothing staged → returns only the summary line with the context prefix)
        │
        ├─ 2. Classifies each file:
        │       extension → category (source/web/docs/build/config)
        │       + content heuristic → Conventional Commits type
        │       (feat/fix/docs/test/chore/build/refactor)
        │
        ├─ 3. DetermineAllTypes(changes) → list of involved types (dominant first)
        │
        ├─ 4. Consolidated subject: BuildConsolidatedTitle(dominantType, changes, types)
        │       = [context prefix] - <type verb> N files (type1, type2, …)
        │       e.g.: "Overlay - Adiciona 10 arquivos (feat, fix, docs)"
        │
        ├─ 5. Body: BuildBody(changes, commentsPerFile, readmeTitle)
        │       up to 5 bullets, one per highest-impact file, each the best
        │       comment sentence from the diff (or the concept from the file name)
        │
        └─ 6. Never empty: with something staged, always at least the summary line
```

## 🔎 Details

- **Verb per type (Conventional Commits):** the type picks the **verb**, it does not become a prefix. `feat`→"Adiciona/Add", `fix`→"Corrige/Fix", `docs`→"Documenta/Document", etc. The type does **not** appear in the message (only in parentheses in the summary, as a hint). See [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject without type prefix]].
- **Context prefix:** 1-3 words derived from the **concept** of the highest-impact file (e.g.: `OverlayController` → `Overlay`), so the title is never generic. `null` when no file yields a readable concept.
- **Main strategy (comments):** `ExtractCommentsByFile()` reads the diff's `+`/`-` lines and captures comments (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). Each file picks the **best** comment (translated to pt-BR when applicable, sanitized, trimmed to the main clause).
- **Fallback strategy (file names):** when there is no useful comment, derives the **concept** from the name by removing semantic suffixes (`Service`, `Controller`, `Tests`, …) and translating words.
- **Sanitization:** discards comments with **unbalanced delimiters** (`()`, `[]`, `{}`, quotes, `<>`) or ending in a **dangling connective**; among valid ones, picks the best quality (not the longest).
- **README:** if `README.md` is staged, its title (first `#` line) is used as a descriptive fallback.

See [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]].

## 🌐 Example (pt-BR / EN)

```
Implementa autenticação
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```
```
Implement authentication
- Add authentication
- Add payment processing
- Add token management
```

## 🔗 Links

- [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]]
- [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]

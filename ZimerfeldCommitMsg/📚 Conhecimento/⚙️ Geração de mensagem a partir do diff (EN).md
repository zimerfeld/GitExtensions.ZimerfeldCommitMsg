---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [conhecimento, diff, extração, comentários, saneamento, tradução]
---

# ⚙️ Message generation from the diff

## 📝 Summary
How the [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] turns `git diff --cached` into a commit message: classification → consolidated subject → bulleted body, with comment extraction, concept derivation, sanitization and translation.

## 🔄 Pipeline

```
git diff --cached
   │
   ▼
FileChange[]  (path, +/- lines)
   │
   ├─ extension category (source/web/docs/build/config)
   ├─ Conventional Commits type (dominant first)
   │
   ├─ SUBJECT: [context prefix] - <verb> N files (types)
   │      prefix = concept of the highest-impact file (e.g. OverlayController → Overlay)
   │
   └─ BODY: up to 5 bullets, one per highest-impact file
          strategy 1: best diff comment (translated, sanitized, trimmed)
          strategy 2 (fallback): concept from the file name
```

## 💬 Comment extraction

`ExtractCommentText` recognizes:
- `//`, `///` — C#, Java, JS, TS, Go, Rust, C/C++…
- `/* */`, `/** */`, `/*! */` — one-line C-style block
- `* ` — JSDoc/Javadoc continuation (outside `.md`, where `*` is a bullet)
- `<!-- -->` — HTML/XML
- `--` — SQL, Lua, Haskell, Ada
- `'` — VB/VBScript (outside `.md`)
- `#` — Python, Shell, YAML, Ruby (in `.md` it is a heading, ignored)

## 🧹 Sanitization (`CleanCommentText`)

Discards:
- **Visual separators** and commented-out code.
- **Unbalanced delimiters** — unmatched parentheses/brackets/braces; quotes `"`/`` ` ``/`'` in odd count (ignores contraction apostrophes like `don't`, `it's`); `<`/`>` in unequal count.
- Phrases ending in a **dangling connector word** (`de`, `para`, `que`…) — truncated comment.

Among valid candidates it picks the one of **best quality** (most descriptive), not the longest.

## 🌐 EN→PT translation

When the output is pt-BR, phrases and verbs are translated by dictionaries (longest first), with **protection** of gitflow branch slugs (`feature/…`) and CC types via regex — so `feature/search` is not corrupted into `feature/buscar`. In English, comments pass through intact.

## 🔡 Concept derivation (fallback)

`SemanticSuffixes` removes suffixes like `Service`, `Controller`, `Repository`, `Tests`, `ViewModel`, … to reach the name's concept. E.g. `OverlayController` → `Overlay`. Known/rejected vocabulary and concept phrases come from the defaults + [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]].

## ✅ Guarantees

- **Never empty:** with a stage present, always at least the summary line.
- **Always at least one bullet**, even with a single file.
- **Staged README:** the title (`#`) serves as a descriptive fallback.

## 🔗 Related
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies: diff comments + file names]]
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]

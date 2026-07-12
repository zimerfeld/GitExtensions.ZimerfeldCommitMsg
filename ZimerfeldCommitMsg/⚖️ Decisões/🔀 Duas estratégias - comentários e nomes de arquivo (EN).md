---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [decisao, adr, extração, comentários, nomes-de-arquivo]
status: aceita
---

# 🔀 ADR — Two strategies: diff comments + file names

> 🇧🇷 Português → [[🔀 Duas estratégias - comentários e nomes de arquivo]] · 🇪🇸 Español → [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)]]

## 🎯 Context
To describe **what** changed, the generator needs a content source. Simple generators use only file names, which produces generic messages ("update Service"). On the other hand, not every diff carries useful comments.

## ✅ Decision
Use **two complementary strategies**, prioritized:
1. **Diff comments (primary):** read the `+`/`-` lines of `git diff --cached` and extract comments in several syntaxes (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). The best comment per file is translated (when pt-BR), sanitized and trimmed to the main clause.
2. **File names (fallback):** when there is no useful comment, derive the **concept** from the name by removing semantic suffixes (`Service`, `Controller`, `Tests`, …) and translating words.

Both rely on the **per-repository vocabulary** (`.zimerfeldcommitmsg.json`) for domain terms. See [[📓 Vocabulário por repositório (EN)|Per-repository vocabulary]].

## 🔀 Alternatives considered
- **File names only** — cheap, but generic and uninformative.
- **Comments only** — rich when present, but fails on diffs without comments.
- **Two strategies with fallback (chosen)** — covers both cases; the real diff content is favored, with a floor derived from names.

## ⚖️ Consequences
**Positive:**
- More informative messages when the developer comments the code.
- Never left without content — there is always at least the summary line and one bullet.

**Negative / trade-offs:**
- Comment extraction requires robust **sanitization** (balanced delimiters, dangling connectors) so it does not inject noise.
- EN→PT translation is heuristic (dictionaries), not perfect.

## 🔗 Related
- [[📓 Vocabulário por repositório (EN)|Per-repository vocabulary]]
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]

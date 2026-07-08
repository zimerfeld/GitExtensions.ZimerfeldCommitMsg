---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [decisao, adr, conventional-commits, mensagem, verbo]
status: aceita
---

# ✍️ ADR — Verb-first subject, without the literal "type:" prefix

## 🎯 Context
The **Conventional Commits** standard defines types (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) usually written as a literal prefix: `feat: add authentication`. The plugin classifies changes by these types, but must decide **how** that appears in the message.

## ✅ Decision
Use the Conventional Commits type to **choose the verb**, and **not** to prefix the message with `type:`. The subject starts with the corresponding **verb** (3rd person present in pt-BR / imperative in English). The type itself only appears, as a hint, in parentheses on the consolidated summary line.

Examples:
- `feat` → `Adiciona autenticação` / `Add authentication`
- `fix` → `Corrige …` / `Fix …`
- Consolidated summary: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`

## 🔀 Alternatives considered
- **Literal `feat:` prefix** — canonical CC, but noisy and less natural in pt-BR; many teams already generate that prefix via tools/hooks.
- **Verb only, no types (chosen)** — readable, natural message, guided by the same semantic classification; the types stay visible only in the parenthesized summary.

## ⚖️ Consequences
**Positive:**
- Natural, bilingual messages, without prefix pollution.
- Still **guided** by the Conventional Commits classification (the verb carries the intent).

**Negative / trade-offs:**
- It is not the literal CC format — anyone requiring `type:` in the subject must adjust (the type appears only in the summary).
- Quality depends on the type → verb mapping in the `LanguagePack`.

## 🔗 Related
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]

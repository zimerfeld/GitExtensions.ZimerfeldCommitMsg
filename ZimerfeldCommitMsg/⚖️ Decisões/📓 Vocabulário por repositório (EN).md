---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [decisao, adr, vocabulário, configuração, json]
status: aceita
---

# 📓 ADR — Per-repository vocabulary (.zimerfeldcommitmsg.json)

> 🇧🇷 Leia esta página em português → [[📓 Vocabulário por repositório]]

## 🎯 Context
Every project has its own jargon: domain names ("widget", "overlay") that should become concepts, and proper names/namespaces ("Acme", "Contoso") that should **not**. Embedding all that vocabulary in the plugin would require recompiling for each project.

## ✅ Decision
Read an **optional** `.zimerfeldcommitmsg.json` file at the working-dir root, with three lists added to the built-in defaults:
- `knownVocabulary` — words accepted as part of a descriptive name.
- `rejectedVocabulary` — words that force rejecting the name as a concept.
- `concepts` — concept-word → pt-BR phrase translation (priority over the built-in dictionary).

Read/parse failures are **silent** (empty config) — they never break generation. See [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]].

## 🔀 Alternatives considered
- **Built-in vocabulary only** — does not scale to each project's jargon; would require recompiling.
- **Global GitExtensions setting** — not per-repository; hard to version alongside the code.
- **Versioned per-repo file (chosen)** — travels with the project, editable by any team member, no recompile.

## ⚖️ Consequences
**Positive:**
- Per-project customization without recompiling; versionable alongside the code.
- Robust to malformed JSON (ignores and proceeds with the defaults).

**Negative / trade-offs:**
- One more convention file for the team to know.
- No explicit validation/error feedback (by design — silent).

## 🔗 Related
- [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies: diff comments + file names]]
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]

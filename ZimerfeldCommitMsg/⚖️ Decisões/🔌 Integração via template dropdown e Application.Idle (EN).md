---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [decisao, adr, integração, application-idle, template, formcommit]
status: aceita
---

# 🔌 ADR — Integration via template dropdown + Application.Idle

> 🇧🇷 Português → [[🔌 Integração via template dropdown e Application.Idle]] · 🇪🇸 Español → [[🔌 Integração via template dropdown e Application.Idle (ES)]]

## 🎯 Context
The plugin needs to **fill the message box** of the GitExtensions commit dialog and **keep the message up to date** while the user changes the stage. But the host's extensibility **does not expose** a "commit dialog opened" event nor a "template item clicked" callback.

## ✅ Decision
Combine two mechanisms:
1. **Template items in the commit dropdown** (`CommitTemplateManager`) — one per language. They materialize the generated message and allow quick language selection.
2. **`Application.Idle`** — detects the freshly opened `FormCommit` (scanning `Application.OpenForms` by type name) and **fills the box**, with gates per instance and working dir to avoid reprocessing on every tick.

Choosing a dropdown item is recognized **indirectly**: the host materializes the text of **all** items when the dropdown opens and, on click, only applies the text. The plugin records each generated text (msg → language) and detects, via the box (`TextChanged`), which was chosen, pinning the session language.

## 🔀 Alternatives considered
- **Template only** — does not cover auto-refresh on stage/unstage nor reliable initial fill.
- **Subscribe to host events** — there is no suitable event; it would rely on fragile internal types across versions.
- **Reflection + Application.Idle (chosen)** — works with the public API, version-tolerant (matches by the `FormCommit` type **name**).

## ⚖️ Consequences
**Positive:**
- Automatic fill on open and on every stage change, without overwriting typed text (**non-destructive**).
- Per-commit language choice right in the dialog.

**Negative / trade-offs:**
- `Application.Idle` fires a lot → requires careful gates (instance + working dir) to avoid reprocessing.
- Dropdown click detection is indirect (via `TextChanged`) — subtler than a callback.
- Locating the message box relies on reflection over `FormCommit`.

## 🔗 Related
- [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]]
- [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]
- [[🏗️ Arquitetura (EN)|Architecture]]

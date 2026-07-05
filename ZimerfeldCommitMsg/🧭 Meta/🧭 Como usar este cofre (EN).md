---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [meta, protocolo]
---

# 🧭 How to use this vault (Claude's protocol)

> 🇧🇷 Leia esta página em português → [[🧭 Como usar este cofre]]

> [!important] Memory protocol
> At the **start** of each session, read: [[🏠 Home (EN)|Home]], [[🔑 Fatos-Chave (EN)|Key Facts]] and the [[📌 Backlog (EN)|Backlog]] (resumption point).
> At the **end** of each session, update the affected notes and the [[📌 Backlog (EN)|Backlog]].

## ✍️ When to record memory
| Situation | Where to record |
|----------|-------------|
| Discovered structure/behavior of a critical file | `🔑 Arquivos-Chave/` |
| Learned about a reused subsystem | `🧩 Sistemas/` |
| Documented a step-by-step usage flow | `🔀 Fluxos/` |
| Wrote a dev/deploy runbook | `🚀 Operação/` |
| We made an architecture decision | `⚖️ Decisões/<title>.md` |
| Learned a reusable concept or pattern | `📚 Conhecimento/` |
| Business facts, adoption, monetization | `💼 Negócio/` |
| Renato's preference/context, tools, key facts | `🧭 Meta/` |

## 🔗 Writing rules
1. **Always use frontmatter** (`tipo`, `projeto`, `lang`, `atualizado`).
2. **Bilingual pair:** each PT note `<emoji> Name.md` has its `<emoji> Name (EN).md` pair.
3. **Emoji + space** at the start of every folder and `.md` file (except `sortspec.md` and images in `📎 Anexos/`).
4. **Interlink** with `[[wikilinks]]` — the vault's value is in the connections.
5. **ISO dates** `YYYY-MM-DD`.
6. Use **callouts** (`> [!note]`, `> [!warning]`) for highlights.

## 🧩 Obsidian plugins
- **Custom File Explorer sorting** (`custom-sort`) — **installed and active**; reads the [[sortspec]] to order folders by priority.
- **Dataview** / **Templater** — optional; without them the vault works normally.

## 🔗 Related
- [[🏠 Home (EN)|Home]]
- [[🔑 Fatos-Chave (EN)|Key Facts]]
- [[📌 Backlog (EN)|Backlog]]

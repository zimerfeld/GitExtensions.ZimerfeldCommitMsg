---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [meta, protocolo]
---

# 🧭 Como usar este cofre (protocolo do Claude)

> 🇧🇷 Portugués → [[🧭 Como usar este cofre]] · 🇺🇸 English → [[🧭 Como usar este cofre (EN)]]

> [!important] Protocolo de memoria
> Al **inicio** de cada sesión, lee: [[🏠 Home (ES)|Home]], [[🔑 Fatos-Chave (ES)|Hechos Clave]] y el [[📌 Backlog (ES)|Backlog]] (punto de retomada).
> Al **final** de cada sesión, actualiza las notas afectadas y el [[📌 Backlog (ES)|Backlog]].

## ✍️ Cuándo grabar memoria
| Situación | Dónde grabar |
|----------|-------------|
| Descubrí la estructura/comportamiento de un archivo crítico | `🔑 Arquivos-Chave/` |
| Aprendí sobre un subsistema reutilizado | `🧩 Sistemas/` |
| Documenté un flujo de uso paso a paso | `🔀 Fluxos/` |
| Escribí un runbook de dev/deploy | `🚀 Operação/` |
| Tomamos una decisión de arquitectura | `⚖️ Decisões/<título>.md` |
| Aprendí un concepto o patrón reutilizable | `📚 Conhecimento/` |
| Hechos de negocio, adopción, monetización | `💼 Negócio/` |
| Preferencia/contexto de Renato, herramientas, hechos clave | `🧭 Meta/` |

## 🔗 Reglas de escritura
1. **Usa siempre frontmatter** (`tipo`, `projeto`, `lang`, `atualizado`).
2. **Par bilingüe:** cada nota PT `<emoji> Nombre.md` tiene su par `<emoji> Nombre (EN).md`.
3. **Emoji + espacio** al inicio de toda carpeta y archivo `.md` (excepto `sortspec.md` e imágenes en `📎 Anexos/`).
4. **Interconecta** con `[[wikilinks]]` — el valor del cofre está en las conexiones.
5. **Fechas en ISO** `AAAA-MM-DD`.
6. Usa **callouts** (`> [!note]`, `> [!warning]`) para destacar.

## 🧩 Plugins de Obsidian
- **Custom File Explorer sorting** (`custom-sort`) — **instalado y activo**; lee el [[sortspec]] para ordenar las carpetas por prioridad.
- **Dataview** / **Templater** — opcionales; sin ellos el cofre funciona con normalidad.

## 🔗 Relacionado
- [[🏠 Home (ES)|Home]]
- [[🔑 Fatos-Chave (ES)|Hechos Clave]]
- [[📌 Backlog (ES)|Backlog]]

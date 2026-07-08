---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [meta, protocolo]
---

# 🧭 Como usar este cofre (protocolo do Claude)

> [!important] Protocolo de memória
> No **início** de cada sessão, leia: [[🏠 Home]], [[🔑 Fatos-Chave]] e o [[📌 Backlog]] (ponto de retomada).
> No **fim** de cada sessão, atualize as notas afetadas e o [[📌 Backlog]].

## ✍️ Quando gravar memória
| Situação | Onde gravar |
|----------|-------------|
| Descobri estrutura/comportamento de um arquivo crítico | `🔑 Arquivos-Chave/` |
| Aprendi sobre um subsistema reutilizado | `🧩 Sistemas/` |
| Documentei um fluxo de uso passo a passo | `🔀 Fluxos/` |
| Escrevi um runbook de dev/deploy | `🚀 Operação/` |
| Tomamos uma decisão de arquitetura | `⚖️ Decisões/<título>.md` |
| Aprendi um conceito ou padrão reutilizável | `📚 Conhecimento/` |
| Fatos de negócio, adoção, monetização | `💼 Negócio/` |
| Preferência/contexto do Renato, ferramentas, fatos-chave | `🧭 Meta/` |

## 🔗 Regras de escrita
1. **Sempre use frontmatter** (`tipo`, `projeto`, `lang`, `atualizado`).
2. **Par bilíngue:** cada nota PT `<emoji> Nome.md` tem seu par `<emoji> Nome (EN).md`.
3. **Emoji + espaço** no início de toda pasta e arquivo `.md` (exceto `sortspec.md` e imagens em `📎 Anexos/`).
4. **Interligue** com `[[wikilinks]]` — o valor do cofre está nas conexões.
5. **Datas em ISO** `AAAA-MM-DD`.
6. Use **callouts** (`> [!note]`, `> [!warning]`) para destaques.

## 🧩 Plugins do Obsidian
- **Custom File Explorer sorting** (`custom-sort`) — **instalado e ativo**; lê o [[sortspec]] para ordenar as pastas por prioridade.
- **Dataview** / **Templater** — opcionais; sem eles o cofre funciona normalmente.

## 🔗 Relacionado
- [[🏠 Home]]
- [[🔑 Fatos-Chave]]
- [[📌 Backlog]]

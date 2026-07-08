---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [conhecimento, git, conventional-commits, mensagem-de-commit]
---

# 📜 Conventional Commits — Concepts

## 📝 Summary
**Conventional Commits** is a convention for commit messages that classifies each change by a semantic **type**. The canonical format is `<type>[optional scope]: <description>`, with optional body and footer. See https://www.conventionalcommits.org/en/v1.0.0/.

## 🏷️ Types used by the plugin

| Type | Meaning | Verb (pt / en) |
|---|---|---|
| `feat` | new feature | Adiciona / Add |
| `fix` | bug fix | Corrige / Fix |
| `docs` | documentation | Documenta / Document |
| `test` | tests | Testa / Test |
| `chore` | maintenance tasks | Atualiza / Update |
| `build` | build/dependencies | Compila / Build |
| `refactor` | behavior-preserving refactor | Refatora / Refactor |

## ⚙️ How the plugin applies it

> [!important] Verb, not literal prefix
> The plugin uses the type to **choose the verb**, **not** to write `feat:`. The subject starts with the verb; the involved types appear only in parentheses on the summary line. See [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject]].

Two-dimensional classification:
- **Extension category** (`ExtCategory`): `source`, `web`, `docs`, `build`, `config` — used to prioritize files and infer type.
- **Conventional Commits type**: heuristic over category + diff content (e.g. test files → `test`; `.md` → `docs`; `.csproj`/`.sln` → `build`).

## 🧾 Generated message format

```
[<context>] - <verb> N files (type1, type2, …)         ← consolidated subject
                                                       ← blank line
- <summary phrase for file 1>                          ← body (up to 5 bullets)
- <summary phrase for file 2>
```

Example:
```
Overlay - Adiciona 10 arquivos (feat, fix, docs)

- Adiciona autenticação
- Corrige cálculo de saldo
- Documenta instalação
```

## 📍 Where this appears in the plugin
The [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] does the classification and the `LanguagePack` maps type → verb per language. See [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]].

## 🔗 Related
- [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]]
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject]]

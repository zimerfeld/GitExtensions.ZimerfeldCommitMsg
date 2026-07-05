---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [conhecimento, readme, instalacao, build, uso, conventional-commits, i18n]
fonte: README.md
versao: 1.0.97
---

# 📖 README — Install, Use and Build

> 🇧🇷 Leia esta página em português → [[📖 README — Instalação, Uso e Build]]

> Mirror of the repository root `README.md` (bilingual EN/PT), reconciled with the code on 2026-07-04.
> Project note: [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]]. Concepts in [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]].
> `build.ps1` stamps version + date into the READMEs **and** into the vault notes that mirror the current version — see [[🏷️ Versionamento (EN)|Versioning]].

Plugin for **[GitExtensions](https://gitextensions.github.io/)** that **generates commit messages automatically** by analyzing the real content of the staged changes. Changes are classified by the **Conventional Commits** types to choose the **verb**; the message is a **verb-first subject** (without the `type:` prefix) plus a **bulleted body**. **Bilingual** output (pt-BR / English), detected from the OS with a manual override.

## ✨ High-level features
- **Automatic generation** from the staged diff content — not just file names.
- **Verb guided by Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) — the type does **not** appear in the message. See [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject]].
- **Two strategies** — diff comments (primary) + file names (fallback). See [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies: diff comments + file names]].
- **Per-repository vocabulary** — `.zimerfeldcommitmsg.json` extends vocabulary/concepts without recompiling. See [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]].
- **Multilingual (PT-BR / EN)** — automatic from the OS + override (3-item dropdown and setting).
- **Auto-fill** on opening the dialog and on stage/unstage; **non-destructive**.

## 🧩 How it works
When the commit dialog opens, the plugin reads `git diff --cached`, classifies the changes and fills the message box. Details in [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]] and [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]].

## 🗣️ Language
Two ways to choose (bilingual labels):
1. **Templates dropdown** in the commit dialog — three items (Automatic / Portuguese / English), per-commit choice.
2. **Settings → Plugins → ZimerfeldCommitMsg** — "Idioma da mensagem / Message language" selector (default for auto-refresh and the Plugins menu).

| Option | Behavior |
|---|---|
| `Automático/Automatic` | **Default.** Detected from the OS (`pt-*` → Portuguese; otherwise → English) |
| `Português/Portuguese` | Forces pt-BR |
| `Inglês/English` | Forces English |

Side-by-side example:
| Português-BR | English |
|---|---|
| `Implementa autenticação` | `Implement authentication` |
| `- Adiciona autenticação` | `- Add authentication` |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |

## 📦 Install
**Via the GitExtensions Plugin Manager:** search for *ZimerfeldCommitMsg* (Plugins → Plugin Manager), install and restart.

**Manual:** run `build.ps1` (as Administrator for automatic deploy), or copy `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` to `C:\Program Files\GitExtensions\Plugins\`, or run `tools\install.ps1` as Administrator.

> The **ZimerfeldCommitMsg** node only appears in **Settings → Plugins** after the DLL with the language selector is installed and GitExtensions is restarted.

## ✅ Requirements
- GitExtensions 6.x (.NET 9)
- `git` on the `PATH` (the generator runs `git diff --cached`)

## 🛠️ Build
```powershell
pwsh .\build.ps1          # bumps version, Release build, packs the .nupkg
pwsh .\build.ps1 -Force   # always recompiles/repackages
```
See [[🏷️ Versionamento (EN)|Versioning]] and [[🛠️ build.ps1 (EN)|build.ps1]].

## 💜 Support the project
**GitHub Sponsors:** [github.com/sponsors/zimerfeld](https://github.com/sponsors/zimerfeld) · **Ko-fi:** [ko-fi.com/C0D621FCGD](https://ko-fi.com/C0D621FCGD). Badges at the top of the README (version + NuGet downloads).

## 📄 License
Copyright © 2026 Zimerfeld — **CC BY-NC-ND 4.0** (`LICENSE.txt`).

## 🔗 Related
- [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]]
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]
- [[🔑 Fatos-Chave (EN)|Key Facts]]

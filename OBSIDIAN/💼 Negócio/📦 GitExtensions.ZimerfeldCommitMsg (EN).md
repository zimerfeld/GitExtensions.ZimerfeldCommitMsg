---
tipo: negocio
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [projeto, negocio, csharp, gitextensions, plugin, winforms, commit-message, conventional-commits, i18n]
status: ativo
linguagem: C#
versao: 1.0.97
repo: C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg
---

# 📦 GitExtensions.ZimerfeldCommitMsg

> 🇧🇷 Leia esta página em português → [[📦 GitExtensions.ZimerfeldCommitMsg]]

## 🎯 Objective
Plugin for **[GitExtensions](https://gitextensions.github.io/)** that **generates commit messages automatically** by analyzing the **real content** of the staged changes (not just file names). Changes are classified by the **Conventional Commits** types (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) to choose the right **verb**; the message is a **verb-first subject** (without the literal `type:` prefix) plus a **bulleted body**. See [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]].

## 💜 Funding / Sponsorship
Donation channels configured (badges at the top of the README):
- **GitHub Sponsors:** `@zimerfeld` → https://github.com/sponsors/zimerfeld
- **Ko-fi:** `C0D621FCGD` → https://ko-fi.com/C0D621FCGD
- **Social proof in the README:** version and **NuGet downloads** badges (`shields.io/nuget/v` and `/dt`).

## 📂 Repository structure
```
C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\
├─ src\GitExtensions.ZimerfeldCommitMsg\        # plugin code (.csproj)
│   ├─ ZimerfeldCommitMsgPlugin.cs             # MEF entry point + commit-dialog integration
│   ├─ CommitMessageGenerator.cs               # engine: diff → CC types → verb → subject + body (~1200 lines)
│   ├─ RepoVocabularyConfig.cs                 # extra per-repo vocabulary (.zimerfeldcommitmsg.json)
│   ├─ Localization\                           # MessageLanguage, LanguagePack, Strings
│   ├─ Resources\                              # icon.png, icon-128.png, Strings.resx, StringsPtBr.resx
│   ├─ *.csproj / *.nuspec                     # build + NuGet manifest
├─ tests\GitExtensions.ZimerfeldCommitMsg.Tests\  # xUnit: comments, concepts, vocab, translation
├─ refs\                                        # versioned host DLLs (deterministic build)
├─ tools\                                       # install/uninstall/update-dll .ps1, nuget.exe, generate-icon.ps1
│   └─ net9.0-windows\                          # build output (DLL) used by the nupkg
├─ OBSIDIAN\                                    # 🧠 this memory vault
├─ build.ps1                                    # bumps version + build + deploy + nupkg
├─ README.md / README.pt-BR.md / README.en-US.md  # mirrored in [[📖 README — Instalação, Uso e Build (EN)|README — Install, Use and Build]]
└─ GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg
```

## ⚙️ Technical stack
- **Language:** C# (`net9.0-windows`), `Nullable` + `ImplicitUsings` enabled
- **UI:** WinForms (`UseWindowsForms`) — the plugin **has no window of its own**; it integrates into the host's **commit dialog**
- **Output type:** `Library` (DLL loaded by GitExtensions)
- **AssemblyName:** `GitExtensions.Plugins.ZimerfeldCommitMsg`
- **Root namespace:** `GitExtensions.ZimerfeldCommitMsg`
- **NeutralLanguage:** `pt-BR`
- **Plugin model:** MEF (`[Export(typeof(IGitPlugin))]`) — see [[🧩 Plugin MEF para GitExtensions (EN)|MEF plugin for GitExtensions]]
- **External references** (from `refs\`, `Private=false`): `GitExtensions.Extensibility.dll`, `System.ComponentModel.Composition.dll`
- **Tests:** xUnit, with `InternalsVisibleTo` exposing the generator's pure functions

## ✨ Main features
- **Automatic generation** from the real staged diff content (`git diff --cached`), not just file names. See [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]].
- **Verb guided by Conventional Commits** — classifies changes by type (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) and prefixes the corresponding **verb** (3rd person in pt-BR / imperative in English). The type itself does **not** appear. See [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject]].
- **Two content strategies:** based on diff **comments** (primary) and on **file names** (fallback). See [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies: diff comments + file names]].
- **Bulleted body** — up to 5 one-line phrases, each summarizing a file's most significant change; **always at least one bullet**.
- **English → Portuguese translation** of comments when the output is pt-BR; in English they pass through intact.
- **Sanitization** — discards comments with unbalanced delimiters or ending in a dangling connector word; picks the one of **best quality** (not the longest).
- **Per-repository vocabulary** — `.zimerfeldcommitmsg.json` extends known/rejected vocabulary and concept phrases without recompiling. See [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]].
- **Multilingual (PT-BR / EN):** automatic from the OS + override (3-item dropdown in the commit dialog **and** selector in Settings → Plugins). See [[🌐 Localization (EN)|Localization]].
- **Three integration modes:** commit-dialog template, Plugins menu and **auto-fill** (on opening the dialog and on stage/unstage). See [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]].
- **Non-destructive** — never overwrites manually typed text.

## 🏗️ Architecture (Plugin → Generator → Localization)
See [[🏗️ Arquitetura (EN)|Architecture]]:
```
GitExtensions (host)
    │ MEF
    ▼
ZimerfeldCommitMsgPlugin  ← [Export(IGitPlugin)]; registers commit template + Application.Idle
    │ on FormCommit open/change, generates and injects the message (without overwriting user input)
    ▼
CommitMessageGenerator    ── reads ──►  git diff --cached  (comments + file names)
    │ classifies CC types → verb → consolidated subject + bulleted body
    ▼
Localization (LanguagePack / Strings / MessageLanguage)  → output language + UI strings
```

## 🛠️ Build / install
```powershell
# Build: bumps build, compiles Release, deploys (Admin), generates nupkg
.\build.ps1
.\build.ps1 -Force   # always recompiles/repackages

# Helper scripts in tools\ (Admin for Program Files)
tools\install.ps1      # installs the plugin (also via Package Manager Console)
tools\uninstall.ps1    # removes it (does not affect the rest of GitExtensions)
tools\update-dll.ps1   # updates just the DLL in the Plugins folder
```
> Via the **GitExtensions Plugin Manager**: search for *ZimerfeldCommitMsg* and install. Step by step in [[📖 README — Instalação, Uso e Build (EN)|README — Install, Use and Build]] and [[🏷️ Versionamento (EN)|Versioning]]. Runbooks: [[💻 Ambiente Local (Dev) (EN)|Local Environment (Dev)]] · [[🚀 Deploy em Produção (Prod) (EN)|Production Deploy (Prod)]].

## 💰 Investment angle
- **Daily friction:** every dev commits several times a day; good messages are expensive to write by hand. This plugin removes that friction without taking away control (non-destructive).
- **Technical differentiator:** it reads the **diff content** (comments) and classifies by Conventional Commits — richer than generators based only on file names.
- **Low marginal cost:** it shares the infrastructure (build, i18n, versioned refs, packaging) of the siblings `GitExtensions.ZimerfeldLFS` and `GitExtensions.ZimerfeldTree` — a cohesive portfolio reinforcing the **Zimerfeld** brand in the GitExtensions/NuGet ecosystem.
- **Ready distribution:** published on NuGet and visible in the internal Plugin Manager (marker dependency `GitExtensions.Extensibility`).

## 🐛 Known pitfalls
> [!warning] The host does not notify "commit dialog opened"
> There is no API event for "FormCommit opened". The plugin detects the freshly opened `FormCommit` in the UI's **`Application.Idle`** and gates by instance + working dir to avoid reprocessing on every tick. See [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]] and [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown + Application.Idle]].

<!-- -->

> [!warning] Embedded UI strings (no satellite assemblies)
> Strings live in `Resources\Strings.resx` / `StringsPtBr.resx` with a fixed `LogicalName`, embedded in the **main assembly** — so the deploy is a **single DLL**. See [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded UI strings]] and [[🏷️ Versionamento (EN)|Versioning]].

<!-- -->

> [!warning] DLL in the ROOT `lib\` in the nuspec (intentional NU5101 warning)
> As in the siblings, the DLL goes into the **root** `lib\` ("any" group) for the Plugin Manager to extract; the `nuget pack` **NU5101** warning is deliberately filtered in `build.ps1`. See [[🏷️ Versionamento (EN)|Versioning]] and [[🔗 Dependências (EN)|Dependencies]].

## 🔢 Versioning
- Current version: **1.0.97** (csproj + nuspec synced by `build.ps1`)
- Scheme: `major.minor.BUILD`, BUILD auto-incremented on each build
- On every build, `build.ps1` stamps version + date into the **READMEs** (and keeps this vault in sync)

## 🔗 Related
- [[📖 README — Instalação, Uso e Build (EN)|README — Install, Use and Build]] — mirror of `README.md`
- [[🧩 Plugin MEF para GitExtensions (EN)|MEF plugin for GitExtensions]]
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]] · [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]
- [[🏗️ Arquitetura (EN)|Architecture]] · [[🔭 Visão Geral (EN)|Overview]] · [[🏷️ Versionamento (EN)|Versioning]] · [[🔗 Dependências (EN)|Dependencies]]
- `GitExtensions.ZimerfeldLFS` — sibling (Git LFS)
- `GitExtensions.ZimerfeldTree` — sibling (branch tree)
- [[🔑 Fatos-Chave (EN)|Key Facts]]

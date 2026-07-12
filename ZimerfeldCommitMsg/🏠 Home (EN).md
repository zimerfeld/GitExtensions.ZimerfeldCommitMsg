---
tipo: moc
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-05
---

# 🏠 GitExtensions.ZimerfeldCommitMsg — Cofre de Neurônios

> 🇧🇷 Português → [[🏠 Home]] · 🇪🇸 Español → [[🏠 Home (ES)]]

> [!abstract] 🧠 What this vault is
> Claude's persistent memory for the **GitExtensions.ZimerfeldCommitMsg** project — a MEF plugin for GitExtensions that generates commit messages automatically from the staged diff. This vault is maintained every session: bilingual-paired notes (PT/EN), standardized frontmatter and priority ordering.

## ⚡ Executive summary
- **What it is:** a (MEF plugin) extension for **GitExtensions** that integrates into the **commit dialog** and **fills the message automatically** by analyzing `git diff --cached`.
- **Problem it solves:** writing good commit messages is tedious and inconsistent. The plugin reads what **actually** changed (comments added in the diff + file names), classifies by Conventional Commits and materializes a ready subject + body — in pt-BR or English.
- **Differentiators:** analyzes the **diff content**, not just file names; **verb guided by Conventional Commits** without polluting the message with `type:`; **two strategies** (comments + file names); **per-repository vocabulary** (`.zimerfeldcommitmsg.json`) without recompiling; **auto-refresh** on stage/unstage; **non-destructive**; **i18n** (Automatic / PT-BR / EN).
- **Stack:** C# / WinForms `Library`, targeting **net9.0-windows**, packaged as a **nupkg**; build and versioning via `build.ps1`.
- **Current state:** version **`1.0.97`** — functional, with an **xUnit test suite**.
- **Target audience:** developers and teams using GitExtensions on Windows who want consistent commit messages with little effort.
- **Business/portfolio angle:** **open source** product under the owner `zimerfeld`, alongside the siblings `GitExtensions.ZimerfeldLFS` and `GitExtensions.ZimerfeldTree`.

## 🧭 Navigation by priority

### 1️⃣ 🔑 Impact — Key Files
> Files that, if manipulated, have a large impact on the system.
- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]] — MEF entry point + commit-dialog integration
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] — the engine: diff → CC types → verb → subject + body
- [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]] — extra per-repository vocabulary (`.zimerfeldcommitmsg.json`)
- [[🌐 Localization (EN)|Localization]] — message language + UI strings
- [[🛠️ build.ps1 (EN)|build.ps1]] — build, versioning and deploy script

### 2️⃣ 🧩 Reuse — Systems
> Subsystems reused across the project.
- [[🔭 Visão Geral (EN)|Overview]] — what the plugin does, stack, current version
- [[🏗️ Arquitetura (EN)|Architecture]] — Plugin (host integration) → Generator → Localization
- [[🏷️ Versionamento (EN)|Versioning]] — build.ps1 / nuspec / csproj cycle
- [[🔗 Dependências (EN)|Dependencies]] — GitExtensions assemblies + Conventional Commits

### 3️⃣ 🔀 Use — Flows
> Step-by-step usage flows.
- [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]] — Application.Idle detects the FormCommit and fills it
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]] — diff → CC types → verb → subject + body
- [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]] — 3-language dropdown, setting, refresh on stage/unstage

## 🚀 Operation
- [[💻 Ambiente Local (Dev) (EN)|Local Environment (Dev)]] — `.\build.ps1 -Force` (builds + installs into local GitExtensions)
- [[🚀 Deploy em Produção (Prod) (EN)|Production Deploy (Prod)]] — `.\build.ps1` → `.nupkg` at root → NuGet + GitHub release

## ⚖️ Decisions (ADRs)
- [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown + Application.Idle]] — how the plugin enters the commit dialog
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject]] — Conventional Commits without the literal "type:"
- [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies: diff comments + file names]] — where the content comes from
- [[📓 Vocabulário por repositório (EN)|Per-repository vocabulary]] — `.zimerfeldcommitmsg.json` without recompiling
- [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded UI strings]] — single-DLL deploy

## 📚 Knowledge
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]] — types, verbs, subject/body format
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]] — extraction and sanitization pipeline
- [[🧩 Plugin MEF para GitExtensions (EN)|MEF plugin for GitExtensions]] — MEF plugin model
- [[📖 README — Instalação, Uso e Build (EN)|README — Install, Use and Build]] — README mirror

## 💼 Business
- [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]] — mother note: objective, stack, features, investment angle, funding

## 🧭 Meta
- [[🔑 Fatos-Chave (EN)|Key Facts]] — paths, conventions, tools
- [[🧭 Como usar este cofre (EN)|How to use this vault]] — Claude's read/write protocol
- [[👤 Renato (EN)|Renato]] — preferences and context · [[🦀 RTK (EN)|RTK]] — token-saving CLI proxy · [[📥 Inbox (EN)|Inbox]]

## 🧱 Templates
- [[⚖️ Template - Decisão (ADR)|Decision (ADR) template]] — template for new Decisions (ADRs)
- [[💼 Template - Negócio|Business template]] — template for Business notes
- [[📚 Template - Conhecimento|Knowledge template]] — template for Knowledge notes

## 📂 Repo Folder Structure
```
GitExtensions.ZimerfeldCommitMsg/
├── src/GitExtensions.ZimerfeldCommitMsg/
│   ├── ZimerfeldCommitMsgPlugin.cs   ← MEF entry point + commit-dialog integration
│   ├── CommitMessageGenerator.cs     ← engine: diff → CC types → verb → subject + body
│   ├── RepoVocabularyConfig.cs        ← extra per-repo vocabulary (.zimerfeldcommitmsg.json)
│   ├── Localization/                  ← MessageLanguage, LanguagePack, Strings
│   ├── Resources/                     ← icon.png, icon-128.png, Strings.resx, StringsPtBr.resx
│   ├── *.csproj / *.nuspec
├── tests/GitExtensions.ZimerfeldCommitMsg.Tests/  ← xUnit (comments, concepts, vocab, translation)
├── refs/                              ← versioned host DLLs (Private=false)
├── tools/
│   ├── install.ps1 / uninstall.ps1 / update-dll.ps1
│   ├── net9.0-windows/                ← DLL for the nupkg
│   └── generate-icon.ps1
├── ZimerfeldCommitMsg/                ← 🧠 this memory vault
├── build.ps1                          ← bumps version + build + deploy + nupkg
└── README.md / README.pt-BR.md / README.en-US.md
```

## 📌 Resumption
- [[📌 Backlog (EN)|Backlog]] — **start here** when resuming the project in another session

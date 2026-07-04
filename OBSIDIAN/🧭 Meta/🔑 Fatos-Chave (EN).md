---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [fatos-chave, referencia, meta]
---

# 🔑 Key Facts

> 🇧🇷 Leia esta página em português → [[🔑 Fatos-Chave]]

> [!tip] Read first
> Stable, always-useful truths. Update when something changes.

## 👤 User
- Name: **Renato Zimerfeld**
- Email: renato.zimerfeld@gmail.com
- Working language: **Portuguese (BR)**
- See [[👤 Renato (EN)|Renato]] for detailed preferences

## 📁 Important paths
- Memory vault (this one): `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\OBSIDIAN`
- Project root: `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg`
- `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg` **is** the repository of the [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]] project (C#, GitExtensions plugin that generates commit messages)
- GitExtensions installed at: `C:\Program Files\GitExtensions\`
- GitExtensions plugins: `C:\Program Files\GitExtensions\Plugins\`
- GitExtensions settings (working-dir source): `%APPDATA%\GitExtensions\GitExtensions\GitExtensions.settings`
- Optional per-repository config: `.zimerfeldcommitmsg.json` at the working-dir root — see [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]]
- Host reference assemblies: versioned in `refs\` (deterministic, offline build)
- Sibling projects: `GitExtensions.ZimerfeldLFS` (Git LFS) and `GitExtensions.ZimerfeldTree` (branch tree), each with its own vault

## 🛠️ Active tools
- **RTK (Rust Token Killer)** — CLI proxy that saves tokens. See [[🦀 RTK (EN)|RTK]]
- **Obsidian** — this memory vault
- **Claude Code** on Windows (shell: PowerShell / git bash)

## 📐 Conventions
- Dates: `YYYY-MM-DD`
- **This project** is a **GitExtensions MEF plugin** that **generates commit messages** from the real staged diff content, classifying changes by **Conventional Commits** to choose the **verb** — the subject starts with a verb, **without** the literal `type:` prefix. See [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]] and [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]
- **Bilingual** output (pt-BR / English): automatic from the OS + manual override (setting and templates dropdown)
- Runtime requirement: **GitExtensions 6.x (.NET 9)** — the plugin runs `git diff --cached` to read the stage
- Versioning `major.minor.BUILD`, build auto-incremented by `build.ps1`
- License: **CC BY-NC-ND 4.0 © 2026 Zimerfeld**
- Co-authored commits when a push is requested
- Do not commit/push without an explicit request

## 🔗 Related
- [[🏠 Home (EN)|Home]]
- [[📌 Backlog (EN)|Backlog]]
- [[🧭 Como usar este cofre (EN)|How to use this vault]]

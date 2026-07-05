---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [sistema, overview, plugin, gitextensions, commit-message, conventional-commits, i18n]
---

# 🔭 Overview

> 🇧🇷 Leia esta página em português → [[🔭 Visão Geral]]

## 🎯 What it is

A plugin for **GitExtensions** (Windows) that **automatically generates the commit message** from the real content of the staged changes. It integrates with the host's **commit dialog**: when the dialog opens (and whenever files are staged/unstaged), the plugin reads `git diff --cached`, classifies the changes using **Conventional Commits** and materializes a **verb-first subject** + a **bullet body**, in the chosen language. See [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]] and [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject without type prefix]].

## 🧱 Stack

| Item | Value |
|---|---|
| Language | C# (.NET 9) |
| Target | `net9.0-windows` |
| UI Framework | Windows Forms (`UseWindowsForms`) — no window of its own; integrates with the commit dialog |
| Output type | `Library` (DLL loaded by GitExtensions) |
| Output assembly | `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` |
| Namespace | `GitExtensions.ZimerfeldCommitMsg` |
| Current version | `1.0.94` |
| Languages | Brazilian Portuguese / English (auto via OS + manual override) |
| License | CC BY-NC-ND 4.0 © 2026 Zimerfeld |
| Author | Zimerfeld |

> The plugin shows an **icon** (PNG embedded at `Resources/icon.png`) in the Plugins menu and in the commit dialog's template dropdown.

## 🔌 The three integration modes

### 1️⃣ Template in the commit dialog dropdown
The plugin registers **three template items** (one per language) via `CommitTemplateManager`. Choosing an item applies the message generated in that language and pins the language for the session's auto-refresh. See [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]].

### 2️⃣ Plugins menu → ZimerfeldCommitMsg
Triggers `Execute` and generates the message for the current repository, using the **setting** language (Automatic/PT/EN).

### 3️⃣ Auto-fill (Application.Idle)
Since there is no "commit dialog opened" event, the plugin watches **`Application.Idle`**, detects the freshly opened `FormCommit` and **fills the message box** — repeating on stage/unstage. It **never overwrites** manually typed text. See [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]].

## ⚙️ How the message is generated

The [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] reads `git diff --cached` and produces:
- **Consolidated subject:** `[<context>] - <verb> N files (types)` — e.g.: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`.
- **Bullet body:** up to 5 sentences, each summarizing the most significant change of one file, extracted from the diff **comments** (main strategy) or derived from **file names** (fallback).

See [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]].

## 🧰 Cross-cutting features

| Feature | Detail |
|---|---|
| Two strategies | diff comments (main) + file names (fallback) — [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies - comments and file names]] |
| Per-repo vocabulary | `.zimerfeldcommitmsg.json` extends vocabulary/concepts — [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]] |
| EN→PT translation | comments translated when output is pt-BR; untouched in English |
| Sanitization | discards comments with unbalanced delimiters or a dangling connective at the end |
| Localization | embedded UI strings (`Strings.resx`/`StringsPtBr.resx`) + message language |
| Non-destructive | never overwrites what the user already typed |

## ✅ Runtime requirements

- **GitExtensions 6.x** (.NET 9)
- **`git`** on the `PATH` (the generator runs `git diff --cached`)

## 🔗 Links

- [[🏗️ Arquitetura (EN)|Architecture]]
- [[🏷️ Versionamento (EN)|Versioning]]
- [[🔗 Dependências (EN)|Dependencies]]
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]

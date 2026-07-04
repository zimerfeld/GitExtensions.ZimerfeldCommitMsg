---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [conhecimento, csharp, gitextensions, mef, plugin]
---

# 🧩 MEF plugin for GitExtensions

> 🇧🇷 Leia esta página em português → [[🧩 Plugin MEF para GitExtensions]]

## 📝 Summary
GitExtensions loads plugins via **MEF** (Managed Extensibility Framework). The entry point is an exported class that implements `IGitPlugin` (usually inheriting from `GitPluginBase`).

## 🔑 Key points
- Export with `[Export(typeof(IGitPlugin))]` using `System.ComponentModel.Composition`.
- The project compiles as a **`Library`** (DLL), `net9.0-windows`, with WinForms enabled.
- Reference the host assemblies with **`<Private>false</Private>`** (do not copy to output — the host already has them). Here they are **versioned in `refs\`** (deterministic, offline build):
  - `GitExtensions.Extensibility.dll`
  - `System.ComponentModel.Composition.dll`
- The `AssemblyName` must match what `install.ps1` / nuspec expect (`GitExtensions.Plugins.<Name>`).
- To appear in the internal **Plugin Manager**, the NuGet package must **depend** on `GitExtensions.Extensibility` (marker dependency). See [[🔗 Dependências (EN)|Dependencies]].

## ♻️ Plugin lifecycle
- `Register(IGitUICommands)` — called on load. A good place to **capture the `IGitUICommands`**, register commit templates and subscribe to UI events (here: `Application.Idle`).
- `Unregister(IGitUICommands)` — unsubscribe / clear the captured reference.
- `Execute(GitUIEventArgs)` — triggered by the **Plugins → \<name\>** menu.
- `GetSettings()` — exposes configurable settings (here: the language selector).

## ⚙️ How ZimerfeldCommitMsg uses this model
- Registers **three commit template items** (one per language) via `CommitTemplateManager`.
- Subscribes to **`Application.Idle`** to detect the freshly opened `FormCommit` and fill the message — the host does **not** expose a "commit dialog opened" event. See [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]] and [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown + Application.Idle]].
- `Execute` generates the message for the current repo (Plugins menu).
- `GetSettings` exposes the language `ChoiceSetting` (Automatic/PT/EN).

## 🔀 Contrast with the siblings
- `GitExtensions.ZimerfeldLFS` and `GitExtensions.ZimerfeldTree` open **their own windows** (non-modal). CommitMsg, in contrast, **has no window**: it integrates into the host's existing commit dialog.

## 🔗 Related
- [[📦 GitExtensions.ZimerfeldCommitMsg (EN)|GitExtensions.ZimerfeldCommitMsg]]
- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]
- [[🔗 Dependências (EN)|Dependencies]]

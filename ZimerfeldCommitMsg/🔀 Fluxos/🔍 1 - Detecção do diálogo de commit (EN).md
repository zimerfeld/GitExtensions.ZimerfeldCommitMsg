---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [fluxo, application-idle, formcommit, etapa1]
---

# 🔍 Flow: Commit dialog detection (Application.Idle)

> 🇧🇷 Leia esta página em português → [[🔍 1 - Detecção do diálogo de commit]]

How the plugin enters the GitExtensions **commit dialog** and fills the message, without the host offering a "dialog opened" event.

## 🪜 Steps

```
Register(commands)
        │  captures _gitUiCommands + _syncContext (UI thread)
        │  registers 3 template items (languages) in the CommitTemplateManager
        │  Application.Idle += OnAppIdle
        ▼
User opens the commit dialog (FormCommit)
        │
        ▼
OnAppIdle  (fires many times)
        │
        ├─ scans Application.OpenForms for a Form whose GetType().Name == "FormCommit"
        ├─ gate by instance (_handledCommitForm: WeakReference<Form>) — already handled?
        ├─ gate by working dir (_handledWorkingDir) — the host reuses the FormCommit when
        │   switching repos; if the dir changed, RE-fills
        ├─ locates the FormCommit's message box (TextBoxBase)
        ├─ if the box is EMPTY (or it is a legitimate reprocess) → generates and injects the message
        │   (NON-destructive: never overwrites user-typed text)
        └─ subscribes TextChanged once (_subscribedTextBox) → detects the dropdown choice
```

## 🔎 Details

- **No API event:** there is no "FormCommit opened" in the GitExtensions extensibility. That is why the plugin watches the UI's **`Application.Idle`** and looks for the `FormCommit` in `Application.OpenForms` by **type name** (avoids depending on the host's internal types).
- **Gates to avoid repeating:** `Application.Idle` fires continuously. The plugin stores the already-handled **instance** (`WeakReference<Form>`, so it doesn't pin the form) and the **working dir** of the last fill. The host may **reuse** the same `FormCommit` when switching repositories — without tracking the working dir, the instance gate would block re-filling and the box would go stale.
- **Working dir:** `ResolveCommitWorkingDir()` prefers the working dir of the **open `FormCommit` itself** (via reflection), falling back to the captured `_gitUiCommands.Module.WorkingDir` — the same source of truth as the dropdown generation, avoiding generating in the wrong repo.
- **Auto-refresh on stage/unstage:** since the dialog stays open, a new `Idle` with the box still "ours" regenerates the message when the staged set changes. See [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]].
- **Protection:** everything is best-effort inside `try/catch` — exceptions in the plugin never bring GitExtensions down.

## 🔗 Links

- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]
- [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]
- [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]]

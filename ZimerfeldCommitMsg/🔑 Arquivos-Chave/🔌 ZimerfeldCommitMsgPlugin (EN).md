---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquivo, plugin, entry-point, mef, application-idle]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs
---

# 🔌 ZimerfeldCommitMsgPlugin.cs

> 🇧🇷 Português → [[🔌 ZimerfeldCommitMsgPlugin]] · 🇪🇸 Español → [[🔌 ZimerfeldCommitMsgPlugin (ES)]]

The plugin's entry point and **integration with the GitExtensions commit dialog**. Exported via MEF. ~626 lines.

**Path:** `src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs`

---

## 📜 Declaration

```csharp
[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
```

The `[Export]` attribute is the host's MEF discovery point. See [[🧩 Plugin MEF para GitExtensions (EN)|MEF plugin for GitExtensions]].

---

## 🧩 Template items and setting

- `TemplatePrefix = "Zimerfeld Commit Msg"`.
- `_templateItems` — three `(Label, MessageLanguage?)` items: Automatic (`null`), Portuguese (`PtBr`), English (`En`). The host's template API is **flat**, so one item per language is exposed.
- `_languageSetting` — `ChoiceSetting("ZimerfeldCommitMsg_Language", …)` with the options Automatic/Portuguese/English (default Automatic), exposed by `GetSettings()`.

---

## 🗃️ Instance fields (gates and state)

| Field | Purpose |
|---|---|
| `_syncContext` | `SynchronizationContext` captured in `Register` (UI thread) for marshalling |
| `_gitUiCommands` | source of the working dir for the `Application.Idle` trigger |
| `_handledCommitForm` | `WeakReference<Form>` — FormCommit already filled (avoids reprocessing on every Idle) |
| `_handledWorkingDir` | working dir of the last fill (the host reuses the FormCommit when switching repos) |
| `_sessionLanguage` | language pinned by choosing a dropdown item (`null` = follow setting/OS) |
| `_templateMessages` | generated-text → item-language map, to recognize the dropdown choice |
| `_subscribedTextBox` | `WeakReference<TextBoxBase>` of the box we already subscribed `TextChanged` on |
| `_resyncedCommitForm` | guarantees ONE opening re-sync per window |

---

## 🗣️ Effective language

```csharp
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage();
CurrentLanguage()   = MessageLanguageResolver.Resolve(_languageSetting.ValueOrDefault(Settings));
```
The dropdown choice takes priority over the setting/OS. See [[🌐 Localization (EN)|Localization]] and [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]].

---

## ⚙️ Methods (IGitPlugin)

### `Register(IGitUICommands)`
- `base.Register` + captures `_gitUiCommands` and `_syncContext`.
- Registers the **three template items** in the `CommitTemplateManager` (one generation `Func` per language).
- Subscribes **`Application.Idle += OnAppIdle`** — the host does not expose "commit dialog opened", so we detect the `FormCommit` on Idle.

### `OnAppIdle`
- Scans `Application.OpenForms` for a `Form` whose `GetType().Name == "FormCommit"`.
- Gates by **instance** (`_handledCommitForm`) and by **working dir** (`_handledWorkingDir`).
- Fills the message box if empty (**non-destructive**); subscribes `TextChanged` to detect the dropdown choice. See [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]].

### `GenerateForTemplate(MessageLanguage? forced)`
- Generates the message for a dropdown item, in the item's language.
- **Does not pin the language here** (the host calls this `Func` for all items when the dropdown opens). Instead, `RememberTemplateMessage(msg, forced)` records the text → language to later recognize which item the user clicked (`DetectTemplateSelection`). See [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]].

### `ResolveCommitWorkingDir()`
- Prefers the working dir of the open `FormCommit` itself (via reflection), falling back to `_gitUiCommands.Module.WorkingDir` — avoids generating in the wrong/empty repo.

### `Execute(GitUIEventArgs)` ← Plugins menu → ZimerfeldCommitMsg
- Generates the message for the current repository using the setting language.

### `Unregister(IGitUICommands)`
- Unregisters the template, `Application.Idle -= OnAppIdle`, clears the captured commands.

---

## 🛡️ Crash protection

Delegates, reflection over the `FormCommit` and the generation are wrapped in `try/catch` — exceptions in the plugin never bring GitExtensions down.

---

## 🔗 Links

- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[🌐 Localization (EN)|Localization]]
- [[🏗️ Arquitetura (EN)|Architecture]]
- [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]]
- [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]]

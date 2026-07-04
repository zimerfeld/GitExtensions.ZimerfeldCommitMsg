---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquitetura, classes, design, i18n, application-idle]
---

# 🏗️ Architecture

> 🇧🇷 Leia esta página em português → [[🏗️ Arquitetura]]

## 🗺️ Class diagram

```
GitExtensions (host)
    │
    │  MEF (System.ComponentModel.Composition)
    ▼
ZimerfeldCommitMsgPlugin   ← [Export(IGitPlugin)] : GitPluginBase
    │  Register()   → captures IGitUICommands, registers the commit template,
    │                 subscribes to Application.Idle
    │  OnAppIdle()  → detects the freshly opened FormCommit and fills the message
    │  Execute()    → generates for the current repo (Plugins menu)
    │  GetSettings() → language selector (ChoiceSetting)
    ▼
CommitMessageGenerator      ← the engine
    │  Generate()  → consolidated subject + bullet body
    │  reads `git diff --cached`; classifies CC types; extracts comments; derives concepts
    ▼
git diff --cached (subprocess)
        +
Localization (LanguagePack / Strings / MessageLanguage)  → output language + UI strings
        +
RepoVocabularyConfig  ← .zimerfeldcommitmsg.json (extra per-repo vocabulary)
```

## 🧩 The classes

### 🔌 `ZimerfeldCommitMsgPlugin` — entry point and integration
Inherits from `GitPluginBase`, exported via MEF. Responsible for **entering the host's commit dialog** and injecting the message. See [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]].
- **`Register`** — captures the `IGitUICommands` and the `SynchronizationContext` (UI thread), registers the **three commit template items** (one per language) and subscribes to **`Application.Idle`**.
- **`OnAppIdle`** — detects the freshly opened `FormCommit` (there is no API event), generates the message and fills the box; gates by **instance** (`WeakReference<Form>`) and by **working dir** to avoid reprocessing on every tick, and **detects the dropdown choice** by watching the box's `TextChanged`.
- **`Execute`** — generates the message for the current repository (triggered from the Plugins menu).
- **`GetSettings`** — exposes the language `ChoiceSetting` (Automatic / Portuguese / English).

### ⚙️ `CommitMessageGenerator` — the engine
Reads `git diff --cached`, classifies files by **semantic category** (extension → source/web/docs/build/config) and by **Conventional Commits type**, extracts **comments** from the diff and derives **concepts** from file names, and assembles the **consolidated subject** + the **bullet body**. ~1200 lines of mostly pure functions (testable via `InternalsVisibleTo`). See [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] and [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]].

### 🌐 `Localization` — output language + UI strings
- `MessageLanguage` (`PtBr`/`En`) + `MessageLanguageResolver` (resolves "Automatic" via `CultureInfo.CurrentUICulture`).
- `LanguagePack` — verbs, type words, pluralization, etc., per language (used by the generator).
- `Strings` — UI strings (warnings) read from the embedded resources `Strings.resx`/`StringsPtBr.resx`. See [[🌐 Localization (EN)|Localization]].

### 📓 `RepoVocabularyConfig` — per-repository extension
Loads `.zimerfeldcommitmsg.json` from the working dir root (optional): `knownVocabulary`, `rejectedVocabulary`, `concepts`. Added on top of the built-in defaults, without recompiling. Parse failures are silent. See [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]].

## 🔌 Host integration

> [!important] The plugin **fills** the commit dialog without relying on a dedicated event
> GitExtensions does not expose "commit dialog opened" in the API, so the plugin watches for the `FormCommit` via **`Application.Idle`**. Choosing an item from the **template dropdown** is recognized indirectly — the host materializes the text of **all** items when the dropdown opens and, on click, only applies the text; the plugin records each generated text (msg → language) and detects via the box which one was chosen (`TextChanged`). See [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]].

The filling is **non-destructive**: if the box already contains user-typed text, the plugin does not overwrite it.

## 🌐 Localization (i18n)

Two independent axes:
1. **Message language** — `EffectiveLanguage()` = the dropdown choice (`_sessionLanguage`) takes priority over the setting/OS (`CurrentLanguage()`), which resolves "Automatic" via `CultureInfo.CurrentUICulture`.
2. **UI strings** — read by `Strings` from the embedded neutral resources (`InvariantCulture` avoids satellite assembly probing). See [[🌐 Localization (EN)|Localization]] and [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded strings without satellite assemblies]].

## 🧵 Threading

- `Register` runs on the UI thread and captures the `SynchronizationContext` for safe marshalling.
- `OnAppIdle` runs on the UI thread (UI event); the generation work is fast (`git diff --cached` subprocess), with gates to avoid repeating on every tick.

## 🔗 Links

- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[🌐 Localization (EN)|Localization]]
- [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)|Verb-first subject without type prefix]]
- [[🔗 Dependências (EN)|Dependencies]]

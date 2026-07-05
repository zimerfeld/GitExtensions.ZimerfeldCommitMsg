---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquivo, i18n, localização, resx, cultura]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/Localization/
---

# 🌐 Localization/ (MessageLanguage, LanguagePack, Strings)

> 🇧🇷 Leia esta página em português → [[🌐 Localization]]

Localization folder — two axes: **language of the generated message** and **UI strings**.

**Path:** `src/GitExtensions.ZimerfeldCommitMsg/Localization/`

---

## 🗣️ `MessageLanguage.cs`

- `enum MessageLanguage { PtBr, En }` — output language of the message.
- `LanguageOption` — **bilingual** labels of the `ChoiceSetting`: `Automático/Automatic`, `Português/Portuguese`, `Inglês/English` (clear regardless of the OS language).
- `MessageLanguageResolver`:
  - `Resolve(settingValue)` — matches by **substring** (tolerant to bilingual labels and old values): contains `portug` → PtBr; `ingl`/`english` → En; otherwise detects via the OS.
  - `FromCulture(culture)` — `pt-*` → PtBr; anything else → En (uses `CultureInfo.CurrentUICulture`).

---

## 📦 `LanguagePack.cs`

Per-language tables used by the [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]: verbs per Conventional Commits type, pluralization (`FilesWord`), the "file(s)" word, connectors, etc. ~418 lines. It is what makes the **same logic** produce output in pt-BR or English.

---

## 🧵 `Strings.cs` + `Resources/Strings.resx` / `StringsPtBr.resx`

**UI** strings (warning messages), read from the **embedded neutral** resources in the main assembly (no satellite assemblies — single-DLL deployment):

```csharp
Strings.RepoInvalido(lang)       // invalid repository
Strings.SemMudancasStaged(lang)  // no staged changes
Strings.PluginDescription(lang)  // plugin description (menu)
```

- Two `ResourceManager`s: `Strings` (en) and `StringsPtBr` (pt).
- `Get(key, lang)` uses **`InvariantCulture`** to avoid satellite assembly probing; fallback en → key.
- Selection is by the **resolved language**, not by the thread's global culture → the manual override is honored.

> [!note] Single-DLL deployment
> The `.resx` files are embedded with a fixed `LogicalName` in the `.csproj`, so MSBuild does not divert them into satellite assemblies. See [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded strings without satellite assemblies]] and [[🏷️ Versionamento (EN)|Versioning]].

---

## 🗣️ Effective language (in the plugin)

`EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()` — the dropdown choice takes priority over the setting/OS. See [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]] and [[🌐 3 - Idioma e auto-refresh (EN)|3 - Language and auto-refresh]].

---

## 🔗 Links

- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[🏗️ Arquitetura (EN)|Architecture]]
- [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded strings without satellite assemblies]]

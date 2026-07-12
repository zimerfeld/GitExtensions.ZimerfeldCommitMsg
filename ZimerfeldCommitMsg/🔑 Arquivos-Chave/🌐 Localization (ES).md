---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquivo, i18n, localização, resx, cultura]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/Localization/
---

# 🌐 Localization/ (MessageLanguage, LanguagePack, Strings)

> 🇧🇷 Portugués → [[🌐 Localization]] · 🇺🇸 English → [[🌐 Localization (EN)]]

Carpeta de localización — dos ejes: **idioma del mensaje generado** y **strings de UI**.

**Ruta:** `src/GitExtensions.ZimerfeldCommitMsg/Localization/`

---

## 🗣️ `MessageLanguage.cs`

- `enum MessageLanguage { PtBr, En, EsEs }` — idioma de salida del mensaje.
- `LanguageOption` — etiquetas **bilingües** del `ChoiceSetting`: `Automático/Automatic`, `Português/Portuguese`, `Inglês/English`, `Espanhol/Español` (claras independientemente del idioma del SO).
- `MessageLanguageResolver`:
  - `Resolve(settingValue)` — casa por **subcadena** (tolerante a etiquetas bilingües y valores antiguos): contiene `portug` → PtBr; `ingl`/`english` → En; `espa`/`spanish` → EsEs; en caso contrario detecta por el SO.
  - `FromCulture(culture)` — `pt-*` → PtBr; `es-*` → EsEs; cualquier otro → En (usa `CultureInfo.CurrentUICulture`).

---

## 📦 `LanguagePack.cs`

Tablas por idioma usadas por el [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]: verbos por tipo Conventional Commits, pluralización (`FilesWord`), palabra de "archivo(s)", conectores, etc. ~418 líneas. Es lo que hace que la **misma lógica** produzca salida en pt-BR, inglés o español. `LanguagePack.For` mapea cada `MessageLanguage`; el español lo cubre `EsEsLanguagePack` (conceptos, verbos, conjugación y palabras de enlace en español).

---

## 🧵 `Strings.cs` + `Resources/Strings.resx` / `StringsPtBr.resx` / `StringsEsEs.resx`

Strings de **UI** (mensajes de aviso), leídas de los recursos **neutros embebidos** en el assembly principal (sin satellite assemblies — deploy de una sola DLL):

```csharp
Strings.RepoInvalido(lang)       // repositorio inválido
Strings.SemMudancasStaged(lang)  // sin cambios en stage
Strings.PluginDescription(lang)  // descripción del plugin (menú)
```

- Tres `ResourceManager`: `Strings` (en), `StringsPtBr` (pt) y `EsRm` para `StringsEsEs.resx` (es, embebido con `LogicalName` `GitExtensions.ZimerfeldCommitMsg.Resources.StringsEsEs.resources`).
- `Get(key, lang)` usa **`InvariantCulture`** para evitar el probing de satellite assemblies; fallback en → clave.
- La selección es por el **idioma resuelto**, no por la cultura global del thread → el override manual se respeta.

> [!note] Deploy de una sola DLL
> Los `.resx` se embeben con `LogicalName` fijo en el `.csproj`, para que MSBuild no los desvíe a satellite assemblies. Ver [[📦 Strings embutidas sem satellite assemblies (ES)|Strings embebidas sin satellite assemblies]] y [[🏷️ Versionamento (ES)|Versionado]].

---

## 🗣️ Idioma efectivo (en el plugin)

`EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()` — la elección del dropdown tiene prioridad sobre el setting/SO. Ver [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]] y [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]].

---

## 🔗 Enlaces

- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[🏗️ Arquitetura (ES)|Arquitectura]]
- [[📦 Strings embutidas sem satellite assemblies (ES)|Strings embebidas sin satellite assemblies]]

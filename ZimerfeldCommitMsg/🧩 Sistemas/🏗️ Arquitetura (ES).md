---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquitetura, classes, design, i18n, application-idle]
---

# 🏗️ Arquitectura

> 🇧🇷 Portugués → [[🏗️ Arquitetura]] · 🇺🇸 English → [[🏗️ Arquitetura (EN)]]

## 🗺️ Diagrama de clases

```
GitExtensions (host)
    │
    │  MEF (System.ComponentModel.Composition)
    ▼
ZimerfeldCommitMsgPlugin   ← [Export(IGitPlugin)] : GitPluginBase
    │  Register()   → captura IGitUICommands, registra la plantilla de commit,
    │                 se suscribe a Application.Idle
    │  OnAppIdle()  → detecta el FormCommit recién abierto y rellena el mensaje
    │  Execute()    → genera para el repo actual (menú Plugins)
    │  GetSettings() → selector de idioma (ChoiceSetting)
    ▼
CommitMessageGenerator      ← el motor
    │  Generate()  → subject consolidado + cuerpo en bullets
    │  lee `git diff --cached`; clasifica tipos CC; extrae comentarios; deriva conceptos
    ▼
git diff --cached (subproceso)
        +
Localization (LanguagePack / Strings / MessageLanguage)  → idioma de salida + strings de UI
        +
RepoVocabularyConfig  ← .zimerfeldcommitmsg.json (vocabulario extra por repo)
```

## 🧩 Las clases

### 🔌 `ZimerfeldCommitMsgPlugin` — punto de entrada e integración
Hereda de `GitPluginBase`, exportado vía MEF. Responsable de **entrar en el diálogo de commit** del host e inyectar el mensaje. Ver [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]].
- **`Register`** — captura el `IGitUICommands` y el `SynchronizationContext` (hilo de UI), registra los **cuatro elementos de plantilla** de commit (uno por idioma) y se suscribe a **`Application.Idle`**.
- **`OnAppIdle`** — detecta el `FormCommit` recién abierto (no hay evento de API), genera el mensaje y rellena la caja; aplica gates por **instancia** (`WeakReference<Form>`) y por **working dir** para no reprocesar en cada tick, y **detecta la elección del dropdown** observando el `TextChanged` de la caja.
- **`Execute`** — genera el mensaje para el repositorio actual (accionado desde el menú Plugins).
- **`GetSettings`** — expone el `ChoiceSetting` de idioma (Automático / Portugués / Inglés / Español).

### ⚙️ `CommitMessageGenerator` — el motor
Lee el `git diff --cached`, clasifica los archivos por **categoría semántica** (extensión → source/web/docs/build/config) y por **tipo Conventional Commits**, extrae **comentarios** del diff y deriva **conceptos** de los nombres de archivo, y monta el **subject consolidado** + el **cuerpo en bullets**. ~1200 líneas de funciones en su mayoría puras (testables vía `InternalsVisibleTo`). Ver [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] y [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]].

### 🌐 `Localization` — idioma de salida + strings de UI
- `MessageLanguage` (`PtBr`/`En`/`EsEs`) + `MessageLanguageResolver` (resuelve "Automático" vía `CultureInfo.CurrentUICulture`).
- `LanguagePack` — verbos, palabras de tipo, pluralización, etc., por idioma (usado por el generador).
- `Strings` — strings de UI (avisos) leídas de los recursos embebidos `Strings.resx`/`StringsPtBr.resx`/`StringsEsEs.resx`. Ver [[🌐 Localization (ES)|Localización]].

### 📓 `RepoVocabularyConfig` — extensión por repositorio
Carga `.zimerfeldcommitmsg.json` de la raíz del working dir (opcional): `knownVocabulary`, `rejectedVocabulary`, `concepts`. Sumado a los defaults embebidos, sin recompilar. Los fallos de parseo son silenciosos. Ver [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]].

## 🔌 Integración con el host

> [!important] El plugin **rellena** el diálogo de commit sin depender de un evento dedicado
> GitExtensions no expone "diálogo de commit abierto" en la API, así que el plugin observa el `FormCommit` vía **`Application.Idle`**. La elección de un elemento del **dropdown de plantillas** se reconoce indirectamente — el host materializa el texto de **todos** los elementos al abrir el dropdown y, al hacer clic, solo aplica el texto; el plugin registra cada texto generado (msg → idioma) y detecta por la caja cuál fue elegido (`TextChanged`). Ver [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía dropdown de plantillas y Application.Idle]].

El rellenado es **no destructivo**: si la caja ya contiene texto escrito por el usuario, el plugin no lo sobrescribe.

## 🌐 Localización (i18n)

Dos ejes independientes:
1. **Idioma del mensaje** — `EffectiveLanguage()` = la elección del dropdown (`_sessionLanguage`) tiene prioridad sobre el setting/SO (`CurrentLanguage()`), que resuelve "Automático" vía `CultureInfo.CurrentUICulture`.
2. **Strings de UI** — leídas por `Strings` de los recursos neutros embebidos (`InvariantCulture` evita el probing de satellite assemblies). Ver [[🌐 Localization (ES)|Localización]] y [[📦 Strings embutidas sem satellite assemblies (ES)|Strings embebidas sin satellite assemblies]].

## 🧵 Threading

- `Register` corre en el hilo de UI y captura el `SynchronizationContext` para un marshalling seguro.
- `OnAppIdle` corre en el hilo de UI (evento de UI); el trabajo de generación es rápido (subproceso `git diff --cached`), con gates para no repetir en cada tick.

## 🔗 Enlaces

- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[🌐 Localization (ES)|Localización]]
- [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía dropdown de plantillas y Application.Idle]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo sin prefijo de tipo]]
- [[🔗 Dependências (ES)|Dependencias]]

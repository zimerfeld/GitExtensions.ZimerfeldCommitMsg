---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquivo, plugin, entry-point, mef, application-idle]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs
---

# 🔌 ZimerfeldCommitMsgPlugin.cs

> 🇧🇷 Portugués → [[🔌 ZimerfeldCommitMsgPlugin]] · 🇺🇸 English → [[🔌 ZimerfeldCommitMsgPlugin (EN)]]

Punto de entrada del plugin e **integración con el diálogo de commit** de GitExtensions. Exportado vía MEF. ~626 líneas.

**Ruta:** `src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs`

---

## 📜 Declaración

```csharp
[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
```

El atributo `[Export]` es el punto de descubrimiento por el MEF del host. Ver [[🧩 Plugin MEF para GitExtensions (ES)|Plugin MEF para GitExtensions]].

---

## 🧩 Ítems de plantilla y setting

- `TemplatePrefix = "Zimerfeld Commit Msg"`.
- `_templateItems` — cuatro ítems `(Label, MessageLanguage?)`: Automático (`null`), Português (`PtBr`), Inglês (`En`), Espanhol (`EsEs`). La API de plantillas del host es **plana**, así que se expone un ítem por idioma.
- `_languageSetting` — `ChoiceSetting("ZimerfeldCommitMsg_Language", …)` con las opciones Automático/Português/Inglês/Espanhol (default Automático), expuesto por `GetSettings()`.

---

## 🗃️ Campos de instancia (gates y estado)

| Campo | Propósito |
|---|---|
| `_syncContext` | `SynchronizationContext` capturado en `Register` (UI thread) para el marshalling |
| `_gitUiCommands` | fuente del working dir para el disparador de `Application.Idle` |
| `_handledCommitForm` | `WeakReference<Form>` — FormCommit ya rellenado (evita reprocesar en cada Idle) |
| `_handledWorkingDir` | working dir del último rellenado (el host reaprovecha el FormCommit al cambiar de repo) |
| `_sessionLanguage` | idioma fijado por la elección de un ítem del dropdown (`null` = seguir setting/SO) |
| `_templateMessages` | mapa texto-generado → idioma del ítem, para reconocer la elección del dropdown |
| `_subscribedTextBox` | `WeakReference<TextBoxBase>` de la caja en la que ya suscribimos `TextChanged` |
| `_resyncedCommitForm` | garantiza UNA re-sincronización de apertura por ventana |

---

## 🗣️ Idioma efectivo

```csharp
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage();
CurrentLanguage()   = MessageLanguageResolver.Resolve(_languageSetting.ValueOrDefault(Settings));
```
La elección del dropdown tiene prioridad sobre el setting/SO. Ver [[🌐 Localization (ES)|Localization]] y [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]].

---

## ⚙️ Métodos (IGitPlugin)

### `Register(IGitUICommands)`
- `base.Register` + captura `_gitUiCommands` y `_syncContext`.
- Registra los **cuatro ítems de plantilla** en el `CommitTemplateManager` (un `Func` de generación por idioma).
- Suscribe **`Application.Idle += OnAppIdle`** — el host no expone "diálogo de commit abierto", así que detectamos el `FormCommit` en el Idle.

### `OnAppIdle`
- Recorre `Application.OpenForms` por un `Form` cuyo `GetType().Name == "FormCommit"`.
- Gates por **instancia** (`_handledCommitForm`) y por **working dir** (`_handledWorkingDir`).
- Rellena la caja de mensaje si está vacía (**no destructivo**); suscribe `TextChanged` para detectar la elección del dropdown. Ver [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]].

### `GenerateForTemplate(MessageLanguage? forced)`
- Genera el mensaje para un ítem del dropdown, en el idioma del ítem.
- **No fija el idioma aquí** (el host llama a este `Func` para todos los ítems al abrir el dropdown). En su lugar, `RememberTemplateMessage(msg, forced)` registra el texto → idioma para reconocer después qué ítem eligió el usuario (`DetectTemplateSelection`). Ver [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown y Application.Idle]].

### `ResolveCommitWorkingDir()`
- Prefiere el working dir del propio `FormCommit` abierto (vía reflection), con reserva al `_gitUiCommands.Module.WorkingDir` — evita generar en el repo equivocado/vacío.

### `Execute(GitUIEventArgs)` ← menú Plugins → ZimerfeldCommitMsg
- Genera el mensaje para el repositorio actual usando el idioma del setting.

### `Unregister(IGitUICommands)`
- Desregistra la plantilla, `Application.Idle -= OnAppIdle`, limpia el commands capturado.

---

## 🛡️ Protección contra crash

Delegates, reflection sobre el `FormCommit` y la generación están envueltos en `try/catch` — las excepciones del plugin nunca tumban GitExtensions.

---

## 🔗 Enlaces

- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[🌐 Localization (ES)|Localization]]
- [[🏗️ Arquitetura (ES)|Arquitectura]]
- [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown y Application.Idle]]
- [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]]

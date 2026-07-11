---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [conhecimento, csharp, gitextensions, mef, plugin]
---

# 🧩 Plugin MEF para GitExtensions

> 🇧🇷 Portugués → [[🧩 Plugin MEF para GitExtensions]] · 🇺🇸 English → [[🧩 Plugin MEF para GitExtensions (EN)]]

## 📝 Resumen
GitExtensions carga plugins vía **MEF** (Managed Extensibility Framework). El entry point es una clase exportada que implementa `IGitPlugin` (normalmente heredando de `GitPluginBase`).

## 🔑 Puntos clave
- Exportar con `[Export(typeof(IGitPlugin))]` usando `System.ComponentModel.Composition`.
- El proyecto compila como **`Library`** (DLL), `net9.0-windows`, con WinForms habilitado.
- Referenciar los assemblies del host con **`<Private>false</Private>`** (no copiar a la salida — el host ya los tiene). Aquí quedan **versionados en `refs\`** (build determinista y offline):
  - `GitExtensions.Extensibility.dll`
  - `System.ComponentModel.Composition.dll`
- El `AssemblyName` debe coincidir con lo que `install.ps1` / nuspec esperan (`GitExtensions.Plugins.<Nombre>`).
- Para aparecer en el **Plugin Manager** interno, el paquete NuGet debe **depender** de `GitExtensions.Extensibility` (dependencia marcadora). Ver [[🔗 Dependências (ES)|Dependencias]].

## ♻️ Ciclo de vida del plugin
- `Register(IGitUICommands)` — llamado al cargar. Buen lugar para **capturar el `IGitUICommands`**, registrar templates de commit y suscribir eventos de la UI (aquí: `Application.Idle`).
- `Unregister(IGitUICommands)` — desuscribir / limpiar la referencia capturada.
- `Execute(GitUIEventArgs)` — disparado por el menú **Plugins → \<nombre\>**.
- `GetSettings()` — expone settings configurables (aquí: el selector de idioma).

## ⚙️ Cómo usa este modelo ZimerfeldCommitMsg
- Registra **cuatro elementos de template** de commit (uno por idioma) vía `CommitTemplateManager`.
- Se suscribe a **`Application.Idle`** para detectar el `FormCommit` recién abierto y rellenar el mensaje — el host **no** expone un evento de "diálogo de commit abierto". Ver [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]] y [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown + Application.Idle]].
- `Execute` genera el mensaje para el repo actual (menú Plugins).
- `GetSettings` expone el `ChoiceSetting` de idioma (Automático/PT/EN/ES).

## 🔀 Contraste con los hermanos
- `GitExtensions.ZimerfeldLFS` y `GitExtensions.ZimerfeldTree` abren **ventanas propias** (no modales). En cambio, CommitMsg **no tiene ventana**: se integra en el diálogo de commit existente del host.

## 🔗 Relacionado
- [[📦 GitExtensions.ZimerfeldCommitMsg (ES)|GitExtensions.ZimerfeldCommitMsg]]
- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]
- [[🔗 Dependências (ES)|Dependencias]]

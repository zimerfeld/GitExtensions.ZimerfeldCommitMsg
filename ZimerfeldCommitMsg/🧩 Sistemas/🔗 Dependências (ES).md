---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [dependências, assemblies, gitextensions, nuget]
---

# 🔗 Dependencias

> 🇧🇷 Portugués → [[🔗 Dependências]] · 🇺🇸 English → [[🔗 Dependências (EN)]]

## 🧩 Assemblies de GitExtensions (referencias de compilación)

Ambas referenciadas con `<Private>false</Private>` — **no** copiadas al output (el host ya las provee en runtime).

| Assembly | Ruta | Uso |
|---|---|---|
| `GitExtensions.Extensibility.dll` | `refs\` (versionado en el repo) | `IGitPlugin`, `GitPluginBase`, `IGitUICommands`, `ISetting`/`ChoiceSetting`, `CommitTemplateManager` |
| `System.ComponentModel.Composition.dll` | `refs\` (versionado en el repo) | MEF — `[Export(typeof(IGitPlugin))]` |

> **Build determinista (cualquier máquina Windows):** los assemblies de referencia quedan **versionados en `refs\`** (apuntados por `$(GitExtensionsRefPath)` en el `.csproj`), **no** descargados en prebuild. Garantiza una compilación reproducible y **offline**. Una descarga anterior podía traer el asset arm64 (6.0.5.75) incompatible con el x64 instalado (6.0.5.18375) — de ahí el versionado en `refs\` (ver `refs/README.md`). El `.csproj` rebaja el aviso `MSB3277` (conflicto benigno entre el ref pack net9 4.0 y el VS.Threading 8.0 del host — resuelto en runtime).

## 📦 Dependencia del paquete NuGet (marcador del Plugin Manager)

```xml
<dependency id="GitExtensions.Extensibility" version="[0.4.0, 0.5.0)" />
```

> [!important] Por qué existe la dependencia marcadora
> El Plugin Manager de GitExtensions filtra el feed de nuget.org por paquetes que **dependen** de
> `GitExtensions.Extensibility`. **Sin** esa dependencia, el paquete se publica pero **nunca aparece**
> en el Plugin Manager interno. Además, el filtro compara el **rango de versión** de la dependencia con la
> versión que el **manager anuncia** para el host en ejecución (**no** el runtime instalado): el manager
> v3.x de GitExtensions 6.x anuncia `0.4.0`, así que el rango debe **contener** 0.4.0 → `[0.4.0, 0.5.0)`,
> igual que los demás plugins que funcionan en GE6 (AITools, BundleBackuper, Gerrit, SolutionRunner…).
> Un valor suelto como `1.0.0.129` significa `>= 1.0.0.129`, que **no** incluye 0.4.0 — y el paquete
> se **filtraba silenciosamente hacia fuera** del Plugin Manager. Para GitExtensions 7 (el manager
> anuncia `7.0.0`), usar `[7.0.0, 8.0.0)`. Alineado con [[GitExtensions.ZimerfeldTree]].

## 📦 Empaquetado (nuspec)

- DLL en **`lib\` raíz** (grupo "any" que el Plugin Manager extrae) — genera el aviso **NU5101**, intencional y filtrado en el `build.ps1`. Ver [[🏷️ Versionamento (ES)|Versionado]].
- La misma DLL también en `tools\net9.0-windows\` para la instalación vía **Package Manager Console** (`install.ps1`).
- `LICENSE.txt` (CC BY-NC-ND 4.0, `type="file"`), `README.md`/`README.pt-BR.md`/`README.en-US.md`, e `icon-128.png` (en `images\`) en el paquete.

## 🔑 Interfaces clave usadas

### `IGitPlugin` (vía `GitPluginBase`)
- `Register(IGitUICommands)` / `Unregister(IGitUICommands)` — captura/limpia el commands, registra/desregistra la plantilla de commit y suscribe/desuscribe `Application.Idle`
- `Execute(GitUIEventArgs)` — menú Plugins → ZimerfeldCommitMsg
- `GetSettings()` — expone el `ChoiceSetting` de idioma

### `IGitUICommands` / host
- `Module.WorkingDir` — working dir usado para ejecutar `git diff --cached`
- `CommitTemplateManager` — registro de los elementos de plantilla (uno por idioma) en el dropdown del diálogo de commit
- `FormCommit` (vía reflection sobre `Application.OpenForms`) — la caja de mensaje a rellenar

## ✅ Runtime (lo que el usuario necesita tener)

| Requisito | Valor |
|---|---|
| GitExtensions | 6.x (.NET 9) |
| .NET | 9.0 (Windows) — provisto por el host |
| `git` | en el `PATH` (el generador ejecuta `git diff --cached`) |
| PowerShell | 5.1+ (scripts de build/deploy) |
| .NET 9 SDK + nuget | para compilar y empaquetar |

## 🔗 Enlaces

- [[🏗️ Arquitetura (ES)|Arquitectura]]
- [[🏷️ Versionamento (ES)|Versionado]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]

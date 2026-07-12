---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [dependências, assemblies, gitextensions, nuget]
---

# 🔗 Dependencies

> 🇧🇷 Português → [[🔗 Dependências]] · 🇪🇸 Español → [[🔗 Dependências (ES)]]

## 🧩 GitExtensions assemblies (compile references)

Both referenced with `<Private>false</Private>` — **not** copied to the output (the host already provides them at runtime).

| Assembly | Path | Usage |
|---|---|---|
| `GitExtensions.Extensibility.dll` | `refs\` (versioned in the repo) | `IGitPlugin`, `GitPluginBase`, `IGitUICommands`, `ISetting`/`ChoiceSetting`, `CommitTemplateManager` |
| `System.ComponentModel.Composition.dll` | `refs\` (versioned in the repo) | MEF — `[Export(typeof(IGitPlugin))]` |

> **Deterministic build (any Windows machine):** the reference assemblies are **versioned in `refs\`** (pointed to by `$(GitExtensionsRefPath)` in the `.csproj`), **not** downloaded in prebuild. This guarantees a reproducible and **offline** compilation. A previous download could bring the arm64 asset (6.0.5.75), incompatible with the installed x64 (6.0.5.18375) — hence versioning them in `refs\` (see `refs/README.md`). The `.csproj` demotes the `MSB3277` warning (benign conflict between the net9 4.0 ref pack and the host's VS.Threading 8.0 — resolved at runtime).

## 📦 NuGet package dependency (Plugin Manager marker)

```xml
<dependency id="GitExtensions.Extensibility" version="[0.4.0, 0.5.0)" />
```

> [!important] Why the marker dependency exists
> The GitExtensions Plugin Manager filters the nuget.org feed for packages that **depend** on
> `GitExtensions.Extensibility`. **Without** this dependency, the package is published but **never shows up**
> in the built-in Plugin Manager. Furthermore, the filter matches the dependency's **version range** against the
> version the **manager advertises** for the running host (**not** the installed runtime): the v3.x manager
> of GitExtensions 6.x advertises `0.4.0`, so the range must **contain** 0.4.0 → `[0.4.0, 0.5.0)`,
> just like the other plugins that work on GE6 (AITools, BundleBackuper, Gerrit, SolutionRunner…).
> A loose value such as `1.0.0.129` means `>= 1.0.0.129`, which does **not** include 0.4.0 — and the package
> was **silently filtered out** of the Plugin Manager. For GitExtensions 7 (the manager
> advertises `7.0.0`), use `[7.0.0, 8.0.0)`. Aligned with [[GitExtensions.ZimerfeldTree]].

## 📦 Packaging (nuspec)

- DLL in the **root `lib\`** (the "any" group that the Plugin Manager extracts) — produces the **NU5101** warning, intentional and filtered in `build.ps1`. See [[🏷️ Versionamento (EN)|Versioning]].
- The same DLL also in `tools\net9.0-windows\` for installing via **Package Manager Console** (`install.ps1`).
- `LICENSE.txt` (CC BY-NC-ND 4.0, `type="file"`), `README.md`/`README.pt-BR.md`/`README.en-US.md`, and `icon-128.png` (in `images\`) in the package.

## 🔑 Key interfaces used

### `IGitPlugin` (via `GitPluginBase`)
- `Register(IGitUICommands)` / `Unregister(IGitUICommands)` — captures/clears the commands, registers/unregisters the commit template and subscribes/unsubscribes `Application.Idle`
- `Execute(GitUIEventArgs)` — Plugins menu → ZimerfeldCommitMsg
- `GetSettings()` — exposes the language `ChoiceSetting`

### `IGitUICommands` / host
- `Module.WorkingDir` — working dir used to run `git diff --cached`
- `CommitTemplateManager` — registration of the template items (one per language) in the commit dialog dropdown
- `FormCommit` (via reflection over `Application.OpenForms`) — the message box to fill

## ✅ Runtime (what the user needs)

| Requirement | Value |
|---|---|
| GitExtensions | 6.x (.NET 9) |
| .NET | 9.0 (Windows) — provided by the host |
| `git` | on the `PATH` (the generator runs `git diff --cached`) |
| PowerShell | 5.1+ (build/deploy scripts) |
| .NET 9 SDK + nuget | to compile and package |

## 🔗 Links

- [[🏗️ Arquitetura (EN)|Architecture]]
- [[🏷️ Versionamento (EN)|Versioning]]
- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]

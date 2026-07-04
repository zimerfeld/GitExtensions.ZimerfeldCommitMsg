---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [build, versão, nupkg, deploy]
---

# 🏷️ Versioning and Build

> 🇧🇷 Leia esta página em português → [[🏷️ Versionamento]]

## 🔢 Version scheme

`major.minor.build` — only `build` is incremented automatically by `build.ps1`. Major and minor are changed manually.

**Current version:** `1.0.97` *(source of truth: `.nuspec` / `.csproj`)*

> [!note] Embedded UI strings (no satellite assemblies)
> UI strings live in `Resources/Strings.resx` and `Resources/StringsPtBr.resx`, embedded into the
> **main assembly** with an explicit `LogicalName` — so MSBuild does **not** divert them into
> satellite assemblies and deployment remains a **single DLL**. Read at runtime by `Strings`.
> See [[📦 Strings embutidas sem satellite assemblies (EN)|Embedded strings without satellite assemblies]].

## 🔄 build.ps1 cycle

```
build.ps1  [-Force]
  │
  ├─ 1. Reads the current version from the .nuspec
  ├─ 2. Computes newVersion (build +1)
  ├─ 2b. Detects changes (sources/docs newer than the last .nupkg); without -Force and with
  │      no change → keeps the version and exits (build/pack skipped)
  ├─ 2c. Closes GitExtensions if it is running
  ├─ 3. Bumps the .nuspec  ← <version>
  ├─ 4. Bumps the .csproj  ← <Version>
  ├─ 4b. Stamps version + date at the top of the READMEs (README.md / .pt-BR / .en-US)
  ├─ 5. dotnet build -c Release
  ├─ 6. Copies the DLL → C:\Program Files\GitExtensions\Plugins\  (requires Admin)
  ├─ 6b. Copies the DLL → tools\net9.0-windows\  (for the nupkg)
  ├─ 7. nuget pack .nuspec → .nupkg at the root (filters the NU5101 warning)
  └─ 7b. Removes .nupkg files of previous versions
```

> [!warning] Warning **NU5101** is intentional
> The DLL is packaged into the **root** `lib\` (the "any" group that the Plugin Manager extracts), not into
> `lib\net9.0-windows\`. This produces the NU5101 warning, which `build.ps1` **filters on purpose** in
> `nuget pack`. Details in [[🔗 Dependências (EN)|Dependencies]] and in the `.nuspec`.

<!-- -->

> Requires the **.NET 9 SDK** (`dotnet`) and, for deployment, **Administrator** permission. Without Admin, the deploy step is skipped with a warning; `nuget` lives at `tools\nuget.exe`.

## 📄 Versioned files

| File | Updated field |
|---|---|
| `GitExtensions.ZimerfeldCommitMsg.nuspec` | `<version>` |
| `GitExtensions.ZimerfeldCommitMsg.csproj` | `<Version>` |
| `README.md` / `README.pt-BR.md` / `README.en-US.md` | `**Version/Versão:**` and `**Updated/Atualizado em:**` |

## 🧰 Manual install / uninstall

```powershell
tools\install.ps1        # requires Admin — copies the DLL to the Plugins folder (also via PMC)
tools\uninstall.ps1      # requires Admin — removes the DLL (does not affect the rest of GitExtensions)
tools\update-dll.ps1     # updates only the DLL in the Plugins folder
```

## 🧪 Tests

**xUnit** suite in `tests\GitExtensions.ZimerfeldCommitMsg.Tests\` (the generator's pure functions are exposed via `InternalsVisibleTo`):
- `CommentExtractionTests` — comment recognition and cleanup (several syntaxes)
- `ConceptExtractionTests` — concept derivation from file names
- `RepoVocabularyConfigTests` — loading of `.zimerfeldcommitmsg.json`
- `TranslationTests` — English → Portuguese translation

## 🔗 Links

- [[🛠️ build.ps1 (EN)|build.ps1]]
- [[🔗 Dependências (EN)|Dependencies]]
- [[💻 Ambiente Local (Dev) (EN)|Local Environment (Dev)]]
- [[🚀 Deploy em Produção (Prod) (EN)|Production Deploy (Prod)]]

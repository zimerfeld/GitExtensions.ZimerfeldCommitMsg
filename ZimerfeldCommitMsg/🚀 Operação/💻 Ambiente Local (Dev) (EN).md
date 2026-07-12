---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [operação, dev, build, install, powershell]
---

# 💻 Local Environment (Dev)

> 🇧🇷 Português → [[💻 Ambiente Local (Dev)]] · 🇪🇸 Español → [[💻 Ambiente Local (Dev) (ES)]]

How to build and install the plugin into the local GitExtensions to develop and test.

## ⚡ TL;DR — the single command

```powershell
# at the repo root, PowerShell as Administrator (for the Program Files deploy)
.\build.ps1 -Force
```

Compiles in Release, deploys the DLL to `C:\Program Files\GitExtensions\Plugins\` and packs the `.nupkg`. Restart GitExtensions to load the new DLL.

## ⚙️ What the script does (in order)

`build.ps1` (details in [[🛠️ build.ps1 (EN)|build.ps1]] and [[🏷️ Versionamento (EN)|Versioning]]):
1. Reads the current version from the `.nuspec` and computes `newVersion` (build +1).
2. Detects changes (sources/docs newer than the last `.nupkg`); without `-Force` and with no change → keeps the version and exits.
3. Closes GitExtensions if it is running (releases the DLL).
4. Stamps version + date into the READMEs and vault notes; bumps the `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copies the DLL to `C:\Program Files\GitExtensions\Plugins\` (Admin) and to `tools\net9.0-windows\`.
7. `nuget pack` → `.nupkg` at the root (filters the NU5101 warning) and removes old `.nupkg` files.

## 🧰 Manual install (alternative to build.ps1)

```powershell
tools\install.ps1      # requires Admin — copies the DLL to the Plugins folder (also via NuGet PMC)
tools\uninstall.ps1    # requires Admin — removes the DLL
tools\update-dll.ps1   # updates just the DLL in the Plugins folder
```
`install.ps1` runs two ways: standalone (`cd tools; .\install.ps1`) or via the **NuGet Package Manager Console** (`Install-Package GitExtensions.ZimerfeldCommitMsg -Source C:\NUGET`). If the DLL is missing in `bin\Release`, it triggers a `build.ps1 -Force`.

## 🧪 Tests

```powershell
dotnet test tests\GitExtensions.ZimerfeldCommitMsg.Tests
```
**xUnit** suite: comment extraction, concept derivation, `.zimerfeldcommitmsg.json`, EN→PT translation. The generator's pure functions are exposed by `InternalsVisibleTo`.

## 🚦 Flags and requirements
- `-Force` — recompiles/repackages even without a detected change.
- **.NET 9 SDK** (`dotnet`) required.
- **Administrator** required for the deploy to `C:\Program Files\GitExtensions\Plugins\`. Without Admin, the deploy step is skipped with a warning.
- **GitExtensions 6.x (.NET 9)** installed; `git` on the `PATH`.

## 🔀 Rules it respects (GitFlow)
- Development on a **feature branch**; do not commit/push without an explicit request.
- The version/docs stamp is local — the commit is up to the user.

## 🩺 Troubleshooting
- **DLL locked / not updating:** close GitExtensions (`build.ps1` already tries to close it) and run again.
- **Deploy skipped:** run PowerShell as **Administrator**.
- **Plugin does not appear:** confirm the DLL is in `C:\Program Files\GitExtensions\Plugins\` and **restart** GitExtensions. The Settings → Plugins node only appears after restarting.

## 🔗 Links
- [[🚀 Deploy em Produção (Prod) (EN)|Production Deploy (Prod)]]
- [[🛠️ build.ps1 (EN)|build.ps1]]
- [[🏷️ Versionamento (EN)|Versioning]]
- [[📖 README — Instalação, Uso e Build (EN)|README — Install, Use and Build]]

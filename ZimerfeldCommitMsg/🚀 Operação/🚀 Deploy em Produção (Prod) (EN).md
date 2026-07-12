---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [operação, deploy, release, nupkg, nuget, github]
---

# 🚀 Production Deploy (Prod)

> 🇧🇷 Português → [[🚀 Deploy em Produção (Prod)]] · 🇪🇸 Español → [[🚀 Deploy em Produção (Prod) (ES)]]

How to publish a new plugin version: generate the versioned `.nupkg` and distribute it (NuGet + GitHub release). Distribution to end users is via the **GitExtensions Plugin Manager** and **NuGet**.

## ⚡ TL;DR — the single command

```powershell
# at the repo root (Admin), generates the release .nupkg with the bumped version
.\build.ps1
```

The production `.nupkg` lands at the **repo root** (`GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`). Publish it to NuGet and attach it to the GitHub release.

## ⚙️ What the script does (in order)

`build.ps1` (version source of truth: `.nuspec`/`.csproj`):
1. Reads the current version and computes `newVersion` (build +1).
2. Detects changes; without `-Force` and with no change → does not produce a new package.
3. Closes GitExtensions.
4. Stamps version + date into the **READMEs** and vault notes mirroring the current version; bumps the `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copies the DLL to `tools\net9.0-windows\` (nupkg source).
7. `nuget pack` → `.nupkg` at the root; removes older `.nupkg` files. The DLL goes into the **root** `lib\` ("any" group) — hence the **NU5101** warning, deliberately filtered.

## 📤 Publishing (after the build)

```powershell
# 1) Publish to NuGet.org (requires API key)
nuget push .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey <API_KEY>

# 2) Create the GitHub release with the version tag and attach the .nupkg
gh release create vX.Y.Z .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg --title "vX.Y.Z" --notes "..."
```
> Social proof in the README: `shields.io/nuget/v` (version) and `/dt` (downloads) badges — they update by themselves after the NuGet publish.

## 🚦 Requirements
- **.NET 9 SDK**, `nuget.exe` (in `tools\nuget.exe`), NuGet API key, authenticated `gh`.
- Version synced in `.nuspec` **and** `.csproj` (`build.ps1` ensures this).

## 🔀 Rules it respects (GitFlow)
- Do **not** deploy production from a **release branch**: validate in develop on the release branch → finish the release updating `develop` and `main` → create the **tag** `YYYYMMddhhmm<phase>` → only then publish.
- Ensure `main` is the **default** branch (reflects production).
- Do not commit/push/publish without an explicit user request.

## 🩺 Troubleshooting
- **NU5101 on packing:** expected (DLL in the root `lib\` for the Plugin Manager to extract) — `build.ps1` filters it.
- **Package not generated:** run `.\build.ps1 -Force` (with no detected change, the build is skipped).
- **Plugin Manager does not list it:** confirm the marker dependency `GitExtensions.Extensibility` in the `.nuspec`. See [[🔗 Dependências (EN)|Dependencies]].

## 🔗 Links
- [[💻 Ambiente Local (Dev) (EN)|Local Environment (Dev)]]
- [[🛠️ build.ps1 (EN)|build.ps1]]
- [[🏷️ Versionamento (EN)|Versioning]]
- [[🔗 Dependências (EN)|Dependencies]]

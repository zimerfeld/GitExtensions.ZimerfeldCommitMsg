# `refs/` — pinned GitExtensions reference assemblies

These DLLs are the **compile-time reference assemblies** for the plugin. They are
committed to the repository on purpose so the project builds **deterministically on any
Windows machine**, with no network access and no surprises about which CPU architecture
gets pulled in.

| File | Version | Source |
|------|---------|--------|
| `GitExtensions.Extensibility.dll` | `6.0.5.18375` (x64) | GitExtensions 6.0.5 (x64 release) |
| `System.ComponentModel.Composition.dll` | `6.0.21.52210` | GitExtensions 6.0.5 (x64 release) |

They are referenced from
[`GitExtensions.ZimerfeldCommitMsg.csproj`](../src/GitExtensions.ZimerfeldCommitMsg/GitExtensions.ZimerfeldCommitMsg.csproj)
via `$(GitExtensionsRefPath)` with `<Private>false</Private>` — i.e. compile-time only.
GitExtensions supplies the real assemblies at runtime, so nothing here is copied to the
build output or shipped in the `.nupkg`.

## Why pinned instead of downloaded

Previously the build relied on the `GitExtensions.Extensibility` NuGet package, whose
prebuild step downloaded a GitExtensions release into `gitextensions.shared/` (gitignored).
That download (`Download-GitExtensions.ps1`) selects the **first asset whose name contains
`portable` and ends in `.zip`** — it is **not** architecture-aware. On a fresh clone it
could therefore fetch an **arm64** build (e.g. `6.0.5.75-arm64`) instead of the x64 build
the user actually runs (`6.0.5.18375`). The API surfaces differ between those builds, which
produced a runtime `Method not found: …IGitUICommands.AddCommitTemplate(…)` crash. The
download also required network access, so a fresh clone could not build offline.

Pinning the reference assemblies removes both problems: the compile is reproducible and
offline-capable, and it always targets the x64 6.0.5 API.

## Updating

Replace these files with the matching DLLs from a newer GitExtensions **x64** release (and
bump `GitExtensionsReferenceVersion`-style expectations in the README if the target version
changes). The plugin guards its host calls (`try/catch` around
`AddCommitTemplate`), so a minor host/reference skew degrades gracefully instead of crashing.

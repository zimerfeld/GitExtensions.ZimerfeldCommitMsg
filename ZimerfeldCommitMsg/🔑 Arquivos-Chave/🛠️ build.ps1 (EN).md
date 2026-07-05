---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquivo, build, powershell, versão, nupkg, deploy]
arquivo: build.ps1
---

# 🛠️ build.ps1

> 🇧🇷 Leia esta página em português → [[🛠️ build.ps1]]

The plugin's **build + versioning + deploy + packaging** script.

**Path:** `build.ps1` (repo root)

---

## ⚙️ What it does

1. **Reads the current version** from the `.nuspec` and computes `newVersion` (`major.minor.build+1`).
2. **Detects changes** — compares the timestamps of the inputs (`*.cs`, `*.csproj`, `*.nuspec`, `*.resx`, `*.png`, `*.md`) against the last `.nupkg`. Without `-Force` and with no change → keeps the version and exits.
3. **Closes GitExtensions** if it is running (releases the DLL for deploy).
4. **Stamps version + date into the docs FIRST** (READMEs and Obsidian vault), then bumps the source of truth (`.nuspec`/`.csproj`) — so the docs already reflect the new version.
5. **`dotnet build -c Release`**.
6. **Deploy** — copies the DLL to `C:\Program Files\GitExtensions\Plugins\` (requires Admin) and to `tools\net9.0-windows\` (for the nupkg).
7. **`nuget pack`** → `.nupkg` at the root (deliberately filters the **NU5101** warning) and removes old `.nupkg` files.

---

## 🏷️ Version stamp

### 📄 READMEs (section 2a)
`README.md`, `README.pt-BR.md`, `README.en-US.md` — updates `**Version/Versão:**` and the date.

### 🧠 Obsidian vault (section 2b)
Only the notes that mirror the **current** version:
- `ZimerfeldCommitMsg\💼 Negócio\📦 GitExtensions.ZimerfeldCommitMsg.md`
- `ZimerfeldCommitMsg\📚 Conhecimento\📖 README — Instalação, Uso e Build.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🏷️ Versionamento.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🔭 Visão Geral.md`

Replaces `versao: …` in the frontmatter and the `**X.Y.Z**` / `` `X.Y.Z` `` occurrences of the previous version.

> [!warning] Stamp paths × vault v2 restructure (2026-07-04)
> The vault was restructured to the "Cofre de Neurônios v2" standard and the 4 notes above **changed folder/name** (before: `🚀 Projetos\GitExtensions.ZimerfeldCommitMsg.md`, `📚 Conhecimento\README — Instalação, Uso e Build.md`, `🏛 Sistema\Versionamento.md`, `🏛 Sistema\Visão Geral.md`). The **section 2b of `build.ps1` must be updated** to the new paths — tracked in [[📌 Backlog (EN)]]. Also consider stamping the `(EN)` pairs of those notes.

---

## 💻 Usage

```powershell
.\build.ps1          # incremental build (only if something changed)
.\build.ps1 -Force   # always recompile/repackage
```

Requires **.NET 9 SDK** and, for the deploy to `Program Files`, **Administrator**.

---

## 🔗 Links

- [[🏷️ Versionamento (EN)|Versioning]]
- [[🔗 Dependências (EN)|Dependencies]]
- [[💻 Ambiente Local (Dev) (EN)|Local Environment (Dev)]]
- [[🚀 Deploy em Produção (Prod) (EN)|Production Deploy (Prod)]]

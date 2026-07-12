---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquivo, build, powershell, versão, nupkg, deploy]
arquivo: build.ps1
---

# 🛠️ build.ps1

> 🇺🇸 English → [[🛠️ build.ps1 (EN)]] · 🇪🇸 Español → [[🛠️ build.ps1 (ES)]]

Script de **build + versionamento + deploy + empacotamento** do plugin.

**Caminho:** `build.ps1` (raiz do repo)

---

## ⚙️ O que faz

1. **Lê a versão atual** do `.nuspec` e calcula `newVersion` (`major.minor.build+1`).
2. **Detecta mudanças** — compara a data das entradas (`*.cs`, `*.csproj`, `*.nuspec`, `*.resx`, `*.png`, `*.md`) com o último `.nupkg`. Sem `-Force` e sem mudança → mantém a versão e sai.
3. **Fecha o GitExtensions** se estiver em execução (libera a DLL para deploy).
4. **Carimba versão + data nos docs PRIMEIRO** (READMEs e cofre Obsidian), depois dá o bump na fonte da verdade (`.nuspec`/`.csproj`) — assim os docs já refletem a nova versão.
5. **`dotnet build -c Release`**.
6. **Deploy** — copia a DLL para `C:\Program Files\GitExtensions\Plugins\` (requer Admin) e para `tools\net9.0-windows\` (para o nupkg).
7. **`nuget pack`** → `.nupkg` na raiz (filtra o aviso **NU5101** de propósito) e remove `.nupkg` antigos.

---

## 🏷️ Carimbo de versão

### 📄 READMEs (seção 2a)
`README.md`, `README.pt-BR.md`, `README.en-US.md` — atualiza `**Version/Versão:**` e a data.

### 🧠 Cofre Obsidian (seção 2b)
Somente as notas que espelham a versão **atual**:
- `ZimerfeldCommitMsg\💼 Negócio\📦 GitExtensions.ZimerfeldCommitMsg.md`
- `ZimerfeldCommitMsg\📚 Conhecimento\📖 README — Instalação, Uso e Build.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🏷️ Versionamento.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🔭 Visão Geral.md`

Substitui `versao: …` no frontmatter e as ocorrências de `**X.Y.Z**` / `` `X.Y.Z` `` da versão anterior.

> [!warning] Caminhos do carimbo × reestruturação v2 do cofre (2026-07-04)
> O cofre foi reestruturado para o padrão "Cofre de Neurônios v2" e as 4 notas acima **mudaram de pasta/nome** (antes: `🚀 Projetos\GitExtensions.ZimerfeldCommitMsg.md`, `📚 Conhecimento\README — Instalação, Uso e Build.md`, `🏛 Sistema\Versionamento.md`, `🏛 Sistema\Visão Geral.md`). A **seção 2b do `build.ps1` precisa ser atualizada** para os novos caminhos — pendência registrada em [[📌 Backlog]]. Considerar também carimbar os pares `(EN)` dessas notas.

---

## 💻 Uso

```powershell
.\build.ps1          # build incremental (só se houve mudança)
.\build.ps1 -Force   # sempre recompila/empacota
```

Requer **.NET 9 SDK** e, para o deploy em `Program Files`, **Administrador**.

---

## 🔗 Ligações

- [[🏷️ Versionamento]]
- [[🔗 Dependências]]
- [[💻 Ambiente Local (Dev)]]
- [[🚀 Deploy em Produção (Prod)]]

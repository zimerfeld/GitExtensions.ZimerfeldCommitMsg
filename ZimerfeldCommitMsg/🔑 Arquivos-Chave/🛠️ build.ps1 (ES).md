---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquivo, build, powershell, versão, nupkg, deploy]
arquivo: build.ps1
---

# 🛠️ build.ps1

> 🇧🇷 Portugués → [[🛠️ build.ps1]] · 🇺🇸 English → [[🛠️ build.ps1 (EN)]]

Script de **build + versionado + deploy + empaquetado** del plugin.

**Ruta:** `build.ps1` (raíz del repo)

---

## ⚙️ Qué hace

1. **Lee la versión actual** del `.nuspec` y calcula `newVersion` (`major.minor.build+1`).
2. **Detecta cambios** — compara la fecha de las entradas (`*.cs`, `*.csproj`, `*.nuspec`, `*.resx`, `*.png`, `*.md`) con el último `.nupkg`. Sin `-Force` y sin cambio → mantiene la versión y sale.
3. **Cierra GitExtensions** si está en ejecución (libera la DLL para el deploy).
4. **Sella versión + fecha en los docs PRIMERO** (READMEs y cofre Obsidian), después hace el bump en la fuente de verdad (`.nuspec`/`.csproj`) — así los docs ya reflejan la nueva versión.
5. **`dotnet build -c Release`**.
6. **Deploy** — copia la DLL a `C:\Program Files\GitExtensions\Plugins\` (requiere Admin) y a `tools\net9.0-windows\` (para el nupkg).
7. **`nuget pack`** → `.nupkg` en la raíz (filtra a propósito el aviso **NU5101**) y elimina los `.nupkg` antiguos.

---

## 🏷️ Sello de versión

### 📄 READMEs (sección 2a)
`README.md`, `README.pt-BR.md`, `README.en-US.md` — actualiza `**Version/Versão:**` y la fecha.

### 🧠 Cofre Obsidian (sección 2b)
Solo las notas que reflejan la versión **actual**:
- `ZimerfeldCommitMsg\💼 Negócio\📦 GitExtensions.ZimerfeldCommitMsg.md`
- `ZimerfeldCommitMsg\📚 Conhecimento\📖 README — Instalação, Uso e Build.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🏷️ Versionamento.md`
- `ZimerfeldCommitMsg\🧩 Sistemas\🔭 Visão Geral.md`

Sustituye `versao: …` en el frontmatter y las ocurrencias de `**X.Y.Z**` / `` `X.Y.Z` `` de la versión anterior.

> [!warning] Rutas del sello × reestructuración v2 del cofre (2026-07-04)
> El cofre se reestructuró al estándar "Cofre de Neurônios v2" y las 4 notas de arriba **cambiaron de carpeta/nombre** (antes: `🚀 Projetos\GitExtensions.ZimerfeldCommitMsg.md`, `📚 Conhecimento\README — Instalação, Uso e Build.md`, `🏛 Sistema\Versionamento.md`, `🏛 Sistema\Visão Geral.md`). La **sección 2b del `build.ps1` debe actualizarse** a las nuevas rutas — pendiente registrada en [[📌 Backlog (ES)]]. Considerar también sellar los pares `(EN)` de esas notas.

---

## 💻 Uso

```powershell
.\build.ps1          # build incremental (solo si hubo cambios)
.\build.ps1 -Force   # siempre recompila/reempaqueta
```

Requiere **.NET 9 SDK** y, para el deploy en `Program Files`, **Administrador**.

---

## 🔗 Enlaces

- [[🏷️ Versionamento (ES)|Versionado]]
- [[🔗 Dependências (ES)|Dependencias]]
- [[💻 Ambiente Local (Dev) (ES)|Entorno Local (Dev)]]
- [[🚀 Deploy em Produção (Prod) (ES)|Deploy en Producción (Prod)]]

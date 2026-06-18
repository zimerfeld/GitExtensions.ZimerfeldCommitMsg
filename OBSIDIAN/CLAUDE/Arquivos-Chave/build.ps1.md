---
tipo: arquivo
tags: [arquivo, build, powershell, versionamento, nupkg]
arquivo: build.ps1
atualizado: 2026-06-16
---

# build.ps1

Script principal de build, versionamento e empacotamento.

**Caminho:** `build.ps1` (raiz do repositório)

**Requer:** PowerShell 5.1+, `nuget` CLI no PATH, Admin para deploy.

---

## Parâmetros / variáveis principais

```powershell
$nuspec  = ".\src\...\GitExtensions.ZimerfeldCommitMsg.nuspec"
$csproj  = ".\src\...\GitExtensions.ZimerfeldCommitMsg.csproj"
$outDir  = $PSScriptRoot   # raiz do repo
```

---

## Passos detalhados

### 1. Lê versão atual
```powershell
[xml]$spec = Get-Content $nuspec -Encoding UTF8
$current   = $spec.package.metadata.version   # ex: "1.0.12"
```

### 2. Incrementa build
```powershell
$build      = [int]$parts[2] + 1
$newVersion = "$major.$minor.$build"           # ex: "1.0.13"
```

### 3. Escreve nos docs PRIMEIRO (versão + data)
Antes do _bump_ na fonte da verdade, carimba a nova versão/data nos documentos:
- **READMEs:** `README.md`, `README.pt-BR.md`, `README.en-US.md` (`**Version/Versão:**`, `**Updated/Atualizado em:**`).
- **Cofre Obsidian** (4 notas que exibem a versão atual): `01 - Projetos/GitExtensions.ZimerfeldCommitMsg.md`, `02 - Conhecimento/README — Instalação, Uso e Build.md`, `Sistema/Versionamento.md`, `Sistema/Visão Geral.md` — frontmatter `versao:`/`atualizado:` e a linha "Versão atual".

> Notas de **sessão/histórico** não são tocadas de propósito (mencionam versões antigas).

### 4. Bump no .nuspec
```powershell
$spec.package.metadata.version = $newVersion
$spec.Save($nuspec)
```

### 5. Bump no .csproj
Regex replace: `<Version>[^<]+</Version>` → `<Version>1.0.13</Version>`

### 6. Build
```powershell
dotnet build $csproj -c Release --nologo -v quiet
```

### 7. Deploy (Admin)
```powershell
$pluginsDir = "C:\Program Files\GitExtensions\Plugins"
Copy-Item $dll $pluginsDir -Force
```
Pulado com aviso se sem Admin ou pasta não encontrada.

### 8. Atualiza tools\net9.0-windows\
Copia DLL para a pasta usada pelo nupkg.

### 9. Gera nupkg
```powershell
nuget pack $nuspec -OutputDirectory $outDir
```

### 10. Remove versões antigas
```powershell
Get-ChildItem "*.nupkg" | Where-Object { $_.Name -notlike "*$newVersion*" } | Remove-Item -Force
```

---

## Saídas

| Artefato | Localização |
|---|---|
| DLL compilada | `src\...\bin\Release\net9.0-windows\*.dll` |
| DLL instalada | `C:\Program Files\GitExtensions\Plugins\` |
| DLL no nupkg | `tools\net9.0-windows\*.dll` |
| Pacote NuGet | `.\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg` |

---

## Como executar

```powershell
# Recomendado (Admin para deploy completo):
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1

# Sem Admin (Bash tool ou Git Bash):
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "build.ps1"
```

---

## Relacionado

- [[../Sistema/Versionamento]]
- [[../Fluxos/Instalação e Deploy]]

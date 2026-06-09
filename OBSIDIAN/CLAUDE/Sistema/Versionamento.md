---
tipo: sistema
tags: [build, versão, nupkg, deploy]
atualizado: 2026-06-09
---

# Versionamento e Build

## Esquema de versão

`major.minor.build` — somente o `build` é incrementado automaticamente pelo `build.ps1`. Major e minor são alterados manualmente.

**Versão atual:** `1.0.40` *(fonte da verdade: `.nuspec` / `.csproj`)*

> [!note] Recursos `.resx` embutidos (sem satellite assemblies)
> As strings de UI vivem em `Resources/Strings.resx` (EN neutro) e `Resources/StringsPtBr.resx` (PT),
> embutidas no **assembly principal** (PT sem sufixo de cultura → não vira satellite), preservando o
> deploy de **DLL única**. Ver [[../Decisoes/Suporte Multilíngue PT-EN]].

## Ciclo build.ps1

```
build.ps1
  │
  ├─ 1. Lê versão atual do .nuspec
  ├─ 2. Incrementa build (+1) → newVersion
  ├─ 3. Atualiza .nuspec  ← <version>
  ├─ 4. Atualiza .csproj  ← <Version>
  ├─ 5. Atualiza FUNCIONALIDADES.md ← **Versão:** e **Atualizado em:**
  ├─ 6. dotnet build -c Release
  ├─ 7. Copia DLL → C:\Program Files\GitExtensions\Plugins\  (requer Admin)
  ├─ 8. Copia DLL → tools\net9.0-windows\  (para o nupkg)
  ├─ 9. nuget pack .nuspec → .nupkg na raiz
  └─ 10. Remove .nupkg de versões anteriores
```

> Requer `nuget` CLI e permissão de **Administrador** para o deploy. Sem Admin, o passo 7 é pulado com aviso.

## Arquivos versionados

| Arquivo | Campo atualizado |
|---|---|
| `GitExtensions.ZimerfeldCommitMsg.nuspec` | `<version>` |
| `GitExtensions.ZimerfeldCommitMsg.csproj` | `<Version>` |
| `FUNCIONALIDADES.md` | `**Versão:**` e `**Atualizado em:**` |

## Deploy rápido (sem incrementar versão)

```powershell
.\tools\update-dll.ps1
```

Só copia o DLL compilado para a pasta de plugins, sem alterar versão ou gerar nupkg.

## Instalação manual

```powershell
.\tools\install.ps1        # requer Admin
```

Localiza automaticamente a pasta `C:\Program Files\GitExtensions\Plugins\` (ou x86).

## Desinstalação

```powershell
.\tools\uninstall.ps1      # requer Admin
```

Remove `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` da pasta de plugins. Não afeta nada mais.

## Relacionado

- [[../Arquivos-Chave/build.ps1]]
- [[Dependências]]

---
tipo: sistema
tags: [build, versão, nupkg, deploy]
atualizado: 2026-06-22
---

# Versionamento e Build

## Esquema de versão

`major.minor.build` — somente o `build` é incrementado automaticamente pelo `build.ps1`. Major e minor são alterados manualmente.

**Versão atual:** `1.0.84` *(fonte da verdade: `.nuspec` / `.csproj`)*

> [!note] Recursos `.resx` embutidos (sem satellite assemblies)
> As strings de UI vivem em `Resources/Strings.resx` (EN neutro) e `Resources/StringsPtBr.resx` (PT),
> embutidas no **assembly principal** (PT sem sufixo de cultura → não vira satellite), preservando o
> deploy de **DLL única**. Ver [[../Decisoes/Suporte Multilíngue PT-EN]].

## Ciclo build.ps1

```
build.ps1
  │
  ├─ 1. Lê versão atual do .nuspec
  ├─ 2. Calcula newVersion (build +1)
  ├─ 3. Escreve nos DOCS primeiro (versão + data):
  │       ├─ READMEs (README.md / README.pt-BR.md / README.en-US.md)
  │       └─ cofre Obsidian (Projeto, README espelho, Versionamento, Visão Geral)
  ├─ 4. Bump no .nuspec  ← <version>
  ├─ 5. Bump no .csproj  ← <Version>
  ├─ 6. dotnet build -c Release
  ├─ 7. Copia DLL → C:\Program Files\GitExtensions\Plugins\  (requer Admin)
  ├─ 8. Copia DLL → tools\net9.0-windows\  (para o nupkg)
  ├─ 9. nuget pack .nuspec → .nupkg na raiz
  └─ 10. Remove .nupkg de versões anteriores
```

> **Ordem proposital:** os docs (READMEs + cofre) são carimbados **antes** do _bump_ no
> `.nuspec`/`.csproj`. O _pack_ (passo 9) roda **depois** de todos os carimbos, então o `.nupkg`
> continua sendo o artefato mais recente — o que mantém a detecção de mudanças por timestamp correta.

> Requer `nuget` CLI e permissão de **Administrador** para o deploy. Sem Admin, o passo 7 é pulado com aviso.

## Arquivos versionados

| Arquivo | Campo atualizado |
|---|---|
| `GitExtensions.ZimerfeldCommitMsg.nuspec` | `<version>` |
| `GitExtensions.ZimerfeldCommitMsg.csproj` | `<Version>` |
| `README.md` / `README.pt-BR.md` / `README.en-US.md` | `**Version/Versão:**` e `**Updated/Atualizado em:**` |
| Cofre Obsidian (4 notas: Projeto, README espelho, Versionamento, Visão Geral) | frontmatter `versao:`/`atualizado:` e a linha "Versão atual" |

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

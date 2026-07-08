---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [build, versão, nupkg, deploy]
---

# 🏷️ Versionamento e Build

## 🔢 Esquema de versão

`major.minor.build` — somente o `build` é incrementado automaticamente pelo `build.ps1`. Major e minor são alterados manualmente.

**Versão atual:** `1.0.97` *(fonte da verdade: `.nuspec` / `.csproj`)*

> [!note] Strings de UI embutidas (sem satellite assemblies)
> As strings de UI vivem em `Resources/Strings.resx` e `Resources/StringsPtBr.resx`, embutidas no
> **assembly principal** com `LogicalName` explícito — assim o MSBuild **não** as desvia para
> satellite assemblies e o deploy continua sendo de **DLL única**. Lidas em runtime por `Strings`.
> Ver [[📦 Strings embutidas sem satellite assemblies]].

## 🔄 Ciclo build.ps1

```
build.ps1  [-Force]
  │
  ├─ 1. Lê versão atual do .nuspec
  ├─ 2. Calcula newVersion (build +1)
  ├─ 2b. Detecta mudanças (fontes/docs mais novos que o último .nupkg); sem -Force e sem
  │      mudança → mantém versão e sai (build/pack ignorados)
  ├─ 2c. Fecha o GitExtensions se estiver em execução
  ├─ 3. Bump no .nuspec  ← <version>
  ├─ 4. Bump no .csproj  ← <Version>
  ├─ 4b. Carimba versão + data no topo dos READMEs (README.md / .pt-BR / .en-US)
  ├─ 5. dotnet build -c Release
  ├─ 6. Copia DLL → C:\Program Files\GitExtensions\Plugins\  (requer Admin)
  ├─ 6b. Copia DLL → tools\net9.0-windows\  (para o nupkg)
  ├─ 7. nuget pack .nuspec → .nupkg na raiz (filtra o aviso NU5101)
  └─ 7b. Remove .nupkg de versões anteriores
```

> [!warning] Aviso **NU5101** é intencional
> A DLL é empacotada em `lib\` **raiz** (grupo "any" que o Plugin Manager extrai), não em
> `lib\net9.0-windows\`. Isso gera o aviso NU5101, que o `build.ps1` **filtra de propósito** no
> `nuget pack`. Detalhe em [[🔗 Dependências]] e no `.nuspec`.

<!-- -->

> Requer o **.NET 9 SDK** (`dotnet`) e, para o deploy, permissão de **Administrador**. Sem Admin, o passo de deploy é pulado com aviso; `nuget` fica em `tools\nuget.exe`.

## 📄 Arquivos versionados

| Arquivo | Campo atualizado |
|---|---|
| `GitExtensions.ZimerfeldCommitMsg.nuspec` | `<version>` |
| `GitExtensions.ZimerfeldCommitMsg.csproj` | `<Version>` |
| `README.md` / `README.pt-BR.md` / `README.en-US.md` | `**Version/Versão:**` e `**Updated/Atualizado em:**` |

## 🧰 Instalação / desinstalação manual

```powershell
tools\install.ps1        # requer Admin — copia a DLL para a pasta Plugins (também via PMC)
tools\uninstall.ps1      # requer Admin — remove a DLL (não afeta o resto do GitExtensions)
tools\update-dll.ps1     # atualiza apenas a DLL na pasta Plugins
```

## 🧪 Testes

Suíte **xUnit** em `tests\GitExtensions.ZimerfeldCommitMsg.Tests\` (as funções puras do gerador são expostas por `InternalsVisibleTo`):
- `CommentExtractionTests` — reconhecimento e limpeza de comentários (várias sintaxes)
- `ConceptExtractionTests` — derivação de conceitos a partir de nomes de arquivo
- `RepoVocabularyConfigTests` — carga do `.zimerfeldcommitmsg.json`
- `TranslationTests` — tradução inglês → português

## 🔗 Ligações

- [[🛠️ build.ps1]]
- [[🔗 Dependências]]
- [[💻 Ambiente Local (Dev)]]
- [[🚀 Deploy em Produção (Prod)]]

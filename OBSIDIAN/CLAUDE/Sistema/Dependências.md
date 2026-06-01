---
tipo: sistema
tags: [dependências, assemblies, gitextensions]
atualizado: 2026-05-22
---

# Dependências

## Assemblies do GitExtensions (referências externas)

Todas referenciadas com `<Private>false</Private>` — **não** copiadas para o output (já estão no host).

| Assembly | Caminho | Uso |
|---|---|---|
| `GitExtensions.Extensibility.dll` | `C:\Program Files\GitExtensions\` | `IGitPlugin`, `GitPluginBase`, `IGitUICommands`, `GitUIEventArgs`, `IGitModule` |
| `GitUIPluginInterfaces.dll` | `C:\Program Files\GitExtensions\` | interfaces de UI legadas |
| `System.ComponentModel.Composition.dll` | `C:\Program Files\GitExtensions\` | MEF — `[Export(typeof(IGitPlugin))]` |

## Interfaces-chave implementadas

### `IGitPlugin` (via `GitPluginBase`)
- `Register(IGitUICommands)` — chamado ao carregar o plugin
- `Unregister(IGitUICommands)` — chamado ao descarregar
- `Execute(GitUIEventArgs)` — chamado via menu Plugins

### `IGitUICommands`
- `AddCommitTemplate(key, factory, icon)` — registra template no dropdown
- `RemoveCommitTemplate(key)` — remove o template
- `PostRepositoryChanged` — evento disparado a cada mudança no repositório
- `StartCommitDialog(owner, commitMessage)` — abre `FormCommit`
- `Module.WorkingDir` — diretório de trabalho do repositório atual

## Evento de integração

```
GitExtensions  →  PostRepositoryChanged  →  OnPostRepositoryChanged()
                                                  │
                                                  ▼
                                         RefreshOpenCommitDialog()
                                                  │
                                                  ▼
                                         CommitMessageGenerator.Generate()
```

## Runtime

| Requisito | Valor |
|---|---|
| .NET | 9.0 (Windows) |
| Windows Forms | Sim (herdado do host) |
| PowerShell | 5.1+ (scripts de build/deploy) |
| nuget CLI | Para gerar `.nupkg` |

## Inspector (ferramenta de desenvolvimento)

`inspector/Program.cs` usa `MetadataLoadContext` para inspecionar os assemblies do GitExtensions sem carregá-los em runtime. Útil para descobrir interfaces e membros públicos disponíveis. Ver [[../Arquivos-Chave/inspector]].

## Relacionado

- [[Arquitetura]]
- [[Versionamento]]

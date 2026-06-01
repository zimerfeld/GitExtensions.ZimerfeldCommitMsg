---
tipo: conhecimento
criado: 2026-06-01
tags: [conhecimento, csharp, gitextensions, mef, plugin]
---

# Plugin MEF para GitExtensions

## Resumo
GitExtensions carrega plugins via **MEF** (Managed Extensibility Framework). O entry point é uma classe exportada que implementa `IGitPlugin` (normalmente herdando de `GitPluginBase`).

## Pontos-chave
- Exportar com `[Export(typeof(IGitPlugin))]` usando `System.ComponentModel.Composition`.
- Projeto compila como **`Library`** (DLL), `net9.0-windows`, WinForms habilitado.
- Referenciar os assemblies do GitExtensions de `C:\Program Files\GitExtensions\` com **`<Private>false</Private>`** (não copiar para a saída — o host já os tem):
  - `GitExtensions.Extensibility.dll`
  - `GitUIPluginInterfaces.dll`
  - `System.ComponentModel.Composition.dll`
- O `AssemblyName` precisa bater com o que `install.ps1` / nuspec esperam (`GitExtensions.Plugins.<Nome>`).

## Ciclo de vida do plugin
- `Register(IGitUICommands)` — chamado na **UI thread**. Bom lugar para capturar `SynchronizationContext.Current`, registrar templates de commit (`AddCommitTemplate`) e assinar eventos (`PostRepositoryChanged`).
- `Unregister(IGitUICommands)` — remover templates (`RemoveCommitTemplate`) e desassinar eventos.
- `Execute(GitUIEventArgs)` — disparado pelo menu **Plugins → \<nome\>**. Acesso a `GitModule.WorkingDir`, `StartCommitDialog`, etc.

## Descobrindo a API (truque do `inspector`)
Um console separado com `MetadataLoadContext` + reflection lista os tipos/membros públicos das DLLs do host sem carregá-las no runtime — forma rápida de achar a assinatura certa (`ICommitMessageManager`, `GitUIEventArgs`, …) ao evoluir o plugin. Ver `inspector\Program.cs` em [[GitExtensions.ZimerfeldCommitMsg]].

## 🔗 Relacionado
- [[GitExtensions.ZimerfeldCommitMsg]]
- [[GitExtensions.ZimerfeldTree]]

---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [conhecimento, csharp, gitextensions, mef, plugin]
---

# 🧩 Plugin MEF para GitExtensions

## 📝 Resumo
GitExtensions carrega plugins via **MEF** (Managed Extensibility Framework). O entry point é uma classe exportada que implementa `IGitPlugin` (normalmente herdando de `GitPluginBase`).

## 🔑 Pontos-chave
- Exportar com `[Export(typeof(IGitPlugin))]` usando `System.ComponentModel.Composition`.
- Projeto compila como **`Library`** (DLL), `net9.0-windows`, WinForms habilitado.
- Referenciar os assemblies do host com **`<Private>false</Private>`** (não copiar para a saída — o host já os tem). Aqui eles ficam **versionados em `refs\`** (build determinístico e offline):
  - `GitExtensions.Extensibility.dll`
  - `System.ComponentModel.Composition.dll`
- O `AssemblyName` precisa bater com o que `install.ps1` / nuspec esperam (`GitExtensions.Plugins.<Nome>`).
- Para aparecer no **Plugin Manager** interno, o pacote NuGet precisa **depender** de `GitExtensions.Extensibility` (dependência marcadora). Ver [[🔗 Dependências]].

## ♻️ Ciclo de vida do plugin
- `Register(IGitUICommands)` — chamado ao carregar. Bom lugar para **capturar o `IGitUICommands`**, registrar templates de commit e assinar eventos da UI (aqui: `Application.Idle`).
- `Unregister(IGitUICommands)` — desassinar / limpar o capturado.
- `Execute(GitUIEventArgs)` — disparado pelo menu **Plugins → \<nome\>**.
- `GetSettings()` — expõe settings configuráveis (aqui: o seletor de idioma).

## ⚙️ Como o ZimerfeldCommitMsg usa este modelo
- Registra **três itens de template** de commit (um por idioma) via `CommitTemplateManager`.
- Assina **`Application.Idle`** para detectar o `FormCommit` recém-aberto e preencher a mensagem — o host **não** expõe evento de "diálogo de commit aberto". Ver [[🔌 ZimerfeldCommitMsgPlugin]] e [[🔌 Integração via template dropdown e Application.Idle]].
- `Execute` gera a mensagem para o repo atual (menu Plugins).
- `GetSettings` expõe o `ChoiceSetting` de idioma (Automático/PT/EN).

## 🔀 Contraste com os irmãos
- `GitExtensions.ZimerfeldLFS` e `GitExtensions.ZimerfeldTree` abrem **janelas próprias** (não-modais). Já o CommitMsg **não tem janela**: integra-se ao diálogo de commit existente do host.

## 🔗 Relacionado
- [[📦 GitExtensions.ZimerfeldCommitMsg]]
- [[🔌 ZimerfeldCommitMsgPlugin]]
- [[🔗 Dependências]]

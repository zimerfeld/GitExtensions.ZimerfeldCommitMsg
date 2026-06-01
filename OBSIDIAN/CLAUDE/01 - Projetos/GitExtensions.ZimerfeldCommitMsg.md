---
tipo: projeto
criado: 2026-06-01
atualizado: 2026-06-01
tags: [projeto, csharp, gitextensions, plugin, winforms, conventional-commits]
status: ativo
linguagem: C#
versao: 1.0.12
repo: C:\NUGET\ZimerfeldCommitMsg
---

# ✍️ GitExtensions.ZimerfeldCommitMsg

## 🎯 Objetivo
Plugin para **[GitExtensions](https://gitextensions.github.io/)** que **gera automaticamente** uma mensagem de commit resumindo as mudanças nos arquivos **staged**, no formato **Conventional Commits v1.0.0** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`), com a **descrição em português-BR**.

## 📂 Estrutura do repositório
```
C:\NUGET\ZimerfeldCommitMsg\
├─ src\GitExtensions.ZimerfeldCommitMsg\   # código do plugin (.csproj)
│   ├─ ZimerfeldCommitMsgPlugin.cs        # entry point MEF (~161 linhas)
│   ├─ CommitMessageGenerator.cs          # toda a lógica de geração (~985 linhas)
│   ├─ *.csproj / *.nuspec                # build + manifesto NuGet
├─ inspector\                             # utilitário p/ inspecionar a API do GitExtensions via reflection
│   └─ Program.cs                         # lista tipos/membros de Extensibility e PluginInterfaces
├─ tools\                                 # install/uninstall/update .ps1
│   └─ net9.0-windows\                    # saída do build (DLL)
├─ OBSIDIAN\CLAUDE\                        # 🧠 este cofre de memória
├─ build.ps1                              # incrementa versão + build + deploy + nupkg
├─ README.md                              # instalação e uso
├─ FUNCIONALIDADES.md                     # spec detalhada da geração (atualizada pelo build.ps1)
└─ GitExtensions.ZimerfeldCommitMsg.1.0.12.nupkg
```

## ⚙️ Stack técnica
- **Linguagem:** C# (`net9.0-windows`), `Nullable` + `ImplicitUsings` habilitados
- **UI:** WinForms (`UseWindowsForms`) — usa o `FormCommit` do próprio GitExtensions
- **Tipo de saída:** `Library` (DLL carregada pelo GitExtensions)
- **AssemblyName:** `GitExtensions.Plugins.ZimerfeldCommitMsg`
- **Namespace raiz:** `GitExtensions.ZimerfeldCommitMsg`
- **NeutralLanguage:** `pt-BR`
- **Plugin model:** MEF (`[Export(typeof(IGitPlugin))]`) — ver [[Plugin MEF para GitExtensions]]
- **Referências externas** (de `C:\Program Files\GitExtensions\`, `Private=false`):
  - `GitExtensions.Extensibility.dll`
  - `GitUIPluginInterfaces.dll`
  - `System.ComponentModel.Composition.dll`

## ✨ Funcionalidades principais
- **Template no diálogo de commit:** opção **"Zimerfeld: Auto-resumo"** no dropdown de templates preenche a mensagem (`AddCommitTemplate`)
- **Menu Plugins → ZimerfeldCommitMsg:** abre o `StartCommitDialog` com a mensagem já preenchida (`Execute`)
- **Auto-refresh ao (un)stage:** assina `PostRepositoryChanged`; se o `FormCommit` estiver aberto e a caixa estiver vazia (ou contiver a última mensagem que nós geramos), atualiza a mensagem. **Nunca sobrescreve texto digitado pelo usuário**
- Marshalling seguro pra UI thread via `SynchronizationContext` capturado no `Register()`
- Tudo dentro de `try/catch` — o plugin **nunca derruba** o GitExtensions

## 🧠 Lógica de geração (`CommitMessageGenerator`)
Formato: `<tipo>: <descrição pt-BR>` + corpo opcional. Sem scope. Ver detalhes em [[Geração de mensagem - Conventional Commits]].
- **Estratégia 1 (principal):** extrai comentários adicionados (`+`) do `git diff --cached --no-color`, filtra ruído (separadores, tags XML, código comentado, < 10 chars), combina até 5 com `; `
- **Estratégia 2 (fallback):** deriva conceitos dos nomes de arquivo (remove prefixo `I`, sufixos arquiteturais como `Service`/`Controller`/`Generator`, mapeia para frase pt-BR)
- **Tipo** detectado por: arquivos novos → `feat`; só modificações → `fix`; só docs → `docs`; testes → `test`; config → `chore`; build → `build`; mix → `refactor`

## 🛠️ Build / instalação
```powershell
# Build: incrementa build, compila, faz deploy (Admin), gera nupkg
.\build.ps1
# ou sem elevação (git bash / Bash tool):
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "C:/NUGET/ZimerfeldCommitMsg/build.ps1"

# Scripts auxiliares em tools\ (requerem Admin p/ Program Files)
tools\install.ps1      # instala o plugin
tools\uninstall.ps1    # remove (não afeta nada mais do GitExtensions)
tools\update-dll.ps1   # atualiza só a DLL (dev, sem incrementar versão)
```
> O `build.ps1` incrementa `major.minor.BUILD`, sincroniza versão em nuspec + csproj + `FUNCIONALIDADES.md`, builda em Release, copia a DLL p/ `Plugins\` (se Admin) e p/ `tools\net9.0-windows\`, e roda `nuget pack` removendo nupkgs antigos.

## 🔍 inspector (utilitário de dev)
Projeto console separado (`inspector\Program.cs`) que usa `MetadataLoadContext` + reflection para listar tipos/membros públicos de `GitExtensions.Extensibility.dll` e `GitUIPluginInterfaces.dll` — útil pra descobrir a API correta (`IGitPlugin`, `IGitUICommands`, `ICommitMessageManager`, `GitUIEventArgs` etc.) ao evoluir o plugin.

## 🐛 Armadilhas conhecidas
> [!warning] Não sobrescrever a mensagem do usuário
> O auto-refresh em `PostRepositoryChanged` só atualiza a caixa se ela estiver vazia ou contiver `_lastGeneratedMessage`. Mexer nisso pode apagar texto digitado à mão.

> [!warning] Localizar a caixa de texto do commit
> `FindCommitTextBox` tenta nomes conhecidos (`Message`, `commitMessageEditor`, `_commitMessage`, `commitMessage`) e cai num fallback heurístico (maior `TextBoxBase` multiline editável). Versões diferentes do GitExtensions mudam esses nomes.

## 🔢 Versionamento
- Versão atual: **1.0.12** (csproj + nuspec sincronizados pelo `build.ps1`)
- Esquema: `major.minor.BUILD`, BUILD auto-incrementado a cada build
- `FUNCIONALIDADES.md` carimba versão + data a cada build

## 📜 Histórico de sessões
- [[2026-06-01 - Criação do cofre de neurônios CommitMsg]] — mapeamento inicial do projeto

## 🔗 Relacionado
- [[Plugin MEF para GitExtensions]]
- [[Geração de mensagem - Conventional Commits]]
- [[GitExtensions.ZimerfeldTree]] — projeto irmão (plugin de árvore de branches)
- [[🔑 Fatos-Chave]]

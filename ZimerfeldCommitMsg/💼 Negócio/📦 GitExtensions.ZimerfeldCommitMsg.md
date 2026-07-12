---
tipo: negocio
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [projeto, negocio, csharp, gitextensions, plugin, winforms, commit-message, conventional-commits, i18n]
status: ativo
linguagem: C#
versao: 1.0.97
repo: C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg
---

# 📦 GitExtensions.ZimerfeldCommitMsg

> 🇺🇸 English → [[📦 GitExtensions.ZimerfeldCommitMsg (EN)]] · 🇪🇸 Español → [[📦 GitExtensions.ZimerfeldCommitMsg (ES)]]

## 🎯 Objetivo
Plugin para **[GitExtensions](https://gitextensions.github.io/)** que **gera mensagens de commit automaticamente** analisando o **conteúdo real** das alterações em stage (não apenas os nomes de arquivo). As mudanças são classificadas pelos tipos do **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) para escolher o **verbo** adequado; a mensagem é um **subject iniciado por verbo** (sem o prefixo literal `tipo:`) mais um **corpo em bullets**. Ver [[📜 Conventional Commits - Conceitos]].

## 💜 Financiamento / Patrocínio
Canais de doação configurados (badges no topo do README):
- **GitHub Sponsors:** `@zimerfeld` → https://github.com/sponsors/zimerfeld
- **Ko-fi:** `C0D621FCGD` → https://ko-fi.com/C0D621FCGD
- **Prova social no README:** badges de versão e **downloads do NuGet** (`shields.io/nuget/v` e `/dt`).

## 📂 Estrutura do repositório
```
C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\
├─ src\GitExtensions.ZimerfeldCommitMsg\        # código do plugin (.csproj)
│   ├─ ZimerfeldCommitMsgPlugin.cs             # entry point MEF + integração com o diálogo de commit
│   ├─ CommitMessageGenerator.cs               # motor: diff → tipos CC → verbo → subject + corpo (~1200 linhas)
│   ├─ RepoVocabularyConfig.cs                 # vocabulário extra por repo (.zimerfeldcommitmsg.json)
│   ├─ Localization\                           # MessageLanguage, LanguagePack, Strings
│   ├─ Resources\                              # icon.png, icon-128.png, Strings.resx, StringsPtBr.resx
│   ├─ *.csproj / *.nuspec                     # build + manifesto NuGet
├─ tests\GitExtensions.ZimerfeldCommitMsg.Tests\  # xUnit: comentários, conceitos, vocab, tradução
├─ refs\                                        # DLLs do host versionadas (build determinístico)
├─ tools\                                       # install/uninstall/update-dll .ps1, nuget.exe, generate-icon.ps1
│   └─ net9.0-windows\                          # saída do build (DLL) usada pelo nupkg
├─ ZimerfeldCommitMsg\                          # 🧠 este cofre de memória
├─ build.ps1                                    # incrementa versão + build + deploy + nupkg
├─ README.md / README.pt-BR.md / README.en-US.md  # espelhado em [[📖 README — Instalação, Uso e Build]]
└─ GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg
```

## ⚙️ Stack técnica
- **Linguagem:** C# (`net9.0-windows`), `Nullable` + `ImplicitUsings` habilitados
- **UI:** WinForms (`UseWindowsForms`) — o plugin **não tem janela própria**; integra-se ao **diálogo de commit** do host
- **Tipo de saída:** `Library` (DLL carregada pelo GitExtensions)
- **AssemblyName:** `GitExtensions.Plugins.ZimerfeldCommitMsg`
- **Namespace raiz:** `GitExtensions.ZimerfeldCommitMsg`
- **NeutralLanguage:** `pt-BR`
- **Plugin model:** MEF (`[Export(typeof(IGitPlugin))]`) — ver [[🧩 Plugin MEF para GitExtensions]]
- **Referências externas** (de `refs\`, `Private=false`): `GitExtensions.Extensibility.dll`, `System.ComponentModel.Composition.dll`
- **Testes:** xUnit, com `InternalsVisibleTo` expondo as funções puras do gerador

## ✨ Funcionalidades principais
- **Geração automática** a partir do conteúdo real do diff staged (`git diff --cached`), não só dos nomes de arquivo. Ver [[⚙️ Geração de mensagem a partir do diff]].
- **Verbo guiado por Conventional Commits** — classifica as mudanças por tipo (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) e prefixa o **verbo** correspondente (3ª pessoa em pt-BR / imperativo em inglês). O tipo em si **não** aparece. Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo]].
- **Duas estratégias de conteúdo:** baseada em **comentários** do diff (principal) e baseada em **nomes de arquivo** (fallback). Ver [[🔀 Duas estratégias - comentários e nomes de arquivo]].
- **Corpo em bullets** — até 5 frases de uma linha, cada uma resumindo a mudança mais significativa de um arquivo; **sempre ao menos um bullet**.
- **Tradução inglês → português** dos comentários quando a saída é pt-BR; em inglês, passam intactos.
- **Saneamento** — descarta comentários com delimitadores desbalanceados ou terminados em palavra de ligação solta; escolhe o de **melhor qualidade** (não o mais longo).
- **Vocabulário por repositório** — `.zimerfeldcommitmsg.json` estende vocabulário conhecido/rejeitado e frases de conceito sem recompilar. Ver [[📓 RepoVocabularyConfig]].
- **Multilíngue (PT-BR / EN):** automático pelo SO + override (dropdown de 3 itens no diálogo de commit **e** seletor em Configurações → Plugins). Ver [[🌐 Localization]].
- **Três modos de integração:** template no diálogo de commit, menu Plugins e **auto-preenchimento** (ao abrir o diálogo e ao stage/unstage). Ver [[🔍 1 - Detecção do diálogo de commit]].
- **Não destrutivo** — nunca sobrescreve texto digitado manualmente.

## 🏗️ Arquitetura (Plugin → Generator → Localization)
Ver [[🏗️ Arquitetura]]:
```
GitExtensions (host)
    │ MEF
    ▼
ZimerfeldCommitMsgPlugin  ← [Export(IGitPlugin)]; registra template de commit + Application.Idle
    │ ao abrir/mudar o FormCommit, gera e injeta a mensagem (sem sobrescrever o que o usuário digitou)
    ▼
CommitMessageGenerator    ── lê ──►  git diff --cached  (comentários + nomes de arquivo)
    │ classifica tipos CC → verbo → subject consolidado + corpo em bullets
    ▼
Localization (LanguagePack / Strings / MessageLanguage)  → idioma da saída + strings de UI
```

## 🛠️ Build / instalação
```powershell
# Build: incrementa build, compila Release, faz deploy (Admin), gera nupkg
.\build.ps1
.\build.ps1 -Force   # sempre recompila/empacota

# Scripts auxiliares em tools\ (Admin p/ Program Files)
tools\install.ps1      # instala o plugin (também via Package Manager Console)
tools\uninstall.ps1    # remove (não afeta o resto do GitExtensions)
tools\update-dll.ps1   # atualiza apenas a DLL na pasta Plugins
```
> Via **Plugin Manager do GitExtensions**: buscar por *ZimerfeldCommitMsg* e instalar. Passo a passo em [[📖 README — Instalação, Uso e Build]] e [[🏷️ Versionamento]]. Runbooks: [[💻 Ambiente Local (Dev)]] · [[🚀 Deploy em Produção (Prod)]].

## 💰 Ângulo de investimento
- **Fricção diária:** todo dev commita várias vezes por dia; boas mensagens são caras de escrever à mão. Este plugin remove essa fricção sem tirar o controle (não destrutivo).
- **Diferencial técnico:** lê o **conteúdo do diff** (comentários) e classifica por Conventional Commits — mais rico que geradores baseados só em nome de arquivo.
- **Custo marginal baixo:** compartilha a infraestrutura (build, i18n, refs versionados, empacotamento) dos irmãos `GitExtensions.ZimerfeldLFS` e `GitExtensions.ZimerfeldTree` — portfólio coeso reforça a marca **Zimerfeld** no ecossistema GitExtensions/NuGet.
- **Distribuição pronta:** publicado no NuGet e visível no Plugin Manager interno (dependência marcadora `GitExtensions.Extensibility`).

## 🐛 Armadilhas conhecidas
> [!warning] O host não notifica "diálogo de commit aberto"
> Não há evento de API para "FormCommit abriu". O plugin detecta o `FormCommit` recém-aberto no **`Application.Idle`** da UI e faz gates por instância + working dir para não reprocessar a cada tick. Ver [[🔍 1 - Detecção do diálogo de commit]] e [[🔌 Integração via template dropdown e Application.Idle]].

<!-- -->

> [!warning] Strings de UI embutidas (sem satellite assemblies)
> As strings ficam em `Resources\Strings.resx` / `StringsPtBr.resx` com `LogicalName` fixo, embutidas no **assembly principal** — assim o deploy é de **DLL única**. Ver [[📦 Strings embutidas sem satellite assemblies]] e [[🏷️ Versionamento]].

<!-- -->

> [!warning] DLL em `lib\` RAIZ no nuspec (aviso NU5101 intencional)
> Como nos irmãos, a DLL vai em `lib\` **raiz** (grupo "any") para o Plugin Manager extrair; o aviso **NU5101** do `nuget pack` é filtrado de propósito no `build.ps1`. Ver [[🏷️ Versionamento]] e [[🔗 Dependências]].

## 🔢 Versionamento
- Versão atual: **1.0.97** (csproj + nuspec sincronizados pelo `build.ps1`)
- Esquema: `major.minor.BUILD`, BUILD auto-incrementado a cada build
- A cada build, o `build.ps1` carimba versão + data nos **READMEs** (e mantém este cofre em sincronia)

## 🔗 Relacionado
- [[📖 README — Instalação, Uso e Build]] — espelho do `README.md`
- [[🧩 Plugin MEF para GitExtensions]]
- [[📜 Conventional Commits - Conceitos]] · [[⚙️ Geração de mensagem a partir do diff]]
- [[🏗️ Arquitetura]] · [[🔭 Visão Geral]] · [[🏷️ Versionamento]] · [[🔗 Dependências]]
- `GitExtensions.ZimerfeldLFS` — irmão (Git LFS)
- `GitExtensions.ZimerfeldTree` — irmão (árvore de branches)
- [[🔑 Fatos-Chave]]

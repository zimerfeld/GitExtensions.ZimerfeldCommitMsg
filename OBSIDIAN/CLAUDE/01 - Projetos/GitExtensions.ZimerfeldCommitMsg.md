---
tipo: projeto
criado: 2026-06-01
atualizado: 2026-06-08
tags: [projeto, csharp, gitextensions, plugin, winforms, conventional-commits, i18n]
status: ativo
linguagem: C#
versao: 1.0.40
repo: C:\GitExtensions\ZimerfeldCommitMsg
---

# ✍️ GitExtensions.ZimerfeldCommitMsg

## 🎯 Objetivo
Plugin para **[GitExtensions](https://gitextensions.github.io/)** que **gera automaticamente** uma mensagem de commit resumindo as mudanças nos arquivos **staged**, no formato **Conventional Commits v1.0.0** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`). **Multilíngue**: gera em **português-BR ou inglês**, detectado pelo idioma do SO, com **override manual** nas configurações. Ver [[Suporte Multilíngue PT-EN]].

## 📂 Estrutura do repositório
```
C:\GitExtensions\ZimerfeldCommitMsg\
├─ src\GitExtensions.ZimerfeldCommitMsg\   # código do plugin (.csproj)
│   ├─ ZimerfeldCommitMsgPlugin.cs        # entry point MEF + setting de idioma
│   ├─ CommitMessageGenerator.cs          # lógica de geração (parametrizada por idioma)
│   ├─ Localization\                      # i18n
│   │   ├─ MessageLanguage.cs            # enum + resolvedor (SO/override)
│   │   ├─ LanguagePack.cs               # mapas PT/EN (conceitos, verbos, conjugação)
│   │   └─ Strings.cs                    # acessor das strings de UI (.resx)
│   ├─ Resources\Strings.resx / StringsPtBr.resx  # strings de UI (EN neutro / PT)
│   ├─ *.csproj / *.nuspec                # build + manifesto NuGet
├─ inspector\                             # utilitário p/ inspecionar a API do GitExtensions via reflection
│   └─ Program.cs                         # lista tipos/membros de Extensibility e PluginInterfaces
├─ tools\                                 # install/uninstall/update .ps1
│   └─ net9.0-windows\                    # saída do build (DLL)
├─ OBSIDIAN\CLAUDE\                        # 🧠 este cofre de memória
├─ build.ps1                              # incrementa versão + build + deploy + nupkg
├─ README.md                              # instalação, uso e spec da geração (espelhado em [[README — Instalação, Uso e Build]])
└─ GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg
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
- **Template no diálogo de commit:** opção no dropdown de templates preenche a mensagem (`AddCommitTemplate`)
- **Menu Plugins → ZimerfeldCommitMsg:** abre o `StartCommitDialog` com a mensagem já preenchida (`Execute`)
- **Auto-refresh ao (un)stage:** assina `PostRepositoryChanged`; se o `FormCommit` estiver aberto e a caixa estiver vazia (ou contiver a última mensagem que nós geramos), atualiza a mensagem. **Nunca sobrescreve texto digitado pelo usuário**
- **Multilíngue (PT/EN):** `ChoiceSetting` "Idioma da mensagem / Message language" com rótulos bilíngues (`Automático/Automatic`, `Português/Portuguese`, `Inglês/English`); Auto detecta pelo `CultureInfo.CurrentUICulture`. Toda a saída + diálogos de UI são localizados. Ver [[Suporte Multilíngue PT-EN]]
- Marshalling seguro pra UI thread via `SynchronizationContext` capturado no `Register()`
- Tudo dentro de `try/catch` — o plugin **nunca derruba** o GitExtensions

## 🧠 Lógica de geração (`CommitMessageGenerator`)
Formato: `<Verbo> <descrição no idioma ativo>` + corpo opcional em bullets. **Sem prefixo `tipo:`** — o tipo CC só escolhe o **verbo** (`feat`+só adições → `Implementa`; `fix` → `Corrige`; etc.); o tipo não é impresso. Subject limitado a 72 chars. Sem scope. Idioma injetado no construtor (`MessageLanguage`) e os mapas específicos vêm do `LanguagePack` (PT/EN). Ver detalhes em [[Geração de mensagem - Conventional Commits]] e [[Suporte Multilíngue PT-EN]].
- **Estratégia 1 (principal):** extrai comentários adicionados (`+`) do `git diff --cached --no-color`, filtra ruído (separadores, tags XML, código comentado, < 10 chars). O comentário mais impactante vira a descrição; os demais vão no corpo como bullets `- item`
- **Estratégia 2 (fallback):** `BuildSubject(type, changes)` → verbo do idioma + conceito dominante dos nomes de arquivo (ex: `"Implementa autenticação"` / `"Implement authentication"`)
- **Corpo em bullets:** até 5 frases de uma linha, uma por arquivo mais significativo, com verbo conforme o status git (`StatusVerb`)
- **Tradução EN→PT:** só roda quando o idioma de saída é pt-BR; em inglês os comentários passam intactos
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
> O `build.ps1` incrementa `major.minor.BUILD`, sincroniza versão em nuspec + csproj + `README.md` (carimba versão + data), builda em Release, copia a DLL p/ `Plugins\` (se Admin) e p/ `tools\net9.0-windows\`, e roda `nuget pack` removendo nupkgs antigos. Passo a passo completo em [[README — Instalação, Uso e Build]].

## 🔍 inspector (utilitário de dev)
Projeto console separado (`inspector\Program.cs`) que usa `MetadataLoadContext` + reflection para listar tipos/membros públicos de `GitExtensions.Extensibility.dll` e `GitUIPluginInterfaces.dll` — útil pra descobrir a API correta (`IGitPlugin`, `IGitUICommands`, `ICommitMessageManager`, `GitUIEventArgs` etc.) ao evoluir o plugin.

## 🐛 Armadilhas conhecidas
> [!warning] Não sobrescrever a mensagem do usuário
> O auto-refresh em `PostRepositoryChanged` só atualiza a caixa se ela estiver vazia ou contiver `_lastGeneratedMessage`. Mexer nisso pode apagar texto digitado à mão.

> [!warning] Localizar a caixa de texto do commit
> `FindCommitTextBox` tenta nomes conhecidos (`Message`, `commitMessageEditor`, `_commitMessage`, `commitMessage`) e cai num fallback heurístico (maior `TextBoxBase` multiline editável). Versões diferentes do GitExtensions mudam esses nomes.

## 🔢 Versionamento
- Versão atual: **1.0.40** (csproj + nuspec sincronizados pelo `build.ps1`)
- Esquema: `major.minor.BUILD`, BUILD auto-incrementado a cada build
- `README.md` carimba versão + data a cada build (FUNCIONALIDADES.md foi removido e unificado no README.md)

## 📜 Histórico de sessões
- [[2026-06-01 - Criação do cofre de neurônios CommitMsg]] — mapeamento inicial do projeto
- [[2026-06-02 - Aprimoramento mensagens pt-BR]] — formato `tipo: descrição` corrigido; `BuildSubject` como fallback; FUNCIONALIDADES.md unificado em README.md; deploy 1.0.19
- [[2026-06-05 - Formato imperativo pt-BR]] — verbo em 3ª pessoa capitalizado no subject
- [[2026-06-05 - Suporte multilíngue PT-EN]] — corpo em bullets; arquitetura i18n (LanguagePack + .resx); seletor de idioma; deploy 1.0.35

## 🔗 Relacionado
- [[README — Instalação, Uso e Build]] — espelho do `README.md` (instalação, build, requisitos, licença)
- [[Plugin MEF para GitExtensions]]
- [[Geração de mensagem - Conventional Commits]]
- [[Suporte Multilíngue PT-EN]]
- [[Estratégia de Detecção de Idioma]]
- [[GitExtensions.ZimerfeldTree]] — projeto irmão (plugin de árvore de branches)
- [[🔑 Fatos-Chave]]

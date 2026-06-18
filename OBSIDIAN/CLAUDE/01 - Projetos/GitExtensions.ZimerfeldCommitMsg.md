---
tipo: projeto
criado: 2026-06-01
atualizado: 2026-06-18
tags: [projeto, csharp, gitextensions, plugin, winforms, conventional-commits, i18n]
status: ativo
linguagem: C#
versao: 1.0.82
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
- **Template no diálogo de commit:** um item por idioma via `AddCommitTemplate(Func<string>)`. **Como o host chama:** `CommitTemplateManager.RegisteredTemplates` é enumerado ao **abrir** o dropdown e invoca o `Func` de **cada** item ali (em ordem), materializando o texto; o **clique** aplica esse texto via `ReplaceMessage` (replace) **sem** rechamar o `Func` nem dar callback de clique. Logo, "mensagem nova a cada clique" vem do host: abrir o menu gera os 3 idiomas na hora (frescos do stage) e o item clicado substitui a caixa. **Detecção da escolha + fixação de idioma:** como o `Func` roda para **todos** os itens na abertura, `GenerateForTemplate` **não** fixa idioma ali (fixaria sempre o último); em vez disso registra `msg → idioma` (`RememberTemplateMessage`) e assina o `TextChanged` da caixa (`EnsureTextChangedHook`); quando a caixa vira exatamente uma dessas mensagens (`DetectTemplateSelection`), fixa `_sessionLanguage` no idioma do item clicado → o auto-refresh (`EffectiveLanguage` = `_sessionLanguage ?? CurrentLanguage`) passa a regenerar **fresco do stage nesse idioma**. A fixação vale enquanto o diálogo vive (reiniciada ao fechar). **REGRA — stage vazio:** acionar um item do dropdown com o stage **sem arquivos** limpa **qualquer** texto da caixa (`ClearOpenCommitDialog`); como a geração só é vazia quando nada está em stage (com arquivos staged nunca é vazia), `msg` vazia ⟺ stage vazio. Obs.: o host avalia o `Func` na **abertura** do dropdown, então a limpeza ocorre ao abrir o menu com stage vazio (mesmo resultado vazio para os 3 idiomas)
- **Menu Plugins → ZimerfeldCommitMsg:** abre o `StartCommitDialog` com a mensagem já preenchida (`Execute`)
- **Re-sincronização CIRÚRGICA a cada abertura da janela de commit (REGRA):** toda vez que um `FormCommit` abre — seja pelo GitExtensions, pelo menu Plugins → ZimerfeldCommitMsg, ou por outro plugin (ex.: ZimerfeldTree) — o plugin re-vincula **só o que pode ter mudado** (`ResyncForCommitDialog`): **(1) GARANTE os 3 templates do dropdown** (`EnsureCommitTemplates`, idempotente — `AddCommitTemplate` só adiciona se o nome não existe no storage estático), de modo que **qualquer** plugin que abra o FormCommit tenha o dropdown gerando texto mesmo que a re-registração **assíncrona** do host (ao trocar de repo) ainda não tenha recolocado os templates; (2) re-aponta a fonte de settings; (3) garante **uma** assinatura de `PostRepositoryChanged`; (4) reinicia o estado de sessão. Resolve o `IGitUICommands` do capturado **ou do próprio FormCommit** (`UICommands` por reflexão), então funciona com commands defasado/nulo. **Não** remove nada nem mexe no `Application.Idle`. Detectado no `Application.Idle`; roda **uma vez por janela** (`_resyncedCommitForm`). ⚠️ Substitui o ciclo completo `Unregister`→`Register` anterior, que removia os templates e podia deixá-los removidos numa falha parcial — quebrando a geração via dropdown
- **Auto-preenchimento ao abrir e ao (un)stage:** ao **abrir** o `FormCommit` já com arquivos em stage, preenche automaticamente (detecção do form novo via `Application.Idle`, tratado uma vez por instância com `WeakReference`); e assina `PostRepositoryChanged` para regenerar ao stage/unstage. Só atualiza se a caixa estiver vazia (ou contiver a última mensagem que nós geramos). **Nunca sobrescreve texto digitado pelo usuário**. **Working dir = o do PRÓPRIO diálogo** (`GitModuleForm.Module.WorkingDir` por reflexão, via `GetCommitFormWorkingDir`), com fallback ao `IGitUICommands` capturado — corrige o bug de "parar de funcionar ao trocar de repositório/branch", em que a geração rodava no working dir antigo (stage diferente → mensagem errada/vazia)
- **Multilíngue (PT/EN):** `ChoiceSetting` "Idioma da mensagem / Message language" com rótulos bilíngues (`Automático/Automatic`, `Português/Portuguese`, `Inglês/English`); Auto detecta pelo `CultureInfo.CurrentUICulture`. Toda a saída + diálogos de UI são localizados. Ver [[Suporte Multilíngue PT-EN]]
- Marshalling seguro pra UI thread via `SynchronizationContext` capturado no `Register()`
- Tudo dentro de `try/catch` — o plugin **nunca derruba** o GitExtensions

## 🧠 Lógica de geração (`CommitMessageGenerator`)
Formato: `<Contexto> - <Verbo> <N> <arquivos> (<tipos>)` + corpo em bullets. **Prefixo de contexto (até 5 palavras)** derivado do conceito do **nome do arquivo de maior impacto** (`BuildContextPrefix`) — em **pt-BR** é uma frase de ação nominalizada: `StatusNoun` (`Adição`/`Remoção`/`Atualização`/`Renomeação`) + `de` + conceito traduzido e reordenado (ex.: `New Text Document` deletado → `Remoção de documento de texto`; `UserService` mod → `Atualização de gerenciamento de usuários`); em **inglês** é o conceito humanizado (`OverlayController` → `Overlay`). Garante que o título **nunca** seja genérico (ex.: nunca só `Adiciona 1 arquivo (fix)`). **Sem prefixo `tipo:`** — o tipo CC só escolhe o **verbo** (`feat`+só adições → `Implementa`; `fix` → `Corrige`; etc.); o tipo não é impresso. Subject limitado a 80 chars. Sem scope. Idioma injetado no construtor (`MessageLanguage`) e os mapas específicos vêm do `LanguagePack` (PT/EN). Ver detalhes em [[Geração de mensagem - Conventional Commits]] e [[Suporte Multilíngue PT-EN]].
- **Estratégia 1 (principal):** extrai comentários adicionados (`+`) do `git diff --cached --no-color`, filtra ruído (separadores, tags XML, código comentado, < 10 chars). O comentário mais impactante vira a descrição; os demais vão no corpo como bullets `- item`
- **Estratégia 2 (fallback):** `BuildSubject(type, changes)` → verbo do idioma + conceito dominante dos nomes de arquivo (ex: `"Implementa autenticação"` / `"Implement authentication"`)
- **Corpo em bullets:** até 5 frases de uma linha, uma por arquivo mais significativo, com verbo conforme o status git (`StatusVerb`); **sempre ao menos um bullet**, mesmo com um único arquivo — sem comentário nem conceito legível, recai no próprio nome do arquivo (`FormatFileLine`)
- **Tradução EN→PT:** só roda quando o idioma de saída é pt-BR; em inglês os comentários passam intactos. Branches/tipos CC são mascarados por placeholders `N` (sentinelas de controle, ausentes em código) e restaurados no fim — assim números **reais** do texto (ex.: `255`, `200`) nunca são confundidos com o índice, corrigindo o bug que cortava palavras (`assíncrona` → `255ncrona`)
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
> O `build.ps1` incrementa `major.minor.BUILD`, carimba versão + data **primeiro nos docs** (READMEs + este cofre Obsidian) e **só então** dá o _bump_ em nuspec + csproj, builda em Release, copia a DLL p/ `Plugins\` (se Admin) e p/ `tools\net9.0-windows\`, e roda `nuget pack` removendo nupkgs antigos. Passo a passo completo em [[README — Instalação, Uso e Build]] e [[Versionamento]].

## 🔍 inspector (utilitário de dev)
Projeto console separado (`inspector\Program.cs`) que usa `MetadataLoadContext` + reflection para listar tipos/membros públicos de `GitExtensions.Extensibility.dll` e `GitUIPluginInterfaces.dll` — útil pra descobrir a API correta (`IGitPlugin`, `IGitUICommands`, `ICommitMessageManager`, `GitUIEventArgs` etc.) ao evoluir o plugin.

## 🐛 Armadilhas conhecidas
> [!warning] Não sobrescrever a mensagem do usuário
> O auto-refresh em `PostRepositoryChanged` só atualiza a caixa se ela estiver vazia ou contiver `_lastGeneratedMessage`. Mexer nisso pode apagar texto digitado à mão.

> [!warning] Localizar a caixa de texto do commit
> `FindCommitTextBox` tenta nomes conhecidos (`Message`, `commitMessageEditor`, `_commitMessage`, `commitMessage`) e cai num fallback heurístico (maior `TextBoxBase` multiline editável). Versões diferentes do GitExtensions mudam esses nomes.

## 🔢 Versionamento
- Versão atual: **1.0.82** (csproj + nuspec sincronizados pelo `build.ps1`)
- Esquema: `major.minor.BUILD`, BUILD auto-incrementado a cada build
- A cada build, o `build.ps1` carimba versão + data nos **READMEs e neste cofre** (notas Projeto, README espelho, Versionamento, Visão Geral) **antes** do bump no nuspec/csproj

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

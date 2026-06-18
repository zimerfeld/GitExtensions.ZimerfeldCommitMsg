---
tipo: conhecimento
criado: 2026-06-08
atualizado: 2026-06-18
tags: [conhecimento, readme, instalacao, build, uso, conventional-commits, i18n]
fonte: README.md
versao: 1.0.78
---

# README — Instalação, Uso e Build

> Espelho fiel do `README.md` da raiz do repositório (carimbado em **v1.0.73 / 2026-06-18**), reconciliado com o código em 2026-06-18.
> Nota de projeto: [[GitExtensions.ZimerfeldCommitMsg]]. Lógica em [[Geração de mensagem - Conventional Commits]].
> O `build.ps1` carimba versão + data nos READMEs **e nesta nota** (frontmatter `versao:`/`atualizado:`) a cada build — reespelhar o corpo quando o README mudar de forma significativa.

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit analisando o conteúdo real das alterações staged. As mudanças são classificadas pelos tipos do **Conventional Commits** (`feat`/`fix`/`docs`/…) apenas para **escolher o verbo**; a mensagem é uma **frase iniciada por verbo** + bullets — **sem** o prefixo `tipo:`. **Multilíngue**: gera em **português-BR ou inglês**, detectado pelo SO, com **override manual**.

> [!important] Não há prefixo `tipo:`
> A saída **não** é `feat: …` / `fix: …` nem lista de tipos por vírgula. `Generate()` monta `FormatTitle(type, changes, desc)` = `<Verbo> <descrição>`. O tipo CC só seleciona o verbo. Ver [[Título como Lista de Types]] (decisão superada) e [[2026-06-05 - Formato imperativo pt-BR]].

## ✨ Funcionalidades em alto nível
- **Geração automática** a partir do conteúdo real do diff staged (não só dos nomes de arquivo).
- **Verbo guiado por Conventional Commits** — classifica as mudanças nos tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) e prefixa o **verbo** (3ª pessoa do presente em pt-BR / imperativo em inglês). O tipo não aparece.
- **Multilíngue (PT/EN)** — idioma automático pelo SO, com seletor manual de override.
- **Duas estratégias de conteúdo**: comentários do diff (principal) e nomes de arquivo (fallback).
- **Corpo em bullets** — até 5 frases de uma linha, cada uma resumindo a mudança mais significativa de um arquivo; **sempre ao menos um bullet**, mesmo com um único arquivo.
- **Tradução EN→PT** dos comentários (apenas quando a saída é pt-BR); em inglês passam intactos.
- **Três modos de integração**: template no diálogo de commit, menu Plugins e auto-preenchimento (ao abrir o diálogo e ao stage/unstage).
- **Não destrutivo** — nunca sobrescreve texto digitado manualmente.

## 🌐 Multilíngue (Português / Inglês)
Gera toda a mensagem (descrição, corpo e verbos) **no idioma escolhido** e localiza também as mensagens de UI. Ver [[Suporte Multilíngue PT-EN]] e [[Estratégia de Detecção de Idioma]].

### Seleção do idioma (duas formas, rótulos bilíngues)
**1. No dropdown de templates da tela de commit** — três itens planos, um por idioma (escolha rápida por commit):
```
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```

![[ScreenshotDropDown.png]]

**2. Em Configurações → Plugins → ZimerfeldCommitMsg** — o seletor **"Idioma da mensagem / Message language"** (`ChoiceSetting` `ZimerfeldCommitMsg_Language`) define o **padrão** usado pelo menu Plugins e pelo auto-refresh.

| Opção | Comportamento |
|---|---|
| `Automático/Automatic` | **Padrão.** Detecta pelo SO/GitExtensions (`CultureInfo.CurrentUICulture`: `pt-*` → português; qualquer outro → inglês). |
| `Português/Portuguese` | Força a saída em português-BR (resolver casa por subtrecho `portug`). |
| `Inglês/English` | Força a saída em inglês (casa `ingl`/`english`). |

> Escolher um item de idioma no dropdown **fixa** (`_sessionLanguage`) aquele idioma também para o auto-refresh enquanto o diálogo estiver aberto. Prioridade: `_sessionLanguage` > setting > SO.
> **Obs.:** o nó **ZimerfeldCommitMsg** só aparece em **Configurações → Plugins** depois que a DLL com o seletor (≥ 1.0.36) é instalada e o GitExtensions reiniciado.

### Exemplo lado a lado
| Português-BR | English |
|---|---|
| `Implementa autenticação` | `Implement authentication` |
| `- Adiciona autenticação` | `- Add authentication` |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |
| `- Adiciona gerenciamento de token` | `- Add token management` |

## 🔌 Modos de integração
- **Template no diálogo de commit:** um item por idioma no dropdown (`— Automático/Automatic`, `— Português/Portuguese`, `— Inglês/English`). Ao **abrir** o menu, o host gera os 3 idiomas na hora (frescos do stage); ao **clicar**, a caixa recebe a mensagem daquele idioma e o plugin detecta a escolha (via `TextChanged`) para **fixar** o idioma do auto-refresh. Ver [[../Fluxos/Template Dropdown (Auto-resumo)]].
- **Menu Plugins:** `Plugins → ZimerfeldCommitMsg` valida o repositório (`IsValidGitWorkingDir`) e abre `StartCommitDialog` com a mensagem já preenchida.
- **Auto-preenchimento ao abrir e ao stage/unstage:** ao **abrir** o diálogo já com arquivos em stage, preenche automaticamente (detecção do `FormCommit` novo via `Application.Idle`, tratado uma vez por instância com `WeakReference`); e enquanto o diálogo estiver aberto, `PostRepositoryChanged` regenera a mensagem quando arquivos entram/saem do stage. Só sobrescreve se a caixa estiver vazia ou contiver `_lastGeneratedMessage`. Ver [[Stage Trigger]].

## 📝 Formato da mensagem gerada
```
<Verbo> <descrição no idioma ativo>

- <bullet 1>
- <bullet 2>
```
- **Sem prefixo `tipo:`** — a primeira linha começa direto pelo verbo (ex.: `Implementa`, `Corrige`, `Atualiza`).
- **Sem scope**; **sem cor** (`git diff --no-color`, evita ANSI).
- **Limite de 72 caracteres** na primeira linha (`TruncateTitle` corta no último espaço + `…`).
- **Corpo sempre presente** — até 5 bullets de uma linha; havendo qualquer arquivo em stage, gera **ao menos um** bullet (mesmo com um único arquivo).

![[ScreenShots/ScreenshotCommitMsg.png]]

### Tipos detectados (definem o verbo)
Cada arquivo staged recebe um tipo (`DetermineAllTypes`). O verbo da primeira linha vem do tipo de **maior prioridade** (ordem: `feat` → `fix` → `refactor` → `perf` → `test` → `build` → `ci` → `chore` → `docs` → `style`). **Só o verbo é impresso.**

| Tipo | Atribuído a um arquivo quando… |
|---|---|
| `feat` | código **adicionado** (status `A`/`C`, categoria source/web) |
| `fix` | código **modificado/renomeado** (status `M`/`R`/`T`) |
| `docs` | documentação (`.md`, `.txt`, `.rst`, `.adoc`) |
| `test` | caminho de teste (pasta `test`/`tests`/`spec` ou sufixo `Test`/`Spec`) |
| `chore` | configuração (`.json`, `.yml`, etc.) **ou** qualquer arquivo **deletado** (`D`) |
| `build` | build (`.csproj`, `.sln`, `Dockerfile`, etc.) |
| `refactor` | demais casos sem padrão claro |

## 🧠 Como a mensagem é gerada

### Estratégia 1 — comentários do diff (principal)
`git diff --cached --no-color` → coleta linhas de comentário **adicionadas** (`+`) ou **removidas** (`-`). Prioridade pela categoria do arquivo (source=4 > web=3 > build=2 / config=1 / docs=1; teste=0); removidas entram com prioridade um grau menor. Varre até 15 linhas, usa até **5 comentários**; dentro da mesma prioridade, os mais longos primeiro.

**Padrões reconhecidos**
| Sintaxe | Linguagens |
|---|---|
| `// texto` ou `/// texto` | C#, Java, JavaScript, TypeScript, Go |
| `# texto` | Python, Shell, YAML, Ruby (ignorado em `.md`, onde `#` é heading) |

**Comentários rejeitados**
| Condição | Exemplo |
|---|---|
| Separador visual | `// ─────────────` |
| Tag XML de doc | `/// <summary>` |
| Código comentado (`{` `}`) | `// if (x) { return; }` |
| Código comentado (chamada de método) | `// método(argumento)` |
| Texto < 10 chars | `// ok` |
| Sem espaço (não é frase) | `// TODO` |

O comentário mais impactante vira a **descrição** (com o verbo inicial normalizado); os demais viram bullets:
```
Valida o token antes de processar a requisição

- Filtra requisições sem cabeçalho de autenticação
```
> Se o comentário tem conector de justificativa (` para `, ` pois `, …), a parte após o conector é descartada e a descrição passa a usar a frase funcional da Estratégia 2 (evita repetir o primeiro bullet).

Saída pt-BR → comentários em inglês são traduzidos antes do uso (descartados se ficarem com >25% de inglês). Saída inglês → comentários passam intactos. Nomes de branch (`feature/…`) e tipos CC são preservados na tradução.

### Estratégia 2 — nomes de arquivo (fallback)
Usada quando nenhum comentário válido é encontrado. Para cada arquivo staged, o nome (sem extensão):
1. Stem com `.` ou caractere não-ASCII → **ignorado** (nome de assembly/projeto).
2. **Remove prefixo de interface** — `IUserService` → `UserService`.
3. **Remove sufixo arquitetural** (maior correspondência primeiro): `ServiceTests`, `ControllerTests`, `RepositoryTests` … `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator` … `Helper`, `Provider`, `Factory`, `Builder`, `Middleware`, `Validator`, `Mapper`, `Resolver`, `Extension`, `Util` … `Tests`, `Test`, `Spec`, `Mock`, `Command`, `Query`, `Event` … `Dto`, `ViewModel`, `Model`, `Entity`, `Config`, `Settings`, `Options` … `Facade`, `Adapter`, `Client`, `Endpoint`, `Base`, `Impl` …
4. **Filtros:** < 2 chars → rejeitado; **`RejectedVocabulary`** (qualquer palavra proibida → rejeita, ex.: `zimerfeld`/`git`/`extensions`); nome com 3+ palavras → rejeitado como namespace, **exceto** se for conceito do dicionário (`HasConcept`) **ou** todas as palavras forem vocabulário reconhecido (`IsKnownVocabulary`: `KnownVocabulary` + `WordTranslations` + `ConceptPhrases`). Ex.: `New Text Document` passa; `ZimerfeldCommitMsg` não (`zimerfeld` rejeitado).

**Mapeamento conceito → frase (pt-BR, exemplos do dicionário)**
| Conceito | Frase |
|---|---|
| `Auth` / `Authentication` | autenticação |
| `User` / `Users` | gerenciamento de usuários |
| `Token` / `Jwt` | gerenciamento de token / autenticação JWT |
| `Payment` | processamento de pagamento |
| `Order` | processamento de pedidos |
| `Notification` | notificações |
| `Cache` | cache |
| `Migration` | migração de banco de dados |
| `Report` | relatórios |
| `CommitMessage` | mensagem de commit |

**Verbos por tipo** (escolhem o verbo da primeira linha)
| Tipo | pt-BR | en | Condição |
|---|---|---|---|
| `feat` | Implementa / Adiciona | Implement / Add | `Implementa` se só há adições; senão `Adiciona` |
| `fix` | Corrige | Fix | — |
| `refactor` | Refatora | Refactor | — |
| `docs` | Documenta / Atualiza | Document / Update | `Documenta` se há adições; senão `Atualiza` |
| `build` | Configura | Configure | — |
| `chore` | Remove / Configura | Remove / Configure | `Remove` se há deleções; senão `Configura` |
| `test` | Adiciona | Add | — |
| `perf` | Otimiza | Optimize | — |
| `ci` | Configura | Configure | — |
| `style` | Padroniza | Standardize | — |

> Se a descrição já começa com verbo conhecido, ele é **normalizado** (pt-BR: 3ª pessoa → `Filtra`; en: imperativo → `Filter`) em vez de prefixar novo verbo.

**Corpo (body):** até **5 bullets** ordenados por relevância do arquivo — **ao menos um, mesmo com um único arquivo** — `- <StatusVerb> <conceito>` (status: `A`/`C` → Adiciona/Add, `D` → Remove/Remove, `R` → Renomeia/Rename, demais → Atualiza/Update). Quando o nome não rende conceito legível e não há comentário, o bullet recai no **próprio nome do arquivo** (ex.: `Remove New Text Document.txt`):
```
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```

### Exemplos de mensagens geradas
| Arquivos staged | pt-BR | en |
|---|---|---|
| `AuthService.cs` adicionado | `Implementa autenticação` | `Implement authentication` |
| `PaymentService.cs` adicionado | `Implementa processamento de pagamento` | `Implement payment processing` |
| `UserService.cs` modificado | `Corrige gerenciamento de usuários` | `Fix user management` |
| `README.md` modificado | `Atualiza documentação` | `Update documentation` |
| `UserService.cs` + `TokenService.cs` adicionados | `Implementa gerenciamento de usuários` + bullets | `Implement user management` + bullets |
| `.cs` com `// Valida o token antes de processar a requisição` | `Valida o token antes de processar a requisição` | _(comentário em pt passa intacto)_ |

## ✅ Requisitos
- PowerShell 5.1 ou superior.
- Permissão de **Administrador** para instalar/desinstalar.

## 📦 Instalação
**Opção A — PowerShell (recomendado), como Administrador:**
```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

![[ScreenShots/ScreenshotInstall.png]]

**Opção B — Manual:** copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para `C:\Program Files\GitExtensions\Plugins\` e reinicie o GitExtensions.

## 🗑️ Desinstalação
```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

![[ScreenShots/ScreenshotUninstall.png]]

A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

## 🛠️ Build e versionamento
A cada execução do `build.ps1`, o script:
1. Lê a versão atual do `.nuspec`.
2. Calcula a nova versão: incrementa o `build` em +1 → `major.minor.build`.
3. Escreve a nova versão e data **primeiro nos docs**: os READMEs e o cofre Obsidian.
4. Só então dá o _bump_ da versão no `.nuspec` e no `.csproj`.
5. Compila em Release.
6. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*.
7. Atualiza `tools\net9.0-windows\` com a DLL nova.
8. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`.
9. Remove `.nupkg` de versões anteriores.

> [!info] Conteúdo bilíngue no nuget.org
> Tudo que aparece na página do pacote em **nuget.org** é **bilíngue (cada parágrafo em inglês + o correspondente em português)**. São duas fontes: a `<description>` do `.nuspec` (exibida no topo) — um parágrafo EN seguido da tradução PT — e o `README.md` (empacotado via `<readme>`), já bilíngue, com rótulos de link também em EN/PT (`Pacote NuGet`, `Repositório no GitHub`). O pacote ainda inclui `README.pt-BR.md` e `README.en-US.md` completos para quem abrir o conteúdo do `.nupkg`.

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

![[ScreenShots/ScreenshotBuild.png]]

**Deploy rápido (sem incrementar versão):**
```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

![[ScreenShots/ScreenshotUpdate.png]]

Ver [[Versionamento]] e [[Instalação e Deploy]].

## 🤝 Plugins relacionados
- **[GitExtensions.ZimerfeldTree](https://www.nuget.org/packages/GitExtensions.ZimerfeldTree/)** — exibe branches hierarquicamente numa janela de árvore persistente e não-modal; branches separados por `/` viram nós de pasta aninhados sob LOCAL, REMOTES e tags. Por **zimerfeld**. Ver [[GitExtensions.ZimerfeldTree]].

## 💜 Apoie o projeto
Ajude a manter este projeto sempre atualizado via **GitHub Sponsors**: [github.com/sponsors/zimerfeld](https://github.com/sponsors/zimerfeld), ou via **Ko-fi**: [ko-fi.com/C0D621FCGD](https://ko-fi.com/C0D621FCGD). No topo do `README.md`, lado a lado e com as mesmas dimensões (altura de 28px), ficam o badge **Sponsor** (shields.io, `style=for-the-badge`) e o botão oficial **"Buy me a coffee"** do Ko-fi (`<img src="https://storage.ko-fi.com/cdn/kofi2.png" height="28">` clicável para a URL do Ko-fi); o `.github/FUNDING.yml` aponta para o GitHub Sponsors, e o README empacotado leva o link clicável para a página do pacote no nuget.org.

## 📄 Licença
[MIT](LICENSE.txt)

## 🔗 Relacionado
- [[GitExtensions.ZimerfeldCommitMsg]]
- [[Geração de mensagem - Conventional Commits]]
- [[Suporte Multilíngue PT-EN]]
- [[🔑 Fatos-Chave]]

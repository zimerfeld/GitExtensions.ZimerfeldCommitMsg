---
tipo: conhecimento
criado: 2026-06-08
atualizado: 2026-06-09
tags: [conhecimento, readme, instalacao, build, uso, conventional-commits, i18n]
fonte: README.md
versao: 1.0.40
---

# README — Instalação, Uso e Build

> Espelho fiel do `README.md` da raiz do repositório (carimbado em **v1.0.40 / 2026-06-09**), reconciliado com o código em 2026-06-09.
> Nota de projeto: [[GitExtensions.ZimerfeldCommitMsg]]. Lógica em [[Geração de mensagem - Conventional Commits]].
> O `build.ps1` carimba versão + data no `README.md` a cada build — reespelhar esta nota quando o README mudar de forma significativa.

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit analisando o conteúdo real das alterações staged. As mudanças são classificadas pelos tipos do **Conventional Commits** (`feat`/`fix`/`docs`/…) apenas para **escolher o verbo**; a mensagem é uma **frase iniciada por verbo** + bullets — **sem** o prefixo `tipo:`. **Multilíngue**: gera em **português-BR ou inglês**, detectado pelo SO, com **override manual**.

> [!important] Não há prefixo `tipo:`
> A saída **não** é `feat: …` / `fix: …` nem lista de tipos por vírgula. `Generate()` monta `FormatTitle(type, changes, desc)` = `<Verbo> <descrição>`. O tipo CC só seleciona o verbo. Ver [[Título como Lista de Types]] (decisão superada) e [[2026-06-05 - Formato imperativo pt-BR]].

## ✨ Funcionalidades em alto nível
- **Geração automática** a partir do conteúdo real do diff staged (não só dos nomes de arquivo).
- **Verbo guiado por Conventional Commits** — classifica as mudanças nos tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) e prefixa o **verbo** (3ª pessoa do presente em pt-BR / imperativo em inglês). O tipo não aparece.
- **Multilíngue (PT/EN)** — idioma automático pelo SO, com seletor manual de override.
- **Duas estratégias de conteúdo**: comentários do diff (principal) e nomes de arquivo (fallback).
- **Corpo em bullets** — até 5 frases de uma linha, cada uma resumindo a mudança mais significativa de um arquivo.
- **Tradução EN→PT** dos comentários (apenas quando a saída é pt-BR); em inglês passam intactos.
- **Três modos de integração**: template no diálogo de commit, menu Plugins e auto-refresh ao stage/unstage.
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
- **Template no diálogo de commit:** um item por idioma no dropdown (`— Automático/Automatic`, `— Português/Portuguese`, `— Inglês/English`); selecione e a mensagem é gerada nesse idioma e preenchida pelo GitExtensions.
- **Menu Plugins:** `Plugins → ZimerfeldCommitMsg` valida o repositório (`IsValidGitWorkingDir`) e abre `StartCommitDialog` com a mensagem já preenchida.
- **Auto-refresh ao stage/unstage:** enquanto o diálogo estiver aberto, `PostRepositoryChanged` regenera a mensagem quando arquivos entram/saem do stage; só sobrescreve se a caixa estiver vazia ou contiver `_lastGeneratedMessage`. Ver [[Stage Trigger]].

## 📝 Formato da mensagem gerada
```
<Verbo> <descrição no idioma ativo>

- <bullet 1>
- <bullet 2>
```
- **Sem prefixo `tipo:`** — a primeira linha começa direto pelo verbo (ex.: `Implementa`, `Corrige`, `Atualiza`).
- **Sem scope**; **sem cor** (`git diff --no-color`, evita ANSI).
- **Limite de 72 caracteres** na primeira linha (`TruncateTitle` corta no último espaço + `…`).
- **Corpo opcional** — até 5 bullets de uma linha (gerado com 2+ arquivos ou comentários extras).

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
4. **Filtros:** < 2 chars → rejeitado; não está no dicionário e tem > 2 palavras PascalCase → rejeitado (nome de projeto).

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

**Corpo (body):** com 2+ arquivos, até **5 bullets** ordenados por relevância do arquivo, `- <StatusVerb> <conceito>` (status: `A`/`C` → Adiciona/Add, `D` → Remove/Remove, `R` → Renomeia/Rename, demais → Atualiza/Update):
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
**Opção B — Manual:** copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para `C:\Program Files\GitExtensions\Plugins\` e reinicie o GitExtensions.

## 🗑️ Desinstalação
```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```
A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

## 🛠️ Build e versionamento
A cada execução do `build.ps1`, o script:
1. Lê a versão atual do `.nuspec`.
2. Incrementa o `build` em +1 → `major.minor.build`.
3. Atualiza `.nuspec`, `.csproj` e `README.md` com nova versão e data.
4. Compila em Release.
5. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*.
6. Atualiza `tools\net9.0-windows\` com a DLL nova.
7. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`.
8. Remove `.nupkg` de versões anteriores.

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```
**Deploy rápido (sem incrementar versão):**
```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```
Ver [[Versionamento]] e [[Instalação e Deploy]].

## 🤝 Plugins relacionados
- **[GitExtensions.ZimerfeldTree](https://www.nuget.org/packages/GitExtensions.ZimerfeldTree/)** — exibe branches hierarquicamente numa janela de árvore persistente e não-modal; branches separados por `/` viram nós de pasta aninhados sob LOCAL, REMOTES e tags. Por **zimerfeld**. Ver [[GitExtensions.ZimerfeldTree]].

## 📄 Licença
[MIT](LICENSE.txt)

## 🔗 Relacionado
- [[GitExtensions.ZimerfeldCommitMsg]]
- [[Geração de mensagem - Conventional Commits]]
- [[Suporte Multilíngue PT-EN]]
- [[🔑 Fatos-Chave]]

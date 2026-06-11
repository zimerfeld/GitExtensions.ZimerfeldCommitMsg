**Versão:** 1.0.63
**Atualizado em:** 2026-06-11

# GitExtensions.ZimerfeldCommitMsg

<p align="right">
  <sub>Ajude a manter este projeto sempre atualizado 💜</sub><br>
  <a href="https://github.com/sponsors/zimerfeld">
    <img src="https://img.shields.io/badge/Apoiar-zimerfeld-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white" alt="Apoiar @zimerfeld no GitHub Sponsors">
  </a>
</p>

[English](README.en-US.md) | [Português-BR](README.pt-BR.md)

![Screenshot](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotUsage.png)

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit analisando o conteúdo real das alterações staged. As mudanças são classificadas pelos tipos do **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) para escolher o **verbo** adequado, e a mensagem resultante é uma **frase iniciada por verbo** seguida de um corpo em bullets — **sem** o prefixo `tipo:`. **Multilíngue**: gera em **português-BR ou inglês**, detectado automaticamente pelo idioma do sistema operacional, com **override manual** nas configurações do plugin.

---

## Funcionalidades em alto nível

- **Geração automática** da mensagem de commit a partir do conteúdo real do diff staged (não apenas dos nomes de arquivo).
- **Verbo guiado por Conventional Commits** — classifica as mudanças nos tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) e prefixa o **verbo** correspondente (3ª pessoa do presente em pt-BR / imperativo em inglês). O tipo em si **não** aparece na mensagem.
- **Multilíngue (Português/Inglês)** — idioma escolhido automaticamente pelo SO, com seletor manual de override.
- **Duas estratégias de conteúdo**: baseada em comentários do diff (principal) e baseada em nomes de arquivo (fallback).
- **Corpo em bullets** — até 5 frases de uma linha, cada uma resumindo a mudança mais significativa de um arquivo.
- **Tradução inglês → português** dos comentários (apenas quando a saída é pt-BR); em inglês, os comentários passam intactos.
- **Três modos de integração**: template no diálogo de commit, menu Plugins e auto-refresh ao stage/unstage.
- **Não destrutivo** — nunca sobrescreve texto digitado manualmente pelo usuário.

---

## Multilíngue (Português / Inglês)

O plugin gera toda a mensagem (descrição, corpo e verbos) **no idioma escolhido**, e localiza também as mensagens de UI (diálogos de aviso).

### Seleção do idioma

Há **duas formas** de escolher o idioma, com os mesmos rótulos bilíngues (claros independentemente do idioma do sistema):

**1. No dropdown de templates da tela de commit** — três itens, um por idioma (escolha rápida por commit):

```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```

![Dropdown de templates de commit com os três itens de idioma](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotUsage.png)

**2. Em Configurações → Plugins → ZimerfeldCommitMsg** — o seletor **"Idioma da mensagem / Message language"** define o **padrão** usado pelo menu Plugins e pelo auto-refresh.

| Opção | Comportamento |
|---|---|
| `Automático/Automatic` | **Padrão.** Detecta pelo idioma do sistema operacional/GitExtensions (`pt-*` → português; qualquer outro → inglês). |
| `Português/Portuguese` | Força a saída em português-BR. |
| `Inglês/English` | Força a saída em inglês. |

> Escolher um item de idioma no dropdown **fixa** aquele idioma também para o auto-refresh enquanto o diálogo estiver aberto. O override prevalece sobre o idioma do SO; a detecção automática usa `CultureInfo.CurrentUICulture`.
>
> **Obs.:** o nó **ZimerfeldCommitMsg** só aparece na árvore de **Configurações → Plugins** depois que a DLL com o seletor (≥ 1.0.36) é instalada e o GitExtensions é reiniciado.

### Exemplo lado a lado

| Português-BR | English |
|---|---|
| `Implementa autenticação` | `Implement authentication` |
| `- Adiciona autenticação` | `- Add authentication` |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |
| `- Adiciona gerenciamento de token` | `- Add token management` |

---

## Modos de integração

### Template no diálogo de commit

No dropdown de templates da janela de commit há um item por idioma — **"Zimerfeld Commit Msg — Automático/Automatic"**, **"— Português/Portuguese"** e **"— Inglês/English"**. Selecione um e a mensagem é gerada nesse idioma e preenchida automaticamente no campo de texto.

### Menu Plugins

Acesse **Plugins → ZimerfeldCommitMsg**. O diálogo de commit abre com a mensagem já preenchida.

### Auto-refresh ao stage/unstage

Enquanto o diálogo de commit estiver aberto, a mensagem é atualizada automaticamente sempre que arquivos entrarem ou saírem do stage. O texto digitado manualmente nunca é sobrescrito.

---

## Formato da mensagem gerada

```text
<Verbo> <descrição no idioma ativo>

- <bullet 1>
- <bullet 2>
```

- **Sem prefixo `tipo:`** — a primeira linha começa direto pelo **verbo** escolhido a partir do tipo (ex.: `Implementa`, `Corrige`, `Atualiza`).
- **Sem scope** — evita redundância com o nome do projeto.
- **Sem realce de cores** — usa `git diff --no-color` para evitar códigos ANSI.
- **Limite de 72 caracteres** na primeira linha (corta no último espaço e adiciona `…`).
- **Descrição e verbos no idioma ativo** (português-BR ou inglês).
- **Corpo opcional** — até 5 bullets de uma linha, gerado quando há 2+ arquivos ou comentários extras.

![Mensagem de commit gerada no diálogo de commit do GitExtensions](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotCommitMsg.png)

### Tipos detectados (definem o verbo)

Cada arquivo staged recebe um tipo. O **verbo** da primeira linha vem do tipo de **maior prioridade** entre todos os arquivos (ordem: `feat` → `fix` → `refactor` → `perf` → `test` → `build` → `ci` → `chore` → `docs` → `style`). **O tipo não é impresso** — só seleciona o verbo.

| Tipo | Atribuído a um arquivo quando… |
|---|---|
| `feat` | arquivo de código **adicionado** (status `A`/`C`, categoria source/web) |
| `fix` | arquivo de código **modificado/renomeado** (status `M`/`R`/`T`) |
| `docs` | arquivo de documentação (`.md`, `.txt`, `.rst`, `.adoc`) |
| `test` | caminho de teste (pasta `test`/`tests`/`spec` ou sufixo `Test`/`Spec`) |
| `chore` | arquivo de configuração (`.json`, `.yml`, etc.) **ou** qualquer arquivo **deletado** (status `D`) |
| `build` | arquivo de build (`.csproj`, `.sln`, `Dockerfile`, etc.) |
| `refactor` | demais casos sem padrão claro |

---

## Como a mensagem é gerada

### Estratégia 1 — Baseada em comentários do diff (principal)

Executa `git diff --cached --no-color` e coleta as linhas de **comentário** que foram **adicionadas** (`+`) ou **removidas** (`-`). As adicionadas têm prioridade pela **categoria do arquivo** (source = 4 > web = 3 > build = 2 / config = 1 / docs = 1; arquivos de teste = 0); as removidas entram com prioridade um grau menor. São varridas até 15 linhas e usados até **5 comentários**; dentro de uma mesma prioridade, os mais longos vêm primeiro.

#### Padrões reconhecidos

| Sintaxe | Linguagens |
|---|---|
| `// texto` ou `/// texto` | C#, Java, JavaScript, TypeScript, Go |
| `# texto` | Python, Shell, YAML, Ruby |

#### Comentários rejeitados

| Condição | Exemplo rejeitado |
|---|---|
| Separador visual | `// ─────────────────────` |
| Tag XML de documentação | `/// <summary>` |
| Código comentado (tem `{` `}`) | `// if (x) { return; }` |
| Código comentado (chamada de método) | `// método(argumento)` |
| Texto muito curto (< 10 chars) | `// ok` |
| Sem espaço (não é frase) | `// TODO` |

#### Como os comentários são usados

O comentário mais impactante vira a **descrição** na primeira linha (com o verbo inicial normalizado para 3ª pessoa/imperativo). Os demais aparecem no **corpo** como itens de lista:

```text
Valida o token antes de processar a requisição

- Filtra requisições sem cabeçalho de autenticação
```

> Se o comentário escolhido contém um conector de justificativa (` para `, ` pois `, ` porque `, …), a parte após o conector é descartada da primeira linha, e a descrição passa a usar a frase funcional dos nomes de arquivo (Estratégia 2) para evitar repetição com os bullets.

Quando a saída é **português-BR**, comentários em inglês são traduzidos automaticamente antes de serem usados (e descartados se a tradução ficar com mais de 25% de inglês). Quando a saída é **inglês**, os comentários passam intactos (e comentários em português permanecem em português). Nomes de branch (`feature/…`, `release/…`) e tipos Conventional Commits são preservados na tradução.

---

### Estratégia 2 — Baseada em nomes de arquivo (fallback)

Usada quando nenhum comentário válido é encontrado no diff.

#### Extração de conceitos semânticos

Para cada arquivo staged, o nome (sem extensão) passa por:

1. **Remoção do prefixo de interface** — `IUserService` → `UserService`
2. **Remoção do sufixo arquitetural** (maior correspondência primeiro):

   `ServiceTests`, `ControllerTests`, `RepositoryTests` …
   `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator` …
   `Middleware`, `Validator`, `Mapper`, `Factory`, `Builder` …
   `Tests`, `Test`, `Spec`, `Mock`, `Command`, `Query`, `Event` …
   `Dto`, `ViewModel`, `Model`, `Entity`, `Config`, `Settings` …
   `Facade`, `Adapter`, `Client`, `Endpoint`, `Base`, `Impl` …

3. **Filtros de qualidade:**
   - Nome com ponto no stem → rejeitado (ex: `GitExtensions.ZimerfeldCommitMsg`)
   - Mais de 2 palavras PascalCase sem entrada no dicionário → rejeitado (nomes de projeto)

#### Mapeamento de conceito → frase em pt-BR (exemplos)

| Conceito extraído | Frase gerada |
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

#### Verbos por tipo

O verbo é escolhido pelo tipo (e, em alguns casos, pelo contexto das mudanças):

| Tipo | Verbo (pt-BR) | Verbo (en) | Condição |
|---|---|---|---|
| `feat` | Implementa / Adiciona | Implement / Add | `Implementa` quando só há adições; `Adiciona` caso contrário |
| `fix` | Corrige | Fix | — |
| `refactor` | Refatora | Refactor | — |
| `docs` | Documenta / Atualiza | Document / Update | `Documenta` quando há adições; `Atualiza` caso contrário |
| `build` | Configura | Configure | — |
| `chore` | Remove / Configura | Remove / Configure | `Remove` quando há deleções; `Configura` caso contrário |
| `test` | Adiciona | Add | — |
| `perf` | Otimiza | Optimize | — |
| `ci` | Configura | Configure | — |
| `style` | Padroniza | Standardize | — |

> Se a descrição já começa com um verbo conhecido (ex.: o comentário `filtrar stems…`), ele é **normalizado** (pt-BR: 3ª pessoa do presente → `Filtra`; en: imperativo → `Filter`) em vez de prefixar um novo verbo do tipo.

#### Corpo da mensagem (body)

Quando há 2+ arquivos, o corpo lista até **5 bullets**, cada um com uma frase de uma linha resumindo a mudança mais significativa de um arquivo (ordenados por relevância do arquivo). O verbo de cada bullet acompanha o status no git (adicionado/removido/renomeado/modificado):

```text
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```

---

## Exemplos de mensagens geradas

| Arquivos staged | Mensagem gerada (pt-BR) | Mensagem gerada (en) |
|---|---|---|
| `AuthService.cs` adicionado | `Implementa autenticação` | `Implement authentication` |
| `PaymentService.cs` adicionado | `Implementa processamento de pagamento` | `Implement payment processing` |
| `UserService.cs` modificado | `Corrige gerenciamento de usuários` | `Fix user management` |
| `README.md` modificado | `Atualiza documentação` | `Update documentation` |
| `UserService.cs` + `TokenService.cs` adicionados | `Implementa gerenciamento de usuários`<br>`- Adiciona gerenciamento de usuários`<br>`- Adiciona gerenciamento de token` | `Implement user management`<br>`- Add user management`<br>`- Add token management` |
| `.cs` modificado com comentário `// Valida o token antes de processar a requisição` | `Valida o token antes de processar a requisição` | _(comentário em pt passa intacto)_ |

---

## Requisitos

- PowerShell 5.1 ou superior
- Permissão de **Administrador** para instalar/desinstalar

---

## Instalação

### Opção A — Via PowerShell (recomendado)

Execute o PowerShell **como Administrador**:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

![Saída do install.ps1 confirmando a instalação do plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotInstall.png)

### Opção B — Manual

Copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para:

```text
C:\Program Files\GitExtensions\Plugins\
```

Reinicie o GitExtensions.

---

## Desinstalação

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

![Saída do uninstall.ps1 confirmando a remoção do plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotUninstall.png)

A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

---

## Build e versionamento

A cada execução do `build.ps1`, o script:

1. Lê a versão atual do `.nuspec`
2. Incrementa o `build` em +1 → `major.minor.build`
3. Atualiza `.nuspec`, `.csproj` e os READMEs com a nova versão e data
4. Compila em Release
5. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*
6. Atualiza `tools\net9.0-windows\` com a DLL nova
7. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
8. Remove `.nupkg` de versões anteriores

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

![Saída do build.ps1 com incremento de versão e empacotamento](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotBuild.png)

### Deploy rápido (sem incrementar versão)

Para atualizar apenas a DLL durante desenvolvimento:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

![Saída do update-dll.ps1 atualizando apenas a DLL](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/ScreenshotUpdate.png)

---

## Plugins relacionados

### [GitExtensions.ZimerfeldTree](https://www.nuget.org/packages/GitExtensions.ZimerfeldTree/)

Plugin para GitExtensions que exibe branches hierarquicamente em uma janela de árvore persistente e não-modal. Branches separados por `/` são mostrados como nós de pasta aninhados sob três seções fixas — LOCAL, REMOTES e tags. Por **zimerfeld**.

---

## Licença

[MIT](LICENSE.txt)

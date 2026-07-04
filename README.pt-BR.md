# GitExtensions.ZimerfeldCommitMsg

![Icone](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/src/GitExtensions.ZimerfeldCommitMsg/Resources/icon-128.png)

[![NuGet version](https://img.shields.io/nuget/v/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=NuGet)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/) &nbsp; [![NuGet downloads](https://img.shields.io/nuget/dt/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=Downloads)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/)

Este plugin é construído e mantido no meu tempo livre. Se ele te poupa tempo a cada commit, um patrocínio ajuda a mantê-lo atualizado para as novas versões do GitExtensions. 💜

[![GitHub Sponsor](https://img.shields.io/badge/Sponsor-zimerfeld-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white)](https://github.com/sponsors/zimerfeld) &nbsp;&nbsp;&nbsp;&nbsp; [![Ko-fi](https://img.shields.io/badge/Ko--fi-Buy%20me%20a%20coffee-FF5E2B?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/C0D621FCGD)

**Versão:** 1.0.97
**Atualizado em:** 2026-07-04

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit analisando o conteúdo real das alterações staged. As mudanças são classificadas pelos tipos do **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) para escolher o **verbo** adequado, e a mensagem resultante é uma **frase iniciada por verbo** seguida de um corpo em bullets — **sem** o prefixo `tipo:`. **Multilíngue**: gera em **português-BR ou inglês**, detectado automaticamente pelo idioma do sistema operacional, com **override manual** nas configurações do plugin.

![Screenshot](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

[English](README.en-US.md) | [Português-BR](README.pt-BR.md)

[...Mais informações](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg "Mais informações sobre GitExtensions.ZimerfeldTree package")

---

## Funcionalidades em alto nível

- **Geração automática** da mensagem de commit a partir do conteúdo real do diff staged (não apenas dos nomes de arquivo).
- **Verbo guiado por Conventional Commits** — classifica as mudanças nos tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) e prefixa o **verbo** correspondente (3ª pessoa do presente em pt-BR / imperativo em inglês). O tipo em si **não** aparece na mensagem.
- **Multilíngue (Português/Inglês)** — idioma escolhido automaticamente pelo SO, com seletor manual de override.
- **Duas estratégias de conteúdo**: baseada em comentários do diff (principal) e baseada em nomes de arquivo (fallback). A extração reconhece várias sintaxes de comentário — `//`, `///`, blocos C-style `/* */` `/** */`, JSDoc `* `, HTML `<!-- -->`, SQL/Lua `--`, VB `'` e `#`.
- **Vocabulário por repositório** — um arquivo opcional `.zimerfeldcommitmsg.json` estende o vocabulário conhecido/rejeitado e as frases de conceito sem recompilar.
- **Corpo em bullets** — até 5 frases de uma linha, cada uma resumindo a mudança mais significativa de um arquivo; **sempre ao menos um bullet**, mesmo com um único arquivo alterado.
- **Tradução inglês → português** dos comentários (apenas quando a saída é pt-BR); em inglês, os comentários passam intactos.
- **Saneamento das frases** — descarta comentários com **delimitadores desbalanceados** (`()`, `[]`, `{}`, aspas `"` `'` `` ` ``, `<>`) ou que **terminem em palavra de ligação solta** (`de`, `para`, `que`…); entre os candidatos válidos, escolhe o de **melhor qualidade** (não o mais longo).
- **Nunca vazio** — havendo arquivos em stage, sempre produz ao menos a linha-resumo (`<verbo> N arquivos`).
- **Três modos de integração**: template no diálogo de commit, menu Plugins e auto-preenchimento (ao abrir o diálogo e ao stage/unstage).
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

![Dropdown de templates de commit com os três itens de idioma](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

**2. Em Configurações → Plugins → ZimerfeldCommitMsg** — o seletor **"Idioma da mensagem / Message language"** define o **padrão** usado pelo menu Plugins e pelo auto-refresh.

| Opção                  | Comportamento                                                                                                       |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------- |
| `Automático/Automatic` | **Padrão.** Detecta pelo idioma do sistema operacional/GitExtensions (`pt-*` → português; qualquer outro → inglês). |
| `Português/Portuguese` | Força a saída em português-BR.                                                                                      |
| `Inglês/English`       | Força a saída em inglês.                                                                                            |

> Escolher um item no **dropdown** fixa aquele idioma para o **auto-refresh** enquanto o diálogo estiver aberto (tem prioridade sobre o setting/SO). O seletor em Configurações define o padrão usado quando nenhum item do dropdown foi escolhido (e pelo menu Plugins). A detecção automática usa `CultureInfo.CurrentUICulture`.
>
> **Obs.:** o nó **ZimerfeldCommitMsg** só aparece na árvore de **Configurações → Plugins** depois que a DLL com o seletor (≥ 1.0.36) é instalada e o GitExtensions é reiniciado.

### Exemplo lado a lado

| Português-BR                            | English                    |
| --------------------------------------- | -------------------------- |
| `Implementa autenticação`               | `Implement authentication` |
| `- Adiciona autenticação`               | `- Add authentication`     |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |
| `- Adiciona gerenciamento de token`     | `- Add token management`   |

---

## Modos de integração

### Template no diálogo de commit

No dropdown de templates da janela de commit há um item por idioma — **"Zimerfeld Commit Msg — Automático/Automatic"**, **"— Português/Portuguese"** e **"— Inglês/English"**. Selecione um e a mensagem é gerada nesse idioma e preenchida automaticamente no campo de texto.

> Ao **abrir** o dropdown, os três idiomas são gerados na hora (mensagens frescas a partir do stage atual); **clicar** em um item **substitui** o conteúdo do campo pela mensagem daquele idioma — inclusive texto digitado manualmente. (Isso difere do auto-refresh, que preserva o texto do usuário.)

### Menu Plugins

Acesse **Plugins → ZimerfeldCommitMsg**. O diálogo de commit abre com a mensagem já preenchida.

### Auto-refresh ao abrir e ao stage/unstage

Ao **abrir** o diálogo de commit já com arquivos em stage, a mensagem é preenchida automaticamente (sem precisar mexer no stage). E enquanto o diálogo estiver aberto, ela é atualizada sempre que arquivos entrarem ou saírem do stage. O texto digitado manualmente nunca é sobrescrito.

---

## Formato da mensagem gerada

```text
<Contexto> - <Verbo> <N> <arquivos> (<tipos>)

- <bullet 1>
- <bullet 2>
```

- **Prefixo de contexto (até 5 palavras)** — a primeira linha começa por um conceito derivado do **nome do arquivo de maior impacto**, seguido de ` - `. Em **pt-BR** o prefixo é uma **frase de ação nominalizada**: substantivo da ação por status (`Adição`/`Remoção`/`Atualização`/`Renomeação`) + `de` + conceito traduzido e reordenado (ex.: `New Text Document` deletado → `Remoção de documento de texto`; `UserService` modificado → `Atualização de gerenciamento de usuários`). Em **inglês** mantém o conceito humanizado (ex.: `OverlayController` → `Overlay`). Garante que o título **nunca** seja genérico, dando identidade ao commit. É omitido só quando nenhum arquivo rende um conceito legível.
- **Resumo consolidado** — após o contexto vem o **verbo** do tipo dominante + a **contagem de arquivos** + a lista de **tipos** envolvidos (ex.: `Implementa 4 arquivos (feat, build, docs)`).
- **Sem prefixo `tipo:`** — o tipo Conventional Commits **não é impresso**; ele só seleciona o verbo (ex.: `Implementa`, `Corrige`, `Atualiza`).
- **Sem scope** — evita redundância com o nome do projeto.
- **Sem realce de cores** — usa `git diff --no-color` para evitar códigos ANSI.
- **Limite de 80 caracteres** na primeira linha (corta no último espaço e adiciona `…`).
- **Descrição e verbos no idioma ativo** (português-BR ou inglês).
- **Corpo sempre presente** — até 5 bullets de uma linha; havendo qualquer arquivo em stage, gera **ao menos um** bullet (mesmo com um único arquivo).

![Mensagem de commit gerada no diálogo de commit do GitExtensions](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotCommitMsg.png)

### Tipos detectados (definem o verbo)

Cada arquivo staged recebe um tipo. O **verbo** da primeira linha vem do tipo de **maior prioridade** entre todos os arquivos (ordem: `feat` → `fix` → `refactor` → `perf` → `test` → `build` → `ci` → `chore` → `docs` → `style`). **O tipo não é impresso** — só seleciona o verbo.

| Tipo       | Atribuído a um arquivo quando…                                                                    |
| ---------- | ------------------------------------------------------------------------------------------------- |
| `feat`     | arquivo de código **adicionado** (status `A`/`C`, categoria source/web)                           |
| `fix`      | arquivo de código **modificado/renomeado** (status `M`/`R`/`T`)                                   |
| `docs`     | arquivo de documentação (`.md`, `.txt`, `.rst`, `.adoc`)                                          |
| `test`     | caminho de teste (pasta `test`/`tests`/`spec` ou sufixo `Test`/`Spec`)                            |
| `chore`    | arquivo de configuração (`.json`, `.yml`, etc.) **ou** qualquer arquivo **deletado** (status `D`) |
| `build`    | arquivo de build (`.csproj`, `.sln`, `Dockerfile`, etc.)                                          |
| `refactor` | demais casos sem padrão claro                                                                     |

---

## Como a mensagem é gerada

### Estratégia 1 — Baseada em comentários do diff (principal)

Executa `git diff --cached --no-color` e coleta as linhas de **comentário** que foram **adicionadas** (`+`) ou **removidas** (`-`). As adicionadas têm prioridade pela **categoria do arquivo** (source = 4 > web = 3 > build = 2 / config = 1 / docs = 1; arquivos de teste = 0); as removidas entram com prioridade um grau menor. São usados até **5 comentários** (um por arquivo, do mais relevante ao menos). Dentro de um mesmo arquivo, em vez de pegar o comentário mais comprido, o plugin **pontua** os candidatos — premiando frase fechada, comprimento equilibrado (~20–72 chars) e início por verbo — e escolhe o de maior nota.

#### Padrões reconhecidos

| Sintaxe                       | Linguagens                           |
| ----------------------------- | ------------------------------------ |
| `// texto` ou `/// texto`      | C#, Java, JavaScript, TypeScript, Go |
| `/* texto */`, `/** texto */`  | Bloco C-style (C#, Java, JS, C/C++…) |
| `* texto`                      | Continuação de bloco JSDoc/Javadoc   |
| `<!-- texto -->`               | HTML, XML                            |
| `-- texto`                     | SQL, Lua, Haskell, Ada               |
| `' texto`                      | VB, VBScript                         |
| `# texto`                      | Python, Shell, YAML, Ruby            |

#### Comentários rejeitados

| Condição                                | Exemplo rejeitado              |
| --------------------------------------- | ------------------------------ |
| Separador visual                        | `// ─────────────────────`     |
| Tag XML de documentação                 | `/// <summary>`                |
| Código comentado (tem `{` `}`)          | `// if (x) { return; }`        |
| Código comentado (chamada de método)    | `// método(argumento)`         |
| Texto muito curto (< 10 chars)          | `// ok`                        |
| Sem espaço (não é frase)                | `// TODO`                      |
| **Delimitador desbalanceado**           | `// monta a árvore (recursivo` |
| **Termina em palavra de ligação solta** | `// mapeia o token para`       |

#### Como os comentários são usados

O comentário mais impactante vira a **descrição** na primeira linha (com o verbo inicial normalizado para 3ª pessoa/imperativo). Os demais aparecem no **corpo** como itens de lista:

```text
Valida o token antes de processar a requisição

- Filtra requisições sem cabeçalho de autenticação
```

> Se o comentário escolhido contém um conector de justificativa (`para`, `pois`, `porque`, …), a parte após o conector é descartada da primeira linha, e a descrição passa a usar a frase funcional dos nomes de arquivo (Estratégia 2) para evitar repetição com os bullets.

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
   - **Vocabulário de rejeição** (`RejectedVocabulary`) — se **qualquer** palavra do nome estiver nesse conjunto, o nome é rejeitado (precede tudo). Para nomes próprios/namespaces (ex.: `zimerfeld`, `git`, `extensions`). Estenda conforme o projeto.
   - Nome com 3+ palavras → rejeitado como namespace, **exceto** quando é um conceito do dicionário **ou** todas as suas palavras são vocabulário reconhecido (`KnownVocabulary` + dicionário de tradução + conceitos). Ex.: `New Text Document` **passa** (new/text/document conhecidos); `ZimerfeldCommitMsg` **não passa** (`zimerfeld` no vocabulário de rejeição). Estenda `KnownVocabulary` para cobrir mais termos.

#### Vocabulário por repositório (`.zimerfeldcommitmsg.json`)

É possível estender a extração de conceitos **sem recompilar**, colocando um arquivo
opcional `.zimerfeldcommitmsg.json` na raiz do repositório:

```json
{
  "knownVocabulary":    ["widget", "gadget"],
  "rejectedVocabulary": ["acme", "contoso"],
  "concepts":           { "widget": "componente", "gadget": "acessório" }
}
```

- **`knownVocabulary`** — palavras extras aceitas como parte de um nome descritivo (somadas ao `KnownVocabulary` embutido; vale para os dois idiomas).
- **`rejectedVocabulary`** — palavras que forçam a rejeição do nome como conceito (nomes próprios/namespaces do projeto; somadas ao `RejectedVocabulary` embutido).
- **`concepts`** — tradução palavra-de-conceito → frase pt-BR usada no prefixo nominal do título (tem prioridade sobre o dicionário embutido).

Arquivo ausente ou malformado é ignorado silenciosamente — nunca quebra a geração.

#### Mapeamento de conceito → frase em pt-BR (exemplos)

| Conceito extraído         | Frase gerada                              |
| ------------------------- | ----------------------------------------- |
| `Auth` / `Authentication` | autenticação                              |
| `User` / `Users`          | gerenciamento de usuários                 |
| `Token` / `Jwt`           | gerenciamento de token / autenticação JWT |
| `Payment`                 | processamento de pagamento                |
| `Order`                   | processamento de pedidos                  |
| `Notification`            | notificações                              |
| `Cache`                   | cache                                     |
| `Migration`               | migração de banco de dados                |
| `Report`                  | relatórios                                |
| `CommitMessage`           | mensagem de commit                        |

#### Verbos por tipo

O verbo é escolhido pelo tipo (e, em alguns casos, pelo contexto das mudanças):

| Tipo       | Verbo (pt-BR)         | Verbo (en)         | Condição                                                     |
| ---------- | --------------------- | ------------------ | ------------------------------------------------------------ |
| `feat`     | Implementa / Adiciona | Implement / Add    | `Implementa` quando só há adições; `Adiciona` caso contrário |
| `fix`      | Corrige               | Fix                | —                                                            |
| `refactor` | Refatora              | Refactor           | —                                                            |
| `docs`     | Documenta / Atualiza  | Document / Update  | `Documenta` quando há adições; `Atualiza` caso contrário     |
| `build`    | Configura             | Configure          | —                                                            |
| `chore`    | Remove / Configura    | Remove / Configure | `Remove` quando há deleções; `Configura` caso contrário      |
| `test`     | Adiciona              | Add                | —                                                            |
| `perf`     | Otimiza               | Optimize           | —                                                            |
| `ci`       | Configura             | Configure          | —                                                            |
| `style`    | Padroniza             | Standardize        | —                                                            |

> Se a descrição já começa com um verbo conhecido (ex.: o comentário `filtrar stems…`), ele é **normalizado** (pt-BR: 3ª pessoa do presente → `Filtra`; en: imperativo → `Filter`) em vez de prefixar um novo verbo do tipo.

#### Corpo da mensagem (body)

O corpo lista até **5 bullets** — **ao menos um, mesmo com um único arquivo** — cada um com uma frase de uma linha resumindo a mudança mais significativa de um arquivo (ordenados por relevância do arquivo). O verbo de cada bullet acompanha o status no git (adicionado/removido/renomeado/modificado):

```text
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```

> **Fallback de nome de arquivo:** quando o nome não rende um conceito legível (nomes com ponto, multi-palavra ou namespaces) e não há comentário no diff, o bullet recai no **próprio nome do arquivo** (ex.: `Remove New Text Document.txt`). Assim, **nenhum arquivo fica sem linha** — sempre há título **e** corpo.

---

## Exemplos de mensagens geradas

| Arquivos staged                                                                     | Mensagem gerada (pt-BR)                                                                                                 | Mensagem gerada (en)                                                               |
| ----------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| `AuthService.cs` adicionado                                                         | `Implementa autenticação`                                                                                               | `Implement authentication`                                                         |
| `PaymentService.cs` adicionado                                                      | `Implementa processamento de pagamento`                                                                                 | `Implement payment processing`                                                     |
| `UserService.cs` modificado                                                         | `Corrige gerenciamento de usuários`                                                                                     | `Fix user management`                                                              |
| `README.md` modificado                                                              | `Atualiza documentação`                                                                                                 | `Update documentation`                                                             |
| `UserService.cs` + `TokenService.cs` adicionados                                    | `Implementa gerenciamento de usuários`<br>`- Adiciona gerenciamento de usuários`<br>`- Adiciona gerenciamento de token` | `Implement user management`<br>`- Add user management`<br>`- Add token management` |
| `.cs` modificado com comentário `// Valida o token antes de processar a requisição` | `Valida o token antes de processar a requisição`                                                                        | _(comentário em pt passa intacto)_                                                 |

---

## Requisitos

- PowerShell 5.1 ou superior
- Permissão de **Administrador** para instalar/desinstalar

---

## Instalação

### Opção A — Gerenciador de Plugins do GitExtensions (recomendado)

No próprio GitExtensions, vá em **Plugins → Plugin Manager**, procure por
**GitExtensions.ZimerfeldCommitMsg** no feed do nuget.org e clique em instalar.
Reinicie o GitExtensions. Não requer PowerShell nem permissão de Administrador.

### Opção B — Via PowerShell

Execute o PowerShell **como Administrador**:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

![Saída do install.ps1 confirmando a instalação do plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotInstall.png)

### Opção C — Manual

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

![Saída do uninstall.ps1 confirmando a remoção do plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUninstall.png)

A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

---

## Build e versionamento

A cada execução do `build.ps1`, o script:

1. Lê a versão atual do `.nuspec`
2. Calcula a nova versão: incrementa o `build` em +1 → `major.minor.build`
3. Escreve a nova versão e data **primeiro nos docs**: os READMEs e o cofre Obsidian
4. Só então dá o _bump_ da versão no `.nuspec` e no `.csproj`
5. Compila em Release
6. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` _(requer Admin)_
7. Atualiza `tools\net9.0-windows\` com a DLL nova
8. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
9. Remove `.nupkg` de versões anteriores

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

![Saída do build.ps1 com incremento de versão e empacotamento](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotBuild.png)

### Deploy rápido (sem incrementar versão)

Para atualizar apenas a DLL durante desenvolvimento:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

![Saída do update-dll.ps1 atualizando apenas a DLL](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUpdate.png)

---

## Plugins integrados

### [GitExtensions.ZimerfeldTree](https://github.com/zimerfeld/GitExtensions.ZimerfeldTree)

Plugin para GitExtensions que exibe branches hierarquicamente em uma janela de árvore persistente e não-modal. Branches separados por `/` são mostrados como nós de pasta aninhados sob três seções fixas — LOCAL, REMOTES e tags. Por **zimerfeld**.

### [GitExtensions.ZimerfeldLFS](https://github.com/zimerfeld/GitExtensions.ZimerfeldLFS)

Plugin para GitExtensions que integra o **Git LFS** (Large File Storage) ao fluxo de trabalho, facilitando o versionamento de arquivos grandes. Por **zimerfeld**.

---

## Licença

[CC BY-NC-ND 4.0](LICENSE.txt)

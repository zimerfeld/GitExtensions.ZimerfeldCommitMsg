# GitExtensions.ZimerfeldCommitMsg

**Versão:** 1.0.41
**Atualizado em:** 2026-06-08

![Screenshot](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/Screenshot.png)

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit no formato **Conventional Commits v1.0.0**, analisando o conteúdo real das alterações staged. **Multilíngue**: gera a mensagem em **português-BR ou inglês**, detectado automaticamente pelo idioma do sistema operacional, com **override manual** nas configurações do plugin.

---

## Funcionalidades em alto nível

- **Geração automática** da mensagem de commit a partir do conteúdo real do diff staged (não apenas dos nomes de arquivo).
- **Conventional Commits v1.0.0** — detecta o(s) tipo(s) (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) e prefixa o verbo imperativo adequado.
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

```
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```

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
| `feat: Implementa autenticação` | `feat: Implement authentication` |
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

```
<tipo>: <descrição em pt-BR>

<corpo opcional>
```

- Sem scope — evita redundância com o nome do projeto
- Sem realce de cores — usa `git diff --no-color` para evitar códigos ANSI
- Descrição no idioma ativo (português-BR ou inglês)

Quando as mudanças envolvem mais de um tipo, todos aparecem na primeira linha separados por vírgula:

```
feat, chore: adicionar autenticação
```

### Tipos detectados (Conventional Commits)

| Tipo | Quando é usado |
|---|---|
| `feat` | Arquivos adicionados (novos) |
| `fix` | Apenas modificações em arquivos existentes |
| `docs` | Somente arquivos de documentação (`.md`, `.txt`, etc.) |
| `test` | Arquivos em pastas de teste ou com sufixo `Test`/`Spec` |
| `chore` | Somente arquivos de configuração (`.json`, `.yml`, etc.) |
| `build` | Somente arquivos de build (`.csproj`, `.sln`, `Dockerfile`, etc.) |
| `refactor` | Mix de adições e modificações sem padrão claro |

---

## Como a mensagem é gerada

### Estratégia 1 — Baseada em comentários do diff (principal)

Executa `git diff --cached --no-color` e extrai as linhas **adicionadas** (`+`) que são comentários explicativos.

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

O comentário mais impactante vira a **descrição** na primeira linha. Os demais aparecem no **corpo** como itens de lista:

```
fix: filtrar stems com ponto para evitar nomes de assembly

- ignorar conceitos com mais de 2 palavras PascalCase
- adicionar sufixo Generator aos SemanticSuffixes
```

Quando a saída é **português-BR**, comentários em inglês são traduzidos automaticamente antes de serem usados. Quando a saída é **inglês**, os comentários passam intactos (e comentários em português permanecem em português).

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

#### Verbos por tipo (exemplos em pt-BR)

| Tipo | Verbo (pt-BR) | Verbo (en) | Exemplo gerado |
|---|---|---|---|
| `feat` | Adiciona / Implementa | Add / Implement | `feat: Implementa gerenciamento de usuários` |
| `fix` | Corrige | Fix | `fix: Corrige processamento de pagamento` |
| `docs` | Documenta / Atualiza | Document / Update | `docs: Atualiza documentação` |
| `test` | Adiciona | Add | `test: Adiciona testes de integração` |
| `chore` | Configura / Remove | Configure / Remove | `chore: Configura configuração` |
| `build` | Configura | Configure | `build: Configura configuração de build` |
| `refactor` | Refatora | Refactor | `refactor: Refatora gerenciamento de usuários` |

#### Corpo da mensagem (body)

Quando há 2+ arquivos, o corpo lista até **5 bullets**, cada um com uma frase de uma linha resumindo a mudança mais significativa de um arquivo (ordenados por relevância do arquivo). O verbo de cada bullet acompanha o status no git (adicionado/removido/renomeado/modificado):

```
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```

---

## Exemplos de mensagens geradas

| Arquivos staged | Mensagem gerada (pt-BR) | Mensagem gerada (en) |
|---|---|---|
| `AuthService.cs` adicionado | `feat: Implementa autenticação` | `feat: Implement authentication` |
| `UserService.cs` modificado | `fix: Corrige gerenciamento de usuários` | `fix: Fix user management` |
| `README.md` modificado | `docs: Atualiza documentação` | `docs: Update documentation` |
| `appsettings.json` alterado | `chore: Configura configuração` | `chore: Configure configuration` |
| `UserService.cs` + `UserRepository.cs` adicionados | `feat: Implementa gerenciamento de usuários` + bullets | `feat: Implement user management` + bullets |
| `.cs` com comentário `// filtrar stems com ponto` staged | `fix: Filtra stems com ponto` | `fix: Filter stems with dot` |

---

## Requisitos

- PowerShell 5.1 ou superior
- Permissão de **Administrador** para instalar/desinstalar

---

## Instalação

### Opção A — Via PowerShell (recomendado)

Execute o PowerShell **como Administrador**:

Caso o arquivo tenha sido baixado para a pasta C:\Downloads

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

### Opção B — Manual

Para esse pacote, a instalação manual faça assim:

Caso o arquivo tenha sido baixado para a pasta C:\Downloads

Extraia o arquivo C:\Downloads\gitextensions.zimerfeldcommitmsg.1.0.0.nupkg.
Você pode renomear para .zip e abrir, ou extrair direto com um descompactador.

Dentro do pacote, pegue esta DLL:
tools\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll

Copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para:

```
C:\Program Files\GitExtensions\Plugins\
```

Reinicie o GitExtensions.

Feche e abra o GitExtensions novamente.
Se precisar de permissão de administrador para copiar em Program Files, abra o Explorer ou PowerShell como admin.

Observações importantes:

Esse pacote foi empacotado para net9.0-windows. Se sua versão do GitExtensions não estiver compatível com essa runtime, o plugin pode não carregar.
Depois de reiniciar, o plugin deve aparecer em Plugins e nas configurações em Settings -> Plugins -> ZimerfeldCommitMsg.

---

## Desinstalação

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

---

## Build e versionamento

A cada execução do `build.ps1`, o script:

1. Lê a versão atual do `.nuspec`
2. Incrementa o `build` em +1 → `major.minor.build`
3. Atualiza `.nuspec`, `.csproj` e `README.md` com a nova versão e data
4. Compila em Release
5. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*
6. Atualiza `tools\net9.0-windows\` com a DLL nova
7. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
8. Remove `.nupkg` de versões anteriores

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

### Deploy rápido (sem incrementar versão)

Para atualizar apenas a DLL durante desenvolvimento:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

---

## Plugins relacionados

### [GitExtensions.ZimerfeldTree](https://www.nuget.org/packages/GitExtensions.ZimerfeldTree/)

Plugin para GitExtensions que exibe branches hierarquicamente em uma janela de árvore persistente e não-modal. Branches separados por `/` são mostrados como nós de pasta aninhados sob três seções fixas — LOCAL, REMOTES e tags. Por **zimerfeld**.

---

## Licença

[MIT](LICENSE.txt)

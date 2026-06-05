# GitExtensions.ZimerfeldCommitMsg

**Versão:** 1.0.29
**Atualizado em:** 2026-06-05

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que gera automaticamente mensagens de commit no formato **Conventional Commits v1.0.0**, em **português-BR**, analisando o conteúdo real das alterações staged.

---

## Modos de integração

### Template no diálogo de commit
Selecione **"Zimerfeld Commit Msg"** no dropdown de templates da janela de commit. A mensagem é gerada e preenchida automaticamente no campo de texto.

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
- Descrição sempre em português-BR

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

Comentários em inglês são traduzidos automaticamente para português-BR antes de serem usados.

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

#### Verbos em pt-BR por tipo

| Tipo | Verbo | Exemplo gerado |
|---|---|---|
| `feat` | adicionar | `feat: adicionar gerenciamento de usuários` |
| `fix` | corrigir | `fix: corrigir processamento de pagamento` |
| `docs` | atualizar | `docs: atualizar documentação` |
| `test` | adicionar / atualizar | `test: adicionar testes de integração` |
| `chore` | atualizar / remover | `chore: atualizar configuração` |
| `build` | adicionar / atualizar | `build: atualizar configuração de build` |
| `refactor` | *(omitido — redundante)* | `refactor: gerenciamento de usuários` |

#### Corpo da mensagem (body)

Gerado quando há 2+ arquivos com camadas arquiteturais distintas:

```
Abrange autenticação e gerenciamento de token nas camadas de serviço, repositório e controlador.
```

---

## Exemplos de mensagens geradas

| Arquivos staged | Mensagem gerada |
|---|---|
| `AuthService.cs` adicionado | `feat: adicionar autenticação` |
| `UserService.cs` modificado | `fix: corrigir gerenciamento de usuários` |
| `README.md` modificado | `docs: atualizar documentação` |
| `appsettings.json` alterado | `chore: atualizar configuração` |
| `UserService.cs` + `UserRepository.cs` adicionados | `feat: adicionar gerenciamento de usuários` + corpo com camadas |
| `.cs` com comentário `// filtrar stems com ponto` staged | `fix: filtrar stems com ponto` |

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

### Opção B — Manual

Copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para:

```
C:\Program Files\GitExtensions\Plugins\
```

Reinicie o GitExtensions.

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

# GitExtensions.ZimerfeldCommitMsg — Funcionalidades

**Versão:** 1.0.16
**Atualizado em:** 2026-06-01

---

## Visão geral

Plugin para **GitExtensions** que gera automaticamente uma mensagem de commit no formato **Conventional Commits v1.0.0**, em **português-br**, analisando o conteúdo real das alterações staged.

---

## Modos de integração com GitExtensions

### Template no diálogo de commit
Selecione **"Zimerfeld: Auto-resumo"** no dropdown de templates da janela de commit. A mensagem é gerada e preenchida automaticamente no campo de texto.

### Menu Plugins
Acesse **Plugins → ZimerfeldCommitMsg**. O diálogo de commit abre com a mensagem já preenchida.

---

## Geração da mensagem de commit

### Formato gerado

```
<tipo>: <descrição em pt-BR>

<corpo opcional>
```

- Sem scope — evita redundância com o nome do projeto
- Sem realce de cores — usa `git diff --no-color` para evitar códigos ANSI
- Descrição sempre em português-br

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

## Estratégia 1 — Baseada em comentários do diff (principal)

Executa `git diff --cached --no-color` e extrai as linhas **adicionadas** (`+`) que são comentários explicativos.

### Padrões de comentário reconhecidos

| Sintaxe | Linguagens |
|---|---|
| `// texto` ou `/// texto` | C#, Java, JavaScript, TypeScript, Go |
| `# texto` | Python, Shell, YAML, Ruby |

### Filtros aplicados — comentários rejeitados

| Condição | Exemplo rejeitado |
|---|---|
| Separador visual | `// ─────────────────────` |
| Tag XML de documentação | `/// <summary>` |
| Código comentado (tem `{` `}`) | `// if (x) { return; }` |
| Código comentado (chamada de método) | `// método(argumento)` |
| Texto muito curto (< 10 chars) | `// ok` |
| Sem espaço (não é frase) | `// TODO` |

### Combinação dos comentários

Todos os comentários válidos encontrados (máx. 5) são combinados na descrição separados por `; `:

```
fix: filtrar stems com ponto para evitar nomes de assembly; ignorar conceitos com mais de 2 palavras PascalCase; adicionar sufixo Generator aos SemanticSuffixes
```

---

## Estratégia 2 — Baseada em nomes de arquivo (fallback)

Usada quando nenhum comentário válido é encontrado no diff.

### Extração de conceitos semânticos

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

### Mapeamento de conceito → frase em pt-BR (exemplos)

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

### Verbos em pt-BR por tipo

| Tipo | Verbo | Exemplo |
|---|---|---|
| `feat` | adicionar | `feat: adicionar gerenciamento de usuários` |
| `fix` | corrigir | `fix: corrigir processamento de pagamento` |
| `docs` | atualizar | `docs: atualizar documentação` |
| `test` | adicionar / atualizar | `test: adicionar testes de integração` |
| `chore` | atualizar / remover | `chore: atualizar configuração` |
| `build` | adicionar / atualizar | `build: atualizar configuração de build` |
| `refactor` | *(omitido — redundante)* | `refactor: gerenciamento de usuários` |

### Corpo da mensagem (body)

Gerado quando há 2+ arquivos com camadas arquiteturais distintas:

```
Abrange autenticação e gerenciamento de token nas camadas de serviço, repositório e controlador.
```

---

## Instalação e desinstalação

### Instalar

```powershell
cd C:\NUGET\ZimerfeldCommitMsg\tools
.\install.ps1
```

Ou copie manualmente:
```
GitExtensions.Plugins.ZimerfeldCommitMsg.dll  →  C:\Program Files\GitExtensions\Plugins\
```

### Desinstalar

```powershell
cd C:\NUGET\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

A remoção da DLL não afeta nenhuma outra parte do GitExtensions.

---

## Build e versionamento

A cada execução do `build.ps1`, o script:

1. Lê a versão atual do `.nuspec`
2. Incrementa o `build` em +1 → `major.minor.build`
3. Atualiza `.nuspec`, `.csproj` e `FUNCIONALIDADES.md` com a nova versão e data
4. Compila em Release
5. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*
6. Atualiza `tools\net9.0-windows\` com a DLL nova
7. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
8. Remove `.nupkg` de versões anteriores

```powershell
cd C:\NUGET\ZimerfeldCommitMsg
.\build.ps1
```

---

## Licença

[MIT](LICENSE.txt)

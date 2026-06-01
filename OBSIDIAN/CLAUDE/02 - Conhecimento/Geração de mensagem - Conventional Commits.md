---
tipo: conhecimento
criado: 2026-06-01
tags: [conhecimento, conventional-commits, git, algoritmo, pt-br]
---

# Geração de mensagem — Conventional Commits

> Lógica implementada em `CommitMessageGenerator.cs` (~985 linhas) do projeto [[GitExtensions.ZimerfeldCommitMsg]]. Spec completa em `FUNCIONALIDADES.md`.

## Formato gerado
```
<tipo>: <descrição em pt-BR>

<corpo opcional>
```
- **Sem scope** — evita redundância com o nome do projeto.
- **Sem cor** — usa `git diff --no-color` para não capturar códigos ANSI.
- Descrição **sempre em português-BR**.

## Tipos detectados
| Tipo | Quando |
|---|---|
| `feat` | Há arquivos **adicionados** (novos) |
| `fix` | Apenas **modificações** em arquivos existentes |
| `docs` | Só documentação (`.md`, `.txt`, …) |
| `test` | Pastas de teste ou sufixo `Test`/`Spec` |
| `chore` | Só configuração (`.json`, `.yml`, …) |
| `build` | Só arquivos de build (`.csproj`, `.sln`, `Dockerfile`, …) |
| `refactor` | Mix de adições e modificações sem padrão claro |

## Estratégia 1 — comentários do diff (principal)
1. Roda `git diff --cached --no-color`.
2. Extrai linhas **adicionadas** (`+`) que são **comentários** (`//`, `///`, `#`).
3. **Rejeita** ruído: separadores visuais (`// ─────`), tags XML (`/// <summary>`), código comentado (tem `{`/`}` ou chamada `metodo(arg)`), texto < 10 chars, sem espaço (`// TODO`).
4. Combina até **5** comentários válidos separados por `; `.

```
fix: filtrar stems com ponto para evitar nomes de assembly; ignorar conceitos com mais de 2 palavras PascalCase
```

## Estratégia 2 — nomes de arquivo (fallback)
Usada quando nenhum comentário válido é encontrado.
1. Remove prefixo de interface: `IUserService` → `UserService`.
2. Remove sufixo arquitetural (maior correspondência primeiro): `ServiceTests`, `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator`, `Middleware`, `Validator`, `Factory`, `Builder`, `Dto`, `ViewModel`, `Config`, `Adapter`, `Client`, `Impl`, …
3. Filtros de qualidade: stem com ponto → rejeitado (ex.: `GitExtensions.ZimerfeldCommitMsg`); > 2 palavras PascalCase sem entrada no dicionário → rejeitado (nome de projeto).
4. Mapeia conceito → frase pt-BR (`Auth`→autenticação, `User`→gerenciamento de usuários, `Payment`→processamento de pagamento, `CommitMessage`→mensagem de commit, …).

### Verbos por tipo
`feat`→adicionar · `fix`→corrigir · `docs`/`chore`/`build`→atualizar · `test`→adicionar/atualizar · `refactor`→*(verbo omitido, redundante)*.

### Corpo (body)
Gerado quando há 2+ arquivos com camadas arquiteturais distintas, ex.:
```
Abrange autenticação e gerenciamento de token nas camadas de serviço, repositório e controlador.
```

## 🔗 Relacionado
- [[GitExtensions.ZimerfeldCommitMsg]]
- [[Plugin MEF para GitExtensions]]

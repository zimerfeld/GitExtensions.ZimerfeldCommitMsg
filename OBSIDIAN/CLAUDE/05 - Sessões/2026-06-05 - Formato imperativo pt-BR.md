# 2026-06-05 — Formato imperativo pt-BR (verbo + objeto)

## Objetivo
Substituir o prefixo Conventional Commits (`feat:`, `fix:`, etc.) por um **verbo imperativo em português-BR** no início da mensagem, seguindo o padrão `<VERBO> <OBJETO> [DETALHE]`.

## Motivação
O padrão CC (feat/fix/docs…) é técnico e em inglês. O novo formato é mais profissional, legível e alinhado com equipes brasileiras que seguem a convenção descrita pelo usuário:

> "Uma boa mensagem de commit deve descrever o que foi feito, usando um verbo no imperativo, de forma objetiva e consistente."

## Formato anterior
```
feat: gerenciamento de usuários
fix, build: configuração de build
```

## Formato novo
```
Implementa gerenciamento de usuários
Corrige configuração de build
```

## Tabela de verbos por intenção

| Tipo CC interno | Condição                        | Verbo           |
|-----------------|---------------------------------|-----------------|
| feat            | só arquivos novos (A/C)         | **Implementa**  |
| feat            | mistura de novos + modificados  | **Adiciona**    |
| fix             | —                               | **Corrige**     |
| refactor        | —                               | **Refatora**    |
| docs            | há arquivos novos               | **Documenta**   |
| docs            | só modificações                 | **Atualiza**    |
| build           | —                               | **Configura**   |
| chore           | há deleções                     | **Remove**      |
| chore           | sem deleções                    | **Configura**   |
| test            | —                               | **Adiciona**    |
| perf            | —                               | **Otimiza**     |
| ci              | —                               | **Configura**   |
| style           | —                               | **Padroniza**   |
| (qualquer outro)| —                               | **Atualiza**    |

## Lógica de detecção de verbo em comentários

Quando a descrição vem de um comentário extraído do diff, o plugin verifica se ela já começa com um verbo em português:

1. **3ª pessoa presente** (ex: `filtra`, `retorna`, `corrige`) → capitaliza a primeira letra  
   `"filtra stems com ponto"` → `"Filtra stems com ponto"`

2. **Infinitivo mapeado** (ex: `filtrar`, `adicionar`, `corrigir`) → converte para 3ª pessoa  
   `"filtrar stems com ponto"` → `"Filtra stems com ponto"`

3. **Sem verbo reconhecido** (frase substantiva) → prefixa o verbo do tipo  
   `"gerenciamento de token"` → `"Adiciona gerenciamento de token"`

## Exemplos de saída

| Situação                                  | Antes                                 | Depois                                      |
|-------------------------------------------|---------------------------------------|---------------------------------------------|
| Novo arquivo `UserService.cs`             | `feat: gerenciamento de usuários`     | `Implementa gerenciamento de usuários`      |
| Modificação em `AuthService.cs`           | `fix: autenticação`                   | `Corrige autenticação`                      |
| README atualizado                         | `docs: Plugin GitExtensions…`         | `Atualiza plugin GitExtensions…`            |
| Comentário `// filtrar stems com ponto`  | `fix: filtrar stems com ponto`        | `Filtra stems com ponto`                    |
| Comentário `// retorna null quando…`      | `fix: retorna null quando…`           | `Retorna null quando…`                      |
| Múltiplos arquivos novos + corpo          | `feat: autenticação\n\nAbrange…`      | `Implementa autenticação\n\nAbrange…`       |

## Arquivos alterados
- `src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs`
  - Adicionado: `PtVerbs3rd` (HashSet — verbos PT 3ª pessoa)
  - Adicionado: `InfinitiveTo3rd` (Dictionary — infinitivo → 3ª pessoa)
  - Adicionado: `MapTypeToVerb()` — tipo CC + contexto → verbo imperativo PT
  - Adicionado: `FormatTitle()` — combina verbo + objeto
  - Adicionado: `ExtractLeadingVerb()` — detecta e extrai verbo inicial da descrição
  - Modificado: `Generate()` — usa `FormatTitle()` no lugar de `{typeStr}: {desc}`
  - Modificado: bullets do corpo também usam `FormatTitle()`
  - Simplificado: `BuildSubject()` (comentário desatualizado removido)

## Build
- **Build Release**: ✅ sucesso, 0 warnings, 0 errors
- Versão: 1.0.19 (sem incremento — aguardando `build.ps1`)

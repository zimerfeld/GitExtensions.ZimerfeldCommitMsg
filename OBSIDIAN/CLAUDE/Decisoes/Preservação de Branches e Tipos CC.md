---
tipo: decisão
tags: [decisão, tradução, branch, conventional-commits, preservação]
branch: feature/modelo
data: 2026-06-01
status: implementada
---

# Decisão: Preservar Nomes de Branch e Tipos CC na Tradução

## Contexto

A tradução de comentários do diff (en→pt-BR) em `TranslateToPortuguese` é **palavra-a-palavra** na Fase 3. Como o dicionário `WordTranslations` contém verbos comuns (`search`→buscar, `filter`→filtro, `update`→atualizar, `import`, etc.), um comentário em inglês que mencione um **nome de branch** seria corrompido:

```
"create feature/search service"  →  "criar feature/buscar service"   (ERRADO)
```

O mesmo risco existe para os **tipos Conventional Commits** caso futuramente entrem no dicionário.

## Decisão

**Mascarar nomes de branch (padrão gitflow) e tipos Conventional Commits antes de traduzir e restaurá-los intactos no fim.**

## Implementação

### `PreservePattern` (regex, ~L358 de `CommitMessageGenerator.cs`)

```csharp
private const string PreservePattern =
    @"\b(?:feature|release|hotfix|bugfix|support|feat|fix|chore|docs|refactor)/[A-Za-z0-9._\-/]+" + // branches gitflow
    @"|\b(?:feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)\b";                       // tipos CC
```

### Máscara → traduz → restaura (em `TranslateToPortuguese`)

```
1. Se !IsEnglishText(text) → retorna texto (já pt-BR), sem mascarar.

2. Mascara tokens preservados com placeholder  U+0001 N U+0001
   (delimitador de controle U+0001, sem letras → as 3 fases de tradução o ignoram).

3. Fases 1–3 de tradução rodam sobre o texto mascarado.

4. Avalia qualidade ANTES de restaurar:
   IsEnglishText(result) sobre o texto mascarado → se ainda inglês, descarta (null).
   (Tokens preservados NÃO contam como "inglês não traduzido".)

5. Restaura os tokens via regex delimitada  U+0001 (\d+) U+0001 .
   O delimitador evita capturar números comuns do texto (ex.: "3 arquivos").
```

## Pontos sutis

- **Quality check antes da restauração:** se a checagem rodasse após restaurar, um slug como `feature/search` (que contém a palavra inglesa "search") inflaria o percentual de inglês e poderia **descartar uma boa tradução**. Por isso `IsEnglishText` roda sobre o texto ainda mascarado.
- **Delimitador U+0001 (caractere de controle):** escolhido por não ter letras nem casar com `\b[A-Za-z][a-z]*\b` (Fase 3) nem com `\w+-based` (Fase 2). Placeholders adjacentes não fundem dígitos porque cada um é envolto pelos delimitadores.
- **Tipos CC hoje são no-op:** nenhum dos tipos (`feat`, `fix`, …) está em `WordTranslations`, então a proteção deles é um **safeguard** contra mudanças futuras no dicionário.

## Trade-offs

| Aspecto | Antes | Depois |
|---|---|---|
| `feature/search` em comentário inglês | virava `feature/buscar` | preservado intacto |
| Tipos CC no corpo | dependiam de não estarem no dicionário | sempre preservados |
| Complexidade | tradução direta | + máscara/restauração |

## Arquivos modificados

- `CommitMessageGenerator.cs` — `PreservePattern` adicionado (~L358); `TranslateToPortuguese` com máscara/restauração (~L632)

## Relacionado

- [[Estratégia de Detecção de Idioma]]
- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]

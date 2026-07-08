---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [fluxo, geração, diff, conventional-commits, etapa2]
---

# ⚙️ Fluxo: Geração da mensagem

O que o `CommitMessageGenerator.Generate()` faz para transformar o diff staged numa mensagem.

## 🪜 Passos

```
Generate()
        │
        ├─ 1. Lê o diff staged: `git diff --cached` no working dir
        │       (sem stage → devolve só a linha-resumo com prefixo de contexto)
        │
        ├─ 2. Classifica cada arquivo:
        │       extensão → categoria (source/web/docs/build/config)
        │       + heurística de conteúdo → tipo Conventional Commits
        │       (feat/fix/docs/test/chore/build/refactor)
        │
        ├─ 3. DetermineAllTypes(changes) → lista de tipos envolvidos (dominante primeiro)
        │
        ├─ 4. Subject consolidado: BuildConsolidatedTitle(tipoDominante, changes, tipos)
        │       = [prefixo de contexto] - <verbo do tipo> N arquivos (tipo1, tipo2, …)
        │       ex.: "Overlay - Adiciona 10 arquivos (feat, fix, docs)"
        │
        ├─ 5. Corpo: BuildBody(changes, comentáriosPorArquivo, tituloReadme)
        │       até 5 bullets, um por arquivo de maior impacto, cada um a melhor
        │       frase de comentário do diff (ou conceito do nome do arquivo)
        │
        └─ 6. Nunca vazio: havendo stage, sempre ao menos a linha-resumo
```

## 🔎 Detalhes

- **Verbo por tipo (Conventional Commits):** o tipo escolhe o **verbo**, não vira prefixo. `feat`→"Adiciona/Add", `fix`→"Corrige/Fix", `docs`→"Documenta/Document", etc. O tipo **não** aparece na mensagem (só entre parênteses no resumo, como pista). Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo]].
- **Prefixo de contexto:** 1-3 palavras derivadas do **conceito** do arquivo de maior impacto (ex.: `OverlayController` → `Overlay`), para o título nunca ser genérico. `null` quando nenhum arquivo rende conceito legível.
- **Estratégia principal (comentários):** `ExtractCommentsByFile()` lê as linhas `+`/`-` do diff e capta comentários (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). Cada arquivo escolhe o **melhor** comentário (traduzido para pt-BR quando aplicável, saneado, recortado na cláusula principal).
- **Estratégia fallback (nomes de arquivo):** quando não há comentário útil, deriva o **conceito** do nome removendo sufixos semânticos (`Service`, `Controller`, `Tests`, …) e traduzindo palavras.
- **Saneamento:** descarta comentários com **delimitadores desbalanceados** (`()`, `[]`, `{}`, aspas, `<>`) ou terminados em **palavra de ligação solta**; entre válidos, escolhe o de melhor qualidade (não o mais longo).
- **README:** se o `README.md` está staged, o título (primeira linha `#`) é usado como fallback descritivo.

Ver [[⚙️ Geração de mensagem a partir do diff]].

## 🌐 Exemplo (pt-BR / EN)

```
Implementa autenticação
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```
```
Implement authentication
- Add authentication
- Add payment processing
- Add token management
```

## 🔗 Ligações

- [[🔍 1 - Detecção do diálogo de commit]]
- [[🌐 3 - Idioma e auto-refresh]]
- [[⚙️ CommitMessageGenerator]]
- [[📜 Conventional Commits - Conceitos]]

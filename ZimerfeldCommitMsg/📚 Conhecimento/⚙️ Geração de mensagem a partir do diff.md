---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [conhecimento, diff, extração, comentários, saneamento, tradução]
---

# ⚙️ Geração de mensagem a partir do diff

## 📝 Resumo
Como o [[⚙️ CommitMessageGenerator]] transforma o `git diff --cached` numa mensagem de commit: classificação → subject consolidado → corpo em bullets, com extração de comentários, derivação de conceitos, saneamento e tradução.

## 🔄 Pipeline

```
git diff --cached
   │
   ▼
FileChange[]  (path, +/- linhas)
   │
   ├─ categoria por extensão (source/web/docs/build/config)
   ├─ tipo Conventional Commits (dominante primeiro)
   │
   ├─ SUBJECT: [prefixo de contexto] - <verbo> N arquivos (tipos)
   │      prefixo = conceito do arquivo de maior impacto (ex.: OverlayController → Overlay)
   │
   └─ CORPO: até 5 bullets, um por arquivo de maior impacto
          estratégia 1: melhor comentário do diff (traduzido, saneado, recortado)
          estratégia 2 (fallback): conceito do nome do arquivo
```

## 💬 Extração de comentários

`ExtractCommentText` reconhece:
- `//`, `///` — C#, Java, JS, TS, Go, Rust, C/C++…
- `/* */`, `/** */`, `/*! */` — bloco C-style em uma linha
- `* ` — continuação JSDoc/Javadoc (fora de `.md`, onde `*` é bullet)
- `<!-- -->` — HTML/XML
- `--` — SQL, Lua, Haskell, Ada
- `'` — VB/VBScript (fora de `.md`)
- `#` — Python, Shell, YAML, Ruby (em `.md` é heading, ignorado)

## 🧹 Saneamento (`CleanCommentText`)

Descarta:
- **Separadores visuais** e código comentado.
- **Delimitadores desbalanceados** — parênteses/colchetes/chaves não casados; aspas `"`/`` ` ``/`'` em número ímpar (ignora apóstrofes de contração como `don't`, `it's`); `<`/`>` em número desigual.
- Frases terminadas em **palavra de ligação solta** (`de`, `para`, `que`…) — comentário cortado.

Entre os candidatos válidos, escolhe o de **melhor qualidade** (mais descritivo), não o mais longo.

## 🌐 Tradução EN→PT

Quando a saída é pt-BR, frases e verbos são traduzidos por dicionários (mais longos primeiro), com **proteção** de slugs de branch gitflow (`feature/…`) e tipos CC via regex — para não corromper `feature/search` em `feature/buscar`. Em inglês, os comentários passam intactos.

## 🔡 Derivação de conceitos (fallback)

`SemanticSuffixes` remove sufixos como `Service`, `Controller`, `Repository`, `Tests`, `ViewModel`, … para chegar ao conceito do nome. Ex.: `OverlayController` → `Overlay`. Vocabulário conhecido/rejeitado e frases de conceito vêm dos defaults + [[📓 RepoVocabularyConfig]].

## ✅ Garantias

- **Nunca vazio:** havendo stage, sempre ao menos a linha-resumo.
- **Sempre ao menos um bullet**, mesmo com um único arquivo.
- **README staged:** o título (`#`) serve de fallback descritivo.

## 🔗 Relacionado
- [[📜 Conventional Commits - Conceitos]]
- [[⚙️ CommitMessageGenerator]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo]]
- [[⚙️ 2 - Geração da mensagem]]

---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [decisao, adr, extração, comentários, nomes-de-arquivo]
status: aceita
---

# 🔀 ADR — Duas estratégias: comentários do diff + nomes de arquivo

## 🎯 Contexto
Para descrever o **que** mudou, o gerador precisa de uma fonte de conteúdo. Geradores simples usam só nomes de arquivo, o que produz mensagens genéricas ("update Service"). Por outro lado, nem todo diff traz comentários úteis.

## ✅ Decisão
Usar **duas estratégias complementares**, com prioridade:
1. **Comentários do diff (principal):** ler as linhas `+`/`-` do `git diff --cached` e extrair comentários em várias sintaxes (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). O melhor comentário por arquivo é traduzido (quando pt-BR), saneado e recortado na cláusula principal.
2. **Nomes de arquivo (fallback):** quando não há comentário útil, derivar o **conceito** do nome removendo sufixos semânticos (`Service`, `Controller`, `Tests`, …) e traduzindo palavras.

Ambas se apoiam no **vocabulário por repositório** (`.zimerfeldcommitmsg.json`) para termos do domínio. Ver [[📓 Vocabulário por repositório]].

## 🔀 Alternativas consideradas
- **Só nomes de arquivo** — barato, mas genérico e pouco informativo.
- **Só comentários** — rico quando existem, mas falha em diffs sem comentários.
- **Duas estratégias com fallback (escolhida)** — cobre os dois casos; o conteúdo real do diff é privilegiado, com um piso derivado dos nomes.

## ⚖️ Consequências
**Positivas:**
- Mensagens mais informativas quando o dev comenta o código.
- Nunca fica sem conteúdo — sempre há ao menos a linha-resumo e um bullet.

**Negativas / trade-offs:**
- A extração de comentários exige **saneamento** robusto (delimitadores balanceados, ligação solta) para não injetar ruído.
- Tradução EN→PT é heurística (dicionários), não perfeita.

## 🔗 Relacionado
- [[📓 Vocabulário por repositório]]
- [[⚙️ Geração de mensagem a partir do diff]]
- [[⚙️ CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem]]

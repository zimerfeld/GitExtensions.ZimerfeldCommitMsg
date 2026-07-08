---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [decisao, adr, conventional-commits, mensagem, verbo]
status: aceita
---

# ✍️ ADR — Subject iniciado por verbo, sem o prefixo literal "tipo:"

## 🎯 Contexto
O padrão **Conventional Commits** define tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) normalmente escritos como prefixo literal: `feat: add authentication`. O plugin classifica as mudanças por esses tipos, mas precisa decidir **como** isso aparece na mensagem.

## ✅ Decisão
Usar o tipo Conventional Commits para **escolher o verbo**, e **não** para prefixar a mensagem com `tipo:`. O subject começa pelo **verbo** correspondente (3ª pessoa do presente em pt-BR / imperativo em inglês). O tipo em si só aparece, como pista, entre parênteses na linha-resumo consolidada.

Exemplos:
- `feat` → `Adiciona autenticação` / `Add authentication`
- `fix` → `Corrige …` / `Fix …`
- Resumo consolidado: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`

## 🔀 Alternativas consideradas
- **Prefixo literal `feat:`** — canônico do CC, mas ruidoso e menos natural em pt-BR; muitos times já geram esse prefixo via ferramentas/hooks.
- **Só verbo, sem tipos (escolhida)** — mensagem legível e natural, guiada pela mesma classificação semântica; os tipos ficam visíveis só no resumo entre parênteses.

## ⚖️ Consequências
**Positivas:**
- Mensagens naturais e bilíngues, sem poluição de prefixo.
- Ainda **guiadas** pela classificação Conventional Commits (o verbo carrega a intenção).

**Negativas / trade-offs:**
- Não é o formato literal CC — quem exige `type:` no subject precisa ajustar (o tipo aparece só no resumo).
- A qualidade depende do mapeamento tipo → verbo no `LanguagePack`.

## 🔗 Relacionado
- [[📜 Conventional Commits - Conceitos]]
- [[⚙️ CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem]]

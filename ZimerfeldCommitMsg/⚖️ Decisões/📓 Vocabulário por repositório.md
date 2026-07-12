---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [decisao, adr, vocabulário, configuração, json]
status: aceita
---

# 📓 ADR — Vocabulário por repositório (.zimerfeldcommitmsg.json)

> 🇺🇸 English → [[📓 Vocabulário por repositório (EN)]] · 🇪🇸 Español → [[📓 Vocabulário por repositório (ES)]]

## 🎯 Contexto
Cada projeto tem seu próprio jargão: nomes de domínio ("widget", "overlay") que deveriam virar conceitos, e nomes próprios/namespaces ("Acme", "Contoso") que **não** deveriam. Embutir todo esse vocabulário no plugin exigiria recompilar a cada projeto.

## ✅ Decisão
Ler um arquivo **opcional** `.zimerfeldcommitmsg.json` na raiz do working dir, com três listas somadas aos defaults embutidos:
- `knownVocabulary` — palavras aceitas como parte de um nome descritivo.
- `rejectedVocabulary` — palavras que forçam a rejeição do nome como conceito.
- `concepts` — tradução palavra-de-conceito → frase pt-BR (prioridade sobre o dicionário embutido).

Falhas de leitura/parse são **silenciosas** (config vazia) — nunca quebram a geração. Ver [[📓 RepoVocabularyConfig]].

## 🔀 Alternativas consideradas
- **Só vocabulário embutido** — não escala para o jargão de cada projeto; exigiria recompilar.
- **Setting global no GitExtensions** — não é por repositório; difícil versionar junto do código.
- **Arquivo por repo versionado (escolhida)** — viaja com o projeto, editável por qualquer membro do time, sem recompilar.

## ⚖️ Consequências
**Positivas:**
- Personalização por projeto sem recompilar; versionável junto do código.
- Robusto a JSON malformado (ignora e segue com os defaults).

**Negativas / trade-offs:**
- Mais um arquivo de convenção para o time conhecer.
- Sem validação/feedback explícito de erro (por design — silencioso).

## 🔗 Relacionado
- [[📓 RepoVocabularyConfig]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo]]
- [[⚙️ Geração de mensagem a partir do diff]]

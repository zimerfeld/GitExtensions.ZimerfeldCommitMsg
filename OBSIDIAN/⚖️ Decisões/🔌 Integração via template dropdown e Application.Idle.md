---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [decisao, adr, integração, application-idle, template, formcommit]
status: aceita
---

# 🔌 ADR — Integração via template dropdown + Application.Idle

> 🇺🇸 Read this page in English → [[🔌 Integração via template dropdown e Application.Idle (EN)]]

## 🎯 Contexto
O plugin precisa **preencher a caixa de mensagem** do diálogo de commit do GitExtensions e **manter a mensagem atualizada** enquanto o usuário mexe no stage. Porém a extensibilidade do host **não expõe** um evento de "diálogo de commit aberto" nem um callback de "item de template clicado".

## ✅ Decisão
Combinar dois mecanismos:
1. **Itens de template no dropdown de commit** (`CommitTemplateManager`) — um por idioma. Materializam a mensagem gerada e permitem a escolha rápida do idioma.
2. **`Application.Idle`** — detecta o `FormCommit` recém-aberto (varrendo `Application.OpenForms` por nome de tipo) e **preenche a caixa**, com gates por instância e working dir para não reprocessar a cada tick.

A escolha de um item do dropdown é reconhecida **indiretamente**: o host materializa o texto de **todos** os itens ao abrir o dropdown e, no clique, só aplica o texto. O plugin registra cada texto gerado (msg → idioma) e detecta pela caixa (`TextChanged`) qual foi escolhido, fixando o idioma da sessão.

## 🔀 Alternativas consideradas
- **Só template** — não cobre auto-refresh ao stage/unstage nem o preenchimento inicial confiável.
- **Assinar eventos do host** — não há evento adequado; dependeria de tipos internos frágeis entre versões.
- **Reflection + Application.Idle (escolhida)** — funciona com a API pública, tolerante a versões (casa por **nome** de tipo `FormCommit`).

## ⚖️ Consequências
**Positivas:**
- Preenchimento automático ao abrir e a cada mudança de stage, sem sobrescrever texto digitado (**não destrutivo**).
- Escolha de idioma por commit no próprio diálogo.

**Negativas / trade-offs:**
- `Application.Idle` dispara muito → exige gates cuidadosos (instância + working dir) para não reprocessar.
- Detecção de clique do dropdown é indireta (via `TextChanged`) — mais sutil que um callback.
- Localizar a caixa de mensagem depende de reflection sobre o `FormCommit`.

## 🔗 Relacionado
- [[🔍 1 - Detecção do diálogo de commit]]
- [[🌐 3 - Idioma e auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin]]
- [[🏗️ Arquitetura]]

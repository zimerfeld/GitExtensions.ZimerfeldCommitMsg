---
tipo: backlog
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-07
---

# 📌 Backlog

> 🇺🇸 Read this page in English → [[📌 Backlog (EN)]]

> [!tip] Comece por aqui
> Ponto de retomada do projeto. Ao voltar em outra sessão, leia a [[🏠 Home]] e este backlog.

## 📍 Estado atual
- **Versão:** `1.0.94` (`major.minor.BUILD`, build auto-incrementado pelo `build.ps1`).
- **Testes:** suíte **xUnit** em `tests\GitExtensions.ZimerfeldCommitMsg.Tests\` — extração de comentários, derivação de conceitos, `.zimerfeldcommitmsg.json`, tradução EN→PT.
- **Cofre:** reestruturado para o padrão **"Cofre de Neurônios v2"** (emoji + frontmatter §3 + pares EN + `sortspec` por prioridade).

## ✅ Próximos passos

### 🔧 Manutenção do build (derivado de [[🛠️ build.ps1]])
- [ ] **Atualizar a seção 2b do `build.ps1`** para os novos caminhos do cofre v2 — as 4 notas carimbadas mudaram de pasta/nome:
  - `💼 Negócio\📦 GitExtensions.ZimerfeldCommitMsg.md`
  - `📚 Conhecimento\📖 README — Instalação, Uso e Build.md`
  - `🧩 Sistemas\🏷️ Versionamento.md`
  - `🧩 Sistemas\🔭 Visão Geral.md`
- [ ] Considerar carimbar também os **pares `(EN)`** dessas notas na seção 2b.

### 🧪 Qualidade
- [ ] Manter a suíte xUnit verde a cada mudança; adicionar casos conforme novas sintaxes de comentário/tradução surgirem.

## 🧊 Ideias / mais tarde
- [ ] (nada pendente registrado — capturar em [[📥 Inbox]] quando surgir)

## ✅ Feito recente
- [x] **Fix na landing page — quebra de linha de títulos/subtítulos em PT** — 2026-07-07: a landing page (`index.html`, publicada em **commitmsg.zimerfeld.com** via GitHub Pages) compartilha um template i18n com a regra `html[data-lang="pt"] .lang-pt{display:inline}`, que tornava **todo** elemento em português `inline` — incluindo `h2`/`h3` — fazendo título/subtítulo colarem no texto seguinte quando o site abre em PT (em EN funcionava, pois `h2`/`h3` já são `block` por padrão). **Correção de 1 linha de CSS:** `html[data-lang="pt"] h2.lang-pt,html[data-lang="pt"] h3.lang-pt{display:block}` — restaura a quebra apenas nos títulos/subtítulos em PT, sem afetar o EN. Publicado via GitFlow como **hotfix** (`hotfix/pt-heading-break` → `main`, com back-merge no `develop`, que estava atrás do `main`) + tag **`202607071915pt-heading-break`**; `CNAME` preservado; deploy confirmado ao vivo.

## 🔗 Ligações
- [[🏠 Home]]
- [[🔑 Fatos-Chave]]
- [[🛠️ build.ps1]]

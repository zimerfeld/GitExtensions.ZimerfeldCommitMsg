---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [conhecimento, git, conventional-commits, mensagem-de-commit]
---

# 📜 Conventional Commits — Conceitos

> 🇺🇸 Read this page in English → [[📜 Conventional Commits - Conceitos (EN)]]

## 📝 Resumo
**Conventional Commits** é uma convenção para mensagens de commit que classifica cada mudança por um **tipo** semântico. O formato canônico é `<tipo>[escopo opcional]: <descrição>`, com corpo e rodapé opcionais. Ver https://www.conventionalcommits.org/en/v1.0.0/.

## 🏷️ Tipos usados pelo plugin

| Tipo | Significado | Verbo (pt / en) |
|---|---|---|
| `feat` | nova funcionalidade | Adiciona / Add |
| `fix` | correção de bug | Corrige / Fix |
| `docs` | documentação | Documenta / Document |
| `test` | testes | Testa / Test |
| `chore` | tarefas de manutenção | Atualiza / Update |
| `build` | build/dependências | Compila / Build |
| `refactor` | refatoração sem mudança de comportamento | Refatora / Refactor |

## ⚙️ Como o plugin aplica

> [!important] Verbo, não prefixo literal
> O plugin usa o tipo para **escolher o verbo**, **não** para escrever `feat:`. O subject começa pelo verbo; os tipos envolvidos aparecem só entre parênteses na linha-resumo. Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo]].

Classificação em duas dimensões:
- **Categoria por extensão** (`ExtCategory`): `source`, `web`, `docs`, `build`, `config` — usada para priorizar arquivos e inferir tipo.
- **Tipo Conventional Commits**: heurística sobre categoria + conteúdo do diff (ex.: arquivos de teste → `test`; `.md` → `docs`; `.csproj`/`.sln` → `build`).

## 🧾 Formato da mensagem gerada

```
[<contexto>] - <verbo> N arquivos (tipo1, tipo2, …)   ← subject consolidado
                                                       ← linha em branco
- <frase resumo do arquivo 1>                          ← corpo (até 5 bullets)
- <frase resumo do arquivo 2>
```

Exemplo:
```
Overlay - Adiciona 10 arquivos (feat, fix, docs)

- Adiciona autenticação
- Corrige cálculo de saldo
- Documenta instalação
```

## 📍 Onde isto aparece no plugin
O [[⚙️ CommitMessageGenerator]] faz a classificação e o `LanguagePack` mapeia tipo → verbo por idioma. Ver [[⚙️ Geração de mensagem a partir do diff]].

## 🔗 Relacionado
- [[📦 GitExtensions.ZimerfeldCommitMsg]]
- [[⚙️ Geração de mensagem a partir do diff]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo]]

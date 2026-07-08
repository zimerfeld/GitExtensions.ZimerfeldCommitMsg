---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [sistema, overview, plugin, gitextensions, commit-message, conventional-commits, i18n]
---

# 🔭 Visão Geral

## 🎯 O que é

Plugin para **GitExtensions** (Windows) que **gera a mensagem de commit automaticamente** a partir do conteúdo real das alterações em stage. Ele se integra ao **diálogo de commit** do host: ao abrir o diálogo (e sempre que arquivos entram/saem do stage), o plugin lê o `git diff --cached`, classifica as mudanças por **Conventional Commits** e materializa um **subject iniciado por verbo** + **corpo em bullets**, no idioma escolhido. Ver [[📜 Conventional Commits - Conceitos]] e [[✍️ Subject iniciado por verbo sem prefixo de tipo]].

## 🧱 Stack

| Item | Valor |
|---|---|
| Linguagem | C# (.NET 9) |
| Target | `net9.0-windows` |
| UI Framework | Windows Forms (`UseWindowsForms`) — sem janela própria; integra ao diálogo de commit |
| Tipo de saída | `Library` (DLL carregada pelo GitExtensions) |
| Assembly de saída | `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` |
| Namespace | `GitExtensions.ZimerfeldCommitMsg` |
| Versão atual | `1.0.97` |
| Idiomas | Português-BR / Inglês (auto pelo SO + override) |
| Licença | CC BY-NC-ND 4.0 © 2026 Zimerfeld |
| Autor | Zimerfeld |

> O plugin exibe um **ícone** (PNG embutido em `Resources/icon.png`) no menu Plugins e no dropdown de templates do diálogo de commit.

## 🔌 Os três modos de integração

### 1️⃣ Template no dropdown do diálogo de commit
O plugin registra **três itens de template** (um por idioma) via `CommitTemplateManager`. Escolher um item aplica a mensagem gerada naquele idioma e fixa o idioma para o auto-refresh da sessão. Ver [[🌐 3 - Idioma e auto-refresh]].

### 2️⃣ Menu Plugins → ZimerfeldCommitMsg
Dispara `Execute` e gera a mensagem para o repositório atual, usando o idioma do **setting** (Automático/PT/EN).

### 3️⃣ Auto-preenchimento (Application.Idle)
Como não há evento de "diálogo de commit aberto", o plugin observa o **`Application.Idle`**, detecta o `FormCommit` recém-aberto e **preenche a caixa de mensagem** — repetindo ao stage/unstage. **Nunca sobrescreve** texto digitado manualmente. Ver [[🔍 1 - Detecção do diálogo de commit]].

## ⚙️ Como a mensagem é gerada

O [[⚙️ CommitMessageGenerator]] lê o `git diff --cached` e produz:
- **Subject consolidado:** `[<contexto>] - <verbo> N arquivos (tipos)` — ex.: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`.
- **Corpo em bullets:** até 5 frases, cada uma resumindo a mudança mais significativa de um arquivo, extraída dos **comentários** do diff (estratégia principal) ou derivada dos **nomes de arquivo** (fallback).

Ver [[⚙️ Geração de mensagem a partir do diff]].

## 🧰 Recursos transversais

| Recurso | Detalhe |
|---|---|
| Duas estratégias | comentários do diff (principal) + nomes de arquivo (fallback) — [[🔀 Duas estratégias - comentários e nomes de arquivo]] |
| Vocabulário por repo | `.zimerfeldcommitmsg.json` estende vocabulário/conceitos — [[📓 RepoVocabularyConfig]] |
| Tradução EN→PT | comentários traduzidos quando a saída é pt-BR; intactos em inglês |
| Saneamento | descarta comentários com delimitadores desbalanceados ou ligação solta no fim |
| Localização | strings de UI embutidas (`Strings.resx`/`StringsPtBr.resx`) + idioma da mensagem |
| Não destrutivo | jamais sobrescreve o que o usuário já digitou |

## ✅ Requisitos de runtime

- **GitExtensions 6.x** (.NET 9)
- **`git`** no `PATH` (o gerador roda `git diff --cached`)

## 🔗 Ligações

- [[🏗️ Arquitetura]]
- [[🏷️ Versionamento]]
- [[🔗 Dependências]]
- [[📜 Conventional Commits - Conceitos]]

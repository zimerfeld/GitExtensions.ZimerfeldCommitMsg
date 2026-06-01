---
tipo: sistema
tags: [sistema, overview, plugin, gitextensions]
atualizado: 2026-05-22
---

# Visão Geral

## O que é

Plugin para **GitExtensions** (Windows) que gera automaticamente uma mensagem de commit no formato **Conventional Commits v1.0.0**, em **português-BR**, analisando as alterações staged reais.

## Stack

| Item | Valor |
|---|---|
| Linguagem | C# 13 (.NET 9) |
| Target | `net9.0-windows` |
| UI Framework | Windows Forms (herdado do GitExtensions) |
| Assembly de saída | `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` |
| Namespace | `GitExtensions.ZimerfeldCommitMsg` |
| Versão atual | `1.0.13` |
| Autor | Zimerfeld |

## Modos de uso

### 1. Template no diálogo de commit
Selecione **"Zimerfeld: Auto-resumo"** no dropdown de templates → mensagem preenchida automaticamente.

### 2. Menu Plugins
`Plugins → ZimerfeldCommitMsg` → abre o `FormCommit` com a mensagem já inserida.

### 3. Auto-refresh por evento
Sempre que arquivos entram ou saem do stage, o plugin detecta e atualiza o campo de mensagem no `FormCommit` aberto (via `PostRepositoryChanged`).

## Tipos Conventional Commits detectados

| Tipo | Critério |
|---|---|
| `feat` | Arquivos fonte adicionados |
| `fix` | Apenas modificações em fontes existentes |
| `docs` | Somente `.md`, `.txt`, `.rst`, `.adoc` |
| `test` | Pastas `test/tests/spec` ou sufixo `Test/Spec` |
| `chore` | Somente configs (`.json`, `.yml`, `.env`, etc.) |
| `build` | Somente build files (`.csproj`, `.sln`, `Dockerfile`) |
| `refactor` | Mix sem padrão dominante |

> **Novidade branch `feature/titulo` (2026-05-22):** o título passa a listar **todos** os types envolvidos separados por vírgula (ex: `feat, docs, chore`). Ver [[../Decisoes/Título como Lista de Types]].

## Relacionado

- [[Arquitetura]]
- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]

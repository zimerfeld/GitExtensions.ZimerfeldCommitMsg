---
tipo: sistema
tags: [sistema, overview, plugin, gitextensions, i18n]
atualizado: 2026-06-05
---

# Visão Geral

## O que é

Plugin para **GitExtensions** (Windows) que gera automaticamente uma mensagem de commit no formato **Conventional Commits v1.0.0**, analisando as alterações staged reais. **Multilíngue**: gera em **português-BR ou inglês**, detectado pelo idioma do SO, com **override manual** no seletor "Idioma da mensagem / Message language". Ver [[../Decisoes/Suporte Multilíngue PT-EN]].

## Stack

| Item | Valor |
|---|---|
| Linguagem | C# 13 (.NET 9) |
| Target | `net9.0-windows` |
| UI Framework | Windows Forms (herdado do GitExtensions) |
| Assembly de saída | `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` |
| Namespace | `GitExtensions.ZimerfeldCommitMsg` |
| Versão atual | `1.0.35` |
| Idiomas | Português-BR / Inglês (auto pelo SO + override) |
| Autor | Zimerfeld |

> O plugin exibe um **ícone** (PNG embutido em `Resources/icon.png`) tanto no menu Plugins quanto na entrada do dropdown de templates. Carregado por `LoadIcon()` em [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]].

## Modos de uso (três portas de entrada)

### 1. Template no diálogo de commit
Selecione **"Zimerfeld: Auto-resumo"** no dropdown de templates → mensagem preenchida automaticamente. Passos detalhados em [[../Fluxos/Template Dropdown (Auto-resumo)]].

### 2. Menu Plugins
`Plugins → ZimerfeldCommitMsg` → valida o repositório e abre o `FormCommit` com a mensagem já inserida (`Execute` → `StartCommitDialog`). Exibe `MessageBox` se não houver repo válido ou mudanças staged.

### 3. Auto-refresh por evento
Sempre que arquivos entram ou saem do stage, o plugin detecta e atualiza o campo de mensagem no `FormCommit` aberto (via `PostRepositoryChanged`) — **sem sobrescrever texto digitado à mão**. Ver [[../Fluxos/Stage Trigger]].

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
- [[../Fluxos/Template Dropdown (Auto-resumo)]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[../Decisoes/Suporte Multilíngue PT-EN]]
- [[../Decisoes/Preservação de Branches e Tipos CC]]

---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [conhecimento, readme, instalacao, build, uso, conventional-commits, i18n]
fonte: README.md
versao: 1.0.97
---

# 📖 README — Instalação, Uso e Build

> 🇺🇸 English → [[📖 README — Instalação, Uso e Build (EN)]] · 🇪🇸 Español → [[📖 README — Instalação, Uso e Build (ES)]]

> Espelho do `README.md` da raiz do repositório (bilíngue EN/PT), reconciliado com o código em 2026-07-04.
> Nota de projeto: [[📦 GitExtensions.ZimerfeldCommitMsg]]. Conceitos em [[📜 Conventional Commits - Conceitos]].
> O `build.ps1` carimba versão + data nos READMEs **e** nas notas do cofre que espelham a versão atual — ver [[🏷️ Versionamento]].

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que **gera mensagens de commit automaticamente** analisando o conteúdo real das alterações em stage. As mudanças são classificadas pelos tipos do **Conventional Commits** para escolher o **verbo**; a mensagem é um **subject iniciado por verbo** (sem o prefixo `tipo:`) mais um **corpo em bullets**. Saída **bilíngue** (pt-BR / inglês), detectada pelo SO com override manual.

## ✨ Funcionalidades em alto nível
- **Geração automática** a partir do conteúdo do diff staged — não só dos nomes de arquivo.
- **Verbo guiado por Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) — o tipo **não** aparece na mensagem. Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo]].
- **Duas estratégias** — comentários do diff (principal) + nomes de arquivo (fallback). Ver [[🔀 Duas estratégias - comentários e nomes de arquivo]].
- **Vocabulário por repositório** — `.zimerfeldcommitmsg.json` estende vocabulário/conceitos sem recompilar. Ver [[📓 RepoVocabularyConfig]].
- **Multilíngue (PT-BR / EN)** — automático pelo SO + override (dropdown de 3 itens e setting).
- **Auto-preenchimento** ao abrir o diálogo e ao stage/unstage; **não destrutivo**.

## 🧩 Como funciona
Ao abrir o diálogo de commit, o plugin lê o `git diff --cached`, classifica as mudanças e preenche a caixa de mensagem. Detalhes em [[⚙️ Geração de mensagem a partir do diff]] e [[🔍 1 - Detecção do diálogo de commit]].

## 🗣️ Idioma
Duas formas de escolher (rótulos bilíngues):
1. **Dropdown de templates** do diálogo de commit — três itens (Automático / Português / Inglês), escolha por commit.
2. **Configurações → Plugins → ZimerfeldCommitMsg** — seletor "Idioma da mensagem / Message language" (padrão do auto-refresh e do menu Plugins).

| Opção | Comportamento |
|---|---|
| `Automático/Automatic` | **Padrão.** Detecta pelo SO (`pt-*` → português; outro → inglês) |
| `Português/Portuguese` | Força pt-BR |
| `Inglês/English` | Força inglês |

Exemplo lado a lado:
| Português-BR | English |
|---|---|
| `Implementa autenticação` | `Implement authentication` |
| `- Adiciona autenticação` | `- Add authentication` |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |

## 📦 Instalação
**Via Plugin Manager do GitExtensions:** procure por *ZimerfeldCommitMsg* (Plugins → Plugin Manager), instale e reinicie.

**Manual:** rode `build.ps1` (como Administrador para deploy automático), ou copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para `C:\Program Files\GitExtensions\Plugins\`, ou rode `tools\install.ps1` como Administrador.

> O nó **ZimerfeldCommitMsg** só aparece em **Configurações → Plugins** depois que a DLL com o seletor de idioma é instalada e o GitExtensions é reiniciado.

## ✅ Requisitos
- GitExtensions 6.x (.NET 9)
- `git` no `PATH` (o gerador roda `git diff --cached`)

## 🛠️ Build
```powershell
pwsh .\build.ps1          # incrementa versão, build Release, empacota o .nupkg
pwsh .\build.ps1 -Force   # sempre recompila/empacota
```
Ver [[🏷️ Versionamento]] e [[🛠️ build.ps1]].

## 💜 Apoie o projeto
**GitHub Sponsors:** [github.com/sponsors/zimerfeld](https://github.com/sponsors/zimerfeld) · **Ko-fi:** [ko-fi.com/C0D621FCGD](https://ko-fi.com/C0D621FCGD). Badges no topo do README (versão + downloads do NuGet).

## 📄 Licença
Copyright © 2026 Zimerfeld — **CC BY-NC-ND 4.0** (`LICENSE.txt`).

## 🔗 Relacionado
- [[📦 GitExtensions.ZimerfeldCommitMsg]]
- [[📜 Conventional Commits - Conceitos]]
- [[⚙️ Geração de mensagem a partir do diff]]
- [[🔑 Fatos-Chave]]

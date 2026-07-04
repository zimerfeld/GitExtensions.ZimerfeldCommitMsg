---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [fatos-chave, referencia, meta]
---

# 🔑 Fatos-Chave

> 🇺🇸 Read this page in English → [[🔑 Fatos-Chave (EN)]]

> [!tip] Leia primeiro
> Verdades estáveis e sempre úteis. Atualize quando algo mudar.

## 👤 Usuário
- Nome: **Renato Zimerfeld**
- Email: renato.zimerfeld@gmail.com
- Idioma de trabalho: **Português (BR)**
- Ver [[👤 Renato]] para preferências detalhadas

## 📁 Caminhos importantes
- Cofre de memória (este): `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\OBSIDIAN`
- Raiz do projeto: `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg`
- `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg` **é** o repositório do projeto [[📦 GitExtensions.ZimerfeldCommitMsg]] (C#, plugin GitExtensions que gera mensagens de commit)
- GitExtensions instalado em: `C:\Program Files\GitExtensions\`
- Plugins do GitExtensions: `C:\Program Files\GitExtensions\Plugins\`
- Settings do GitExtensions (fonte do working dir): `%APPDATA%\GitExtensions\GitExtensions\GitExtensions.settings`
- Config opcional por repositório: `.zimerfeldcommitmsg.json` na raiz do diretório de trabalho — ver [[📓 RepoVocabularyConfig]]
- Assemblies de referência do host: versionados em `refs\` (build determinístico, offline)
- Projetos irmãos: `GitExtensions.ZimerfeldLFS` (Git LFS) e `GitExtensions.ZimerfeldTree` (árvore de branches), cada um com seu próprio cofre

## 🛠️ Ferramentas ativas
- **RTK (Rust Token Killer)** — proxy CLI que economiza tokens. Ver [[🦀 RTK]]
- **Obsidian** — este cofre de memória
- **Claude Code** no Windows (shell: PowerShell / git bash)

## 📐 Convenções
- Datas: `AAAA-MM-DD`
- **Este projeto** é um **plugin MEF de GitExtensions** que **gera mensagens de commit** a partir do conteúdo real do diff em stage, classificando as mudanças por **Conventional Commits** para escolher o **verbo** — o subject começa por verbo, **sem** o prefixo literal `tipo:`. Ver [[📦 GitExtensions.ZimerfeldCommitMsg]] e [[📜 Conventional Commits - Conceitos]]
- Saída **bilíngue** (pt-BR / inglês): automática pelo SO + override manual (setting e dropdown de templates)
- Requisito de runtime: **GitExtensions 6.x (.NET 9)** — o plugin roda `git diff --cached` para ler o stage
- Versionamento `major.minor.BUILD`, build incrementado automaticamente pelo `build.ps1`
- Licença: **CC BY-NC-ND 4.0 © 2026 Zimerfeld**
- Commits co-authored quando solicitado push
- Não fazer commit/push sem pedido explícito

## 🔗 Relacionado
- [[🏠 Home]]
- [[📌 Backlog]]
- [[🧭 Como usar este cofre]]

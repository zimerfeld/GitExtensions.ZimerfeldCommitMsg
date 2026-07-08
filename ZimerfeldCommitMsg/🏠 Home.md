---
tipo: moc
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-05
---

# 🏠 GitExtensions.ZimerfeldCommitMsg — Cofre de Neurônios

> [!abstract] 🧠 O que é este cofre
> Memória persistente do Claude para o projeto **GitExtensions.ZimerfeldCommitMsg** — um plugin MEF para o GitExtensions que gera mensagens de commit automaticamente a partir do diff em stage. Este cofre é mantido a cada sessão: notas com par bilíngue (PT/EN), frontmatter padronizado e ordenação por prioridade.

## ⚡ Resumo executivo
- **O que é:** extensão (plugin MEF) para o **GitExtensions** que se integra ao **diálogo de commit** e **preenche a mensagem automaticamente** analisando o `git diff --cached`.
- **Problema que resolve:** escrever boas mensagens de commit é chato e inconsistente. O plugin lê o que **de fato** mudou (comentários adicionados no diff + nomes de arquivo), classifica por Conventional Commits e materializa um subject + corpo prontos — em pt-BR ou inglês.
- **Diferenciais:** analisa o **conteúdo do diff**, não só nomes de arquivo; **verbo guiado por Conventional Commits** sem poluir a mensagem com `tipo:`; **duas estratégias** (comentários + nomes de arquivo); **vocabulário por repositório** (`.zimerfeldcommitmsg.json`) sem recompilar; **auto-refresh** ao stage/unstage; **não destrutivo**; **i18n** (Automático / PT-BR / EN).
- **Stack:** C# / WinForms `Library`, alvo **net9.0-windows**, empacotado como **nupkg**; build e versionamento via `build.ps1`.
- **Estado atual:** versão **`1.0.97`** — funcional, com **suíte de testes xUnit**.
- **Público-alvo:** desenvolvedores e times que usam GitExtensions no Windows e querem mensagens de commit consistentes com pouco esforço.
- **Ângulo de negócio/portfólio:** produto **open source** sob o owner `zimerfeld`, ao lado dos irmãos `GitExtensions.ZimerfeldLFS` e `GitExtensions.ZimerfeldTree`.

## 🧭 Navegação por prioridade

### 1️⃣ 🔑 Impacto — Arquivos-Chave
> Arquivos que, se manipulados, têm grande impacto no sistema.
- [[🔌 ZimerfeldCommitMsgPlugin]] — entry point MEF + integração com o diálogo de commit
- [[⚙️ CommitMessageGenerator]] — o motor: diff → tipos CC → verbo → subject + corpo
- [[📓 RepoVocabularyConfig]] — vocabulário extra por repositório (`.zimerfeldcommitmsg.json`)
- [[🌐 Localization]] — idioma da mensagem + strings de UI
- [[🛠️ build.ps1]] — script de build, versionamento e deploy

### 2️⃣ 🧩 Reutilização — Sistemas
> Subsistemas reutilizados por várias partes do projeto.
- [[🔭 Visão Geral]] — o que o plugin faz, stack, versão atual
- [[🏗️ Arquitetura]] — Plugin (integração com o host) → Generator → Localization
- [[🏷️ Versionamento]] — ciclo build.ps1 / nuspec / csproj
- [[🔗 Dependências]] — assemblies do GitExtensions + Conventional Commits

### 3️⃣ 🔀 Uso — Fluxos
> Fluxos de uso passo a passo.
- [[🔍 1 - Detecção do diálogo de commit]] — Application.Idle detecta o FormCommit e preenche
- [[⚙️ 2 - Geração da mensagem]] — diff → tipos CC → verbo → subject + corpo
- [[🌐 3 - Idioma e auto-refresh]] — dropdown de 3 idiomas, setting, refresh ao stage/unstage

## 🚀 Operação
- [[💻 Ambiente Local (Dev)]] — `.\build.ps1 -Force` (compila + instala no GitExtensions local)
- [[🚀 Deploy em Produção (Prod)]] — `.\build.ps1` → `.nupkg` na raiz → NuGet + release GitHub

## ⚖️ Decisões (ADRs)
- [[🔌 Integração via template dropdown e Application.Idle]] — como o plugin entra no diálogo de commit
- [[✍️ Subject iniciado por verbo sem prefixo de tipo]] — Conventional Commits sem o literal "tipo:"
- [[🔀 Duas estratégias - comentários e nomes de arquivo]] — de onde vem o conteúdo
- [[📓 Vocabulário por repositório]] — `.zimerfeldcommitmsg.json` sem recompilar
- [[📦 Strings embutidas sem satellite assemblies]] — deploy de DLL única

## 📚 Conhecimento
- [[📜 Conventional Commits - Conceitos]] — tipos, verbos, formato do subject/corpo
- [[⚙️ Geração de mensagem a partir do diff]] — pipeline de extração e saneamento
- [[🧩 Plugin MEF para GitExtensions]] — modelo MEF de plugin
- [[📖 README — Instalação, Uso e Build]] — espelho do README

## 💼 Negócio
- [[📦 GitExtensions.ZimerfeldCommitMsg]] — nota-mãe: objetivo, stack, funcionalidades, ângulo de investimento, financiamento

## 🧭 Meta
- [[🔑 Fatos-Chave]] — caminhos, convenções, ferramentas
- [[🧭 Como usar este cofre]] — protocolo de leitura/escrita do Claude
- [[👤 Renato]] — preferências e contexto · [[🦀 RTK]] — proxy CLI de economia de tokens · [[📥 Inbox]]

## 🧱 Templates
- [[⚖️ Template - Decisão (ADR)]] — modelo para novas Decisões (ADRs)
- [[💼 Template - Negócio]] — modelo para notas de Negócio
- [[📚 Template - Conhecimento]] — modelo para notas de Conhecimento

## 📂 Estrutura de Pastas do Repo
```
GitExtensions.ZimerfeldCommitMsg/
├── src/GitExtensions.ZimerfeldCommitMsg/
│   ├── ZimerfeldCommitMsgPlugin.cs   ← entry point MEF + integração com o diálogo de commit
│   ├── CommitMessageGenerator.cs     ← motor: diff → tipos CC → verbo → subject + corpo
│   ├── RepoVocabularyConfig.cs        ← vocabulário extra por repo (.zimerfeldcommitmsg.json)
│   ├── Localization/                  ← MessageLanguage, LanguagePack, Strings
│   ├── Resources/                     ← icon.png, icon-128.png, Strings.resx, StringsPtBr.resx
│   ├── *.csproj / *.nuspec
├── tests/GitExtensions.ZimerfeldCommitMsg.Tests/  ← xUnit (comentários, conceitos, vocab, tradução)
├── refs/                              ← DLLs do host versionadas (Private=false)
├── tools/
│   ├── install.ps1 / uninstall.ps1 / update-dll.ps1
│   ├── net9.0-windows/                ← DLL para o nupkg
│   └── generate-icon.ps1
├── ZimerfeldCommitMsg/                ← 🧠 este cofre de memória
├── build.ps1                          ← incrementa versão + build + deploy + nupkg
└── README.md / README.pt-BR.md / README.en-US.md
```

## 📌 Retomada
- [[📌 Backlog]] — **comece por aqui** ao retomar o projeto em outra sessão

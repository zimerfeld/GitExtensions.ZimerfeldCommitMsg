---
tipo: moc
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-05
---

# 🏠 GitExtensions.ZimerfeldCommitMsg — Cofre de Neurônios

> 🇧🇷 Portugués → [[🏠 Home]] · 🇺🇸 English → [[🏠 Home (EN)]]

> [!abstract] 🧠 Qué es este cofre
> Memoria persistente de Claude para el proyecto **GitExtensions.ZimerfeldCommitMsg** — un plugin MEF para GitExtensions que genera mensajes de commit automáticamente a partir del diff en stage. Este cofre se mantiene en cada sesión: notas con par bilingüe (PT/EN), frontmatter estandarizado y ordenación por prioridad.

## ⚡ Resumen ejecutivo
- **Qué es:** una extensión (plugin MEF) para **GitExtensions** que se integra en el **diálogo de commit** y **rellena el mensaje automáticamente** analizando `git diff --cached`.
- **Problema que resuelve:** escribir buenos mensajes de commit es tedioso e inconsistente. El plugin lee lo que **realmente** cambió (comentarios añadidos en el diff + nombres de archivo), clasifica según Conventional Commits y materializa un subject + cuerpo listos — en pt-BR, inglés o español.
- **Diferenciales:** analiza el **contenido del diff**, no solo los nombres de archivo; **verbo guiado por Conventional Commits** sin contaminar el mensaje con `tipo:`; **dos estrategias** (comentarios + nombres de archivo); **vocabulario por repositorio** (`.zimerfeldcommitmsg.json`) sin recompilar; **auto-refresh** al stage/unstage; **no destructivo**; **i18n** (Automático / PT-BR / EN / ES).
- **Stack:** C# / WinForms `Library`, con destino **net9.0-windows**, empaquetado como **nupkg**; build y versionado mediante `build.ps1`.
- **Estado actual:** versión **`1.0.97`** — funcional, con **suite de tests xUnit**.
- **Público objetivo:** desarrolladores y equipos que usan GitExtensions en Windows y quieren mensajes de commit consistentes con poco esfuerzo.
- **Ángulo de negocio/portafolio:** producto **open source** bajo el owner `zimerfeld`, junto a los hermanos `GitExtensions.ZimerfeldLFS` y `GitExtensions.ZimerfeldTree`.

## 🧭 Navegación por prioridad

### 1️⃣ 🔑 Impacto — Archivos Clave
> Archivos que, si se manipulan, tienen un gran impacto en el sistema.
- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]] — entry point MEF + integración con el diálogo de commit
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] — el motor: diff → tipos CC → verbo → subject + cuerpo
- [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]] — vocabulario extra por repositorio (`.zimerfeldcommitmsg.json`)
- [[🌐 Localization (ES)|Localization]] — idioma del mensaje + strings de UI
- [[🛠️ build.ps1 (ES)|build.ps1]] — script de build, versionado y deploy

### 2️⃣ 🧩 Reutilización — Sistemas
> Subsistemas reutilizados por varias partes del proyecto.
- [[🔭 Visão Geral (ES)|Visión general]] — qué hace el plugin, stack, versión actual
- [[🏗️ Arquitetura (ES)|Arquitectura]] — Plugin (integración con el host) → Generator → Localization
- [[🏷️ Versionamento (ES)|Versionado]] — ciclo build.ps1 / nuspec / csproj
- [[🔗 Dependências (ES)|Dependencias]] — assemblies de GitExtensions + Conventional Commits

### 3️⃣ 🔀 Uso — Flujos
> Flujos de uso paso a paso.
- [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]] — Application.Idle detecta el FormCommit y lo rellena
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]] — diff → tipos CC → verbo → subject + cuerpo
- [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]] — dropdown de 4 idiomas, setting, refresh al stage/unstage

## 🚀 Operación
- [[💻 Ambiente Local (Dev) (ES)|Entorno Local (Dev)]] — `.\build.ps1 -Force` (compila + instala en el GitExtensions local)
- [[🚀 Deploy em Produção (Prod) (ES)|Deploy en Producción (Prod)]] — `.\build.ps1` → `.nupkg` en la raíz → NuGet + release de GitHub

## ⚖️ Decisiones (ADRs)
- [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown + Application.Idle]] — cómo entra el plugin en el diálogo de commit
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo]] — Conventional Commits sin el literal "tipo:"
- [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias: comentarios + nombres de archivo]] — de dónde viene el contenido
- [[📓 Vocabulário por repositório (ES)|Vocabulario por repositorio]] — `.zimerfeldcommitmsg.json` sin recompilar
- [[📦 Strings embutidas sem satellite assemblies (ES)|Strings de UI embebidas]] — deploy de DLL única

## 📚 Conocimiento
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]] — tipos, verbos, formato del subject/cuerpo
- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]] — pipeline de extracción y saneamiento
- [[🧩 Plugin MEF para GitExtensions (ES)|Plugin MEF para GitExtensions]] — modelo MEF de plugin
- [[📖 README — Instalação, Uso e Build (ES)|README — Instalación, Uso y Build]] — espejo del README

## 💼 Negocio
- [[📦 GitExtensions.ZimerfeldCommitMsg (ES)|GitExtensions.ZimerfeldCommitMsg]] — nota madre: objetivo, stack, funcionalidades, ángulo de inversión, financiación

## 🧭 Meta
- [[🔑 Fatos-Chave (ES)|Hechos Clave]] — rutas, convenciones, herramientas
- [[🧭 Como usar este cofre (ES)|Cómo usar este cofre]] — protocolo de lectura/escritura de Claude
- [[👤 Renato (ES)|Renato]] — preferencias y contexto · [[🦀 RTK (ES)|RTK]] — proxy CLI de ahorro de tokens · [[📥 Inbox (ES)|Inbox]]

## 🧱 Templates
- [[⚖️ Template - Decisão (ADR)|Template de Decisión (ADR)]] — modelo para nuevas Decisiones (ADRs)
- [[💼 Template - Negócio|Template de Negocio]] — modelo para notas de Negocio
- [[📚 Template - Conhecimento|Template de Conocimiento]] — modelo para notas de Conocimiento

## 📂 Repo Folder Structure
```
GitExtensions.ZimerfeldCommitMsg/
├── src/GitExtensions.ZimerfeldCommitMsg/
│   ├── ZimerfeldCommitMsgPlugin.cs   ← entry point MEF + integración con el diálogo de commit
│   ├── CommitMessageGenerator.cs     ← motor: diff → tipos CC → verbo → subject + cuerpo
│   ├── RepoVocabularyConfig.cs        ← vocabulario extra por repo (.zimerfeldcommitmsg.json)
│   ├── Localization/                  ← MessageLanguage, LanguagePack, Strings
│   ├── Resources/                     ← icon.png, icon-128.png, Strings.resx, StringsPtBr.resx, StringsEsEs.resx
│   ├── *.csproj / *.nuspec
├── tests/GitExtensions.ZimerfeldCommitMsg.Tests/  ← xUnit (comentarios, conceptos, vocab, traducción)
├── refs/                              ← DLLs del host versionadas (Private=false)
├── tools/
│   ├── install.ps1 / uninstall.ps1 / update-dll.ps1
│   ├── net9.0-windows/                ← DLL para el nupkg
│   └── generate-icon.ps1
├── ZimerfeldCommitMsg/                ← 🧠 este cofre de memoria
├── build.ps1                          ← incrementa versión + build + deploy + nupkg
└── README.md / README.pt-BR.md / README.en-US.md
```

## 📌 Retomada
- [[📌 Backlog (ES)|Backlog]] — **empieza por aquí** al retomar el proyecto en otra sesión

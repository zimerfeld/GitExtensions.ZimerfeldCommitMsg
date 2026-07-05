---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquitetura, classes, design, i18n, application-idle]
---

# 🏗️ Arquitetura

> 🇺🇸 Read this page in English → [[🏗️ Arquitetura (EN)]]

## 🗺️ Diagrama de classes

```
GitExtensions (host)
    │
    │  MEF (System.ComponentModel.Composition)
    ▼
ZimerfeldCommitMsgPlugin   ← [Export(IGitPlugin)] : GitPluginBase
    │  Register()   → captura IGitUICommands, registra template de commit,
    │                 assina Application.Idle
    │  OnAppIdle()  → detecta o FormCommit recém-aberto e preenche a mensagem
    │  Execute()    → gera para o repo atual (menu Plugins)
    │  GetSettings() → seletor de idioma (ChoiceSetting)
    ▼
CommitMessageGenerator      ← o motor
    │  Generate()  → subject consolidado + corpo em bullets
    │  lê `git diff --cached`; classifica tipos CC; extrai comentários; deriva conceitos
    ▼
git diff --cached (subprocesso)
        +
Localization (LanguagePack / Strings / MessageLanguage)  → idioma da saída + strings de UI
        +
RepoVocabularyConfig  ← .zimerfeldcommitmsg.json (vocabulário extra por repo)
```

## 🧩 As classes

### 🔌 `ZimerfeldCommitMsgPlugin` — ponto de entrada e integração
Herda de `GitPluginBase`, exportado via MEF. Responsável por **entrar no diálogo de commit** do host e injetar a mensagem. Ver [[🔌 ZimerfeldCommitMsgPlugin]].
- **`Register`** — captura o `IGitUICommands` e o `SynchronizationContext` (UI thread), registra os **três itens de template** de commit (um por idioma) e assina **`Application.Idle`**.
- **`OnAppIdle`** — detecta o `FormCommit` recém-aberto (não há evento de API), gera a mensagem e preenche a caixa; faz gates por **instância** (`WeakReference<Form>`) e por **working dir** para não reprocessar a cada tick, e **detecta a escolha do dropdown** observando o `TextChanged` da caixa.
- **`Execute`** — gera a mensagem para o repositório atual (acionado pelo menu Plugins).
- **`GetSettings`** — expõe o `ChoiceSetting` de idioma (Automático / Português / Inglês).

### ⚙️ `CommitMessageGenerator` — o motor
Lê o `git diff --cached`, classifica os arquivos por **categoria semântica** (extensão → source/web/docs/build/config) e por **tipo Conventional Commits**, extrai **comentários** do diff e deriva **conceitos** dos nomes de arquivo, e monta o **subject consolidado** + o **corpo em bullets**. ~1200 linhas de funções em sua maioria puras (testáveis via `InternalsVisibleTo`). Ver [[⚙️ CommitMessageGenerator]] e [[⚙️ Geração de mensagem a partir do diff]].

### 🌐 `Localization` — idioma da saída + strings de UI
- `MessageLanguage` (`PtBr`/`En`) + `MessageLanguageResolver` (resolve "Automático" pelo `CultureInfo.CurrentUICulture`).
- `LanguagePack` — verbos, palavras de tipo, pluralização, etc., por idioma (usado pelo gerador).
- `Strings` — strings de UI (avisos) lidas dos recursos embutidos `Strings.resx`/`StringsPtBr.resx`. Ver [[🌐 Localization]].

### 📓 `RepoVocabularyConfig` — extensão por repositório
Carrega `.zimerfeldcommitmsg.json` da raiz do working dir (opcional): `knownVocabulary`, `rejectedVocabulary`, `concepts`. Somado aos defaults embutidos, sem recompilar. Falhas de parse são silenciosas. Ver [[📓 RepoVocabularyConfig]].

## 🔌 Integração com o host

> [!important] O plugin **preenche** o diálogo de commit sem depender de um evento dedicado
> O GitExtensions não expõe "diálogo de commit aberto" na API, então o plugin observa o `FormCommit` via **`Application.Idle`**. A escolha de um item do **dropdown de templates** é reconhecida indiretamente — o host materializa o texto de **todos** os itens ao abrir o dropdown e, no clique, só aplica o texto; o plugin registra cada texto gerado (msg → idioma) e detecta pela caixa qual foi escolhido (`TextChanged`). Ver [[🔌 Integração via template dropdown e Application.Idle]].

O preenchimento é **não destrutivo**: se a caixa já tem texto digitado pelo usuário, o plugin não sobrescreve.

## 🌐 Localização (i18n)

Dois eixos independentes:
1. **Idioma da mensagem** — `EffectiveLanguage()` = escolha do dropdown (`_sessionLanguage`) tem prioridade sobre o setting/SO (`CurrentLanguage()`), que resolve "Automático" pelo `CultureInfo.CurrentUICulture`.
2. **Strings de UI** — lidas por `Strings` dos recursos neutros embutidos (`InvariantCulture` evita probing de satellite assemblies). Ver [[🌐 Localization]] e [[📦 Strings embutidas sem satellite assemblies]].

## 🧵 Threading

- `Register` roda na UI thread e captura o `SynchronizationContext` para marshalling seguro.
- `OnAppIdle` roda na UI thread (evento da UI); o trabalho de geração é rápido (subprocesso `git diff --cached`), com gates para não repetir a cada tick.

## 🔗 Ligações

- [[🔌 ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator]]
- [[🌐 Localization]]
- [[🔌 Integração via template dropdown e Application.Idle]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo]]
- [[🔗 Dependências]]

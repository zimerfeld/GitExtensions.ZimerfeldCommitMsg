---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquivo, plugin, entry-point, mef, application-idle]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs
---

# 🔌 ZimerfeldCommitMsgPlugin.cs

Ponto de entrada do plugin e **integração com o diálogo de commit** do GitExtensions. Exportado via MEF. ~626 linhas.

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs`

---

## 📜 Declaração

```csharp
[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
```

O atributo `[Export]` é o ponto de descoberta pelo MEF do host. Ver [[🧩 Plugin MEF para GitExtensions]].

---

## 🧩 Itens de template e setting

- `TemplatePrefix = "Zimerfeld Commit Msg"`.
- `_templateItems` — três itens `(Label, MessageLanguage?)`: Automático (`null`), Português (`PtBr`), Inglês (`En`). A API de templates do host é **plana**, então expõe-se um item por idioma.
- `_languageSetting` — `ChoiceSetting("ZimerfeldCommitMsg_Language", …)` com as opções Automático/Português/Inglês (default Automático), exposto por `GetSettings()`.

---

## 🗃️ Campos de instância (gates e estado)

| Campo | Propósito |
|---|---|
| `_syncContext` | `SynchronizationContext` capturado no `Register` (UI thread) para marshalling |
| `_gitUiCommands` | fonte do working dir para o gatilho de `Application.Idle` |
| `_handledCommitForm` | `WeakReference<Form>` — FormCommit já preenchido (evita reprocessar a cada Idle) |
| `_handledWorkingDir` | working dir do último preenchimento (host reaproveita o FormCommit ao trocar de repo) |
| `_sessionLanguage` | idioma fixado pela escolha de um item do dropdown (`null` = seguir setting/SO) |
| `_templateMessages` | mapa texto-gerado → idioma do item, para reconhecer a escolha do dropdown |
| `_subscribedTextBox` | `WeakReference<TextBoxBase>` da caixa em que já assinamos `TextChanged` |
| `_resyncedCommitForm` | garante UMA re-sincronização de abertura por janela |

---

## 🗣️ Idioma efetivo

```csharp
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage();
CurrentLanguage()   = MessageLanguageResolver.Resolve(_languageSetting.ValueOrDefault(Settings));
```
A escolha do dropdown tem prioridade sobre o setting/SO. Ver [[🌐 Localization]] e [[🌐 3 - Idioma e auto-refresh]].

---

## ⚙️ Métodos (IGitPlugin)

### `Register(IGitUICommands)`
- `base.Register` + captura `_gitUiCommands` e `_syncContext`.
- Registra os **três itens de template** no `CommitTemplateManager` (um `Func` de geração por idioma).
- Assina **`Application.Idle += OnAppIdle`** — o host não expõe "diálogo de commit aberto", então detectamos o `FormCommit` no Idle.

### `OnAppIdle`
- Varre `Application.OpenForms` por um `Form` cujo `GetType().Name == "FormCommit"`.
- Gates por **instância** (`_handledCommitForm`) e por **working dir** (`_handledWorkingDir`).
- Preenche a caixa de mensagem se vazia (**não destrutivo**); assina `TextChanged` para detectar a escolha do dropdown. Ver [[🔍 1 - Detecção do diálogo de commit]].

### `GenerateForTemplate(MessageLanguage? forced)`
- Gera a mensagem para um item do dropdown, no idioma do item.
- **Não fixa o idioma aqui** (o host chama este `Func` para todos os itens ao abrir o dropdown). Em vez disso, `RememberTemplateMessage(msg, forced)` registra o texto → idioma para reconhecer depois qual item o usuário clicou (`DetectTemplateSelection`). Ver [[🔌 Integração via template dropdown e Application.Idle]].

### `ResolveCommitWorkingDir()`
- Prefere o working dir do próprio `FormCommit` aberto (via reflection), com fallback ao `_gitUiCommands.Module.WorkingDir` — evita gerar no repo errado/vazio.

### `Execute(GitUIEventArgs)` ← menu Plugins → ZimerfeldCommitMsg
- Gera a mensagem para o repositório atual usando o idioma do setting.

### `Unregister(IGitUICommands)`
- Desregistra o template, `Application.Idle -= OnAppIdle`, limpa o commands capturado.

---

## 🛡️ Proteção contra crash

Delegates, reflection sobre o `FormCommit` e a geração são envoltos em `try/catch` — exceções no plugin nunca derrubam o GitExtensions.

---

## 🔗 Ligações

- [[⚙️ CommitMessageGenerator]]
- [[🌐 Localization]]
- [[🏗️ Arquitetura]]
- [[🔌 Integração via template dropdown e Application.Idle]]
- [[🔍 1 - Detecção do diálogo de commit]]

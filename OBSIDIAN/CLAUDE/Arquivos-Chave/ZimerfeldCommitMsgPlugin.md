---
tipo: arquivo
tags: [arquivo, plugin, entry-point, mef, winforms, icone]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs
linhas: 178
atualizado: 2026-06-01
---

# ZimerfeldCommitMsgPlugin.cs

Ponto de entrada do plugin. Exportado via MEF para o GitExtensions.

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/ZimerfeldCommitMsgPlugin.cs`

---

## Declaração

```csharp
[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
```

O atributo `[Export]` é o ponto de descoberta pelo MEF do GitExtensions.

---

## Campos de instância

| Campo | Tipo | Propósito |
|---|---|---|
| `TemplatePrefix` | `const string` | `"Zimerfeld Commit Msg"` — prefixo dos rótulos no dropdown |
| `_templateItems` | `(string Label, MessageLanguage? Lang)[]` | 3 itens: Auto (`null`) / PT / EN |
| `_languageSetting` | `static ChoiceSetting` | `ZimerfeldCommitMsg_Language` — idioma padrão (Auto/PT/EN) exposto em `GetSettings()` |
| `PluginIcon` | `static Image?` | Ícone do plugin (menu Plugins + dropdown), via `LoadIcon()` |
| `_syncContext` | `SynchronizationContext?` | Capturado no `Register()` para marshalling UI |
| `_lastGeneratedMessage` | `string` | Última mensagem gerada — protege texto manual do usuário |
| `_sessionLanguage` | `MessageLanguage?` | Idioma fixado pelo item de dropdown escolhido (prioridade sobre setting/SO) |

### `LoadIcon()` → `Image?`
Lê o recurso embutido `GitExtensions.ZimerfeldCommitMsg.Resources.icon.png` via `Assembly.GetManifestResourceStream`, retorna uma `Bitmap` independente do stream. Falha silenciosa (`null`) se o recurso não existir. Atribuído a `Icon` no construtor quando não-nulo.

---

## Métodos

### `Register(IGitUICommands)`
- Chama `base.Register()`
- Captura `SynchronizationContext.Current` (UI thread)
- Para cada um dos 3 `_templateItems`: `AddCommitTemplate(label, () => GenerateForTemplate(workingDir, forced), icon)` — factory **lazy** que fixa `_sessionLanguage` e instancia `CommitMessageGenerator(workingDir, idioma)`; ver [[../Fluxos/Template Dropdown (Auto-resumo)]]
- Captura `_gitUiCommands` (fonte do working dir para o Idle)
- Assina `PostRepositoryChanged += OnPostRepositoryChanged` (auto-refresh ao stage/unstage)
- Assina `Application.Idle += OnAppIdle` — detecta o `FormCommit` aberto já com arquivos em stage e preenche uma vez por instância (`WeakReference` em `_handledCommitForm`); `RefreshOpenCommitDialog` retorna `bool` (false = UI ainda montando → tenta no próximo Idle). Ambas as assinaturas são desfeitas no `Unregister`

### `GetSettings()` → expõe `_languageSetting`
Faz o nó **ZimerfeldCommitMsg** aparecer em Configurações → Plugins (após instalar a DLL ≥ 1.0.36 e reiniciar). `CurrentLanguage()` lê o valor e resolve "Automático" pelo SO; `EffectiveLanguage()` = `_sessionLanguage ?? CurrentLanguage()`.

### `Unregister(IGitUICommands)`
- Remove o handler do evento
- `RemoveCommitTemplate(label)` para cada um dos 3 itens
- Limpa `_lastGeneratedMessage`

### `Execute(GitUIEventArgs)` ← menu Plugins
- Valida `IsValidGitWorkingDir()`
- Gera mensagem com `CommitMessageGenerator`
- Se vazia → `MessageBox` de aviso
- Se OK → `StartCommitDialog(owner, commitMessage: message)`

### `OnPostRepositoryChanged(object?, GitUIEventArgs)` ← evento
- Extrai `workingDir` de `e.GitModule`
- Marechala para UI thread via `_syncContext.Post()`

### `RefreshOpenCommitDialog(string workingDir)`
- Itera `Application.OpenForms` buscando `FormCommit`
- `FindCommitTextBox()` → localiza o campo de mensagem
- Compara `tb.Text` com `_lastGeneratedMessage` antes de sobrescrever
- Chama `CommitMessageGenerator.Generate()` e atualiza o campo

### `FindCommitTextBox(Form)` ← helper estático
Tenta nomes: `"Message"`, `"commitMessageEditor"`, `"_commitMessage"`, `"commitMessage"`.
Fallback: maior `TextBoxBase` multiline, editável e visível.

### `FindDescendantByName(Control, string)` ← helper estático
Busca em profundidade na árvore de controls por nome.

### `EnumerateDescendants(Control)` ← helper estático
Iterador que percorre toda a árvore de controls recursivamente.

---

## Proteção de thread

```
Register() → UI thread → _syncContext capturado
PostRepositoryChanged → qualquer thread
    → _syncContext.Post(...) → volta para UI thread
    → Application.OpenForms / tb.Text acessados com segurança
```

---

## Proteção contra crash

Todos os handlers têm `try/catch {}` vazio — exceções no plugin nunca derrubam o GitExtensions.

---

## Relacionado

- [[CommitMessageGenerator]]
- [[../Fluxos/Template Dropdown (Auto-resumo)]]
- [[../Fluxos/Stage Trigger]]
- [[../Sistema/Dependências]]

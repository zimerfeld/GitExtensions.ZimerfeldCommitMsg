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
| `_sessionLanguage` | `MessageLanguage?` | Idioma fixado pela escolha de um item do dropdown (prioridade sobre setting/SO); setado por `DetectTemplateSelection`, reiniciado ao fechar o diálogo |
| `_templateMessages` | `Dictionary<string, MessageLanguage?>` | Mensagens geradas para os itens do dropdown na abertura do menu, mapeadas ao idioma do item — base da detecção da escolha |
| `_subscribedTextBox` | `WeakReference<TextBoxBase>?` | Caixa cujo `TextChanged` já assinamos (para detectar a escolha do dropdown) |
| `_resyncedCommitForm` | `WeakReference<Form>?` | Instância de `FormCommit` para a qual a re-sincronização de abertura já rodou (1× por janela); limpa ao fechar |

### `LoadIcon()` → `Image?`
Lê o recurso embutido `GitExtensions.ZimerfeldCommitMsg.Resources.icon.png` via `Assembly.GetManifestResourceStream`, retorna uma `Bitmap` independente do stream. Falha silenciosa (`null`) se o recurso não existir. Atribuído a `Icon` no construtor quando não-nulo.

---

## Métodos

### `Register(IGitUICommands)`
- Chama `base.Register()`
- Captura `SynchronizationContext.Current` (UI thread)
- Para cada um dos 3 `_templateItems`: `AddCommitTemplate(label, () => GenerateForTemplate(workingDir, forced), icon)` — a factory instancia `CommitMessageGenerator(workingDir, idioma)` e registra `msg → idioma` (`RememberTemplateMessage`). ⚠️ O host invoca a factory dos 3 itens ao **ABRIR** o dropdown (não no clique), então a fixação de idioma **não** é feita aqui: é detectada pelo `TextChanged` da caixa (`DetectTemplateSelection`). Ver [[../Fluxos/Template Dropdown (Auto-resumo)]]
- Captura `_gitUiCommands` (fonte do working dir para o Idle)
- Assina `PostRepositoryChanged += OnPostRepositoryChanged` (auto-refresh ao stage/unstage)
- Assina `Application.Idle += OnAppIdle` — detecta o `FormCommit` aberto já com arquivos em stage e preenche uma vez por instância (`WeakReference` em `_handledCommitForm`); `RefreshOpenCommitDialog` retorna `bool` (false = UI ainda montando → tenta no próximo Idle). Ambas as assinaturas são desfeitas no `Unregister`

### `GetSettings()` → expõe `_languageSetting`
Faz o nó **ZimerfeldCommitMsg** aparecer em Configurações → Plugins (após instalar a DLL ≥ 1.0.36 e reiniciar). `CurrentLanguage()` lê o valor e resolve "Automático" pelo SO; `EffectiveLanguage()` = `_sessionLanguage ?? CurrentLanguage()`.

### `Unregister(IGitUICommands)`
- Remove o handler do evento (`PostRepositoryChanged`, `Application.Idle`)
- `RemoveCommitTemplate(label)` para cada um dos 3 itens
- Desassina o `TextChanged` da caixa (`_subscribedTextBox`) e limpa `_templateMessages` / `_sessionLanguage`
- Limpa `_lastGeneratedMessage`

### `Execute(GitUIEventArgs)` ← menu Plugins
- Valida `IsValidGitWorkingDir()`
- Gera mensagem com `CommitMessageGenerator`
- Se vazia → `MessageBox` de aviso
- Se OK → `StartCommitDialog(owner, commitMessage: message)`

### `OnPostRepositoryChanged(object?, GitUIEventArgs)` ← evento
- Extrai `workingDir` de `e.GitModule`
- Marechala para UI thread via `_syncContext.Post()`

### `RefreshOpenCommitDialog(string fallbackWorkingDir)`
- Itera `Application.OpenForms` buscando `FormCommit`
- **Working dir = `GetCommitFormWorkingDir(commitForm)`** (reflexão de `GitModuleForm.Module.WorkingDir`), fallback ao parâmetro — fonte de verdade do repo do diálogo, imune a defasagem do `_gitUiCommands` ao trocar de repo/branch
- `FindCommitTextBox()` → localiza o campo de mensagem
- `EnsureTextChangedHook(tb)` → assina o `TextChanged` (detecção de escolha do dropdown)
- Compara `tb.Text` com `_lastGeneratedMessage` antes de sobrescrever
- Chama `CommitMessageGenerator.Generate()` no idioma `EffectiveLanguage()` e atualiza o campo

### Detecção da escolha do dropdown (pinning de idioma)
- `EnsureTextChangedHook(TextBoxBase)` — assina `OnCommitTextChanged` uma vez por instância de caixa (reassina se mudar)
- `OnCommitTextChanged(...)` → `DetectTemplateSelection(tb)`
- `DetectTemplateSelection(TextBoxBase)` — se `tb.Text` == uma das `_templateMessages`, fixa `_sessionLanguage` no idioma daquele item e marca o texto como nosso. Só observa (não escreve) → sem loop de `TextChanged`
- `RememberTemplateMessage(msg, forced)` — registra `msg → idioma` na abertura do dropdown

### `ResyncForCommitDialog(Form commitForm)` — REGRA: re-sync CIRÚRGICA a cada abertura
- Disparado pelo `OnAppIdle` quando um `FormCommit` **novo** é detectado (qualquer origem: GitExtensions, menu Plugins, ZimerfeldTree)
- Resolve commands de `_gitUiCommands` **ou** do próprio FormCommit (`GetCommitFormUICommands`) → funciona com captura defasada/nula
- **GARANTE os templates** (`EnsureCommitTemplates`, idempotente) — ponto central da regra "qualquer plugin que abra a janela de commit deve ter o dropdown gerando texto" (cobre o gap da re-registração assíncrona do host)
- Re-aponta settings; re-assina `PostRepositoryChanged` (idempotente); reinicia `_sessionLanguage`/`_templateMessages`/`_lastGeneratedMessage`
- **Não** remove nada nem mexe no `Application.Idle`. Roda **1× por janela** via `_resyncedCommitForm`

### `EnsureCommitTemplates(IGitUICommands)`
Adiciona os 3 itens do dropdown (`() => GenerateForTemplate(forced)`). Idempotente: `AddCommitTemplate` só adiciona se o nome não existe no storage **estático**. Chamado no `Register` e no `ResyncForCommitDialog`.

### `GenerateForTemplate(MessageLanguage? forced)` — Func do dropdown
- Resolve o working dir via `ResolveCommitWorkingDir()` (do FormCommit aberto, fallback `_gitUiCommands`), gera, registra `msg → idioma` e retorna
- `ResolveCommitWorkingDir()` usa a mesma fonte de verdade do auto-refresh — evita gerar no repo errado/vazio
- **REGRA stage vazio:** se `msg` for vazia (⟺ nada em stage), chama `ClearOpenCommitDialog()` → limpa QUALQUER texto da caixa. Como o host avalia o Func na abertura do dropdown, a limpeza acontece ao abrir o menu com stage vazio

### `ClearOpenCommitDialog()`
Acha o `FormCommit` aberto + caixa e zera o texto (inclusive digitado pelo usuário); marca `_lastGeneratedMessage = ""`. No-op se já vazia. Tudo em try/catch.

### `GetCommitFormWorkingDir(Form)` ← helper estático
Lê `GitModuleForm.Module.WorkingDir` por reflexão — working dir do repo do diálogo (fonte de verdade).

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

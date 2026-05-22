---
tipo: fluxo
tags: [fluxo, evento, stage, ui, thread]
atualizado: 2026-05-22
---

# Fluxo: Stage Trigger (Auto-refresh)

Quando o usuário faz stage ou unstage de arquivos no GitExtensions, o plugin atualiza automaticamente o campo de mensagem no `FormCommit` se ele estiver aberto.

## Sequência de eventos

```
Usuário faz stage/unstage
        │
        ▼
GitExtensions dispara PostRepositoryChanged
        │
        ▼
OnPostRepositoryChanged(sender, GitUIEventArgs e)
        │   workingDir = e.GitModule.WorkingDir
        ├── _syncContext.Post(...)        ← marshalling para UI thread
        │
        ▼ (UI thread)
RefreshOpenCommitDialog(workingDir)
        │
        ├── Itera Application.OpenForms
        │       └── busca Form onde f.GetType().Name == "FormCommit"
        │
        ├── FindCommitTextBox(commitForm)
        │       ├── busca por nome: "Message", "commitMessageEditor", "_commitMessage", "commitMessage"
        │       └── fallback: maior TextBox multiline editável visível
        │
        ├── Verifica se pode sobrescrever:
        │       current = tb.Text.Trim()
        │       SE current.Length > 0 E current != _lastGeneratedMessage.Trim()
        │           → NÃO atualiza (usuário digitou algo manualmente)
        │
        └── CommitMessageGenerator(workingDir).Generate()
                msg → _lastGeneratedMessage = msg
                     tb.Text = msg
                     tb.SelectionStart = 0
```

## Proteção contra sobrescrita manual

O campo `_lastGeneratedMessage` armazena a última mensagem gerada pelo plugin.

**Lógica:** só atualiza se o campo estiver vazio **ou** contiver exatamente a última mensagem gerada. Se o usuário apagar e redigitar algo diferente, o plugin para de interferir.

## Thread safety

- `SynchronizationContext` capturado no `Register()` (chamado na UI thread)
- Todo acesso a `Application.OpenForms` e `tb.Text` ocorre via `_syncContext.Post()`
- Bloco `try/catch` vazio garante que exceções nunca derrubam o GitExtensions

## Busca do FormCommit

```csharp
// Nomes conhecidos em versões diferentes do GitExtensions:
foreach (var name in ["Message", "commitMessageEditor", "_commitMessage", "commitMessage"])
    if (FindDescendantByName(form, name) is TextBoxBase tb) return tb;

// Fallback: maior área editável
return EnumerateDescendants(form)
    .OfType<TextBoxBase>()
    .Where(t => t.Multiline && !t.ReadOnly && t.Visible)
    .OrderByDescending(t => t.Width * t.Height)
    .FirstOrDefault();
```

`FindDescendantByName` e `EnumerateDescendants` fazem busca em profundidade na árvore de controls.

## Relacionado

- [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]]
- [[Geração da Mensagem]]

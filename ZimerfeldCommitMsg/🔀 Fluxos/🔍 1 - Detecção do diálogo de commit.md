---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [fluxo, application-idle, formcommit, etapa1]
---

# 🔍 Fluxo: Detecção do diálogo de commit (Application.Idle)

Como o plugin entra no **diálogo de commit** do GitExtensions e preenche a mensagem, sem que o host ofereça um evento de "diálogo aberto".

## 🪜 Passos

```
Register(commands)
        │  captura _gitUiCommands + _syncContext (UI thread)
        │  registra 3 itens de template (idiomas) no CommitTemplateManager
        │  Application.Idle += OnAppIdle
        ▼
Usuário abre o diálogo de commit (FormCommit)
        │
        ▼
OnAppIdle  (dispara muitas vezes)
        │
        ├─ varre Application.OpenForms por um Form cujo GetType().Name == "FormCommit"
        ├─ gate por instância (_handledCommitForm: WeakReference<Form>) — já tratada?
        ├─ gate por working dir (_handledWorkingDir) — host reaproveita o FormCommit ao
        │   trocar de repo; se o dir mudou, RE-preenche
        ├─ localiza a caixa de mensagem (TextBoxBase) do FormCommit
        ├─ se a caixa está VAZIA (ou é reprocesso legítimo) → gera e injeta a mensagem
        │   (NÃO destrutivo: nunca sobrescreve texto digitado pelo usuário)
        └─ assina TextChanged uma vez (_subscribedTextBox) → detecta escolha do dropdown
```

## 🔎 Detalhes

- **Sem evento de API:** não existe "FormCommit aberto" na extensibilidade do GitExtensions. Por isso o plugin observa o **`Application.Idle`** da UI e procura o `FormCommit` em `Application.OpenForms` por **nome de tipo** (evita depender de tipos internos do host).
- **Gates para não repetir:** `Application.Idle` dispara continuamente. O plugin guarda a **instância** já tratada (`WeakReference<Form>`, para não prender o form) e o **working dir** do último preenchimento. O host pode **reaproveitar** o mesmo `FormCommit` ao trocar de repositório — sem rastrear o working dir, o gate por instância bloquearia o repreenchimento e a caixa ficaria desatualizada.
- **Working dir:** `ResolveCommitWorkingDir()` prefere o working dir do **próprio** `FormCommit` aberto (via reflection), com fallback ao `_gitUiCommands.Module.WorkingDir` capturado — mesma fonte de verdade da geração via dropdown, evitando gerar no repo errado.
- **Auto-refresh ao stage/unstage:** como o diálogo permanece aberto, um novo `Idle` com a caixa ainda "nossa" regenera a mensagem quando o conjunto staged muda. Ver [[🌐 3 - Idioma e auto-refresh]].
- **Proteção:** tudo é best-effort em `try/catch` — exceções no plugin nunca derrubam o GitExtensions.

## 🔗 Ligações

- [[⚙️ 2 - Geração da mensagem]]
- [[🌐 3 - Idioma e auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin]]
- [[🔌 Integração via template dropdown e Application.Idle]]

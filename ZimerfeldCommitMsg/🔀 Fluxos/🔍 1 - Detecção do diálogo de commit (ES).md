---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [fluxo, application-idle, formcommit, etapa1]
---

# 🔍 Flujo: Detección del diálogo de commit (Application.Idle)

> 🇧🇷 Portugués → [[🔍 1 - Detecção do diálogo de commit]] · 🇺🇸 English → [[🔍 1 - Detecção do diálogo de commit (EN)]]

Cómo el plugin entra en el **diálogo de commit** de GitExtensions y rellena el mensaje, sin que el host ofrezca un evento de "diálogo abierto".

## 🪜 Pasos

```
Register(commands)
        │  captura _gitUiCommands + _syncContext (UI thread)
        │  registra 4 ítems de plantilla (idiomas) en el CommitTemplateManager
        │  Application.Idle += OnAppIdle
        ▼
El usuario abre el diálogo de commit (FormCommit)
        │
        ▼
OnAppIdle  (se dispara muchas veces)
        │
        ├─ recorre Application.OpenForms por un Form cuyo GetType().Name == "FormCommit"
        ├─ gate por instancia (_handledCommitForm: WeakReference<Form>) — ¿ya tratada?
        ├─ gate por working dir (_handledWorkingDir) — el host reaprovecha el FormCommit al
        │   cambiar de repo; si el dir cambió, RE-rellena
        ├─ localiza la caja de mensaje (TextBoxBase) del FormCommit
        ├─ si la caja está VACÍA (o es un reproceso legítimo) → genera e inyecta el mensaje
        │   (NO destructivo: nunca sobrescribe texto escrito por el usuario)
        └─ suscribe TextChanged una vez (_subscribedTextBox) → detecta la elección del dropdown
```

## 🔎 Detalles

- **Sin evento de API:** no existe "FormCommit abierto" en la extensibilidad de GitExtensions. Por eso el plugin observa el **`Application.Idle`** de la UI y busca el `FormCommit` en `Application.OpenForms` por **nombre de tipo** (evita depender de tipos internos del host).
- **Gates para no repetir:** `Application.Idle` se dispara continuamente. El plugin guarda la **instancia** ya tratada (`WeakReference<Form>`, para no retener el form) y el **working dir** del último rellenado. El host puede **reaprovechar** el mismo `FormCommit` al cambiar de repositorio — sin rastrear el working dir, el gate por instancia bloquearía el rerrellenado y la caja quedaría desactualizada.
- **Working dir:** `ResolveCommitWorkingDir()` prefiere el working dir del **propio** `FormCommit` abierto (vía reflection), con reserva al `_gitUiCommands.Module.WorkingDir` capturado — la misma fuente de verdad que la generación vía dropdown, evitando generar en el repo equivocado.
- **Auto-refresh al stage/unstage:** como el diálogo permanece abierto, un nuevo `Idle` con la caja aún "nuestra" regenera el mensaje cuando el conjunto staged cambia. Ver [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]].
- **Protección:** todo es best-effort en `try/catch` — las excepciones del plugin nunca tumban GitExtensions.

## 🔗 Enlaces

- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]
- [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]
- [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown y Application.Idle]]

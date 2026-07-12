---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [decisao, adr, integração, application-idle, template, formcommit]
status: aceita
---

# 🔌 ADR — Integración vía dropdown de templates + Application.Idle

> 🇧🇷 Portugués → [[🔌 Integração via template dropdown e Application.Idle]] · 🇺🇸 English → [[🔌 Integração via template dropdown e Application.Idle (EN)]]

## 🎯 Contexto
El plugin necesita **rellenar la caja de mensaje** del diálogo de commit de GitExtensions y **mantener el mensaje actualizado** mientras el usuario modifica el stage. Pero la extensibilidad del host **no expone** un evento de "diálogo de commit abierto" ni un callback de "ítem de template pulsado".

## ✅ Decisión
Combinar dos mecanismos:
1. **Ítems de template en el dropdown de commit** (`CommitTemplateManager`) — uno por idioma (Automático / portugués / inglés / español, cuatro ítems en total). Materializan el mensaje generado y permiten la elección rápida del idioma.
2. **`Application.Idle`** — detecta el `FormCommit` recién abierto (recorriendo `Application.OpenForms` por nombre de tipo) y **rellena la caja**, con gates por instancia y working dir para no reprocesar en cada tick.

La elección de un ítem del dropdown se reconoce **indirectamente**: el host materializa el texto de **todos** los ítems al abrir el dropdown y, al hacer clic, solo aplica el texto. El plugin registra cada texto generado (msg → idioma) y detecta mediante la caja (`TextChanged`) cuál fue elegido, fijando el idioma de la sesión.

## 🔀 Alternativas consideradas
- **Solo template** — no cubre el auto-refresh al hacer stage/unstage ni el rellenado inicial fiable.
- **Suscribirse a eventos del host** — no hay un evento adecuado; dependería de tipos internos frágiles entre versiones.
- **Reflection + Application.Idle (elegida)** — funciona con la API pública, tolerante a versiones (encaja por el **nombre** del tipo `FormCommit`).

## ⚖️ Consecuencias
**Positivas:**
- Rellenado automático al abrir y en cada cambio de stage, sin sobrescribir el texto tecleado (**no destructivo**).
- Elección de idioma por commit en el propio diálogo.

**Negativas / trade-offs:**
- `Application.Idle` se dispara mucho → exige gates cuidadosos (instancia + working dir) para no reprocesar.
- La detección del clic en el dropdown es indirecta (vía `TextChanged`) — más sutil que un callback.
- Localizar la caja de mensaje depende de reflection sobre el `FormCommit`.

## 🔗 Relacionado
- [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]]
- [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]]
- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]
- [[🏗️ Arquitetura (ES)|Arquitectura]]

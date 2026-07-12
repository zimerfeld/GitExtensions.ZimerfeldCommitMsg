---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [fluxo, i18n, dropdown, auto-refresh, etapa3]
---

# 🌐 Flujo: Idioma y auto-refresh

> 🇧🇷 Portugués → [[🌐 3 - Idioma e auto-refresh]] · 🇺🇸 English → [[🌐 3 - Idioma e auto-refresh (EN)]]

Cómo el usuario elige el idioma y cómo el mensaje se mantiene actualizado mientras el diálogo está abierto.

## 🗣️ Dos formas de elegir el idioma

**1. Dropdown de plantillas de la pantalla de commit** — cuatro ítems (elección rápida por commit):
```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
Zimerfeld Commit Msg — Espanhol/Español
```
**2. Configuración → Plugins → ZimerfeldCommitMsg** — el selector **"Idioma da mensagem / Message language"** define el **valor por defecto** (usado por el menú Plugins y por el auto-refresh cuando no se ha elegido ningún ítem del dropdown).

| Opción | Comportamiento |
|---|---|
| `Automático/Automatic` | **Por defecto.** Detecta por el SO (`pt-*` → portugués; `es-*` → español; otro → inglés) |
| `Português/Portuguese` | Fuerza pt-BR |
| `Inglês/English` | Fuerza inglés |
| `Espanhol/Español` | Fuerza español (es-ES) |

## 🔎 Cómo se detecta la elección del dropdown

```
El host abre el dropdown de plantillas
        │  invoca el Func de generación de TODOS los ítems, en secuencia
        │  (NO hay callback de clic — el host materializa el texto de cada ítem)
        ▼
GenerateForTemplate(forced)  → genera en el idioma del ítem + RememberTemplateMessage(msg → idioma)
        │
        ▼
El usuario hace clic en un ítem  → el host aplica el texto materializado en la caja (ReplaceMessage)
        │
        ▼
TextChanged de la caja  → DetectTemplateSelection: ¿la caja se volvió exactamente un texto conocido?
        │  si sí → fija _sessionLanguage en el idioma de ese ítem
        ▼
El auto-refresh pasa a usar EffectiveLanguage() = _sessionLanguage (prioridad sobre setting/SO)
```

> [!note] Por qué la detección es indirecta
> La API de plantillas del host **no** da callback de clic: llama al `Func` de **todos** los ítems al **abrir** el dropdown y, al hacer clic, solo aplica el texto ya materializado. Si el plugin fijara el idioma dentro del `Func`, se ejecutaría para todos los ítems y fijaría siempre el último. Por eso el plugin **registra cada texto generado** (msg → idioma) y reconoce la elección observando la caja (`TextChanged`). Ver [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown y Application.Idle]].

## 🔄 Auto-refresh al stage/unstage

Con el diálogo abierto, cuando los archivos entran/salen del stage, el siguiente `Application.Idle` regenera el mensaje en el idioma efectivo (`EffectiveLanguage()`), siempre que la caja siga siendo "nuestra" (no editada manualmente). Ver [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]].

## 🗣️ Idioma efectivo

```
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()
CurrentLanguage()   = setting (Automático/PT/EN/ES) → MessageLanguageResolver.Resolve
                      "Automático" → FromCulture(CultureInfo.CurrentUICulture)
```

## 🔗 Enlaces

- [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]]
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]
- [[🌐 Localization (ES)|Localization]]
- [[🔌 ZimerfeldCommitMsgPlugin (ES)|ZimerfeldCommitMsgPlugin]]

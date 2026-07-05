---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [fluxo, i18n, dropdown, auto-refresh, etapa3]
---

# 🌐 Flow: Language and auto-refresh

> 🇧🇷 Leia esta página em português → [[🌐 3 - Idioma e auto-refresh]]

How the user picks the language and how the message stays up to date while the dialog is open.

## 🗣️ Two ways to pick the language

**1. Commit screen template dropdown** — three items (quick per-commit choice):
```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```
**2. Settings → Plugins → ZimerfeldCommitMsg** — the **"Idioma da mensagem / Message language"** selector sets the **default** (used by the Plugins menu and by the auto-refresh when no dropdown item has been chosen).

| Option | Behavior |
|---|---|
| `Automático/Automatic` | **Default.** Detects via the OS (`pt-*` → Portuguese; anything else → English) |
| `Português/Portuguese` | Forces pt-BR |
| `Inglês/English` | Forces English |

## 🔎 How the dropdown choice is detected

```
Host opens the template dropdown
        │  invokes the generation Func of ALL items, in sequence
        │  (there is NO click callback — the host materializes each item's text)
        ▼
GenerateForTemplate(forced)  → generates in the item's language + RememberTemplateMessage(msg → language)
        │
        ▼
User clicks an item  → host applies the materialized text to the box (ReplaceMessage)
        │
        ▼
Box TextChanged  → DetectTemplateSelection: did the box become exactly a known text?
        │  if yes → pins _sessionLanguage to that item's language
        ▼
Auto-refresh now uses EffectiveLanguage() = _sessionLanguage (priority over setting/OS)
```

> [!note] Why the detection is indirect
> The host's template API gives **no** click callback: it calls the `Func` of **all** items when the dropdown **opens** and, on click, only applies the already materialized text. If the plugin pinned the language inside the `Func`, it would run for every item and always pin the last one. So the plugin **records each generated text** (msg → language) and recognizes the choice by watching the box (`TextChanged`). See [[🔌 Integração via template dropdown e Application.Idle (EN)|Integration via template dropdown and Application.Idle]].

## 🔄 Auto-refresh on stage/unstage

With the dialog open, when files are staged/unstaged, the next `Application.Idle` regenerates the message in the effective language (`EffectiveLanguage()`), as long as the box is still "ours" (not manually edited). See [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]].

## 🗣️ Effective language

```
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()
CurrentLanguage()   = setting (Automatic/PT/EN) → MessageLanguageResolver.Resolve
                      "Automatic" → FromCulture(CultureInfo.CurrentUICulture)
```

## 🔗 Links

- [[🔍 1 - Detecção do diálogo de commit (EN)|1 - Commit dialog detection]]
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]
- [[🌐 Localization (EN)|Localization]]
- [[🔌 ZimerfeldCommitMsgPlugin (EN)|ZimerfeldCommitMsgPlugin]]

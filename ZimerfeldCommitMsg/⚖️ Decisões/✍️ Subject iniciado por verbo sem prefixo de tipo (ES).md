---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [decisao, adr, conventional-commits, mensagem, verbo]
status: aceita
---

# ✍️ ADR — Subject iniciado por verbo, sin el prefijo literal "tipo:"

> 🇧🇷 Portugués → [[✍️ Subject iniciado por verbo sem prefixo de tipo]] · 🇺🇸 English → [[✍️ Subject iniciado por verbo sem prefixo de tipo (EN)]]

## 🎯 Contexto
El estándar **Conventional Commits** define tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) normalmente escritos como prefijo literal: `feat: add authentication`. El plugin clasifica los cambios según esos tipos, pero debe decidir **cómo** aparece eso en el mensaje.

## ✅ Decisión
Usar el tipo de Conventional Commits para **elegir el verbo**, y **no** para prefijar el mensaje con `tipo:`. El subject empieza por el **verbo** correspondiente (3.ª persona del presente en pt-BR / imperativo en inglés). El tipo en sí solo aparece, a modo de pista, entre paréntesis en la línea-resumen consolidada.

Ejemplos:
- `feat` → `Adiciona autenticação` / `Add authentication`
- `fix` → `Corrige …` / `Fix …`
- Resumen consolidado: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`

## 🔀 Alternativas consideradas
- **Prefijo literal `feat:`** — canónico de CC, pero ruidoso y menos natural en pt-BR; muchos equipos ya generan ese prefijo mediante herramientas/hooks.
- **Solo verbo, sin tipos (elegida)** — mensaje legible y natural, guiado por la misma clasificación semántica; los tipos quedan visibles solo en el resumen entre paréntesis.

## ⚖️ Consecuencias
**Positivas:**
- Mensajes naturales y multilingües (pt-BR / inglés / español), sin contaminación de prefijos.
- Aún **guiados** por la clasificación de Conventional Commits (el verbo lleva la intención).

**Negativas / trade-offs:**
- No es el formato literal de CC — quien exija `type:` en el subject debe ajustarse (el tipo aparece solo en el resumen).
- La calidad depende del mapeo tipo → verbo en el `LanguagePack`.

## 🔗 Relacionado
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]

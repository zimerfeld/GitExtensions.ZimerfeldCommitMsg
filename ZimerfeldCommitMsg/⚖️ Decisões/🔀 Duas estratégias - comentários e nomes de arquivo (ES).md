---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [decisao, adr, extração, comentários, nomes-de-arquivo]
status: aceita
---

# 🔀 ADR — Dos estrategias: comentarios del diff + nombres de archivo

> 🇧🇷 Portugués → [[🔀 Duas estratégias - comentários e nomes de arquivo]] · 🇺🇸 English → [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)]]

## 🎯 Contexto
Para describir **qué** cambió, el generador necesita una fuente de contenido. Los generadores simples usan solo nombres de archivo, lo que produce mensajes genéricos ("update Service"). Por otro lado, no todo diff trae comentarios útiles.

## ✅ Decisión
Usar **dos estrategias complementarias**, con prioridad:
1. **Comentarios del diff (principal):** leer las líneas `+`/`-` del `git diff --cached` y extraer comentarios en varias sintaxis (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). El mejor comentario por archivo se traduce (para pt-BR/es-ES), se sanea y se recorta a la cláusula principal.
2. **Nombres de archivo (fallback):** cuando no hay comentario útil, derivar el **concepto** del nombre eliminando sufijos semánticos (`Service`, `Controller`, `Tests`, …) y traduciendo palabras.

Ambas se apoyan en el **vocabulario por repositorio** (`.zimerfeldcommitmsg.json`) para términos del dominio. Ver [[📓 Vocabulário por repositório (ES)|Vocabulario por repositorio]].

## 🔀 Alternativas consideradas
- **Solo nombres de archivo** — barato, pero genérico y poco informativo.
- **Solo comentarios** — rico cuando existen, pero falla en diffs sin comentarios.
- **Dos estrategias con fallback (elegida)** — cubre ambos casos; se privilegia el contenido real del diff, con un piso derivado de los nombres.

## ⚖️ Consecuencias
**Positivas:**
- Mensajes más informativos cuando el dev comenta el código.
- Nunca se queda sin contenido — siempre hay al menos la línea-resumen y un bullet.

**Negativas / trade-offs:**
- La extracción de comentarios exige un **saneamiento** robusto (delimitadores balanceados, conectores sueltos) para no inyectar ruido.
- La traducción EN→PT/ES es heurística (diccionarios), no perfecta.

## 🔗 Relacionado
- [[📓 Vocabulário por repositório (ES)|Vocabulario por repositorio]]
- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación del mensaje a partir del diff]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]

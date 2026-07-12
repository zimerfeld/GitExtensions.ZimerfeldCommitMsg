---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [conhecimento, diff, extração, comentários, saneamento, tradução]
---

# ⚙️ Geração de mensagem a partir do diff

> 🇧🇷 Portugués → [[⚙️ Geração de mensagem a partir do diff]] · 🇺🇸 English → [[⚙️ Geração de mensagem a partir do diff (EN)]]

## 📝 Resumen
Cómo el [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] transforma el `git diff --cached` en un mensaje de commit: clasificación → subject consolidado → cuerpo en viñetas, con extracción de comentarios, derivación de conceptos, saneamiento y traducción.

## 🔄 Pipeline

```
git diff --cached
   │
   ▼
FileChange[]  (path, +/- líneas)
   │
   ├─ categoría por extensión (source/web/docs/build/config)
   ├─ tipo Conventional Commits (dominante primero)
   │
   ├─ SUBJECT: [prefijo de contexto] - <verbo> N archivos (tipos)
   │      prefijo = concepto del archivo de mayor impacto (ej.: OverlayController → Overlay)
   │
   └─ CUERPO: hasta 5 viñetas, una por archivo de mayor impacto
          estrategia 1: mejor comentario del diff (traducido, saneado, recortado)
          estrategia 2 (fallback): concepto del nombre del archivo
```

## 💬 Extracción de comentarios

`ExtractCommentText` reconoce:
- `//`, `///` — C#, Java, JS, TS, Go, Rust, C/C++…
- `/* */`, `/** */`, `/*! */` — bloque C-style en una línea
- `* ` — continuación JSDoc/Javadoc (fuera de `.md`, donde `*` es viñeta)
- `<!-- -->` — HTML/XML
- `--` — SQL, Lua, Haskell, Ada
- `'` — VB/VBScript (fuera de `.md`)
- `#` — Python, Shell, YAML, Ruby (en `.md` es heading, ignorado)

## 🧹 Saneamiento (`CleanCommentText`)

Descarta:
- **Separadores visuales** y código comentado.
- **Delimitadores desbalanceados** — paréntesis/corchetes/llaves sin casar; comillas `"`/`` ` ``/`'` en número impar (ignora apóstrofes de contracción como `don't`, `it's`); `<`/`>` en número desigual.
- Frases terminadas en **palabra de enlace suelta** (`de`, `para`, `que`…) — comentario cortado.

Entre los candidatos válidos, elige el de **mejor calidad** (más descriptivo), no el más largo.

## 🌐 Traducción EN→PT/ES

Cuando la salida es pt-BR o es-ES, frases y verbos se traducen mediante diccionarios (los más largos primero), con **protección** de slugs de rama gitflow (`feature/…`) y tipos CC vía regex — para no corromper `feature/search` en `feature/buscar`. En inglés, los comentarios pasan intactos.

## 🔡 Derivación de conceptos (fallback)

`SemanticSuffixes` elimina sufijos como `Service`, `Controller`, `Repository`, `Tests`, `ViewModel`, … para llegar al concepto del nombre. Ej.: `OverlayController` → `Overlay`. El vocabulario conocido/rechazado y las frases de concepto vienen de los defaults + [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]].

## ✅ Garantías

- **Nunca vacío:** habiendo stage, siempre al menos la línea-resumen.
- **Siempre al menos una viñeta**, incluso con un único archivo.
- **README en stage:** el título (`#`) sirve de fallback descriptivo.

## 🔗 Relacionado
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias: comentarios y nombres de archivo]]
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]

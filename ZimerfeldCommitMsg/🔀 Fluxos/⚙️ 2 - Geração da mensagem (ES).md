---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [fluxo, geração, diff, conventional-commits, etapa2]
---

# ⚙️ Flujo: Generación del mensaje

> 🇧🇷 Portugués → [[⚙️ 2 - Geração da mensagem]] · 🇺🇸 English → [[⚙️ 2 - Geração da mensagem (EN)]]

Lo que hace `CommitMessageGenerator.Generate()` para convertir el diff staged en un mensaje.

## 🪜 Pasos

```
Generate()
        │
        ├─ 1. Lee el diff staged: `git diff --cached` en el working dir
        │       (nada en stage → devuelve solo la línea-resumen con el prefijo de contexto)
        │
        ├─ 2. Clasifica cada archivo:
        │       extensión → categoría (source/web/docs/build/config)
        │       + heurística de contenido → tipo Conventional Commits
        │       (feat/fix/docs/test/chore/build/refactor)
        │
        ├─ 3. DetermineAllTypes(changes) → lista de tipos implicados (dominante primero)
        │
        ├─ 4. Subject consolidado: BuildConsolidatedTitle(tipoDominante, changes, tipos)
        │       = [prefijo de contexto] - <verbo del tipo> N archivos (tipo1, tipo2, …)
        │       ej.: "Overlay - Adiciona 10 arquivos (feat, fix, docs)"
        │
        ├─ 5. Cuerpo: BuildBody(changes, comentariosPorArchivo, tituloReadme)
        │       hasta 5 bullets, uno por archivo de mayor impacto, cada uno la mejor
        │       frase de comentario del diff (o el concepto del nombre del archivo)
        │
        └─ 6. Nunca vacío: si hay stage, siempre al menos la línea-resumen
```

## 🔎 Detalles

- **Verbo por tipo (Conventional Commits):** el tipo elige el **verbo**, no se convierte en prefijo. `feat`→"Adiciona/Add", `fix`→"Corrige/Fix", `docs`→"Documenta/Document", etc. El tipo **no** aparece en el mensaje (solo entre paréntesis en el resumen, como pista). Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo sin prefijo de tipo]].
- **Prefijo de contexto:** 1-3 palabras derivadas del **concepto** del archivo de mayor impacto (ej.: `OverlayController` → `Overlay`), para que el título nunca sea genérico. `null` cuando ningún archivo produce un concepto legible.
- **Estrategia principal (comentarios):** `ExtractCommentsByFile()` lee las líneas `+`/`-` del diff y capta comentarios (`//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, SQL/Lua `--`, VB `'`, `#`). Cada archivo elige el **mejor** comentario (traducido al idioma de salida cuando aplica, saneado, recortado en la cláusula principal).
- **Estrategia de reserva (nombres de archivo):** cuando no hay comentario útil, deriva el **concepto** del nombre eliminando sufijos semánticos (`Service`, `Controller`, `Tests`, …) y traduciendo palabras.
- **Saneamiento:** descarta comentarios con **delimitadores desbalanceados** (`()`, `[]`, `{}`, comillas, `<>`) o terminados en **palabra de enlace suelta**; entre los válidos, elige el de mejor calidad (no el más largo).
- **README:** si el `README.md` está en stage, el título (primera línea `#`) se usa como reserva descriptiva.

Ver [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]].

## 🌐 Ejemplo (pt-BR / EN)

```
Implementa autenticação
- Adiciona autenticação
- Adiciona processamento de pagamento
- Adiciona gerenciamento de token
```
```
Implement authentication
- Add authentication
- Add payment processing
- Add token management
```

## 🔗 Enlaces

- [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]]
- [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]]
- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]

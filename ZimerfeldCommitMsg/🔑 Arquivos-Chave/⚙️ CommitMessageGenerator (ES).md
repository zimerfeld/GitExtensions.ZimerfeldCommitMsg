---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquivo, gerador, diff, conventional-commits, i18n]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs
---

# ⚙️ CommitMessageGenerator.cs

> 🇧🇷 Portugués → [[⚙️ CommitMessageGenerator]] · 🇺🇸 English → [[⚙️ CommitMessageGenerator (EN)]]

El **motor** del plugin: convierte el `git diff --cached` en un mensaje de commit (subject + cuerpo). ~1237 líneas, en su mayoría funciones puras testeables (`internal`, expuestas por `InternalsVisibleTo`).

**Ruta:** `src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs`

---

## 🏗️ Construcción

```csharp
new CommitMessageGenerator(workingDir, language).Generate()
```
Carga el [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]] (`.zimerfeldcommitmsg.json`) del `workingDir` y el `LanguagePack` del idioma. Ver [[🌐 Localization (ES)|Localization]].

---

## 🗃️ Tablas estáticas

- **`ExtCategory`** — extensión → categoría semántica (`source`, `web`, `docs`, `build`, `config`). Ej.: `cs/py/go/rs` → source; `ts/tsx/html/css` → web; `md/txt` → docs; `csproj/sln` → build; `json/yaml/ini` → config.
- **`SemanticSuffixes`** — sufijos eliminados para llegar al **concepto** del nombre (los más largos primero): `ServiceTests`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator`, `Tests`, `Dto`, `ViewModel`, … Así `OverlayController` → `Overlay`.
- **`PhraseTranslations`** / diccionarios de palabras — traducción inglés → pt-BR de frases y verbos, aplicada cuando la salida es pt-BR (las más largas primero). El español tiene sus equivalentes propios: `TranslateToSpanish` con `WordTranslationsEs`, `PhraseTranslationsEs` y `ConceptWordEs`, aplicados cuando la salida es es-ES.
- Protección de **branches gitflow** y **tipos CC** por regex — evita traducir palabra por palabra slugs como `feature/search`.

---

## 🔄 `Generate()` — pipeline

1. Lee el diff staged (`git diff --cached`). Sin stage → devuelve solo la línea-resumen (con el prefijo de contexto).
2. `DetermineAllTypes(changes)` → lista de tipos Conventional Commits (dominante primero).
3. `BuildConsolidatedTitle(tipo, changes, tipos)` → subject: `[prefijo] - <verbo> N archivos (tipos)`.
4. `ExtractCommentsByFile()` → comentarios alterados por archivo.
5. `BuildBody(changes, comentarios, tituloReadme)` → hasta 5 bullets.
6. **Nunca vacío:** si hay stage, al menos la línea-resumen.

---

## 🔑 Métodos clave

| Método | Papel |
|---|---|
| `BuildConsolidatedTitle` | subject consolidado: verbo del tipo dominante + total + lista de tipos |
| `BuildContextPrefix` | 1-3 palabras del concepto del archivo de mayor impacto (título nunca genérico) |
| `TypeVerb` | verbo del tipo CC en el idioma (3ª persona pt/es · imperativo en) |
| `ConceptToNominal` | convierte el concepto en el prefijo nominal por idioma; pt-BR y es-ES usan el nombre de acción (ej.: "Eliminación de documento de texto"), mientras que en inglés se humaniza el concepto |
| `CommentFilePriority` | ordena archivos por impacto (source > web > … > docs) |
| `ExtractCommentsByFile` | lee líneas `+`/`-` del diff y agrupa comentarios por archivo |
| `ExtractCommentText` | reconoce sintaxis: `//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, `--`, `'`, `#` |
| `CleanCommentText` | descarta separadores visuales, código comentado y tags; valida delimitadores balanceados |
| `BuildBody` | elige el mejor comentario por archivo, traduce, sanea, recorta la cláusula principal |

---

## 🧹 Saneamiento (por qué evita ruido)

- **Delimitadores balanceados:** paréntesis/corchetes/llaves emparejados; comillas `"`/`` ` ``/`'` en número par (ignora apóstrofes de contracción como `don't`); `<`/`>` en igual número. Detecta el caso "abre y no cierra".
- **Enlace suelto al final:** descarta frases terminadas en `de`, `para`, `que`, … (comentario cortado).
- **Mejor calidad, no la más larga:** entre candidatos válidos, elige el más descriptivo — no simplemente el más largo.

---

## 🧪 Tests

`CommentExtractionTests`, `ConceptExtractionTests`, `TranslationTests`, `RepoVocabularyConfigTests` cubren las funciones puras de arriba. Ver [[🏷️ Versionamento (ES)|Versionado]].

---

## 🔗 Enlaces

- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]]
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]
- [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]] · [[🌐 Localization (ES)|Localization]]
- [[⚙️ 2 - Geração da mensagem (ES)|2 - Generación del mensaje]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias - comentarios y nombres de archivo]]

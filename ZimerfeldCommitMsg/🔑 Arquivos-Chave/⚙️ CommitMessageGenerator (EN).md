---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquivo, gerador, diff, conventional-commits, i18n]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs
---

# ⚙️ CommitMessageGenerator.cs

> 🇧🇷 Leia esta página em português → [[⚙️ CommitMessageGenerator]]

The plugin's **engine**: turns `git diff --cached` into a commit message (subject + body). ~1237 lines, mostly pure testable functions (`internal`, exposed via `InternalsVisibleTo`).

**Path:** `src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs`

---

## 🏗️ Construction

```csharp
new CommitMessageGenerator(workingDir, language).Generate()
```
Loads the [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]] (`.zimerfeldcommitmsg.json`) from the `workingDir` and the language's `LanguagePack`. See [[🌐 Localization (EN)|Localization]].

---

## 🗃️ Static tables

- **`ExtCategory`** — extension → semantic category (`source`, `web`, `docs`, `build`, `config`). E.g.: `cs/py/go/rs` → source; `ts/tsx/html/css` → web; `md/txt` → docs; `csproj/sln` → build; `json/yaml/ini` → config.
- **`SemanticSuffixes`** — suffixes removed to reach the name's **concept** (longest first): `ServiceTests`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator`, `Tests`, `Dto`, `ViewModel`, … So `OverlayController` → `Overlay`.
- **`PhraseTranslations`** / word dictionaries — English → pt-BR translation of phrases and verbs, applied when the output is pt-BR (longest first).
- **Gitflow branch** and **CC type** protection via regex — avoids word-by-word translation of slugs like `feature/search`.

---

## 🔄 `Generate()` — pipeline

1. Reads the staged diff (`git diff --cached`). Nothing staged → returns only the summary line (with the context prefix).
2. `DetermineAllTypes(changes)` → list of Conventional Commits types (dominant first).
3. `BuildConsolidatedTitle(type, changes, types)` → subject: `[prefix] - <verb> N files (types)`.
4. `ExtractCommentsByFile()` → changed comments per file.
5. `BuildBody(changes, comments, readmeTitle)` → up to 5 bullets.
6. **Never empty:** with something staged, at least the summary line.

---

## 🔑 Key methods

| Method | Role |
|---|---|
| `BuildConsolidatedTitle` | consolidated subject: dominant type's verb + total + list of types |
| `BuildContextPrefix` | 1-3 words from the highest-impact file's concept (title never generic) |
| `TypeVerb` | the CC type's verb in the language (3rd person pt / imperative en) |
| `CommentFilePriority` | ranks files by impact (source > web > … > docs) |
| `ExtractCommentsByFile` | reads the diff's `+`/`-` lines and groups comments per file |
| `ExtractCommentText` | recognizes syntaxes: `//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, `--`, `'`, `#` |
| `CleanCommentText` | discards visual separators, commented-out code and tags; validates balanced delimiters |
| `BuildBody` | picks the best comment per file, translates, sanitizes, trims to the main clause |

---

## 🧹 Sanitization (why it avoids noise)

- **Balanced delimiters:** matched parentheses/brackets/braces; quotes `"`/`` ` ``/`'` in even count (ignores contraction apostrophes like `don't`); `<`/`>` in equal count. Catches the "opens and never closes" case.
- **Dangling connective at the end:** discards sentences ending in `de`, `para`, `que`, … (truncated comment).
- **Best quality, not longest:** among valid candidates, picks the most descriptive — not simply the longest.

---

## 🧪 Tests

`CommentExtractionTests`, `ConceptExtractionTests`, `TranslationTests`, `RepoVocabularyConfigTests` cover the pure functions above. See [[🏷️ Versionamento (EN)|Versioning]].

---

## 🔗 Links

- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]
- [[📜 Conventional Commits - Conceitos (EN)|Conventional Commits - Concepts]]
- [[📓 RepoVocabularyConfig (EN)|RepoVocabularyConfig]] · [[🌐 Localization (EN)|Localization]]
- [[⚙️ 2 - Geração da mensagem (EN)|2 - Message generation]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (EN)|Two strategies - comments and file names]]

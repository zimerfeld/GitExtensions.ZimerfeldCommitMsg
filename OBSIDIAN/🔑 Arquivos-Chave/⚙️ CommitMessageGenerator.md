---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquivo, gerador, diff, conventional-commits, i18n]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs
---

# ⚙️ CommitMessageGenerator.cs

> 🇺🇸 Read this page in English → [[⚙️ CommitMessageGenerator (EN)]]

O **motor** do plugin: transforma o `git diff --cached` em uma mensagem de commit (subject + corpo). ~1237 linhas, em sua maioria funções puras testáveis (`internal`, expostas por `InternalsVisibleTo`).

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs`

---

## 🏗️ Construção

```csharp
new CommitMessageGenerator(workingDir, language).Generate()
```
Carrega o [[📓 RepoVocabularyConfig]] (`.zimerfeldcommitmsg.json`) do `workingDir` e o `LanguagePack` do idioma. Ver [[🌐 Localization]].

---

## 🗃️ Tabelas estáticas

- **`ExtCategory`** — extensão → categoria semântica (`source`, `web`, `docs`, `build`, `config`). Ex.: `cs/py/go/rs` → source; `ts/tsx/html/css` → web; `md/txt` → docs; `csproj/sln` → build; `json/yaml/ini` → config.
- **`SemanticSuffixes`** — sufixos removidos para chegar ao **conceito** do nome (mais longos primeiro): `ServiceTests`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator`, `Tests`, `Dto`, `ViewModel`, … Assim `OverlayController` → `Overlay`.
- **`PhraseTranslations`** / dicionários de palavras — tradução inglês → pt-BR de frases e verbos, aplicada quando a saída é pt-BR (mais longas primeiro).
- Proteção de **branches gitflow** e **tipos CC** por regex — evita traduzir palavra-a-palavra slugs como `feature/search`.

---

## 🔄 `Generate()` — pipeline

1. Lê o diff staged (`git diff --cached`). Sem stage → devolve só a linha-resumo (com prefixo de contexto).
2. `DetermineAllTypes(changes)` → lista de tipos Conventional Commits (dominante primeiro).
3. `BuildConsolidatedTitle(tipo, changes, tipos)` → subject: `[prefixo] - <verbo> N arquivos (tipos)`.
4. `ExtractCommentsByFile()` → comentários alterados por arquivo.
5. `BuildBody(changes, comentários, tituloReadme)` → até 5 bullets.
6. **Nunca vazio:** havendo stage, ao menos a linha-resumo.

---

## 🔑 Métodos-chave

| Método | Papel |
|---|---|
| `BuildConsolidatedTitle` | subject consolidado: verbo do tipo dominante + total + lista de tipos |
| `BuildContextPrefix` | 1-3 palavras do conceito do arquivo de maior impacto (título nunca genérico) |
| `TypeVerb` | verbo do tipo CC no idioma (3ª pessoa pt / imperativo en) |
| `CommentFilePriority` | ranqueia arquivos por impacto (source > web > … > docs) |
| `ExtractCommentsByFile` | lê linhas `+`/`-` do diff e agrupa comentários por arquivo |
| `ExtractCommentText` | reconhece sintaxes: `//`, `///`, `/* */`, `/** */`, JSDoc `* `, `<!-- -->`, `--`, `'`, `#` |
| `CleanCommentText` | descarta separadores visuais, código comentado e tags; valida delimitadores balanceados |
| `BuildBody` | escolhe o melhor comentário por arquivo, traduz, saneia, recorta a cláusula principal |

---

## 🧹 Saneamento (por que evita ruído)

- **Delimitadores balanceados:** parênteses/colchetes/chaves casados; aspas `"`/`` ` ``/`'` em número par (ignora apóstrofes de contração `don't`); `<`/`>` em igual número. Pega o caso "abre e não fecha".
- **Ligação solta no fim:** descarta frases terminadas em `de`, `para`, `que`, … (comentário cortado).
- **Melhor qualidade, não maior:** entre candidatos válidos, escolhe o mais descritivo — não simplesmente o mais longo.

---

## 🧪 Testes

`CommentExtractionTests`, `ConceptExtractionTests`, `TranslationTests`, `RepoVocabularyConfigTests` cobrem as funções puras acima. Ver [[🏷️ Versionamento]].

---

## 🔗 Ligações

- [[⚙️ Geração de mensagem a partir do diff]]
- [[📜 Conventional Commits - Conceitos]]
- [[📓 RepoVocabularyConfig]] · [[🌐 Localization]]
- [[⚙️ 2 - Geração da mensagem]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo]]

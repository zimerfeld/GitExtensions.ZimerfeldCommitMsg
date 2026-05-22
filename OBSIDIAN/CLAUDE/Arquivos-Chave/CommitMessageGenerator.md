---
tipo: arquivo
tags: [arquivo, gerador, núcleo, c#]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs
linhas: 987
atualizado: 2026-05-22
---

# CommitMessageGenerator.cs

Núcleo do plugin. `internal sealed class` com um único campo de instância: `_workingDir`.

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/CommitMessageGenerator.cs`

---

## API Pública

### `Generate() → string`
Único ponto de entrada. Retorna a mensagem de commit completa ou `string.Empty` se não há staged changes.

---

## Métodos por responsabilidade

### Git I/O
| Método | Linha aprox. | Descrição |
|---|---|---|
| `RunGit(params string[])` | ~963 | Executa `git` como subprocess com timeout 8s |

### Parsing
| Método | Linha | Descrição |
|---|---|---|
| `ParseChanges(string)` | ~659 | Parseia `diff --name-status` → `List<FileChange>` |

### Classificação de tipos
| Método | Linha | Descrição |
|---|---|---|
| `DetermineType(List<FileChange>)` | ~676 | Retorna o type dominante (legacy — ainda existente) |
| `DetermineAllTypes(List<FileChange>)` | ~698 | **Novo.** Retorna todos os types envolvidos, ordenados |

### Extração de comentários
| Método | Linha | Descrição |
|---|---|---|
| `ExtractDiffComments()` | ~451 | Lê diff, rankeia comentários por prioridade de arquivo |
| `CommentFilePriority(string)` | ~499 | Prioridade 0–4 por categoria de arquivo |
| `ExtractCommentText(string)` | ~521 | Detecta `//`, `///`, `#` em linhas de diff |
| `CleanCommentText(string)` | ~543 | Filtra separadores, tags XML, código comentado |

### Tradução
| Método | Linha | Descrição |
|---|---|---|
| `IsEnglishText(string)` | ~601 | ≥25% das palavras no dicionário → inglês |
| `TranslateToPortuguese(string)` | ~616 | Frases → padrões estruturais → palavras individuais |

### Construção da mensagem
| Método | Linha | Descrição |
|---|---|---|
| `ReadStagedReadmeTitle(List<FileChange>)` | ~420 | Lê `# Título` do README staged |
| `ExtractMainClause(string)` | ~571 | Corta comentário no primeiro conector de propósito |
| `NormalizeDesc(string)` | ~589 | 1ª letra minúscula (exceto acrônimos) |
| `BuildSubject(string, List<FileChange>)` | ~697 | Verbo + frase funcional pt-BR (fallback) |
| `BuildBody(List<FileChange>)` | ~723 | Frase de camadas arquiteturais |
| `BuildFunctionalPhrase(List<FileChange>)` | ~813 | Conceito mais dominante como frase |
| `TruncateTitle(string, int=72)` | ~924 | Trunca no último espaço com `…` |

### Extração de conceitos
| Método | Linha | Descrição |
|---|---|---|
| `ExtractUniqueConcepts(List<FileChange>)` | ~760 | Frequência de conceitos por filename |
| `ExtractRawConcept(string)` | ~778 | Remove prefixo I + sufixo arquitetural |
| `MapConcept(string)` | ~831 | Conceito → frase pt-BR (via `ConceptPhrases`) |
| `FallbackPhrase(List<FileChange>)` | ~834 | Frase genérica por categoria |

### Camadas arquiteturais
| Método | Linha | Descrição |
|---|---|---|
| `ExtractArchLayerNames(List<FileChange>)` | ~856 | Detecta camadas por sufixo de filename |

### Helpers genéricos
| Método | Linha | Descrição |
|---|---|---|
| `GetCategory(string)` | ~884 | Extensão → categoria (source/web/docs/build/config) |
| `IsTestPath(FileChange)` | ~898 | Detecta pastas/sufixos de teste |
| `HumanizeName(string)` | ~912 | PascalCase → palavras minúsculas (preserva acrônimos) |
| `JoinPhrases(List<string>)` | ~931 | "a, b e c" em português |
| `GetCommonDirectory(List<string>)` | ~939 | Diretório comum de um conjunto de paths |

---

## Dicionários e tabelas estáticas

### ExtCategory (~L21)
Mapeia extensão de arquivo → categoria semântica:
- `source`: cs, vb, fs, cpp, c, h, java, py, rb, go, rs, kt, swift, php, dart, lua, scala
- `web`: js, ts, jsx, tsx, vue, svelte, html, htm, razor, cshtml, css, scss, less, sass
- `docs`: md, txt, rst, adoc
- `build`: csproj, vbproj, fsproj, sln, props, targets, dockerfile, makefile
- `config`: json, xml, yaml, yml, toml, ini, env, config, conf, lock, editorconfig

### SemanticSuffixes (~L47)
48 sufixos arquiteturais removidos do filename para chegar ao conceito de domínio. Ordenados do mais longo ao mais curto (longest-match first).

### ConceptPhrases (~L64)
~80 entradas: conceito PascalCase → frase pt-BR.
Agrupados por domínio: identidade/acesso, usuário, comércio, comunicação, infraestrutura, dados, relatórios, busca, config, API, testes, docs.

### ArchLayers (~L179)
~18 entradas: sufixo arquitetural → nome da camada em pt-BR.

### PhraseTranslations (~L194)
~25 frases compostas en→pt-BR (longest first para evitar fragmentação).

### WordTranslations (~L231)
~120 entradas: verbos, advérbios, preposições, substantivos técnicos, adjetivos.

---

## Record auxiliar

```csharp
internal sealed record FileChange(char Status, string Path);
```

---

## Relacionado

- [[../Fluxos/Geração da Mensagem]]
- [[../Sistema/Arquitetura]]
- [[../Decisoes/Título como Lista de Types]]
- [[../Decisoes/Prioridade de Comentários]]

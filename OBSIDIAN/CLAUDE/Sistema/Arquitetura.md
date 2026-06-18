---
tipo: sistema
tags: [arquitetura, classes, design, i18n]
atualizado: 2026-06-05
---

# Arquitetura

## Diagrama de classes

```
GitExtensions (host)
    │
    │  MEF (System.ComponentModel.Composition)
    ▼
ZimerfeldCommitMsgPlugin   ← [Export(IGitPlugin)]
    │  ChoiceSetting idioma → CurrentLanguage() → MessageLanguage
    │  instancia em cada chamada com (workingDir, lang)
    ▼
CommitMessageGenerator ──uses──► LanguagePack (PtBr | En)   ← mapas por idioma
    │                       └──► Strings (.resx PT/EN)        ← strings de UI
    │  chama
    ▼
git subprocess (diff --cached --name-status / diff --cached --no-color)
```

> Camada **`Localization/`**: `MessageLanguage` (enum + resolvedor SO/override), `LanguagePack`
> (abstrata + PtBr/En), `Strings` (acessor `.resx`). Ver [[../Decisoes/Suporte Multilíngue PT-EN]].

## ZimerfeldCommitMsgPlugin

Ponto de entrada. Herda de `GitPluginBase`, exportado via MEF.

**Responsabilidades:**
- Registrar **três** templates `"Zimerfeld Commit Msg — <Auto/PT/EN>"` (com ícone) no dropdown do `FormCommit`; cada item força um idioma (`null`=Auto). O host gera os 3 ao **abrir** o menu; a escolha é detectada pelo `TextChanged` da caixa (`DetectTemplateSelection`), que fixa `_sessionLanguage` → [[../Fluxos/Template Dropdown (Auto-resumo)]]
- Ouvir `PostRepositoryChanged` → disparar [[../Fluxos/Stage Trigger]]
- Implementar `Execute()` → acionar via menu Plugins
- Encontrar o `TextBoxBase` do `FormCommit` aberto (busca por nome e por fallback área)
- Carregar o ícone do plugin de `Resources/icon.png` embutido (`LoadIcon()` → `PluginIcon`)

**Thread safety:** captura `SynchronizationContext` no `Register()` e usa `_syncContext.Post()` para sempre atualizar a UI na thread correta.

## CommitMessageGenerator

Núcleo. `internal sealed class`, sem estado global — instanciado a cada geração com
`(workingDir, MessageLanguage)`. O idioma seleciona o `LanguagePack` (`_lang`) usado em toda a montagem.

**Método público:** `Generate() → string`

**Pipeline interno:**
1. `RunGit("diff --cached --name-status")` → lista de arquivos
2. `ParseChanges()` → `List<FileChange>`
3. `DetermineAllTypes()` → `List<string>` de CC types
4. `ExtractDiffComments()` → comentários ranqueados do diff
5. `TranslateToPortuguese()` → **só quando idioma = pt-BR**; em inglês os comentários passam intactos
6. `ReadStagedReadmeTitle()` → título do README se staged
7. Montagem via `_lang`: subject (`FormatTitle`/`TypeVerb`) + corpo em **bullets** (`BuildBody`/`StatusVerb`)

## Dados imutáveis (static readonly)

Mapas específicos de idioma migraram para `LanguagePack` (`ConceptPhrases`, conjugação de verbos,
conectores, fallbacks). `ArchLayers`/`ExtractArchLayerNames`/`JoinPhrases` foram **removidos** (mortos
após o corpo virar bullets). Permanecem no gerador:

| Campo | Tipo | Uso |
|---|---|---|
| `ExtCategory` | `Dictionary<string,string>` | extensão → categoria semântica |
| `SemanticSuffixes` | `string[]` | sufixos arquiteturais a remover |
| `PhraseTranslations` | `(string,string)[]` | frases compostas en→pt-BR (caminho pt-BR) |
| `WordTranslations` | `Dictionary<string,string>` | palavras individuais en→pt-BR (caminho pt-BR) |
| `EnglishWords` | `HashSet<string>` | detecção de idioma inglês do comentário |
| `PreservePattern` | `const string` (regex) | branches gitflow + tipos CC preservados na tradução |

## Relacionado

- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]]
- [[../Fluxos/Geração da Mensagem]]
- [[../Fluxos/Template Dropdown (Auto-resumo)]]
- [[../Decisoes/Suporte Multilíngue PT-EN]]
- [[../Decisoes/Preservação de Branches e Tipos CC]]

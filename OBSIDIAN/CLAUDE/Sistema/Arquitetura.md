---
tipo: sistema
tags: [arquitetura, classes, design]
atualizado: 2026-05-22
---

# Arquitetura

## Diagrama de classes

```
GitExtensions (host)
    │
    │  MEF (System.ComponentModel.Composition)
    ▼
ZimerfeldCommitMsgPlugin   ← [Export(IGitPlugin)]
    │  instancia em cada chamada
    ▼
CommitMessageGenerator
    │  chama
    ▼
git subprocess (diff --cached --name-status / diff --cached --no-color)
```

## ZimerfeldCommitMsgPlugin

Ponto de entrada. Herda de `GitPluginBase`, exportado via MEF.

**Responsabilidades:**
- Registrar template `"Zimerfeld: Auto-resumo"` no dropdown do `FormCommit`
- Ouvir `PostRepositoryChanged` → disparar [[../Fluxos/Stage Trigger]]
- Implementar `Execute()` → acionar via menu Plugins
- Encontrar o `TextBoxBase` do `FormCommit` aberto (busca por nome e por fallback área)

**Thread safety:** captura `SynchronizationContext` no `Register()` e usa `_syncContext.Post()` para sempre atualizar a UI na thread correta.

## CommitMessageGenerator

Núcleo. `internal sealed class`, sem estado global — instanciado a cada geração com o `workingDir`.

**Método público:** `Generate() → string`

**Pipeline interno:**
1. `RunGit("diff --cached --name-status")` → lista de arquivos
2. `ParseChanges()` → `List<FileChange>`
3. `DetermineAllTypes()` → `List<string>` de CC types (branch `feature/titulo`)
4. `ExtractDiffComments()` → comentários ranqueados do diff
5. `TranslateToPortuguese()` → tradução en→pt-BR
6. `ReadStagedReadmeTitle()` → título do README se staged
7. Montagem: header = types, body = desc + contexto arquitetural

## Dados imutáveis (static readonly)

| Campo | Tipo | Uso |
|---|---|---|
| `ExtCategory` | `Dictionary<string,string>` | extensão → categoria semântica |
| `SemanticSuffixes` | `string[]` | sufixos arquiteturais a remover |
| `ConceptPhrases` | `Dictionary<string,string>` | conceito → frase pt-BR |
| `ArchLayers` | `Dictionary<string,string>` | sufixo → camada arquitetural |
| `PhraseTranslations` | `(string,string)[]` | frases compostas en→pt-BR |
| `WordTranslations` | `Dictionary<string,string>` | palavras individuais en→pt-BR |
| `EnglishWords` | `HashSet<string>` | detecção de idioma inglês |

## Relacionado

- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]]
- [[../Fluxos/Geração da Mensagem]]

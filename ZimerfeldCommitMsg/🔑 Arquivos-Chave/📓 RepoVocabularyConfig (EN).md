---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [arquivo, config, vocabulário, json, por-repositório]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs
---

# 📓 RepoVocabularyConfig.cs

> 🇧🇷 Português → [[📓 RepoVocabularyConfig]] · 🇪🇸 Español → [[📓 RepoVocabularyConfig (ES)]]

**Additional per-repository** vocabulary, allowing the concept extraction to be extended **without recompiling** the plugin. ~77 lines.

**Path:** `src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs`

---

## 📄 File read

`.zimerfeldcommitmsg.json` at the **working directory root** (optional).

```json
{
  "knownVocabulary":    ["widget", "gadget"],
  "rejectedVocabulary": ["acme", "contoso"],
  "concepts":           { "widget": "componente", "gadget": "acessório" }
}
```

| Key | Effect |
|---|---|
| `knownVocabulary` | words accepted as part of a descriptive name (added to the built-in set; applies to both languages) |
| `rejectedVocabulary` | words that force the name to be rejected as a concept (proper names/namespaces) |
| `concepts` | concept-word → pt-BR phrase translation (priority over the built-in dictionary) |

---

## ⚙️ API

```csharp
RepoVocabularyConfig.Load(workingDir)  // empty instance if the file is missing / invalid
    .Known      // HashSet<string> (OrdinalIgnoreCase)
    .Rejected   // HashSet<string>
    .ConceptPt  // Dictionary<string,string>
```

- Tolerant parser: `PropertyNameCaseInsensitive`, `ReadCommentHandling = Skip`, `AllowTrailingCommas`.
- **Read/parse failures are silent** (empty config) — they never break the message generation.
- Loaded by the [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]] at construction, added to the built-in defaults.

---

## 🧪 Tests

`RepoVocabularyConfigTests` — covers loading, merging and robustness against malformed JSON. See [[🏷️ Versionamento (EN)|Versioning]].

---

## 🔗 Links

- [[⚙️ CommitMessageGenerator (EN)|CommitMessageGenerator]]
- [[📓 Vocabulário por repositório (EN)|Per-repository vocabulary]]
- [[⚙️ Geração de mensagem a partir do diff (EN)|Message generation from the diff]]

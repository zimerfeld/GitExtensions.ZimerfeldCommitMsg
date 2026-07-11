---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [arquivo, config, vocabulário, json, por-repositório]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs
---

# 📓 RepoVocabularyConfig.cs

> 🇧🇷 Portugués → [[📓 RepoVocabularyConfig]] · 🇺🇸 English → [[📓 RepoVocabularyConfig (EN)]]

Vocabulario **adicional por repositorio**, que permite extender la extracción de conceptos **sin recompilar** el plugin. ~77 líneas.

**Ruta:** `src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs`

---

## 📄 Archivo leído

`.zimerfeldcommitmsg.json` en la **raíz del directorio de trabajo** (opcional).

```json
{
  "knownVocabulary":    ["widget", "gadget"],
  "rejectedVocabulary": ["acme", "contoso"],
  "concepts":           { "widget": "componente", "gadget": "acessório" }
}
```

| Clave | Efecto |
|---|---|
| `knownVocabulary` | palabras aceptadas como parte de un nombre descriptivo (se suma al set embebido; vale para todos los idiomas) |
| `rejectedVocabulary` | palabras que fuerzan el rechazo del nombre como concepto (nombres propios/namespaces) |
| `concepts` | traducción palabra-de-concepto → frase pt-BR (prioridad sobre el diccionario embebido) |

---

## ⚙️ API

```csharp
RepoVocabularyConfig.Load(workingDir)  // instancia vacía si no hay archivo / es inválido
    .Known      // HashSet<string> (OrdinalIgnoreCase)
    .Rejected   // HashSet<string>
    .ConceptPt  // Dictionary<string,string>
```

- Parser tolerante: `PropertyNameCaseInsensitive`, `ReadCommentHandling = Skip`, `AllowTrailingCommas`.
- **Los fallos de lectura/parseo son silenciosos** (config vacía) — nunca rompen la generación del mensaje.
- Cargado por el [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] en la construcción, sumado a los defaults embebidos.

---

## 🧪 Tests

`RepoVocabularyConfigTests` — cubre carga, merge y robustez ante JSON malformado. Ver [[🏷️ Versionamento (ES)|Versionado]].

---

## 🔗 Enlaces

- [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]]
- [[📓 Vocabulário por repositório (ES)|Vocabulario por repositorio]]
- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]]

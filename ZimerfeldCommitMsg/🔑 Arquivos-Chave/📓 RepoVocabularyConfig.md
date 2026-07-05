---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquivo, config, vocabulário, json, por-repositório]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs
---

# 📓 RepoVocabularyConfig.cs

> 🇺🇸 Read this page in English → [[📓 RepoVocabularyConfig (EN)]]

Vocabulário **adicional por repositório**, permitindo estender a extração de conceitos **sem recompilar** o plugin. ~77 linhas.

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/RepoVocabularyConfig.cs`

---

## 📄 Arquivo lido

`.zimerfeldcommitmsg.json` na **raiz do diretório de trabalho** (opcional).

```json
{
  "knownVocabulary":    ["widget", "gadget"],
  "rejectedVocabulary": ["acme", "contoso"],
  "concepts":           { "widget": "componente", "gadget": "acessório" }
}
```

| Chave | Efeito |
|---|---|
| `knownVocabulary` | palavras aceitas como parte de um nome descritivo (soma ao embutido; vale p/ os 2 idiomas) |
| `rejectedVocabulary` | palavras que forçam a rejeição do nome como conceito (nomes próprios/namespaces) |
| `concepts` | tradução palavra-de-conceito → frase pt-BR (prioridade sobre o dicionário embutido) |

---

## ⚙️ API

```csharp
RepoVocabularyConfig.Load(workingDir)  // instância vazia se não houver arquivo / for inválido
    .Known      // HashSet<string> (OrdinalIgnoreCase)
    .Rejected   // HashSet<string>
    .ConceptPt  // Dictionary<string,string>
```

- Parser tolerante: `PropertyNameCaseInsensitive`, `ReadCommentHandling = Skip`, `AllowTrailingCommas`.
- **Falhas de leitura/parse são silenciosas** (config vazia) — nunca quebram a geração da mensagem.
- Carregado pelo [[⚙️ CommitMessageGenerator]] na construção, somado aos defaults embutidos.

---

## 🧪 Testes

`RepoVocabularyConfigTests` — cobre carga, merge e robustez a JSON malformado. Ver [[🏷️ Versionamento]].

---

## 🔗 Ligações

- [[⚙️ CommitMessageGenerator]]
- [[📓 Vocabulário por repositório]]
- [[⚙️ Geração de mensagem a partir do diff]]

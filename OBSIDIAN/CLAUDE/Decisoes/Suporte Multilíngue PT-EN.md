---
tipo: decisão
tags: [decisão, idioma, i18n, multilíngue, português, inglês, resx, deploy]
status: estável
criado: 2026-06-05
---

# Decisão: Suporte Multilíngue (Português / Inglês)

## Contexto

O plugin era **unidirecionalmente português**: montava todos os textos em pt-BR e ainda
traduzia comentários ingleses do código para pt-BR. O objetivo passou a ser gerar a mensagem
**no idioma do sistema operacional** (Português ou Inglês), detectado automaticamente, com
**override manual**.

## Decisões confirmadas

1. **Detecção:** automática pelo SO (`CultureInfo.CurrentUICulture`) **+ override manual**.
2. **Tradução:** quando a saída é inglês, comentários ingleses passam **intactos**; comentários
   em português **permanecem como estão** (não foi criado tradutor PT→EN).
3. **Recursos:** **híbrido** — strings de UI em `.resx`; mapas grandes (conceitos, verbos,
   conjugação) em classes C# por idioma.

## Arquitetura

```
ZimerfeldCommitMsgPlugin
  ├─ ChoiceSetting "Idioma da mensagem / Message language"
  │     [Automático/Automatic | Português/Portuguese | Inglês/English]
  ├─ CurrentLanguage() → MessageLanguageResolver.Resolve(valor do setting)
  └─ new CommitMessageGenerator(workingDir, lang)

CommitMessageGenerator(workingDir, lang)
  ├─ LanguagePack _lang  (PtBrLanguagePack | EnLanguagePack)   ← mapas grandes
  ├─ Strings.Get(key, lang)                                    ← UI (.resx)
  └─ tradução EN→PT só roda quando lang == PtBr; senão identidade
```

### `MessageLanguage` + resolvedor
- `enum MessageLanguage { PtBr, En }`.
- `MessageLanguageResolver.Resolve(string?)` — correspondência **por subtrecho** (tolerante a
  rótulos bilíngues e a valores antigos): contém `"portug"` → PtBr; contém `"ingl"`/`"english"`
  → En; caso contrário (`"Automático/Automatic"`, vazio, desconhecido) → `FromCulture`.
- `FromCulture` — `pt-*` → PtBr; qualquer outro → En.

### `LanguagePack` (mapas por idioma)
Classe abstrata + `PtBrLanguagePack` + `EnLanguagePack`. Expõe:
- `ConceptPhrases` — **chaves idênticas** entre idiomas (identificadores em inglês: Auth,
  Payment…); só os valores mudam. Assim `HasConcept` se comporta igual nos dois.
- `TypeVerb(type, …)` — verbo do tipo CC (Adiciona/Implementa… ↔ Add/Implement…).
- `StatusVerb(status)` — verbo do bullet por status git (Adiciona/Remove/Renomeia/Atualiza ↔
  Add/Remove/Rename/Update).
- `LeadingVerb(desc)` — conjugação: PT reconhece 3ª pessoa/infinitivo; EN normaliza formas
  verbais comuns para o imperativo capitalizado.
- `MainClauseConnectors` — corta justificativa (PT " para "/" pois "… ↔ EN " to "/" because "…).
- `FallbackPhrase(category)` — frase por categoria de arquivo.

### Seleção do idioma na UI — DUAS formas

1. **Dropdown de templates da tela de commit** — três itens planos, um por idioma:
   `Zimerfeld Commit Msg — Automático/Automatic | — Português/Portuguese | — Inglês/English`.
   Escolher um **fixa** o idioma (`_sessionLanguage`) para o auto-refresh manter o mesmo.

<<<<<<< HEAD
   ![[Anexos/ScreenshotDropDown.png]]
=======
   ![[Anexos/ScreenshotDropdown.png]]
>>>>>>> feature/build
2. **`ChoiceSetting` em Configurações → Plugins → ZimerfeldCommitMsg** — define o **padrão**
   (menu Plugins + auto-refresh). O nó só aparece na árvore quando a DLL com `GetSettings()`
   (≥ 1.0.36) está instalada e o GitExtensions reiniciado.

Os rótulos são **bilíngues** (claros independentemente do idioma do SO) e evitam o dilema de
"em que língua mostrar o seletor antes de escolher a língua".

> [!important] A API de templates de plugin é PLANA — não há submenu aninhado
> Confirmado por reflexão: `IGitUICommands.AddCommitTemplate(name, Func<string>, icon)` e
> `CommitTemplateItem { Name, Text, Icon }` — sem hierarquia. O submenu **"Conventional Commits"**
> visto no dropdown é uma função **hardcoded** do `FormCommit`
> (`commitTemplatesToolStripMenuItem_DropDownOpening` → `AddConventionalCommitsItems`), **não** uma
> API disponível a plugins. Por isso o seletor de idioma é exposto como **três itens planos**, não
> como submenu. Prioridade efetiva: `_sessionLanguage` (dropdown) > `ChoiceSetting` > SO.

## Deploy: recursos embutidos, SEM satellite assemblies

> [!important] Por que NÃO usar o mecanismo padrão de `.resx` por cultura
> O `.nuspec`/`install.ps1` entregam **uma única DLL**. O mecanismo padrão (`Strings.pt-BR.resx`)
> geraria **satellite assemblies** (DLLs separadas em subpasta `pt-BR/`), que **não seriam
> entregues** → o pt-BR cairia para o neutro.

**Solução:** o arquivo PT é nomeado `StringsPtBr.resx` (sem sufixo de cultura), então é
embutido no **assembly principal** como recurso neutro. `Strings.cs` mantém dois
`ResourceManager` (um por idioma) com `LogicalName` fixo no `.csproj`
(`...Resources.Strings.resources` e `...Resources.StringsPtBr.resources`) e lê com
`GetString(key, CultureInfo.InvariantCulture)` — evita probing de satellite e **honra o override**
(lê pela cultura resolvida, não pela global da thread).

## Arquivos
- **Novos:** `Localization/MessageLanguage.cs`, `Localization/LanguagePack.cs`,
  `Localization/Strings.cs`, `Resources/Strings.resx`, `Resources/StringsPtBr.resx`.
- **Modificados:** `CommitMessageGenerator.cs` (parametrizado por idioma; removido código morto
  `ArchLayers`/`ExtractArchLayerNames`/`JoinPhrases`), `ZimerfeldCommitMsgPlugin.cs`
  (setting + `GetSettings` + UI localizada), `.csproj`/`.nuspec` (LogicalName, versão 1.0.35).

## Verificação (harness por reflexão + repo de exemplo)

| Português-BR | English |
|---|---|
| `Implementa autenticação` | `Implement authentication` |
| `- Adiciona autenticação` | `- Add authentication` |

`Resolve`: `Português/Portuguese`→PtBr, `Inglês/English`→En (override força); `Automático/Automatic`
/vazio→detecção pelo SO. Retrocompatível com valores antigos (`Português`, `English`).

## Limitação herdada

No caminho pt-BR baseado em comentário, comentários ingleses abaixo do **limiar de 25%** do
tradutor (muitos substantivos de domínio fora do dicionário) **não são traduzidos** — comportamento
**pré-existente** (pipeline de tradução inalterado), ver [[Estratégia de Detecção de Idioma]].

## Relacionado
- [[Estratégia de Detecção de Idioma]] — detecção do idioma do *comentário* (entrada) e tradução
- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[GitExtensions.ZimerfeldCommitMsg]]

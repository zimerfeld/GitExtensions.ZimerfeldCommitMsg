# 2026-06-05 — Suporte multilíngue PT-EN + corpo em bullets

![[Anexos/ScreenshotDropdown.png]]

## Objetivo
1. **Corpo em bullets:** abaixo do subject CC, gerar até **5 frases de uma linha** (cada uma com `-`)
   resumindo as mudanças mais significativas dos arquivos.
2. **Multilíngue (PT/EN):** gerar a mensagem no idioma do SO (Português ou Inglês), detectado
   automaticamente, com **override manual** nas configurações do plugin.

## Decisões do usuário
- Detecção **automática pelo SO + override manual**.
- Em inglês, comentários ingleses passam **intactos**; PT permanece PT (sem tradutor PT→EN).
- Recursos **híbridos**: `.resx` (UI) + classes C# por idioma (mapas grandes).
- Combo do seletor com **rótulos bilíngues**: `Automático/Automatic`, `Português/Portuguese`,
  `Inglês/English`.

## Mudança 1 — Corpo em bullets
`BuildBody` deixou de gerar prosa (`"Abrange … nas camadas de …"`) e passou a listar até 5 bullets
`- <verbo> <conceito>`, ordenados por prioridade do arquivo, verbo conforme status git
(`StatusVerb`: Adiciona/Remove/Renomeia/Atualiza ↔ Add/Remove/Rename/Update).

## Mudança 2 — Arquitetura i18n
- **`Localization/MessageLanguage.cs`** — `enum {PtBr, En}`, `LanguageOption` (rótulos bilíngues),
  `MessageLanguageResolver.Resolve` (match por subtrecho: `portug`→PT, `ingl`/`english`→EN, senão
  detecta pelo `CultureInfo.CurrentUICulture`).
- **`Localization/LanguagePack.cs`** — abstrata + `PtBrLanguagePack`/`EnLanguagePack`:
  `ConceptPhrases` (chaves iguais, valores por idioma), `TypeVerb`, `StatusVerb`, `LeadingVerb`
  (conjugação PT / normalização EN), `MainClauseConnectors`, `FallbackPhrase`.
- **`Localization/Strings.cs` + `Resources/Strings.resx` (EN neutro) + `Resources/StringsPtBr.resx`**
  — strings de UI; dois `ResourceManager` por idioma, lidos pela cultura resolvida.
- **`CommitMessageGenerator`** — construtor recebe `MessageLanguage`; métodos passaram a usar
  `_lang.*`; tradução EN→PT gateada (`_language == PtBr`). Removido código morto:
  `ArchLayers`, `ExtractArchLayerNames`, `JoinPhrases`.
- **`ZimerfeldCommitMsgPlugin`** — `ChoiceSetting` "Idioma da mensagem / Message language",
  `GetSettings()`, `CurrentLanguage()`, `lang` nos 3 pontos de criação, MessageBoxes/Description
  via `Strings`.

## Detalhe crítico de deploy
Deploy é **DLL única** → satellite assemblies (`pt-BR/…resources.dll`) **não** seriam entregues.
Por isso o PT é `StringsPtBr.resx` (sem sufixo de cultura) → embutido no assembly principal, com
`LogicalName` fixo no `.csproj`. Ver [[Suporte Multilíngue PT-EN]].

## Verificação (harness por reflexão + repo de exemplo)
```
[PtBr]  Implementa autenticação      [En]  Implement authentication
        - Adiciona autenticação            - Add authentication
        - Adiciona processamento de…       - Add payment processing
        - Adiciona gerenciamento de token  - Add token management
```
`Resolve`: override (`Português`/`English`) força; `Automático`/vazio → SO. Retrocompatível com
valores antigos. **Build Release: ✅ 0 warnings, 0 errors.**

## Arquivos
- Novos: `Localization/{MessageLanguage,LanguagePack,Strings}.cs`,
  `Resources/{Strings,StringsPtBr}.resx`.
- Modificados: `CommitMessageGenerator.cs`, `ZimerfeldCommitMsgPlugin.cs`, `*.csproj`, `*.nuspec`,
  `README.md`, cofre Obsidian.
- Versão: **1.0.34 → 1.0.35**.

## Pendente
- Instalar a DLL em `C:\Program Files\GitExtensions\Plugins\` exige **Admin** (não foi possível
  pelo ambiente) — rodar `tools\update-dll.ps1` elevado e reiniciar o GitExtensions.

## Relacionado
- [[Suporte Multilíngue PT-EN]]
- [[Estratégia de Detecção de Idioma]]
- [[2026-06-05 - Formato imperativo pt-BR]]
- [[GitExtensions.ZimerfeldCommitMsg]]

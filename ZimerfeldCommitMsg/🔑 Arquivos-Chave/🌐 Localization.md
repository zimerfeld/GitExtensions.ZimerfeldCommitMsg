---
tipo: arquivo-chave
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [arquivo, i18n, localização, resx, cultura]
arquivo: src/GitExtensions.ZimerfeldCommitMsg/Localization/
---

# 🌐 Localization/ (MessageLanguage, LanguagePack, Strings)

> 🇺🇸 English → [[🌐 Localization (EN)]] · 🇪🇸 Español → [[🌐 Localization (ES)]]

Pasta de localização — dois eixos: **idioma da mensagem gerada** e **strings de UI**.

**Caminho:** `src/GitExtensions.ZimerfeldCommitMsg/Localization/`

---

## 🗣️ `MessageLanguage.cs`

- `enum MessageLanguage { PtBr, En }` — idioma de saída da mensagem.
- `LanguageOption` — rótulos **bilíngues** do `ChoiceSetting`: `Automático/Automatic`, `Português/Portuguese`, `Inglês/English` (claros independentemente do idioma do SO).
- `MessageLanguageResolver`:
  - `Resolve(settingValue)` — casa por **subtrecho** (tolerante a rótulos bilíngues e valores antigos): contém `portug` → PtBr; `ingl`/`english` → En; caso contrário detecta pelo SO.
  - `FromCulture(culture)` — `pt-*` → PtBr; qualquer outro → En (usa `CultureInfo.CurrentUICulture`).

---

## 📦 `LanguagePack.cs`

Tabelas por idioma usadas pelo [[⚙️ CommitMessageGenerator]]: verbos por tipo Conventional Commits, pluralização (`FilesWord`), palavra de "arquivo(s)", conectores, etc. ~418 linhas. É o que faz a **mesma lógica** produzir saída em pt-BR ou inglês.

---

## 🧵 `Strings.cs` + `Resources/Strings.resx` / `StringsPtBr.resx`

Strings de **UI** (mensagens de aviso), lidas dos recursos **neutros embutidos** no assembly principal (sem satellite assemblies — deploy de DLL única):

```csharp
Strings.RepoInvalido(lang)       // repositório inválido
Strings.SemMudancasStaged(lang)  // sem mudanças em stage
Strings.PluginDescription(lang)  // descrição do plugin (menu)
```

- Dois `ResourceManager`: `Strings` (en) e `StringsPtBr` (pt).
- `Get(key, lang)` usa **`InvariantCulture`** para evitar probing de satellite assemblies; fallback en → chave.
- A seleção é pelo **idioma resolvido**, não pela cultura global da thread → o override manual é honrado.

> [!note] Deploy de DLL única
> Os `.resx` são embutidos com `LogicalName` fixo no `.csproj`, para o MSBuild não os desviar para satellite assemblies. Ver [[📦 Strings embutidas sem satellite assemblies]] e [[🏷️ Versionamento]].

---

## 🗣️ Idioma efetivo (no plugin)

`EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()` — escolha do dropdown tem prioridade sobre o setting/SO. Ver [[🔌 ZimerfeldCommitMsgPlugin]] e [[🌐 3 - Idioma e auto-refresh]].

---

## 🔗 Ligações

- [[🔌 ZimerfeldCommitMsgPlugin]]
- [[⚙️ CommitMessageGenerator]]
- [[🏗️ Arquitetura]]
- [[📦 Strings embutidas sem satellite assemblies]]

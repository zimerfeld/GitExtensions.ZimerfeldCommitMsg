# GitExtensions.ZimerfeldCommitMsg

![Icone](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/src/GitExtensions.ZimerfeldCommitMsg/Resources/icon-128.png)

[![NuGet version](https://img.shields.io/nuget/v/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=NuGet)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/) &nbsp; [![NuGet downloads](https://img.shields.io/nuget/dt/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=Downloads)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/)

This plugin is built and maintained in my free time. If it saves you time on every commit, a sponsorship helps keep it updated for new GitExtensions versions. 💜

[![GitHub Sponsor](https://img.shields.io/badge/Sponsor-zimerfeld-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white)](https://github.com/sponsors/zimerfeld) &nbsp;&nbsp;&nbsp;&nbsp; [![Ko-fi](https://img.shields.io/badge/Ko--fi-Buy%20me%20a%20coffee-FF5E2B?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/C0D621FCGD)

**Version:** 1.0.89
**Updated:** 2026-06-27

Plugin for **[GitExtensions](https://gitextensions.github.io/)** that automatically generates commit messages by analyzing the real content of staged changes. Changes are classified by **Conventional Commits** types (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) to choose the appropriate **verb**, and the resulting message is a **verb-led sentence** followed by a bulleted body — **without** the `type:` prefix. **Multilingual**: generates output in **Brazilian Portuguese or English**, automatically detected from the operating system language, with a **manual override** in the plugin settings.

![Screenshot](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

[English](README.en-US.md) | [Português-BR](README.pt-BR.md)

[...More information](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg "More information about GitExtensions.ZimerfeldTree package")

---

## High-level features

- **Automatic generation** of the commit message from the real staged diff content, not only from file names.
- **Conventional Commits-guided verb** — classifies changes into types (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) and prefixes the corresponding **verb** (third-person present in pt-BR / imperative in English). The type itself **does not** appear in the message.
- **Multilingual (Portuguese/English)** — language selected automatically from the OS, with a manual override selector.
- **Two content strategies**: diff comment-based (primary) and file name-based (fallback).
- **Bulleted body** — up to 5 one-line sentences, each summarizing the most significant change in a file; **always at least one bullet**, even with a single changed file.
- **English → Portuguese translation** of comments (only when the output is pt-BR); for English output, comments are kept as-is.
- **Sentence sanitizing** — discards comments with **unbalanced delimiters** (`()`, `[]`, `{}`, quotes `"` `'` `` ` ``, `<>`) or that **end in a dangling connector** (`of`, `to`, `with`…); among the valid candidates it picks the **highest-quality** one (not the longest).
- **Never empty** — when there are staged files, it always produces at least the summary line (`<verb> N files`).
- **Three integration modes**: commit dialog template, Plugins menu, and auto-fill (on dialog open and on stage/unstage).
- **Non-destructive** — never overwrites text typed manually by the user.

---

## Multilingual (Portuguese / English)

The plugin generates the entire message (description, body, and verbs) **in the selected language**, and also localizes UI messages (warning dialogs).

### Language selection

There are **two ways** to choose the language, using the same bilingual labels so they remain clear regardless of the system language:

**1. In the commit screen template dropdown** — three items, one per language (quick choice per commit):

```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```

![Commit template dropdown with the three language items](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

**2. In Settings → Plugins → ZimerfeldCommitMsg** — the **"Idioma da mensagem / Message language"** selector defines the **default** used by the Plugins menu and auto-refresh.

| Option                 | Behavior                                                                                                              |
| ---------------------- | --------------------------------------------------------------------------------------------------------------------- |
| `Automático/Automatic` | **Default.** Detects from the operating system/GitExtensions language (`pt-*` → Portuguese; anything else → English). |
| `Português/Portuguese` | Forces Brazilian Portuguese output.                                                                                   |
| `Inglês/English`       | Forces English output.                                                                                                |

> Choosing an item in the **dropdown** pins that language for **auto-refresh** while the dialog is open (it takes precedence over the setting/OS). The Settings selector defines the default used when no dropdown item has been chosen (and by the Plugins menu). Automatic detection uses `CultureInfo.CurrentUICulture`.
>
> **Note:** the **ZimerfeldCommitMsg** node only appears in the **Settings → Plugins** tree after the DLL with the selector (≥ 1.0.36) is installed and GitExtensions is restarted.

### Side-by-side example

| Português-BR                            | English                    |
| --------------------------------------- | -------------------------- |
| `Implementa autenticação`               | `Implement authentication` |
| `- Adiciona autenticação`               | `- Add authentication`     |
| `- Adiciona processamento de pagamento` | `- Add payment processing` |
| `- Adiciona gerenciamento de token`     | `- Add token management`   |

---

## Integration modes

### Template in the commit dialog

The commit window template dropdown includes one item per language — **"Zimerfeld Commit Msg — Automático/Automatic"**, **"— Português/Portuguese"**, and **"— Inglês/English"**. Select one and the message is generated in that language and automatically filled into the text field.

> **Opening** the dropdown generates all three languages on the spot (fresh messages from the current stage); **clicking** an item **replaces** the field content with that language's message — including manually typed text. (This differs from auto-refresh, which preserves user text.)

### Plugins menu

Open **Plugins → ZimerfeldCommitMsg**. The commit dialog opens with the message already filled in.

### Auto-fill on open and on stage/unstage

When you **open** the commit dialog with files already staged, the message is filled in automatically (no need to touch the stage). And while the dialog is open, it is updated whenever files are staged or unstaged. Text typed manually is never overwritten.

---

## Generated message format

```text
<Context> - <Verb> <N> <files> (<types>)

- <bullet 1>
- <bullet 2>
```

- **Context prefix (up to 5 words)** — the first line starts with a concept derived from the **most impactful file name**, followed by ` - `. In **English** it is the humanized concept (e.g. `OverlayController` → `Overlay`). In **pt-BR** it is a nominalized action phrase: the status action noun (`Adição`/`Remoção`/`Atualização`/`Renomeação`) + `de` + translated, reordered concept (e.g. `New Text Document` deleted → `Remoção de documento de texto`). This guarantees the title is **never** generic (e.g. never just `Add 1 file (fix)`), giving the commit an identity so it can be told apart later. It is omitted only when no file yields a readable concept.
- **Consolidated summary** — after the context comes the **verb** of the dominant type + the **file count** + the list of **types** involved (e.g. `Implement 4 files (feat, build, docs)`).
- **No `type:` prefix** — the Conventional Commits type is **not printed**; it only selects the verb, such as `Implement`, `Fix`, or `Update`.
- **No scope** — avoids redundancy with the project name.
- **No color highlighting** — uses `git diff --no-color` to avoid ANSI codes.
- **80-character limit** on the first line, trimming at the last space and adding `…`.
- **Description and verbs in the active language** (Brazilian Portuguese or English).
- **Always-present body** — up to 5 one-line bullets; whenever any file is staged, it produces **at least one** bullet (even with a single file).

![Generated commit message in the GitExtensions commit dialog](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotCommitMsg.png)

### Detected types (define the verb)

Each staged file receives a type. The first-line **verb** comes from the **highest-priority** type across all files (order: `feat` → `fix` → `refactor` → `perf` → `test` → `build` → `ci` → `chore` → `docs` → `style`). **The type is not printed** — it only selects the verb.

| Type       | Assigned to a file when…                                                            |
| ---------- | ----------------------------------------------------------------------------------- |
| `feat`     | source code file **added** (status `A`/`C`, source/web category)                    |
| `fix`      | source code file **modified/renamed** (status `M`/`R`/`T`)                          |
| `docs`     | documentation file (`.md`, `.txt`, `.rst`, `.adoc`)                                 |
| `test`     | test path (`test`/`tests`/`spec` folder or `Test`/`Spec` suffix)                    |
| `chore`    | configuration file (`.json`, `.yml`, etc.) **or** any **deleted** file (status `D`) |
| `build`    | build file (`.csproj`, `.sln`, `Dockerfile`, etc.)                                  |
| `refactor` | remaining cases without a clear pattern                                             |

---

## How the message is generated

### Strategy 1 — Diff comment-based (primary)

Runs `git diff --cached --no-color` and collects **comment** lines that were **added** (`+`) or **removed** (`-`). Added comments are prioritized by **file category** (source = 4 > web = 3 > build = 2 / config = 1 / docs = 1; test files = 0); removed comments receive one lower priority level. Up to **5 comments** are used (one per file, most to least relevant). Within a single file, instead of taking the longest comment, the plugin **scores** the candidates — rewarding a closed sentence, balanced length (~20–72 chars), and a leading verb — and picks the highest-scoring one.

#### Recognized patterns

| Syntax                  | Languages                            |
| ----------------------- | ------------------------------------ |
| `// text` or `/// text` | C#, Java, JavaScript, TypeScript, Go |
| `# text`                | Python, Shell, YAML, Ruby            |

#### Rejected comments

| Condition                        | Rejected example                |
| -------------------------------- | ------------------------------- |
| Visual separator                 | `// ─────────────────────`      |
| XML documentation tag            | `/// <summary>`                 |
| Commented-out code (has `{` `}`) | `// if (x) { return; }`         |
| Commented-out code (method call) | `// method(argument)`           |
| Too short (< 10 chars)           | `// ok`                         |
| No spaces (not a sentence)       | `// TODO`                       |
| **Unbalanced delimiter**         | `// builds the tree (recursive` |
| **Ends in a dangling connector** | `// maps the token to`          |

#### How comments are used

The most impactful comment becomes the first-line **description** (with the initial verb normalized to third-person/imperative). The remaining comments appear in the **body** as list items:

```text
Validate the token before processing the request

- Filter requests without an authentication header
```

> If the selected comment contains a justification connector (`para`, `pois`, `porque`, …), the part after the connector is discarded from the first line, and the description uses the functional phrase from file names (Strategy 2) to avoid repeating the bullets.

When the output is **Brazilian Portuguese**, English comments are automatically translated before being used (and discarded if the translation remains more than 25% English). When the output is **English**, comments are kept as-is (and Portuguese comments remain in Portuguese). Branch names (`feature/…`, `release/…`) and Conventional Commits types are preserved during translation.

---

### Strategy 2 — File name-based (fallback)

Used when no valid comment is found in the diff.

#### Semantic concept extraction

For each staged file, the file name (without extension) goes through:

1. **Interface prefix removal** — `IUserService` → `UserService`
2. **Architectural suffix removal** (longest match first):

   `ServiceTests`, `ControllerTests`, `RepositoryTests` …
   `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator` …
   `Middleware`, `Validator`, `Mapper`, `Factory`, `Builder` …
   `Tests`, `Test`, `Spec`, `Mock`, `Command`, `Query`, `Event` …
   `Dto`, `ViewModel`, `Model`, `Entity`, `Config`, `Settings` …
   `Facade`, `Adapter`, `Client`, `Endpoint`, `Base`, `Impl` …

3. **Quality filters:**
   - Name with a dot in the stem → rejected (for example, `GitExtensions.ZimerfeldCommitMsg`)
   - **Rejected vocabulary** (`RejectedVocabulary`) — if **any** word of the name is in this set, the name is rejected (takes precedence over everything). For proper nouns/namespaces (e.g. `zimerfeld`, `git`, `extensions`). Extend per project.
   - Name with 3+ words → rejected as a namespace, **except** when it is a dictionary concept **or** all of its words are recognized vocabulary (`KnownVocabulary` + translation dictionary + concepts). Example: `New Text Document` **passes** (new/text/document are known); `ZimerfeldCommitMsg` **does not** (`zimerfeld` is in rejected vocabulary). Extend `KnownVocabulary` to cover more terms.

#### Concept → pt-BR phrase mapping (examples)

| Extracted concept         | Generated phrase                          |
| ------------------------- | ----------------------------------------- |
| `Auth` / `Authentication` | autenticação                              |
| `User` / `Users`          | gerenciamento de usuários                 |
| `Token` / `Jwt`           | gerenciamento de token / autenticação JWT |
| `Payment`                 | processamento de pagamento                |
| `Order`                   | processamento de pedidos                  |
| `Notification`            | notificações                              |
| `Cache`                   | cache                                     |
| `Migration`               | migração de banco de dados                |
| `Report`                  | relatórios                                |
| `CommitMessage`           | mensagem de commit                        |

#### Verbs by type

The verb is selected by type and, in some cases, by change context:

| Type       | Verb (pt-BR)          | Verb (en)          | Condition                                                        |
| ---------- | --------------------- | ------------------ | ---------------------------------------------------------------- |
| `feat`     | Implementa / Adiciona | Implement / Add    | `Implementa` when there are only additions; `Adiciona` otherwise |
| `fix`      | Corrige               | Fix                | —                                                                |
| `refactor` | Refatora              | Refactor           | —                                                                |
| `docs`     | Documenta / Atualiza  | Document / Update  | `Documenta` when there are additions; `Atualiza` otherwise       |
| `build`    | Configura             | Configure          | —                                                                |
| `chore`    | Remove / Configura    | Remove / Configure | `Remove` when there are deletions; `Configura` otherwise         |
| `test`     | Adiciona              | Add                | —                                                                |
| `perf`     | Otimiza               | Optimize           | —                                                                |
| `ci`       | Configura             | Configure          | —                                                                |
| `style`    | Padroniza             | Standardize        | —                                                                |

> If the description already starts with a known verb (for example, the comment `filter stems…`), it is **normalized** (pt-BR: third-person present → `Filtra`; en: imperative → `Filter`) instead of prefixing a new type-based verb.

#### Message body

The body lists up to **5 bullets** — **at least one, even with a single file** — each with a one-line sentence summarizing the most significant change in a file (ordered by file relevance). Each bullet verb follows the git status (added/removed/renamed/modified):

```text
- Add authentication
- Add payment processing
- Add token management
```

> **File-name fallback:** when the name yields no readable concept (dotted names, multi-word names, or namespaces) and there is no diff comment, the bullet falls back to the **file name itself** (e.g., `Remove New Text Document.txt`). This way **no file is left without a line** — there is always a title **and** a body.

---

## Generated message examples

| Staged files                                                                    | Generated message (pt-BR)                                                                                               | Generated message (en)                                                             |
| ------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| `AuthService.cs` added                                                          | `Implementa autenticação`                                                                                               | `Implement authentication`                                                         |
| `PaymentService.cs` added                                                       | `Implementa processamento de pagamento`                                                                                 | `Implement payment processing`                                                     |
| `UserService.cs` modified                                                       | `Corrige gerenciamento de usuários`                                                                                     | `Fix user management`                                                              |
| `README.md` modified                                                            | `Atualiza documentação`                                                                                                 | `Update documentation`                                                             |
| `UserService.cs` + `TokenService.cs` added                                      | `Implementa gerenciamento de usuários`<br>`- Adiciona gerenciamento de usuários`<br>`- Adiciona gerenciamento de token` | `Implement user management`<br>`- Add user management`<br>`- Add token management` |
| `.cs` modified with comment `// Valida o token antes de processar a requisição` | `Valida o token antes de processar a requisição`                                                                        | _(pt comment is kept as-is)_                                                       |

---

## Requirements

- PowerShell 5.1 or later
- **Administrator** permission to install/uninstall

---

## Installation

### Option A — Via PowerShell (recommended)

Run PowerShell **as Administrator**:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

![install.ps1 output confirming plugin installation](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotInstall.png)

### Option B — Manual

Copy `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` to:

```text
C:\Program Files\GitExtensions\Plugins\
```

Restart GitExtensions.

---

## Uninstallation

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

![uninstall.ps1 output confirming plugin removal](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUninstall.png)

Removing the DLL does not affect any other part of GitExtensions.

---

## Build and versioning

Each time `build.ps1` runs, the script:

1. Reads the current version from `.nuspec`
2. Computes the new version: increments `build` by +1 → `major.minor.build`
3. Writes the new version and date to the **docs first**: the READMEs and the Obsidian vault
4. Only then bumps the version in `.nuspec` and `.csproj`
5. Compiles in Release
6. Copies the DLL to `C:\Program Files\GitExtensions\Plugins\` _(requires Admin)_
7. Updates `tools\net9.0-windows\` with the new DLL
8. Generates `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
9. Removes `.nupkg` files from previous versions

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

![build.ps1 output with version increment and packaging](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotBuild.png)

### Fast deploy (without incrementing version)

To update only the DLL during development:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

![update-dll.ps1 output updating only the DLL](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUpdate.png)

---

## Related plugins

### [GitExtensions.ZimerfeldTree](https://www.nuget.org/packages/GitExtensions.ZimerfeldTree/)

Plugin for GitExtensions that displays branches hierarchically in a persistent, non-modal tree window. Branches separated by `/` are shown as nested folder nodes under three fixed sections — LOCAL, REMOTES, and tags. By **zimerfeld**.

---

## License

[CC BY-NC-ND 4.0](LICENSE.txt)

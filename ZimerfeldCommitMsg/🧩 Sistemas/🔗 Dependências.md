---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [dependências, assemblies, gitextensions, nuget]
---

# 🔗 Dependências

> 🇺🇸 Read this page in English → [[🔗 Dependências (EN)]]

## 🧩 Assemblies do GitExtensions (referências de compilação)

Ambas referenciadas com `<Private>false</Private>` — **não** copiadas para o output (o host já as fornece em runtime).

| Assembly | Caminho | Uso |
|---|---|---|
| `GitExtensions.Extensibility.dll` | `refs\` (versionado no repo) | `IGitPlugin`, `GitPluginBase`, `IGitUICommands`, `ISetting`/`ChoiceSetting`, `CommitTemplateManager` |
| `System.ComponentModel.Composition.dll` | `refs\` (versionado no repo) | MEF — `[Export(typeof(IGitPlugin))]` |

> **Build determinístico (qualquer máquina Windows):** os assemblies de referência ficam **versionados em `refs\`** (apontados por `$(GitExtensionsRefPath)` no `.csproj`), **não** baixados em prebuild. Garante compilação reprodutível e **offline**. Um download anterior podia trazer o asset arm64 (6.0.5.75) incompatível com o x64 instalado (6.0.5.18375) — daí a versionagem em `refs\` (ver `refs/README.md`). O `.csproj` demove o aviso `MSB3277` (conflito benigno entre o ref pack net9 4.0 e o VS.Threading 8.0 do host — resolvido em runtime).

## 📦 Dependência do pacote NuGet (marcador do Plugin Manager)

```xml
<dependency id="GitExtensions.Extensibility" version="[0.4.0, 0.5.0)" />
```

> [!important] Por que a dependência marcadora existe
> O Plugin Manager do GitExtensions filtra o feed do nuget.org por pacotes que **dependem** de
> `GitExtensions.Extensibility`. **Sem** essa dependência, o pacote é publicado mas **nunca aparece**
> no Plugin Manager interno. Além disso, o filtro casa a **faixa de versão** da dependência com a
> versão que o **manager anuncia** para o host em execução (**não** o runtime instalado): o manager
> v3.x do GitExtensions 6.x anuncia `0.4.0`, então a faixa precisa **conter** 0.4.0 → `[0.4.0, 0.5.0)`,
> igual aos demais plugins que funcionam no GE6 (AITools, BundleBackuper, Gerrit, SolutionRunner…).
> Um valor solto como `1.0.0.129` significa `>= 1.0.0.129`, que **não** inclui 0.4.0 — e o pacote
> era **silenciosamente filtrado para fora** do Plugin Manager. Para o GitExtensions 7 (manager
> anuncia `7.0.0`), usar `[7.0.0, 8.0.0)`. Alinhado ao [[GitExtensions.ZimerfeldTree]].

## 📦 Empacotamento (nuspec)

- DLL em **`lib\` raiz** (grupo "any" que o Plugin Manager extrai) — gera o aviso **NU5101**, intencional e filtrado no `build.ps1`. Ver [[🏷️ Versionamento]].
- Mesma DLL também em `tools\net9.0-windows\` para o install via **Package Manager Console** (`install.ps1`).
- `LICENSE.txt` (CC BY-NC-ND 4.0, `type="file"`), `README.md`/`README.pt-BR.md`/`README.en-US.md`, e `icon-128.png` (em `images\`) no pacote.

## 🔑 Interfaces-chave usadas

### `IGitPlugin` (via `GitPluginBase`)
- `Register(IGitUICommands)` / `Unregister(IGitUICommands)` — captura/limpa o commands, registra/desregistra o template de commit e assina/desassina `Application.Idle`
- `Execute(GitUIEventArgs)` — menu Plugins → ZimerfeldCommitMsg
- `GetSettings()` — expõe o `ChoiceSetting` de idioma

### `IGitUICommands` / host
- `Module.WorkingDir` — working dir usado para rodar `git diff --cached`
- `CommitTemplateManager` — registro dos itens de template (um por idioma) no dropdown do diálogo de commit
- `FormCommit` (via reflection sobre `Application.OpenForms`) — a caixa de mensagem a preencher

## ✅ Runtime (o que o usuário precisa ter)

| Requisito | Valor |
|---|---|
| GitExtensions | 6.x (.NET 9) |
| .NET | 9.0 (Windows) — fornecido pelo host |
| `git` | no `PATH` (o gerador roda `git diff --cached`) |
| PowerShell | 5.1+ (scripts de build/deploy) |
| .NET 9 SDK + nuget | para compilar e empacotar |

## 🔗 Ligações

- [[🏗️ Arquitetura]]
- [[🏷️ Versionamento]]
- [[⚙️ CommitMessageGenerator]]

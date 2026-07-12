---
tipo: negocio
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [projeto, negocio, csharp, gitextensions, plugin, winforms, commit-message, conventional-commits, i18n]
status: ativo
linguagem: C#
versao: 1.0.97
repo: C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg
---

# 📦 GitExtensions.ZimerfeldCommitMsg

> 🇧🇷 Portugués → [[📦 GitExtensions.ZimerfeldCommitMsg]] · 🇺🇸 English → [[📦 GitExtensions.ZimerfeldCommitMsg (EN)]]

## 🎯 Objetivo
Plugin para **[GitExtensions](https://gitextensions.github.io/)** que **genera mensajes de commit automáticamente** analizando el **contenido real** de los cambios en stage (no solo los nombres de archivo). Los cambios se clasifican por los tipos de **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) para elegir el **verbo** adecuado; el mensaje es un **subject iniciado por verbo** (sin el prefijo literal `tipo:`) más un **cuerpo en viñetas**. Ver [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]].

## 💜 Financiación / Patrocinio
Canales de donación configurados (badges en la parte superior del README):
- **GitHub Sponsors:** `@zimerfeld` → https://github.com/sponsors/zimerfeld
- **Ko-fi:** `C0D621FCGD` → https://ko-fi.com/C0D621FCGD
- **Prueba social en el README:** badges de versión y **descargas de NuGet** (`shields.io/nuget/v` y `/dt`).

## 📂 Estructura del repositorio
```
C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\
├─ src\GitExtensions.ZimerfeldCommitMsg\        # código del plugin (.csproj)
│   ├─ ZimerfeldCommitMsgPlugin.cs             # entry point MEF + integración con el diálogo de commit
│   ├─ CommitMessageGenerator.cs               # motor: diff → tipos CC → verbo → subject + cuerpo (~1200 líneas)
│   ├─ RepoVocabularyConfig.cs                 # vocabulario extra por repo (.zimerfeldcommitmsg.json)
│   ├─ Localization\                           # MessageLanguage, LanguagePack, Strings
│   ├─ Resources\                              # icon.png, icon-128.png, Strings.resx, StringsPtBr.resx, StringsEsEs.resx
│   ├─ *.csproj / *.nuspec                     # build + manifiesto NuGet
├─ tests\GitExtensions.ZimerfeldCommitMsg.Tests\  # xUnit: comentarios, conceptos, vocab, traducción
├─ refs\                                        # DLLs del host versionadas (build determinista)
├─ tools\                                       # install/uninstall/update-dll .ps1, nuget.exe, generate-icon.ps1
│   └─ net9.0-windows\                          # salida del build (DLL) usada por el nupkg
├─ ZimerfeldCommitMsg\                          # 🧠 este cofre de memoria
├─ build.ps1                                    # incrementa versión + build + deploy + nupkg
├─ README.md / README.pt-BR.md / README.en-US.md  # reflejado en [[📖 README — Instalação, Uso e Build (ES)|README — Instalación, Uso y Build]]
└─ GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg
```

## ⚙️ Stack técnico
- **Lenguaje:** C# (`net9.0-windows`), `Nullable` + `ImplicitUsings` habilitados
- **UI:** WinForms (`UseWindowsForms`) — el plugin **no tiene ventana propia**; se integra en el **diálogo de commit** del host
- **Tipo de salida:** `Library` (DLL cargada por GitExtensions)
- **AssemblyName:** `GitExtensions.Plugins.ZimerfeldCommitMsg`
- **Namespace raíz:** `GitExtensions.ZimerfeldCommitMsg`
- **NeutralLanguage:** `pt-BR`
- **Plugin model:** MEF (`[Export(typeof(IGitPlugin))]`) — ver [[🧩 Plugin MEF para GitExtensions (ES)|Plugin MEF para GitExtensions]]
- **Referencias externas** (de `refs\`, `Private=false`): `GitExtensions.Extensibility.dll`, `System.ComponentModel.Composition.dll`
- **Tests:** xUnit, con `InternalsVisibleTo` exponiendo las funciones puras del generador

## ✨ Funcionalidades principales
- **Generación automática** a partir del contenido real del diff en stage (`git diff --cached`), no solo de los nombres de archivo. Ver [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]].
- **Verbo guiado por Conventional Commits** — clasifica los cambios por tipo (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) y antepone el **verbo** correspondiente (3ª persona en pt-BR / imperativo en inglés / 3ª persona en español). El tipo en sí **no** aparece. Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo]].
- **Dos estrategias de contenido:** basada en **comentarios** del diff (principal) y basada en **nombres de archivo** (fallback). Ver [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias: comentarios y nombres de archivo]].
- **Cuerpo en viñetas** — hasta 5 frases de una línea, cada una resumiendo el cambio más significativo de un archivo; **siempre al menos una viñeta**.
- **Traducción inglés → portugués/español** de los comentarios cuando la salida es pt-BR o es-ES; en inglés, pasan intactos.
- **Saneamiento** — descarta comentarios con delimitadores desbalanceados o terminados en palabra de enlace suelta; elige el de **mejor calidad** (no el más largo).
- **Vocabulario por repositorio** — `.zimerfeldcommitmsg.json` extiende vocabulario conocido/rechazado y frases de concepto sin recompilar. Ver [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]].
- **Multilingüe (PT-BR / EN / ES):** automático por el SO + override (dropdown de 4 elementos en el diálogo de commit **y** selector en Configuración → Plugins). Ver [[🌐 Localization (ES)|Localization]].
- **Tres modos de integración:** template en el diálogo de commit, menú Plugins y **autorrelleno** (al abrir el diálogo y al stage/unstage). Ver [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]].
- **No destructivo** — nunca sobrescribe texto tecleado manualmente.

## 🏗️ Arquitectura (Plugin → Generator → Localization)
Ver [[🏗️ Arquitetura (ES)|Arquitectura]]:
```
GitExtensions (host)
    │ MEF
    ▼
ZimerfeldCommitMsgPlugin  ← [Export(IGitPlugin)]; registra template de commit + Application.Idle
    │ al abrir/cambiar el FormCommit, genera e inyecta el mensaje (sin sobrescribir lo que el usuario tecleó)
    ▼
CommitMessageGenerator    ── lee ──►  git diff --cached  (comentarios + nombres de archivo)
    │ clasifica tipos CC → verbo → subject consolidado + cuerpo en viñetas
    ▼
Localization (LanguagePack / Strings / MessageLanguage)  → idioma de la salida + strings de UI
```

## 🛠️ Build / instalación
```powershell
# Build: incrementa build, compila Release, hace deploy (Admin), genera nupkg
.\build.ps1
.\build.ps1 -Force   # siempre recompila/reempaqueta

# Scripts auxiliares en tools\ (Admin p/ Program Files)
tools\install.ps1      # instala el plugin (también vía Package Manager Console)
tools\uninstall.ps1    # elimina (no afecta al resto de GitExtensions)
tools\update-dll.ps1   # actualiza solo la DLL en la carpeta Plugins
```
> Vía **Plugin Manager de GitExtensions**: buscar *ZimerfeldCommitMsg* e instalar. Paso a paso en [[📖 README — Instalação, Uso e Build (ES)|README — Instalación, Uso y Build]] y [[🏷️ Versionamento (ES)|Versionado]]. Runbooks: [[💻 Ambiente Local (Dev) (ES)|Entorno Local (Dev)]] · [[🚀 Deploy em Produção (Prod) (ES)|Deploy en Producción (Prod)]].

## 💰 Ángulo de inversión
- **Fricción diaria:** todo dev commitea varias veces al día; los buenos mensajes son caros de escribir a mano. Este plugin elimina esa fricción sin quitar el control (no destructivo).
- **Diferencial técnico:** lee el **contenido del diff** (comentarios) y clasifica por Conventional Commits — más rico que los generadores basados solo en el nombre de archivo.
- **Coste marginal bajo:** comparte la infraestructura (build, i18n, refs versionados, empaquetado) de los hermanos `GitExtensions.ZimerfeldLFS` y `GitExtensions.ZimerfeldTree` — un portafolio cohesionado refuerza la marca **Zimerfeld** en el ecosistema GitExtensions/NuGet.
- **Distribución lista:** publicado en NuGet y visible en el Plugin Manager interno (dependencia marcadora `GitExtensions.Extensibility`).

## 🐛 Trampas conocidas
> [!warning] El host no notifica "diálogo de commit abierto"
> No hay evento de API para "FormCommit abrió". El plugin detecta el `FormCommit` recién abierto en el **`Application.Idle`** de la UI y hace gates por instancia + working dir para no reprocesar en cada tick. Ver [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]] y [[🔌 Integração via template dropdown e Application.Idle (ES)|Integración vía template dropdown + Application.Idle]].

<!-- -->

> [!warning] Strings de UI embebidas (sin satellite assemblies)
> Las strings quedan en `Resources\Strings.resx` / `StringsPtBr.resx` / `StringsEsEs.resx` con `LogicalName` fijo, embebidas en el **assembly principal** — así el deploy es de **DLL única**. Ver [[📦 Strings embutidas sem satellite assemblies (ES)|Strings de UI embebidas]] y [[🏷️ Versionamento (ES)|Versionado]].

<!-- -->

> [!warning] DLL en `lib\` RAÍZ en el nuspec (aviso NU5101 intencional)
> Como en los hermanos, la DLL va en `lib\` **raíz** (grupo "any") para que el Plugin Manager la extraiga; el aviso **NU5101** del `nuget pack` se filtra a propósito en el `build.ps1`. Ver [[🏷️ Versionamento (ES)|Versionado]] y [[🔗 Dependências (ES)|Dependencias]].

## 🔢 Versionado
- Versión actual: **1.0.97** (csproj + nuspec sincronizados por el `build.ps1`)
- Esquema: `major.minor.BUILD`, BUILD auto-incrementado en cada build
- En cada build, el `build.ps1` sella versión + fecha en los **READMEs** (y mantiene este cofre en sincronía)

## 🔗 Relacionado
- [[📖 README — Instalação, Uso e Build (ES)|README — Instalación, Uso y Build]] — espejo del `README.md`
- [[🧩 Plugin MEF para GitExtensions (ES)|Plugin MEF para GitExtensions]]
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]] · [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]]
- [[🏗️ Arquitetura (ES)|Arquitectura]] · [[🔭 Visão Geral (ES)|Visión General]] · [[🏷️ Versionamento (ES)|Versionado]] · [[🔗 Dependências (ES)|Dependencias]]
- `GitExtensions.ZimerfeldLFS` — hermano (Git LFS)
- `GitExtensions.ZimerfeldTree` — hermano (árbol de branches)
- [[🔑 Fatos-Chave (ES)|Hechos Clave]]

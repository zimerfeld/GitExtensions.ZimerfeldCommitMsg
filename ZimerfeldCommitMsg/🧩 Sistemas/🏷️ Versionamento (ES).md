---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [build, versão, nupkg, deploy]
---

# 🏷️ Versionado y Build

> 🇧🇷 Portugués → [[🏷️ Versionamento]] · 🇺🇸 English → [[🏷️ Versionamento (EN)]]

## 🔢 Esquema de versión

`major.minor.build` — solo el `build` se incrementa automáticamente por el `build.ps1`. Major y minor se cambian manualmente.

**Versión actual:** `1.0.97` *(fuente de la verdad: `.nuspec` / `.csproj`)*

> [!note] Strings de UI embebidas (sin satellite assemblies)
> Las strings de UI viven en `Resources/Strings.resx`, `Resources/StringsPtBr.resx` y `Resources/StringsEsEs.resx`, embebidas en el
> **assembly principal** con `LogicalName` explícito — así MSBuild **no** las desvía hacia
> satellite assemblies y el deploy sigue siendo de **DLL única**. Leídas en runtime por `Strings`.
> Ver [[📦 Strings embutidas sem satellite assemblies (ES)|Strings embebidas sin satellite assemblies]].

## 🔄 Ciclo build.ps1

```
build.ps1  [-Force]
  │
  ├─ 1. Lee la versión actual del .nuspec
  ├─ 2. Calcula newVersion (build +1)
  ├─ 2b. Detecta cambios (fuentes/docs más nuevos que el último .nupkg); sin -Force y sin
  │      cambio → mantiene la versión y sale (build/pack omitidos)
  ├─ 2c. Cierra el GitExtensions si está en ejecución
  ├─ 3. Bump en el .nuspec  ← <version>
  ├─ 4. Bump en el .csproj  ← <Version>
  ├─ 4b. Sella versión + fecha al inicio de los READMEs (README.md / .pt-BR / .en-US)
  ├─ 5. dotnet build -c Release
  ├─ 6. Copia la DLL → C:\Program Files\GitExtensions\Plugins\  (requiere Admin)
  ├─ 6b. Copia la DLL → tools\net9.0-windows\  (para el nupkg)
  ├─ 7. nuget pack .nuspec → .nupkg en la raíz (filtra el aviso NU5101)
  └─ 7b. Elimina .nupkg de versiones anteriores
```

> [!warning] El aviso **NU5101** es intencional
> La DLL se empaqueta en `lib\` **raíz** (grupo "any" que el Plugin Manager extrae), no en
> `lib\net9.0-windows\`. Esto genera el aviso NU5101, que el `build.ps1` **filtra a propósito** en
> `nuget pack`. Detalle en [[🔗 Dependências (ES)|Dependencias]] y en el `.nuspec`.

<!-- -->

> Requiere el **.NET 9 SDK** (`dotnet`) y, para el deploy, permiso de **Administrador**. Sin Admin, el paso de deploy se omite con aviso; `nuget` queda en `tools\nuget.exe`.

## 📄 Archivos versionados

| Archivo | Campo actualizado |
|---|---|
| `GitExtensions.ZimerfeldCommitMsg.nuspec` | `<version>` |
| `GitExtensions.ZimerfeldCommitMsg.csproj` | `<Version>` |
| `README.md` / `README.pt-BR.md` / `README.en-US.md` | `**Version/Versão:**` y `**Updated/Atualizado em:**` |

## 🧰 Instalación / desinstalación manual

```powershell
tools\install.ps1        # requiere Admin — copia la DLL a la carpeta Plugins (también vía PMC)
tools\uninstall.ps1      # requiere Admin — elimina la DLL (no afecta al resto de GitExtensions)
tools\update-dll.ps1     # actualiza solo la DLL en la carpeta Plugins
```

## 🧪 Tests

Suite **xUnit** en `tests\GitExtensions.ZimerfeldCommitMsg.Tests\` (las funciones puras del generador se exponen vía `InternalsVisibleTo`):
- `CommentExtractionTests` — reconocimiento y limpieza de comentarios (varias sintaxis)
- `ConceptExtractionTests` — derivación de conceptos a partir de nombres de archivo
- `RepoVocabularyConfigTests` — carga del `.zimerfeldcommitmsg.json`
- `TranslationTests` — traducción inglés → portugués/español

## 🔗 Enlaces

- [[🛠️ build.ps1 (ES)|build.ps1]]
- [[🔗 Dependências (ES)|Dependencias]]
- [[💻 Ambiente Local (Dev) (ES)|Entorno Local (Dev)]]
- [[🚀 Deploy em Produção (Prod) (ES)|Despliegue en Producción (Prod)]]

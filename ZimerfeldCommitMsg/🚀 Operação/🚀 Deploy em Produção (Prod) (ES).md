---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [operação, deploy, release, nupkg, nuget, github]
---

# 🚀 Despliegue en Producción (Prod)

> 🇧🇷 Portugués → [[🚀 Deploy em Produção (Prod)]] · 🇺🇸 English → [[🚀 Deploy em Produção (Prod) (EN)]]

Cómo publicar una nueva versión del plugin: generar el `.nupkg` versionado y distribuirlo (NuGet + release en GitHub). La distribución al usuario final es vía **Plugin Manager de GitExtensions** y **NuGet**.

## ⚡ TL;DR — el comando único

```powershell
# en la raíz del repo (Admin), genera el .nupkg de release con la versión bumpeada
.\build.ps1
```

El `.nupkg` de producción queda en la **raíz del repo** (`GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`). Publícalo en NuGet y adjúntalo a la release de GitHub.

## ⚙️ Qué hace el script (en orden)

`build.ps1` (fuente de la verdad de la versión: `.nuspec`/`.csproj`):
1. Lee la versión actual y calcula `newVersion` (build +1).
2. Detecta cambios; sin `-Force` y sin cambio → no genera un nuevo paquete.
3. Cierra el GitExtensions.
4. Sella versión + fecha en los **READMEs** y en las notas del cofre que reflejan la versión actual; hace el bump del `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copia la DLL a `tools\net9.0-windows\` (fuente del nupkg).
7. `nuget pack` → `.nupkg` en la raíz; elimina `.nupkg` de versiones anteriores. La DLL va en `lib\` **raíz** (grupo "any") — de ahí el aviso **NU5101**, filtrado a propósito.

## 📤 Publicación (tras el build)

```powershell
# 1) Publicar en NuGet.org (requiere API key)
nuget push .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey <API_KEY>

# 2) Crear la release en GitHub con la tag de la versión y adjuntar el .nupkg
gh release create vX.Y.Z .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg --title "vX.Y.Z" --notes "..."
```
> Prueba social en el README: badges `shields.io/nuget/v` (versión) y `/dt` (descargas) — se actualizan solos tras la publicación en NuGet.

## 🚦 Requisitos
- **.NET 9 SDK**, `nuget.exe` (en `tools\nuget.exe`), API key de NuGet, `gh` autenticado.
- Versión sincronizada en `.nuspec` **y** `.csproj` (el `build.ps1` lo garantiza).

## 🔀 Reglas que respeta (GitFlow)
- **No** hacer deploy de producción desde una **release branch**: validar en develop en la release branch → finalizar la release actualizando `develop` y `main` → generar la **tag** `YYYYMMddhhmm<fase>` → solo entonces publicar.
- Garantizar que `main` sea la branch **default** (refleja producción).
- No commitear/pushear/publicar sin petición explícita del usuario.

## 🩺 Troubleshooting
- **NU5101 al empaquetar:** esperado (DLL en `lib\` raíz para que el Plugin Manager la extraiga) — el `build.ps1` lo filtra.
- **Paquete no generado:** ejecuta `.\build.ps1 -Force` (sin cambio detectado, el build se omite).
- **El Plugin Manager no lo lista:** confirma la dependencia marcadora `GitExtensions.Extensibility` en el `.nuspec`. Ver [[🔗 Dependências (ES)|Dependencias]].

## 🔗 Enlaces
- [[💻 Ambiente Local (Dev) (ES)|Entorno Local (Dev)]]
- [[🛠️ build.ps1 (ES)|build.ps1]]
- [[🏷️ Versionamento (ES)|Versionado]]
- [[🔗 Dependências (ES)|Dependencias]]

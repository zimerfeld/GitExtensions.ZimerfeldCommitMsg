---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [operação, dev, build, install, powershell]
---

# 💻 Entorno Local (Dev)

> 🇧🇷 Portugués → [[💻 Ambiente Local (Dev)]] · 🇺🇸 English → [[💻 Ambiente Local (Dev) (EN)]]

Cómo compilar e instalar el plugin en el GitExtensions local para desarrollar y probar.

## ⚡ TL;DR — el comando único

```powershell
# en la raíz del repo, PowerShell como Administrador (para el deploy en Program Files)
.\build.ps1 -Force
```

Compila en Release, hace deploy de la DLL en `C:\Program Files\GitExtensions\Plugins\` y empaqueta el `.nupkg`. Reinicia el GitExtensions para cargar la nueva DLL.

## ⚙️ Qué hace el script (en orden)

`build.ps1` (detalle en [[🛠️ build.ps1 (ES)|build.ps1]] y [[🏷️ Versionamento (ES)|Versionado]]):
1. Lee la versión actual del `.nuspec` y calcula `newVersion` (build +1).
2. Detecta cambios (fuentes/docs más nuevos que el último `.nupkg`); sin `-Force` y sin cambio → mantiene la versión y sale.
3. Cierra el GitExtensions si está en ejecución (libera la DLL).
4. Sella versión + fecha en los READMEs y en las notas del cofre; hace el bump del `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copia la DLL a `C:\Program Files\GitExtensions\Plugins\` (Admin) y a `tools\net9.0-windows\`.
7. `nuget pack` → `.nupkg` en la raíz (filtra el aviso NU5101) y elimina `.nupkg` antiguos.

## 🧰 Instalación manual (alternativa a build.ps1)

```powershell
tools\install.ps1      # requiere Admin — copia la DLL a la carpeta Plugins (también vía NuGet PMC)
tools\uninstall.ps1    # requiere Admin — elimina la DLL
tools\update-dll.ps1   # actualiza solo la DLL en la carpeta Plugins
```
`install.ps1` corre de dos formas: standalone (`cd tools; .\install.ps1`) o vía **NuGet Package Manager Console** (`Install-Package GitExtensions.ZimerfeldCommitMsg -Source C:\NUGET`). Si la DLL no existe en `bin\Release`, dispara un `build.ps1 -Force`.

## 🧪 Tests

```powershell
dotnet test tests\GitExtensions.ZimerfeldCommitMsg.Tests
```
Suite **xUnit**: extracción de comentarios, derivación de conceptos, `.zimerfeldcommitmsg.json`, traducción EN→PT/ES. Las funciones puras del generador se exponen por `InternalsVisibleTo`.

## 🚦 Flags y requisitos
- `-Force` — recompila/reempaqueta incluso sin cambio detectado.
- **.NET 9 SDK** (`dotnet`) obligatorio.
- **Administrador** obligatorio para el deploy en `C:\Program Files\GitExtensions\Plugins\`. Sin Admin, el paso de deploy se omite con aviso.
- **GitExtensions 6.x (.NET 9)** instalado; `git` en el `PATH`.

## 🔀 Reglas que respeta (GitFlow)
- Desarrollo en **feature branch**; no commitear/pushear sin petición explícita.
- El sellado de versión/docs es local — el commit queda a criterio del usuario.

## 🩺 Troubleshooting
- **DLL bloqueada / no se actualiza:** cierra el GitExtensions (el `build.ps1` ya intenta cerrarlo) y ejecútalo de nuevo.
- **Deploy omitido:** ejecuta el PowerShell como **Administrador**.
- **El plugin no aparece:** confirma la DLL en `C:\Program Files\GitExtensions\Plugins\` y **reinicia** el GitExtensions. El nodo en Configuración → Plugins solo aparece tras reiniciar.

## 🔗 Enlaces
- [[🚀 Deploy em Produção (Prod) (ES)|Despliegue en Producción (Prod)]]
- [[🛠️ build.ps1 (ES)|build.ps1]]
- [[🏷️ Versionamento (ES)|Versionado]]
- [[📖 README — Instalação, Uso e Build (ES)|README — Instalación, Uso y Build]]

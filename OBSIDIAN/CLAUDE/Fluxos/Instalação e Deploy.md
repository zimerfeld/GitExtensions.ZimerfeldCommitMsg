---
tipo: fluxo
tags: [fluxo, instalação, deploy, powershell]
atualizado: 2026-05-22
---

# Fluxo: Instalação e Deploy

## Scripts disponíveis

| Script | Localização | Requer Admin | Propósito |
|---|---|---|---|
| `build.ps1` | raiz | Sim (deploy) | Build completo + versão + nupkg |
| `tools\install.ps1` | tools/ | Sim | Instala DLL no GitExtensions |
| `tools\uninstall.ps1` | tools/ | Sim | Remove DLL do GitExtensions |
| `tools\update-dll.ps1` | tools/ | Sim | Copia DLL sem alterar versão |

## Caminhos de instalação

```
DLL fonte (compilada):
  src\...\bin\Release\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll

Destino de runtime:
  C:\Program Files\GitExtensions\Plugins\GitExtensions.Plugins.ZimerfeldCommitMsg.dll
  (ou C:\Program Files (x86)\GitExtensions\Plugins\ como fallback)

Cópia para nupkg:
  tools\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll
```

## Fluxo install.ps1

```
1. Localiza DLL em tools\net9.0-windows\
2. Procura pasta Plugins do GitExtensions (x64 → x86)
3. Verifica permissão de Admin
4. Copy-Item -Force → pluginsDir
```

Se não encontrar pasta de plugins → aviso, sem erro. Se não tiver Admin → erro explícito com instruções.

![[screenshots/screenshotInstall.png]]

### uninstall.ps1 / update-dll.ps1

`uninstall.ps1` remove a DLL do diretório de plugins; `update-dll.ps1` recopia a DLL sem mexer na versão (deploy rápido em desenvolvimento).

![[screenshots/screenshotUninstall.png]]

![[screenshots/screenshotUpdate.png]]

## Fluxo via NuGet PMC

O `install.ps1` aceita parâmetros `$installPath`, `$toolsPath`, `$package`, `$project` (convenção NuGet). Quando chamado pelo NuGet Package Manager Console do Visual Studio, `$toolsPath` é passado automaticamente apontando para a pasta `tools\` do pacote.

## Geração do .nupkg

O `.nuspec` descreve o pacote. O `build.ps1` chama `nuget pack` apontando para o `.nuspec`. O pacote gerado fica na raiz: `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`.

Versões antigas do `.nupkg` são deletadas automaticamente.

![[screenshots/screenshotBuild.png]]

## Após instalação

Reiniciar o GitExtensions é necessário para carregar o plugin via MEF.

Verificar em: `Ferramentas → Plugins → ZimerfeldCommitMsg`

## Relacionado

- [[../Sistema/Versionamento]]
- [[../Arquivos-Chave/build.ps1]]

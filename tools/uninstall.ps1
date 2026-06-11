#Requires -Version 5.1
<#
.SYNOPSIS
    Remove o plugin ZimerfeldCommitMsg do GitExtensions sem causar danos ao programa.

.DESCRIPTION
    Pode ser executado de duas formas:

      Opcao A - standalone:
          cd C:\NUGET\ZimerfeldCommitMsg\tools
          .\uninstall.ps1

      Opcao B - via NuGet Package Manager Console (Visual Studio):
          Uninstall-Package GitExtensions.ZimerfeldCommitMsg
          (O NuGet invoca este script automaticamente.)

.NOTES
    Apenas o DLL do plugin e removido.
    O GitExtensions e seus dados nao sao afetados.
    Requer permissao de Administrador.
#>

param(
    # Provided automatically by NuGet PMC; ignored when run standalone
    $installPath,
    $toolsPath,
    $package,
    $project
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$dllName = "GitExtensions.Plugins.ZimerfeldCommitMsg.dll"

# -- Locate GitExtensions Plugins folder --------------------------------------

$candidates = @(
    "C:\Program Files\GitExtensions\Plugins",
    "C:\Program Files (x86)\GitExtensions\Plugins"
)

$pluginsDir = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $pluginsDir) {
    Write-Warning "Pasta de plugins do GitExtensions nao encontrada. Nada a remover."
    exit 0
}

$target = Join-Path $pluginsDir $dllName

if (-not (Test-Path $target)) {
    Write-Host "Plugin nao esta instalado em '$pluginsDir'. Nada a remover." -ForegroundColor Yellow
    exit 0
}

# -- Check administrator rights -----------------------------------------------

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Warning @"
Este script precisa de permissao de Administrador para remover:
  $target

Re-execute o PowerShell como Administrador e repita:
  cd $PSScriptRoot
  .\uninstall.ps1
"@
    exit 1
}

# -- Close GitExtensions if running -------------------------------------------
# O DLL fica bloqueado enquanto o GitExtensions estiver aberto (carregado em
# memoria), causando "Access to the path is denied" mesmo como Administrador.

$geProcs = Get-Process -Name "GitExtensions" -ErrorAction SilentlyContinue

if ($geProcs) {
    Write-Host "GitExtensions esta aberto. Fechando antes de remover o plugin..." -ForegroundColor Yellow

    # Tenta fechar de forma graciosa primeiro
    foreach ($p in $geProcs) {
        $null = $p.CloseMainWindow()
    }

    # Aguarda ate 10s pelo encerramento gracioso
    $deadline = (Get-Date).AddSeconds(10)
    while ((Get-Process -Name "GitExtensions" -ErrorAction SilentlyContinue) -and (Get-Date) -lt $deadline) {
        Start-Sleep -Milliseconds 300
    }

    # Se ainda persistir, forca o encerramento
    $still = Get-Process -Name "GitExtensions" -ErrorAction SilentlyContinue
    if ($still) {
        Write-Host "Forcando encerramento do GitExtensions..." -ForegroundColor Yellow
        $still | Stop-Process -Force -ErrorAction Stop
        Start-Sleep -Milliseconds 500
    }

    Write-Host "GitExtensions encerrado." -ForegroundColor Green
}

# -- Remove DLL ---------------------------------------------------------------

try {
    Remove-Item -Path $target -Force
    Write-Host ""
    Write-Host "✔  Plugin removido com sucesso:" -ForegroundColor Green
    Write-Host "   $target" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Os dados do GitExtensions nao foram afetados. Voce ja pode reabri-lo."
}
catch {
    Write-Error "Falha ao remover o arquivo: $_"
    exit 1
}

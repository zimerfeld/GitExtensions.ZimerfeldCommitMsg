param($installPath, $toolsPath, $package, $project)

# Auto-elevate quando nao estiver rodando como Administrador
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $args = "-ExecutionPolicy Bypass -File `"$PSCommandPath`""
    Start-Process powershell -Verb RunAs -ArgumentList $args
    exit
}

# Quando executado diretamente (fora do NuGet Package Manager Console),
# $toolsPath e nulo — usa o diretorio do proprio script como base.
if (-not $toolsPath) {
    $toolsPath = $PSScriptRoot
}

$pluginsFolder = "C:\Program Files\GitExtensions\Plugins"
$dllName       = "GitExtensions.Plugins.ZimerfeldCommitMsg.dll"
$source        = Join-Path $toolsPath "net9.0-windows\$dllName"

if (-not (Test-Path $source)) {
    Write-Warning "DLL nao encontrada em: $source"
    Read-Host "Pressione Enter para sair"
    exit 1
}

if (-not (Test-Path $pluginsFolder)) {
    $pluginsFolder = "C:\Program Files (x86)\GitExtensions\Plugins"
}

if (-not (Test-Path $pluginsFolder)) {
    Write-Warning "Pasta de plugins do GitExtensions nao encontrada."
    Write-Host "Copie manualmente o arquivo abaixo para a pasta Plugins do GitExtensions:"
    Write-Host "  $source"
    Read-Host "Pressione Enter para sair"
    exit 1
}

Copy-Item -Path $source -Destination $pluginsFolder -Force
Write-Host "GitExtensions.ZimerfeldCommitMsg instalado com sucesso em: $pluginsFolder"
Write-Host "Reinicie o GitExtensions para ativar o plugin."
Read-Host "Pressione Enter para sair"

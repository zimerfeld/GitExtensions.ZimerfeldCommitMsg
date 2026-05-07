param($installPath, $toolsPath, $package, $project)

# Auto-elevate quando nao estiver rodando como Administrador
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $args = "-ExecutionPolicy Bypass -File `"$PSCommandPath`""
    Start-Process powershell -Verb RunAs -ArgumentList $args
    exit
}

$dllName   = "GitExtensions.Plugins.ZimerfeldCommitMsg.dll"
$locations = @(
    "C:\Program Files\GitExtensions\Plugins\$dllName",
    "C:\Program Files (x86)\GitExtensions\Plugins\$dllName"
)

$removed = $false
foreach ($path in $locations) {
    if (Test-Path $path) {
        Remove-Item -Path $path -Force
        Write-Host "Plugin removido de: $path"
        $removed = $true
    }
}

if (-not $removed) {
    Write-Host "Plugin nao encontrado nos locais padrao. Nenhum arquivo removido."
}

Read-Host "Pressione Enter para sair"

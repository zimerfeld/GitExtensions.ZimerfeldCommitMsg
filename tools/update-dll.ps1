# Copia o DLL mais recente da pasta de build para os plugins do GitExtensions (requer Admin).
# Se a DLL de bin\Release estiver ausente ou mais antiga que fontes/recursos, executa build.ps1 -Force.

param([string]$Config = "Release")

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$projectRoot = Join-Path $repoRoot "src\GitExtensions.ZimerfeldCommitMsg"
$dll = Join-Path $projectRoot "bin\$Config\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll"
$buildScript = Join-Path $repoRoot "build.ps1"

$inputs = Get-ChildItem $projectRoot -Recurse -File -Include *.cs,*.csproj,*.resx,*.png |
          Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
$newestInput = ($inputs | Measure-Object -Property LastWriteTimeUtc -Maximum).Maximum

if (-not (Test-Path $dll)) {
    Write-Warning "DLL nao encontrada em bin\$Config. Executando build forcado..."
    & $buildScript -Force
    $buildSucceeded = $?
    $buildExitCode = Get-Variable -Name LASTEXITCODE -ValueOnly -ErrorAction SilentlyContinue
    if ((-not $buildSucceeded) -or ($buildExitCode -is [int] -and $buildExitCode -ne 0)) { exit 1 }
}
elseif ($newestInput -and ((Get-Item $dll).LastWriteTimeUtc -lt $newestInput)) {
    Write-Warning "DLL em bin\$Config esta mais antiga que as fontes/recursos em tools\net9.0-windows."
    & $buildScript -Force
    $buildSucceeded = $?
    $buildExitCode = Get-Variable -Name LASTEXITCODE -ValueOnly -ErrorAction SilentlyContinue
    if ((-not $buildSucceeded) -or ($buildExitCode -is [int] -and $buildExitCode -ne 0)) { exit 1 }
}

if (-not (Test-Path $dll)) {
    Write-Warning "DLL nao encontrada apos o build: $dll"
    exit 1
}

if ($newestInput -and ((Get-Item $dll).LastWriteTimeUtc -lt $newestInput)) {
    Write-Warning "DLL continua mais antiga que as fontes/recursos apos o build: $dll"
    exit 1
}

$dest = "C:\Program Files\GitExtensions\Plugins"
if (-not (Test-Path $dest)) { $dest = "C:\Program Files (x86)\GitExtensions\Plugins" }

if (-not (Test-Path $dest)) {
    Write-Warning "Pasta de plugins nao encontrada."
    exit 1
}

# -- Fecha GitExtensions se estiver aberto ------------------------------------
# A DLL fica bloqueada enquanto o GitExtensions estiver aberto (carregada em
# memoria), causando "The process cannot access the file ... being used by
# another process" no Copy-Item.

$geProcs = Get-Process -Name "GitExtensions" -ErrorAction SilentlyContinue

if ($geProcs) {
    Write-Host "GitExtensions esta aberto. Fechando antes de atualizar a DLL..." -ForegroundColor Yellow

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

Copy-Item $dll $dest -Force
Write-Host ""
Write-Host "DLL atualizada com sucesso em:" -ForegroundColor Green
Write-Host "  $dest" -ForegroundColor Cyan
Write-Host ""
Write-Host "Reinicie o GitExtensions para aplicar."

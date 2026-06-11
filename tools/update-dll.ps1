# Copia o DLL mais recente da pasta de build para os plugins do GitExtensions (requer Admin)
# Uso rapido durante desenvolvimento, sem incrementar versao ou fazer pack.

param([string]$Config = "Release")

$dll = "$PSScriptRoot\..\src\GitExtensions.ZimerfeldCommitMsg\bin\$Config\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll"

if (-not (Test-Path $dll)) {
    Write-Warning "DLL nao encontrada: $dll"
    Write-Host "Execute primeiro: dotnet build -c $Config"
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

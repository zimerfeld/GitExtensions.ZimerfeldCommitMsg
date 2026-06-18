#Requires -Version 5.1
<#
.SYNOPSIS
    Carimba a nova versao/data nos docs (READMEs + cofre Obsidian), da' bump na
    versao (major.minor.build) no nuspec/csproj, compila o plugin e gera o .nupkg.
    Executar como Administrador para tambem fazer o deploy em GitExtensions.
#>

param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$nuspec  = "$PSScriptRoot\src\GitExtensions.ZimerfeldCommitMsg\GitExtensions.ZimerfeldCommitMsg.nuspec"
$csproj  = "$PSScriptRoot\src\GitExtensions.ZimerfeldCommitMsg\GitExtensions.ZimerfeldCommitMsg.csproj"
$outDir  = $PSScriptRoot

# -- 1. Ler versao atual do nuspec ---------------------------------------------
[xml]$spec = Get-Content $nuspec -Encoding UTF8
$current   = $spec.package.metadata.version
$parts     = $current -split '\.'
if ($parts.Count -ne 3) {
    Write-Error "Versao '$current' nao esta no formato major.minor.build"
    exit 1
}
$major      = [int]$parts[0]
$minor      = [int]$parts[1]
$build      = [int]$parts[2] + 1
$newVersion = "$major.$minor.$build"

# -- 1b. Detectar mudancas -----------------------------------------------------
# Incrementa a versao (e recompila/empacota) se qualquer ENTRADA DO PACOTE for mais
# nova que o .nupkg ja' gerado. Entradas = fontes (*.cs/*.csproj/*.nuspec/*.resx/*.png),
# QUALQUER *.md do repositorio (raiz e subpastas) e os demais textos empacotados
# (LICENSE, scripts de install). Assim, editar qualquer .md ou texto tambem gera pacote
# novo. Sem mudancas => mantem a versao e encerra.
#
# Comparamos contra o .nupkg (e nao a DLL) de proposito: quando so' um texto muda, o
# build incremental do dotnet nao regenera a DLL (timestamp antigo), o que faria o
# script rebuildar eternamente. O .nupkg e' sempre reescrito no pack, sendo a
# referencia correta para "ja' empacotado nesta forma".
$srcRoot = "$PSScriptRoot\src\GitExtensions.ZimerfeldCommitMsg"
$inputs  = @()
$inputs += Get-ChildItem $srcRoot -Recurse -File -Include *.cs,*.csproj,*.nuspec,*.resx,*.png |
           Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
# Qualquer *.md em todo o repositorio (raiz e subpastas) conta como entrada, para nao
# precisar manter uma lista fixa ao adicionar/editar READMEs ou outras notas .md.
$inputs += Get-ChildItem $PSScriptRoot -Recurse -File -Include *.md |
           Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
$inputs += @(
    "$PSScriptRoot\LICENSE.txt",
    "$PSScriptRoot\tools\install.ps1",
    "$PSScriptRoot\tools\uninstall.ps1"
) | Where-Object { Test-Path $_ } | Get-Item
$newestInput = ($inputs | Measure-Object -Property LastWriteTimeUtc -Maximum).Maximum

$lastPkg = Get-ChildItem "$outDir\GitExtensions.ZimerfeldCommitMsg.*.nupkg" -ErrorAction SilentlyContinue |
           Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1

if ((-not $Force) -and $lastPkg -and $newestInput -le $lastPkg.LastWriteTimeUtc) {
    Write-Host ""
    Write-Host "Nenhuma mudanca detectada (fontes ou textos) -- versao mantida em $current (build/pack ignorados)." -ForegroundColor Cyan
    exit 0
}

if ($Force) {
    Write-Host "Build forcado: ignorando verificacao incremental." -ForegroundColor Yellow
}

Write-Host "Versao: $current  ->  " -NoNewline
Write-Host $newVersion -ForegroundColor Green

# -- 1c. Fechar GitExtensions e plugins antes de compilar ----------------------
# Feito so' quando ha' mudancas, para nao encerrar o GitExtensions num run sem efeito.
$geProcs = Get-Process -Name GitExtensions -ErrorAction SilentlyContinue
if ($geProcs) {
    Write-Host "Fechando GitExtensions e plugins..."

    # Tenta fechar de forma graciosa primeiro
    foreach ($p in $geProcs) {
        $null = $p.CloseMainWindow()
    }

    # Aguarda ate 10s pelo encerramento gracioso
    $deadline = (Get-Date).AddSeconds(10)
    while ((Get-Process -Name GitExtensions -ErrorAction SilentlyContinue) -and (Get-Date) -lt $deadline) {
        Start-Sleep -Milliseconds 300
    }

    # Se ainda persistir, forca o encerramento
    $still = Get-Process -Name GitExtensions -ErrorAction SilentlyContinue
    if ($still) {
        Write-Host "Forcando encerramento do GitExtensions..."
        $still | Stop-Process -Force
        Start-Sleep -Milliseconds 500
    }

    Write-Host "GitExtensions encerrado."
} else {
    Write-Host "GitExtensions nao esta em execucao."
}

# -- 2. Atualizar docs (READMEs + cofre Obsidian) ------------------------------
# Os documentos sao escritos PRIMEIRO; o bump da fonte da verdade (nuspec/csproj)
# vem logo em seguida (passos 3 e 4). Assim a versao/data so' chegam ao
# nuspec/csproj depois que README e cofre Obsidian ja' refletem a nova versao.
$today = (Get-Date).ToString("yyyy-MM-dd")

# 2a. READMEs (raiz, todas as variantes localizadas)
$readmeDocs = @(
    "$PSScriptRoot\README.md",
    "$PSScriptRoot\README.pt-BR.md",
    "$PSScriptRoot\README.en-US.md"
)
foreach ($readmeDoc in $readmeDocs) {
    if (Test-Path $readmeDoc) {
        $content = Get-Content $readmeDoc -Raw -Encoding UTF8
        $content = $content -replace '\*\*Version / Vers[aã]o:\*\* [^\r\n]+', "**Version / Versão:** $newVersion"
        $content = $content -replace '\*\*Updated / Atualizado em:\*\* [^\r\n]+', "**Updated / Atualizado em:** $today"
        $content = $content -replace '\*\*Versão:\*\* [^\r\n]+', "**Versão:** $newVersion"
        $content = $content -replace '\*\*Atualizado em:\*\* [^\r\n]+', "**Atualizado em:** $today"
        $content = $content -replace '\*\*Version:\*\* [^\r\n]+', "**Version:** $newVersion"
        $content = $content -replace '\*\*Updated:\*\* [^\r\n]+', "**Updated:** $today"
        [System.IO.File]::WriteAllText($readmeDoc, $content, [System.Text.Encoding]::UTF8)
        Write-Host "$(Split-Path $readmeDoc -Leaf) atualizado para $newVersion ($today)"
    }
}

# 2b. Cofre Obsidian -- somente as notas que carimbam a versao ATUAL do projeto.
# Notas de sessao/historico mencionam versoes antigas e NAO entram aqui de proposito.
$obsidianDocs = @(
    "$PSScriptRoot\OBSIDIAN\CLAUDE\01 - Projetos\GitExtensions.ZimerfeldCommitMsg.md",
    "$PSScriptRoot\OBSIDIAN\CLAUDE\02 - Conhecimento\README — Instalação, Uso e Build.md",
    "$PSScriptRoot\OBSIDIAN\CLAUDE\Sistema\Versionamento.md",
    "$PSScriptRoot\OBSIDIAN\CLAUDE\Sistema\Visão Geral.md"
)
$verBold   = '**' + $newVersion + '**'
$verTicked = '`'  + $newVersion + '`'
foreach ($obsDoc in $obsidianDocs) {
    if (Test-Path $obsDoc) {
        $content = Get-Content $obsDoc -Raw -Encoding UTF8
        # Frontmatter YAML
        $content = $content -replace '(?m)^versao:\s*[^\r\n]+',     "versao: $newVersion"
        $content = $content -replace '(?m)^atualizado:\s*[^\r\n]+', "atualizado: $today"
        # Corpo -- tres formatos de "versao atual" usados nas notas
        $content = $content -replace 'Versão atual:\s*\*\*[^\*\r\n]+\*\*',     "Versão atual: $verBold"
        $content = $content -replace '\*\*Versão atual:\*\*\s*`[^`\r\n]+`',    "**Versão atual:** $verTicked"
        $content = $content -replace '(\|\s*Versão atual\s*\|\s*)`[^`\r\n]+`', ('${1}' + $verTicked)
        [System.IO.File]::WriteAllText($obsDoc, $content, [System.Text.Encoding]::UTF8)
        Write-Host "Obsidian: $(Split-Path $obsDoc -Leaf) atualizado para $newVersion ($today)"
    }
}

# -- 3. Bump da versao no nuspec (fonte da verdade) ----------------------------
$spec.package.metadata.version = $newVersion
$spec.Save($nuspec)

# -- 4. Bump da versao no csproj -----------------------------------------------
$csprojContent = Get-Content $csproj -Raw -Encoding UTF8
$csprojContent = $csprojContent -replace '<Version>[^<]+</Version>', "<Version>$newVersion</Version>"
[System.IO.File]::WriteAllText($csproj, $csprojContent, [System.Text.Encoding]::UTF8)

# -- 5. Build ------------------------------------------------------------------
Write-Host "Compilando..."
$buildArgs = @("build", $csproj, "-c", "Release", "--nologo", "-v", "minimal")
if ($Force) {
    $buildArgs += "--no-incremental"
}
$buildOutput = & dotnet @buildArgs 2>&1
$buildExit   = $LASTEXITCODE
$buildOutput | ForEach-Object {
    $line = "$_"
    if     ($line -match 'GitExtensions\.Extensibility') { return }                       # oculta eventos de prebuild
    elseif ($line -match 'Error\(s\)')                   { Write-Host $line -ForegroundColor Red }
    elseif ($line -match 'Warning\(s\)')                 { Write-Host $line -ForegroundColor Yellow }
    elseif ($line -match 'Build succeeded\.')            { Write-Host $line -ForegroundColor Green }
    else                                                 { Write-Host $line }
}

# Analisa o resultado do build a partir dos diagnosticos emitidos (formato MSBuild:
# "arquivo(linha,col): error CSxxxx" / "... : warning CSxxxx").
$buildText    = $buildOutput | Out-String
$errorCount   = ([regex]::Matches($buildText, '(?im):\s*error\s')).Count
$warningCount = ([regex]::Matches($buildText, '(?im):\s*warning\s')).Count

if ($buildExit -ne 0 -or $errorCount -gt 0) {
    Write-Host "Build falhou: $errorCount erro(s)." -ForegroundColor Red
    exit 1
}
elseif ($warningCount -gt 0) {
    Write-Host "Build concluido com $warningCount aviso(s)." -ForegroundColor Yellow
}
else {
    Write-Host "Build concluido com sucesso (nenhum erro ou aviso)." -ForegroundColor Green
}

# -- 6. Deploy (requer Admin) --------------------------------------------------
$pluginsDir = "C:\Program Files\GitExtensions\Plugins"
if (-not (Test-Path $pluginsDir)) {
    $pluginsDir = "C:\Program Files (x86)\GitExtensions\Plugins"
}
$dll = "$PSScriptRoot\src\GitExtensions.ZimerfeldCommitMsg\bin\Release\net9.0-windows\GitExtensions.Plugins.ZimerfeldCommitMsg.dll"

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)

if ($isAdmin -and (Test-Path $pluginsDir)) {
    Copy-Item $dll $pluginsDir -Force
    Write-Host "Plugin instalado em: $pluginsDir"
} else {
    Write-Warning "Sem permissao de Admin ou pasta nao encontrada -- deploy pulado."
    Write-Host "  Copie manualmente: $dll"
    Write-Host "  Para: $pluginsDir"
}

# Atualiza copia na pasta tools (usada pelo nupkg)
$toolsTarget = "$PSScriptRoot\tools\net9.0-windows"
if (-not (Test-Path $toolsTarget)) { New-Item -ItemType Directory $toolsTarget | Out-Null }
Copy-Item $dll $toolsTarget -Force

# -- 7. Pack -------------------------------------------------------------------
Write-Host "Gerando pacote $newVersion..."

# Resolve nuget.exe: PATH -> tools\ local -> download automatico
$nugetCmd = Get-Command nuget -ErrorAction SilentlyContinue
$nugetExe = if ($nugetCmd) { $nugetCmd.Source } else { $null }
if (-not $nugetExe) {
    $nugetExe = Join-Path $PSScriptRoot "tools\nuget.exe"
    if (-not (Test-Path $nugetExe)) {
        Write-Host "nuget.exe nao encontrado - baixando para tools\nuget.exe..."
        Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" `
                          -OutFile $nugetExe -UseBasicParsing
        Write-Host "Download concluido."
    }
}

# NU5101 (DLL diretamente em lib\) e' INTENCIONAL: o GitExtensions Plugin Manager so'
# extrai o grupo lib cujo framework esta na sua lista de monikers { net5.0..net10.0, any,
# netstandard2.0 }. lib\ raiz = grupo "any" (extraido); uma subpasta net9.0-windows NAO
# esta na lista e quebraria a instalacao. Por isso filtramos esse aviso especifico.
& $nugetExe pack $nuspec -OutputDirectory $outDir 2>&1 |
    ForEach-Object {
        if ($_ -notmatch 'NU5101') { Write-Host $_ }   # NU5101 e' intencional -- ocultado do output
    }
if ($LASTEXITCODE -ne 0) { Write-Error "nuget pack falhou."; exit 1 }

# Remove pacotes de versoes anteriores
Get-ChildItem "$outDir\GitExtensions.ZimerfeldCommitMsg.*.nupkg" |
    Where-Object { $_.Name -notlike "*$newVersion*" } |
    Remove-Item -Force

Write-Host ""
Write-Host "Concluido: GitExtensions.ZimerfeldCommitMsg.$newVersion.nupkg"

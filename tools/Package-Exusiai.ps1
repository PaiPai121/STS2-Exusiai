param(
    [string]$Configuration = "Debug",
    [switch]$SkipBuild,
    [switch]$SkipPckExport,
    [string]$Exporter = "D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe"
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $projectRoot "Exusiai.json"
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$version = [string]$manifest.version
if ([string]::IsNullOrWhiteSpace($version)) {
    throw "Exusiai.json does not define a version."
}

if (-not $SkipBuild) {
    Push-Location $projectRoot
    try {
        dotnet build
    }
    finally {
        Pop-Location
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE."
    }
}

if (-not $SkipPckExport) {
    if (-not (Test-Path -LiteralPath $Exporter -PathType Leaf)) {
        throw "PCK exporter not found: $Exporter"
    }

    & $Exporter --headless --path $projectRoot --export-pack BasicExport (Join-Path $projectRoot "Exusiai.pck")
    if ($LASTEXITCODE -ne 0) {
        throw "PCK export failed with exit code $LASTEXITCODE."
    }
}

$dllPath = Join-Path $projectRoot ".godot\mono\temp\bin\$Configuration\Exusiai.dll"
$pckPath = Join-Path $projectRoot "Exusiai.pck"
$requiredFiles = @($dllPath, $pckPath, $manifestPath)
foreach ($path in $requiredFiles) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required release file is missing: $path"
    }
}

$distDir = Join-Path $projectRoot "dist"
$stagingRoot = Join-Path $distDir "staging"
$stagingModDir = Join-Path $stagingRoot "Exusiai"
$zipPath = Join-Path $distDir "Exusiai-$version.zip"

if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $stagingModDir -Force | Out-Null

Copy-Item -LiteralPath $dllPath -Destination (Join-Path $stagingModDir "Exusiai.dll") -Force
Copy-Item -LiteralPath $pckPath -Destination (Join-Path $stagingModDir "Exusiai.pck") -Force
Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $stagingModDir "Exusiai.json") -Force

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
Compress-Archive -LiteralPath $stagingModDir -DestinationPath $zipPath -CompressionLevel Optimal

$hash = Get-FileHash -LiteralPath $zipPath -Algorithm SHA256
$file = Get-Item -LiteralPath $zipPath
[pscustomobject]@{
    ZipPath = $file.FullName
    Version = $version
    SizeBytes = $file.Length
    Sha256 = $hash.Hash
}

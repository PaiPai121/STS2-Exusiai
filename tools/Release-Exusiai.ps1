param(
    [ValidateSet("Deployed", "Build")]
    [string]$Source = "Deployed",

    [string]$Description,
    [string]$ZipPath,
    [string]$ModDir = "D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\Exusiai",
    [string]$Configuration = "Debug",
    [string]$FileName = "Exusiai",
    [string]$FileCategory = "main",
    [string]$UpdateGroupId,
    [string]$PreviousVersionId,
    [string]$ApiKey,
    [string]$ConfigPath,

    [switch]$ArchiveExistingFile,
    [switch]$SkipValidate,
    [switch]$DryRun,
    [switch]$Yes
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$distDir = Join-Path $projectRoot "dist"
$manifestPath = Join-Path $projectRoot "Exusiai.json"

function Get-ReleaseVersion {
    param([Parameter(Mandatory = $true)][string]$ManifestPath)

    $manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
    $version = [string]$manifest.version
    if ([string]::IsNullOrWhiteSpace($version)) {
        throw "$ManifestPath does not define a version."
    }

    return $version
}

function New-ZipFromDeployedMod {
    param(
        [Parameter(Mandatory = $true)][string]$ModDir,
        [Parameter(Mandatory = $true)][string]$ZipPath
    )

    $requiredFiles = @(
        (Join-Path $ModDir "Exusiai.dll"),
        (Join-Path $ModDir "Exusiai.pck"),
        (Join-Path $ModDir "Exusiai.json")
    )
    foreach ($path in $requiredFiles) {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Required deployed release file is missing: $path"
        }
    }

    $stagingRoot = Join-Path $distDir "staging-release"
    $stagingModDir = Join-Path $stagingRoot "Exusiai"
    if (Test-Path -LiteralPath $stagingRoot) {
        Remove-Item -LiteralPath $stagingRoot -Recurse -Force
    }
    New-Item -ItemType Directory -Path $stagingModDir -Force | Out-Null

    Copy-Item -LiteralPath (Join-Path $ModDir "Exusiai.dll") -Destination (Join-Path $stagingModDir "Exusiai.dll") -Force
    Copy-Item -LiteralPath (Join-Path $ModDir "Exusiai.pck") -Destination (Join-Path $stagingModDir "Exusiai.pck") -Force
    Copy-Item -LiteralPath (Join-Path $ModDir "Exusiai.json") -Destination (Join-Path $stagingModDir "Exusiai.json") -Force

    if (Test-Path -LiteralPath $ZipPath) {
        Remove-Item -LiteralPath $ZipPath -Force
    }
    $zipDirectory = Split-Path -Parent $ZipPath
    if (-not (Test-Path -LiteralPath $zipDirectory -PathType Container)) {
        New-Item -ItemType Directory -Path $zipDirectory -Force | Out-Null
    }
    Compress-Archive -LiteralPath $stagingModDir -DestinationPath $ZipPath -CompressionLevel Optimal

    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}

function Test-ReleaseZip {
    param([Parameter(Mandatory = $true)][string]$ZipPath)

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($ZipPath)
    try {
        $entries = @($zip.Entries | ForEach-Object { $_.FullName -replace '/', '\' })
        $expected = @(
            "Exusiai\Exusiai.dll",
            "Exusiai\Exusiai.json",
            "Exusiai\Exusiai.pck"
        )

        foreach ($entry in $expected) {
            if ($entries -notcontains $entry) {
                throw "Release zip is missing $entry"
            }
        }

        $unexpected = @($entries | Where-Object { $_ -and ($expected -notcontains $_) })
        if ($unexpected.Count -gt 0) {
            throw "Release zip contains unexpected entries: $($unexpected -join ', ')"
        }
    }
    finally {
        $zip.Dispose()
    }
}

if (-not $SkipValidate) {
    Write-Host "Running release validation..."
    & (Join-Path $PSScriptRoot "Validate-Exusiai.ps1")
    if ($? -eq $false -or ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0)) {
        throw "Validate-Exusiai.ps1 failed with exit code $LASTEXITCODE."
    }
}

$version = Get-ReleaseVersion -ManifestPath $manifestPath
if ([string]::IsNullOrWhiteSpace($ZipPath)) {
    if ($DryRun) {
        $ZipPath = Join-Path (Join-Path $distDir "dry-run") "Exusiai-$version.zip"
    }
    else {
        $ZipPath = Join-Path $distDir "Exusiai-$version.zip"
    }
}

if ($Source -eq "Build") {
    Write-Host "Packaging from source build artifacts..."
    $packageArgs = @("-File", (Join-Path $PSScriptRoot "Package-Exusiai.ps1"), "-Configuration", $Configuration)
    & powershell -NoProfile -ExecutionPolicy Bypass @packageArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Package-Exusiai.ps1 failed with exit code $LASTEXITCODE."
    }
}
else {
    Write-Host "Packaging from deployed mod files: $ModDir"
    New-ZipFromDeployedMod -ModDir $ModDir -ZipPath $ZipPath
}

Test-ReleaseZip -ZipPath $ZipPath

$zip = Get-Item -LiteralPath $ZipPath
$hash = Get-FileHash -LiteralPath $ZipPath -Algorithm SHA256

if ([string]::IsNullOrWhiteSpace($Description)) {
    $Description = "Exusiai $version release."
}

$summary = [pscustomobject]@{
    Source = $Source
    ZipPath = $zip.FullName
    Version = $version
    SizeBytes = $zip.Length
    Sha256 = $hash.Hash
    DryRun = [bool]$DryRun
}
$summary | Format-List

if ($DryRun) {
    Write-Host "Dry run complete. Nexus upload was not started."
    return
}

if (-not $Yes) {
    $answer = Read-Host "Publish this package to Nexus? Type YES to continue"
    if ($answer -ne "YES") {
        Write-Host "Publish cancelled."
        return
    }
}

$publishArgs = @(
    "-File", (Join-Path $PSScriptRoot "Publish-Nexus.ps1"),
    "-ZipPath", $zip.FullName,
    "-Version", $version,
    "-FileName", $FileName,
    "-Description", $Description,
    "-FileCategory", $FileCategory
)
if (-not [string]::IsNullOrWhiteSpace($UpdateGroupId)) {
    $publishArgs += @("-UpdateGroupId", $UpdateGroupId)
}
if (-not [string]::IsNullOrWhiteSpace($PreviousVersionId)) {
    $publishArgs += @("-PreviousVersionId", $PreviousVersionId)
}
if (-not [string]::IsNullOrWhiteSpace($ApiKey)) {
    $publishArgs += @("-ApiKey", $ApiKey)
}
if (-not [string]::IsNullOrWhiteSpace($ConfigPath)) {
    $publishArgs += @("-ConfigPath", $ConfigPath)
}
if ($ArchiveExistingFile) {
    $publishArgs += "-ArchiveExistingFile"
}

& powershell -NoProfile -ExecutionPolicy Bypass @publishArgs
if ($LASTEXITCODE -ne 0) {
    throw "Publish-Nexus.ps1 failed with exit code $LASTEXITCODE."
}

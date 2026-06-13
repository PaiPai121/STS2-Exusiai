param(
    [Parameter(Mandatory = $true)]
    [string]$ZipPath,

    [string]$Version,
    [string]$FileName = "Exusiai",
    [string]$Description,
    [string]$FileCategory = "main",
    [string]$UpdateGroupId,
    [string]$PreviousVersionId,
    [switch]$ArchiveExistingFile,
    [string]$ApiKey,
    [string]$ConfigPath
)

$ErrorActionPreference = "Stop"
$apiBase = "https://api.nexusmods.com/v3"
$projectRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot "NexusConfig.ps1")

$config = if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    Read-ExusiaiNexusConfig
}
else {
    Read-ExusiaiNexusConfig -ConfigPath $ConfigPath
}

$ApiKey = Resolve-ExusiaiNexusValue -ExplicitValue $ApiKey -EnvName "NEXUS_API_KEY" -ConfigKey "NEXUS_API_KEY" -Config $config
$UpdateGroupId = Resolve-ExusiaiNexusValue -ExplicitValue $UpdateGroupId -EnvName "NEXUS_UPDATE_GROUP_ID" -ConfigKey "NEXUS_UPDATE_GROUP_ID" -Config $config
$PreviousVersionId = Resolve-ExusiaiNexusValue -ExplicitValue $PreviousVersionId -EnvName "NEXUS_PREVIOUS_VERSION_ID" -ConfigKey "NEXUS_PREVIOUS_VERSION_ID" -Config $config

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Set NEXUS_API_KEY, pass -ApiKey, or run tools/Configure-Nexus.ps1. Do not commit API keys."
}
if ([string]::IsNullOrWhiteSpace($UpdateGroupId)) {
    throw "Set NEXUS_UPDATE_GROUP_ID, pass -UpdateGroupId, or run tools/Configure-Nexus.ps1."
}
if (-not (Test-Path -LiteralPath $ZipPath -PathType Leaf)) {
    throw "Zip file not found: $ZipPath"
}

if ([string]::IsNullOrWhiteSpace($Version)) {
    $manifest = Get-Content -LiteralPath (Join-Path $projectRoot "Exusiai.json") -Raw | ConvertFrom-Json
    $Version = [string]$manifest.version
}
if ([string]::IsNullOrWhiteSpace($Version)) {
    throw "Version was not provided and Exusiai.json does not define one."
}
if ([string]::IsNullOrWhiteSpace($Description)) {
    $Description = "Exusiai $Version release."
}

$headers = @{
    apikey = $ApiKey
}

function Invoke-NexusJson {
    param(
        [Parameter(Mandatory = $true)][string]$Method,
        [Parameter(Mandatory = $true)][string]$Uri,
        [object]$Body
    )

    $params = @{
        Method = $Method
        Uri = $Uri
        Headers = $headers
    }
    if ($null -ne $Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 8)
    }
    Invoke-RestMethod @params
}

$zip = Get-Item -LiteralPath $ZipPath
$curl = Get-Command curl.exe -ErrorAction SilentlyContinue
if (-not $curl) {
    throw "curl.exe is required for Nexus presigned multipart uploads."
}

Write-Host "Creating Nexus multipart upload session for $($zip.Name)..."
$uploadResponse = Invoke-NexusJson -Method Post -Uri "$apiBase/uploads/multipart" -Body @{
    size_bytes = $zip.Length
    filename = $zip.Name
}
$upload = $uploadResponse.data
if (-not $upload.part_presigned_urls -or [string]::IsNullOrWhiteSpace([string]$upload.complete_presigned_url)) {
    throw "Nexus did not return multipart upload URLs."
}

$partSize = [int64]$upload.part_size_bytes
if ($partSize -le 0) {
    throw "Nexus returned an invalid multipart part size: $partSize"
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("exusiai-nexus-upload-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null
try {
    $parts = @()
    $buffer = New-Object byte[] (4MB)
    $inputStream = [System.IO.File]::OpenRead($zip.FullName)
    try {
        $partNumber = 1
        while ($inputStream.Position -lt $inputStream.Length) {
            $partPath = Join-Path $tempRoot ("part-{0:D4}.bin" -f $partNumber)
            $remainingInPart = [Math]::Min($partSize, $inputStream.Length - $inputStream.Position)
            $outputStream = [System.IO.File]::Create($partPath)
            try {
                while ($remainingInPart -gt 0) {
                    $readSize = [int][Math]::Min($buffer.Length, $remainingInPart)
                    $read = $inputStream.Read($buffer, 0, $readSize)
                    if ($read -le 0) {
                        break
                    }
                    $outputStream.Write($buffer, 0, $read)
                    $remainingInPart -= $read
                }
            }
            finally {
                $outputStream.Dispose()
            }

            $parts += [pscustomobject]@{
                Number = $partNumber
                Path = $partPath
                Url = [string]$upload.part_presigned_urls[$partNumber - 1]
                HeaderPath = Join-Path $tempRoot ("part-{0:D4}.headers" -f $partNumber)
            }
            $partNumber++
        }
    }
    finally {
        $inputStream.Dispose()
    }

    foreach ($part in $parts) {
        Write-Host "Uploading part $($part.Number) of $($parts.Count)..."
        & $curl.Source `
            --fail `
            --silent `
            --show-error `
            --request PUT `
            --dump-header $part.HeaderPath `
            --upload-file $part.Path `
            $part.Url
        if ($LASTEXITCODE -ne 0) {
            throw "Multipart part $($part.Number) upload failed with exit code $LASTEXITCODE."
        }

        $etagLine = Get-Content -LiteralPath $part.HeaderPath | Where-Object { $_ -match '^ETag:\s*(.+)\s*$' } | Select-Object -Last 1
        if (-not $etagLine -or $etagLine -notmatch '^ETag:\s*(.+)\s*$') {
            throw "Multipart part $($part.Number) response did not include an ETag."
        }
        $part | Add-Member -NotePropertyName ETag -NotePropertyValue ($Matches[1].Trim()) -Force
    }

    $xmlPath = Join-Path $tempRoot "complete-multipart.xml"
    $xmlLines = @("<CompleteMultipartUpload>")
    foreach ($part in $parts) {
        $escapedEtag = [System.Security.SecurityElement]::Escape($part.ETag)
        $xmlLines += "  <Part>"
        $xmlLines += "    <PartNumber>$($part.Number)</PartNumber>"
        $xmlLines += "    <ETag>$escapedEtag</ETag>"
        $xmlLines += "  </Part>"
    }
    $xmlLines += "</CompleteMultipartUpload>"
    Set-Content -LiteralPath $xmlPath -Value $xmlLines -Encoding ASCII

    Write-Host "Completing multipart upload $($upload.id)..."
    & $curl.Source `
        --fail `
        --silent `
        --show-error `
        --request POST `
        --header "Content-Type: application/xml" `
        --data-binary "@$xmlPath" `
        ([string]$upload.complete_presigned_url) | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Multipart completion failed with exit code $LASTEXITCODE."
    }
}
finally {
    if (Test-Path -LiteralPath $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}

Write-Host "Finalising upload $($upload.id)..."
Invoke-NexusJson -Method Post -Uri "$apiBase/uploads/$($upload.id)/finalise" | Out-Null

Write-Host "Waiting for upload to become available..."
$deadline = (Get-Date).AddMinutes(5)
do {
    Start-Sleep -Seconds 3
    $stateResponse = Invoke-NexusJson -Method Get -Uri "$apiBase/uploads/$($upload.id)"
    $state = [string]$stateResponse.data.state
    Write-Host "Upload state: $state"
    if ($state -eq "available") {
        break
    }
} while ((Get-Date) -lt $deadline)

if ($state -ne "available") {
    throw "Upload did not become available before timeout."
}

$body = @{
    upload_id = $upload.id
    name = $FileName
    version = $Version.TrimStart("v")
    description = $Description
    file_category = $FileCategory
    primary_mod_manager_download = $true
    allow_mod_manager_download = $true
    show_requirements_pop_up = $false
    archive_existing_file = [bool]$ArchiveExistingFile
    previous_version_id = $null
}
if (-not [string]::IsNullOrWhiteSpace($PreviousVersionId)) {
    $body.previous_version_id = $PreviousVersionId
}

Write-Host "Creating new Nexus update group version..."
$versionResponse = Invoke-NexusJson -Method Post -Uri "$apiBase/mod-file-update-groups/$UpdateGroupId/versions" -Body $body
$versionResponse.data

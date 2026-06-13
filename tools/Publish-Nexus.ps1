param(
    [Parameter(Mandatory = $true)]
    [string]$ZipPath,

    [string]$Version,
    [string]$FileName = "Exusiai",
    [string]$Description,
    [string]$FileCategory = "main",
    [string]$UpdateGroupId = $env:NEXUS_UPDATE_GROUP_ID,
    [string]$PreviousVersionId = $env:NEXUS_PREVIOUS_VERSION_ID,
    [switch]$ArchiveExistingFile,
    [string]$ApiKey = $env:NEXUS_API_KEY
)

$ErrorActionPreference = "Stop"
$apiBase = "https://api.nexusmods.com/v3"

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Set NEXUS_API_KEY or pass -ApiKey. Do not commit API keys."
}
if ([string]::IsNullOrWhiteSpace($UpdateGroupId)) {
    throw "Set NEXUS_UPDATE_GROUP_ID or pass -UpdateGroupId."
}
if (-not (Test-Path -LiteralPath $ZipPath -PathType Leaf)) {
    throw "Zip file not found: $ZipPath"
}

$projectRoot = Split-Path -Parent $PSScriptRoot
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

Write-Host "Creating Nexus upload session for $($zip.Name)..."
$uploadResponse = Invoke-NexusJson -Method Post -Uri "$apiBase/uploads" -Body @{
    size_bytes = $zip.Length
    filename = $zip.Name
}
$upload = $uploadResponse.data
if ([string]::IsNullOrWhiteSpace($upload.presigned_url)) {
    throw "Nexus did not return a presigned upload URL."
}

Write-Host "Uploading zip bytes..."
Invoke-RestMethod -Method Put -Uri $upload.presigned_url -InFile $zip.FullName -ContentType "application/zip" | Out-Null

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
$versionResponse = Invoke-NexusJson -Method Post -Uri "$apiBase/file-update-groups/$UpdateGroupId/versions" -Body $body
$versionResponse.data

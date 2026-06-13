param(
    [string]$ModUrl,
    [string]$GameDomain,
    [string]$GameScopedModId,
    [string]$ModId,
    [string]$UpdateGroupId,
    [string]$ApiKey,
    [string]$ConfigPath
)

$ErrorActionPreference = "Stop"
$apiBase = "https://api.nexusmods.com/v3"
. (Join-Path $PSScriptRoot "NexusConfig.ps1")

$config = if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    Read-ExusiaiNexusConfig
}
else {
    Read-ExusiaiNexusConfig -ConfigPath $ConfigPath
}

$ApiKey = Resolve-ExusiaiNexusValue -ExplicitValue $ApiKey -EnvName "NEXUS_API_KEY" -ConfigKey "NEXUS_API_KEY" -Config $config

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Set NEXUS_API_KEY, pass -ApiKey, or run tools/Configure-Nexus.ps1. Do not commit API keys."
}

if (-not [string]::IsNullOrWhiteSpace($ModUrl)) {
    $uri = [Uri]$ModUrl
    $segments = $uri.AbsolutePath.Trim("/").Split("/")
    if ($segments.Length -lt 3 -or $segments[1] -ne "mods") {
        throw "Expected a Nexus mod URL like https://www.nexusmods.com/<game_domain>/mods/<mod_id>."
    }
    $GameDomain = $segments[0]
    $GameScopedModId = $segments[2]
}

$headers = @{
    apikey = $ApiKey
}

function Invoke-NexusGet {
    param([Parameter(Mandatory = $true)][string]$Uri)
    Invoke-RestMethod -Method Get -Uri $Uri -Headers $headers
}

function Show-UpdateGroupVersions {
    param([Parameter(Mandatory = $true)][string]$GroupId)

    $versionsResponse = Invoke-NexusGet -Uri "$apiBase/file-update-groups/$GroupId/versions"
    $versions = @($versionsResponse.data.versions)
    if ($versions.Count -eq 0) {
        Write-Host "  No versions."
        return
    }

    $versions |
        Sort-Object { [decimal]$_.position } |
        Select-Object `
            @{Name = "VersionId"; Expression = { $_.id }},
            Position,
            @{Name = "FileId"; Expression = { $_.file.id }},
            @{Name = "FileGameScopedId"; Expression = { $_.file.game_scoped_id }},
            @{Name = "FileName"; Expression = { $_.file.name }},
            @{Name = "Version"; Expression = { $_.file.version }},
            @{Name = "Category"; Expression = { $_.file.file_category }} |
        Format-Table -AutoSize
}

if (-not [string]::IsNullOrWhiteSpace($UpdateGroupId)) {
    Write-Host "Update group versions for ${UpdateGroupId}:"
    Show-UpdateGroupVersions -GroupId $UpdateGroupId
    return
}

if ([string]::IsNullOrWhiteSpace($ModId)) {
    if ([string]::IsNullOrWhiteSpace($GameDomain) -or [string]::IsNullOrWhiteSpace($GameScopedModId)) {
        throw "Pass -ModUrl, -ModId, or both -GameDomain and -GameScopedModId."
    }

    $encodedGame = [Uri]::EscapeDataString($GameDomain)
    $encodedScopedId = [Uri]::EscapeDataString($GameScopedModId)
    $modResponse = Invoke-NexusGet -Uri "$apiBase/games/$encodedGame/mods/$encodedScopedId"
    $mod = $modResponse.data
    $ModId = [string]$mod.id

    Write-Host "Resolved mod:"
    [pscustomobject]@{
        ModId = $ModId
        GameDomain = $GameDomain
        GameScopedModId = $mod.game_scoped_id
        Name = $mod.name
    } | Format-List
}

$groupsResponse = Invoke-NexusGet -Uri "$apiBase/mods/$ModId/file-update-groups"
$groups = @($groupsResponse.data.groups)

if ($groups.Count -eq 0) {
    Write-Host "No file update groups found for mod id $ModId."
    return
}

foreach ($group in $groups) {
    Write-Host ""
    Write-Host "Update group:"
    [pscustomobject]@{
        GroupId = $group.id
        Name = $group.name
        Active = $group.is_active
        Versions = $group.versions_count
        Archived = $group.archived_count
        LastUploadedAt = $group.last_file_uploaded_at
    } | Format-List

    Show-UpdateGroupVersions -GroupId $group.id
}

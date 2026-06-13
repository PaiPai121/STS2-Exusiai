param(
    [string]$ModUrl,
    [string]$GameDomain,
    [string]$GameScopedModId,
    [string]$ModId,
    [string]$ApiKey = $env:NEXUS_API_KEY
)

$ErrorActionPreference = "Stop"
$apiBase = "https://api.nexusmods.com/v3"

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Set NEXUS_API_KEY or pass -ApiKey. Do not commit API keys."
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

    $versionsResponse = Invoke-NexusGet -Uri "$apiBase/file-update-groups/$($group.id)/versions"
    $versions = @($versionsResponse.data.versions)
    if ($versions.Count -eq 0) {
        Write-Host "  No versions."
        continue
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

param(
    [string]$ApiKey,
    [string]$UpdateGroupId,
    [string]$PreviousVersionId,
    [string]$ModUrl,
    [string]$GameDomain,
    [string]$GameScopedModId,
    [string]$ModId,
    [string]$ConfigPath,
    [switch]$ProcessOnly
)

$ErrorActionPreference = "Stop"
$apiBase = "https://api.nexusmods.com/v3"
. (Join-Path $PSScriptRoot "NexusConfig.ps1")

function Read-PlainSecret {
    param([Parameter(Mandatory = $true)][string]$Prompt)

    $secure = Read-Host -Prompt $Prompt -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try {
        return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

function Invoke-NexusGet {
    param(
        [Parameter(Mandatory = $true)][string]$Uri,
        [Parameter(Mandatory = $true)][string]$ApiKey
    )

    Invoke-RestMethod -Method Get -Uri $Uri -Headers @{ apikey = $ApiKey }
}

function Resolve-NexusModId {
    param(
        [string]$ModUrl,
        [string]$GameDomain,
        [string]$GameScopedModId,
        [string]$ModId,
        [Parameter(Mandatory = $true)][string]$ApiKey
    )

    if (-not [string]::IsNullOrWhiteSpace($ModId)) {
        return $ModId
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

    if ([string]::IsNullOrWhiteSpace($GameDomain) -or [string]::IsNullOrWhiteSpace($GameScopedModId)) {
        return $null
    }

    $encodedGame = [Uri]::EscapeDataString($GameDomain)
    $encodedScopedId = [Uri]::EscapeDataString($GameScopedModId)
    $modResponse = Invoke-NexusGet -Uri "$apiBase/games/$encodedGame/mods/$encodedScopedId" -ApiKey $ApiKey
    return [string]$modResponse.data.id
}

function Resolve-UpdateGroupId {
    param(
        [string]$UpdateGroupId,
        [string]$ModUrl,
        [string]$GameDomain,
        [string]$GameScopedModId,
        [string]$ModId,
        [Parameter(Mandatory = $true)][string]$ApiKey
    )

    if (-not [string]::IsNullOrWhiteSpace($UpdateGroupId)) {
        return $UpdateGroupId
    }

    $resolvedModId = Resolve-NexusModId -ModUrl $ModUrl -GameDomain $GameDomain -GameScopedModId $GameScopedModId -ModId $ModId -ApiKey $ApiKey
    if ([string]::IsNullOrWhiteSpace($resolvedModId)) {
        Write-Host "No update group id or mod URL was provided."
        Write-Host "Run Get-NexusUpdateGroups.ps1 later, then rerun this script with -UpdateGroupId."
        return $null
    }

    $groupsResponse = Invoke-NexusGet -Uri "$apiBase/mods/$resolvedModId/file-update-groups" -ApiKey $ApiKey
    $groups = @($groupsResponse.data.groups)
    if ($groups.Count -eq 0) {
        throw "No file update groups found for Nexus mod id $resolvedModId."
    }

    if ($groups.Count -eq 1) {
        $group = $groups[0]
        Write-Host "Using only update group: $($group.name) ($($group.id))"
        return [string]$group.id
    }

    Write-Host "Available update groups:"
    for ($i = 0; $i -lt $groups.Count; $i++) {
        $group = $groups[$i]
        Write-Host "[$($i + 1)] $($group.name) id=$($group.id) active=$($group.is_active) versions=$($group.versions_count)"
    }

    do {
        $choiceText = Read-Host "Choose update group number"
        $choice = 0
        $ok = [int]::TryParse($choiceText, [ref]$choice)
    } while (-not $ok -or $choice -lt 1 -or $choice -gt $groups.Count)

    return [string]$groups[$choice - 1].id
}

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    $existingApiKey = [Environment]::GetEnvironmentVariable("NEXUS_API_KEY")
    if (-not [string]::IsNullOrWhiteSpace($existingApiKey)) {
        $ApiKey = $existingApiKey
    }
}

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    $ApiKey = Read-PlainSecret -Prompt "Nexus API key"
}

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Nexus API key is required."
}

$UpdateGroupId = Resolve-UpdateGroupId `
    -UpdateGroupId $UpdateGroupId `
    -ModUrl $ModUrl `
    -GameDomain $GameDomain `
    -GameScopedModId $GameScopedModId `
    -ModId $ModId `
    -ApiKey $ApiKey

[Environment]::SetEnvironmentVariable("NEXUS_API_KEY", $ApiKey, "Process")
if (-not [string]::IsNullOrWhiteSpace($UpdateGroupId)) {
    [Environment]::SetEnvironmentVariable("NEXUS_UPDATE_GROUP_ID", $UpdateGroupId, "Process")
}
if (-not [string]::IsNullOrWhiteSpace($PreviousVersionId)) {
    [Environment]::SetEnvironmentVariable("NEXUS_PREVIOUS_VERSION_ID", $PreviousVersionId, "Process")
}

if ($ProcessOnly) {
    Write-Host "Configured Nexus release variables for this PowerShell process only."
    return
}

$values = @{
    NEXUS_API_KEY = $ApiKey
    NEXUS_UPDATE_GROUP_ID = $UpdateGroupId
}
if (-not [string]::IsNullOrWhiteSpace($PreviousVersionId)) {
    $values.NEXUS_PREVIOUS_VERSION_ID = $PreviousVersionId
}

$path = if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    Write-ExusiaiNexusConfig -Values $values
}
else {
    Write-ExusiaiNexusConfig -Values $values -ConfigPath $ConfigPath
}

Write-Host "Saved Nexus release configuration:"
[pscustomobject]@{
    ConfigPath = $path
    HasApiKey = -not [string]::IsNullOrWhiteSpace($ApiKey)
    UpdateGroupId = $UpdateGroupId
    PreviousVersionId = $PreviousVersionId
} | Format-List

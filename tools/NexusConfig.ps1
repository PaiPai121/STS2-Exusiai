$ErrorActionPreference = "Stop"

function Get-ExusiaiNexusConfigPath {
    if ($IsWindows -or [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
        $base = if ([string]::IsNullOrWhiteSpace($env:APPDATA)) {
            Join-Path $HOME "AppData\Roaming"
        }
        else {
            $env:APPDATA
        }
        return Join-Path $base "Exusiai\nexus-release.env"
    }

    if ($IsMacOS -or [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)) {
        return Join-Path $HOME "Library/Application Support/Exusiai/nexus-release.env"
    }

    $configHome = if ([string]::IsNullOrWhiteSpace($env:XDG_CONFIG_HOME)) {
        Join-Path $HOME ".config"
    }
    else {
        $env:XDG_CONFIG_HOME
    }
    return Join-Path $configHome "exusiai/nexus-release.env"
}

function Read-ExusiaiNexusConfig {
    param([string]$ConfigPath = (Get-ExusiaiNexusConfigPath))

    $config = @{}
    if (-not (Test-Path -LiteralPath $ConfigPath -PathType Leaf)) {
        return $config
    }

    foreach ($line in Get-Content -LiteralPath $ConfigPath -Encoding UTF8) {
        $trimmed = $line.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith("#")) {
            continue
        }

        $separator = $trimmed.IndexOf("=")
        if ($separator -lt 1) {
            continue
        }

        $key = $trimmed.Substring(0, $separator).Trim()
        $value = $trimmed.Substring($separator + 1).Trim()
        $config[$key] = $value
    }

    return $config
}

function Resolve-ExusiaiNexusValue {
    param(
        [string]$ExplicitValue,
        [Parameter(Mandatory = $true)][string]$EnvName,
        [Parameter(Mandatory = $true)][string]$ConfigKey,
        [hashtable]$Config = (Read-ExusiaiNexusConfig)
    )

    if (-not [string]::IsNullOrWhiteSpace($ExplicitValue)) {
        return $ExplicitValue
    }

    $envValue = [Environment]::GetEnvironmentVariable($EnvName)
    if (-not [string]::IsNullOrWhiteSpace($envValue)) {
        return $envValue
    }

    if ($Config.ContainsKey($ConfigKey) -and -not [string]::IsNullOrWhiteSpace([string]$Config[$ConfigKey])) {
        return [string]$Config[$ConfigKey]
    }

    return $null
}

function Write-ExusiaiNexusConfig {
    param(
        [Parameter(Mandatory = $true)][hashtable]$Values,
        [string]$ConfigPath = (Get-ExusiaiNexusConfigPath)
    )

    $directory = Split-Path -Parent $ConfigPath
    if (-not (Test-Path -LiteralPath $directory -PathType Container)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $lines = @(
        "# Exusiai Nexus release configuration",
        "# Keep this file private. Do not commit API keys.",
        "NEXUS_API_KEY=$($Values.NEXUS_API_KEY)",
        "NEXUS_UPDATE_GROUP_ID=$($Values.NEXUS_UPDATE_GROUP_ID)"
    )

    if ($Values.ContainsKey("NEXUS_PREVIOUS_VERSION_ID") -and -not [string]::IsNullOrWhiteSpace([string]$Values.NEXUS_PREVIOUS_VERSION_ID)) {
        $lines += "NEXUS_PREVIOUS_VERSION_ID=$($Values.NEXUS_PREVIOUS_VERSION_ID)"
    }

    Set-Content -LiteralPath $ConfigPath -Value $lines -Encoding UTF8

    if (-not ($IsWindows -or [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows))) {
        $chmod = Get-Command chmod -ErrorAction SilentlyContinue
        if ($chmod) {
            & chmod 600 $ConfigPath
        }
    }

    return $ConfigPath
}

param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Read-Json($Path) {
    Get-Content -Raw -Encoding UTF8 $Path | ConvertFrom-Json
}

function Get-JsonKeys($RelativePath) {
    $json = Read-Json (Join-Path $Root $RelativePath)
    @($json.PSObject.Properties.Name | Sort-Object)
}

function Assert-KeyParity($Name) {
    $zhs = Get-JsonKeys "myfirstmod\localization\zhs\$Name.json"
    $eng = Get-JsonKeys "myfirstmod\localization\eng\$Name.json"
    $missingInZhs = Compare-Object $zhs $eng | Where-Object SideIndicator -eq '=>'
    $missingInEng = Compare-Object $zhs $eng | Where-Object SideIndicator -eq '<='

    if ($missingInZhs -or $missingInEng) {
        Write-Error "Localization key mismatch in $Name. missingInZhs=$($missingInZhs.Count) missingInEng=$($missingInEng.Count)"
    }

    "localization parity ok: $Name ($($zhs.Count) keys)"
}

function Assert-RootLocalizationMirror($Name) {
    foreach ($locale in @('zhs', 'eng')) {
        $rootPath = "localization\$locale\$Name.json"
        $modPath = "myfirstmod\localization\$locale\$Name.json"
        $rootText = Get-Content -Raw -Encoding UTF8 (Join-Path $Root $rootPath)
        $modText = Get-Content -Raw -Encoding UTF8 (Join-Path $Root $modPath)

        if ($rootText -ne $modText) {
            Write-Error "Root localization mirror mismatch: $rootPath differs from $modPath"
        }
    }

    "root localization mirror ok: $Name"
}

function Assert-EnglishHasNoCjk {
    $issues = @()
    Get-ChildItem -Recurse -LiteralPath (Join-Path $Root 'myfirstmod\localization\eng') -Filter *.json | ForEach-Object {
        $text = Get-Content -Raw -Encoding UTF8 $_.FullName
        $text | ConvertFrom-Json | Out-Null
        if ($text -match '[\p{IsCJKUnifiedIdeographs}]') {
            $issues += $_.FullName
        }
    }

    if ($issues.Count -gt 0) {
        Write-Error "English localization contains CJK text: $($issues -join ', ')"
    }

    'english localization cjk scan ok'
}

function Assert-CardImages {
    $classes = @()
    Get-ChildItem -LiteralPath (Join-Path $Root 'Code\Cards') -Filter *.cs | ForEach-Object {
        $text = Get-Content -Raw -Encoding UTF8 $_.FullName
        [regex]::Matches($text, 'public\s+class\s+(\w+)\s*:\s*(?:RapidFireCardModel|MyFirstModCardModel)') |
            ForEach-Object { $classes += $_.Groups[1].Value }
    }
    $classes = $classes | Sort-Object -Unique

    $images = Get-ChildItem -LiteralPath (Join-Path $Root 'myfirstmod\images\cards') -File |
        Where-Object { $_.Extension -in '.jpg', '.png' } |
        ForEach-Object { $_.BaseName } |
        Sort-Object -Unique

    $missingImages = Compare-Object $classes $images | Where-Object SideIndicator -eq '<='
    $extraImages = Compare-Object $classes $images | Where-Object SideIndicator -eq '=>'

    if ($missingImages -or $extraImages) {
        Write-Error "Card image mismatch. missing=$($missingImages.InputObject -join ', ') extra=$($extraImages.InputObject -join ', ')"
    }

    "card image parity ok: classes=$($classes.Count) images=$($images.Count)"
}

function Assert-ConcreteResourcePaths {
    $paths = @()
    Get-ChildItem -Recurse -LiteralPath (Join-Path $Root 'Code'), (Join-Path $Root 'myfirstmod') -File |
        Where-Object { $_.Name -notlike '*.import' -and $_.Name -notlike '*.uid' } |
        ForEach-Object {
            $text = Get-Content -Raw -Encoding UTF8 $_.FullName
            [regex]::Matches($text, 'res://myfirstmod/[^\s"'')\]]+') |
                ForEach-Object { $paths += $_.Value }
        }

    $paths = $paths | Sort-Object -Unique
    $missing = @()
    foreach ($path in $paths) {
        if ($path.Contains('{') -or $path -match '%s') {
            continue
        }

        $localPath = Join-Path $Root ($path -replace '^res://', '')
        if (-not (Test-Path -LiteralPath $localPath)) {
            $missing += $path
        }
    }

    if ($missing.Count -gt 0) {
        Write-Error "Missing concrete resource paths: $($missing -join ', ')"
    }

    "resource path scan ok: paths=$($paths.Count)"
}

foreach ($file in @(
    'localization\zhs\cards.json',
    'localization\zhs\relics.json',
    'localization\zhs\characters.json',
    'localization\zhs\ancients.json',
    'localization\zhs\card_keywords.json',
    'localization\eng\cards.json',
    'localization\eng\relics.json',
    'localization\eng\characters.json',
    'localization\eng\ancients.json',
    'localization\eng\card_keywords.json',
    'myfirstmod\localization\zhs\cards.json',
    'myfirstmod\localization\zhs\relics.json',
    'myfirstmod\localization\zhs\characters.json',
    'myfirstmod\localization\zhs\ancients.json',
    'myfirstmod\localization\zhs\card_keywords.json',
    'myfirstmod\localization\eng\cards.json',
    'myfirstmod\localization\eng\relics.json',
    'myfirstmod\localization\eng\characters.json',
    'myfirstmod\localization\eng\ancients.json',
    'myfirstmod\localization\eng\card_keywords.json'
)) {
    Read-Json (Join-Path $Root $file) | Out-Null
    "json ok: $file"
}

foreach ($name in @('cards', 'relics', 'characters', 'ancients', 'card_keywords')) {
    Assert-KeyParity $name
    Assert-RootLocalizationMirror $name
}

Assert-EnglishHasNoCjk
Assert-CardImages
Assert-ConcreteResourcePaths

'validation complete'

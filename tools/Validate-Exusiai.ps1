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

function Assert-ModIdLocalizationMirror($Name) {
    foreach ($locale in @('zhs', 'eng')) {
        $rootPath = "exusiai\localization\$locale\$Name.json"
        $modPath = "myfirstmod\localization\$locale\$Name.json"
        $rootText = Get-Content -Raw -Encoding UTF8 (Join-Path $Root $rootPath)
        $modText = Get-Content -Raw -Encoding UTF8 (Join-Path $Root $modPath)

        if ($rootText -ne $modText) {
            Write-Error "mod-id localization mirror mismatch: $rootPath differs from $modPath"
        }
    }

    "mod-id localization mirror ok: $Name"
}

function Assert-RequiredKeywordLocalization {
    $sourceText = Get-ChildItem -Recurse -LiteralPath (Join-Path $Root 'Code') -Filter *.cs |
        ForEach-Object { Get-Content -Raw -Encoding UTF8 $_.FullName }

    $required = @('PERIOD')
    if ($sourceText -match 'MyKeywords\.RapidFire') {
        $required += 'MYFIRSTMOD-RAPID_FIRE.title'
        $required += 'MYFIRSTMOD-RAPID_FIRE.description'
    }
    if ($sourceText -match 'CardKeyword\.Exhaust') {
        $required += 'EXHAUST.title'
        $required += 'EXHAUST.description'
    }
    if ($sourceText -match 'CardKeyword\.Ethereal') {
        $required += 'ETHEREAL.title'
        $required += 'ETHEREAL.description'
    }

    $required = $required | Sort-Object -Unique
    foreach ($locale in @('zhs', 'eng')) {
        $keys = Get-JsonKeys "exusiai\localization\$locale\card_keywords.json"
        $missing = Compare-Object $keys $required | Where-Object SideIndicator -eq '=>'
        if ($missing) {
            Write-Error "Missing required card keyword localization in $locale`: $($missing.InputObject -join ', ')"
        }
    }

    "required keyword localization ok: $($required.Count) keys"
}

function Assert-CodeLocStringKeys {
    $issues = @()
    Get-ChildItem -Recurse -LiteralPath (Join-Path $Root 'Code') -Filter *.cs | ForEach-Object {
        $text = Get-Content -Raw -Encoding UTF8 $_.FullName
        [regex]::Matches($text, 'new\s+LocString\("([^"]+)",\s*"([^"]+)"\)') | ForEach-Object {
            $table = $_.Groups[1].Value
            $key = $_.Groups[2].Value
            foreach ($locale in @('zhs', 'eng')) {
                $relativePath = "exusiai\localization\$locale\$table.json"
                $path = Join-Path $Root $relativePath
                if (-not (Test-Path -LiteralPath $path)) {
                    $issues += "$($_.Path): missing table $relativePath"
                    continue
                }

                $keys = Get-JsonKeys $relativePath
                if ($keys -notcontains $key) {
                    $issues += "$($_.Path): missing $locale $table key $key"
                }
            }
        }
    }

    if ($issues.Count -gt 0) {
        Write-Error "Missing LocString localization keys: $($issues -join '; ')"
    }

    'code LocString key scan ok'
}

function ConvertTo-ModelId($Name) {
    [regex]::Replace($Name, '(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])', '_').ToUpperInvariant()
}

function Assert-ModelLocalizationKeys {
    $checks = @(
        @{
            Path = 'Code\Cards'
            Pattern = 'public\s+class\s+(\w+)\s*:\s*(?:RapidFireCardModel|MyFirstModCardModel)'
            Table = 'cards'
            Suffixes = @('title', 'description')
        },
        @{
            Path = 'Code\Powers'
            Pattern = 'public\s+class\s+(\w+)\s*:\s*CustomPowerModel'
            Table = 'cards'
            Suffixes = @('title', 'description')
        },
        @{
            Path = 'Code\Relics'
            Pattern = 'public\s+class\s+(\w+)\s*:\s*MyFirstModRelicModel'
            Table = 'relics'
            Suffixes = @('title', 'description', 'flavor')
        }
    )

    $issues = @()
    foreach ($check in $checks) {
        $classes = @()
        Get-ChildItem -LiteralPath (Join-Path $Root $check.Path) -Filter *.cs | ForEach-Object {
            $text = Get-Content -Raw -Encoding UTF8 $_.FullName
            [regex]::Matches($text, $check.Pattern) | ForEach-Object {
                $classes += $_.Groups[1].Value
            }
        }
        $classes = $classes | Sort-Object -Unique

        foreach ($locale in @('zhs', 'eng')) {
            $keys = Get-JsonKeys "exusiai\localization\$locale\$($check.Table).json"
            foreach ($class in $classes) {
                $id = "MYFIRSTMOD-$(ConvertTo-ModelId $class)"
                foreach ($suffix in $check.Suffixes) {
                    $key = "$id.$suffix"
                    if ($keys -notcontains $key) {
                        $issues += "$locale $($check.Table) missing $key"
                    }
                }
            }
        }
    }

    if ($issues.Count -gt 0) {
        Write-Error "Missing model localization keys: $($issues -join '; ')"
    }

    'model localization key scan ok'
}

function Assert-NoAutoKeywordTextInCardDescriptions {
    $allowed = @(
        'MYFIRSTMOD-DELIVERY_GUARANTEED.description'
    )

    $exhaustZhs = -join ([char]0x6D88, [char]0x8017)
    $etherealZhs = -join ([char]0x865A, [char]0x65E0)
    $patterns = @{
        zhs = @($exhaustZhs, $etherealZhs)
        eng = @('Exhaust', 'Ethereal')
    }

    $issues = @()
    foreach ($locale in @('zhs', 'eng')) {
        $json = Read-Json (Join-Path $Root "exusiai\localization\$locale\cards.json")
        foreach ($property in $json.PSObject.Properties) {
            if (-not $property.Name.EndsWith('.description')) {
                continue
            }
            if ($allowed -contains $property.Name) {
                continue
            }

            foreach ($pattern in $patterns[$locale]) {
                if ($property.Value -match [regex]::Escape($pattern)) {
                    $issues += "$locale $($property.Name) contains auto keyword text '$pattern'"
                }
            }
        }
    }

    if ($issues.Count -gt 0) {
        Write-Error "Card descriptions duplicate auto keyword text: $($issues -join '; ')"
    }

    'auto keyword duplication scan ok'
}

function Assert-NoRawLocalizationKeysInText {
    $issues = @()
    foreach ($locale in @('zhs', 'eng')) {
        Get-ChildItem -LiteralPath (Join-Path $Root "exusiai\localization\$locale") -Filter *.json | ForEach-Object {
            $json = Read-Json $_.FullName
            foreach ($property in $json.PSObject.Properties) {
                if ($property.Value -is [string] -and $property.Value -match '(?i)card[ _-]?keywords|\.title|\.description') {
                    $issues += "$($_.FullName): $($property.Name)"
                }
            }
        }
    }

    if ($issues.Count -gt 0) {
        Write-Error "Localization values appear to contain raw keys: $($issues -join '; ')"
    }

    'raw localization key text scan ok'
}

function Assert-ExportPresetIncludesModLocalization {
    $path = Join-Path $Root 'export_presets.cfg'
    $text = Get-Content -Raw -Encoding UTF8 $path
    if ($text -notmatch 'include_filter=.*exusiai/\*\*') {
        Write-Error 'export_presets.cfg must include exusiai/** so mod localization tables are packaged'
    }

    'export preset localization include ok'
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
    'exusiai\localization\zhs\cards.json',
    'exusiai\localization\zhs\relics.json',
    'exusiai\localization\zhs\characters.json',
    'exusiai\localization\zhs\ancients.json',
    'exusiai\localization\zhs\card_keywords.json',
    'exusiai\localization\eng\cards.json',
    'exusiai\localization\eng\relics.json',
    'exusiai\localization\eng\characters.json',
    'exusiai\localization\eng\ancients.json',
    'exusiai\localization\eng\card_keywords.json',
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
    Assert-ModIdLocalizationMirror $name
}

Assert-EnglishHasNoCjk
Assert-RequiredKeywordLocalization
Assert-CodeLocStringKeys
Assert-ModelLocalizationKeys
Assert-NoAutoKeywordTextInCardDescriptions
Assert-NoRawLocalizationKeysInText
Assert-ExportPresetIncludesModLocalization
Assert-CardImages
Assert-ConcreteResourcePaths

'validation complete'

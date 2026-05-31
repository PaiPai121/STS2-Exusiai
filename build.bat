@echo off
echo ========== Step 1: Build DLL ==========
dotnet build
if %errorlevel% neq 0 (
    echo BUILD FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 2: Export PCK ==========
"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64.exe" --headless --export-pack "BasicExport" "%~dp0Exusiai.pck"
if %errorlevel% neq 0 (
    echo PCK EXPORT FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 3: Copy to mods ==========
set "MODS_DIR=C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai"
copy /Y "%~dp0Exusiai.pck" "%MODS_DIR%\Exusiai.pck"
copy /Y "%~dp0.godot\mono\temp\bin\Debug\Exusiai.dll" "%MODS_DIR%\Exusiai.dll"
copy /Y "%~dp0Exusiai.json" "%MODS_DIR%\Exusiai.json"

echo.
echo ========== DONE! ==========
echo DLL + PCK deployed. Open game and test.

@echo off
echo ========== Step 1: Build DLL ==========
dotnet build
if %errorlevel% neq 0 (
    echo BUILD FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 2: Export PCK ==========
"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64.exe" --headless --export-pack "BasicExport" "%~dp0MyFirstMod.pck"
if %errorlevel% neq 0 (
    echo PCK EXPORT FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 3: Copy to mods ==========
set "MODS_DIR=C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod"
copy /Y "%~dp0MyFirstMod.pck" "%MODS_DIR%\MyFirstMod.pck"
copy /Y "%~dp0.godot\mono\temp\bin\Debug\MyFirstMod.dll" "%MODS_DIR%\MyFirstMod.dll"
copy /Y "%~dp0myfirstmod.json" "%MODS_DIR%\myfirstmod.json"

echo.
echo ========== DONE! ==========
echo DLL + PCK deployed. Open game and test.

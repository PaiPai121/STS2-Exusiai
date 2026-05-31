@echo off
setlocal
set "PROJECT_DIR=%~dp0"
set "PROJECT_DIR=%PROJECT_DIR:~0,-1%"
set "EXPORTER=D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe"
set "MODS_DIR=C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai"

echo ========== Step 1: Build DLL ==========
dotnet build "%PROJECT_DIR%\MyFirstMod.csproj"
if %errorlevel% neq 0 (
    echo BUILD FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 2: Export PCK ==========
"%EXPORTER%" --headless --path "%PROJECT_DIR%" --export-pack BasicExport "%PROJECT_DIR%\Exusiai.pck"
if %errorlevel% neq 0 (
    echo PCK EXPORT FAILED
    exit /b %errorlevel%
)

echo.
echo ========== Step 3: Copy to mods ==========
copy /Y "%PROJECT_DIR%\Exusiai.pck" "%MODS_DIR%\Exusiai.pck"
copy /Y "%PROJECT_DIR%\.godot\mono\temp\bin\Debug\Exusiai.dll" "%MODS_DIR%\Exusiai.dll"
copy /Y "%PROJECT_DIR%\Exusiai.json" "%MODS_DIR%\Exusiai.json"

echo.
echo ========== DONE! ==========
echo DLL + PCK deployed. Open game and test.

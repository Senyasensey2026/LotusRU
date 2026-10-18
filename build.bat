@echo off
cd /d "%~dp0"
set DOTNET=dotnet
if exist "%USERPROFILE%\.dotnet\dotnet.exe" set DOTNET="%USERPROFILE%\.dotnet\dotnet.exe"
%DOTNET% --list-sdks | findstr /b "10." >nul || (
  echo .NET 10 SDK not found. Install it: https://dotnet.microsoft.com/download/dotnet/10.0
  pause & exit /b 1
)
for /f "delims=" %%v in ('%DOTNET% msbuild LotusRU.csproj -nologo -getProperty:Version') do set VER=%%v
%DOTNET% build -c Release || (pause & exit /b 1)
set OUT=release\SPT\user\mods\LotusRU
if exist release rmdir /s /q release
mkdir "%OUT%"
xcopy /e /y /q "bin\Release\LotusRU\*" "%OUT%\" >nul
del /q "%OUT%\*.staticwebassets.*" 2>nul
copy /y LICENSE "%OUT%\" >nul
copy /y README.md "%OUT%\" >nul
if exist LotusRU-%VER%.zip del /q LotusRU-%VER%.zip
powershell -NoProfile -Command "Compress-Archive -Path 'release\SPT' -DestinationPath 'LotusRU-%VER%.zip'" || (pause & exit /b 1)
echo.
echo Done: %CD%\LotusRU-%VER%.zip
pause

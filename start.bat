@echo off
rem Double-click to build and run AppHub against SQL Server LocalDB (database AppHubDB)
setlocal
cd /d "%~dp0"
title AppHub

set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
    echo [!] Visual Studio 2022 or Build Tools for Visual Studio is not installed.
    echo     Install it with the ".NET desktop development" workload and run this file again.
    pause
    exit /b 1
)
for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"
if not defined MSBUILD (
    echo [!] MSBuild was not found. Add the ".NET desktop development" workload in the Visual Studio Installer.
    pause
    exit /b 1
)

where sqllocaldb >nul 2>nul
if errorlevel 1 (
    echo [!] SQL Server LocalDB is not installed. Install it with Visual Studio or SQL Server Express.
    pause
    exit /b 1
)
sqllocaldb start MSSQLLocalDB >nul

rem setup.sql only creates what is missing, so it is safe to run on every start.
where sqlcmd >nul 2>nul
if errorlevel 1 (
    echo [i] sqlcmd was not found, so the database step is skipped.
    echo     If AppHubDB does not exist yet, run setup.sql once in SQL Server Management Studio.
) else (
    echo Preparing database AppHubDB...
    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -f 65001 -i setup.sql >nul
    if errorlevel 1 goto :fail
)

echo Restoring packages...
"%MSBUILD%" AppHub.sln -t:restore -p:RestorePackagesConfig=true -v:minimal -nologo
if errorlevel 1 goto :fail

echo Building...
"%MSBUILD%" AppHub.sln -p:Configuration=Debug -v:minimal -nologo
if errorlevel 1 goto :fail

echo.
echo Starting AppHub. Demo accounts: admin / admin1234, user1 / user1234
start "" "AppHub.Launcher\bin\Debug\AppHub.Launcher.exe"
goto :eof

:fail
echo.
echo [!] Something went wrong. Read the messages above.
pause
exit /b 1

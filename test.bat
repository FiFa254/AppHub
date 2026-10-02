@echo off
rem Double-click to build AppHub and run the MSTest suite (uses database AppHubDB_Test on LocalDB)
setlocal
cd /d "%~dp0"
title AppHub - tests

set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
    echo [!] Visual Studio 2022 or Build Tools for Visual Studio is not installed.
    echo     Install it with the ".NET desktop development" workload and run this file again.
    pause
    exit /b 1
)
for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"
for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -find Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe`) do set "VSTEST=%%i"
if not defined MSBUILD (
    echo [!] MSBuild was not found. Add the ".NET desktop development" workload in the Visual Studio Installer.
    pause
    exit /b 1
)
if not defined VSTEST (
    echo [!] vstest.console.exe was not found. Add the "Testing tools core features" component in the Visual Studio Installer.
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

echo Restoring packages...
"%MSBUILD%" AppHub.sln -t:restore -p:RestorePackagesConfig=true -v:minimal -nologo
if errorlevel 1 goto :fail

echo Building...
"%MSBUILD%" AppHub.sln -p:Configuration=Debug -v:minimal -nologo
if errorlevel 1 goto :fail

echo Running tests...
"%VSTEST%" AppHub.Tests\bin\Debug\AppHub.Tests.dll /Settings:AppHub.Tests\AppHub.runsettings /TestAdapterPath:packages\MSTest.TestAdapter.2.2.10\build\_common
if errorlevel 1 goto :fail

echo.
echo All tests passed.
pause
goto :eof

:fail
echo.
echo [!] Some steps failed. Read the messages above.
pause
exit /b 1

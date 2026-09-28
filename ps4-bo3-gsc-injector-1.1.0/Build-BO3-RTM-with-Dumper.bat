@echo off
setlocal EnableExtensions
title BO3 RTM + Memory Dumper - Build Fix

echo ============================================================
echo  BO3 RTM + Memory Dumper - NuGet Restore and Release Build
echo ============================================================
echo.

rem This BAT should be in BO3-RTM-with-Memory-Dumper-SOURCE.
rem Build the solution path directly; avoid the broken recursive FOR path.
set "ROOT=%~dp0"
set "SLN=%ROOT%ps4-bo3-gsc-injector-1.1.0\PS4 BO3 GSC.sln"

rem If the BAT is instead placed inside the project folder, use that solution.
if not exist "%SLN%" set "SLN=%ROOT%PS4 BO3 GSC.sln"

if not exist "%SLN%" (
    echo ERROR: Solution file not found at either expected location:
    echo "%ROOT%ps4-bo3-gsc-injector-1.1.0\PS4 BO3 GSC.sln"
    echo "%ROOT%PS4 BO3 GSC.sln"
    echo.
    echo Put this BAT in the SOURCE folder or directly in the project folder.
    pause
    exit /b 1
)

for %%D in ("%SLN%") do set "SLNDIR=%%~dpD"
set "PROJ=%SLNDIR%PS4 BO3 GSC"
set "PKGCFG=%PROJ%\packages.config"
set "PKGDIR=%SLNDIR%packages"

echo Solution:
echo "%SLN%"
echo.

if not exist "%PKGCFG%" (
    echo ERROR: packages.config not found:
    echo "%PKGCFG%"
    pause
    exit /b 1
)

rem Locate NuGet, downloading the official executable if needed.
set "NUGET="
if exist "%ROOT%nuget.exe" set "NUGET=%ROOT%nuget.exe"
if not defined NUGET (
    for /f "delims=" %%N in ('where nuget.exe 2^>nul') do if not defined NUGET set "NUGET=%%N"
)
if not defined NUGET (
    echo Downloading NuGet...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile '%ROOT%nuget.exe' -UseBasicParsing } catch { Write-Error $_; exit 1 }"
    if errorlevel 1 (
        echo ERROR: Could not download NuGet.
        pause
        exit /b 1
    )
    set "NUGET=%ROOT%nuget.exe"
)

echo Restoring NuGet packages...
"%NUGET%" restore "%PKGCFG%" -PackagesDirectory "%PKGDIR%" -NonInteractive
if errorlevel 1 (
    echo ERROR: NuGet restore failed.
    pause
    exit /b 1
)

rem Find MSBuild using Visual Studio's vswhere first.
set "MSBUILD="
set "VSWHERE="
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not defined VSWHERE if exist "%ProgramFiles%\Microsoft Visual Studio\Installer\vswhere.exe" set "VSWHERE=%ProgramFiles%\Microsoft Visual Studio\Installer\vswhere.exe"

if defined VSWHERE (
    for /f "usebackq delims=" %%I in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do if not defined MSBUILD set "MSBUILD=%%I"
)
if not defined MSBUILD (
    for %%M in (
        "%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
        "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
        "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
        "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    ) do if not defined MSBUILD if exist "%%~M" set "MSBUILD=%%~M"
)
if not defined MSBUILD (
    for /f "delims=" %%M in ('where msbuild.exe 2^>nul') do if not defined MSBUILD set "MSBUILD=%%M"
)
if not defined MSBUILD (
    echo ERROR: MSBuild not found. Install Visual Studio Build Tools with .NET desktop build tools.
    pause
    exit /b 1
)

echo.
echo MSBuild:
echo "%MSBUILD%"
echo.
echo Building Release...
"%MSBUILD%" "%SLN%" /t:Rebuild /p:Configuration=Release "/p:Platform=Any CPU" /m
if errorlevel 1 (
    echo.
    echo BUILD FAILED. Send the complete output above.
    pause
    exit /b 1
)

echo.
echo BUILD SUCCEEDED.
echo Check the project's bin\Release folder:
echo "%PROJ%\bin\Release"
pause

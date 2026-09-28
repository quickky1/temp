@echo off
title Upload to GitHub - quickky1/temp
cd /d "%~dp0"

where git >nul 2>&1
if errorlevel 1 (
    echo [ERROR] git is not installed or not on PATH.
    echo Install it from https://git-scm.com/download/win and run this again.
    pause
    exit /b 1
)

if not exist ".git" (
    echo Initializing new git repo...
    git init
)

git remote remove origin 2>nul
git remote add origin https://github.com/quickky1/temp.git
git branch -M main

git add -A
set /p msg="Commit message (press Enter for default 'upload'): "
if "%msg%"=="" set msg=upload
git commit -m "%msg%"

echo.
echo Pushing to https://github.com/quickky1/temp ...
git push -u origin main

if errorlevel 1 (
    echo.
    echo [ERROR] Push failed. If it asked for a login, sign in with your
    echo GitHub account (a browser window may open), then run this again.
) else (
    echo.
    echo Done! Check https://github.com/quickky1/temp
)
pause

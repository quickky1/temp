@echo off
setlocal
rem Locks the Inject GSC button size in CODE (runs after the designer),
rem so no designer weirdness can override it.
set "CS=C:\Users\death\Downloads\BO3-RTM-with-Memory-Dumper-SOURCE\ps4-bo3-gsc-injector-1.1.0\PS4 BO3 GSC\MainWindow.cs"

if not exist "%CS%" (
    echo [ERROR] MainWindow.cs not found:
    echo %CS%
    echo Right-click this bat, choose Edit, and fix the path on the "set CS=" line.
    pause
    exit /b 1
)

powershell -NoProfile -ExecutionPolicy Bypass -Command "$t = Get-Content '%CS%' -Raw; if ($t -match 'injectGscButton\.Size = new System\.Drawing\.Size\(175, 38\)') { Write-Output 'ALREADY PATCHED - nothing changed' } else { $a = 'InitializeComponent();'; $i = $t.IndexOf($a) + $a.Length; $n = [Environment]::NewLine + '            injectGscButton.Size = new System.Drawing.Size(175, 38);' + [Environment]::NewLine + '            injectGscButton.Location = new System.Drawing.Point(215, 48);'; $t = $t.Insert($i, $n); Set-Content '%CS%' -Value $t -NoNewline; Write-Output 'PATCHED - now Rebuild the solution in Visual Studio' }"

echo.
echo IMPORTANT: after rebuilding, run the FRESH exe from:
echo   ...\PS4 BO3 GSC\bin\Debug\PS4 BO3 GSC.exe
echo Check its "Date modified" is just now - do not launch an old copy
echo sitting in Downloads or on your Desktop.
pause

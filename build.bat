@echo off
setlocal enabledelayedexpansion
title Compilador de MantenimientoPC

echo ========================================================
echo   COMPILANDO SISTEMA DE MANTENIMIENTO DE PC (.EXE)
echo ========================================================
echo.

set CSC_64=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set CSC_32=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
set COMPILER=

if exist "%CSC_64%" (
    set COMPILER=%CSC_64%
) else if exist "%CSC_32%" (
    set COMPILER=%CSC_32%
)

if "%COMPILER%"=="" (
    echo [!] ERROR: No se encontro el compilador de C# [csc.exe] de .NET Framework v4.0.
    echo Asegurese de tener instalado .NET Framework 4.0 o superior.
    if "%1"=="/nopause" goto end
    pause
    exit /b 1
)

echo [+] Compilador encontrado: !COMPILER!
echo [+] Compilando Program.cs con manifiesto de elevacion (UAC)...
echo.

"!COMPILER!" /win32manifest:app.manifest /out:MantenimientoPC.exe Program.cs

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo   [OK] COMPILACION EXITOSA: MantenimientoPC.exe creado.
    echo ========================================================
) else (
    echo.
    echo ========================================================
    echo   [!] ERROR: Fallo la compilacion del codigo fuente.
    echo ========================================================
)
echo.

:end
if "%1"=="/nopause" exit /b %ERRORLEVEL%
pause

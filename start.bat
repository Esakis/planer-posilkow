@echo off
setlocal enabledelayedexpansion

REM ============================================================
REM  TaniTydzien - lokalny launcher (Windows)
REM  1) zabija procesy na portach API/frontendu
REM  2) instaluje zaleznosci jesli brakuja
REM  3) uruchamia API (.NET) i frontend (Angular)
REM ============================================================

set "ROOT=%~dp0"
set "API_DIR=%ROOT%api"
set "WEB_DIR=%ROOT%web"

set "API_PORT=5080"
set "WEB_PORT=4200"

echo(
echo === TaniTydzien: start ===
echo(

REM ---------- 1. Zwolnienie portow ----------
call :kill_port %API_PORT%
call :kill_port %WEB_PORT%

REM ---------- 2. Instalacja zaleznosci (w razie potrzeby) ----------
echo(
echo [restore] dotnet restore (API)...
pushd "%API_DIR%"
dotnet restore
popd

if not exist "%WEB_DIR%\node_modules" (
    echo(
    echo [install] npm install ^(web^) - brak node_modules...
    pushd "%WEB_DIR%"
    call npm install
    popd
) else (
    echo [install] web: node_modules obecne - pomijam npm install
)

REM ---------- 3. Uruchomienie aplikacji ----------
echo(
echo [run] API   -^> http://localhost:%API_PORT%
start "TaniTydzien API" cmd /k "cd /d "%API_DIR%" && dotnet run --urls http://localhost:%API_PORT%"

echo [run] WEB   -^> http://localhost:%WEB_PORT%
start "TaniTydzien WEB" cmd /k "cd /d "%WEB_DIR%" && npm start"

echo(
echo === Uruchomiono. Zamknij okna API/WEB aby zatrzymac. ===
echo(
exit /b 0

REM ============================================================
REM  Funkcja: zabij proces trzymajacy dany port
REM ============================================================
:kill_port
set "PORT=%~1"
set "FOUND="
for /f "tokens=5" %%P in ('netstat -ano ^| findstr /r /c:":%PORT% .*LISTENING"') do (
    if not "%%P"=="0" (
        set "FOUND=1"
        echo [port %PORT%] zabijam PID %%P
        taskkill /F /PID %%P >nul 2>&1
    )
)
if not defined FOUND echo [port %PORT%] wolny
exit /b 0

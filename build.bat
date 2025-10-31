@echo off
setlocal
set "buildTool=src\BuildTool\bin\Debug\BuildTool.exe"

:: Check if the task is provided
if "%~1"=="" (
    set "task=test"
    PowerShell -NoProfile -Command "& {Write-Host 'Defaulting task to test...' -ForegroundColor Yellow}"
) else (
    :: If provided, use the argument as is
    set "task=%~1"
)

:: Check if the version is provided
if "%~2"=="" (
    for /f "usebackq" %%i in (`powershell -NoProfile -Command "Get-Date -Format 'yyyy.MM.dd.HHmm'"`) do set "version=%%i"
    PowerShell -NoProfile -Command "& {Write-Host 'Defaulting Version to date...' -ForegroundColor Yellow}"
) else (
    set "version=%~2"
)



:: Run the BuildTool passing incoming parameters
PowerShell -NoProfile -Command "& {Write-Host 'Calling BuildTool...' -ForegroundColor Green}"
"%buildTool%" version="%version%" task="%task%"

endlocal
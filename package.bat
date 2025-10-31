@echo off
set ORIGINAL_DIR=%CD%
cd src\Faktory
nuget.exe pack
cd %ORIGINAL_DIR%
powershell -Command "Set-Location -LiteralPath '%CD%'; Move-Item -Path .\src\Faktory\*.nupkg -Destination .\ -Force"
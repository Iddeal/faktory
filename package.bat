@echo off
MSBuild.exe src\Faktory\Faktory.Core.csproj /p:Configuration="Release"
set ORIGINAL_DIR=%CD%
cd src\Faktory
nuget.exe pack
cd %ORIGINAL_DIR%
powershell -Command "Set-Location -LiteralPath '%CD%'; Move-Item -Path .\src\Faktory\*.nupkg -Destination .\ -Force"
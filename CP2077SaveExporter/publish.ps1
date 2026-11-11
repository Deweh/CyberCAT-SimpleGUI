#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

dotnet publish CP2077SaveExporter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/win-x64
dotnet publish CP2077SaveExporter.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux-x64

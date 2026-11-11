#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet publish CP2077SaveExporter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/win-x64
dotnet publish CP2077SaveExporter.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux-x64

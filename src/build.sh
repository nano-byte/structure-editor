#!/usr/bin/env bash
set -e
cd `dirname $0`

echo "WARNING: You need Visual Studio 2026 v18.0 or newer to perform a full build of this project" >&2

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml"
fi

echo "Build binaries"
$dotnet msbuild -v:Quiet -restore -t:Build -p:Configuration=Release -p:Version=${1:-1.0.0-pre}

# Unit tests are not run here because they depend on WinForms, which is only available on Windows

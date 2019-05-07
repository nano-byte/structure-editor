﻿$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

# Ensure 0install is in the PATH
if (!(Get-Command 0install -ErrorAction SilentlyContinue)) {
    mkdir -Force "$env:TEMP\zero-install" | Out-Null
    Invoke-WebRequest "https://0install.de/files/0install.exe" -OutFile "$env:TEMP\zero-install\0install.exe"
    $env:PATH = "$env:TEMP\zero-install;$env:PATH"
}

if (Test-Path ..\artifacts\Documentation) {rm -Recurse -Force ..\artifacts\Documentation}
mkdir ..\artifacts\Documentation | Out-Null

# Download tag files for external references
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12'
Invoke-WebRequest https://common.nano-byte.net/nanobyte-common.tag -OutFile nanobyte-common.tag

0install run --batch http://repo.roscidus.com/devel/doxygen

cp CNAME ..\artifacts\Documentation\

popd

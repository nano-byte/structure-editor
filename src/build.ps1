Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

if (Test-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe") {
  $vsDir = . "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath -format value
  $msBuild = if (Test-Path "$vsDir\MSBuild\Current") {"$vsDir\MSBuild\Current\Bin\amd64\MSBuild.exe"} else {"$vsDir\MSBuild\15.0\Bin\amd64\MSBuild.exe"}
} else {
  Write-Host -ForegroundColor yellow "WARNING: You need Visual Studio to perform a full Release build"
  $msBuild = "dotnet msbuild"
}

. $msBuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=$Version
if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}

popd

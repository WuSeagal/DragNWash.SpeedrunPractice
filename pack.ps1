# Build the plugin and produce two drop-in zips under dist/:
#   DragNWash.SpeedrunPractice-vX.Y.Z-full.zip         BepInEx 5 + plugin (for players without BepInEx)
#   DragNWash.SpeedrunPractice-vX.Y.Z-plugin-only.zip  plugin only (for players who already have BepInEx)
# Usage: .\pack.ps1 [-GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"] [-BepInExVersion 5.4.23.5]
param(
    [string]$GameDir = "",
    [string]$BepInExVersion = "5.4.23.5"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$version = ([xml](Get-Content "$root\SpeedrunPractice.csproj")).Project.PropertyGroup.Version | Select-Object -First 1
$pluginDir = "BepInEx\plugins\DragNWash.SpeedrunPractice"

# ---- build ----------------------------------------------------------------
$buildArgs = @("build", "$root\SpeedrunPractice.csproj", "-c", "Release")
if ($GameDir -ne "") { $buildArgs += "-p:GameDir=$GameDir" }
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

# ---- fetch BepInEx (cached in vendor/, gitignored) -------------------------
$vendor = "$root\vendor"
$bepZip = "$vendor\BepInEx_win_x64_$BepInExVersion.zip"
if (-not (Test-Path $bepZip)) {
    New-Item -ItemType Directory -Force $vendor | Out-Null
    $url = "https://github.com/BepInEx/BepInEx/releases/download/v$BepInExVersion/BepInEx_win_x64_$BepInExVersion.zip"
    Write-Host "Downloading $url"
    Invoke-WebRequest -Uri $url -OutFile $bepZip
}
$bepLicense = "$vendor\BepInEx-LICENSE.txt"
if (-not (Test-Path $bepLicense)) {
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/BepInEx/BepInEx/master/LICENSE" -OutFile $bepLicense
}

# ---- stage ----------------------------------------------------------------
$dist = "$root\dist"
New-Item -ItemType Directory -Force $dist | Out-Null

function New-Stage([string]$name) {
    $path = "$dist\stage-$name"
    if (Test-Path $path) { Remove-Item -Recurse -Force $path }
    New-Item -ItemType Directory -Force $path | Out-Null
    return $path
}

function Add-Plugin([string]$stage) {
    $target = "$stage\$pluginDir"
    New-Item -ItemType Directory -Force $target | Out-Null
    Copy-Item "$root\bin\Release\netstandard2.1\DragNWash.SpeedrunPractice.dll" $target
    Copy-Item "$root\README.md" $target
    Copy-Item "$root\README.zh-TW.md" $target
    Copy-Item "$root\LICENSE" $target
}

function New-Zip([string]$stage, [string]$zipName) {
    $zip = "$dist\$zipName"
    if (Test-Path $zip) { Remove-Item -Force $zip }
    Compress-Archive -Path "$stage\*" -DestinationPath $zip
    Remove-Item -Recurse -Force $stage
    Write-Host "Packed: $zip"
}

# plugin-only
$stage = New-Stage "plugin"
Add-Plugin $stage
New-Zip $stage "DragNWash.SpeedrunPractice-v$version-plugin-only.zip"

# full: BepInEx + plugin. BepInEx is LGPL-2.1; its LICENSE ships inside the zip.
$stage = New-Stage "full"
Expand-Archive -Path $bepZip -DestinationPath $stage
Copy-Item $bepLicense "$stage\BepInEx\LICENSE.BepInEx.txt"
Add-Plugin $stage
New-Zip $stage "DragNWash.SpeedrunPractice-v$version-full.zip"

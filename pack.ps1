# Build the plugin and produce a drop-in zip under dist/.
# Usage: .\pack.ps1 [-GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"]
param([string]$GameDir = "")

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$version = ([xml](Get-Content "$root\SpeedrunPractice.csproj")).Project.PropertyGroup.Version | Select-Object -First 1

$buildArgs = @("build", "$root\SpeedrunPractice.csproj", "-c", "Release")
if ($GameDir -ne "") { $buildArgs += "-p:GameDir=$GameDir" }
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

$stage = "$root\dist\stage\BepInEx\plugins\DragNWash.SpeedrunPractice"
if (Test-Path "$root\dist\stage") { Remove-Item -Recurse -Force "$root\dist\stage" }
New-Item -ItemType Directory -Force $stage | Out-Null
Copy-Item "$root\bin\Release\netstandard2.1\DragNWash.SpeedrunPractice.dll" $stage
Copy-Item "$root\README.md" $stage
Copy-Item "$root\README.zh-TW.md" $stage
Copy-Item "$root\LICENSE" $stage

$zip = "$root\dist\DragNWash.SpeedrunPractice-v$version.zip"
if (Test-Path $zip) { Remove-Item -Force $zip }
Compress-Archive -Path "$root\dist\stage\BepInEx" -DestinationPath $zip
Remove-Item -Recurse -Force "$root\dist\stage"
Write-Host "Packed: $zip"

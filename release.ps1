$csprojCmlLib = Join-Path $PSScriptRoot "src\CmlLib.Core.csproj"
$csprojCmlLibCoreSample = Join-Path $PSScriptRoot "examples\console\CmlLibCoreSample.csproj"
$csprojCmlLibWinFormSample = Join-Path $PSScriptRoot "examples\winform\CmlLibWinFormSample.csproj"

if (-not (Test-Path $csprojCmlLib)) { Write-Host "Cannot find CmlLib.Core.csproj file"; exit }
if (-not (Test-Path $csprojCmlLibCoreSample)) { Write-Host "Cannot find CmlLibCoreSample.csproj file"; exit }
if (-not (Test-Path $csprojCmlLibWinFormSample)) { Write-Host "Cannot find CmlLibWinFormSample.csproj file"; exit }

$outDir = Join-Path $PSScriptRoot "release"
$publishDir = Join-Path $outDir "CmlLib.Core"
$sampleCoreLauncherDir = Join-Path $outDir "SampleCoreLauncher"
$sampleWinFormLauncherDir = Join-Path $outDir "SampleWinFormLauncher"

if (Test-Path $outDir) {
    Remove-Item -Recurse -Force $outDir -ErrorAction Stop
}

Write-Host "Packing nupkg file..."
dotnet pack $csprojCmlLib -o $outDir -c Release
if ($LASTEXITCODE -ne 0) { exit }

Write-Host "Publishing CmlLibCoreSample project..."
dotnet publish $csprojCmlLibCoreSample -o $sampleCoreLauncherDir -c Release --no-self-contained
if ($LASTEXITCODE -ne 0) { exit }
Compress-Archive -Path $sampleCoreLauncherDir -DestinationPath "${sampleCoreLauncherDir}.zip"
Remove-Item -Recurse -Force $sampleCoreLauncherDir

Write-Host "Publishing CmlLibWinFormSample project..."
dotnet publish $csprojCmlLibWinFormSample -o $sampleWinFormLauncherDir -c Release --no-self-contained
if ($LASTEXITCODE -ne 0) { exit }
Compress-Archive -Path $sampleWinFormLauncherDir -DestinationPath "${sampleWinFormLauncherDir}.zip"
Remove-Item -Recurse -Force $sampleWinFormLauncherDir

Write-Host "Done"
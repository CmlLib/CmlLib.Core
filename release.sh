#!/bin/sh
export CMLLIB_RELEASE_VERSION_NAME

if [ "$1" != "" ]; then
    versionName=$1
elif [ "$CMLLIB_RELEASE_VERSION_NAME" != "" ]; then 
    versionName=$CMLLIB_RELEASE_VERSION_NAME;
else
    echo "Error: Specify version name"
    exit
fi

baseDir=$(dirname "$0")

csprojCmlLib="${baseDir}/src/CmlLib.csproj"
csprojCmlLibCoreSample="${baseDir}/examples/console/CmlLibCoreSample.csproj"
csprojCmlLibWinFormSample="${baseDir}/examples/winform/CmlLibWinFormSample.csproj"

[ ! -f $csprojCmlLib ] && { echo "Cannot find CmlLib.csproj file"; exit; }
[ ! -f $csprojCmlLibCoreSample ] && { echo "Cannot find CmlLibCoreSample.csproj file"; exit; }
[ ! -f $csprojCmlLibWinFormSample ] && { echo "Cannot find CmlLibWinFormSample.csproj file"; exit; }

outDir="${baseDir}/release"
publishDir="${outDir}/CmlLib.Core.${versionName}"
sampleCoreLauncherDir="${outDir}/SampleCoreLauncher"
sampleWinFormLauncherDir="${outDir}/SampleWinFormLauncher"

if [ -d $outDir ]; then
    rm -r $outDir || { echo "Failed to remove output directory: ${outDir}"; exit; }
fi

echo
echo "Packing nupkg file..."
dotnet pack "$csprojCmlLib" -o "$outDir" -c Release || exit;

echo
echo "Publishing CmlLibCoreSample project..."
dotnet publish "$csprojCmlLibCoreSample" -o "$sampleCoreLauncherDir" -c Release --no-self-contained || exit;
tar -zcvf "${sampleCoreLauncherDir}.tar.gz" "$sampleCoreLauncherDir"
rm -rf "${sampleCoreLauncherDir}"

echo
echo "Publishing CmlLibWinFormSample project..."
dotnet publish "$csprojCmlLibWinFormSample" -o "$sampleWinFormLauncherDir" -c Release --no-self-contained || exit;
tar -zcvf "${sampleWinFormSample}.tar.gz" "$sampleWinFormLauncherDir"
rm -rf "${sampleWinFormLauncherDir}"

echo
echo "Done"
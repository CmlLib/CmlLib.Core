if ($args.Length -gt 0)
{
    $ver=$args[0]
}
else
{
    $ver = Read-Host 'version'
}

Remove-Item -Path .\release -Confirm -Recurse

mkdir .\release\CmlLib.Core.$ver
mkdir .\release\SampleCoreLauncher$ver
mkdir .\release\SampleWinformLauncher$ver

Copy-Item .\CmlLib\bin\Release\* -Destination .\release\CmlLib.Core.$ver -Recurse
Get-ChildItem .\release\CmlLib.Core.$ver -Recurse -File | Where {($_.Extension -ne ".dll")} | Remove-Item
Copy-Item -Path .\CmlLibCoreSample\bin\Release\netcoreapp3.1\* -Destination .\release\SampleCoreLauncher$ver
Copy-Item .\CmlLibWinFormSample\bin\Release\* -Destination .\release\SampleWinformLauncher$ver -Include *.exe,*.dll,*.pdb,*.config

Compress-Archive -Path .\release\CmlLib.Core.$ver -DestinationPath .\release\CmlLib.Core.$ver.zip
Compress-Archive -Path .\release\SampleCoreLauncher$ver -DestinationPath .\release\SampleCoreLauncher$ver.zip
Compress-Archive -Path .\release\SampleWinformLauncher$ver -DestinationPath .\release\SampleWinformLauncher$ver.zip

Remove-Item -Path .\release\CmlLib.Core.$ver -Confirm -Recurse
Remove-Item -Path .\release\SampleCoreLauncher$ver -Confirm -Recurse
Remove-Item -Path .\release\SampleWinformLauncher$ver -Confirm -Recurse
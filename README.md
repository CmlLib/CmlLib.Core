# CmlLib.Core

## Minecraft Launcher Library

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/icon.png' width=128>

[![Nuget Badge](https://img.shields.io/nuget/v/CmlLib.Core)](https://www.nuget.org/packages/CmlLib.Core)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3f55a130ec3f4bccb55e7def97cfa2ce)](https://www.codacy.com/gh/CmlLib/CmlLib.Core/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=CmlLib/CmlLib.Core\&utm_campaign=Badge_Grade)

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord\&logo=discord\&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)

CmlLib.Core is a Minecraft launcher library for .NET\
support all vanilla and mod versions (including Forge, Fabric, etc...)

[한국어 문서](https://alphabs.gitbook.io/cmllib/v/v4-kr)

## Features

* Authenticate with Microsoft Xbox account
* Get vanilla versions and installed versions
* Install vanilla versions
* Launch any vanilla version (tested up to 1.21)
* Launch Forge, Optifine, FabricMC, LiteLoader or any other custom version
* Install Java runtime
* Install LiteLoader, FabricMC
* Launch with options (direct server connecting, screen resolution, JVM arguments)
* Cross-platform (Windows, Linux, macOS)

[Go to the wiki for all features](https://alphabs.gitbook.io/cmllib)

## Install

Install the [CmlLib.Core Nuget package](https://www.nuget.org/packages/CmlLib.Core)

## QuickStart

### Get All Versions

```csharp
using CmlLib.Core;

var launcher = new MinecraftLauncher();
var versions = await launcher.GetAllVersionsAsync();
foreach (var version in versions)
{
    Console.WriteLine($"{version.Type} {version.Name}");
}
```

### Launch the Game

```csharp
using CmlLib.Core;
using CmlLib.Core.ProcessBuilder;

var launcher = new MinecraftLauncher();
var process = await launcher.InstallAndBuildProcessAsync("1.21", new MLaunchOption());
process.Start();
```

### Launch the Game with Options

```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

var path = new MinecraftPath("./my_game_dir");
var launcher = new MinecraftLauncher(path);
launcher.FileProgressChanged += (sender, args) =>
{
    Console.WriteLine($"Name: {args.Name}");
    Console.WriteLine($"Type: {args.EventType}");
    Console.WriteLine($"Total: {args.TotalTasks}");
    Console.WriteLine($"Progressed: {args.ProgressedTasks}");
};
launcher.ByteProgressChanged += (sender, args) =>
{
    Console.WriteLine($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
};

await launcher.InstallAsync("1.20.4");
var process = await launcher.BuildProcessAsync("1.20.4", new MLaunchOption
{
    Session = MSession.CreateOfflineSession("Gamer123"),
    MaximumRamMb = 4096
});
process.Start();
```

## Documentation

**[Official documentation](https://alphabs.gitbook.io/cmllib)**

**[한국어 문서](https://alphabs.gitbook.io/cmllib/v/v4-kr)**

## Example

[Sample Launcher](https://github.com/CmlLib/CmlLib-Minecraft-Launcher)

## Contributors

<a href="https://github.com/cmllib/cmllib.core/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=cmllib/cmllib.core" />
</a>

Made with [contrib.rocks](https://contrib.rocks).
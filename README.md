# CmlLib.Core

## Minecraft Launcher Library

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/icon.png' width=128>

[![Nuget Badge](https://img.shields.io/nuget/v/CmlLib.Core)](https://www.nuget.org/packages/CmlLib.Core)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3f55a130ec3f4bccb55e7def97cfa2ce)](https://www.codacy.com/gh/CmlLib/CmlLib.Core/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=CmlLib/CmlLib.Core\&utm_campaign=Badge_Grade)

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord\&logo=discord\&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)

CmlLib.Core is a Minecraft launcher library for .NET\
It supports all versions, including Forge

[한국어 README](https://github.com/AlphaBs/CmlLib.Core/blob/master/docs/README-kr.md)

## Features

* Asynchronous APIs
* Mojang Authentication
* Microsoft Xbox Live Login
* Download the game files from the Mojang file server
* Launch any version (tested up to 1.17.1)
* Launch Forge, Optifine, FabricMC, LiteLoader or any other custom version
* Install Java runtime
* Install Forge, LiteLoader, FabricMC
* Launch with options (direct server connecting, screen resolution)
* Cross-platform (Windows, Linux, macOS)

[Go to the wiki for all features](https://github.com/CmlLib/CmlLib.Core/wiki)

## Install

Install the [CmlLib.Core Nuget package](https://www.nuget.org/packages/CmlLib.Core)

or download the DLL files in [Releases](https://github.com/AlphaBs/CmlLib.Core/releases) and add references to them in your project.

Write this at the top of your source code:

```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
```

## Documentation

There are many features for custom launchers. Read the wiki to see all of the features.\
**Official documentation: [wiki](https://github.com/CmlLib/CmlLib.Core/wiki)**

## Quick start

### Microsoft Xbox Login

[Wiki](https://github.com/CmlLib/CmlLib.Core/wiki/Microsoft-Xbox-Live-Login)

### Mojang Login

[Login Process](https://github.com/AlphaBs/CmlLib.Core/wiki/Login-and-Sessions)

```csharp
var login = new MLogin();
var response = login.TryAutoLogin();

if (!response.IsSuccess) // failed to automatically log in
{
    var email = Console.ReadLine();
    var pw = Console.ReadLine();
    response = login.Authenticate(email, pw);

    if (!response.IsSuccess)
         throw new Exception(response.Result.ToString()); // failed to log in
}

// This session variable is the result of logging in and is used in MLaunchOption, in the Launch part below.
var session = response.Session;
```

### Offline Login

```csharp
// This session variable is the result of logging in and is used in MLaunchOption, in the Launch part below.
var session = MSession.GetOfflineSession("USERNAME");
```

### Launch

```csharp
// increase connection limit to fast download
System.Net.ServicePointManager.DefaultConnectionLimit = 256;

//var path = new MinecraftPath("game_directory_path");
var path = new MinecraftPath(); // use default directory

var launcher = new CMLauncher(path);

// show launch progress to console
launcher.FileChanged += (e) =>
{
    Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
};
launcher.ProgressChanged += (s, e) =>
{
    Console.WriteLine("{0}%", e.ProgressPercentage);
};

var versions = await launcher.GetAllVersionsAsync();
foreach (var item in versions)
{
    // show all version names
    // use this version name in CreateProcessAsync method.
    Console.WriteLine(item.Name);
}

var launchOption = new MLaunchOption
{
    MaximumRamMb = 1024,
    Session = MSession.GetOfflineSession("hello"), // Login Session. ex) Session = MSession.GetOfflineSession("hello")

    //ScreenWidth = 1600,
    //ScreenHeigth = 900,
    //ServerIp = "mc.hypixel.net"
};

//var process = await launcher.CreateProcessAsync("input version name here", launchOption);
var process = await launcher.CreateProcessAsync("1.15.2", launchOption); // vanilla
// var process = await launcher.CreateProcessAsync("1.12.2-forge1.12.2-14.23.5.2838", launchOption); // forge
// var process = await launcher.CreateProcessAsync("1.12.2-LiteLoader1.12.2"); // liteloader
// var process = await launcher.CreateProcessAsync("fabric-loader-0.11.3-1.16.5") // fabric-loader

process.Start();
```

## Example

[Sample Code](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)

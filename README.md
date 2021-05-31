# CmlLib.Core

## Minecraft Launcher Library

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/logo.png' width=150>

[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)

This library is minecraft launcher library for .NET 5.0 / .NET Core 3.1 / .NET Framework 4.6.2  
Support all version, with Forge

[한국어 README](https://github.com/AlphaBs/CmlLib.Core/blob/master/docs/README-kr.md)  
한국어 문서는 최신 버전이 아닙니다. 빠른 시일 내에 업데이트하겠습니다.  
궁금하신게 있다면 아래 디스코드 서버 접속해서 한국어 문의하기 채널에서 물어봐주세요.  

### Contact

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord&logo=discord&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)


## Version

Current version: 3.1.1

## Functions

-   [x] Online or Offline login
-   [x] Download the game files from the Mojang file server
-   [x] Launch any version (tested up to 1.16.2)
-   [x] Launch Forge, Optifine or any other custom version
-   [x] Download the Minecraft java runtime from the Mojang file server
-   [x] Install Forge
-   [x] Launch with options (direct server connecting, screen resolution)
-   [x] Supports cross-platform (windows, ubuntu, mac, only on .NET Core)
-   [x] Microsoft Xbox Live Login

## How to Install

Install the 'CmlLib.Core' Nuget package or download the dll files in [Releases](https://github.com/AlphaBs/CmlLib.Core/releases) and add references to them in your project.

Write this at the top of your source code:
```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
```
## How to Use

Official documentation: [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki)

**[Sample Code](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)**

**Login**

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
         throw new Exception(session.Result.ToString()) // failed to log in
}

// This session variable is the result of logging in and is used in MLaunchOption, in the Launch part below.
var session = response.Session;
```
**Offline Login**
```csharp
// This session variable is the result of logging in and is used in MLaunchOption, in the Launch part below.
var session = MSession.GetOfflineSession("USERNAME");
```
**Launch**
```csharp
//var path = new MinecraftPath("game_directory_path");
var path = new MinecraftPath(); // use default directory

var launcher = new CMLauncher(path);
launcher.FileChanged += (e) =>
{
    Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
};
launcher.ProgressChanged += (s, e) =>
{
    Console.WriteLine("{0}%", e.ProgressPercentage);
};

foreach (var item in launcher.GetAllVersions())
{
    Console.WriteLine(item.Name); // list version names
}

// more options : https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
var launchOption = new MLaunchOption
{
    MaximumRamMb = 1024,
    Session = session, // Login session. ex: Session = MSession.GetOfflineSession("hello")

    //ScreenWidth = 1600,
    //ScreenHeigth = 900,
    //ServerIp = "mc.hypixel.net"
};

// launch vanila
var process = launcher.CreateProcess("1.15.2", launchOption);

// launch forge (already installed, using this if you want to launch forge or optifine)
// var process = launcher.CreateProcess("1.16.2-forge-33.0.5", launchOption);

// launch forge (install forge if not installed, not stable, not recommended)
// var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);

process.Start();
```
### More information

Go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki)

### What is different between CmlLib.Core and [CmlLib](https://github.com/AlphaBs/MinecraftLauncherLibrary)?

CmlLib.Core is developed using .NET Core and supports cross platform, but CmlLib doesn't support it, and is deprecated.

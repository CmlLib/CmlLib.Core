# CmlLib.Core

## Minecraft Launcher Library

This library is minecraft launcher library for .NET Core and .NET Framework  
Support all version, with Forge

[한국어 README](https://github.com/AlphaBs/CmlLib.Core/blob/master/docs/README-kr.md)  
한국어 문서는 최신 버전이 아닙니다. 빠른 시일 내에 업데이트하겠습니다.

## Version

Current version: 3.0.0

Old versions:
* [v2.0.2](https://github.com/AlphaBs/CmlLib.Core/tree/v2.0.2)

## Functions

-   [x] Online or Offline login
-   [x] Download the game files from the Mojang file server
-   [x] Launch any version (tested up to 1.16.2)
-   [x] Launch Forge, Optifine or any other custom version
-   [x] Download the Minecraft java runtime from the Mojang file server
-   [x] Install Forge
-   [x] Launch with options (direct server connecting, screen resolution)
-   [x] Supports cross-platform (windows, ubuntu, mac, only on .NET Core)

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
    Console.WriteLine(item.Name);
}

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

// launch forge (already installed)
// var process = launcher.CreateProcess("1.16.2-forge-33.0.5", launchOption);

// launch forge (install forge if not installed)
// var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);

process.Start();
```
### More information

Go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption)

### What is different between CmlLib.Core and [CmlLib](https://github.com/AlphaBs/MinecraftLauncherLibrary)?

CmlLib.Core is developed using .NET Core and supports cross platform, but CmlLib doesn't support it, and is deprecated.

### Contact

Email: ksi123456ab@naver.com  
Discord: ksi123456ab#3719  
_(I prefer Discord)_

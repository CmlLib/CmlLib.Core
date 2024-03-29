# CmlLib.Core

## Minecraft 启动器库

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/icon.png' width=128>

[![Nuget Badge](https://img.shields.io/nuget/v/CmlLib.Core)](https://www.nuget.org/packages/CmlLib.Core)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3f55a130ec3f4bccb55e7def97cfa2ce)](https://www.codacy.com/gh/CmlLib/CmlLib.Core/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=CmlLib/CmlLib.Core\&utm_campaign=Badge_Grade)

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord\&logo=discord\&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)

CmlLib.Core 是.NET上的一个Minecraft启动库\
它支持所有版本，包括Forge。

## 特点

* 异步 API
* Mojang 认证
* 微软 Xbox Live 登录
* 从Mojang文件服务器下载游戏文件
* 启动所有版本 (最高测试到1.18.2)
* 启动Forge, Optifine, FabricMC, LiteLoader或者其他任何自定义的版本。
* 安装Java运行时
* 安装LiteLoader、FabricMC。
* 自定义参数启动 (服务器直连, 屏幕分辨率)
* 跨平台 (Windows, Linux, macOS)

[去Wiki查看所有特性](https://alphabs.gitbook.io/cmllib/cmllib.core/cmllib)

## 安装

在Nuget上安装 [CmlLib.Core](https://www.nuget.org/packages/CmlLib.Core)

或从 [Releases](https://github.com/AlphaBs/CmlLib.Core/releases) 中下载DLL文件，并将其引用至你的项目。

在源代码的顶端写入下列语句：

```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
```

## 文档

自定义启动器具有许多特性，查看文档了解所有特性： \
**官方文档: [wiki](https://github.com/CmlLib/CmlLib.Core/wiki)**

## 快速开始

### 微软 Xbox 登录

[CmlLib.Core.Auth.Microsoft](https://github.com/CmlLib/CmlLib.Core.Auth.Microsoft)\
[文档](https://github.com/CmlLib/CmlLib.Core/wiki/Microsoft-Xbox-Live-Login)

### Mojang 登录

[登录过程](https://github.com/AlphaBs/CmlLib.Core/wiki/Login-and-Sessions)

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

### 离线登录

```csharp
// This session variable is the result of logging in and is used in MLaunchOption, in the Launch part below.
var session = MSession.GetOfflineSession("USERNAME");
```

### 启动

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
    Session = session, // replace this with login session value. ex) Session = MSession.GetOfflineSession("hello")

    //ScreenWidth = 1600,
    //ScreenHeight = 900,
    //ServerIp = "mc.hypixel.net"
};

//var process = await launcher.CreateProcessAsync("input version name here", launchOption);
var process = await launcher.CreateProcessAsync("1.15.2", launchOption); // vanilla
// var process = await launcher.CreateProcessAsync("1.12.2-forge1.12.2-14.23.5.2838", launchOption); // forge
// var process = await launcher.CreateProcessAsync("1.12.2-LiteLoader1.12.2"); // liteloader
// var process = await launcher.CreateProcessAsync("fabric-loader-0.11.3-1.16.5") // fabric-loader

process.Start();
```

## 示例

[示例代码](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)

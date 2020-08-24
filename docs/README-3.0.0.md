# CmlLib.Core

## Minecraft Launcher Library

This library is minecraft launcher library for .NET Core and .NET Framework  
Support all version, with Forge

[한국어 README](https://github.com/AlphaBs/CmlLib.Core/blob/master/docs/README-kr.md)  
한국어 문서는 최신 버전이 아닙니다. 빠른 시일 내에 업데이트하겠습니다.

## Version

3.0.0-alpha3 - current   
[release 2.0.2](https://github.com/AlphaBs/CmlLib.Core/tree/v2.0.2)

## Functions

-   [x] Online / Offline Login
-   [x] Download game files in mojang file server
-   [x] Launch All Versions (tested up to 1.16.2)
-   [x] Launch Forge, Optifine or custom versions
-   [x] Download minecraft java runtime in mojang file server
-   [x] Launch with options (direct server connecting, screen resolution)
-   [x] Support cross-platform (win10, ubuntu, mac, only on .NET Core)

## **Install**

Install Nuget Package 'CmlLib.Core'  
or download dll files in [Releases](https://github.com/AlphaBs/CmlLib.Core/releases) and add reference to your project.

write this on the top of your source code:

     using CmlLib.Core;

## How To Use

Official Documentation : [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki)

**[Sample Code](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)**

**Login**

     var login = new MLogin();
     var response = login.TryAutoLogin();

     if (!response.IsSuccess) // failed to auto login
     {
         var email = Console.ReadLine();
         var pw = Console.ReadLine();
         response = login.Authenticate(email, pw);

         if (!response.IsSuccess)
              throw new Exception(session.Result.ToString()) // failed to login
     }

     // This session variable is the result of login.
     // and used in MLaunchOption, in the Launch part below
     var session = response.Session;

**Offline Login**

     // This session variable is the result of login.
     // and used in MLaunchOption, in the Launch part below
     var session = MSession.GetOfflineSession("USERNAME");

**Launch**

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
         Session = session, // Login Session. ex) Session = MSession.GetOfflineSession("hello")

         //ScreenWidth = 1600,
         //ScreenHeigth = 900,
         //ServerIp = "mc.hypixel.net"
     };

     // launch vanila
     var process = launcher.CreateProcess("1.15.2", launchOption);
     
     // launch forge
     // var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);

     process.Start();

### More Information

Go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption)

### What is different between CmlLib.Core and [CmlLib](https://github.com/AlphaBs/MinecraftLauncherLibrary)?

CmlLib.Core is developed using .NET Core and support crossplatform. but CmlLib doesn't support it is deprecated.

### Contacts

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719  
*(I prefer discord)*

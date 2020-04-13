# CmlLib.Core

## Minecraft Launcher Library

This library is minecraft launcher library for .NET Core and .NET Framework  
Support all version, with Forge

## What is different between CmlLib.Core and [CmlLib](https://github.com/AlphaBs/MinecraftLauncherLibrary)?
CmlLib.Core is developed using .NET Core and support crossplatform. but CmlLib doesn't support it and will be deprecated.

## Contacts

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719

## License

MIT License

## Crossplatform

.NET Core version support crossplatform. It is tested in Windows10, Ubuntu 18.04, macOS Catalina

## Dependency

Newtonsoft.Json  
SharpZipLib  

## Functions

-   [x] Online / Offline Login
-   [x] Download game files in mojang file server
-   [x] Launch All Versions (tested up to 1.15.2)
-   [x] Launch Forge, Optifine or custom versions
-   [x] Download minecraft java runtime in mojang file server
-   [x] Launch with options (direct server connecting, screen resolution)
-   [x] Support cross-platform

## How To Use

If you want to learn more features of this library, go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki)

**[Sample Code](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)**

### **Install**

Install Nuget Package 'CmlLib.Core'  
or download dll files in [Releases](https://github.com/AlphaBs/CmlLib.Core/releases) and add reference to your project.

write this on the top of your source code:

     using CmlLib;
     using CmlLib.Core;

### **Sample**

**Login**

     var login = new MLogin();
     var session = login.TryAutoLogin(); // 'session' is used in LaunchOption

     if (session.Result != MLoginResult.Success) // failed to auto login
     {
         var email = Console.ReadLine();
         var pw = Console.ReadLine();
         session = login.Authenticate(email, pw);

         if (session.Result != MLoginResult.Success)
              throw new Exception(session.Result.ToString()) // failed to login
     }

**Offline Login**

     var session = MSession.GetOfflineSession("USERNAME"); // 'session' is used in LaunchOption

**Launch**

     //var path = new Minecraft("your minecraft directory);
     var path = Minecraft.GetOSDefaultPath(); // mc directory

     var launcher = new CmlLib.CMLauncher(path);
     launcher.ProgressChanged += (s, e) =>
     {
          Console.WriteLine("{0}%", e.ProgressPercentage);
     };
     launcher.FileChanged += (e) =>
     {
          Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
     };

     foreach (var item in launcher.ProfileInfos)
     {
         Console.WriteLine(item.Name);
     }

     var launchOption = new MLaunchOption
     {
         MaximumRamMb = 1024,
         Session = session, // Login Session. ex) Session = MSession.GetOfflineSession("hello")

         //LauncherName = "MyLauncher",
         //ScreenWidth = 1600,
         //ScreenHeigth = 900,
         //ServerIp = "mc.hypixel.net"
     };

     // launch forge
     //var process = launcher.Launch("1.12.2", "14.23.5.2768", launchOption);

     // launch vanila
     var process = launcher.Launch("1.15.2", launchOption);

     process.Start();


**More**
Go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption)

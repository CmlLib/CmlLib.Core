Minecraft Launcher Library
======================

### Online / Offline Login, Download, Launch with various options, Forge Support

### Support All Versions, Forge
### Sample Project(not completed) : https://github.com/AlphaBs/AlphaMinecraftLauncher  
  
한국어
-------------
 =====>>> [한국어 README](https://github.com/AlphaBs/MinecraftLauncherLibrary/blob/master/README-kr.md)

Contacts
-------------

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719  
KaKaoTalk : ksi123456ab

License
--------------

<a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc/4.0/88x31.png" /></a><br />This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/">Creative Commons Attribution-NonCommercial 4.0 International License</a>.

****NO COMMERCIAL****

Dependancy
-------------
Newtonsoft.Json
DotNetZip

Use Nuget-Restore

How To Use
-------------

*Sorry about my poor english skill*

If you want to learn more like java runtime download, detail launch options, full methods, go to [wiki](https://merong)

#### 1. Prepare
Open the 'CmlLib' Project, restore nuget packages, build CmlLib Project.
and add reference to 'CmlLib.dll', 'Newtonsoft.Json.dll', 'DotNetZip.dll' to your own project.

write this on the top of your source code.


      using CmlLib.Launcher;

#### 2. Minecraft Initialize
You should write this code before work.

      Minecraft.Initialize("GAME_DIRECTORY");

It set Game Directory that is used to download game files, load profiles, save login session, Launch, etc...

#### 3. Login

     MLogin login = new MLogin();
     MSession session = null;

     session = login.TryAutoLogin();
     if (session.Result != MLoginResult.Success)
     {
          session = login.Authenticate(
               "YOUR_MOJANG_EMAIL",
               "PASSWORD");

          if (session.result != MLoginResult.Success)
               throw new Exception("Wrong Account");
     }

     Console.WriteLine("Hello, " + session.Username);

The 'session' is login result.
if you want to connect online-mode server, you use this session to launch.
note : you can't use old login which use username instead mojang email.

or you can use offline session :

     MSession session = MSession.GetOfflineSession("USERNAME");

#### 4. Get Profile Infos
Profile contain various data which launcher need.
All Game Versions has its own profile, even old alpha version or forge.
You can find at (GameDirectory)￦versions￦(any-version)￦(version-name).json.
MProfileInfo is Profile's Metadata, contains Name, Profile Path(Url), Type(Release, Snapshot, Old), ReleaseTime.
and this code get profile info :

     MProfilesInfo[] infos = MProfileInfo.GetProfiles();

or you can choose source :

     // get profiles from mojang server
     var web = MProfileInfo.GetProfilesFromWeb();
     // get profiles from game directory
     var local = MProfileInfi.GetProfilesFromLocal();

#### 5. Choose ProfileInfo and Parse.

In order to use profile's data, you should parse Profile from ProfileInfo.

First, Search Profile Infos you want to launch : (very simple search algorithm)

     MProfile profile = null;
     foreach (var item in infos)
     {
          if (item.Name == "1.7.10")
          {
                profile = item;
                break;
          }
     }

Parse Profile is simple :

     MProfile profile = MProfile.Parse(info);

#### 6. Check & Download Game Files

     MDownloader downloader = new MDownloader(profile);
     downloader.ChangeFileChange += change_file;
     downloader.ChangeProgressChange += change_progress;
     downloader.DownloadAll();

ChangeFileChange Event : Change Download File Name

ChangeProgressChange : Change One File's Download Progress

DownloadAll : Check And Download All Game Files

DownloadAll() Method do this :

     // downloader.DownloadAll() do this :
     
     downloader.DownloadLibrary(); // libraries
     downloader.DownloadIndex(); // asset index
     downloader.DownloadResource(); // assets / resources
     downloader.DownloadMinecraft(); // game jar

It's so long. so just use DownloadAll();

Each Download~~ method check game file exist, and if file doesn't exist, download game file from mojang server.

#### 7. Make game args and Launch

     var option = new MLaunchOption()
     {
          // must require
          StartProfile = profile,
          JavaPath = "java.exe", //SET YOUR JAVA PATH (if you want autoset, goto wiki)
          MaximumRamMb = int.Parse(xmx),
          Session = session,
          
          // not require
          ServerIP = "", // connect server directly
          LauncherName = "", // display launcher name at main window
          CustomJavaParameter = "" // set your own java args
     };
     
     var launch = new MLaunch(option);
     var process = launch.MakeProcess();
     process.Start();

Set launch options, and Launch it!

#### 8. Launch Forge

goto [wiki](https://merong)

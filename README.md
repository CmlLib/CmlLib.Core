Csharp Minecraft Launcher Library  
======================
### CmlLib 0.0.9 <1.4 forge, optifine>

### Online / Offline Login, Download, Launch with various options, Forge Support


### Support All Versions, Forge
### see CmlLibSample project in this repo

Only Windows (use [pml](https://github.com/AlphaBs/pml) if you want crossplatform)

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

Crossplatform
-------------
This library doesn't support crossplatform. Only Windows  
if you want Cross-Platform Minecraft Launcher Library,  
use this python library.  
[pml github](https://github.com/AlphaBs/pml)

Dependancy
-------------
Newtonsoft.Json  
DotNetZip

Use Nuget-Restore

How To Use
-------------

*Sorry about my poor english skill*

If you want to learn more features of this library such as to download java runtime or launch with more detailed options, go to wiki

**[Sample Code](https://github.com/AlphaBs/MinecraftLauncherLibrary/wiki/Sample-Code)**

#### 1. Prepare

Install Nuget Package 'CustomMinecraftLauncher'  
or download dll files in Release tab (CmlLib.dll, Newtonsoft.Json.dll, DotNetZip.dll) and add reference

write this on the top of your source code:  


      using CmlLib.Launcher;

#### 2. Minecraft Initialize
You should write this code before work.

      Minecraft.Initialize("GAME_DIRECTORY");

It set Game Directory that is used to download game files, load profiles, save login session, Launch, etc...  
**You can't use relative path.**

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
               throw new Exception("Failed login : " + session.result.ToString());
     }

     Console.WriteLine("Hello, " + session.Username);

The 'session' is login result.
note : you can't use old login using username instead of mojang email.

or you can use offline session :

     MSession session = MSession.GetOfflineSession("USERNAME");

#### 4. Get Profile Infos
Profile contain various data which launcher need.
All Game Versions has its own profile, even old alpha version and forge.
You can find it at (GameDirectory)￦versions￦(any-version)￦(version-name).json.
MProfileInfo is metadata of profile, containing Name, Profile Path(Url), Type(Release, Snapshot, Old), ReleaseTime.
and this code get profile info :

     MProfilesInfo[] infos = MProfileInfo.GetProfiles();

It will return all metadata from mojang web server and your game directory.  
but you can choose source :

     // get profiles from mojang server
     var web = MProfileInfo.GetProfilesFromWeb();
     // get profiles from game directory
     var local = MProfileInfi.GetProfilesFromLocal();

#### 5. Choose ProfileInfo and Parse.

In order to use profile data, you should parse profile.  
This simple code will search version from metadatas, and return parsed profile data.

     MProfile profile = MProfile.GetProfile(infos, "1.14.4");

#### 6. Check & Download Game Files

     MDownloader downloader = new MDownloader(profile);
     downloader.ChangeFile += change_file;
     downloader.ChangeProgress += change_progress;
     downloader.DownloadAll();

ChangeFile : when download file was changed

ChangeProgress : when the progress of current downloading file was changed

     private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
     {
         // 20%, 30%, 80%, ...
         Console.WriteLine("{0}%", e.ProgressPercentage);
     }
 
     private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
     {
         // [Library] hi.jar - 3/51
         Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.CurrentValue, e.MaxValue);
     }

Download method check the existence of game file, and download game file from mojang server if file is not exist or not valid(compare hash).  

#### 7. Make game args and Launch

     var option = new MLaunchOption()
     {
          // must require
          StartProfile = profile,
          JavaPath = "java.exe", //SET YOUR JAVA PATH (if you want autoset, goto wiki)
          MaximumRamMb = 1024,
          Session = session,
          
          // not require
          ServerIP = "", // connect server directly
          LauncherName = "", // display launcher name at main window
          CustomJavaParameter = "" // set your own java args
     };
     
     var launch = new MLaunch(option);
     var process = launch.MakeProcess();
     process.Start();

set launch options, and launch it!


#### More Information

**[Sample Code](https://github.com/AlphaBs/MinecraftLauncherLibrary/wiki/Sample-Code)**  

launch forge : You don't need any additional work to launch forge  

bugs : go to issue tab



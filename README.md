Minecraft Launcher Library
======================

### Online / Offline Login, Download, Launch with various options, Forge Support

### Support All Versions, Forge
### Sample Project(not completed) : https://github.com/AlphaBs/AlphaMinecraftLauncher

KOREANS :  
상업적 사용 금지.  수정 후 재사용 가능.  
자세한 내용은 LICENSE 단락을 확인하세요.  
주문제작 문의는 아래 아이디로 디스코드 친추걸어주세요.

Contacts
-------------

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719  
KaKaoTalk : ksi123456ab

License
--------------

<a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/"><img alt="크리에이티브 커먼즈 라이선스" style="border-width:0" src="https://i.creativecommons.org/l/by-nc/4.0/88x31.png" /></a><br />이 저작물은 <a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/">크리에이티브 커먼즈 저작자표시-비영리 4.0 국제 라이선스</a>에 따라 이용할 수 있습니다.

****NO COMMERCIAL****

How To Use
-------------

*Sorry about my poor english skill*

If you want to learn more like java runtime download, detail launch options, full methods, go to [wiki](https://merong)

#### 1. Prepare
Build 'CmlLib' project yourself and add reference to your project.

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

or you can use offline session :

     MSession session = MSession.GetOfflineSession("USERNAME");

note : you can't use old login which use username instead mojang email.

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

First, Search Profile Infos you want to launch : (very simple search algoritm)

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

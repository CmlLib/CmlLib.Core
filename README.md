Minecraft Launcher Library
======================

### Online / Offline Login, Download, Launch with various options, Forge Support

#### Support ~1.3 , Forge
#### Sample Project(not completed) : Sample_Url

Made by AlphaBs.

github_url

*Sorry about my poor english skill*

How To Use
-------------
#### 1. Prepare
Build 'CmlLib' project yourself and add reference to your project.

#### 2. Minecraft Initialize
You must write this code before work.

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
if you want connect online-mode server, you should need this session.

or you can use offline session :

     MSession session = MSession.GetOfflineSession("USERNAME");

note : you can't use old login which use username instead mojang email.

#### 4. Get Profile Infos
Profile contain various data which launcher need.

All Game Versions has its profile, even old alpha version or forge.

You can find at
(GameDirectory)￦versions￦(any-version)￦(version-name).json.

Profile info is Profile's Metadata, contains Name, Path, Type(Release, Snapshot, Old), etc...

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

#### 6. 


--Writing--

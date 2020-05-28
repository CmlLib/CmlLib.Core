# CmlLib.Core

## Minecraft Launcher Library

.NET Core와 .NET Framework 를 지원하는 마인크래프트 런처 라이브러리입니다.
포지를 포함한 모든 버전 실행 가능합니다

## CmlLib.Core와 [CmlLib](https://github.com/AlphaBs/MinecraftLauncherLibrary)의 차이점?
CmlLib.Core 는 .NET Core 에서도 동작하도록 만들어졌으며, 윈도우 뿐만 아니라 다른 운영체제에서도 작동합니다.  
기존 라이브러리(MinecraftLauncherLibrary)는 더이상 개발하지 않습니다.  

## Contacts

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719

## License

MIT License  
자세한 설명 : [LICENSE](https://github.com/AlphaBs/CmlLib.Core/blob/master/LICENSE)


## Crossplatform

.NET Core 버전은 크로스플랫폼을 지원합니다. Windows10, Ubuntu 18.04, macOS Catalina 에서 테스트하였습니다.  

## 종속성

Newtonsoft.Json 12.0.3  
SharpZipLib 1.2.0  
LZMA-SDK 19.0.0  

## Functions

- [x] 정품/복돌 로그인
- [x] 게임 서버에서 게임 파일 다운로드
- [x] 모든 버전 실행 (1.15.2 까지 테스트)
- [x] 포지, 옵티파인 등 커스텀 버전 실행 가능
- [x] 자바 런타임 다운로드
- [x] 다양한 실행 옵션 (서버 주소, 창 크기, 런처 이름 등)
- [x] 크로스플랫폼 지원

## 사용법

더 자세한 내용은 위키에 있습니다. [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki)

**[샘플 코드](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)**

### **설치**

Nuget Package `CmlLib.Core` 를 설치하세요.  
혹은 dll 파일을 직접 다운로드한 후 참조 추가하세요 : [Releases](https://github.com/AlphaBs/CmlLib.Core/releases)  

소스코드 최상단에 아래 코드를 입력하세요:  

     using CmlLib;
     using CmlLib.Core;

### **예시 코드**

**로그인**

     var login = new MLogin();
     var session = login.TryAutoLogin(); // 'session' 변수는 실행할 때 LaunchOption 에서 사용됩니다.

     if (session.Result != MLoginResult.Success) // 자동 로그인 실패시
     {
         var email = Console.ReadLine();
         var pw = Console.ReadLine();
         session = login.Authenticate(email, pw);

         if (session.Result != MLoginResult.Success)
              throw new Exception(session.Result.ToString()) // 로그인 실패
     }

**복돌 로그인**

     var session = MSession.GetOfflineSession("USERNAME"); // 'session' 변수는 실행할 때 LaunchOption 에서 사용됩니다.

**실행**

     //var path = new Minecraft("게임 폴더 경로");
     var path = Minecraft.GetOSDefaultPath(); // 기본 게임 폴더

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
         Session = session, // 로그인 세션. ex) Session = MSession.GetOfflineSession("hello")

         //LauncherName = "MyLauncher",
         //ScreenWidth = 1600,
         //ScreenHeigth = 900,
         //ServerIp = "mc.hypixel.net"
     };

     // 포지 실행
     //var process = launcher.Launch("1.12.2", "14.23.5.2768", launchOption);

     // 바닐라 실행
     var process = launcher.Launch("1.15.2", launchOption);

     process.Start();


### More Information 
Go to [wiki](https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption)

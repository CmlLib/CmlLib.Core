# Csharp 마인크래프트 런처 라이브러리

## CmlLib 1.0.0
 
 모든 게임 버전 지원, 포지 지원  
 윈도우만 지원

## Contacts

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719  

## License

<a rel="license" href="http://creativecommons.org/licenses/by/4.0/"><img alt="크리에이티브 커먼즈 라이선스" style="border-width:0" src="https://i.creativecommons.org/l/by/4.0/88x31.png" /></a><br />이 저작물은 <a rel="license" href="http://creativecommons.org/licenses/by/4.0/">크리에이티브 커먼즈 저작자표시 4.0 국제 라이선스</a>에 따라 이용할 수 있습니다.

## Crossplatform

이 라이브러리는 윈도우만 지원합니다.  
크로스플랫폼 라이브러리가 필요하다면 pml을 사용하세요.  
[pml github](https://github.com/AlphaBs/pml)

## Dependency

Newtonsoft.Json  
DotNetZip

## Functions

- [x] 정품/복돌 로그인
- [x] 게임 서버에서 게임 파일 다운로드
- [x] 모든 버전 실행 (1.14.4 까지 태스트)
- [x] 포지, 옵티파인 등 커스텀 버전 실행 가능
- [x] 자바 런타임 다운로드
- [x] 다양한 실행 옵션 (서버 주소, 창 크기, 런처 이름 등)
- [ ] .NET CORE 로 포팅 (크로스플랫폼) ([pml](https://github.com/AlphaBs/pml))

## How To Use

아래는 간략한 사용방법만 소개합니다. 자세한 정보는 wiki로

**[Sample Code](https://github.com/AlphaBs/MinecraftLauncherLibrary/wiki/Sample-Code)**

### **1. 준비**

Nuget 패키지 관리자에서 'CustomMinecraftLauncher' 를 검색하고 설치하세요.  
혹은 release 에서 dll 파일들(CmlLib.dll, Newtonsoft.Json.dll, DotNetZip.dll)을 다운받고 참조 추가를 해주세요. 

소스 최상단에 아래 코드를 입력하세요 :  


      using CmlLib.Launcher;

### **2. 게임 폴더**

라이브러리 기능을 사용하기 전에 게임 폴더 설정을 반드시 해주세요.

      Minecraft.Initialize("게임폴더 경로");

위 코드가 게임 파일 다운로드, 프로필 로드, 게임 세션 저장, 실행 등에 필요한 게임 폴더를 설정합니다.  
**절대 경로를 입력해 주세요.**

### **3. 로그인**

     MLogin login = new MLogin();
     MSession session = null;

     session = login.TryAutoLogin();
     if (session.Result != MLoginResult.Success)
     {
          session = login.Authenticate(
               "모장이메일",
               "비밀번호");

          if (session.result != MLoginResult.Success)
               throw new Exception("로그인 실패 : " + session.result.ToString());
     }

     Console.WriteLine("Hello, " + session.Username);

session 변수에 로그인 결과가 저장됩니다.  
참고 : 모장 이메일 대신 닉네임을 입력하는 옛날 로그인 방식은 사용할 수 없습니다.  

혹은 복돌 로그인도 사용할 수 있습니다 :

     MSession session = MSession.GetOfflineSession("닉네임");

### **4. 프로필 불러오기**

프로필은 런처에서 사용하는 다양한 정보가 포함되어 있습니다. 모든 버전은 프로필을 가지고 있으며,  (GameDirectory)￦versions￦(any-version)￦(version-name).json 파일 혹은 모장 서버에 저장되어 있습니다.  

MProfileInfo 은 프로필의 메타데이터를 나타내는 클래스입니다. 

     MProfileInfo[] infos = MProfileInfo.GetProfiles();
     foreach (var item in infos)
     {
          Console.WriteLine(item.Type + " : " + item.Name);
     }

위 코드는 모장 서버에 저장된 프로필과 게임 폴더에 저장된 모든 프로필을 표시합니다.  

### **5. 프로필 선택, 파싱**

프로필을 사용하기 위해서는 프로필을 파싱해야 합니다. 아래 코드는 프로필을 찾고 파싱해서 반환해줍니다.  

     MProfile profile = MProfile.FindProfile(infos, "1.14.4");

### **6. 게임 파일 확인/다운로드**

     MDownloader downloader = new MDownloader(profile);
     downloader.ChangeFile += change_file;
     downloader.ChangeProgress += change_progress;
     downloader.DownloadAll();

다운로드 이벤트 헨들러 :  

     private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
     {
         // 다운로드하는 파일의 진행률
         // 20%, 30%, 80%, ...
         Console.WriteLine("{0}%", e.ProgressPercentage);
     }
 
     private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
     {
         // 다운로드하는 파일이 바뀌었을때
         // [Library] hi.jar - 3/51
         Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
     }

DownloadAll() 메서드는 게임 파일의 존재 여부, 무결성을 검사하고 올바르지 않은 파일이라면 게임 파일을 모장 서버에서 다운로드하는 역할을 합니다.   

### **7. 게임 인수 생성 후 실행**

     var option = new MLaunchOption()
     {
          // 필수 인수
          StartProfile = profile,
          JavaPath = "java.exe", //자바 경로 설정
          MaximumRamMb = 1024, // MB
          Session = session,
          
          // 필수 아님
          ServerIP = "", // 서버로 바로 접속
          LauncherName = "", // 게임 메인화면에 런처 이름 표시
          CustomJavaParameter = "" // JVM 인수 설정
     };
     
     var launch = new MLaunch(option);
     var process = launch.GetProcess();
     process.Start();

게임 옵션을 설정하고 실행하면 됩니다.  

### **8. More Information**

**[Sample Code](https://github.com/AlphaBs/MinecraftLauncherLibrary/wiki/Sample-Code)**  

포지 실행 : 위 코드대로 만들면 실행 됩니다. 

버그 : issue 에서 알려주세요.





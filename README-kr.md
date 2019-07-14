마인크래프트 런처 라이브러리
======================

### 정품 / 복돌 로그인, 다운로드, 다양한 옵션으로 실행, 포지 실행 가능

### 지원 버전 : 모든 버전, 포지
### 샘플 프로젝트(미완성) : https://github.com/AlphaBs/AlphaMinecraftLauncher
### 소스코드 안에 CmlLibSample 프로젝트 참고

상업적 사용 금지.  수정 후 재사용 가능.  
자세한 내용은 라이센스 단락을 확인하세요.  
주문제작 문의는 아래 아이디로 디스코드 친추걸어주세요.

연락
-------------

Email : ksi123456ab@naver.com  
Discord : ksi123456ab#3719  
KaKaoTalk : ksi123456ab

라이센스
--------------

<a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/"><img alt="크리에이티브 커먼즈 라이선스" style="border-width:0" src="https://i.creativecommons.org/l/by-nc/4.0/88x31.png" /></a><br />이 저작물은 <a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/">크리에이티브 커먼즈 저작자표시-비영리 4.0 국제 라이선스</a>에 따라 이용할 수 있습니다.

****상업적 이용 금지****

종속성
-------------


Newtonaoft.Json
DotNetZip

Nuget 복구 기능을 사용하세요.

사용 방법
-------------


자바 런타임 다운로드, 세부적인 실행 옵션,  등등 모든 기능을 보고 싶다면 위키로 가세요. [wiki](https://merong)

#### 1. 준비
소스코드를 다운받고, CmlLib 프로젝트를 컴파일 한 뒤에 당신의 프로젝트 파일에 CmlLib.dll , DotNetZip.dll , Newtonsoft.Json.dll 파일을 참조 추가하세요.

그리고 소스코드 최상단에 이걸 넣으세요 :


using CmlLib.Launcher;

#### 2. 마인크래프트 폴더 설정
실행 코드를 적기 전에, 반드시 마인크래프트 폴더를 설정해야 합니다.

      Minecraft.Initialize("설정할 마인크래프트 폴더의 경로");

이 경로가 게임 파일을 다운로드하고, 실행할때 사용됩니다.  
**상대 경로 사용 불가능. 반드시 절대 경로만 입력하세요.**

#### 3. 로그인

     MLogin login = new MLogin();
     MSession session = null;

     session = login.TryAutoLogin();
     if (session.Result != MLoginResult.Success)
     {
          session = login.Authenticate(
               "모장 이메일",
               "비밀번호");

          if (session.result != MLoginResult.Success)
               throw new Exception("Wrong Account");
     }

     Console.WriteLine("Hello, " + session.Username);

'session' 변수에 로그인 결과가 저장됩니다.
정품 서버에 접속하려면 실행할때 이 세션을 넣어주면 됩니다.

아니면 복돌 세션도 사용할 수 있습니다 : 

     MSession session = MSession.GetOfflineSession("닉네임");

참고 : 정품 로그인에서 이메일 대신 닉네임으로 로그인하는 옛날 로그인 방식은 사용 불가능합니다.

#### 4. 프로필 정보 가져오기
프로필에는 실행에 필요한 다양한 정보가 포함되어 있습니다.
모든 버전들은 자신만의 프로필이 있습니다. 스냅샷, 옛날 버전들, 포지조차도 자신의 프로필이 있습니다.
이 프로필은 (마크경로)￦versions￦(아무 버전)￦(버전이름).json 파일에 있습니다.
MProfileInfo 는 프로필의 이름, 경로(URL), 종류(릴리즈, 스냅샷, 올드), 릴리즈타임이 포함된 메타데이터가 들어있는 클래스입니다.
아래 코드로 모든 MProfileInfo 들을 가져올 수 있습니다 :

     MProfilesInfo[] infos = MProfileInfo.GetProfiles();

아니면 프로필 정보를 가져올 곳을 선택할 수도 있습니다.

     // 모장 서버에서 가져오기
     var web = MProfileInfo.GetProfilesFromWeb();
     // versions 폴더에서 가져오기
     var local = MProfileInfi.GetProfilesFromLocal();

#### 5. 프로필을 선택하고 파싱

프로필의 데이터를 사용하기 위해서는 MProfileInfo 의 정보를 사용해서 해당 프로필을 파싱해야 합니다.

먼저, 실행할 프로필을 검색합니다. (간단한 검색 알고리즘 사용) : 

     MProfile profile = null;
     foreach (var item in infos)
     {
          if (item.Name == "1.7.10") // 1.7.10 검색
          {
                profile = item;
                break;
          }
     }

프로파일 파싱하기 :

     MProfile profile = MProfile.Parse(info);

Parse 메서드에 실행할 MProfileInfo의 인스턴스를 넣어주면 됩니다.

#### 6. 게임 파일 확인 / 다운로드

     MDownloader downloader = new MDownloader(profile); // 위에서 파싱한 프로필
     downloader.ChangeFileChange += change_file;
     downloader.ChangeProgressChange += change_progress;
     downloader.DownloadAll();

ChangeFileChange Event : 다운받는 파일이름이 바뀔때

ChangeProgressChange : 다운받는 파일의 진행률

DownloadAll : 모든 게임 파일을 확인/다운로드

DownloadAll() 메서드는 아래와 같은 역할을 합니다 :

     // downloader.DownloadAll() do this :
     
     downloader.DownloadLibrary(); // libraries
     downloader.DownloadIndex(); // asset index
     downloader.DownloadResource(); // assets / resources
     downloader.DownloadMinecraft(); // game jar

너무 기니까 그냥 DownloadAll() 하세요.

각각의 Download~~~ 메서드는 게임파일이 있는지 확인하고 없으면 모장 서버에서 다운받는 일을 합니다.

#### 7. 실행 인수 만들고 실행

     var option = new MLaunchOption()
     {
          // 필수 옵션
          StartProfile = profile,
          JavaPath = "java.exe", //자바 경로 (자동 자바 설치는 위키 참고)
          MaximumRamMb = 1024,
          Session = session, // 로그인 세션
          
          // 필수 아님
          ServerIP = "", // 설정한 아이피로 바로 잡속
          LauncherName = "", // 메인 화면에 표시할 런처 이름
          CustomJavaParameter = "" // 커스텀 자바 파라미터
     };
     
     var launch = new MLaunch(option);
     var process = launch.MakeProcess();
     process.Start();

실행 옵션을 지정하고 실행하면 됩니다.

#### 8. 포지 실행 방법

위키 참고 [wiki](https://merong)

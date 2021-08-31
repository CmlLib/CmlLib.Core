# CmlLib.Core

## Minecraft Launcher Library

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/icon.png' width=128>

[![Nuget Badge](https://img.shields.io/nuget/v/CmlLib.Core)](https://www.nuget.org/packages/CmlLib.Core)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3f55a130ec3f4bccb55e7def97cfa2ce)](https://www.codacy.com/gh/CmlLib/CmlLib.Core/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=CmlLib/CmlLib.Core\&utm_campaign=Badge_Grade)

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord\&logo=discord\&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)

CmlLib.Core 는 마인크래프트 커스텀 런처 제작을 위한 C# 라이브러리입니다.\
포지를 포함한 모든 버전을 실행 가능합니다.\

## [AD - 주문제작]
**커스텀 런처 주문제작을 받고 있습니다!**  
라이브러리에는 없는 다양한 추가 기능들이 포함되어 있습니다.  
ksi123456ab#3719 디스코드로 연락주세요.

## 기능

*   비동기 APIs
*   모장 인증(로그인)
*   마이크로소프트 엑스박스 계정으로 로그인
*   모장 파일 서버에서 게임 파일 다운로드
*   모든 버전 실행(1.17.1 까지 테스트 완료)
*   모든 커스텀 버전(포지, 옵티파인, Fabric, 라이트로더 등등) 실행 가능
*   자바 런타임 설치
*   Forge, LiteLoader, FabricMC 설치
*   다양한 실행 옵션 (서버 바로 접속, 화면 크기 조정 등)
*   크로스플랫폼 (windows, ubuntu, macOS)

[모든 기능 보기](https://github.com/CmlLib/CmlLib.Core/wiki)

## 설치

[CmlLib.Core Nuget package](https://www.nuget.org/packages/CmlLib.Core) 를 설치하거나,

[Releases](https://github.com/AlphaBs/CmlLib.Core/releases) 에서 dll 파일을 다운받고 프로젝트에 참조 추가하세요.

소스코드 최상단에 아래 소스코드를 입력하세요:

```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
```

## 문서

커스텀 런처를 위한 많은 기능들이 있습니다. 위키에서 모든 기능 목록을 확인하세요.\
**공식 문서: [wiki](https://github.com/CmlLib/CmlLib.Core/wiki)**

## QuickStart

### 마이크로소프트 엑스박스 계정으로 로그인

[Wiki](https://github.com/CmlLib/CmlLib.Core/wiki/Microsoft-Xbox-Live-Login)

### 모장 계정으로 로그인

[Login Process](https://github.com/AlphaBs/CmlLib.Core/wiki/Login-and-Sessions)

```csharp
var login = new MLogin();
var response = login.TryAutoLogin();

if (!response.IsSuccess) // 자동 로그인 실패
{
    var email = Console.ReadLine();
    var pw = Console.ReadLine();
    response = login.Authenticate(email, pw);

    if (!response.IsSuccess)
         throw new Exception(response.Result.ToString()); // 로그인 실패
}

// session 변수가 로그인 결과를 나타내는 변수입니다. 아래 실행 부분에 있는 MLaunchOption에 같이 넣어서 게임을 실행하면 됩니다.
var session = response.Session;
```

### Offline Login

```csharp
// session 변수가 로그인 결과를 나타내는 변수입니다. 아래 실행 부분에 있는 MLaunchOption에 같이 넣어서 게임을 실행하면 됩니다.
var session = MSession.GetOfflineSession("USERNAME");
```

### 실행

```csharp
// 빠른 다운로드를 위해 커넥션 제한을 늘립니다.
System.Net.ServicePointManager.DefaultConnectionLimit = 256;

//var path = new MinecraftPath("게임 폴더 경로");
var path = new MinecraftPath(); // 기본 게임 경로 사용

var launcher = new CMLauncher(path);

// 콘솔에 실행 진행률 표시
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
    // 모든 버전 이름 표시
    // 여기서 출력되는 버전 이름을 CreateProcessAsync 메서드를 호출할 때 사용하면 됩니다.
    Console.WriteLine(item.Name);
}

var launchOption = new MLaunchOption
{
    MaximumRamMb = 1024,
    Session = MSession.GetOfflineSession("hello"), // Login Session. ex) Session = MSession.GetOfflineSession("hello")

    //ScreenWidth = 1600,
    //ScreenHeigth = 900,
    //ServerIp = "mc.hypixel.net"
};

//var process = await launcher.CreateProcessAsync("실행할 버전 이름을 여기에 입력하세요", launchOption);
var process = await launcher.CreateProcessAsync("1.15.2", launchOption); // 바닐라
// var process = await launcher.CreateProcessAsync("1.12.2-forge1.12.2-14.23.5.2838", launchOption); // 포지
// var process = await launcher.CreateProcessAsync("1.12.2-LiteLoader1.12.2"); // 라이트로더
// var process = await launcher.CreateProcessAsync("fabric-loader-0.11.3-1.16.5") // fabricMC

process.Start();
```

## 예제 코드

[Sample Code](https://github.com/AlphaBs/CmlLib.Core/wiki/Sample-Code)

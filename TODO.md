## next

- [ ] QuickPlay 프로필
- [ ] IDisposable 확인 https://stackoverflow.com/questions/4737056/detecting-leaked-idisposable-objects

## 4.0.0

- [x] MLaunch 통합 테스트 작성. 주요 버전 파싱, 최종 argument 확인
- [x] exception 타입 확인 
- [x] 다른 라이브러리와 호환성 확인하고 베타 버전 릴리즈하기
- [x] 문서 작성
- [x] Version 을 쉽게 바꿀 수 있는 무언가가 필요해
- [x] RulesContext 가 MinecraftLauncher 에서도 설정 가능하고 LaunchOption 에서도 설정 가능한데 이거 해결할 필요가 있음 -> LaunchOption 에서 Feature 만 유저가 설정가능하게 제한하기
- [x] MojangLauncher 와 호환성 확인: LibraryFileExtractorTests 에서 실제 url 에 파일 존재하는지, 실제 인스톨러는 어디다 설치하는지 확인 - runtime 내용이 다른거같음 - 고침
- [x] excludes 구현
- [x] json profile 파일 내용이 비어있음 - PipedStream 구현
- [x] features, MLaunchOption 테스트 아직 안됨
- [x] 여기까지 하고 4.0.0-beta.1 내놓기
- [x] Memory 위에서 mutable 한 IVersion 구현
- [x] LauncherTester 에서 자주 쓰는 버전들 미리추가 예를들면 1.12.2 1.14.4 1.16.5 1.20.4 그리고 3D shockwave 같은것들도
- [x] master 브랜치에서 v3.4.0 으로 cherry-pick
- [x] disableMultiplayer 같은 option 도 추가 -> ExtraGameArguments 로 추가해라
- [x] IUpdateTask 항상 실행해야 할듯 
- [x] -Dos.name= 이거 추가하기
- [x] DefaultJvmArguments 이거 필요할듯
- [x] 자동화된 e2e test runner 만들기
- [x] quickPlayServer, serverip, 같은 feature 자동설정
- [x] MojangVersionLoader v2 구현
- [x] MArgument 유닛테스트
- [x] 포지 옵티파인 iconic-mixed vexed
- [x] { "name": "name" } 이런식으로 name 만 있는것도 GameFile 로 추출해야하나? 이전버전 어떻게했나 확인해보고 수정 -> 추출했음
- [x] Path 가 비어있는 파일, Url 이 비어있는 파일은 큐잉하지 않고 스킵
- [x] JvmArgumentOverrides 에서 띄어쓰기 포함되어있으면 어떡하지? 예시: -Darg="hi -cp" -> CommandLineParser
- [x] 4.0.0 으로 올릴지 3.4.0 으로 올릴지 결정 -> 4.0.0 으로 올리고 레거시 쓸때없는거 다 지우고 바꾸고 가는게 좋을듯
- [x] GameInstaller 에서 IReadOnlyCollection 아니라 IEnumerable 받도록
- [x] MLaunchOption 에서 ExtraArguments 같은거 추가
- [x] RulesEvalutor 에서 ${arch} 와 os: 에서 쓰이는거 용도따라 바뀌게
- [x] GameInstaller 에서 중복 파일 확인
- [x] GameInstaller 에서 UpdateExcludeFiles 같은거 만들기
- [x] Extractors 최대한 유닛테스트
- [x] Extractor 통합 테스트 작성. 주요 버전 파싱, GameFile 확인
- [x] 바닐라 버전 우선적으로 완벽하게 처리하도록 테스트 케이스 작성
- [x] LegacyJava 버그: task의 file이 실제 java binary 을 나타내지 않음
- [x] introduce record type: AssetObject

# Flow

```
   { JsonVersionLoader }
             |
    JsonVersionMetadata
             |
        JsonVersion



        JsonVersion
             |
|----------------------------|
| IEnumerable<FileExtractor> |
|----------------------------|
             |
    IEnumerable<GameFile>
             |
|----------------------------|
|      IGameInstaller        |
|----------------------------|



        JsonVersion
             |
|-------------------------|
| INativeLibraryExtractor |
| MinecraftProcessBuilder |
|-------------------------|
             |
          Process
```
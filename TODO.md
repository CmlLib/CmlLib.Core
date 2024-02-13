- [] 테스트케이스 더 자세하게 작성
- [] MLaunchOption 에서 ExtraArguments 같은거 추가
- [] master 브랜치에서 v3.4.0 으로 cherry-pick
- [] RulesEvalutor 에서 ${arch} 와 os: 에서 쓰이는거 용도따라 바뀌게
- [] GameInstaller 에서 중복 파일 확인
- [] GameInstaller 에서 UpdateExcludeFiles 같은거 만들기
- [] 통합 테스트 작성. 모든 주요 버전 한번씩 실제로 실행해보는 테스트 프로그램 작성

# Flow

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

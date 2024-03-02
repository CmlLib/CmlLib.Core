- [x] MLaunchOption 에서 ExtraArguments 같은거 추가
- [ ] master 브랜치에서 v3.4.0 으로 cherry-pick
- [x] RulesEvalutor 에서 ${arch} 와 os: 에서 쓰이는거 용도따라 바뀌게
- [x] GameInstaller 에서 중복 파일 확인
- [x] GameInstaller 에서 UpdateExcludeFiles 같은거 만들기
- [ ] MArgument 유닛테스트
- [ ] Extractors 최대한 유닛테스트
- [ ] Extractor 통합 테스트 작성. 주요 버전 파싱, GameFile 확인
- [ ] MLaunch 통합 테스트 작성. 주요 버전 파싱, 최종 argument 확인
- [ ] 바닐라 버전 우선적으로 완벽하게 처리하도록 테스트 케이스 작성, 이후 포지 라이트로더 옵티파인 패브릭 tlauncher 커스텀클라이언트 등등 확장가능하게 테스트 케이스 작성
- [x] LegacyJava 버그: task의 file이 실제 java binary 을 나타내지 않음
- [ ] 4.0.0 으로 올릴지 3.4.0 으로 올릴지 결정
- [ ] Memory 위에서 mutable 한 IVersion 구현
- [ ] JvmArgumentOverrides 에서 띄어쓰기 포함되어있으면 어떡하지? 예시: 
-Darg="hi -cp"
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
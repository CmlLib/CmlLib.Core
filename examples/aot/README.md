# Native AOT smoke test

This app runs deterministic JSON scenarios shared with the test suite, with JSON
reflection disabled. It checks version manifests and metadata, Java manifests,
Fabric/Quilt loaders, LiteLoader profile generation, and version sorting. HTTP
responses are local fixtures; no Minecraft downloads or live services are needed.

The entire `CmlLib.Core` assembly is rooted during publishing so AOT/trimming
analysis also checks library methods outside the executed scenarios. Warnings
are treated as errors.

With .NET 8 and the [Native AOT build prerequisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
installed, run from the repository root:

```sh
dotnet publish examples/aot/CmlLibAotSample.csproj -c Release -r linux-x64
./examples/aot/bin/Release/net8.0/linux-x64/publish/CmlLibAotSample
```

Use the appropriate RID and executable path when building on another platform.
This smoke test does not verify actual game installation/launch or OS-specific
native calls on every supported platform.

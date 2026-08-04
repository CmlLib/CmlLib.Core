# CmlLib.Core
**Мови:** [English](README.md) | [Українська](docs/README-uk.md) | [简体中文](docs/README-chs.md)
## Бібліотека для лаунчера Minecraft

<img src='https://raw.githubusercontent.com/CmlLib/CmlLib.Core/master/icon.png' width=128>

[![Nuget Badge](https://img.shields.io/nuget/v/CmlLib.Core)](https://www.nuget.org/packages/CmlLib.Core)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/CmlLib/CmlLib.Core/blob/master/LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3f55a130ec3f4bccb55e7def97cfa2ce)](https://www.codacy.com/gh/CmlLib/CmlLib.Core/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=CmlLib/CmlLib.Core\&utm_campaign=Badge_Grade)

[![Discord](https://img.shields.io/discord/795952027443527690?label=discord\&logo=discord\&style=for-the-badge)](https://discord.gg/cDW2pvwHSc)

CmlLib.Core це бібліотека для створення лаунчерів Minecraft на .NET

Підтримує всі ванільні та модифіковані версії (включаючи Forge, Fabric тощо...)

[Документація корейською мовою](https://cmllib.github.io/CmlLib.Core-wiki/ko/)

[Реклама: Створимо лаунчер для вас!](https://cmllib.github.io/CmlLib.Core-wiki/ko/ad/)

## Можливості

* Авторизація через обліковий запис Microsoft Xbox
* Отримання списку ванільних та встановлених версій
* Встановлення ванільних версій
* Запуск будь-якої ванільної версії (протестовано до 1.21)
* Запуск Forge, Optifine, FabricMC, LiteLoader або будь-якої іншої кастомної версії
* Встановлення середовища виконання Java (JRE)
* Встановлення LiteLoader, FabricMC
* Запуск із додатковими параметрами (пряме підключення до сервера, роздільна здатність екрана, аргументи JVM)
* Кросплатформеність (Windows, Linux, macOS)

[Перейти до Wiki для перегляду всіх можливостей(англійська)](https://cmllib.github.io/CmlLib.Core-wiki/en/)

## Встановлення

Встановіть [NuGet-пакет CmlLib.Core](https://www.nuget.org/packages/CmlLib.Core)

## Швидкий початок

### Отримання всіх версій

```csharp
using CmlLib.Core;

var launcher = new MinecraftLauncher();
var versions = await launcher.GetAllVersionsAsync();
foreach (var version in versions)
{
    Console.WriteLine($"{version.Type} {version.Name}");
}

```

### Запуск гри

```csharp
using CmlLib.Core;
using CmlLib.Core.ProcessBuilder;

var launcher = new MinecraftLauncher();
var process = await launcher.InstallAndBuildProcessAsync("1.21", new MLaunchOption());
process.Start();

```

### Запуск гри з додатковими параметрами

```csharp
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

var path = new MinecraftPath("./my_game_dir");
var launcher = new MinecraftLauncher(path);
launcher.FileProgressChanged += (sender, args) =>
{
    Console.WriteLine($"Назва: {args.Name}");
    Console.WriteLine($"Тип: {args.EventType}");
    Console.WriteLine($"Всього: {args.TotalTasks}");
    Console.WriteLine($"Виконано: {args.ProgressedTasks}");
};
launcher.ByteProgressChanged += (sender, args) =>
{
    Console.WriteLine($"{args.ProgressedBytes} байт / {args.TotalBytes} байт");
};

await launcher.InstallAsync("1.20.4");
var process = await launcher.BuildProcessAsync("1.20.4", new MLaunchOption
{
    Session = MSession.CreateOfflineSession("Gamer123"),
    MaximumRamMb = 4096
});
process.Start();

```

## Документація

**[Документація англійською мовою](https://cmllib.github.io/CmlLib.Core-wiki/en/)**

**[Документація корейською мовою](https://cmllib.github.io/CmlLib.Core-wiki/ko/)**

## Приклад

[Приклад лаунчера](https://github.com/CmlLib/CmlLib-Minecraft-Launcher)

## Співрозробники
<a href="https://github.com/cmllib/cmllib.core/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=cmllib/cmllib.core" />
</a>

Створено за допомогою [contrib.rocks](https://contrib.rocks).

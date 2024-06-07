using CmlLib.Core.Internals;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Rules;

public class LauncherOSRule
{
    public const string Windows = "windows";
    public const string OSX = "osx";
    public const string Linux = "linux";

    public const string X86 = "32";
    public const string X64 = "64";
    public const string Arm = "arm";
    public const string Arm64 = "arm64";

    private static LauncherOSRule? _current;
    public static LauncherOSRule Current => _current ??= createCurrent();

    private static LauncherOSRule createCurrent()
    {
        string name, arch, version;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            name = OSX;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            name = Windows;
        else
            name = Linux;
        
        arch = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X86 => X86,
            Architecture.X64 => X64,
            Architecture.Arm => Arm,
            Architecture.Arm64 => Arm64,
            _ => ""
        };

        // Environment.OSVersion only returns the correct version on .NET 5.0 or later
        // https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/environment-osversion-returns-correct-version 

        var osVersion = Environment.OSVersion.Version;
        version = osVersion.ToString();

        // A common use case for LauncherOSRule is to check if the OS you are running is
        // Windows 10 or later. However, the .NET Framework does not properly recognize
        // Windows 10 or later versions and returns a 6.x version. To work around this,
        // we double-check the correct Windows version by calling the Windows Native API
        // directly when Environment.OSVersion returns an older Windows version.

        if (osVersion.Major < 10 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            version = NativeMethods.GetWindowsVersion(version);

        // For OSX, we didn't implement it because we don't need the exact OSX version.
        // 1. Most macOS users are on .NET 5.0 or later (are there still Mono or .NET Core users out there?)
        // 2. Very few use cases require checking the version of OSX
        // 3. Incorrect version does not cause any problems

        return new LauncherOSRule(name, arch, version);
    }

    public LauncherOSRule()
    {

    }

    public LauncherOSRule(string name, string arch, string version)
    {
        Name = name;
        Arch = arch;
        Version = version;
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arch")]
    public string? Arch { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
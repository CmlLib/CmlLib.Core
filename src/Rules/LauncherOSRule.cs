using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Rules;

public record LauncherOSRule
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
        string name, arch;

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

        return new LauncherOSRule(name, arch);
    }

    public LauncherOSRule()
    {
        
    }

    public LauncherOSRule(string name, string arch) =>
        (Name, Arch) = (name, arch);

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arch")]
    public string? Arch { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
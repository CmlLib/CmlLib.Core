using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Rules;

public record LauncherOSRule
{
    public const string Windows = "windows";
    public const string OSX = "osx";
    public const string Linux = "linux";

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
        
        arch = RuntimeInformation.OSArchitecture switch // TODO: find exact value
        {
            Architecture.X86 => "32",
            Architecture.X64 => "64",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm",
            _ => ""
        };

        return new LauncherOSRule(name, arch);
    }

    [JsonConstructor]
    public LauncherOSRule(string name, string arch) =>
        (Name, Arch) = (name, arch);

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("arch")]
    public string Arch { get; set; }

    public bool Match(LauncherOSRule toMatch)
    {
        return isPropMatch(Name, toMatch.Name) &&
               isPropMatch(Arch, toMatch.Arch);
    }

    private bool isPropMatch(string? rule, string? toMatch)
    {
        if (string.IsNullOrEmpty(rule))
            return true;
        return rule == toMatch;
    }
}
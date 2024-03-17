using CmlLib.Core.Auth;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.ProcessBuilder;

public class MLaunchOption
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> EmptyDictionary =
        new Lazy<IReadOnlyDictionary<string, string>>(() => new Dictionary<string, string>());

    public readonly static MArgument[] DefaultJvmArguments =
    [
        new MArgument 
        { 
            Values = 
            [
                "-XX:+UnlockExperimentalVMOptions",
                "-XX:+UseG1GC",
                "-XX:G1NewSizePercent=20",
                "-XX:G1ReservePercent=20",
                "-XX:MaxGCPauseMillis=50",
                "-XX:G1HeapRegionSize=16M",
                "-Dlog4j2.formatMsgNoLookups=true" 
            ]
        },
        new MArgument
        {
            Values = ["-XstartOnFirstThread"],
            Rules = 
            [
                new LauncherRule
                {
                    Action = "allow",
                    OS = new LauncherOSRule
                    {
                        Name = "osx"
                    }
                }
            ]
        },
        new MArgument
        {
            Values = ["-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump"],
            Rules = 
            [
                new LauncherRule
                {
                    Action = "allow",
                    OS = new LauncherOSRule
                    {
                        Name = "windows"
                    }
                }
            ]
        },
        new MArgument
        {
            Values = ["-Xss1M"],
            Rules = 
            [
                new LauncherRule
                {
                    Action = "allow",
                    OS = new LauncherOSRule
                    {
                        Arch = "x86"
                    }
                }
            ]
        }
    ];

    public MinecraftPath? Path { get; set; }
    public IVersion? StartVersion { get; set; }
    public MSession? Session { get; set; }
    public string? NativesDirectory { get; set; }
    public RulesEvaluatorContext? RulesContext { get; set; }
    public string PathSeparator { get; set; } = System.IO.Path.PathSeparator.ToString();

    public string? JavaVersion { get; set; }
    public string? JavaPath { get; set; }
    public int MaximumRamMb { get; set; } = 1024;
    public int MinimumRamMb { get; set; }

    public string? DockName { get; set; }
    public string? DockIcon { get; set; }

    public bool IsDemo { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public bool FullScreen { get; set; }
    public string? QuickPlayPath { get; set; }
    public string? QuickPlaySingleplayer { get; set; }
    public string? QuickPlayRealms { get; set; }

    // QuickPlayMultiplayer
    public string? ServerIp { get; set; }
    public int ServerPort { get; set; } = 25565;

    public string? ClientId { get; set; }
    public string? VersionType { get; set; }
    public string? GameLauncherName { get; set; } = "minecraft-launcher";
    public string? GameLauncherVersion { get; set; } = "2";
    public string? UserProperties { get; set; } = "{}";

    public IReadOnlyDictionary<string, string> ArgumentDictionary { get; set; } = EmptyDictionary.Value;
    public IEnumerable<MArgument>? JvmArgumentOverrides { get; set; }
    public IEnumerable<MArgument> ExtraJvmArguments { get; set; } = DefaultJvmArguments;
    public IEnumerable<MArgument> ExtraGameArguments { get; set; } = Enumerable.Empty<MArgument>();

    internal void CheckValid()
    {
        string? exMsg = null; // error message

        if (RulesContext == null)
            exMsg = "RulesContext is null";

        if (Path == null)
            exMsg = nameof(Path) + " is null";

        if (StartVersion == null)
            exMsg = "StartVersion is null";

        if (Session == null)
            Session = MSession.GetOfflineSession("tester123");

        if (!Session.CheckIsValid())
            exMsg = "Invalid Session";

        if (ServerPort < 0 || ServerPort > 65535)
            exMsg = "Invalid ServerPort";

        if (ScreenWidth < 0 || ScreenHeight < 0)
            exMsg = "Screen Size must be greater than or equal to zero.";

        if (exMsg != null) // if launch option is invalid, throw exception
            throw new ArgumentException(exMsg);
    }
}

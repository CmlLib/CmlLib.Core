using CmlLib.Core.Auth;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.ProcessBuilder;

public partial class MLaunchOption
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> EmptyDictionary =
        new Lazy<IReadOnlyDictionary<string, string>>(() => new Dictionary<string, string>());

    public MLaunchOption()
    {
        if (LauncherOSRule.Current.Arch == LauncherOSRule.X64)
            MaximumRamMb = 2048;
        else
            MaximumRamMb = 1024;
    }

    // required parameters
    public MinecraftPath? Path { get; set; }
    public IVersion? StartVersion { get; set; }
    public string? NativesDirectory { get; set; }
    public string? JavaPath { get; set; }
    public string PathSeparator { get; set; } = System.IO.Path.PathSeparator.ToString();

    // optional parameters
    public MSession? Session { get; set; }
    public IEnumerable<string> Features { get; set; } = [];

    public int MaximumRamMb { get; set; }
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
    public IEnumerable<MArgument> ExtraJvmArguments { get; set; } = DefaultExtraJvmArguments;
    public IEnumerable<MArgument> ExtraGameArguments { get; set; } = Enumerable.Empty<MArgument>();

    internal void CheckValid()
    {
        if (string.IsNullOrEmpty(JavaPath))
            throw new ArgumentNullException(nameof(JavaPath));

        if (Path == null)
            throw new ArgumentNullException(nameof(Path));

        if (StartVersion == null)
            throw new ArgumentNullException(nameof(StartVersion));

        if (Session == null)
            Session = MSession.CreateOfflineSession("tester123");

        if (!Session.CheckIsValid())
            throw new ArgumentException("Invalid session");

        if (ServerPort < 0 || ServerPort > 65535)
            throw new ArgumentOutOfRangeException(nameof(ServerPort), ServerPort, "Valid range of ServerPort is 0 ~ 65535.");

        if (ScreenWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(ScreenWidth), ScreenWidth, "Cannot be a negative value.");

        if (ScreenHeight < 0)
            throw new ArgumentOutOfRangeException(nameof(ScreenHeight), ScreenHeight, "Cannot be a negative value.");

        if (MaximumRamMb < 0)
            throw new ArgumentOutOfRangeException(nameof(MaximumRamMb), MaximumRamMb, "Cannot be a negative value.");

        if (MinimumRamMb < 0)
            throw new ArgumentOutOfRangeException(nameof(MinimumRamMb), MinimumRamMb, "Cannot be a negative value.");
        
        if (MinimumRamMb > MaximumRamMb)
            throw new ArgumentOutOfRangeException(nameof(MinimumRamMb), MinimumRamMb, "MinimumRamMb cannot be greater than MaximumRamMb.");
    }
}

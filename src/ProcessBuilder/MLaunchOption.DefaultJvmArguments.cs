using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public partial class MLaunchOption
{
    public readonly static MArgument[] DefaultJvmArguments =
    [
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
        },
        new MArgument
        {
            Values =
            [
                "-Dos.name=Windows 10",
                "-Dos.version=10.0"
            ],
            Rules =
            [
                new LauncherRule
                {
                    Action = "allow",
                    OS = new LauncherOSRule
                    {
                        Name = "windows",
                        Version = "^10\\."
                    }
                }
            ]
        }
    ];

    public readonly static MArgument[] DefaultExtraJvmArguments =
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
    ];
}
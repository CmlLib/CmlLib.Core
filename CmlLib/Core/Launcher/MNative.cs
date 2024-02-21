using System.IO;
using CmlLib.Core.Version;
using CmlLib.Utils;
using MethodTimer;

namespace CmlLib.Core;

public class MNative
{
    private readonly MinecraftPath gamePath;

    private readonly MVersion version;

    public MNative(MinecraftPath gamePath, MVersion version)
    {
        this.version = version;
        this.gamePath = gamePath;
    }

    [Time]
    public string ExtractNatives()
    {
        var path = gamePath.GetNativePath(version.Id);
        Directory.CreateDirectory(path);

        if (version.Libraries == null) return path;

        foreach (var item in version.Libraries)
            // do not ignore exception
            if (item.IsRequire && item.IsNative && !string.IsNullOrEmpty(item.Path))
            {
                var zPath = Path.Combine(gamePath.Library, item.Path);
                if (File.Exists(zPath))
                {
                    var z = new SharpZip(zPath);
                    z.Unzip(path);
                }
            }

        return path;
    }

    public void CleanNatives()
    {
        try
        {
            var path = gamePath.GetNativePath(version.Id);
            var di = new DirectoryInfo(path);

            if (!di.Exists)
                return;

            foreach (var item in di.GetFiles()) item.Delete();
        }
        catch
        {
            // ignore exception
            // will be overwriten to new file
        }
    }
}

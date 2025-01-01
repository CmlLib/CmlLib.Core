using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Version;

public static class Extensions
{
    public static MVersionType GetVersionType(this IVersion version)
    {
        if (string.IsNullOrEmpty(version.Type))
            return MVersionType.Custom;
        else
            return MVersionTypeConverter.Parse(version.Type);
    }

    public static T? GetInheritedProperty<T>(this IVersion self, Func<IVersion, T> prop)
    {
        foreach (var version in self.EnumerateToParent())
        {
            var value = prop.Invoke(version);
            if (value is string valueStr && !string.IsNullOrEmpty(valueStr))
                return value;
            else if (value != null)
                return value;
        }
        return default;
    }

    public static IEnumerable<T> ConcatInheritedCollection<T>(
        this IVersion self, 
        Func<IVersion, IEnumerable<T>> prop,
        int maxDepth = 10)
    {
        foreach (var version in self.EnumerateFromParent(maxDepth))
        {
            foreach (var item in prop(version))
            {
                yield return item;
            }
        }
    }
    
    public static IEnumerable<MArgument> ConcatInheritedGameArguments(this IVersion self, int maxDepth = 10)
    {
        foreach (var version in self.EnumerateFromParent(maxDepth))
        {
            foreach (var item in version.GetGameArguments(version != self))
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<MArgument> ConcatInheritedJvmArguments(this IVersion self, int maxDepth = 10)
    {
        foreach (var version in self.EnumerateFromParent(maxDepth))
        {
            foreach (var item in version.GetJvmArguments(version != self))
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<IVersion> EnumerateToParent(this IVersion version)
    {
        IVersion? current = version;
        while (current != null)
        {
            yield return current;
            current = current.ParentVersion;
        }
    }

    public static IEnumerable<IVersion> EnumerateFromParent(this IVersion version, int maxDepth)
    {
        var stack = new Stack<IVersion>();

        IVersion? v = version;
        while (v != null)
        {
            if (stack.Count >= maxDepth)
                throw VersionDependencyException.CreateExcessiveDepthMessage(maxDepth, v.Id);

            stack.Push(v);
            v = v.ParentVersion;
        }

        while (stack.Any())
        {
            yield return stack.Pop();
        }
    }

    public static MinecraftVersion ToMutableVersion(this IVersion self)
    {
        var version = new MinecraftVersion(self.Id)
        {
            MainJarId = self.MainJarId,
            InheritsFrom = self.InheritsFrom,
            ParentVersion = self.ParentVersion,
            AssetIndex = self.AssetIndex,
            Client = self.Client,
            JavaVersion = self.JavaVersion,
            Jar = self.Jar,
            Logging = self.Logging,
            MainClass = self.MainClass,
            ReleaseTime = self.ReleaseTime,
            Type = self.Type,
        };

        version.LibraryList.AddRange(self.Libraries);
        version.GameArguments.AddRange(self.GetGameArguments(false));
        version.GameArgumentsForBaseVersion.AddRange(self.GetGameArguments(true));
        version.JvmArguments.AddRange(self.GetJvmArguments(false));
        version.JvmArgumentsForBaseVersion.AddRange(self.GetJvmArguments(true));
        return version;
    }
}
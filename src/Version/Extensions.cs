using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Version;

public static class Extensions
{
    public static MVersionType GetVersionType(this IVersion version)
    {
        return MVersionTypeConverter.FromString(version.Type);
    }

    public static T? GetInheritedProperty<T>(this IVersion self, Func<IVersion, T> prop)
    {
        IVersion? version = self;
        while (version != null)
        {
            var value = prop.Invoke(version);
            if (value is string valueStr && !string.IsNullOrEmpty(valueStr))
                return value;
            else if (value != null)
                return value;
            version = version.ParentVersion;
        }
        return default;
    }

    public static IEnumerable<T> ConcatInheritedCollection<T>(
        this IVersion self, 
        Func<IVersion, IEnumerable<T>> prop,
        int maxDepth = 10)
    {
        foreach (var version in enumerateFromParent(self, maxDepth))
        {
            foreach (var item in prop(version))
            {
                yield return item;
            }
        }
    }

    private static IEnumerable<IVersion> enumerateFromParent(IVersion version, int maxDepth)
    {
        var stack = new Stack<IVersion>();

        IVersion? v = version;
        while (v != null)
        {
            if (stack.Count >= maxDepth)
                throw new Exception();

            stack.Push(v);
            v = v.ParentVersion;
        }

        while (stack.Any())
        {
            yield return stack.Pop();
        }
    }
}